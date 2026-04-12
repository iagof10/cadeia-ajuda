using CadeiaAjuda.Web;
using CadeiaAjuda.Web.Components;
using CadeiaAjuda.Web.Hubs;
using CadeiaAjuda.Web.Services;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5012);
});

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

builder.Services.AddHttpClient<HelpRequestApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<HelpRequestHub>("/hubs/help-requests");

// --- BFF proxy endpoints for JavaScript pages ---

// /bff/me - returns current logged user info
app.MapGet("/bff/me", (AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    return user is null ? Results.Unauthorized() : Results.Ok(user);
}).DisableAntiforgery();

var bffTenants = app.MapGroup("/bff/tenants").DisableAntiforgery();
bffTenants.MapGet("/", async (TenantApiClient api) => Results.Ok(await api.GetAllAsync()));

var bffSectors = app.MapGroup("/bff/sectors").DisableAntiforgery();
bffSectors.MapGet("/", async (SectorApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

var bffUsers = app.MapGroup("/bff/users").DisableAntiforgery();
bffUsers.MapGet("/", async (UserApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

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

bffAreas.MapGet("/", async (AreaApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

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

// --- BFF: HelpRequestTypes (filtered by tenant) ---
var bffHelpRequestTypes = app.MapGroup("/bff/help-request-types").DisableAntiforgery();
bffHelpRequestTypes.MapGet("/", async (HelpRequestTypeApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

// --- BFF: HelpRequests ---
var bffHelpRequests = app.MapGroup("/bff/help-requests").DisableAntiforgery();

bffHelpRequests.MapGet("/", async (HelpRequestApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

bffHelpRequests.MapGet("/{id:guid}", async (Guid id, HelpRequestApiClient api) =>
{
    var item = await api.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

bffHelpRequests.MapPost("/", async (HelpRequestCreateModel model, HelpRequestApiClient api, IHubContext<HelpRequestHub> hub) =>
{
    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    if (response.IsSuccessStatusCode)
    {
        await hub.Clients.Group(model.TenantId.ToString()).SendAsync("HelpRequestsChanged");
    }
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

// Close help request using the logged-in user
bffHelpRequests.MapPatch("/{id:guid}/close", async (Guid id, HelpRequestApiClient api, AuthStateService auth, IHubContext<HelpRequestHub> hub) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();

    var response = await api.CloseAsync(id, new HelpRequestCloseModel { ClosedByUserId = user.Id });
    var body = await response.Content.ReadAsStringAsync();
    if (response.IsSuccessStatusCode)
    {
        await hub.Clients.Group(user.TenantId.ToString()).SendAsync("HelpRequestsChanged");
    }
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

// Close help request with login/password authentication
bffHelpRequests.MapPatch("/{id:guid}/close-with-auth", async (Guid id, HelpRequestCloseWithAuthModel model, HelpRequestApiClient api, AuthApiClient authApi, IHubContext<HelpRequestHub> hub, AuthStateService auth) =>
{
    var loggedUser = auth.GetCurrentUser();
    if (loggedUser is null) return Results.Unauthorized();

    var authResult = await authApi.LoginAsync(loggedUser.TenantIdentifier, model.Login, model.Password);
    if (!authResult.Success || authResult.User is null)
        return Results.Json(new { error = "Usuário ou senha inválidos." }, statusCode: 401);

    var response = await api.CloseAsync(id, new HelpRequestCloseModel { ClosedByUserId = authResult.User.Id });
    var body = await response.Content.ReadAsStringAsync();
    if (response.IsSuccessStatusCode)
    {
        await hub.Clients.Group(loggedUser.TenantId.ToString()).SendAsync("HelpRequestsChanged");
    }
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

app.MapDefaultEndpoints();

app.Run();
