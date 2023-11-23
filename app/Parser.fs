module Parser

open AST
open Lexer

(*
    Note: this currently generates a binary tree. TBD if all sibling leaf nodes should be flattened.

    Grammar definition

    expression ::= term { ( "and" | "or" ) term }
    term       ::= factor { ( "and" | "or" ) factor }
    factor     ::= atomic_expression| "(" atomic_expression ")" | expression | "(" expression ")"
    atomic_expression ::= identifier "within" number distance_unit
    identifier ::= letter { letter | digit }
    number     ::= digit { digit }
    distance_unit ::= "m" | "km" | "mi" | "ft"
    letter     ::= "a" | "b" | ... | "z" | "A" | "B" | ... | "Z"
    digit      ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

*)

let consumeToken tokens index =
    if index < List.length tokens then List.item index tokens
    else EOF

let parseIdentifier tokens currentIndex =
    match consumeToken tokens currentIndex with
    | Identifier s -> s
    | _ -> failwith "Unexpected token"

let parseNumber tokens currentIndex =
    match consumeToken tokens currentIndex with
    | Number n -> n
    | _ -> failwith "Unexpected token"

let parseDistanceUnit tokens currentIndex =
    match consumeToken tokens currentIndex with
    | DistanceUnit u -> u
    | _ -> failwith "Unexpected token"

let expectTokenType tokens expected index =
    let actual = consumeToken tokens index
    if actual <> expected then
        failwith $"Expected {expected}, but got {actual}"

let rec parseExpression tokens currentIndex =
    let termNode, newIndex = parseTerm tokens currentIndex
    let rec loop node index =
        match consumeToken tokens index with
        | And | Or ->
            let op = match consumeToken tokens index with
                     | And -> LogicalOperator.And
                     | Or -> LogicalOperator.Or
                     | _ -> failwith "Unexpected token"
            let rightTerm, newIndex = parseTerm tokens (index + 1)
            loop (BranchNode { Weight = 1.0; Operator = op; Children = [node; rightTerm] }) newIndex
        | _ -> node, index
    loop termNode newIndex
and parseTerm tokens currentIndex =
    let factorNode, newIndex = parseFactor tokens currentIndex
    let rec loop node index =
        match consumeToken tokens index with
        | And | Or ->
            let op = match consumeToken tokens index with
                     | And -> LogicalOperator.And
                     | Or -> LogicalOperator.Or
                     | _ -> failwith "Unexpected token"
            let rightFactor, newIndex = parseFactor tokens (index + 1)
            loop (BranchNode { Weight = 1.0; Operator = op; Children = [node; rightFactor] }) newIndex
        | _ -> node, index
    loop factorNode newIndex
and parseFactor tokens currentIndex =
    match consumeToken tokens currentIndex with
    | OpenParenthesis ->
        let innerExpression, newIndex = parseExpression tokens (currentIndex + 1)
        expectTokenType tokens CloseParenthesis newIndex
        innerExpression, (newIndex + 1)
    | Identifier _ | Number _ | DistanceUnit _ ->
        parseAtomicExpression tokens currentIndex
    | _ -> failwith "Unexpected token"
and parseAtomicExpression tokens currentIndex =
    let identifier = parseIdentifier tokens currentIndex
    expectTokenType tokens Within (currentIndex + 1)
    let number = parseNumber tokens (currentIndex + 2)
    let distanceUnit = parseDistanceUnit tokens (currentIndex + 3)
    LeafNode { Weight = 1.0; Tag = identifier; Operator = ArithmeticOperator.LtEq; Target = number }, currentIndex + 4

let parse input =
    tokenize input
    |> fun x -> parseExpression x 0
    |> fst

let parseOld raw =
    AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.BranchNode {Weight=0.5; Operator=AST.LogicalOperator.Or; Children=[
                AST.LeafNode {Weight=1.0; Tag="brand=Harris Teeter"; Operator=AST.ArithmeticOperator.Eq; Target=1000.0};
                AST.LeafNode {Weight=1.0; Tag="brand=Whole Foods Market"; Operator=AST.ArithmeticOperator.Eq; Target=1200.0};
                AST.LeafNode {Weight=1.0; Tag="brand=Trader Joe's"; Operator=AST.ArithmeticOperator.Eq; Target=1200.0};
            ]};
            AST.BranchNode {Weight=0.5; Operator=AST.LogicalOperator.And; Children=[
                AST.LeafNode {Weight=0.2; Tag="highway=cycleway"; Operator=AST.ArithmeticOperator.Eq; Target=800.0};
                AST.LeafNode {Weight=0.12; Tag="leisure=park"; Operator=AST.ArithmeticOperator.Eq; Target=1600.0};
                AST.LeafNode {Weight=0.18; Tag="amenity=school"; Operator=AST.ArithmeticOperator.Eq; Target=2000.0};
            ]}
        ]}