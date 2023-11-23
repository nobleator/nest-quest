module AST

type ArithmeticOperator =
    | Lt = 0
    | LtEq = 1
    | Eq = 2
    | GtEq = 3
    | Gt = 4
type LogicalOperator =
    | And = 0
    | Or = 1
type Branch = {
    Weight : float
    Operator : LogicalOperator
    Children : Tree list
}
and Leaf = {
    Weight : float
    Tag : string
    Operator : ArithmeticOperator
    Target : float
    }
and Tree =
    | BranchNode of Branch
    | LeafNode of Leaf

let add x y =
    x + y

let max x y = 
    if x > y then x
    else y

let rec traverse scoreFun loc tree =
        match tree with
        | LeafNode leaf -> leaf.Weight * scoreFun (loc, leaf)
        | BranchNode b ->
            match b.Operator with
            | LogicalOperator.And -> List.fold add 0.0 (List.map (traverse scoreFun loc) b.Children)
            | LogicalOperator.Or -> List.fold max 0.0 (List.map (traverse scoreFun loc) b.Children)
            | _ -> 0.0
            |> fun branchValue -> branchValue * b.Weight