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
    let tryFileInfo (filename: string) : Result<FileInfo, exn * string * string> =
        try
            FileInfo filename |> Ok
        with
        | :? ArgumentNullException as e -> Error(e, "File name is null.", filename)
        | :? Security.SecurityException as e -> Error(e, "Having no permission to open the file.", filename)
        | :? ArgumentException as e -> Error(e, "File was not found.", filename)
        | :? UnauthorizedAccessException as e -> Error(e, "Access to the file is denied.", filename)
        | :? PathTooLongException as e -> Error(e, "File name is too long.", filename)
        | :? NotSupportedException as e -> Error(e, "File name contains a colon (:).", filename)
        | _ as e -> Error(e, "Unexpected error.", filename)
