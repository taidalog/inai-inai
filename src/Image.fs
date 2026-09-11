(*
   Copyright 2026 taidalog

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*)

namespace InaiInai

open System
open System.IO
open System.Drawing
open FaceONNX
open OpenCvSharp
open OpenCvSharp.GdipExtensions
open Utility

module Image =
    let getImageOrientationProperty (bitmap: Bitmap) : Imaging.PropertyItem option =
        let orientationPropertyId = 0x0112

        if Array.contains orientationPropertyId bitmap.PropertyIdList then
            Some(bitmap.GetPropertyItem orientationPropertyId)
        else
            None

    let ksizef (width: int) (height: int) : int =
        min width height / 14 |> toOddNumber |> max 1

    let maskf (ksize: int) (size: Size) : Mat =
        let w, h = size.Width, size.Height

        let mask: Mat = new Mat(size, MatType.CV_32FC1, Scalar.All 0.0)
        let center = Point(w / 2, h / 2)

        let axes =
            let w' = w / 2 - ksize |> max 1
            let h' = h / 2 - ksize |> max 1
            Size(w', h')

        Cv2.Ellipse(mask, center, axes, 0.0, 0.0, 360.0, Scalar.All 255.0, -1)

        Cv2.GaussianBlur(mask, mask, Size(ksize, ksize), 0.0)
        Cv2.Multiply(mask, Scalar.All(1.0 / 255.0), mask)

        mask

    let gaussianBlurCircularEdge (mask: Mat) (img: Mat) (ksize: int) : Mat =
        // Ensure we work on a 3-channel BGR image. PNGs may have alpha (4 channels),
        // which causes channel-count mismatches when multiplying with a 3-channel mask.
        let hasAlpha = img.Channels() = 4

        use src: Mat =
            if hasAlpha then
                let tmp: Mat = new Mat()
                Cv2.CvtColor(img, tmp, ColorConversionCodes.BGRA2BGR)
                tmp
            else
                let tmp: Mat = new Mat()
                img.CopyTo tmp
                tmp

        let size: Size = src.Size()

        use mask3Ch: Mat = new Mat()
        Cv2.Merge(ReadOnlySpan<Mat> [| mask; mask; mask |], mask3Ch)

        use blurredImg: Mat = new Mat()
        Cv2.GaussianBlur(src, blurredImg, Size(ksize, ksize), 0.0)

        use imgFloat: Mat = new Mat()
        use blurFloat: Mat = new Mat()
        src.ConvertTo(imgFloat, MatType.CV_32FC3)
        blurredImg.ConvertTo(blurFloat, MatType.CV_32FC3)

        use invMask: Mat = new Mat(size, MatType.CV_32FC3, Scalar.All 1.0)
        Cv2.Subtract(invMask, mask3Ch, invMask)

        use centerPart: Mat = new Mat()
        Cv2.Multiply(blurFloat, mask3Ch, centerPart)

        use edgePart: Mat = new Mat()
        Cv2.Multiply(imgFloat, invMask, edgePart)

        use outputFloat: Mat = new Mat()
        Cv2.Add(centerPart, edgePart, outputFloat)

        use output3Ch: Mat = new Mat()
        outputFloat.ConvertTo(output3Ch, MatType.CV_8UC3)

        // Restore alpha channel if input had it (for PNG compatibility)
        if hasAlpha then
            let output4Ch = new Mat()
            Cv2.CvtColor(output3Ch, output4Ch, ColorConversionCodes.BGR2BGRA)
            output4Ch
        else
            let output = new Mat()
            output3Ch.CopyTo output
            output

    let blurFaces
        (faceDetector: FaceDetector)
        (verbose: bool)
        (outputDirectory: DirectoryInfo)
        (fileInfo: FileInfo)
        : Result<FaceBlurResult, exn> =
        try
            printfn "%s" (Resources.Strings.``Detecting faces in:\t{0}`` fileInfo)

            if fileInfo.Exists = false then
                let e =
                    new FileNotFoundException($"%s{fileInfo.FullName} is not found.", fileInfo.Name)

                printfn "Error:\t\t\t%s\n" e.Message
                Error e
            else
                let t0 = DateTime.Now

                use bitmap: Bitmap = new Bitmap(fileInfo.FullName)
                let orientation: Imaging.PropertyItem option = getImageOrientationProperty bitmap

                let faces: FaceDetectionResult array = faceDetector.Forward bitmap

                let detectingSeconds = (DateTime.Now - t0).TotalSeconds

                printfn
                    "%s"
                    (Resources.Strings.``Detected face(s):\t{0} face(s), {1} seconds``
                        (Array.length faces)
                        (DateTime.Now - t0).TotalSeconds)

                let t1 = DateTime.Now

                use mat: Mat = bitmap.ToMat()
                let matRect: Rectangle = Rectangle(0, 0, mat.Width, mat.Height)

                let w, h = mat.Width, mat.Height
                let l = fileInfo.Length

                printfn "%s" (Resources.Strings.``Image dimensions:\t{0} x {1} pixels`` mat.Width mat.Height)
                printfn "%s" (Resources.Strings.``Image size:\t\t{0} MB`` $"{float fileInfo.Length / 1024. / 1024.:F2}")

                faces
                |> Array.iter (fun (x: FaceDetectionResult) ->
                    let rect: System.Drawing.Rectangle = x.Rectangle

                    if verbose then
                        printfn "%s" (Resources.Strings.``Face rectangle:\t\t{0}`` rect)

                    let k: int = ksizef rect.Width rect.Height
                    let rectInflated: Rectangle = Rectangle.Inflate(rect, k, k)
                    use mask: Mat = maskf k (Size(rectInflated.Width, rectInflated.Height))

                    let rectClamped: Rectangle =
                        let top = Math.Clamp(rectInflated.Top, matRect.Top, matRect.Bottom)
                        let bottom = Math.Clamp(rectInflated.Bottom, matRect.Top, matRect.Bottom)
                        let left = Math.Clamp(rectInflated.Left, matRect.Left, matRect.Right)
                        let right = Math.Clamp(rectInflated.Right, matRect.Left, matRect.Right)
                        Rectangle(left, top, right - left, bottom - top)

                    let rectForMat: Rectangle =
                        let x = rectClamped.Left - rectInflated.Left
                        let y = rectClamped.Top - rectInflated.Top
                        let width = rectClamped.Width
                        let height = rectClamped.Height
                        Rectangle(x, y, width, height)

                    use faceMask: Mat =
                        mask.Item(rectForMat.Top, rectForMat.Bottom, rectForMat.Left, rectForMat.Right)

                    use facialArea: Mat =
                        mat.Item(rectClamped.Top, rectClamped.Bottom, rectClamped.Left, rectClamped.Right)

                    use facialAreaBlurred: Mat =
                        gaussianBlurCircularEdge faceMask facialArea (k * 10 + 1)

                    mat.Item(rectClamped.Top, rectClamped.Bottom, rectClamped.Left, rectClamped.Right) <-
                        facialAreaBlurred

                // Cv2.ImShow("Result", mat)
                // Cv2.WaitKey 0 |> ignore
                // Cv2.DestroyAllWindows()
                )

                let blurringSeconds = (DateTime.Now - t1).TotalSeconds
                printfn "%s" (Resources.Strings.``Masking time:\t\t{0} seconds`` (DateTime.Now - t1).TotalSeconds)

                let t2 = DateTime.Now

                if not outputDirectory.Exists then
                    outputDirectory.Create()

                use dstBitmap: Bitmap = new Bitmap(fileInfo.FullName)
                mat.ToBitmap dstBitmap
                orientation |> Option.iter (fun x -> dstBitmap.SetPropertyItem x)

                let outputPath = Path.uniqueFileName outputDirectory fileInfo

                match outputPath with
                | Error(e, _, _) ->
                    printfn "%s" (Resources.Strings.``Couldn't save image:\t{0}\n`` fileInfo.FullName)
                    Error e
                | Ok(outputPath: string) ->
                    dstBitmap.Save outputPath
                    // Cv2.ImWrite(outputPath, mat) |> ignore

                    printfn
                        "%s"
                        (Resources.Strings.``Saved image:\t\t{0}, {1} seconds\n``
                            outputPath
                            (DateTime.Now - t2).TotalSeconds)

                    let resultPath = outputPath
                    let savingSeconds = (DateTime.Now - t2).TotalSeconds

                    let res: FaceBlurResult =
                        { Path = fileInfo.FullName
                          Faces = faces
                          DetectingSeconds = detectingSeconds
                          Width = w
                          Height = h
                          Length = l
                          BlurringSeconds = blurringSeconds
                          ResultPath = resultPath
                          SavingSeconds = savingSeconds }

                    Ok res
        with _ as e ->
            printfn "Error:\t\t\t%s\n" e.Message
            Error e

    let blurFacesAsync
        (faceDetector: FaceDetector)
        (verbose: bool)
        (outputDirectory: DirectoryInfo)
        (fileInfos: FileInfo array)
        : Async<Result<FaceBlurResult, exn> array> =

        let blurFaces' = blurFaces faceDetector verbose outputDirectory

        async {
            let results: Result<FaceBlurResult, exn> array = fileInfos |> Array.map blurFaces'
            return results
        }
