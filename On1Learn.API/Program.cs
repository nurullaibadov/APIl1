using Microsoft.EntityFrameworkCore;
using On1Learn.Domain.Interfaces.Repositories;
using On1Learn.Infrastructure.Data;
using On1Learn.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. SERVICES CONFIGURATION (Dependency Injection)
// ============================================================

// MySQL Database bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Pomelo MySQL sağlayıcısı kullan
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            // Migration'lar Infrastructure katmanında
            mySqlOptions.MigrationsAssembly("RealEstateApp.Infrastructure");

            // Command timeout (uzun süren sorgular için)
            mySqlOptions.CommandTimeout(60);

            // Retry on failure (bağlantı kesilirse otomatik tekrar dene)
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    );

    // Development ortamında detaylı log
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); // SQL parametrelerini göster
        options.EnableDetailedErrors(); // Detaylı hata mesajları
    }
});

// ============================================================
// DEPENDENCY INJECTION - REPOSITORIES
// ============================================================

// Unit of Work Pattern (Scoped: Her request için bir instance)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Controller'ları ekle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // JSON serialization ayarları
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase; // camelCase kullan
    });

// CORS (Cross-Origin Resource Sharing) - Frontend'in API'yi kullanabilmesi için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Her domain'den istek kabul et
              .AllowAnyMethod()   // GET, POST, PUT, DELETE hepsi
              .AllowAnyHeader();  // Her header kabul et
    });

    // Production için daha güvenli CORS policy:
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://www.yourfrontend.com", "https://yourfrontend.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Cookie'lere izin ver
    });
});

// Swagger/OpenAPI (API dokümantasyonu)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Real Estate API",
        Version = "v1",
        Description = "Real Estate Management API with Onion Architecture",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // JWT Authentication için Swagger'a ekle
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// 2. HTTP REQUEST PIPELINE
// ============================================================

var app = builder.Build();

// Development ortamında Swagger UI'ı aç
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate API v1");
        options.RoutePrefix = string.Empty; // Swagger'ı root'ta aç: http://localhost:5000
    });
}

// HTTPS yönlendirmesi
app.UseHttpsRedirection();

// Statik dosyalar (resimler, CSS, JS)
app.UseStaticFiles(); // wwwroot klasörünü serve eder

// CORS middleware
app.UseCors("AllowAll"); // Development için
// app.UseCors("Production"); // Production'da bu satırı kullan

// Authentication & Authorization
// app.UseAuthentication(); // JWT token kontrolü (Gün 2'de ekleyeceğiz)
// app.UseAuthorization();  // Yetki kontrolü

// Controller'ları map'le
app.MapControllers();

// ============================================================
// 3. DATABASE MIGRATION (İlk çalıştırmada)
// ============================================================

// Uygulama başlarken database'i otomatik oluştur/güncelle
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Pending migration'ları uygula
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Applying pending migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully!");
        }
        else
        {
            Console.WriteLine("No pending migrations.");
        }

        // Database var mı kontrol et, yoksa oluştur
        context.Database.EnsureCreated();

        Console.WriteLine("Database connection successful!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");

        // Hata durumunda uygulamayı durdur
        throw;
    }
}

// Uygulamayı başlat
app.Run();

