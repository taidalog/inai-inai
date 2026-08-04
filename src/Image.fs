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

    let isFaceWithinBitmap (width: int) (height: int) (x: FaceDetectionResult) : bool =
        let rect = x.Rectangle

        rect.X >= 0
        && rect.Y >= 0
        && rect.X + rect.Width <= width
        && rect.Y + rect.Height <= height

    let gaussianBlurCircularEdge (img: Mat) (edgeBlurKsize: int) (faceBlurKsize: int) : Mat =
        let size = img.Size()
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
        Cv2.GaussianBlur(img, blurredImg, Size(faceBlurKsize, faceBlurKsize), 0.0)

        use imgFloat: Mat = new Mat()
        use blurFloat: Mat = new Mat()
        img.ConvertTo(imgFloat, MatType.CV_32FC3)
        blurredImg.ConvertTo(blurFloat, MatType.CV_32FC3)

        use invMask: Mat = new Mat(size, MatType.CV_32FC3, Scalar.All 1.0)
        Cv2.Subtract(invMask, mask3Ch, invMask)

        use centerPart: Mat = new Mat()
        Cv2.Multiply(blurFloat, mask3Ch, centerPart)

        use edgePart: Mat = new Mat()
        Cv2.Multiply(imgFloat, invMask, edgePart)

        use outputFloat: Mat = new Mat()
        Cv2.Add(centerPart, edgePart, outputFloat)

        let output: Mat = new Mat()
        outputFloat.ConvertTo(output, MatType.CV_8UC3)

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

        let fileinfo = FileInfo file
        printfn $"Image size:\t\t{float fileinfo.Length / 1024. / 1024.:F2} MB"

        // Cv2.ImShow("Original Image", mat)
        // Cv2.WaitKey 0 |> ignore
        // Cv2.DestroyAllWindows()

        faces
        |> Array.filter (isFaceWithinBitmap bitmap.Width bitmap.Height)
        |> Array.iter (fun (x: FaceDetectionResult) ->
            let rect: System.Drawing.Rectangle = x.Rectangle

            if verbose then
                printfn "Face rectangle:\t\t%A" rect

            let k: int = min rect.Width rect.Height / 14 |> toOddNumber |> max 1

            let rect': System.Drawing.Rectangle = Rectangle.Inflate(rect, k, k)
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
