module EvaluatorTests

open Xunit
open DomainTypes
open Evaluators

module OverpassEvaluatorTests = 
    [<Fact>]
    let ``Build query`` () =
        let l = {
            DomainTypes.Location.Address="123 Main Street"
            DomainTypes.Location.Lat=(38.889m<DomainTypes.deg>)
            DomainTypes.Location.Lon=(-77.035m<DomainTypes.deg>)
        }
        let q = OverpassEvaluator.buildQuery "apple" "banana" 1 l
        let e = @"[out:json];nwr[""apple""~""banana"",i](around:1,38.889,-77.035);out count;"
        Assert.Equal(e, q)
