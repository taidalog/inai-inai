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
open System.Diagnostics
open FaceONNX
open OpenCvSharp
open OpenCvSharp.GdipExtensions

module Utility =
    let isDnD (ppid: int) (argsCount: int) : bool =
        use pp = Process.GetProcessById ppid
        let filename = (FileInfo pp.MainModule.FileName).Name
        List.contains (filename.ToLowerInvariant()) [ "explorer.exe" ] && argsCount > 0

    let uniqueFileName (directoryPath: string) (fileName: string) : string =
        let directory = DirectoryInfo directoryPath
        let fileBaseName = Path.GetFileNameWithoutExtension fileName
        let fileExtension = Path.GetExtension fileName

        let rec loop (dir: string) (bas: string) (ext: string) (n: int) : string =
            let duplicationCount = if n = 0 then "" else $" (%d{n})"

            let candidatePath =
                Path.Join [| directory.FullName; $"%s{bas}%s{duplicationCount}%s{ext}" |]

            if Path.Exists candidatePath |> not then
                candidatePath
            else
                loop dir bas ext (n + 1)

        loop directoryPath fileBaseName fileExtension 0

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

    let isSupportedFileFormat (x: string) : bool =
        let extensionName: string = Path.GetExtension x

        List.contains
            (extensionName.ToUpper())
            [ ".BMP"; ".GIF"; ".EXIF"; ".JPG"; ".JPEG"; ".JPE"; ".PNG"; ".TIFF"; ".TIF" ]

    let toOddNumber (n: int) : int = n + if n % 2 = 0 then 1 else 0

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
        |> Array.filter (fun (x: FaceDetectionResult) -> isFaceWithinBitmap bitmap.Width bitmap.Height x)
        |> Array.iter (fun (x: FaceDetectionResult) ->
            let rect = x.Rectangle

            if verbose then
                printfn "Face rectangle:\t\t%A" rect

            // use mask: Mat = new Mat(rect.Height, rect.Width, MatType.CV_8UC3, Scalar.Black)
            // Caused an exception with `Cv2.CopyTo`, so replaced `MatType.CV_8UC3` with `MatType.CV_8U`.
            use mask: Mat = new Mat(rect.Height, rect.Width, MatType.CV_8U, Scalar.Black)
            let center = new Point(rect.Width / 2, rect.Height / 2)
            let axes = new Size(rect.Width / 2, rect.Height / 2)

            if verbose then
                printfn "Mask center:\t\t%A" center
                printfn "Mask axes:\t\t%A" axes

            Cv2.Ellipse(
                img = mask,
                center = center,
                axes = axes,
                angle = double 0.,
                startAngle = double 0.,
                endAngle = double 360.,
                color = Scalar.White,
                thickness = -1
            )
            |> ignore

            // Cv2.ImShow("Mask", mask)
            // Cv2.WaitKey 0 |> ignore
            // Cv2.DestroyAllWindows()

            use facialArea = mat.Item(rect.Y, rect.Y + rect.Height, rect.X, rect.X + rect.Width)
            // Cv2.ImShow("Facial Area", facialArea)
            // Cv2.WaitKey 0 |> ignore
            // Cv2.DestroyAllWindows()

            let ksize: Size = new Size(max 1 (mat.Width / 40), max 1 (mat.Height / 40))

            if verbose then
                printfn "ksize:\t\t\t%A" ksize

            use facialAreaBlurred = new Mat()

            Cv2.Blur(facialArea, facialAreaBlurred, ksize)
            // Cv2.ImShow("Facial Area", facialAreaBlurred)
            // Cv2.WaitKey 0 |> ignore
            // Cv2.DestroyAllWindows()

            Cv2.CopyTo(facialAreaBlurred, facialArea, mask)

            mat.Item(rect.Y, rect.Y + rect.Height, rect.X, rect.X + rect.Width) <- facialArea
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
