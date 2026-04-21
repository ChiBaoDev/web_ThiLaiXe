using System.Text;
using DotNetEnv;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Middleware;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Mapping;
using HeThongThiBangLai.Api.Repositories;
using HeThongThiBangLai.Api.Repositories.CriticalQuestions;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Repositories.Exams;
using HeThongThiBangLai.Api.Repositories.Questions;
using HeThongThiBangLai.Api.Repositories.Topics;
using HeThongThiBangLai.Api.Repositories.Files;
using HeThongThiBangLai.Api.Repositories.History;
using HeThongThiBangLai.Api.Repositories.Cms;
using HeThongThiBangLai.Api.Repositories.WrongQuestions;
using HeThongThiBangLai.Api.Repositories.Dashboard;
using HeThongThiBangLai.Api.Repositories.Entitlements;
using HeThongThiBangLai.Api.Repositories.Certificates;
using HeThongThiBangLai.Api.Services;
using HeThongThiBangLai.Api.Services.CriticalQuestions;
using HeThongThiBangLai.Api.Services.History;
using HeThongThiBangLai.Api.Services.WrongQuestions;
using HeThongThiBangLai.Api.Services.Dashboard;
using HeThongThiBangLai.Api.Services.Files;
using HeThongThiBangLai.Api.Services.Cms;
using HeThongThiBangLai.Api.Services.Exams;
using HeThongThiBangLai.Api.Services.Entitlements;
using HeThongThiBangLai.Api.Services.Certificates;
using HeThongThiBangLai.Api.Services.Interfaces;
using HeThongThiBangLai.Api.Services.Questions;
using HeThongThiBangLai.Api.Services.Topics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var envCandidates = new[]
{
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "HeThongThiBangLai.Api", ".env")
};

foreach (var envPath in envCandidates.Distinct(StringComparer.OrdinalIgnoreCase))
{
    if (File.Exists(envPath))
    {
        Env.Load(envPath);
        Console.WriteLine($"[BOOT] Loaded env file: {envPath}");
    }
}

var builder = WebApplication.CreateBuilder(args);

var effectiveConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(effectiveConnectionString))
{
    var connBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(effectiveConnectionString);
    Console.WriteLine($"[BOOT] SQL DataSource={connBuilder.DataSource}; Database={connBuilder.InitialCatalog}; User={connBuilder.UserID}; IntegratedSecurity={connBuilder.IntegratedSecurity}");
}
else
{
    Console.WriteLine("[BOOT] SQL connection string DefaultConnection is empty.");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HeThongThiBangLai API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Nhập JWT theo định dạng: Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// GlobalExceptionMiddleware is used via UseMiddleware (no DI registration needed)

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

// QuestionBank
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

// Topics
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITopicService, TopicService>();

// File storage
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();

// CMS
builder.Services.AddScoped<ICmsRepository, CmsRepository>();
builder.Services.AddScoped<ICmsService, CmsService>();

// Entitlements
builder.Services.AddScoped<IEntitlementRepository, EntitlementRepository>();
builder.Services.AddScoped<IEntitlementService, EntitlementService>();

// Certificates
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<ICertificateService, CertificateService>();

// Sample exams
builder.Services.AddScoped<ISampleExamRepository, SampleExamRepository>();
builder.Services.AddScoped<ISampleExamService, SampleExamService>();

// Critical questions
builder.Services.AddScoped<ICriticalQuestionRepository, CriticalQuestionRepository>();
builder.Services.AddScoped<ICriticalQuestionService, CriticalQuestionService>();

// History
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

// Wrong questions
builder.Services.AddScoped<IWrongQuestionRepository, WrongQuestionRepository>();
builder.Services.AddScoped<IWrongQuestionService, WrongQuestionService>();

// Dashboard
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Exam sessions
builder.Services.AddScoped<IExamSessionRepository, ExamSessionRepository>();
builder.Services.AddScoped<IExamSessionService, ExamSessionService>();

// Exam structure rules
builder.Services.AddScoped<IExamRuleRepository, ExamRuleRepository>();
builder.Services.AddScoped<IExamRuleService, ExamRuleService>();

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HeThongThiBangLai.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HeThongThiBangLai.Client";
var jwtSecret = builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("Missing required configuration: Jwt:SecretKey");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanGrantEntitlement", policy =>
        policy.RequireRole("ADMIN", "STAFF_SALE"));

    options.AddPolicy("CanIssueCertificate", policy =>
        policy.RequireRole("ADMIN", "GIAO_VIEN"));

    options.AddPolicy("CanTakeExam", policy =>
        policy.RequireRole("ADMIN", "HOC_VIEN"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HeThongThiBangLai API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Global exception middleware must be before other middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/v1/health", () =>
{
    return Results.Ok(ApiResponseFactory.Success(new
    {
        status = "Healthy",
        service = "HeThongThiBangLai.Api"
    }, "Health check passed"));
})
.WithName("HealthCheck")
.AllowAnonymous();

app.Run();
