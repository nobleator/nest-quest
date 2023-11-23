module LexerTests

open Xunit

// Cannot use MemberData as F# lists are not automatically deserializable by xUnit: https://stackoverflow.com/questions/30574322/memberdata-tests-show-up-as-one-test-instead-of-many?
// let tokenizeInputWithResult : obj[] list =
//     [
//         [| @"school within 100 m"; [Lexer.Identifier "school"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"] |]
//         [| @"""Harris Teeter"" within 100 m" ; [Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"] |]
//         [| @"park within 100 m and school within 1000 m" ; [Lexer.Identifier "park"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"] |]
//         [| @"""Harris Teeter"" within 100 m and ""Trader Joe's"" within 1000 m" ; [Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"] |]
//         [| @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m" ; [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"] |]
//         [| @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m and park within 500 m" ; [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"] |]
//         [| @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m)" ; [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.OpenParenthesis; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis] |]
//         [| @"((""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m))" ; [Lexer.OpenParenthesis; Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.OpenParenthesis; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.CloseParenthesis] |]
//         [| @"junk" ; [] |]
//     ]

// [<Theory>]
// [<MemberData(nameof(tokenizeInputWithResult))>]
// let ``Tokenize`` input expected =
//     let t = Lexer.tokenize input
//     Assert.Equal<Lexer.Token list>(expected, t)

[<Fact>]
let ``Tokenize unquoted expression`` () =
    let input = @"school within 100 m"
    let t = Lexer.tokenize input
    let e = [Lexer.Identifier "school"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize quoted expression`` () =
    let input = @"""Harris Teeter"" within 100 m"
    let t = Lexer.tokenize input
    let e = [Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize unquoted conjunction`` () =
    let input = @"park within 100 m and school within 1000 m"
    let t = Lexer.tokenize input
    let e = [Lexer.Identifier "park"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize quoted conjunction`` () =
    let input = @"""Harris Teeter"" within 100 m and ""Trader Joe's"" within 1000 m"
    let t = Lexer.tokenize input
    let e = [Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize parenthesized disjunction and serial conjunction`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m"
    let t = Lexer.tokenize input
    let e = [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize parenthesized disjunction and serial conjunction pair`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and school within 1000 m and park within 500 m"
    let t = Lexer.tokenize input
    let e = [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize parenthesized disjunction and parenthesized conjunction pair`` () =
    let input = @"(""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m)"
    let t = Lexer.tokenize input
    let e = [Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.OpenParenthesis; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis]
    Assert.Equal<Lexer.Token list>(e, t)

[<Fact>]
let ``Tokenize parenthesized root with parenthesized disjunction and parenthesized conjunction pair`` () =
    let input = @"((""Harris Teeter"" within 100 m or ""Trader Joe's"" within 100 m) and (school within 1000 m and park within 500 m))"
    let t = Lexer.tokenize input
    let e = [Lexer.OpenParenthesis; Lexer.OpenParenthesis; Lexer.Identifier "Harris Teeter"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.Or; Lexer.Identifier "Trader Joe's"; Lexer.Within; Lexer.Number 100; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.And; Lexer.OpenParenthesis; Lexer.Identifier "school"; Lexer.Within; Lexer.Number 1000; Lexer.DistanceUnit "m"; Lexer.And; Lexer.Identifier "park"; Lexer.Within; Lexer.Number 500; Lexer.DistanceUnit "m"; Lexer.CloseParenthesis; Lexer.CloseParenthesis]
    Assert.Equal<Lexer.Token list>(e, t)
