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

namespace InaiInai

open System
open System.IO
open FaceONNX

type FaceBlurResult =
    { Path: string
      Faces: FaceDetectionResult array
      DetectingSeconds: float
      Width: int
      Height: int
      Length: int64
      BlurringSeconds: float
      ResultPath: string
      SavingSeconds: float }

[<RequireQualifiedAccess>]
module FaceBlurResult =
    let empty: FaceBlurResult =
        { Path = String.Empty
          Faces = Array.empty
          DetectingSeconds = 0.0
          Width = 0
          Height = 0
          Length = 0L
          BlurringSeconds = 0.0
          ResultPath = String.Empty
          SavingSeconds = 0.0 }

    let toString (x: FaceBlurResult) : string =
        [ Resources.Strings.``Detecting faces in:\t{0}`` (FileInfo x.Path)
          Resources.Strings.``Detected face(s):\t{0} face(s), {1} seconds`` (Array.length x.Faces) x.DetectingSeconds
          Resources.Strings.``Image dimensions:\t{0} x {1} pixels`` x.Width x.Height
          Resources.Strings.``Image size:\t\t{0} MB`` $"{float x.Length / 1024. / 1024.:F2}"
          Resources.Strings.``Masking time:\t\t{0} seconds`` x.BlurringSeconds
          Resources.Strings.``Saved image:\t\t{0}, {1} seconds\n`` x.ResultPath x.SavingSeconds ]
        |> String.concat Environment.NewLine
