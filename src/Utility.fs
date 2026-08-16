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
