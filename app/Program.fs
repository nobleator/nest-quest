[<EntryPoint>]
let main args =
    printfn "Arguments passed to function : %A" args
    match args |> Array.toList with
    | first::[]  ->
        printfn "1 arg"
        let t = Parser.parse first
        printfn $"{t}"
        // TODO: Persist AST?
        0
    | first::rest  ->
        printfn "2+ args"
        let t = Parser.parse first
        // Geocoding lookups for eval target(s)
        let l = List.map Geocoder.geocode rest |> List.choose id
        List.map (fun x ->
            AST.traverse Evaluators.OverpassEvaluator.score x t
                |> fun score -> $"{x.Address}: {score}"
            ) l
            |> fun x -> printfn $"{x}"
        // Return 0. This indicates success.
        0
    | _ ->
        printfn $"no args"
        -1