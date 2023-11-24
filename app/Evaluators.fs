namespace Evaluators

open FSharp.Data
open DomainTypes
open AST

module OverpassEvaluator =
    type OverpassResult = JsonProvider<"overpass_sample.json">
    let [<Literal>] url = "https://www.overpass-api.de/api/interpreter"

    let buildQuery tagKey tagValue distance loc =
        // TODO: taginfo lookup on key and value permutations to handle flexible "search"
        [
            @"[out:json];";
            @"nwr[""";tagKey;@"""~""";tagValue;
            @""",i](around:";distance.ToString();",";
            loc.Lat.ToString();",";loc.Lon.ToString();");";
            @"out count;"
        ] |> String.concat ""

    let score (loc : Location, leaf : Leaf) =
        match leaf.Tag.Split('=') with
        | [|key; value|] ->
            buildQuery key value leaf.Target loc
                |> fun x -> Http.RequestString(url, body = FormValues [ "data", x ])
                |> OverpassResult.Parse
                |> fun x -> x.Elements
                |> Array.toList
                |> List.head
                |> fun x -> float x.Tags.Total
        | _ ->
            printfn $"oops, invalid tag format: {leaf.Tag}"
            0