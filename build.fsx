#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.IO
open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators
open Fake.Core.TargetOperators

module Tools =
    let private findTool tool winTool =
        let tool = if Environment.isUnix then tool else winTool
        match ProcessUtils.tryFindFileOnPath tool with
        | Some t -> t
        | _ ->
            let errorMsg =
                tool + " was not found in path. " +
                "Please install it and make sure it's available from your path. "
            failwith errorMsg

    let private runTool (cmd:string) args workingDir =
        let arguments = args |> String.split ' ' |> Arguments.OfArgs
        Command.RawCommand (cmd, arguments)
        |> CreateProcess.fromCommand
        |> CreateProcess.withWorkingDirectory workingDir
        |> CreateProcess.ensureExitCode
        |> Proc.run
        |> ignore

    let dotnet cmd workingDir =
        let result =
            DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
        if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let librarySrcPath = "src" </> "Tables.FSharp"
let testsSrcPath = "tests" </> "Tables.FSharp.Tests"

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

"Test" ==> "Pack"
"Test" ==> "Publish"
"Clean" ==> "Pack" ==> "Publish"

Target.runOrDefaultWithArguments "Test"