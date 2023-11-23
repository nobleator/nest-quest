module DomainTypes
    [<Measure>] type deg
    type Location = {
        Address: string
        Lat: decimal<deg>
        Lon: decimal<deg>
    }