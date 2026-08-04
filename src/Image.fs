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
    let detectFaces (faceDetector: FaceDetector) (filename: string) : FaceDetectionResult array =
        use bitmap: Bitmap = new Bitmap(filename)
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
        let axes = Size(w / 2 - edgeBlurKsize, h / 2 - edgeBlurKsize)
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

    let blurFaces (faceDetector: FaceDetector) (outputDirectoryPath: string) (file: string) (verbose: bool) : unit =
        printfn $"Detecting faces in:\t%s{file}"

        let t0 = DateTime.Now

        use bitmap: Bitmap = new Bitmap(file)
        let orientation: Imaging.PropertyItem option = getImageOrientationProperty bitmap

        let faces: FaceDetectionResult array = faceDetector.Forward bitmap
        printfn $"Detected face(s):\t{Array.length faces} face(s), %f{(DateTime.Now - t0).TotalSeconds} seconds"

        let t1 = DateTime.Now

        use mat: Mat = bitmap.ToMat()
        printfn "Image dimensions:\t%d x %d pixels" mat.Width mat.Height

        let matRect: Rectangle = Rectangle(0, 0, mat.Width, mat.Height)

        let fileinfo = FileInfo file
        printfn $"Image size:\t\t{float fileinfo.Length / 1024. / 1024.:F2} MB"

        // Cv2.ImShow("Original Image", mat)
        // Cv2.WaitKey 0 |> ignore
        // Cv2.DestroyAllWindows()

        faces
        |> Array.filter (fun (x: FaceDetectionResult) -> matRect.Contains x.Rectangle)
        |> Array.iter (fun (x: FaceDetectionResult) ->
            let rect: System.Drawing.Rectangle = x.Rectangle

            if verbose then
                printfn "Face rectangle:\t\t%A" rect

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

        printfn $"Masking time:\t\t%f{(DateTime.Now - t1).TotalSeconds} seconds"

        let t2 = DateTime.Now

        let outputDirectory = DirectoryInfo outputDirectoryPath

        if not outputDirectory.Exists then
            outputDirectory.Create()

        use dstBitmap: Bitmap = new Bitmap(file)
        mat.ToBitmap dstBitmap

        orientation |> Option.iter (fun x -> dstBitmap.SetPropertyItem x)

        let outputPath = uniqueFileName outputDirectory.FullName fileinfo.Name
        dstBitmap.Save outputPath
        // Cv2.ImWrite(outputPath, mat) |> ignore

        printfn $"Saved image:\t\t%s{outputPath}, %f{(DateTime.Now - t2).TotalSeconds} seconds\n"
