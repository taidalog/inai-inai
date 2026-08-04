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
open System.Diagnostics
open System.Reflection
open FaceONNX
open Argu
open Utility
open Image

module Main =
    [<EntryPoint>]
    let main (args: string array) : int =
        let errorHandler =
            ProcessExiter(
                colorizer =
                    function
                    | ErrorCode.HelpText -> None
                    | _ -> Some ConsoleColor.Red
            )

        let parser: ArgumentParser<Arguments> =
            ArgumentParser.Create<Arguments>(programName = "inai-inai", errorHandler = errorHandler)

        let results: ParseResults<Arguments> = parser.Parse args

        let versionString =
            Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion

        let version = results.Contains Arguments.Version

        if version then
            printfn "%s" versionString
            0
        else
            printfn "inai-inai version %s\n" versionString

            let pathsArgument: string list = results.GetResult(Paths, defaultValue = [])

            let inputDirectoryArgument =
                results.GetResult(Input_Directory, defaultValue = "input")

            let outputDirectoryArgument =
                results.GetResult(Output_Directory, defaultValue = "output")

            let verbose = results.Contains Verbose

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

                let outputDirectory =
                    Path.Join [| workingDirectory; outputDirectoryArgument |] |> DirectoryInfo

                if List.length pathsArgument = 0 then

                    let inputDirectory =
                        Path.Join [| workingDirectory; inputDirectoryArgument |] |> DirectoryInfo

                    if not inputDirectory.Exists then
                        printfn $"Error: The directory %s{inputDirectory.FullName} does not exist."
                        printfn $"Create %s{inputDirectory.FullName}, add image files, and run the program again."
                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore
                        1

                    else
                        let files =
                            Directory.GetFiles(inputDirectory.FullName, "*.*")
                            |> Array.filter isSupportedFileFormat

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
                                blurFaces faceDetector outputDirectory.FullName filePath verbose)

                            printfn "Press any key to exit..."
                            Console.ReadKey() |> ignore

                            0
                else
                    let args' = pathsArgument |> List.filter isSupportedFileFormat

                    if List.length args' = 0 then
                        printfn $"No image files were found."
                        printfn $"Supprted file formats are BMP, GIF, EXIF, JPG, PNG and TIFF."
                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore
                        0
                    else
                        printfn $"Processing {List.length args'} image(s)...\n"

                        use faceDetector: FaceDetector = new FaceDetector()

                        args'
                        |> List.iter (fun (filePath: string) ->
                            blurFaces faceDetector outputDirectory.FullName filePath verbose)

                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore

                        0
