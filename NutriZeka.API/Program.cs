using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT Ýçin Gerekli
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens; // JWT Þifreleme Ýçin Gerekli
using Microsoft.OpenApi.Models; // Swagger Kilidi Ýçin Gerekli
using NutriZeka.Application.Interfaces;
using NutriZeka.Application.Interfaces.Repositories;
using NutriZeka.Application.Interfaces.Services;
using NutriZeka.Application.Services;
using NutriZeka.Domain.Interfaces;
using NutriZeka.Infrastructure.Context;
using NutriZeka.Infrastructure.Persistence.Repositories;
using NutriZeka.Infrastructure.Repositories;
using NutriZeka.Infrastructure.Services;
using System.Text; // Encoding için gerekli

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Adým: CORS Politikasýný Servis Olarak Ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Herhangi bir web sitesinden/kökeninden gelebilir
              .AllowAnyMethod()   // GET, POST, PUT, DELETE hepsine izin ver
              .AllowAnyHeader();  // Her türlü baþlýk (header) bilgisine izin ver
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

// --- 2. ADIM: SWAGGER'A JWT KÝLÝDÝ EKLEME ---
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Lütfen 'Bearer {token}' formatýnda giriniz. Örneðin: Bearer eyJhbG...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<NutriZeka.Application.Mappings.MappingProfile>());

// --- 3. ADIM: JWT KÝMLÝK DOÐRULAMA (AUTHENTICATION) AYARLARI ---
// Güvenlik Kontrolü: Gerçek þifre dosyada yoksa sistemi güvenli þekilde durdur.
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey == "BURASI_GIZLIDIR_GERCEK_SIFRE_YAZILMAMALIDIR")
{
    throw new InvalidOperationException("JWT Secret Key eksik veya varsayýlan deðerde býrakýlmýþ! appsettings.Development.json dosyanýzý kontrol edin.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "NutriZekaAPI",

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "NutriZekaApp",

            ValidateLifetime = true // Token süresi dolduðunda giriþi engeller
        };
    });

// --- 4. ADIM: DEPENDENCY INJECTION (Servis & Repo Eþleþmeleri) ---
// * Yeni Eklenenler *
builder.Services.AddScoped<ITokenService, TokenService>(); // Token Üretici
builder.Services.AddScoped<IUserRepository, UserRepository>(); // User Repo
builder.Services.AddScoped<IUserService, UserService>(); // User Servisi

// * Mevcut Olanlar *
builder.Services.AddScoped<IProductImportService, ProductImportService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IScanHistoryRepository, ScanHistoryRepository>();
builder.Services.AddScoped<IScanHistoryService, ScanHistoryService>();
builder.Services.AddScoped<IAIAnalysisCacheRepository, AIAnalysisCacheRepository>();

// Hem HttpClient fabrikasýný hem de Gemini servisini sisteme tanýtýyoruz
builder.Services.AddHttpClient<IGenerativeAiService, GeminiService>();
// Infrastructure/Services içindeki AIAnalysisService'i sisteme tanýtýyoruz
builder.Services.AddScoped<IAIAnalysisService, AIAnalysisService>();

// --- RESTORANIN ÝNÞAATI BÝTTÝ ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// CORS Politikasýný Aktif Et
app.UseCors("AllowAll");

// --- 5. ADIM: KÝMLÝK VE YETKÝ KONTROLÜ ---
// DÝKKAT: UseAuthentication HER ZAMAN UseAuthorization'dan ÖNCE yazýlmalýdýr!
app.UseAuthentication(); // 1. Sen kimsin? (Token'ý doðrular)
app.UseAuthorization();  // 2. Buraya girmeye yetkin var mý? (Ýzinleri kontrol eder)
app.UseStaticFiles(); // Bu olmadan uploads klasöründeki resimler dýþarýdan GÖRÜNMEZ!
app.MapControllers();
app.Run();