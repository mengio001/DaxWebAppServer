using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using QuizTowerPlatform.Authorization;
using QuizTowerPlatform.Server.Services;
using QuizTowerPlatform.Server.Authorization;
using QuizTowerPlatform.Server.DbContexts;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    // Note: hiermee (JsonSerializerOptions.PropertyNamingPolicy = null) geef je aan dat de eigenschapsnamen precies moeten worden gebruikt zoals ze in de C#-code zijn gedefinieerd, zonder enige aanpassing (bijvoorbeeld zonder omzetting naar camelCase).
    builder.Services.AddControllers()
        .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddDbContext<ApiDbContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // register the repository
    builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IAuthorizationHandler, MustOwnImageHandler>();

    // register AutoMapper-related services
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        // Note: under the hood, the following happens before using reference token 'AddOAuth2Introspection'.
        // Note: for testing (dotnet user-jwts list, print or create [options] --name {the name of the user to create the JWT for.}) without IDP we need return back to AccessTokenType = AccessTokenType.Jwt
        //.AddJwtBearer(options =>
        //{
        //    options.Authority = "https://localhost:5001";
        //    options.Audience = "towerofquizzesapi";
        //    options.TokenValidationParameters = new()
        //    {
        //        NameClaimType = "given_name",
        //        RoleClaimType = "role",
        //        ValidTypes = new[] { "at+jwt" } // note: no more needed by AddOAuth2Introspection to prevent JWT Token attack! because there is nothing to decode and read with reference token.
        //    };
        //});
        .AddOAuth2Introspection(options =>
        {
            options.Authority = "https://localhost:44300"; // note: middleware IDP entry-point URI
            options.ClientId = "towerofquizzesapi";
            options.ClientSecret = "apisecret";
            options.NameClaimType = "given_name";
            options.RoleClaimType = "role";
        });

    builder.Services.AddAuthorization(authorizationOptions =>
    {
        authorizationOptions.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());

        authorizationOptions.AddPolicy("ClientApplicationCanWrite", policyBuilder =>
            {
                policyBuilder.RequireClaim("scope", "usermanagementapi.write");
            }
        );

        authorizationOptions.AddPolicy("MustOwnImage", policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddRequirements(new MustOwnImageRequirement());
            }
        );
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

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

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
