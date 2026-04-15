using CadeiaAjuda.Web;
using CadeiaAjuda.Web.Components;
using CadeiaAjuda.Web.Hubs;
using CadeiaAjuda.Web.Services;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var isAspire = string.IsNullOrWhiteSpace(apiBaseUrl);

if (isAspire)
{
    apiBaseUrl = "https+http://apiservice";
    // Add service defaults & Aspire client integrations (only when running under Aspire).
    builder.AddServiceDefaults();
}
else
{
    // Standalone mode: use PORT env var or default 8080
    var port = builder.Configuration["PORT"] ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var apiBaseUri = new Uri(apiBaseUrl!);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = apiBaseUri;
    });

builder.Services.AddHttpClient<TenantApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<UserApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<SectorApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<HelpRequestTypeApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<EscalationLevelApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<AreaApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<ReasonApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<HelpRequestApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<DashboardApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<SessionApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<RoleApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<AndonSettingsApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
});

builder.Services.AddHttpClient<AndonUserSettingsApiClient>(client =>
{
    client.BaseAddress = apiBaseUri;
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

if (isAspire)
{
    app.MapStaticAssets();
}
else
{
    app.UseStaticFiles();
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<HelpRequestHub>("/hubs/help-requests");

// --- Redirect unknown pages to /not-found ---
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Skip static files, API, BFF, hubs, framework resources
    if (string.IsNullOrEmpty(path)
        || path == "/"
        || path.StartsWith("/not-found", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/_", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/bff/", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/app-assets/", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
        || path.Contains('.'))
    {
        await next();
        return;
    }

    var knownRoutes = new[]
    {
        "/home",
        "/dashboard",
        "/register/help-request-types", "/register/reasons", "/register/sectors",
        "/register/plants", "/register/escalation-levels",
        "/user/users", "/user/roles",
        "/andon", "/andon/settings",
        "/help-requests", "/help-requests/close",
        "/settings/company", "/settings/andon",
        "/tenants",
        "/login",
        "/auth",
        "/continue-to-login",
        "/counter", "/weather", "/Error"
    };

    var isKnown = knownRoutes.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase));

    if (!isKnown)
    {
        var auth = context.RequestServices.GetRequiredService<AuthStateService>();
        var user = auth.GetCurrentUser();
        if (user is null)
        {
            context.Response.Redirect("/");
            return;
        }
        context.Response.Redirect("/not-found");
        return;
    }

    await next();
});

// --- Auth guard: redirect to login if not authenticated ---
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Protected routes
    if (path.StartsWith("/home", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/dashboard", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/register", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/user", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/andon", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/reports", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/tenants", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/help-requests", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/settings", StringComparison.OrdinalIgnoreCase))
    {
        var auth = context.RequestServices.GetRequiredService<AuthStateService>();
        var user = auth.GetCurrentUser();

        if (user is null)
        {
            context.Response.Redirect("/");
            return;
        }

        // Andon users can only access /andon and /home
        if (user.UserType == UserType.Andon
            && !path.StartsWith("/andon", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/home", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/andon");
            return;
        }

        // Non-Andon users cannot access /andon
        if (user.UserType != UserType.Andon && path.StartsWith("/andon", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/home");
            return;
        }

        // Only Administrator can access /settings
        if (path.StartsWith("/settings", StringComparison.OrdinalIgnoreCase) && user.UserType != UserType.Administrator)
        {
            context.Response.Redirect("/home?error=forbidden");
            return;
        }

        var sessionApi = context.RequestServices.GetRequiredService<SessionApiClient>();
        var token = auth.GetSessionToken();

        if (string.IsNullOrEmpty(token))
        {
            auth.Clear();
            context.Response.Redirect("/");
            return;
        }

        var session = await sessionApi.ValidateSessionAsync(token);
        if (session is null || !session.IsActive)
        {
            auth.Clear();
            context.Response.Redirect("/");
            return;
        }

        // Permission check per route
        var requiredPermission = GetRequiredPermission(path);
        if (requiredPermission is not null)
        {
            // Administrator has full access
            // Andon users always have access to /andon
            if (user.UserType == UserType.Administrator) { }
            else if (user.UserType == UserType.Andon && path.StartsWith("/andon", StringComparison.OrdinalIgnoreCase)) { }
            else if (!user.Permissions.Contains(requiredPermission))
            {
                context.Response.Redirect("/home?error=forbidden");
                return;
            }
        }
    }

    await next();
});

