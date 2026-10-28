# inai-inai

A privacy tool that blurs faces in images.

The name "inai-inai" comes from "inai inai baa!", the Japanese version of peek-a-boo.

Written in F#.

[Japanese README](README.ja.md)

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Publishing the application](#publishing-the-application)
- [Usage](#usage)
- [Known Issues](#known-issues)
- [Release Notes](#release-notes)
- [License](#license)
  - [Third-party License Compliance](#third-party-license-compliance)
- [Copyright](#copyright)

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

1. Run `inai-inai.exe` by double-clicking it.
1. Drag and drop image files onto `inai-inai.exe`.

Output images are saved in the `output` directory next to `inai-inai.exe`.

## Known Issues

- This app cannot currently be used from the command line because it is now a GUI app.

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
