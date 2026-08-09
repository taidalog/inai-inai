# inai-inai

画像内の顔にぼかしをかけるプライバシーツール。

「inai-inai (いないいない)」という名前は、「いないいないばあ」から取りました。

F# で書きました。

[English README](README.md)

## 機能

- 画像内の顔を検出してぼかしをかけます。
- 画像はインターネットにアップロードせず、ローカルで動作します。
- 画像内の顔の縦横のピクセル数に応じてぼかしの強さを調整します。

## 動作環境

- Windows (64 ビット)
- .NET 10 (`dotnet publish` のため)
- git (`git clone` のため)

## アプリを発行する

1. リポジトリをクローンしてください。

   ```
   git clone git@github.com:taidalog/inai-inai.git
   cd inai-inai
   ```

もしくは、https://github.com/taidalog/inai-inai から ZIP をダウンロードしてもいいです。

1. 次にアプリケーションを発行してください。

   ```
   dotnet publish
   ```

`inai-inai.exe` がリポジトリのルートに出力されます。この EXE ファイルはパソコン内のどこに置いても動作します。

## 使い方

### 使い方 1

1. `inai-inai.exe` と同じ階層に `input` ディレクトリを作成してください。
1. `input` ディレクトリに画像ファイルを配置してください。
1. `inai-inai.exe` をダブルクリックして実行してください。

結果の画像は `inai-inai.exe` と同じ階層の `output` ディレクトリに保存されます。

### 使い方 2

1. 出来上がった実行ファイルの上に画像ファイルをドラッグアンドドロップしてください。

結果の画像は `inai-inai.exe` と同じ階層の `output` ディレクトリに保存されます。

### 使い方 3

1. カレントディレクトリに `input` ディレクトリを作成してください。
1. `input` ディレクトリに画像ファイルを配置してください。
1. 以下のコマンドで実行ファイルを実行してください。実行ファイルへのパスはカレントディレクトリによって変わります。

   ```
   .\inai-inai.exe
   ```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

### 使い方 4

1. 以下のコマンドで実行ファイルを実行してください。実行ファイルや画像ファイルへのパスはカレントディレクトリによって変わります。複数のファイルを渡すことができます。

   ```
   .\inai-inai.exe your\image.jpg another\input\image.jpg
   ```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

### 使い方 5

PowerShell を使って `inai-inai.exe` に画像ファイルを渡すことができます。以下のコマンドは、カレントディレクトリ内のすべてのファイルを渡します。実行ファイルへのパスはカレントディレクトリによって変わります。

```
.\inai-inai.exe @(ls -File)
```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

### 使い方 6

PowerShell を使って `inai-inai.exe` に画像ファイルを渡すことができます。以下のコマンドは、`input` ディレクトリ内のファイルのうち、名前が "IMG\_" で始まるものを渡します。実行ファイルへのパスはカレントディレクトリによって変わります。

```
.\inai-inai.exe @(ls .\input\ -File | ? { $_.Name -match "^IMG_" })
```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

## オプション

| オプション                                  | 説明                            |
| ------------------------------------------- | ------------------------------- |
| `-i\|--input-directory <INPUT_DIRECTORY>`   | input ディレクトリを指定する。  |
| `-o\|--output-directory <OUTPUT_DIRECTORY>` | output ディレクトリを指定する。 |
| `-vb\|--Verbose`                            | 詳細なログを有効化する。        |
| `-v\|--Version`                             | バージョンを表示する。          |

## 既知の問題

- 特になし。

## リリースノート

[Releases on GitHub](https://github.com/taidalog/inai-inai/releases)

## ライセンス

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

## 著作権表示

Copyright 2026 taidalog
