# ADR-001: Azure Functions for Backend Service

## Status

Accepted

## Date

2025-07-17

## Context

The HslBikeApp frontend (Blazor WASM on GitHub Pages) needs a backend service to:

1. **Protect the HSL Digitransit API key** — the frontend is a static site; secrets cannot be stored client-side.
2. **Aggregate bike availability data** — poll stations periodically, store snapshots, and compute hourly availability profiles.
3. **Serve precomputed analytics** — popular destinations, trend arrows, and hourly graphs should be fast to fetch.

Options considered:

- **Azure Functions (Consumption plan)** — serverless, pay-per-use, free tier covers expected traffic.
- **Azure App Service (Free/Basic)** — always-on but limited free tier, more operational overhead.
- **Self-hosted on a VM** — maximum control but highest cost and maintenance.

## Decision

Use **Azure Functions (isolated worker model, .NET 10)** on the **Consumption plan**.

### Architecture: Write/Read Separation via Blob Storage

```
Timer (every 2 min)          HTTP Functions (on demand)
       │                              │
       ▼                              ▼
  Poll HSL API               Read from Blob Storage
       │                              │
       ▼                              │
  Write to Azure              Return cached JSON
  Blob Storage                (sub-second response)
```

- A **timer-triggered function** polls HSL Digitransit every 2 minutes, aggregates data, and writes JSON to Azure Blob Storage.
- **HTTP-triggered functions** read precomputed JSON from blob storage, avoiding heavy computation on each request.
- This decouples data freshness from request latency.

### Hybrid Fallback for Cold Start

The frontend uses a progressive loading strategy:

1. **Immediate**: Frontend fetches station data directly from HSL Digitransit (current behavior, no backend dependency).
2. **Background**: Frontend calls the aggregator for enriched data (snapshots, trends).
3. **Progressive**: Hourly graphs and popular destinations load when the aggregator responds.

Basic station view is never blocked by the aggregator's cold start.

## Consequences

### Positive

- Zero cost within Azure free tier (~21,600 timer executions + HTTP calls per month).
- API key stays in Azure Functions app settings, never exposed to the client.
- Sub-second HTTP responses since functions only read from blob storage.
- Hybrid fallback ensures the app is usable even if the aggregator is cold.

### Negative

- Cold start latency of 5–15 seconds for the first HTTP request after idle.
- Timer trigger does not prevent HTTP function cold starts (different function instances).
- Data freshness limited to the polling interval (2 minutes).

### Risks

- If the free tier limits change, costs could increase.
- Blob storage adds a dependency; if blob is unavailable, HTTP functions return stale or empty data.
