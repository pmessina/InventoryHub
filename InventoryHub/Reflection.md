# Reflection: Using GitHub Copilot on InventoryHub

InventoryHub is a Blazor Server front end (`ClientApp.ApiService`, Razor components) that calls a minimal API endpoint, `/api/products`, hosted in the same .NET 10 project. This reflection covers how Copilot helped in each part of building it, the problems I hit, and what I learned.

## Integration code

- Copilot helped wire the front end to the back end: a typed `HttpClient` (`AddHttpClient<ApiService>`) with a `BaseAddress` read from the `ApiBaseUrl` setting, so `ApiService` can request the relative path `api/products`.
- It also added CORS (`AddCors` with a default policy) and the minimal API mapping (`MapGet("/api/products", ...)`) that delegates to a `ProductCatalog` service.
- Keeping the endpoint thin and putting the data and caching logic in `ProductCatalog` made the integration code easy to read and test.

## Debugging

Two issues came up while debugging. In both, the exception text and the debugger context were enough for Copilot to find the cause.

1. **Invalid configuration JSON.**
   - The app failed at `WebApplication.CreateBuilder` with an `InvalidDataException`. The inner `JsonReaderException` said `'"' is invalid after a value` at line 5.
   - Copilot opened `appsettings.Development.json` and found two syntax errors. A comma was missing after `ApiBaseUrl`, and `"Logging"` was missing its `: {`. I fixed both.
2. **Redis connection timeout.**
   - `StackExchange.Redis` threw `ConnectTimeout` on `HMSET`. The fields `conn-sec: n/a` and `rs: NotStarted` showed that no connection was ever made.
   - Copilot diagnosed this as an environment problem rather than a code bug: nothing was listening on `localhost:6379`. It listed likely causes (Redis not running, Docker/WSL port not published, wrong host or credentials) and ways to check them.

## Structuring JSON responses

- `ProductCatalog` returns `Product[]`, and each product has a nested `Category`. The minimal API serializes this to JSON automatically, so the response shape is defined by the model classes.
- Copilot helped keep the model and the response aligned. The front end deserializes the same shape it receives, so there is a single shared contract.
- Sample data in `LoadProducts` (a laptop and headphones, with ids, prices, stock and categories) gave realistic test responses early.

## Performance optimization

- I first added `HybridCache` with a distributed Redis cache behind it. After the timeout problem, I decided to drop Redis.
- The final design has two in-memory layers:
  - **Output caching** on `/api/products`. A `Products` policy keeps whole responses for 1 minute and is tagged `products`, so entries can be evicted later. Repeat requests skip the endpoint and the service.
  - **`HybridCache`** in `ProductCatalog`, keyed `products:all` with a 1-minute expiry. It also prevents cache stampedes, because concurrent requests for the same key share one load.
- I put `UseOutputCache` after `UseCors` so CORS headers are applied to cached responses correctly.
- I have not measured the gain. A fair test would call the endpoint repeatedly and compare response times and load counts with and without the caches.

## Challenges and how Copilot helped

| Challenge | How it was resolved |
| --- | --- |
| Malformed `appsettings.Development.json` crashed startup | Copilot read the parser error's line number, inspected the file, and fixed both syntax errors. |
| Redis timeouts, with no Redis server available | Copilot explained the cause and the checks to run, then helped remove the dependency instead of fighting the environment. |
| Removing Redis cleanly | Copilot changed `Program.cs`, the `.csproj` package reference, and the app settings together, then searched for leftover references. |
| A broken edit during the refactor | One of Copilot's edits cut off the `(o =>` part of `AddHybridCache`. The build caught it right away and a follow-up edit fixed it. |

## What I learned about using Copilot

- **Give it concrete evidence.** Pasting the exact exception, stack trace and debugger state produced precise answers. Vague descriptions would not have.
- **Say what you want and what you don't.** Telling it to revert from Redis, and choosing the options it offered (output caching, 1-minute duration), kept the changes small and on target.
- **Review the plan before the edits.** Seeing the steps first made it easy to check the scope across `Program.cs`, the `.csproj` and the settings.
- **Always build and test.** The build caught a bad edit immediately. Copilot's output needs the same verification as any other code, and I still need to run the app to confirm the caching works.
- **Separate code bugs from environment problems.** The Redis error looked like a code failure but was not. Copilot helped tell them apart, which saved time.
- **Full-stack work benefits from one view.** Configuration, DI registration, middleware order, the API contract and the client call all interact. Copilot working across those files at once was most useful where a change touched several of them.
- **It doesn't replace judgment.** I still decided on the architecture, such as in-memory caching over Redis, and on the cache lifetimes.
