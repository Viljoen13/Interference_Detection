# Interference Detection Service

[![CI](https://github.com/Viljoen13/Interference_Detection/actions/workflows/ci.yml/badge.svg)](https://github.com/Viljoen13/Interference_Detection/actions/workflows/ci.yml)
[![CodeQL](https://github.com/Viljoen13/Interference_Detection/actions/workflows/codeql.yml/badge.svg)](https://github.com/Viljoen13/Interference_Detection/actions/workflows/codeql.yml)

A small, clean-architecture **.NET 8 Minimal API** that models LTE and 5G NR cells, persists them to **PostgreSQL** via **EF Core**, and exposes telecoms analytics — distance, coverage overlap, and **RF interference detection** (PCI collisions, co-channel overlap).

The point of the project is to demonstrate the **SOLID principles** in a realistic but small codebase — every layer is a deliberate example of one or more principles, not an academic toy.

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│  InterferenceDetection.Api               (Minimal API)           │
│    └─ depends on Application + Infrastructure (composition root) │
├──────────────────────────────────────────────────────────────────┤
│  InterferenceDetection.Infrastructure                            │
│    EF Core / Npgsql / Repositories — implements abstractions     │
├──────────────────────────────────────────────────────────────────┤
│  InterferenceDetection.Application                               │
│    Abstractions (interfaces) + Services + DTOs + Mapping         │
├──────────────────────────────────────────────────────────────────┤
│  InterferenceDetection.Domain                                    │
│    Entities, Value Objects, Enums — no framework dependencies    │
└──────────────────────────────────────────────────────────────────┘
```

Reference direction is strict: outer layers depend on inner layers, never the other way around. The Domain project has **zero NuGet dependencies**.

---

## SOLID, line by line

### S — Single Responsibility
Each service does one thing:

| Type | Responsibility |
|------|----------------|
| `HaversineDistanceCalculator` | Compute great-circle distance between two coordinates |
| `CircularCoverageAnalyzer`    | Decide whether two cell coverage circles overlap |
| `FrequencyInterferenceDetector` | Identify pairs of cells that may interfere on the RF layer |
| `CellRepository`              | Persist and retrieve cells |
| `CellMapper`                  | Map domain entities to DTOs |

No "manager" or "service" class doing four things at once.

### O — Open/Closed
- Add a new technology (e.g. `NbIotCell`) by creating a new `Cell` subclass and one branch in `FrequencyInterferenceDetector.GetInterferenceReason`. The rest of the system — repository, endpoints, analyzers — is untouched.
- Add a new distance algorithm (e.g. Vincenty) by writing a class that implements `IDistanceCalculator` and changing one DI line.

### L — Liskov Substitution
`LteCell` and `FiveGCell` both substitute cleanly for `Cell`:
- `ICellRepository.GetAllAsync()` returns `IReadOnlyList<Cell>` and analytics operate on the base type.
- `ICoverageAnalyzer.Analyze(Cell, Cell)` works regardless of subtype.

### I — Interface Segregation
Instead of one fat `ICellService`, there are four small interfaces:
`ICellRepository`, `IDistanceCalculator`, `ICoverageAnalyzer`, `IInterferenceDetector`.
A caller that just needs distances depends on `IDistanceCalculator` and nothing else.

### D — Dependency Inversion
- `Application` defines abstractions; `Infrastructure` implements them.
- The API project knows about abstractions and uses `AddInfrastructure(...)` (a single DI extension) to bind them.
- `AppDbContext` implements `IUnitOfWork` so the Application layer never references EF Core.

---

## Telecoms model

### Domain entities
- `Cell` (abstract) — `Id`, `Name`, `Location`, `CoverageRadiusMeters`, `TransmitPowerDbm`
- `LteCell : Cell` — `Earfcn`, `PhysicalCellId` (0–503), `TrackingAreaCode`
- `FiveGCell : Cell` — `Nrarfcn`, `PhysicalCellId` (0–1007), `GnbId`, `TrackingAreaCode`
- `GeoCoordinate` — immutable value object with latitude/longitude bounds validation

Persisted using **EF Core Table-Per-Hierarchy** (`technology` discriminator column).

### Interference rules
Two cells are reported as interfering if:
1. They share **the same channel** (EARFCN for LTE, NR-ARFCN for 5G), **and**
2. Their coverage circles overlap.

A finding is escalated to a **PCI collision** when the Physical Cell ID also matches — that's a hard configuration error in a real network.

---

## Running it

### 1. Start PostgreSQL

```bash
docker compose up -d postgres
```

The container exposes Postgres on `localhost:5434` (host port 5434 → container port 5432, to avoid clashing with any system Postgres on 5432). Database `interference_detection`, user/password `telecom`/`telecom`. The connection string in `appsettings.json` already matches.

### 2. Run the API

```bash
dotnet run --project src/InterferenceDetection.Api
```

On first startup the API will:
1. Apply the EF Core migration (creates the `cells` table).
2. Seed six demo cells around Cape Town — including two pairs deliberately configured to trigger interference findings.

Swagger UI is available at `https://localhost:<port>/swagger`.

### 3. Try the analytics

```bash
# list all cells
curl http://localhost:5000/cells

# distance between two cells
curl "http://localhost:5000/analytics/distance?cellA=11111111-1111-1111-1111-111111111111&cellB=22222222-2222-2222-2222-222222222222"

# all interference findings in the network
curl http://localhost:5000/analytics/interference
```

### 4. Run the tests

```bash
dotnet test
```

---

## API surface

| Method | Route | Purpose |
|--------|-------|---------|
| `GET`    | `/cells`                | List all cells (polymorphic JSON, discriminated by `technology`) |
| `GET`    | `/cells/{id}`           | Get a cell |
| `GET`    | `/cells/lte`            | List LTE cells only |
| `GET`    | `/cells/5g`             | List 5G cells only |
| `POST`   | `/cells/lte`            | Create an LTE cell |
| `POST`   | `/cells/5g`             | Create a 5G cell |
| `DELETE` | `/cells/{id}`           | Delete a cell |
| `GET`    | `/analytics/distance?cellA=&cellB=`         | Distance in meters |
| `GET`    | `/analytics/coverage-overlap?cellA=&cellB=` | Coverage overlap |
| `GET`    | `/analytics/interference`                   | Network-wide interference scan |

---

## Extending the project

Everything below is a worked-out example of how easily the architecture absorbs new functionality:

- **Add a new cell type** (NB-IoT, Wi-Fi AP) — subclass `Cell`, add EF discriminator value, add one switch arm in the interference detector.
- **Swap the distance algorithm** — implement `IDistanceCalculator`, replace the DI registration. No other file changes.
- **Sector-aware coverage** — replace `CircularCoverageAnalyzer` with a beam/sector-shape analyzer that still implements `ICoverageAnalyzer`.
- **PostGIS / spatial queries** — change only the repository and migration; the Application and API layers don't move.
- **Background interference monitoring** — add a `BackgroundService` that resolves `IInterferenceDetector` and pushes findings to a queue.

---

## Tech stack

- .NET 8 (Minimal APIs)
- EF Core 8 + Npgsql (Postgres provider)
- EFCore.NamingConventions (snake_case)
- xUnit, FluentAssertions, NSubstitute
- Swashbuckle (OpenAPI / Swagger UI)
- PostgreSQL 16 via Docker
