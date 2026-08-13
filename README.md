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

- Windows (64 bits)
- .NET 10 (for `dotnet publish`)
- git (for `git clone`)

## Publishing the application

1. Clone the repository:

   ```
   git clone git@github.com:taidalog/inai-inai.git
   cd inai-inai
   ```

   ... or download ZIP from https://github.com/taidalog/inai-inai

1. Publish the application:

   ```
   dotnet publish
   ```

The published executable file (`inai-inai.exe`) will be output to the repository root. The EXE file can be moved anywhere on your computer.

## Usage

### Example 1

1. Create an `input` directory next to `inai-inai.exe`.
1. Place image files in the `input` directory.
1. Run `inai-inai.exe` by double-clicking it.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Example 2

1. Drag and drop image files onto `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Example 3

1. Create an `input` directory in the current directory.
1. Place image files in the `input` directory.
1. Run `inai-inai.exe` using the command below. The path to the EXE depends on your current directory:

   ```
   .\inai-inai.exe
   ```

Output images are saved in the `output` directory in the current directory.

### Example 4

1. Run `inai-inai.exe` using the command below. The path to the EXE and image files depend on your current directory. Multiple files can be accepted:

   ```
   .\inai-inai.exe your\image.jpg another\input\image.jpg
   ```

Output images are saved in the `output` directory in the current directory.

### Example 5

You can pass image files to `inai-inai.exe` with PowerShell. The command below passes all the files in the current directory. The path to the EXE depends on your current directory.

```
.\inai-inai.exe @(ls -File)
```

Output images are saved in the `output` directory in the current directory.

### Example 6

You can pass image files to `inai-inai.exe` with PowerShell. The command below passes the files in the `input` directory whose name starts with "IMG\_". The path to the EXE depends on your current directory.

```
.\inai-inai.exe @(ls .\input\ -File | ? { $_.Name -match "^IMG_" })
```

Output images are saved in the `output` directory in the current directory.

## Options

| Option                                      | Description                  |
| ------------------------------------------- | ---------------------------- |
| `-i\|--input-directory <INPUT_DIRECTORY>`   | Specify an input directory.  |
| `-o\|--output-directory <OUTPUT_DIRECTORY>` | Specify an output directory. |
| `-vb\|--Verbose`                            | Enable verbose logging.      |
| `-v\|--Version`                             | Display the version.         |

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
