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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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

    // Add Dbcontext 
    services.AddDbContext<APIDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies();
    });

    // Add Swagger gen with security definition and security requirement
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
    services.Configure<CodeConfig>(configuration.GetSection("CodeConfig"));

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
    services.AddSingleton(sp => StorageClient.Create());

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

    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IBaseImageService, BaseImageService>();
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<IConfirmationCodeService, ConfirmationCodeService>();
}