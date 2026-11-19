using ClientApp;
using InventoryHub;
using InventoryHub.Components;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<ApiService>(client =>
{
    // Required: ApiService requests a relative path ("api/products").
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7104/");
});

builder.Services.AddOutputCache(o => o.AddPolicy("Products", p => p.Expire(TimeSpan.FromMinutes(1)).Tag("products")));
builder.Services.AddHybridCache(o =>
{
    o.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(1),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddSingleton<ProductCatalog>();

var app = builder.Build();

app.UseCors();
app.UseOutputCache();

app.MapGet("/api/products", (ProductCatalog catalog, CancellationToken ct) => catalog.GetProductsAsync(ct))
    .CacheOutput("Products");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