static string? GetRequiredPermission(string path)
{
    if (path.StartsWith("/dashboard", StringComparison.OrdinalIgnoreCase)) return "dashboard.view";

    // Help Request Types - manage for new/edit
    if (path.Contains("/help-request-types/new", StringComparison.OrdinalIgnoreCase)
        || path.Contains("/help-request-types/edit", StringComparison.OrdinalIgnoreCase))
        return "help_request_types.manage";
    if (path.StartsWith("/register/help-request-types", StringComparison.OrdinalIgnoreCase)) return "help_request_types.view";

    // Reasons - manage for new/edit
    if (path.Contains("/reasons/new", StringComparison.OrdinalIgnoreCase)
        || path.Contains("/reasons/edit", StringComparison.OrdinalIgnoreCase))
        return "reasons.manage";
    if (path.StartsWith("/register/reasons", StringComparison.OrdinalIgnoreCase)) return "reasons.view";

    // Sectors - manage for new/edit
    if (path.Contains("/sectors/new", StringComparison.OrdinalIgnoreCase)
        || path.Contains("/sectors/edit", StringComparison.OrdinalIgnoreCase))
        return "sectors.manage";
    if (path.StartsWith("/register/sectors", StringComparison.OrdinalIgnoreCase)) return "sectors.view";

    // Areas/Plants
    if (path.StartsWith("/register/plants", StringComparison.OrdinalIgnoreCase)) return "areas.view";
    if (path.StartsWith("/register/areas", StringComparison.OrdinalIgnoreCase)) return "areas.view";

    // Users - manage for new/edit
    if (path.Contains("/users/new", StringComparison.OrdinalIgnoreCase)
        || path.Contains("/users/edit", StringComparison.OrdinalIgnoreCase))
        return "users.manage";
    if (path.StartsWith("/user/users", StringComparison.OrdinalIgnoreCase)) return "users.view";

    // Roles - manage for new/edit
    if (path.Contains("/roles/new", StringComparison.OrdinalIgnoreCase)
        || path.Contains("/roles/edit", StringComparison.OrdinalIgnoreCase))
        return "roles.manage";
    if (path.StartsWith("/user/roles", StringComparison.OrdinalIgnoreCase)) return "roles.view";

    if (path.StartsWith("/andon", StringComparison.OrdinalIgnoreCase)) return "andon.view";
    if (path.StartsWith("/reports", StringComparison.OrdinalIgnoreCase)) return "reports.view";
    if (path.StartsWith("/register/escalation", StringComparison.OrdinalIgnoreCase)) return "escalation.view";
    if (path.StartsWith("/help-requests/close", StringComparison.OrdinalIgnoreCase)) return "help_requests.close";
    if (path.StartsWith("/help-requests", StringComparison.OrdinalIgnoreCase)) return "help_requests.view";
    return null;
}

// --- Session validation middleware for BFF endpoints ---
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Only validate BFF endpoints (skip login, static assets, hubs, pages)
    if (path.StartsWith("/bff/", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/bff/auth/", StringComparison.OrdinalIgnoreCase))
    {
        var auth = context.RequestServices.GetRequiredService<AuthStateService>();
        var sessionApi = context.RequestServices.GetRequiredService<SessionApiClient>();

        var token = auth.GetSessionToken();
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Sessão não encontrada. Faça login novamente." });
            return;
        }

        var session = await sessionApi.ValidateSessionAsync(token);
        if (session is null || !session.IsActive)
        {
            auth.Clear();
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Sessão expirada ou encerrada em outro dispositivo." });
            return;
        }

        // Update last activity (fire and forget - don't block the request)
        _ = Task.Run(async () =>
        {
            try { await sessionApi.UpdateActivityAsync(token); } catch { }
        });
    }

    await next();
});

