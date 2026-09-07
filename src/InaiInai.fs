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
// open System.Diagnostics
// open System.Reflection
// open System.Text
// open FaceONNX
// open Argu
// open Utility
// open Image
// open Path
open Avalonia
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Media
open Avalonia.Styling
open Elmish

module InaiInai =
    //     [<EntryPoint>]
    //     let main (args: string array) : int =
    //         Console.OutputEncoding <- Encoding.UTF8

    //         let errorHandler =
    //             ProcessExiter(
    //                 colorizer =
    //                     function
    //                     | ErrorCode.HelpText -> None
    //                     | _ -> Some ConsoleColor.Red
    //             )

    //         let parser: ArgumentParser<Arguments> =
    //             ArgumentParser.Create<Arguments>(programName = "inai-inai", errorHandler = errorHandler)

    //         let results: ParseResults<Arguments> = parser.Parse args

    //         let versionString =
    //             Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion

    //         let version = results.Contains Arguments.Version

    //         if version then
    //             printfn "%s" versionString
    //             0
    //         else
    //             printfn "inai-inai version %s\n" versionString

    //             let pid: int =
    //                 use p = Process.GetCurrentProcess()
    //                 p.Id

    //             match Process.getParentPid pid with
    //             | None ->
    //                 printfn "%s" Resources.Strings.``Error: Parent PID not found.``
    //                 2
    //             | Some ppid ->
    //                 let paths: string list = results.GetResult(Paths, defaultValue = [])

    //                 let verbose = results.Contains Verbose

    //                 let isDnD = Utility.isDragAndDropped ppid (Array.length args)

    //                 let workingDirectory =
    //                     if isDnD then
    //                         AppContext.BaseDirectory
    //                     else
    //                         Environment.CurrentDirectory

    //                 let outputDirectoryInfo =
    //                     let outputDirectory: string =
    //                         results.GetResult(Output_Directory, defaultValue = "output")

    //                     // Path.GetFullPath(outputDirectory, workingDirectory) |> DirectoryInfo
    //                     Path.getFullPath workingDirectory outputDirectory |> Result.map DirectoryInfo

    //                 let inputDirectoryInfo =
    //                     let inputDirectory: string =
    //                         results.GetResult(Input_Directory, defaultValue = "input")

    //                     // Path.GetFullPath(inputDirectory, workingDirectory) |> DirectoryInfo
    //                     Path.getFullPath workingDirectory inputDirectory |> Result.map DirectoryInfo

    //                 match outputDirectoryInfo, inputDirectoryInfo with
    //                 | Error(e, msg, filename), _ ->
    //                     printfn "Error:\t\t\t%s\n%s\n" filename msg
    //                     1
    //                 | _, Error(e, msg, filename) ->
    //                     printfn "Error:\t\t\t%s\n%s\n" filename msg
    //                     1
    //                 | Ok outputDirectoryInfo, Ok inputDirectoryInfo ->
    //                     if not isDnD && List.length paths = 0 && not inputDirectoryInfo.Exists then
    //                         printfn "%s" (Resources.Strings.``Error: The directory {0} does not exist.`` inputDirectoryInfo)

    //                         printfn
    //                             "%s"
    //                             (Resources.Strings.``Create {0}, add image files, and run the program again.``
    //                                 inputDirectoryInfo)

    //                         printfn "%s" Resources.Strings.``Press any key to exit...``
    //                         Console.ReadKey() |> ignore
    //                         1
    //                     else
    //                         let files: string array =
    //                             if List.length paths > 0 then
    //                                 paths |> List.toArray
    //                             else
    //                                 Directory.GetFiles(inputDirectoryInfo.FullName, "*.*")

    //                         if Array.length files = 0 && not inputDirectoryInfo.Exists then
    //                             printfn
    //                                 "%s"
    //                                 (Resources.Strings.``Error: The directory {0} does not exist.`` inputDirectoryInfo)

    //                             printfn
    //                                 "%s"
    //                                 (Resources.Strings.``Create {0}, add image files, and run the program again.``
    //                                     inputDirectoryInfo)

    //                             printfn "%s" Resources.Strings.``Press any key to exit...``
    //                             Console.ReadKey() |> ignore
    //                             1
    //                         else

    //                             printfn "%s" (Resources.Strings.``Processing {0} image(s)...\n`` (Array.length files))

    //                             use faceDetector: FaceDetector = new FaceDetector()

    //                             let fileInfos: Result<FileInfo, (exn * string * string)> array =
    //                                 files
    //                                 |> Array.map (Path.getFullPath workingDirectory)
    //                                 |> Array.filter (fun x ->
    //                                     match x with
    //                                     | Ok v -> isSupportedFileFormat v
    //                                     | Error _ -> false)
    //                                 |> Array.filter (fun x ->
    //                                     match x with
    //                                     | Ok v -> Path.Exists v
    //                                     | Error _ -> false)
    //                                 |> Array.map (Result.bind tryFileInfo)

    //                             let processed =
    //                                 fileInfos
    //                                 |> Array.map (Result.bind (blurFaces faceDetector verbose outputDirectoryInfo))

    //                             processed
    //                             |> Array.iter (fun (x: Result<(string * float), (exn * string * string)>) ->
    //                                 match x with
    //                                 | Ok _ -> ()
    //                                 | Error(e, msg, filename) -> printfn "Error:\t\t\t%s\n%s\n" filename msg)

    //                             if Array.length processed > 0 then
    //                                 printfn "%s" Resources.Strings.``Press any key to exit...``
    //                                 Console.ReadKey() |> ignore
    //                                 0
    //                             else
    //                                 printfn "%s" Resources.Strings.``No image files were found.``

    //                                 printfn
    //                                     "%s"
    //                                     (Resources.Strings.``Place image files in {0} and run the program again.``
    //                                         inputDirectoryInfo)

    //                                 printfn "%s" Resources.Strings.``Press any key to exit...``
    //                                 Console.ReadKey() |> ignore
    //                                 0

    type State =
        { paths: string array
          newpaths: string array
          isOverDragZone: bool }

    [<RequireQualifiedAccess>]
    module State =
        let empty: State =
            { paths = Array.empty
              newpaths = Array.empty
              isOverDragZone = false }

        let init () : State * Cmd<'a> = empty, Cmd.none

    type Msg =
        | DragOver
        | DragEnter
        | DragLeave
        | Drop of string array
        | Completed of string array

    let newName (destinationPathName: string) (path: string) : string =
        let fi = FileInfo path
        Path.Join [| fi.DirectoryName; destinationPathName; fi.Name |]

    let copyFile (path: string) (newPath: string) : unit =
        let fi = FileInfo newPath

        if not (Directory.Exists fi.DirectoryName) then
            Directory.CreateDirectory fi.DirectoryName |> ignore

        File.Copy(path, newPath)

    let copyFilesAsync (paths: string array) : Async<string array> =
        async {
            let newPaths = paths |> Array.map (newName "output")
            (paths, newPaths) ||> Array.iter2 copyFile
            return newPaths
        }

    let update (msg: Msg) (state: State) : State * Cmd<Msg> =
        match msg with
        | DragOver -> { state with isOverDragZone = true }, Cmd.none
        | DragEnter -> { state with isOverDragZone = true }, Cmd.none
        | DragLeave -> { state with isOverDragZone = false }, Cmd.none
        | Drop(paths: string array) ->
            let cmd: Cmd<Msg> = Cmd.OfAsync.perform copyFilesAsync paths Msg.Completed
            { state with paths = paths }, cmd
        | Completed(newpaths: string array) ->
            { state with
                newpaths = newpaths
                isOverDragZone = false },
            Cmd.none

    let getPaths (e: DragEventArgs) : string array =
        use d: IDataTransfer = e.DataTransfer

        if d.Contains DataFormat.File then
            let files: Avalonia.Platform.Storage.IStorageItem array = d.TryGetFiles()

            if files <> null then
                files |> Seq.map (fun x -> x.Path.LocalPath) |> Seq.toArray
            else
                Array.empty
        else
            Array.empty

    let isDarkMode: bool =
        match Application.Current with
        | null -> false
        | app -> app.ActualThemeVariant = ThemeVariant.Dark

    let backgroundColor (isOverDragZone: bool) (isDarkMode: bool) : IBrush =
        match isOverDragZone, isDarkMode with
        | true, true -> SolidColorBrush(Color.FromArgb(90uy, 101uy, 162uy, 172uy))
        | true, false -> SolidColorBrush(Color.FromRgb(193uy, 223uy, 227uy))
        | false, _ -> Brushes.Transparent

    let droppedText (s: State) : string =
        let pathCount = Array.length s.paths

        if pathCount = 0 then
            "Drop files here"
        else
            Array.concat [ [| $"%d{pathCount} file(s) are dropped" |]; Array.sort s.paths ]
            |> String.concat Environment.NewLine

    let view (state: State) (dispatch: Msg -> unit) : Avalonia.FuncUI.Types.IView<DockPanel> =

        DockPanel.create
            [ DockPanel.children
                  [ Border.create
                        [ Border.dock Dock.Top
                          Border.padding 16.0
                          Border.margin 8.0

                          // Border.background is necessary for DragDrop.
                          Border.background (backgroundColor state.isOverDragZone isDarkMode)

                          // Border.border is NOT necessary for DragDrop.
                          Border.borderBrush (SolidColorBrush Colors.Gray)
                          Border.borderThickness 1.0
                          Border.cornerRadius 8.0
                          Border.minHeight 180.0
                          Border.isHitTestVisible true

                          Control.allowDrop true

                          Control.onDragOver (fun (e: DragEventArgs) ->
                              if e.DataTransfer.Contains DataFormat.File then
                                  e.DragEffects <- DragDropEffects.Copy
                              else
                                  e.DragEffects <- DragDropEffects.None

                              Msg.DragOver |> dispatch
                              e.Handled <- true)

                          Control.onDragEnter (fun (e: DragEventArgs) ->
                              Msg.DragEnter |> dispatch
                              e.Handled <- true)

                          Control.onDragLeave (fun (e: DragEventArgs) ->
                              Msg.DragLeave |> dispatch
                              e.Handled <- true)

                          Control.onDrop (fun (e: DragEventArgs) ->
                              getPaths e |> Msg.Drop |> dispatch
                              e.Handled <- true)

                          Border.child (
                              TextBlock.create
                                  [ TextBlock.textWrapping TextWrapping.Wrap
                                    if Array.length state.paths > 0 then
                                        TextBlock.verticalAlignment VerticalAlignment.Top
                                        TextBlock.horizontalAlignment HorizontalAlignment.Left
                                    else
                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                                    TextBlock.text (droppedText state) ]
                          ) ] ] ]
