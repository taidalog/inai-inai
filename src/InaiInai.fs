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
open Path

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

            let pid: int =
                use p = Process.GetCurrentProcess()
                p.Id

            match Process.getParentPid pid with
            | None ->
                printfn "Error: Parent PID not found."
                2
            | Some ppid ->
                let paths: string list = results.GetResult(Paths, defaultValue = [])

                let verbose = results.Contains Verbose

                let isDnD = Utility.isDragAndDropped ppid (Array.length args)

                let workingDirectory =
                    if isDnD then
                        AppContext.BaseDirectory
                    else
                        Environment.CurrentDirectory

                let outputDirectoryInfo =
                    let outputDirectory: string =
                        results.GetResult(Output_Directory, defaultValue = "output")

                    Path.GetFullPath(outputDirectory, workingDirectory) |> DirectoryInfo

                let inputDirectoryInfo =
                    let inputDirectory: string =
                        results.GetResult(Input_Directory, defaultValue = "input")

                    Path.GetFullPath(inputDirectory, workingDirectory) |> DirectoryInfo

                if not inputDirectoryInfo.Exists then
                    printfn $"Error: The directory %s{inputDirectoryInfo.FullName} does not exist."
                    printfn $"Create %s{inputDirectoryInfo.FullName}, add image files, and run the program again."
                    printfn "Press any key to exit..."
                    Console.ReadKey() |> ignore
                    1
                else
                    let files: string array =
                        if List.length paths > 0 then
                            paths |> List.toArray
                        else
                            Directory.GetFiles(inputDirectoryInfo.FullName, "*.*")

                    printfn $"Processing {Array.length files} image(s)...\n"

                    use faceDetector: FaceDetector = new FaceDetector()

                    let fileInfos: Result<FileInfo, (exn * string * string)> array =
                        files
                        |> Array.map (fun x -> Path.GetFullPath(x, workingDirectory))
                        |> Array.filter isSupportedFileFormat
                        |> Array.filter Path.Exists
                        |> Array.map tryFileInfo

                    let processed =
                        fileInfos
                        |> Array.map (Result.bind (blurFaces faceDetector verbose outputDirectoryInfo))

                    processed
                    |> Array.iter (fun (x: Result<(string * float), (exn * string * string)>) ->
                        match x with
                        | Ok _ -> ()
                        | Error(e, msg, filename) -> printfn "Error:\t\t\t%s\n%s\n" filename msg)

                    if Array.length processed > 0 then
                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore
                        0
                    else
                        printfn $"No image files were found."
                        printfn $"Place image files in %s{inputDirectoryInfo.FullName} and run the program again."
                        printfn "Press any key to exit..."
                        Console.ReadKey() |> ignore
                        0
