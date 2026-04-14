using System.Security.Cryptography;
using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedTenantAndAdminUserAsync(db);
    }

    private static async Task SeedTenantAndAdminUserAsync(AppDbContext db)
    {
        const string tenantIdentifier = "admin";
        const string tenantName = "Empresa Padrão";
        const string adminLogin = "admin";
        const string adminPassword = "admin123";
        const string adminName = "Administrador";
        const string adminEmail = "admin@cadeiaajuda.com";

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Identifier == tenantIdentifier);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Name = tenantName,
                TradeName = tenantName,
                Cnpj = "00000000000000",
                Email = adminEmail,
                Phone = "0000000000",
                Identifier = tenantIdentifier,
                Active = true
            };

            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
        }

        var userExists = await db.Users.AnyAsync(u => u.TenantId == tenant.Id && u.Login == adminLogin);

        if (!userExists)
        {
            var user = new User
            {
                Name = adminName,
                Email = adminEmail,
                Phone = "0000000000",
                Login = adminLogin,
                PasswordHash = HashPassword(adminPassword),
                TenantId = tenant.Id,
                Active = true,
                UserType = UserType.Standard
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            Console.WriteLine("============================================");
            Console.WriteLine("  SEED: Usuário administrador criado!");
            Console.WriteLine($"  Tenant Identifier: {tenantIdentifier}");
            Console.WriteLine($"  Login: {adminLogin}");
            Console.WriteLine($"  Senha: {adminPassword}");
            Console.WriteLine("============================================");
        }
    }

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }
}
