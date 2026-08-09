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

- Windows (64ビット)
- .NET 8 以降
- git (`git clone` のため)

## ビルド

リポジトリをクローンしてください。

```
git clone git@github.com:taidalog/inai-inai.git
cd inai-inai
```

もしくは、https://github.com/taidalog/inai-inai から ZIP をダウンロードしてもいいです。

次にアプリケーションをビルドします。

```
dotnet build
```

## パブリッシュ

(すでにリポジトリをクローンしてあり、`inai-inai` ディレクトリにいるものとします)

プロジェクトをパブリッシュします。

```
dotnet publish
```

出来上がった EXE ファイルはパソコン内のどこに置いても動作します。

## 使い方

### 使い方 1

1. カレントディレクトリに `input` ディレクトリを作成してください。
1. `input` ディレクトリに画像ファイルを配置してください。
1. 以下のコマンドでアプリケーションの DLL を実行してください。DLL へのパスはカレントディレクトリによって変わります。

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll
   ```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

### 使い方 2

1. 以下のコマンドでアプリケーションの DLL を実行してください。DLL や画像ファイルへのパスはカレントディレクトリによって変わります。複数のファイルを渡すことができます。

   ```
   dotnet .\src\bin\Debug\net10.0\win-x64\inai-inai.dll your\image.jpg another\input\image.jpg
   ```

結果の画像はカレントディレクトリの `output` ディレクトリに保存されます。

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

1. `inai-inai.exe` と同じ階層に `input` ディレクトリを作成してください。
1. `input` ディレクトリに画像ファイルを配置してください。
1. `inai-inai.exe` をダブルクリックして実行してください。

結果の画像は `inai-inai.exe` と同じ階層の `output` ディレクトリに保存されます。

### 使い方 6

1. 出来上がった実行ファイルの上に画像ファイルをドラッグアンドドロップしてください。

結果の画像は `inai-inai.exe` と同じ階層の `output` ディレクトリに保存されます。

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
