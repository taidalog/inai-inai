# inai-inai

A privacy tool that blurs faces in images.

The name "inai-inai" comes from "inai inai baa!", the Japanese version of peek-a-boo.

Written in F#.

## Features

- Detects faces in images and blurs them.

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

## Usage

There are three ways to run the application.

### Usage 1

1. Create an `input` directory.
1. Place image files in the `input` directory.
1. Run the application DLL:

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll
   ```

Output images are saved in the `output` directory next to the DLL.

### Usage 2

1. Publish the application as a self-contained executable:

   ```
   dotnet publish -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded
   ```

1. Create an `input` directory next to `inai-inai.exe`.
1. Place image files in the `input` directory.
1. Run the application by double-clicking `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

### Usage 3

1. Publish the application as a self-contained executable:

   ```
   dotnet publish -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded
   ```

1. Drag and drop image files onto `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

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

For a complete list of dependencies and their licenses, see [licenses.txt](./licenses.txt) and [NOTICE](./NOTICE).

## Copyright

Copyright 2026 taidalog
