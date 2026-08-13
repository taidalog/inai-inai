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
    let detectFaces (faceDetector: FaceDetector) (fileInfo: FileInfo) : FaceDetectionResult array =
        use bitmap: Bitmap = new Bitmap(fileInfo.FullName)
        let faces: FaceDetectionResult array = faceDetector.Forward bitmap
        faces

    let getImageOrientationProperty (bitmap: Bitmap) : Imaging.PropertyItem option =
        let orientationPropertyId = 0x0112

        if Array.contains orientationPropertyId bitmap.PropertyIdList then
            Some(bitmap.GetPropertyItem orientationPropertyId)
        else
            None

    let gaussianBlurCircularEdge (img: Mat) (edgeBlurKsize: int) (faceBlurKsize: int) : Mat =
        // Ensure we work on a 3-channel BGR image. PNGs may have alpha (4 channels),
        // which causes channel-count mismatches when multiplying with a 3-channel mask.
        let hasAlpha = img.Channels() = 4

        use src =
            if hasAlpha then
                let tmp = new Mat()
                Cv2.CvtColor(img, tmp, ColorConversionCodes.BGRA2BGR)
                tmp
            else
                let tmp = new Mat()
                img.CopyTo tmp
                tmp

        let size = src.Size()
        let w, h = size.Width, size.Height

        use mask: Mat = new Mat(size, MatType.CV_32FC1, Scalar.All 0.0)
        let center = Point(w / 2, h / 2)

        let axes =
            let w' = w / 2 - edgeBlurKsize |> max 1
            let h' = h / 2 - edgeBlurKsize |> max 1
            Size(w', h')

        Cv2.Ellipse(mask, center, axes, 0.0, 0.0, 360.0, Scalar.All 255.0, -1)

        Cv2.GaussianBlur(mask, mask, Size(edgeBlurKsize, edgeBlurKsize), 0.0)
        Cv2.Multiply(mask, Scalar.All(1.0 / 255.0), mask)

        use mask3Ch: Mat = new Mat()
        Cv2.Merge(ReadOnlySpan<Mat> [| mask; mask; mask |], mask3Ch)

        use blurredImg: Mat = new Mat()
        Cv2.GaussianBlur(src, blurredImg, Size(faceBlurKsize, faceBlurKsize), 0.0)

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
        : Result<string * float, exn * string * string> =
        try
            printfn "%s" (Resources.Strings.``Detecting faces in:\t{0}`` fileInfo)

            if fileInfo.Exists = false then
                let e = new FileNotFoundException()
                printfn "Error:\t\t\t%s\n" e.Message
                Error(e, $"%s{fileInfo.FullName} is not found.", fileInfo.FullName)
            else
                let t0 = DateTime.Now

                use bitmap: Bitmap = new Bitmap(fileInfo.FullName)
                let orientation: Imaging.PropertyItem option = getImageOrientationProperty bitmap

                let faces: FaceDetectionResult array = faceDetector.Forward bitmap

                printfn
                    "%s"
                    (Resources.Strings.``Detected face(s):\t{0} face(s), {1} seconds``
                        (Array.length faces)
                        (DateTime.Now - t0).TotalSeconds)

                let t1 = DateTime.Now

                use mat: Mat = bitmap.ToMat()
                let matRect: Rectangle = Rectangle(0, 0, mat.Width, mat.Height)

                // Cv2.ImShow("Original Image", mat)
                // Cv2.WaitKey 0 |> ignore
                // Cv2.DestroyAllWindows()

                let facesToBlur =
                    faces
                    |> Array.filter (fun (x: FaceDetectionResult) -> matRect.Contains x.Rectangle)

                printfn
                    "%s"
                    (Resources.Strings.``Skipped face(s):\t{0} face(s)`` (Array.length faces - Array.length facesToBlur))

                printfn "%s" (Resources.Strings.``Image dimensions:\t{0} x {1} pixels`` mat.Width mat.Height)
                printfn "%s" (Resources.Strings.``Image size:\t\t{0} MB`` $"{float fileInfo.Length / 1024. / 1024.:F2}")

                facesToBlur
                |> Array.iter (fun (x: FaceDetectionResult) ->
                    let rect: System.Drawing.Rectangle = x.Rectangle

                    if verbose then
                        printfn "%s" (Resources.Strings.``Face rectangle:\t\t{0}`` rect)

                    let k = min rect.Width rect.Height / 14 |> toOddNumber |> max 1

                    let inflateAmount: int =
                        let smallest = smallestGap matRect rect |> max 0
                        if smallest > k then k else smallest

                    let rect': System.Drawing.Rectangle =
                        Rectangle.Inflate(rect, inflateAmount, inflateAmount)

                    use facialArea: Mat = mat.Item(rect'.Top, rect'.Bottom, rect'.Left, rect'.Right)
                    use facialAreaBlurred: Mat = gaussianBlurCircularEdge facialArea k (k * 10 + 1)
                    mat.Item(rect'.Top, rect'.Bottom, rect'.Left, rect'.Right) <- facialAreaBlurred

                // Cv2.ImShow("Result", mat)
                // Cv2.WaitKey 0 |> ignore
                // Cv2.DestroyAllWindows()
                )

                printfn "%s" (Resources.Strings.``Masking time:\t\t{0} seconds`` (DateTime.Now - t1).TotalSeconds)

                let t2 = DateTime.Now

                if not outputDirectory.Exists then
                    outputDirectory.Create()

                use dstBitmap: Bitmap = new Bitmap(fileInfo.FullName)
                mat.ToBitmap dstBitmap
                orientation |> Option.iter (fun x -> dstBitmap.SetPropertyItem x)

                let outputPath = uniqueFileName outputDirectory fileInfo
                dstBitmap.Save outputPath
                // Cv2.ImWrite(outputPath, mat) |> ignore

                printfn
                    "%s"
                    (Resources.Strings.``Saved image:\t\t{0}, {1} seconds\n``
                        outputPath
                        (DateTime.Now - t2).TotalSeconds)

                Ok(outputPath, (DateTime.Now - t2).TotalSeconds)
        with
        | :? FileNotFoundException as e ->
            printfn "Error:\t\t\t%s\n" e.Message
            Error(e, $"%s{e.FileName} is not found.", fileInfo.FullName)
        | _ as e ->
            printfn "Error:\t\t\t%s\n" e.Message
            Error(e, "Unexpected error.", fileInfo.FullName)
