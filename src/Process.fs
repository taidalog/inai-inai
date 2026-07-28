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

open System.Management

module Process =
    let getParentPid (pid: int) : int option =
        let query = $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = %d{pid}"
        use searcher = new ManagementObjectSearcher(query)
        use results = searcher.Get()

        results
        |> Seq.cast<ManagementObject>
        |> Seq.tryHead
        |> Option.bind (fun mo ->
            use mo = mo

            match mo["ParentProcessId"] with
            | :? uint32 as x -> Some(int x)
            | :? int as x -> Some x
            | _ -> None)
