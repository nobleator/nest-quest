namespace Evaluators

open FSharp.Data
open DomainTypes
open AST

type EvaluatorEnum =
| Overpass = 0

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

module BaseEvaluator =
    // TODO: move to interface/abstract implementation?

    let evaluate e t l =
        match e with
        | EvaluatorEnum.Overpass ->
            List.map Geocoder.geocode l
            |> List.choose id
            |> List.map (fun x -> AST.traverse OverpassEvaluator.score x t |> fun score -> x.Address, score)
            |> Map.ofList
        | _ -> failwith "Unsupported evaluator type"