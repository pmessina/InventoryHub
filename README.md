# InventoryHub

A .NET 10 Blazor Server app that exposes a cached `/api/products` minimal API and consumes it through a typed `HttpClient`.

## Features
- Blazor interactive server UI
- Minimal API endpoint: `GET /api/products`
- Back-end caching: output caching (1 minute) plus in-memory `HybridCache`
- CORS enabled

## Requirements
- .NET 10 SDK

## Run
```
dotnet run --project InventoryHub
```

## Configuration
`appsettings.Development.json`:
- `ApiBaseUrl`: base address used by `ApiService` (default `https://localhost:7104/`)

## Notes
See [Reflection.md](InventoryHub/Reflection.md) for development notes.
