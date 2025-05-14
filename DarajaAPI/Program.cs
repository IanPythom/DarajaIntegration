using Asp.Versioning;
using Daraja.DbContext;
using DarajaAPI.Models;
using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;
using DarajaAPI.Services;
using DarajaAPI.Services.Daraja;
using DarajaAPI.Swagger;
using MailServiceAPI.Services.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Clear default logging providers
builder.Logging.ClearProviders();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Add this in Program.cs after builder.Services.AddHttpClient();
builder.Services.AddHttpClient("mpesa", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Daraja:MpesaBaseUrl"]);
});

builder.Services.Configure<DarajaSetting>(builder.Configuration.GetSection("Daraja"));
builder.Services.AddScoped<DarajaAuthService>();

// REPOSITORY INJECTION
builder.Services.AddScoped<ITokenRepos, TokenRepos>();

// register the IMailService and MailService
builder.Services.AddTransient<DarajaAPI.Services.Mail.IMailService, MailServiceAPI.Services.Mail.MailService>();

// Now that we are on the Development Environment, it will by default read the data from the appsettings.Development.Json
// Then we will have to attach these data to a MailSettings class by adding builder.Services.Configure<MailSettings>
// This is much much better than to simply hardcode all the details within your application code.
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));

// SWAGGER CONFIGURATIONS
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // Construct the XML file name based on the assembly name
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); // Combine the base directory path with the XML file name
    // Check if the XML documentation file exists
    if (File.Exists(xmlPath))
    {
        // Include XML comments in the Swagger documentation
        options.IncludeXmlComments(xmlPath);
    }
    // Adds customs operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// VERSIONING
builder.Services.AddApiVersioning(Opt =>
{
    Opt.DefaultApiVersion = new ApiVersion(1, 0); // default version
    Opt.AssumeDefaultVersionWhenUnspecified = true; // Takes default version if not specified
    Opt.ReportApiVersions = true;
    Opt.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// VERSIONING PART 2
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// SERILOG
builder.Host.UseSerilog((context, configuration) =>
{
    // Log folder path
    var logFolderPath = @"Serilog";

    // Create the log folder if it doesn't exist
    Directory.CreateDirectory(logFolderPath);

    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("SourceContext", null)
        .WriteTo.File(
            path: Path.Combine(logFolderPath, "Daraja.api.log"),
            rollingInterval: RollingInterval.Hour,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{ClientIp}] [{RequestId}] [{RequestPath}] [{Message:lj}] [{Exception}]{NewLine}"
        );
});

// MySQL INJECTION
builder.Services.AddDbContext<DarajaDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DarajaConnectionString"),
        new MySqlServerVersion(new Version(8, 3, 0)), // Specify MySQL version
        mySqlOptions =>
        {
            mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore); // Ignore schema properties
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 1,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
    );
});

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DarajaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

// JWT CONFIGURATIONS
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

// ADD CONTROLLERS
builder.Services.AddControllers();

// ENDPOINT API EXPLORER
builder.Services.AddEndpointsApiExplorer();

// Configure Data Protection
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys"); // Security feature in .NET Core used to protect sensitive data, especially when stored or transmitted.
builder.Services.AddDataProtection() //  Adds Data Protection services to the application's services container
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder)) //  persist its keys to the file system at the specified location (keysFolder)
    .SetApplicationName("DarajaApplication"); // Sets a specific application name for the Data Protection system. Useful when you have multiple applications that need to share protected data.

var app = builder.Build();

// Configure the HTTP request pipeline. Ensures that Swagger and Swagger UI middlewares are added to the request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        //Build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // I dont have static files (maybe the template)

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<DarajaDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Add logging to trace the seeding process
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Starting role seeding");
        await ContextSeed.SeedRolesAsync(userManager, roleManager); // Seed Roles method
        logger.LogInformation("Role seeding completed");

        logger.LogInformation("Starting super admin seeding");
        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager); // Seed Super Admin User
        logger.LogInformation("Super admin seeding completed");
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();