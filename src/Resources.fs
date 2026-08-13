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
open System.Reflection
open System.Resources

module Resources =
    let assembly: Assembly = Assembly.GetExecutingAssembly()

    let resourceManager: ResourceManager =
        ResourceManager("InaiInai.Resources.Strings", assembly)

    module Strings =
        let ``Error: Parent PID not found.``: string =
            resourceManager.GetString "Error: Parent PID not found."

        let ``Error: The directory {0} does not exist.`` (directoryInfo: DirectoryInfo) : string =
            String.Format(resourceManager.GetString "Error: The directory {0} does not exist.", directoryInfo.FullName)

        let ``Create {0}, add image files, and run the program again.`` (directoryInfo: DirectoryInfo) : string =
            String.Format(
                resourceManager.GetString "Create {0}, add image files, and run the program again.",
                directoryInfo.FullName
            )

        let ``Processing {0} image(s)...\n`` (n: int) : string =
            String.Format(resourceManager.GetString @"Processing {0} image(s)...\n", n, "\n")

        let ``No image files were found.``: string =
            resourceManager.GetString "No image files were found."

        let ``Place image files in {0} and run the program again.`` (directoryInfo: DirectoryInfo) : string =
            String.Format(
                resourceManager.GetString "Place image files in {0} and run the program again.",
                directoryInfo.FullName
            )

        let ``Detecting faces in:\t{0}`` (fileInfo: FileInfo) : string =
            String.Format(resourceManager.GetString @"Detecting faces in:\t{0}", fileInfo.FullName, "\t")

        let ``Detected face(s):\t{0} face(s), {1} seconds`` (n: int) (totalSeconds: float) : string =
            String.Format(
                resourceManager.GetString @"Detected face(s):\t{0} face(s), {1} seconds",
                n,
                totalSeconds,
                "\t"
            )

        let ``Skipped face(s):\t{0} face(s)`` (n: int) : string =
            String.Format(resourceManager.GetString @"Skipped face(s):\t{0} face(s)", n, "\t")

        let ``Image dimensions:\t{0} x {1} pixels`` (width: int) (height: int) : string =
            String.Format(resourceManager.GetString @"Image dimensions:\t{0} x {1} pixels", width, height, "\t")

        let ``Image size:\t\t{0} MB`` (lengthMbString: string) : string =
            String.Format(resourceManager.GetString @"Image size:\t\t{0} MB", lengthMbString, "\t")

        let ``Face rectangle:\t\t{0}`` (rect: System.Drawing.Rectangle) : string =
            String.Format(resourceManager.GetString @"Face rectangle:\t\t{0}", rect.ToString(), "\t")

        let ``Masking time:\t\t{0} seconds`` (totalSeconds: float) : string =
            String.Format(resourceManager.GetString @"Masking time:\t\t{0} seconds", totalSeconds, "\t")

        let ``Saved image:\t\t{0}, {1} seconds\n`` (outputPath: string) (totalSeconds: float) : string =
            String.Format(
                resourceManager.GetString @"Saved image:\t\t{0}, {1} seconds\n",
                outputPath,
                totalSeconds,
                "\t",
                "\n"
            )

        let ``Press any key to exit...``: string =
            resourceManager.GetString "Press any key to exit..."
