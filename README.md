# Nest Quest

This project intends to help identify if a potential home meets your requirements. Users configure their preferences in a tree structure. These preferences include conditional statements as well as arithmetic operators and distance values. This tree can then be used to evaluate a target location using various providers such as Open Street Map.

## Getting Started
To run this project locally, start by cloning this repo. There are multiple microservices, but all of them are included in this monorepo for now. Assuming you have Docker and Docker Compose installed, you can start the stack with:
> docker compose up

I prefer to also include the `detach` and `build` flags to ensure the latest code is being rebuilt each time:
> docker compose up -d --build

Once this is running, you should be able to navigate to `http://localhost/` in your browswer and see the web client running.

### Installing Packages
While the Dockerfiles handle running the existing application, if you want to install any additional packages you will need the [.NET SDK](https://dotnet.microsoft.com/en-us/download) (preferably v9+) and [pnpm](https://pnpm.io/installation) (preferably v10.8+) installed locally.

## Architecture

TODO

The architecture for this project can be decomposed into several subsystems and/or components:
- Client for user preference input. This could take several forms, such as a website, CLI, or browser extension. Regardless of the implementation each client would adhere to the same general structure and interface with the same APIs.
- Lexer/tokenizer for user preferences, which are passed in as a string.
- Parser that converts tokens into an AST.
- Weight calculator using AHP, Paired Comparisons, or Reference Comparisons (http://www2.mitre.org/work/sepo/toolkits/STEP/files/ScoringMethodsContent.pdf).
- Evaluators that evaluate a given location against the AST, returning a utility score. This should be generalized to allow multiple evaluator implementations, e.g. Overpass, Google Maps, Apple Maps, etc.
- Datastore that manages retrieval and storage of data from external locations.

```mermaid
graph TD
    Client --> Location
    Client --> Preferences
    Preferences --> Tokenizer
    Preferences <--> Autocomplete
    Tokenizer --> Parser
    Parser --> AST
    AST --> Evaluator
    Location --> Evaluator
    Evaluator <--> Datastore
    Autocomplete <--> Datastore
    Datastore <--> Overpass
    Datastore <--> GoogleMaps
    Datastore <--> Zillow
    Datastore <--> Redfin
    Datastore <--> ...
```

## External Dependencies

There are several external APIs used during operation of this extension.
- [OpenStreetMap taginfo](https://taginfo.openstreetmap.org/) for location tags
- [Maps.co](https://geocode.maps.co) for geocoding addresses to coordinates [no longer usable without API key]
- [Nominatim](https://nominatim.org) for geocoding address to coordinates
- [Overpass](https://www.overpass-api.de) for proximity calculations

## Roadmap
v0.0.1: static webpage with fictional houses & attractions using basic drop-downs and simple OR logic for all filters.
- [x] Basic HTML page with fake data

v0.0.2: OSM API integration to populate real building/location/attraction data. Add docker compose with vanilla JS UI, API, sqlite database, and evaluation engine services.
Infrastructure:
- [x] Docker compose services: web client, web API, database, web server
- [ ] Language and framework selections for all services
- [x] Web API Dockerfile
- [x] Web client Dockerfile
- [ ] Database Dockerfile?
- [x] Web server Caddyfile for localhost
- [x] Caching for Overpass requests
- [x] Rate limiting for Overpass requests
- [x] Loading indicator while requests are being processed
- [ ] Database versioning: EF code-first or explicit scripts?
- [ ] Logging (writing to Docker via console for now)
- [ ] Unit test framework(s)
- [ ] Swagger page for web API with version tag dropdown properly configured
- [ ] Add version tag to web client
- [ ] Add version tag to web API headers
- [ ] Add healthcheck endpoint to web API, including version tag
Business logic:
- [x] `Criterion` object with category, tolerance, unit, and sign/direction properties (will add weights and travel mode later)
- [x] Point of interest categories defined via enum, including Unknown option
- [x] Web client to allow dynamic addition and removal of criterion objects
- [x] Web client to load critera from stored data via `GET /api/v0/criteria` endpoint
- [x] Database table `Criterion` to store criteria (will need to update this later to include user linkage)
- [ ] `POST /api/v0/criteria` endpoint to store list of criteria (will eventually be replaced with comprehensive tree) TODO check persistence of data between docker rebuilds
- [x] Intergrate with Overpass API to perform POI lookups
- [x] Evaluation engine to take in criteria and a target location and return a score (binary score for now, will return a continuous score later). Score is using simple distance calculation directly in Overpass query for now, this will be expanded with additional options later.
- [x] `GET /api/v0/poi` endpoint to fetch POI data by bounding box and category and mark these points on the map
- [x] `GET /api/v0/homes` endpoint to fetch home data by bounding box (returning dummy data for this version)
- [x] Simpler base map (than basic OSM tile server) for better contrast with POI and home pins
- [x] Split matching homes into a separate list from unmatched homes
- [x] Add unit anotation
- [x] Scale units to kilometers

v0.0.3: add real property data? Add browser extension to allow overlay of Zillow?
- [ ] CI/CD
- [ ] Add expiry time for database cache entries

v0.0.4: strict query language for filters, allowing text input with complex combinations instead of all OR statements
- [ ] Add OR logic with Overpass query unions

v0.1.0: 
- [ ] Hosting model
- [ ] Full CI/CD for all deployments, including production. Maybe use blue/green deployment process?
- [ ] A/B version testing?

v0.1.5: add data pipeline service(s) to ingest & pre-process datasets.
- [ ]

v0.1.6: refactor evaluation engine to use pre-processed datasets.
- [ ]

...

v1.0.0: snap versions of all APIs, docker images, installers, etc.
- [ ] Configure CI/CD for production release
- [ ] Domain name
- [ ] Update Caddyfile with production URL

Backlog:
- [ ] Migrate from Docker to podman
- [ ] Sensitivity analysis to determine constraining criteria
- [ ] Weights for each criterion
- [ ] Continuous scores for criteria
- [ ] Travel mode
- [ ] Convert freeform text to structured format
- [ ] Add brand and name search for Unknown categories