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

module InaiInai.Tests

open System.IO
open Xunit
open InaiInai.Path
open InaiInai.Utility

[<Fact>]
let ``Supposing "no-such-file.na" does't exist in "output"`` () =
    let expected: Result<string, (exn * string * string)> =
        let fileInfo = FileInfo @"output\no-such-file.na"
        Ok fileInfo.FullName

    let actual: Result<string, (exn * string * string)> =
        uniqueFileName (DirectoryInfo "output") (FileInfo @"no-such-file.na")

    Assert.Equal(expected, actual)

[<Fact>]
let ``Supposing "existing-file-name.jpg" exists in "output"`` () =
    let expected: Result<string, (exn * string * string)> =
        let fileInfo = FileInfo @"output\existing-file-name (1).jpg"
        Ok fileInfo.FullName

    let actual: Result<string, (exn * string * string)> =
        uniqueFileName (DirectoryInfo "output") (FileInfo @"existing-file-name.jpg")

    Assert.Equal(expected, actual)

[<Fact>]
let ``Supposing "one-and-two-existing.jpg", "one-and-two-existing (1).jpg" and "one-and-two-existing (2).jpg" exist in "output"``
    ()
    =
    let expected: Result<string, (exn * string * string)> =
        let fileInfo = FileInfo @"output\one-and-two-existing (3).jpg"
        Ok fileInfo.FullName

    let actual: Result<string, (exn * string * string)> =
        uniqueFileName (DirectoryInfo "output") (FileInfo @"one-and-two-existing.jpg")

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData("image.bmp", true)>]
[<InlineData("image.gif", true)>]
[<InlineData("image.exif", true)>]
[<InlineData("image.jpg", true)>]
[<InlineData("image.jpeg", true)>]
[<InlineData("image.jpe", true)>]
[<InlineData("image.png", true)>]
[<InlineData("image.tiff", true)>]
[<InlineData("image.tif", true)>]
[<InlineData("image.txt", false)>]
[<InlineData("", false)>]
let ``isSupportedFileFormat should return false for empty string`` (path: string, expected: bool) =
    let actual: bool = isSupportedFileFormat path
    Assert.Equal(expected, actual)
