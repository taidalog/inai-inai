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

open Xunit
open InaiInai.Image

module Image =
    [<Theory>]
    [<InlineData(1920, 1080, 77)>]
    [<InlineData(100, 60, 5)>]
    [<InlineData(1, 1, 1)>]
    [<InlineData(1, 0, 1)>]
    let ``ksizef should return an odd number greater than 0.`` (width: int, height: int, expected: int) =
        let actual: int = ksizef width height
        Assert.Equal(expected, actual)