// --- BFF proxy endpoints for JavaScript pages ---

// --- Server-side Auth endpoints (run in HTTP context, cookies work) ---

app.MapPost("/continue-to-login", (HttpContext httpContext) =>
{
    var identifier = httpContext.Request.Form["Identifier"].ToString();
    if (string.IsNullOrWhiteSpace(identifier))
        return Results.Redirect("/");

    return Results.Redirect($"/login/{identifier.Trim()}");
}).DisableAntiforgery();

app.MapPost("/auth/do-login", async (HttpContext httpContext, AuthApiClient authApi, SessionApiClient sessionApi, AuthStateService auth) =>
{
    var form = httpContext.Request.Form;
    var tenantIdentifier = form["TenantIdentifier"].ToString();
    var login = form["Login"].ToString();
    var password = form["Password"].ToString();

    if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        return Results.Redirect($"/login/{tenantIdentifier}?error=empty");

    var result = await authApi.LoginAsync(tenantIdentifier, login, password);
    if (!result.Success || result.User is null)
        return Results.Redirect($"/login/{tenantIdentifier}?error=invalid");

    // Create session (invalidates ALL previous active sessions for this user)
    var ip = httpContext.Connection.RemoteIpAddress?.ToString();
    var ua = httpContext.Request.Headers.UserAgent.ToString();
    var session = await sessionApi.CreateSessionAsync(result.User.Id, result.User.TenantId, ip, ua);

    // Set cookies (HttpContext available here)
    auth.SetCurrentUser(result.User);
    if (session is not null)
    {
        auth.SetSessionToken(session.SessionToken);
    }

    // Andon users can only access /andon
    var redirectUrl = result.User.UserType == UserType.Andon ? "/andon" : "/home";
    return Results.Redirect(redirectUrl);
}).DisableAntiforgery();

app.MapPost("/auth/check-session", async (HttpContext httpContext, AuthApiClient authApi, SessionApiClient sessionApi) =>
{
    var dto = await httpContext.Request.ReadFromJsonAsync<BffLoginModel>();
    if (dto is null) return Results.BadRequest();

    var result = await authApi.LoginAsync(dto.TenantIdentifier, dto.Login, dto.Password);
    if (!result.Success || result.User is null)
        return Results.Json(new { valid = false });

    var hasActive = await sessionApi.HasActiveSessionAsync(result.User.Id);
    return Results.Ok(new { valid = true, hasActiveSession = hasActive });
}).DisableAntiforgery();

app.MapGet("/auth/logout", async (AuthStateService auth, SessionApiClient sessionApi) =>
{
    var user = auth.GetCurrentUser();
    var token = auth.GetSessionToken();

    if (!string.IsNullOrEmpty(token))
    {
        try { await sessionApi.InvalidateSessionAsync(token); } catch { }
    }

    auth.Clear();

    var identifier = user?.TenantIdentifier ?? "";
    return Results.Redirect($"/login/{identifier}");
}).DisableAntiforgery();

// --- BFF: Auth ---
var bffAuth = app.MapGroup("/bff/auth").DisableAntiforgery();

