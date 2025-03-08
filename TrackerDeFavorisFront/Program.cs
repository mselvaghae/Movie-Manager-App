using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using TrackerDeFavorisFront.Components;
using TrackerDeFavorisFront.Services;

namespace TrackerDeFavorisFront;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddAuthenticationCore();

        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<FilmService>();
        builder.Services.AddSingleton<OmdbService>();
        builder.Services.AddSingleton<FavoriteService>();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();

        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "auth_cookie";
                options.LoginPath = "/connection"; // Redirection vers la page de connexion
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.UseStatusCodePagesWithRedirects("/Error/{0}");

        app.UseAuthentication();
        app.UseAuthorization();
        app.Run();
    }
}
