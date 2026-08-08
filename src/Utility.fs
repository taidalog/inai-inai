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
        use parentProcces = Process.GetProcessById ppid
        let filename = (FileInfo parentProcces.MainModule.FileName).Name
        List.contains (filename.ToLowerInvariant()) [ "explorer.exe" ] && argsCount > 0

    let uniqueFileName (directoryPath: string) (path: string) : string =
        let directory = DirectoryInfo directoryPath
        let fileBaseName = Path.GetFileNameWithoutExtension path
        let fileExtension = Path.GetExtension path

        let rec loop (dir: string) (bas: string) (ext: string) (n: int) : string =
            let duplicationCount = if n = 0 then "" else $" (%d{n})"

            let candidatePath =
                Path.Join [| directory.FullName; $"%s{bas}%s{duplicationCount}%s{ext}" |]

            if Path.Exists candidatePath |> not then
                candidatePath
            else
                loop dir bas ext (n + 1)

        loop directoryPath fileBaseName fileExtension 0

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