bffAuth.MapPost("/login", async (HttpContext httpContext, AuthApiClient authApi, SessionApiClient sessionApi, AuthStateService auth) =>
{
    var dto = await httpContext.Request.ReadFromJsonAsync<BffLoginModel>();
    if (dto is null) return Results.BadRequest();

    var result = await authApi.LoginAsync(dto.TenantIdentifier, dto.Login, dto.Password);
    if (!result.Success || result.User is null)
        return Results.Json(new { error = "Login ou senha inválidos." }, statusCode: 401);

    // Check for active session (unless user already confirmed)
    if (!dto.ForceLogin)
    {
        var hasActive = await sessionApi.HasActiveSessionAsync(result.User.Id);
        if (hasActive)
        {
            return Results.Json(new { activeSession = true, message = "Este usuário já possui uma sessão ativa em outro dispositivo. Deseja desconectar e continuar?" }, statusCode: 409);
        }
    }

    // Create session (invalidates any previous active session for this user)
    var ip = httpContext.Connection.RemoteIpAddress?.ToString();
    var ua = httpContext.Request.Headers.UserAgent.ToString();
    var session = await sessionApi.CreateSessionAsync(result.User.Id, result.User.TenantId, ip, ua);

    if (session is null)
        return Results.Json(new { error = "Erro ao criar sessão." }, statusCode: 500);

    // Set cookies
    auth.SetCurrentUser(result.User);
    auth.SetSessionToken(session.SessionToken);

    return Results.Ok(new { user = result.User, sessionToken = session.SessionToken, loginAt = session.LoginAt });
});

bffAuth.MapPost("/logout", async (AuthStateService auth, SessionApiClient sessionApi) =>
{
    var token = auth.GetSessionToken();
    if (!string.IsNullOrEmpty(token))
    {
        await sessionApi.InvalidateSessionAsync(token);
    }
    auth.Clear();
    return Results.Ok();
});

bffAuth.MapGet("/session", async (AuthStateService auth, SessionApiClient sessionApi) =>
{
    var token = auth.GetSessionToken();
    if (string.IsNullOrEmpty(token))
        return Results.Json(new { valid = false });

    var session = await sessionApi.ValidateSessionAsync(token);
    if (session is null || !session.IsActive)
        return Results.Json(new { valid = false });

    return Results.Ok(new { valid = true, loginAt = session.LoginAt, lastActivity = session.LastActivityAt });
});

// /bff/me - returns current logged user info
app.MapGet("/bff/me", (AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    return user is null ? Results.Unauthorized() : Results.Ok(user);
}).DisableAntiforgery();

var bffTenants = app.MapGroup("/bff/tenants").DisableAntiforgery();
bffTenants.MapGet("/", async (TenantApiClient api) => Results.Ok(await api.GetAllAsync()));

// /bff/tenant-settings
app.MapGet("/bff/tenant-settings", async (AuthStateService auth, IHttpClientFactory httpFactory) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    var http = httpFactory.CreateClient("ApiClient");
    var resp = await http.GetAsync($"/api/tenant-settings/{user.TenantId}");
    var body = await resp.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)resp.StatusCode);
}).DisableAntiforgery();

app.MapPut("/bff/tenant-settings", async (HttpContext httpContext, AuthStateService auth, IHttpClientFactory httpFactory) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator)
        return Results.Json(new { error = "Sem permissão para alterar configurações." }, statusCode: 403);

    var http = httpFactory.CreateClient("ApiClient");
    var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var resp = await http.PutAsync($"/api/tenant-settings/{user.TenantId}", content);
    var body = await resp.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)resp.StatusCode);
}).DisableAntiforgery();

// /bff/andon-settings
app.MapGet("/bff/andon-settings", async (AuthStateService auth, IHttpClientFactory httpFactory) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    var http = httpFactory.CreateClient("ApiClient");
    var resp = await http.GetAsync($"/api/andon-settings/{user.TenantId}");
    var body = await resp.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)resp.StatusCode);
}).DisableAntiforgery();

app.MapPut("/bff/andon-settings", async (HttpContext httpContext, AuthStateService auth, IHttpClientFactory httpFactory) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator)
        return Results.Json(new { error = "Sem permissão para alterar configurações." }, statusCode: 403);

    var http = httpFactory.CreateClient("ApiClient");
    var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var resp = await http.PutAsync($"/api/andon-settings/{user.TenantId}", content);
    var body = await resp.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)resp.StatusCode);
}).DisableAntiforgery();

// /bff/andon-user-settings
app.MapGet("/bff/andon-user-settings", async (AuthStateService auth, IHttpClientFactory httpFactory) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    var http = httpFactory.CreateClient("ApiClient");
    var resp = await http.GetAsync($"/api/andon-user-settings/{user.Id}");
    var body = await resp.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)resp.StatusCode);
}).DisableAntiforgery();

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

