# inai-inai

A privacy tool that blurs faces in images.

The name "inai-inai" comes from "inai inai baa!", the Japanese version of peek-a-boo.

Written in F#.

[Japanese README](README.ja.md)

## Features

- Detects faces in images and blurs them.
- Works locally, doesn't upload your images.
- Adjusts the blur strength for each face based on its pixel width and height.

## Requirements

- Windows (x64)
- .NET 8 or later
- git (just for `git clone`)

## Building

Clone the repository:

```
git clone git@github.com:taidalog/inai-inai.git
cd inai-inai
```

... or you can just download ZIP from https://github.com/taidalog/inai-inai

Then build the project:

```
dotnet build
```

## Publishing

(Assuming the repository is already cloned and the current directory is `inai-inai`)

Package the project with the following options:

```
dotnet publish
```

The resulting EXE file can be moved anywhere on your computer.

## Usage

### Usage 1

1. Create an `input` directory in the current directory.
1. Place image files in the `input` directory.
1. Run the application DLL using the command below. The path to the DLL depends on your current directory:

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll
   ```

Output images are saved in the `output` directory in the current directory.

### Usage 2

1. Run the application DLL using the command below. The path to the DLL and image files depend on your current directory. Multiple files can be accepted:

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll your\image.jpg another\input\image.jpg
   ```

Output images are saved in the `output` directory in the current directory.

### Usage 3

1. Create an `input` directory in the current directory.
1. Place image files in the `input` directory.
1. Run the published executable using the command below. The path to the EXE depends on your current directory:

   ```
   .\inai-inai.exe
   ```

Output images are saved in the `output` directory in the current directory.

### Usage 4

1. Run the published executable using the command below. The path to the EXE and image files depend on your current directory. Multiple files can be accepted:

   ```
   .\inai-inai.exe your\image.jpg another\input\image.jpg
   ```

Output images are saved in the `output` directory in the current directory.

### Usage 5

1. Create an `input` directory next to `inai-inai.exe`.
1. Place image files in the `input` directory.
1. Run the published executable by double-clicking `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Usage 6

1. Drag and drop image files onto the published executable.

Output images are saved in the `output` directory next to `inai-inai.exe`.

## Options

| Option                                      | Description                  |
| ------------------------------------------- | ---------------------------- |
| `-i\|--input-directory <INPUT_DIRECTORY>`   | Specify an input directory.  |
| `-o\|--output-directory <OUTPUT_DIRECTORY>` | Specify an output directory. |
| `-vb\|--Verbose`                            | Enable verbose logging.      |
| `-v\|--Version`                             | Display version.             |

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
- OpenCvSharp5.GdipExtensions
- OpenCvSharp5.runtime.win
- OpenCvSharp5.Windows

**MIT License:**

- Argu
- FaceONNX
- FSharp.Core
- Microsoft.ML.OnnxRuntime.Managed
- Microsoft.Win32.SystemEvents
- System.CodeDom
- System.Configuration.ConfigurationManager
- System.Drawing.Common
- System.Management
- System.Security.Cryptography.ProtectedData
- UMapx

For a complete list of dependencies and their licenses, see [licenses.md](./licenses.md) and [NOTICE](./NOTICE).

## Copyright

Copyright 2026 taidalog
