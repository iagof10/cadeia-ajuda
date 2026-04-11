using CadeiaAjuda.ApiService.Application.Services;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// Database (Aspire integration - connection string provided by AppHost)
builder.AddNpgsqlDbContext<AppDbContext>("cadeiaajudadb");

// Repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISectorRepository, SectorRepository>();
builder.Services.AddScoped<IHelpRequestTypeRepository, HelpRequestTypeRepository>();
builder.Services.AddScoped<IEscalationLevelRepository, EscalationLevelRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IReasonRepository, ReasonRepository>();

// Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISectorService, SectorService>();
builder.Services.AddScoped<IHelpRequestTypeService, HelpRequestTypeService>();
builder.Services.AddScoped<IEscalationLevelService, EscalationLevelService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IReasonService, ReasonService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply pending migrations on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    //await DataSeeder.SeedAsync(db);
}

// ============ API Endpoints ============

// --- Tenants ---
var tenants = app.MapGroup("/api/tenants");

tenants.MapGet("/", async (ITenantService service) =>
    Results.Ok(await service.GetAllAsync()));

tenants.MapGet("/{id:guid}", async (Guid id, ITenantService service) =>
{
    var tenant = await service.GetByIdAsync(id);
    return tenant is null ? Results.NotFound() : Results.Ok(tenant);
});

tenants.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.TenantCreateDto dto, ITenantService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/tenants/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

tenants.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.TenantUpdateDto dto, ITenantService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

tenants.MapPatch("/{id:guid}/toggle-active", async (Guid id, ITenantService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

// --- Users ---
var users = app.MapGroup("/api/users");

users.MapGet("/", async (IUserService service) =>
    Results.Ok(await service.GetAllAsync()));

users.MapGet("/by-tenant/{tenantId:guid}", async (Guid tenantId, IUserService service) =>
    Results.Ok(await service.GetByTenantIdAsync(tenantId)));

users.MapGet("/{id:guid}", async (Guid id, IUserService service) =>
{
    var user = await service.GetByIdAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

users.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.UserCreateDto dto, IUserService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/users/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

users.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.UserUpdateDto dto, IUserService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

users.MapPatch("/{id:guid}/toggle-active", async (Guid id, IUserService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

// --- Sectors ---
var sectors = app.MapGroup("/api/sectors");

sectors.MapGet("/", async (ISectorService service) =>
    Results.Ok(await service.GetAllAsync()));

sectors.MapGet("/by-tenant/{tenantId:guid}", async (Guid tenantId, ISectorService service) =>
    Results.Ok(await service.GetByTenantIdAsync(tenantId)));

sectors.MapGet("/{id:guid}", async (Guid id, ISectorService service) =>
{
    var sector = await service.GetByIdAsync(id);
    return sector is null ? Results.NotFound() : Results.Ok(sector);
});

sectors.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.SectorCreateDto dto, ISectorService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/sectors/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

sectors.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.SectorUpdateDto dto, ISectorService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

sectors.MapPatch("/{id:guid}/toggle-active", async (Guid id, ISectorService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

// --- HelpRequestTypes ---
var helpRequestTypes = app.MapGroup("/api/help-request-types");

helpRequestTypes.MapGet("/", async (IHelpRequestTypeService service) =>
    Results.Ok(await service.GetAllAsync()));

helpRequestTypes.MapGet("/by-tenant/{tenantId:guid}", async (Guid tenantId, IHelpRequestTypeService service) =>
    Results.Ok(await service.GetByTenantIdAsync(tenantId)));

helpRequestTypes.MapGet("/{id:guid}", async (Guid id, IHelpRequestTypeService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

helpRequestTypes.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.HelpRequestTypeCreateDto dto, IHelpRequestTypeService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/help-request-types/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

helpRequestTypes.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.HelpRequestTypeUpdateDto dto, IHelpRequestTypeService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

helpRequestTypes.MapPatch("/{id:guid}/toggle-active", async (Guid id, IHelpRequestTypeService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

// --- EscalationLevels ---
var escalationLevels = app.MapGroup("/api/escalation-levels");

escalationLevels.MapGet("/", async (IEscalationLevelService service) =>
    Results.Ok(await service.GetAllAsync()));

escalationLevels.MapGet("/by-sector/{sectorId:guid}", async (Guid sectorId, IEscalationLevelService service) =>
    Results.Ok(await service.GetBySectorIdAsync(sectorId)));

escalationLevels.MapGet("/{id:guid}", async (Guid id, IEscalationLevelService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

escalationLevels.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.EscalationLevelCreateDto dto, IEscalationLevelService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/escalation-levels/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

escalationLevels.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.EscalationLevelUpdateDto dto, IEscalationLevelService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

escalationLevels.MapDelete("/{id:guid}", async (Guid id, IEscalationLevelService service) =>
{
    var result = await service.DeleteAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

// --- Areas ---
var areas = app.MapGroup("/api/areas");

areas.MapGet("/", async (IAreaService service) =>
    Results.Ok(await service.GetAllAsync()));

areas.MapGet("/by-tenant/{tenantId:guid}", async (Guid tenantId, IAreaService service) =>
    Results.Ok(await service.GetByTenantIdAsync(tenantId)));

areas.MapGet("/{id:guid}", async (Guid id, IAreaService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

areas.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.AreaCreateDto dto, IAreaService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/areas/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

areas.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.AreaUpdateDto dto, IAreaService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

areas.MapPatch("/{id:guid}/toggle-active", async (Guid id, IAreaService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

areas.MapDelete("/{id:guid}", async (Guid id, IAreaService service) =>
{
    try
    {
        var result = await service.DeleteAsync(id);
        return result ? Results.Ok() : Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// --- Auth ---
app.MapPost("/api/auth/login", async (CadeiaAjuda.ApiService.Application.DTOs.LoginDto dto, IUserService service) =>
{
    var user = await service.AuthenticateAsync(dto);
    return user is null
        ? Results.Unauthorized()
        : Results.Ok(user);
});

// --- Reasons ---
var reasons = app.MapGroup("/api/reasons");

reasons.MapGet("/", async (IReasonService service) =>
    Results.Ok(await service.GetAllAsync()));

reasons.MapGet("/by-tenant/{tenantId:guid}", async (Guid tenantId, IReasonService service) =>
    Results.Ok(await service.GetByTenantIdAsync(tenantId)));

reasons.MapGet("/{id:guid}", async (Guid id, IReasonService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

reasons.MapPost("/", async (CadeiaAjuda.ApiService.Application.DTOs.ReasonCreateDto dto, IReasonService service) =>
{
    try
    {
        var created = await service.CreateAsync(dto);
        return Results.Created($"/api/reasons/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

reasons.MapPut("/{id:guid}", async (Guid id, CadeiaAjuda.ApiService.Application.DTOs.ReasonUpdateDto dto, IReasonService service) =>
{
    dto.Id = id;
    try
    {
        var updated = await service.UpdateAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

reasons.MapPatch("/{id:guid}/toggle-active", async (Guid id, IReasonService service) =>
{
    var result = await service.ToggleActiveAsync(id);
    return result ? Results.Ok() : Results.NotFound();
});

app.MapDefaultEndpoints();

app.Run();