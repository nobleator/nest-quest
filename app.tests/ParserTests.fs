module ParserTests

open Xunit

[<Fact>]
let ``Parse unquoted expression`` () =
    let input = @"school within 100 m"
    let t = Parser.parse input
    let e = AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0}
    Assert.Equivalent(e, t)
    
[<Fact>]
let ``Parse quoted expression`` () =
    let input = @"""Harris Teeter"" within 100 m"
    let t = Parser.parse input
    let e = AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse unquoted conjunction`` () =
    let input = @"park within 100 m and school within 1000 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.LeafNode {Weight=1.0; Tag="park"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse quoted conjunction`` () =
    let input = @"""Harris Teeter"" within 100 m and ""Trader Joe's"" within 1000 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse unquoted disjunction`` () =
    let input = @"park within 100 m or school within 100 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
            AST.LeafNode {Weight=1.0; Tag="park"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse quoted disjunction`` () =
    let input = @"""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
            AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse parenthesized disjunction and serial conjunction`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
                AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
                AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            ]};
            AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse parenthesized disjunction and serial conjunction pair`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m and park within 500 m"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
                AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
                    AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
                    AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
                ]};
                AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
            ]};
            AST.LeafNode {Weight=1.0; Tag="park"; Operator=AST.ArithmeticOperator.LtEq; Target=500.0};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse parenthesized disjunction and parenthesized conjunction pair`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m)"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
                AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
                AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            ]};
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
                AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
                AST.LeafNode {Weight=1.0; Tag="park"; Operator=AST.ArithmeticOperator.LtEq; Target=500.0};
            ]};
        ]}
    Assert.Equivalent(e, t)

[<Fact>]
let ``Parse parenthesized root with parenthesized disjunction and parenthesized conjunction pair`` () =
    let input = @"((""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m))"
    let t = Parser.parse input
    let e = AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.Or; Children=[
                AST.LeafNode {Weight=1.0; Tag="Harris Teeter"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
                AST.LeafNode {Weight=1.0; Tag="Trader Joe's"; Operator=AST.ArithmeticOperator.LtEq; Target=100.0};
            ]};
            AST.BranchNode {Weight=1.0; Operator=AST.LogicalOperator.And; Children=[
                AST.LeafNode {Weight=1.0; Tag="school"; Operator=AST.ArithmeticOperator.LtEq; Target=1000.0};
                AST.LeafNode {Weight=1.0; Tag="park"; Operator=AST.ArithmeticOperator.LtEq; Target=500.0};
            ]};
        ]}
    Assert.Equivalent(e, t)
