[<EntryPoint>]
let main args =
    printfn "Arguments passed to function : %A" args
    match args |> Array.toList with
    | first::[] -> Parser.parse first
                    |> fun t -> printfn $"{t}"
                    |> fun x -> 0 // Return 0. This indicates success.
    | first::rest -> Parser.parse first
                        |> fun t -> Evaluators.MasterEvaluator.evaluate Evaluators.MasterEvaluator.Evaluator.Overpass t rest
                        |> fun x -> printfn $"{x}"
                        |> fun x -> 0 // Return 0. This indicates success.
    | _ -> -1