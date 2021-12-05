open Fake
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.Core.TargetOperators
open System
open System.IO

open BuildHelpers
open BuildTools



initializeContext()

let librarySrcPath = "src" </> "Tables.FSharp"
let testsSrcPath = "tests" </> "Tables.FSharp.Tests"

// Targets
Target.create "Clean" (fun _ ->
    [
        librarySrcPath </> "bin"
        librarySrcPath </> "obj"
    ]
    |> Shell.deleteDirs
)

Target.create "Pack" (fun _ ->
    Tools.dotnet "restore --no-cache" librarySrcPath
    Tools.dotnet "pack -c Release" librarySrcPath
)

Target.create "Test" (fun _ -> Tools.dotnet "run" testsSrcPath)

Target.create "Publish" (fun _ ->
    let nugetKey =
        match Environment.environVarOrNone "NUGET_KEY" with
        | Some nugetKey -> nugetKey
        | None -> failwith "The Nuget API key must be set in a NUGET_KEY environmental variable"
    let nupkg =
        Directory.GetFiles(librarySrcPath </> "bin" </> "Release")
        |> Seq.head
        |> Path.GetFullPath
    Tools.dotnet (sprintf "nuget push %s -s nuget.org -k %s" nupkg nugetKey) librarySrcPath
)

let dependencies = [
    "Test" ==> "Pack"
    "Test" ==> "Publish"
    "Clean" ==> "Pack" ==> "Publish"
]

[<EntryPoint>]
let main args = runOrDefault "Test" args