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

open Avalonia
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Media
open Avalonia.Styling
open Elmish

module Utility =
    let isDragAndDropped (ppid: int) (argsCount: int) : bool =
        if argsCount <= 0 then
            false
        else
            try
                use parentProcess: Process = Process.GetProcessById ppid

                match parentProcess.MainModule with
                | null -> false
                | (mainModule: ProcessModule) ->
                    let filename: string = Path.GetFileName mainModule.FileName
                    StringComparer.OrdinalIgnoreCase.Equals(filename, "explorer.exe")
            with _ ->
                false

    let isSupportedFileFormat (path: string) : bool =
        if String.IsNullOrEmpty path then
            false
        else
            let extension: string = Path.GetExtension path

            List.contains
                (extension.ToUpper())
                [ ".BMP"; ".GIF"; ".EXIF"; ".JPG"; ".JPEG"; ".JPE"; ".PNG"; ".TIFF"; ".TIF" ]

    let toOddNumber (n: int) : int = n + if n % 2 = 0 then 1 else 0

    let isDarkMode: bool =
        match Application.Current with
        | null -> false
        | app -> app.ActualThemeVariant = ThemeVariant.Dark

    let backgroundColor (isOverDragZone: bool) (isDarkMode: bool) : IBrush =
        match isOverDragZone, isDarkMode with
        | true, true -> SolidColorBrush(Color.FromArgb(90uy, 101uy, 162uy, 172uy))
        | true, false -> SolidColorBrush(Color.FromRgb(193uy, 223uy, 227uy))
        | false, _ -> Brushes.Transparent

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
