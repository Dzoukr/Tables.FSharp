[<AutoOpen>]
module Azure.Data.Tables.FSharp.Builders

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
let pk (value:string) = column "PartitionKey" (Eq value)
/// FILTER RK equals
let rk (value:string) = column "RowKey" (Eq value)