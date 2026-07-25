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
