# inai-inai

A privacy tool that blurs faces in images.

The name "inai-inai" comes from "inai inai baa!", the Japanese version of peek-a-boo.

Written in F#.

## Features

- Detects faces in images and blurs them.
- Works locally, doesn't upload your images.

## Requirements

- Windows (x64)
- .NET 8 or later

## Building

Clone the repository:

```
git clone git@github.com:taidalog/inai-inai.git
cd inai-inai
```

Then build the project:

```
dotnet build
```

## Publishing

(Assuming the repository is already cloned and the current directory is `inai-inai`)

Package the project with the following options:

```
dotnet publish -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded
```

The options above enable you to move the EXE file anywhere on your computer.

## Usage

There are four ways to run the application.

### Usage 1

1. Create an `input` directory in the current directory.
1. Place image files in the `input` directory.
1. Run the application DLL using the command below. The path to the DLL depends on your current directory:

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll
   ```

Output images are saved in the `output` directory in the current directory.

### Usage 2

1. Create an `input` directory next to `inai-inai.exe`.
1. Place image files in the `input` directory.
1. Run the application by double-clicking `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Usage 3

1. Drag and drop image files onto `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Usage 4

1. Create an `input` directory in the current directory.
1. Place image files in the `input` directory.
1. Run the published executable using the command below. The path to the EXE depends on your current directory:

   ```
   .\inai-inai.exe
   ```

Output images are saved in the `output` directory in the current directory.

## Known Issues

- None known.

## Release Notes

[Releases on GitHub](https://github.com/taidalog/inai-inai/releases)

## License

This application is licensed under [Apache License Version 2.0](https://github.com/taidalog/inai-inai/blob/main/LICENSE).

### Third-party License Compliance

This product includes the following third-party libraries:

**Apache License 2.0:**

- OpenCvSharp5

**MIT License:**

- FaceONNX
- FSharp.Core
- Microsoft.ML.OnnxRuntime.Managed
- UMapx
- Microsoft.Win32.SystemEvents
- System.Drawing.Common

For a complete list of dependencies and their licenses, see [licenses.md](./licenses.md) and [NOTICE](./NOTICE).

## Copyright

Copyright 2026 taidalog
