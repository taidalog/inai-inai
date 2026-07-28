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
open InaiInai.Main

[<Fact>]
let ``uniqueFileName 1`` () =
    let expected =
        let fileInfo = FileInfo @"output\no-such-file.na"
        fileInfo.FullName

    let actual = uniqueFileName "output" @"no-such-file.na"
    Assert.Equal(expected, actual)

[<Fact>]
let ``uniqueFileName 2`` () =
    let expected =
        let fileInfo = FileInfo @"output\existing-file-name (1).jpg"
        fileInfo.FullName

    let actual = uniqueFileName "output" @"existing-file-name.jpg"
    Assert.Equal(expected, actual)

[<Fact>]
let ``uniqueFileName 3`` () =
    let expected =
        let fileInfo = FileInfo @"output\one-and-two-existing (3).jpg"
        fileInfo.FullName

    let actual = uniqueFileName "output" @"one-and-two-existing.jpg"
    Assert.Equal(expected, actual)