bffEscalation.MapPost("/", async (EscalationLevelSaveModel model, EscalationLevelApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("escalation.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar escalonamento." }, statusCode: 403);

    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffEscalation.MapPut("/{id:guid}", async (Guid id, EscalationLevelSaveModel model, EscalationLevelApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("escalation.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar escalonamento." }, statusCode: 403);

    var response = await api.UpdateAsync(id, model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffEscalation.MapDelete("/{id:guid}", async (Guid id, EscalationLevelApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("escalation.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar escalonamento." }, statusCode: 403);

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

bffAreas.MapPost("/", async (AreaFormModel model, AreaApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("areas.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar plantas/recursos." }, statusCode: 403);

    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffAreas.MapPut("/{id:guid}", async (Guid id, AreaFormModel model, AreaApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("areas.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar plantas/recursos." }, statusCode: 403);

    var response = await api.UpdateAsync(id, model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffAreas.MapPatch("/{id:guid}/toggle-active", async (Guid id, AreaApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("areas.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar plantas/recursos." }, statusCode: 403);

    var response = await api.ToggleActiveAsync(id);
    return response.IsSuccessStatusCode ? Results.Ok() : Results.NotFound();
});

bffAreas.MapDelete("/{id:guid}", async (Guid id, AreaApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("areas.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar plantas/recursos." }, statusCode: 403);

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

bffHelpRequests.MapPost("/", async (HelpRequestCreateModel model, HelpRequestApiClient api, IHubContext<HelpRequestHub> hub, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("help_requests.create"))
        return Results.Json(new { error = "Sem permissão para abrir chamados." }, statusCode: 403);

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
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("help_requests.close"))
        return Results.Json(new { error = "Sem permissão para encerrar chamados." }, statusCode: 403);

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

// --- BFF: Dashboard ---
app.MapGet("/bff/dashboard", async (DashboardApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    var data = await api.GetDashboardAsync(user.TenantId);
    return data is null ? Results.Problem("Erro ao carregar dashboard") : Results.Ok(data);
}).DisableAntiforgery();

// --- BFF: Roles & Permissions ---
var bffRoles = app.MapGroup("/bff/roles").DisableAntiforgery();

bffRoles.MapGet("/", async (RoleApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    return Results.Ok(await api.GetByTenantIdAsync(user.TenantId));
});

bffRoles.MapGet("/{id:guid}", async (Guid id, RoleApiClient api) =>
{
    var item = await api.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

bffRoles.MapPost("/", async (RoleSaveModel model, RoleApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("roles.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar perfis." }, statusCode: 403);

    model.TenantId = user.TenantId;
    var response = await api.CreateAsync(model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffRoles.MapPut("/{id:guid}", async (Guid id, RoleSaveModel model, RoleApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("roles.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar perfis." }, statusCode: 403);

    model.TenantId = user.TenantId;
    var response = await api.UpdateAsync(id, model);
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

bffRoles.MapDelete("/{id:guid}", async (Guid id, RoleApiClient api, AuthStateService auth) =>
{
    var user = auth.GetCurrentUser();
    if (user is null) return Results.Unauthorized();
    if (user.UserType != UserType.Administrator && !user.Permissions.Contains("roles.manage"))
        return Results.Json(new { error = "Sem permissão para gerenciar perfis." }, statusCode: 403);

    var response = await api.DeleteAsync(id);
    if (response.IsSuccessStatusCode) return Results.Ok();
    var body = await response.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
});

app.MapGet("/bff/permissions", async (RoleApiClient api) =>
    Results.Ok(await api.GetAllPermissionsAsync())).DisableAntiforgery();

if (isAspire)
{
    app.MapDefaultEndpoints();
}

app.Run();

record BffLoginModel(string TenantIdentifier, string Login, string Password, bool ForceLogin = false);

