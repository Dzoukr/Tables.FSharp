module Azure.Data.Tables.FSharp.Tests.Program

open Azure.Data.Tables
open Expecto
open Expecto.Logging
open Microsoft.Extensions.Configuration

let testConfig =
    { defaultConfig with
        parallelWorkers = 4
        verbosity = LogLevel.Debug }

[<EntryPoint>]
let main _ =
    let conf = (ConfigurationBuilder()).AddJsonFile("local.settings.json").Build()
    let tableClient = TableClient(conf.["storage"], "TablesFSharpTests")
    Data.prepareData tableClient |> ignore
    QueryTests.tests tableClient |> runTests testConfig