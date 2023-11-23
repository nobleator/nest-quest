module Lexer

type Token =
    | Identifier of string
    | Number of float
    | DistanceUnit of string
    | And | Or
    | OpenParenthesis | CloseParenthesis
    | Within
    | EOF

let tokenize input =
    let rec splitHelper acc currentToken rest =
        match rest with
        | "" -> List.rev (currentToken :: acc)
        | _ ->
            match rest.[0] with
            | ' ' when currentToken = "" -> splitHelper acc "" rest.[1..]
            | ' ' -> splitHelper (currentToken :: acc) "" rest.[1..]
            | '\"' ->
                let endIndex = rest.IndexOf("\"", 1)
                let quotedToken = rest.[1..endIndex-1]
                splitHelper (quotedToken :: acc) "" rest.[endIndex+1..]
            | '(' -> splitHelper ("(" :: currentToken :: acc) "" rest.[1..]
            | ')' -> splitHelper (")" :: currentToken :: acc) "" rest.[1..]
            | _ -> splitHelper acc (currentToken + rest.[0].ToString()) rest.[1..]

    splitHelper [] "" input
    |> List.filter (fun x -> x <> "")
    |> List.map (fun s ->
        match s.ToLower() with
        | "and" -> And
        | "or" -> Or
        | "(" -> OpenParenthesis
        | ")" -> CloseParenthesis
        | "within" -> Within
        | "m" | "km" | "mi" | "ft" -> DistanceUnit s
        | _ ->
            match System.Double.TryParse s with
            | true, n -> Number n
            | _ -> Identifier s)
