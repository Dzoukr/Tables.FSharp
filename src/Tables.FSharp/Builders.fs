[<AutoOpen>]
module Azure.Data.Tables.FSharp.Builders

type TableQueryBuilder() =
    member __.Yield _ = TableQuery.Empty

    /// Sets the FILTER for query
    [<CustomOperation "filter">]
    member __.Filter (state:TableQuery, value) = { state with Filter = value }

    /// Sets the MAXPERPAGE for query
    [<CustomOperation "maxPerPage">]
    member __.MaxPerPage (state:TableQuery, value) = { state with MaxPerPage = Some value }

    /// Sets the SELECT columns for query
    [<CustomOperation "select">]
    member __.Select (state:TableQuery, value) = { state with Select = value }

    /// Sets the CANCELLATION TOKEN for query
    [<CustomOperation "cancellationToken">]
    member __.CancellationToken (state:TableQuery, value) = { state with CancellationToken = value }

let tableQuery = TableQueryBuilder()

/// Creates FILTER condition for column
let column name whereComp = Filter.Column(name, whereComp)
/// FILTER column value equals to
let eq name (o:obj) = column name (Eq o)
/// FILTER column value not equals to
let ne name (o:obj) = column name (Ne o)
/// FILTER column value greater than
let gt name (o:obj) = column name (Gt o)
/// FILTER column value lower than
let lt name (o:obj) = column name (Lt o)
/// FILTER column value greater/equals than
let ge name (o:obj) = column name (Ge o)
/// FILTER column value lower/equals than
let le name (o:obj) = column name (Le o)
/// FILTER PK equals
let pk (value:string) = column Keys.PartitionKey (Eq value)
/// FILTER RK equals
let rk (value:string) = column Keys.RowKey (Eq value)