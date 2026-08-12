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

open System.IO
open System.Diagnostics

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
                    let filename = (FileInfo mainModule.FileName).Name
                    List.contains (filename.ToLowerInvariant()) [ "explorer.exe" ]
            with _ ->
                false

    let uniqueFileName (directoryInfo: DirectoryInfo) (fileInfo: FileInfo) : string =
        let rec loop (dirPath: string) (baseName: string) (extension: string) (n: int) : string =
            let duplicationCount = if n = 0 then "" else $" (%d{n})"

            let candidatePath =
                Path.GetFullPath($"%s{baseName}%s{duplicationCount}%s{extension}", dirPath)

            if Path.Exists candidatePath |> not then
                candidatePath
            else
                loop dirPath baseName extension (n + 1)

        let fileBaseName = Path.GetFileNameWithoutExtension fileInfo.Name
        let fileExtension = fileInfo.Extension

        loop directoryInfo.FullName fileBaseName fileExtension 0

    let isSupportedFileFormat (path: string) : bool =
        let extension: string = Path.GetExtension path

        List.contains
            (extension.ToUpper())
            [ ".BMP"; ".GIF"; ".EXIF"; ".JPG"; ".JPEG"; ".JPE"; ".PNG"; ".TIFF"; ".TIF" ]

    let toOddNumber (n: int) : int = n + if n % 2 = 0 then 1 else 0

    let smallestGap (outerRect: System.Drawing.Rectangle) (innerRect: System.Drawing.Rectangle) : int =
        List.min
            [ abs (outerRect.Top - innerRect.Top)
              abs (outerRect.Bottom - innerRect.Bottom)
              abs (outerRect.Left - innerRect.Left)
              abs (outerRect.Right - innerRect.Right) ]
