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

namespace InaiInai.Tests

open System
open Xunit
open InaiInai.Utility

module Utility =
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

    [<Theory>]
    [<InlineData(0, 1)>]
    [<InlineData(1, 1)>]
    [<InlineData(2, 3)>]
    [<InlineData(3, 3)>]
    [<InlineData(Int32.MaxValue, Int32.MaxValue)>]
    [<InlineData(-1, -1)>]
    [<InlineData(-2, -1)>]
    [<InlineData(-3, -3)>]
    [<InlineData(Int32.MinValue, Int32.MinValue + 1)>]
    let ``toOddNumber should return the minimum odd number that is greater or equal to the input number.``
        (n: int, expected: int)
        =
        let actual: int = toOddNumber n
        Assert.Equal(expected, actual)
