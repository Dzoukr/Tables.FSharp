module Azure.Data.Tables.FSharp.FilterConverter

open System
open System.Globalization
open System.Text

module private ColumnComparison =
    let comparison = function
        | Eq _ -> "eq"
        | Ne _ -> "ne"
        | Gt _ -> "gt"
        | Lt _ -> "lt"
        | Ge _ -> "ge"
        | Le _ -> "le"

    let value = function
        | Eq x
        | Ne x
        | Gt x
        | Lt x
        | Ge x
        | Le x -> x

module private BinaryOperation =
    let operation = function
        | And -> "and"
        | Or -> "or"

module private UnaryOperation =
    let operation = function
        | Not -> "not"

module private StringValue =
    let private formatProvider = CultureInfo.InvariantCulture

    let forBinary (value:byte[]) =
        let sb = StringBuilder()
        for num in value do sb.AppendFormat("{0:x2}", num :> obj) |> ignore
        String.Format(formatProvider, "X'{0}'", sb.ToString())

    let forBool (value:bool) =
        if value then "true" else "false"

    let forDateTimeOffset (value:DateTimeOffset) =
        let v = value.UtcDateTime.ToString("o", formatProvider)
        sprintf "datetime'%s'" v

    let forDateTime (value:DateTime) = DateTimeOffset(value) |> forDateTimeOffset

    let forDouble (value:double) = Convert.ToString(value, formatProvider)

    let forGuid (value:Guid) = sprintf "guid'%s'" (value.ToString())

    let forInt (value:int) = Convert.ToString(value, formatProvider)

    let forLong (value:int64) =
        Convert.ToString(value, formatProvider)
        |> sprintf "%sL"

    let forAny (v:obj) = Convert.ToString(v, formatProvider) |> sprintf "'%s'"

let private getColumnComparison field comp =
    let stringValue =
        match ColumnComparison.value comp with
        | :? (byte[]) as v -> v |> StringValue.forBinary
        | :? bool as v -> v |> StringValue.forBool
        | :? DateTime as v -> v |> StringValue.forDateTime
        | :? DateTimeOffset as v -> v |> StringValue.forDateTimeOffset
        | :? double as v -> v |> StringValue.forDouble
        | :? Guid as v -> v |> StringValue.forGuid
        | :? int as v -> v |> StringValue.forInt
        | :? int64 as v -> v |> StringValue.forLong
        | v -> v |> StringValue.forAny
    sprintf "%s %s %s" field (ColumnComparison.comparison comp) stringValue

let rec toQuery (f:Filter) =
    match f with
    | Empty -> ""
    | Column (field, comp) -> getColumnComparison field comp
    | Binary(w1, op, w2) ->
        match toQuery w1, toQuery w2 with
        | "", fq | fq , "" -> fq
        | fq1, fq2 -> sprintf "(%s) %s %s" fq1 (BinaryOperation.operation op) fq2
    | Unary (op, w) ->
        match toQuery w with
        | "" -> ""
        | v -> sprintf "%s (%s)" (UnaryOperation.operation op) v
