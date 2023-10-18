using keepscape_api.Configurations;
using keepscape_api.Data;
using keepscape_api.Repositories;
using keepscape_api.Repositories.Generics;
using keepscape_api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using keepscape_api.Models;
using keepscape_api.Services.Users;
using Google.Cloud.Storage.V1;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.Tokens;
using keepscape_api.Services.Products;
using keepscape_api.Services.ConfirmationCodes;
using keepscape_api.Services.Emails;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Google.Apis.Auth.OAuth2;
using Microsoft.Identity.Client;
using keepscape_api.Services.Announcements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

await ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

async Task ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddRouting(options => options.LowercaseUrls = true);
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    var keyVaultEndpoint = builder.Configuration.GetSection("VaultUri").Value;
    var keyVault = new Uri(keyVaultEndpoint!);
    var secretClient = new SecretClient(keyVault, new DefaultAzureCredential());
    KeyVaultSecret dbSecret = await secretClient.GetSecretAsync("prod-new-db");
    KeyVaultSecret jwtSecret = await secretClient.GetSecretAsync("jwt-secret");
    KeyVaultSecret googleKey = await secretClient.GetSecretAsync("GoogleCloudServiceAccountKey");
    configuration["JwtConfig:Secret"] = jwtSecret.Value;

    GoogleCredential googleCredential;
    using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(googleKey.Value)))
    {
        googleCredential = GoogleCredential.FromStream(jsonStream);
    }

    services.AddDbContext<APIDbContext>(options =>
    {
        options.UseSqlServer(dbSecret.Value)
        .UseLazyLoadingProxies();
    });
    
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "keepscape_api", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                          Enter 'Bearer' [space] and then your token in the text input below.
                          \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }
        });
    });
    services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
    services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));

    services.AddAuthentication(options => 
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtConfig:Issuer"],
            ValidAudience = configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Secret"]!))
        };
    });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
        options.AddPolicy("Seller", policy => policy.RequireClaim("Role", "Seller"));
        options.AddPolicy("Buyer", policy => policy.RequireClaim("Role", "Buyer"));
    });
    services.AddTransient<APIDbContext>();
    services.AddSingleton(sp => StorageClient.Create(googleCredential));

    services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
    services.AddScoped<IBalanceRepository, BalanceRepository>();
    services.AddScoped<ICartRepository, CartRepository>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IPlaceRepository, PlaceRepository>();
    services.AddScoped<IConfirmationCodeRepository, ConfirmationCodeRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
    services.AddScoped<IProfileRepository<BuyerProfile>, BuyerProfileRepository>();
    services.AddScoped<ISellerProfileRepository, SellerProfileRepository>();
    services.AddScoped<ISellerApplicationRepository, SellerApplicationRepository>();
    services.AddScoped<ITokenRepository, TokenRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<ISellerApplicationRepository, SellerApplicationRepository>();
    services.AddScoped<IBaseImageRepository, BaseImageRepository>();

    services.AddScoped<IAnnouncementService, AnnouncementService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IImageService, ImageService>();
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<IConfirmationCodeService, ConfirmationCodeService>();
    services.AddScoped<IEmailService, EmailService>();
}