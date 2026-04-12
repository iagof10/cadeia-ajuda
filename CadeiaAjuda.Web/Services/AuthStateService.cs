using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CadeiaAjuda.Web.Services;

public class AuthStateService
{
    private const string CookieName = "CadeiaAjuda.Auth";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthStateService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public LoggedUser? GetCurrentUser()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null) return null;

        if (!context.Request.Cookies.TryGetValue(CookieName, out var cookieValue))
            return null;

        try
        {
            var json = Convert.FromBase64String(cookieValue);
            return JsonSerializer.Deserialize<LoggedUser>(json);
        }
        catch
        {
            return null;
        }
    }

    public void SetCurrentUser(LoggedUser user)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null) return;

        var json = JsonSerializer.SerializeToUtf8Bytes(user);
        var base64 = Convert.ToBase64String(json);

        var isHttps = context.Request.IsHttps;

        context.Response.Cookies.Append(CookieName, base64, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.Strict : SameSiteMode.Lax,
            IsEssential = true,
            MaxAge = TimeSpan.FromHours(8)
        });
    }

    public void Clear()
    {
        var context = _httpContextAccessor.HttpContext;
        context?.Response.Cookies.Delete(CookieName);
    }
}
