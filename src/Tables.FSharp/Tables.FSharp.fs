namespace Azure.Data.Tables.FSharp

type ColumnComparison =
    | Eq of obj
    | Ne of obj
    | Gt of obj
    | Lt of obj
    | Ge of obj
    | Le of obj

type BinaryOperation =
    | And
    | Or

type UnaryOperation =
    | Not

type Filter =
    | Empty
    | Column of string * ColumnComparison
    | Binary of Filter * BinaryOperation * Filter
    | Unary of UnaryOperation * Filter
    static member (+) (a, b) = Binary(a, And, b)
    static member (*) (a, b) = Binary(a, Or, b)
    static member (!!) a = Unary (Not, a)

module Keys =
    let [<Literal>] PartitionKey = "PartitionKey"
    let [<Literal>] RowKey = "RowKey"

type TableQuery = {
    Filter : Filter
    MaxPerPage : int option
    Select : string list
    CancellationToken : System.Threading.CancellationToken
}