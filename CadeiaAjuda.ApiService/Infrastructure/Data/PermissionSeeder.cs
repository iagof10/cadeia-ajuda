using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Data;

public static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(AppDbContext db)
    {
        var definitions = GetPermissionDefinitions();
        var existing = await db.Permissions.Select(p => p.Key).ToListAsync();

        foreach (var def in definitions)
        {
            if (!existing.Contains(def.Key))
            {
                db.Permissions.Add(new Permission
                {
                    Key = def.Key,
                    Name = def.Name,
                    Group = def.Group,
                    SortOrder = def.SortOrder
                });
            }
        }

        await db.SaveChangesAsync();
    }

    public static List<PermissionDefinition> GetPermissionDefinitions() =>
    [
        // Dashboard
        new("dashboard.view", "Visualizar Dashboard", "Dashboard", 100),

        // Cadastro
        new("help_request_types.view", "Visualizar Tipos de Ajuda", "Cadastro", 200),
        new("help_request_types.manage", "Gerenciar Tipos de Ajuda", "Cadastro", 201),

        new("reasons.view", "Visualizar Motivos", "Cadastro", 210),
        new("reasons.manage", "Gerenciar Motivos", "Cadastro", 211),

        new("sectors.view", "Visualizar Setores", "Cadastro", 220),
        new("sectors.manage", "Gerenciar Setores", "Cadastro", 221),

        new("areas.view", "Visualizar Plantas/Recursos", "Cadastro", 230),
        new("areas.manage", "Gerenciar Plantas/Recursos", "Cadastro", 231),

        // Cadeia de Ajuda
        new("help_requests.view", "Visualizar Cadeia de Ajuda", "Chamados", 300),
        new("help_requests.create", "Abrir Chamado", "Chamados", 301),
        new("help_requests.close", "Encerrar Chamado", "Chamados", 302),

        // Andon
        new("andon.view", "Visualizar Andon", "Andon", 400),

        // Usuários
        new("users.view", "Visualizar Usuários", "Administração", 500),
        new("users.manage", "Gerenciar Usuários", "Administração", 501),

        // Perfis/Roles
        new("roles.view", "Visualizar Perfis", "Administração", 510),
        new("roles.manage", "Gerenciar Perfis e Permissões", "Administração", 511),

        // Relatórios
        new("reports.view", "Visualizar Relatórios", "Relatórios", 600),

        // Escalation
        new("escalation.view", "Visualizar Escalonamento", "Cadastro", 240),
        new("escalation.manage", "Gerenciar Escalonamento", "Cadastro", 241),

        // Configuração da Empresa
        new("company.view", "Visualizar Configuração da Empresa", "Configuração", 700),
        new("company.manage", "Gerenciar Configuração da Empresa", "Configuração", 701),
    ];

    public record PermissionDefinition(string Key, string Name, string Group, int SortOrder);
}
