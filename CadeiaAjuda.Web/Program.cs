using CadeiaAjuda.Web;
using CadeiaAjuda.Web.Components;
using CadeiaAjuda.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddHttpClient<TenantApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<UserApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<SectorApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<HelpRequestTypeApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<EscalationLevelApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<AreaApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<ReasonApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthStateService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- BFF proxy endpoints for JavaScript pages ---
var bffTenants = app.MapGroup("/bff/tenants").DisableAntiforgery();
bffTenants.MapGet("/", async (TenantApiClient api) => Results.Ok(await api.GetAllAsync()));

var bffSectors = app.MapGroup("/bff/sectors").DisableAntiforgery();
bffSectors.MapGet("/", async (SectorApiClient api) => Results.Ok(await api.GetAllAsync()));

var bffUsers = app.MapGroup("/bff/users").DisableAntiforgery();
bffUsers.MapGet("/", async (UserApiClient api) => Results.Ok(await api.GetAllAsync()));

var bffEscalation = app.MapGroup("/bff/escalation-levels").DisableAntiforgery();
bffEscalation.MapGet("/by-sector/{sectorId:guid}", async (Guid sectorId, EscalationLevelApiClient api) =>
    Results.Ok(await api.GetBySectorIdAsync(sectorId)));

bffEscalation.MapGet("/{id:guid}", async (Guid id, EscalationLevelApiClient api) =>
{
    var item = await api.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

bffEscalation.MapPost("/", async (EscalationLevelSaveModel model, EscalationLevelApiClient api) =>
{
    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffEscalation.MapPut("/{id:guid}", async (Guid id, EscalationLevelSaveModel model, EscalationLevelApiClient api) =>
{
    var response = await api.UpdateAsync(id, model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffEscalation.MapDelete("/{id:guid}", async (Guid id, EscalationLevelApiClient api) =>
{
    var response = await api.DeleteAsync(id);
    return response.IsSuccessStatusCode ? Results.Ok() : Results.NotFound();
});

// --- BFF: Areas ---
var bffAreas = app.MapGroup("/bff/areas").DisableAntiforgery();

bffAreas.MapGet("/", async (AreaApiClient api) =>
    Results.Ok(await api.GetAllAsync()));

bffAreas.MapGet("/{id:guid}", async (Guid id, AreaApiClient api) =>
{
    var item = await api.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

bffAreas.MapPost("/", async (AreaFormModel model, AreaApiClient api) =>
{
    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffAreas.MapPut("/{id:guid}", async (Guid id, AreaFormModel model, AreaApiClient api) =>
{
    var response = await api.UpdateAsync(id, model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffAreas.MapPatch("/{id:guid}/toggle-active", async (Guid id, AreaApiClient api) =>
{
    var response = await api.ToggleActiveAsync(id);
    return response.IsSuccessStatusCode ? Results.Ok() : Results.NotFound();
});

bffAreas.MapDelete("/{id:guid}", async (Guid id, AreaApiClient api) =>
{
    var response = await api.DeleteAsync(id);
    if (response.IsSuccessStatusCode) return Results.Ok();
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

app.MapDefaultEndpoints();

app.Run();
