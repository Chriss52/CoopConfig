namespace CoopCloud.GeneralSettingsAndAuth.Examples;

/// <summary>
/// Ejemplos de cómo usar el cliente OAuth en diferentes escenarios.
/// </summary>
public static class UsageExamples
{
    // ==================== EJEMPLO 1: Aplicación de Consola ====================

    /// <summary>
    /// Ejemplo para aplicación de consola o servicio.
    /// Abre el navegador para login y espera el callback.
    /// </summary>
    public static async Task ConsoleAppExample()
    {
        var options = new OAuthClientOptions
        {
            AuthorityUrl = "https://localhost:5001", // Tu servidor de auth
            ClientId = "mi-app-consola",
            ClientSecret = "mi-secret", // Solo si es cliente confidencial
            RedirectUri = "http://localhost:8080/callback",
            Scopes = ["openid", "profile", "email", "roles"]
        };

        var httpClient = new HttpClient();
        var oauthClient = new OAuthClient(httpClient, options);

        // 1. Generar URL de autorización
        var authRequest = oauthClient.BuildAuthorizationUrl();
        Console.WriteLine($"Abre esta URL en el navegador:\n{authRequest.Url}");

        // 2. Esperar el código (en una app real, levantar un servidor HTTP temporal)
        Console.Write("\nIngresa el código de autorización: ");
        var code = Console.ReadLine()!;

        // 3. Intercambiar código por tokens
        var tokens = await oauthClient.ExchangeCodeAsync(code, authRequest.CodeVerifier);
        Console.WriteLine($"\nAccess Token: {tokens.AccessToken[..50]}...");
        Console.WriteLine($"Expires in: {tokens.ExpiresIn} seconds");

        // 4. Obtener información del usuario
        var userInfo = await oauthClient.GetUserInfoAsync(tokens.AccessToken);
        Console.WriteLine($"\nUsuario: {userInfo.Name} ({userInfo.Email})");
        Console.WriteLine($"Roles: {string.Join(", ", userInfo.Roles ?? [])}");

        // 5. Renovar token cuando expire
        if (!string.IsNullOrEmpty(tokens.RefreshToken))
        {
            var newTokens = await oauthClient.RefreshTokenAsync(tokens.RefreshToken);
            Console.WriteLine($"\nNuevo Access Token: {newTokens.AccessToken[..50]}...");
        }

        // 6. Logout
        var logoutUrl = oauthClient.BuildLogoutUrl(tokens.IdToken, "http://localhost:8080/logged-out");
        Console.WriteLine($"\nURL de logout: {logoutUrl}");
    }

    // ==================== EJEMPLO 2: API Client (llamar APIs protegidas) ====================

    /// <summary>
    /// Ejemplo de cómo llamar una API que está protegida por tu servidor de auth.
    /// </summary>
    public static async Task ApiClientExample(string accessToken)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Llamar a una API protegida
        var response = await httpClient.GetAsync("https://mi-api-protegida.com/api/datos");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Datos: {data}");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Token expirado - necesitas renovar con refresh_token");
        }
    }

    // ==================== EJEMPLO 3: Blazor WebAssembly ====================

    /// <summary>
    /// Configuración para Blazor WebAssembly (cliente público).
    /// En Program.cs de tu app Blazor:
    /// </summary>
    public static void BlazorWasmSetup()
    {
        /*
        // Program.cs de Blazor WebAssembly

        builder.Services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.Authority = "https://tu-servidor-auth.com";
            options.ProviderOptions.ClientId = "mi-blazor-app";
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.DefaultScopes.Add("openid");
            options.ProviderOptions.DefaultScopes.Add("profile");
            options.ProviderOptions.DefaultScopes.Add("email");
        });
        */
    }

    // ==================== EJEMPLO 4: ASP.NET Core MVC/Razor Pages ====================

    /// <summary>
    /// Configuración para ASP.NET Core con páginas/vistas.
    /// En Program.cs:
    /// </summary>
    public static void AspNetCoreMvcSetup()
    {
        /*
        // Program.cs

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = "https://tu-servidor-auth.com";
            options.ClientId = "mi-web-app";
            options.ClientSecret = "mi-secret";
            options.ResponseType = "code";
            options.SaveTokens = true; // Guarda los tokens en la cookie

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("roles");

            // Mapear claims
            options.ClaimActions.MapJsonKey("role", "roles");
            options.TokenValidationParameters.RoleClaimType = "role";

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = context =>
                {
                    // Puedes agregar claims adicionales aquí
                    return Task.CompletedTask;
                }
            };
        });

        // En un controller para obtener el access token:
        // var accessToken = await HttpContext.GetTokenAsync("access_token");
        */
    }

    // ==================== EJEMPLO 5: Minimal API como Resource Server ====================

    /// <summary>
    /// Configuración para una API que valida tokens emitidos por tu servidor.
    /// </summary>
    public static void ResourceServerSetup()
    {
        /*
        // Program.cs de tu API

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Opción A: Si usas RS256 (asimétrico)
                options.Authority = "https://tu-servidor-auth.com";
                options.Audience = "mi-api-client-id";

                // Opción B: Si usas HS256 (simétrico) - necesitas compartir la key
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "YourCompany",
                    ValidateAudience = true,
                    ValidAudience = "mi-api-client-id",
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("TU_SECRET_KEY_COMPARTIDA"))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            // Políticas basadas en roles
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

            // Políticas basadas en permisos
            options.AddPolicy("CanReadUsers", policy =>
                policy.RequireClaim("permissions", "users:read"));
        });

        // En tus endpoints:
        app.MapGet("/api/admin", () => "Solo admins")
            .RequireAuthorization("Admin");

        app.MapGet("/api/users", () => "Lista de usuarios")
            .RequireAuthorization("CanReadUsers");
        */
    }

    // ==================== EJEMPLO 6: Validar token manualmente ====================

    /// <summary>
    /// Validar un token sin usar middleware (útil para servicios/workers).
    /// </summary>
    public static async Task<bool> ValidateTokenManually(string token, string authServerUrl)
    {
        var httpClient = new HttpClient();

        // Opción 1: Usar el endpoint de introspección
        var request = new Dictionary<string, string>
        {
            ["token"] = token,
            ["client_id"] = "mi-app",
            ["client_secret"] = "mi-secret"
        };

        var response = await httpClient.PostAsync(
            $"{authServerUrl}/oauth/introspect",
            new FormUrlEncodedContent(request));

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IntrospectionResult>();
            return result?.Active ?? false;
        }

        return false;
    }

    private class IntrospectionResult
    {
        public bool Active { get; set; }
        public string? Sub { get; set; }
        public string? Scope { get; set; }
    }
}
