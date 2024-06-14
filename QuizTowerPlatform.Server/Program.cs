using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using QuizTowerPlatform.Authorization;
using QuizTowerPlatform.BFF.DbContexts;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    //var apiRoot = builder.Configuration["CoreBackendApiAuthority"];
    var idpAuthority = builder.Configuration["IdPAuthority"];
    const string bffCookieScheme = "BFFCookieScheme";
    const string bffOpenIdConnectChallengeScheme = "BFFChallengeScheme";

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddDbContext<BffDbContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // Note: hiermee (JsonSerializerOptions.PropertyNamingPolicy = null) geef je aan dat de eigenschapsnamen precies moeten worden gebruikt zoals ze in de C#-code zijn gedefinieerd, zonder enige aanpassing (bijvoorbeeld zonder omzetting naar camelCase).
    builder.Services.AddControllers()
        .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddBff()
        .AddRemoteApis();

    // Note: With the next line, clean the JWT Token by removing 'backward compatibility: Microsoft WS-Security standard', which is no longer needed.
    JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

    // Note: Instead of manually passing bearer token we are using package of MS and 'AddAccessTokenManagement' function.
    builder.Services.AddAccessTokenManagement();

    builder.Services.AddHttpClient("IDPClient", client =>
    {
        client.BaseAddress = idpAuthority == null ? null : new Uri(idpAuthority);
    });

    builder.Services.AddAuthentication(options => 
    {
        options.DefaultScheme = bffCookieScheme;
        options.DefaultChallengeScheme = bffOpenIdConnectChallengeScheme;
        options.DefaultSignOutScheme = bffOpenIdConnectChallengeScheme;
    })
    .AddCookie(bffCookieScheme, options =>
    {
        options.Cookie.Name = $"__Host-{bffCookieScheme}";
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    })
    .AddOpenIdConnect(bffOpenIdConnectChallengeScheme, options =>
    {
        //options.SignInScheme = bffOpenIdConnectChallengeScheme;
        options.Authority = $"{idpAuthority}";
        options.ClientId = "towerofquizzesbff";
        options.ClientSecret = "bffclientsecret";
        options.ResponseType = "code";
        options.ResponseMode = "query";
        //options.CallbackPath = new PathString("signin-oidc");
        //SignedOutCallbackPath: default = host:port/signout-callback-oidc.
        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        // https://github.com/dotnet/aspnetcore/blob/v8.0.1/src/Security/Authentication/OpenIdConnect/src/OpenIdConnectOptions.cs
        // options.Scope.Clear(); // Note: If you use Scope.Clear(), then add openid and profile.
        options.ClaimActions.Remove("aud");
        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.Scope.Add("roles");
        options.Scope.Add("towerofquizzesapi.read");
        options.Scope.Add("towerofquizzesapi.write");
        options.Scope.Add("country");
        options.Scope.Add("offline_access");
        options.ClaimActions.MapJsonKey("role", "role");
        options.ClaimActions.MapUniqueJsonKey("country", "country");
        options.TokenValidationParameters = new()
        {
            NameClaimType = "given_name",
            RoleClaimType = "role",
        };
    });

    builder.Services.AddAuthorization(authorizationOptions =>
    {
        authorizationOptions.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseBff();
    app.UseAuthorization();
    app.MapBffManagementEndpoints();

    app.MapControllers();

    //// Comment this out to use the external api
    //app.MapGroup("/todos")
    //    .ToDoGroup()
    //    .RequireAuthorization()
    //    .AsBffApiEndpoint();

    // Comment this in to use the external api
    app.MapRemoteBffApiEndpoint("/toq/images", "https://localhost:7258/api/images")
        .RequireAccessToken(Duende.Bff.TokenType.User);

    app.MapFallbackToFile("/index.html");

    app.Run();

}
catch (HostAbortedException)
{
    // eat exception, cfr https://github.com/dotnet/efcore/issues/29809
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
