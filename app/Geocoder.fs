module Geocoder
    let [<Literal>] baseUri = "https://geocode.maps.co/search?q="
    let [<Literal>] sample = baseUri + "1600+Pennsylvania+Avenue,+Washington,+DC"
    type GeocodedLocation = FSharp.Data.JsonProvider<sample>
    
    let geocode s =
        let uri = baseUri+System.Web.HttpUtility.UrlEncode(str=s)
        let res = GeocodedLocation.Load(uri) |> Array.toList
        match res with
            | [] -> None
            | h::_ -> Some({
                    DomainTypes.Location.Address=h.DisplayName
                    DomainTypes.Location.Lat=h.Lat * 1.0m<DomainTypes.deg>
                    DomainTypes.Location.Lon=h.Lon * 1.0m<DomainTypes.deg>
                })