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

module Main =
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

    let blurFaces (faceDetector: FaceDetector) (outputDirectoryPath: string) (file: string) : unit =
        printfn $"Detecting faces in:\t%s{file}"

        let t0 = DateTime.Now

        let faces: FaceDetectionResult array =
            use bitmap: Bitmap = new Bitmap(file)
            let res: FaceDetectionResult array = faceDetector.Forward bitmap
            res

        printfn $"Detected face(s):\t{Array.length faces} face(s), %f{(DateTime.Now - t0).TotalSeconds} seconds"

        let t1 = DateTime.Now

        let bytes: byte array = File.ReadAllBytes file
        use mat: Mat = Cv2.ImDecode(bytes, ImreadModes.Color)
        printfn "Image dimensions:\t%d x %d pixels" mat.Width mat.Height

        let fileinfo = FileInfo file
        printfn $"Image size:\t\t{float fileinfo.Length / 1024. / 1024.:F2} MB"

        // Cv2.ImShow("Original Image", mat)
        // Cv2.WaitKey 0 |> ignore
        // Cv2.DestroyAllWindows()

        faces
        |> Array.iter (fun (x: FaceDetectionResult) ->
            let rect = x.Rectangle
            // printfn "Face rectangle:\t\t%A" rect

            use mask: Mat = new Mat(rect.Height, rect.Width, MatType.CV_8UC3, Scalar.Black)
            let center = new Point(rect.Width / 2, rect.Height / 2)
            let axes = new Size(rect.Width / 2, rect.Height / 2)
            // printfn "Mask center:\t\t%A" center
            // printfn "Mask axes:\t\t%A" axes

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
            // printfn "ksize:\t\t\t%A" ksize

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

        let outputPath = uniqueFileName outputDirectory.FullName fileinfo.Name
        Cv2.ImWrite(outputPath, mat) |> ignore

        printfn $"Saved image:\t\t%s{outputPath}, %f{(DateTime.Now - t2).TotalSeconds} seconds\n"

    [<EntryPoint>]
    let main (args: string array) : int =
        printfn "inai-inai version 0.2.0\n"

        let pid: int =
            use p = Process.GetCurrentProcess()
            p.Id

        match Process.getParentPid pid with
        | None ->
            printfn "Error: Parent PID not found."
            2
        | Some ppid ->
            let isdd = Utility.isDnD ppid (Array.length args)

            let workingDirectory =
                if isdd then
                    AppContext.BaseDirectory
                else
                    Environment.CurrentDirectory

            printfn "AppContext.BaseDirectory:\t%s" AppContext.BaseDirectory
            printfn "Environment.CurrentDirectory:\t%s" Environment.CurrentDirectory
            printfn "workingDirectory:\t\t%s" workingDirectory

            let outputDirectory = Path.Join [| workingDirectory; "output" |] |> DirectoryInfo

            if Array.length args = 0 then

                let inputDirectory = Path.Join [| workingDirectory; @"input" |] |> DirectoryInfo

                if not inputDirectory.Exists then
                    printfn $"Error: The directory %s{inputDirectory.FullName} does not exist."
                    printfn $"Create %s{inputDirectory.FullName}, add image files, and run the program again."
                    printfn "Press any key to exit..."
                    Console.ReadKey() |> ignore
                    1

                else
                    let files = Directory.GetFiles(inputDirectory.FullName, "*.*")

                    if Array.length files = 0 then
                        printfn $"No image files were found."
                        printfn $"Place image files in %s{inputDirectory.FullName} and run the program again."
                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore
                        0
                    else
                        printfn $"Processing {Array.length files} image(s)...\n"

                        use faceDetector: FaceDetector = new FaceDetector()

                        files
                        |> Array.iter (fun (filePath: string) ->
                            blurFaces faceDetector outputDirectory.FullName filePath)

                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore

                        0
            else
                printfn $"Processing {Array.length args} image(s)...\n"

                use faceDetector: FaceDetector = new FaceDetector()

                args
                |> Array.iter (fun (filePath: string) -> blurFaces faceDetector outputDirectory.FullName filePath)

                printfn "Press any key to exit..."
                Console.ReadKey() |> ignore

                0
