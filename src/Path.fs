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

module Path =
    let tryFileInfo (fileName: string) : Result<FileInfo, exn * string * string> =
        try
            FileInfo fileName |> Ok
        with
        | :? ArgumentNullException as e -> Error(e, "fileName is null.", fileName)
        | :? Security.SecurityException as e -> Error(e, "Not having permission to open the file.", fileName)
        | :? ArgumentException as e -> Error(e, "fileName is invalid or the file is not found.", fileName)
        | :? UnauthorizedAccessException as e -> Error(e, "Access to the file is denied.", fileName)
        | :? PathTooLongException as e -> Error(e, "fileName is too long.", fileName)
        | :? NotSupportedException as e -> Error(e, "fileName contains a colon (:).", fileName)
        | _ as e -> Error(e, "Unexpected error.", fileName)

    let getFullPath (basePath: string) (path: string) : Result<string, exn * string * string> =
        if String.IsNullOrEmpty path then
            Error(ArgumentNullException "path", "", path)
        else if String.IsNullOrEmpty basePath then
            Error(ArgumentNullException "basePath", "", basePath)
        else
            try
                Ok(Path.GetFullPath(path, basePath))
            with e ->
                Error(e, "Unexpected error.", path)

    let uniqueFileName (directoryInfo: DirectoryInfo) (fileInfo: FileInfo) : Result<string, exn * string * string> =

        let rec loop
            (dirPath: string)
            (baseName: string)
            (extension: string)
            (n: int)
            : Result<string, exn * string * string> =

            let candidatePath: Result<string, (exn * string * string)> =
                let duplicationCount = if n = 0 then "" else $" (%d{n})"
                getFullPath dirPath $"%s{baseName}%s{duplicationCount}%s{extension}"

            match candidatePath with
            | Error(x: exn * string * string) -> Error x
            | Ok(v: string) ->
                if File.Exists v |> not then
                    Ok v
                else
                    loop dirPath baseName extension (n + 1)

        let fileBaseName = Path.GetFileNameWithoutExtension fileInfo.Name
        let fileExtension = fileInfo.Extension

        loop directoryInfo.FullName fileBaseName fileExtension 0

    let workingDirectory: string = AppContext.BaseDirectory
