using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gym.Application.Attendaces;
using Gym.Application.Auth;
using Gym.Application.Bookings;
using Gym.Application.Classes;
using Gym.Application.Classes.Validators;
using Gym.Application.DTOs.Members;
using Gym.Application.DTOs.Plans;
using Gym.Application.DTOs.Reports;
using Gym.Application.DTOs.Sessions;
using Gym.Application.DTOs.Trainers;
using Gym.Application.Members;
using Gym.Application.Payments;
using Gym.Application.Plans;
using Gym.Application.Reports;
using Gym.Application.Services;
using Gym.Application.Trainers;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Auth;
using Gym.Infrastructure.Email;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Repositories;
using Gym.Infrastructure.Seed;
using Gym.Presentation.BackgroundJobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;

// =============================
// 1) Load Serilog BEFORE builder
// =============================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile("serilog.json", optional: true)
        .Build()
    )
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Replace default logging
builder.Host.UseSerilog();

// =============================
// 2) AutoMapper
// =============================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// =============================
// 3) DbContext
// =============================
builder.Services.AddDbContext<GymDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================
// 4) Identity
// =============================
builder.Services
    .AddIdentityCore<User>(opt =>
    {
        opt.User.RequireUniqueEmail = false;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireDigit = true;
        opt.Password.RequireUppercase = true;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<GymDbContext>()
    .AddDefaultTokenProviders();

// =============================
// 5) JWT
// =============================
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// =============================
// 6) Repository DI
// =============================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IJobLockRepository, JobLockRepository>();
builder.Services.AddScoped<IJobHistoryRepository, JobHistoryRepository>();

// =============================
// 7) Unit of Work
// =============================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =============================
// 8) Services
// =============================
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<TrainerService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// =============================
// 9) Email
// =============================
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// =============================
// 10) Background Job (only once)
// =============================
builder.Services.AddHostedService<SubscriptionJob>();

// =============================
// 11) Validators
// =============================
builder.Services.AddValidatorsFromAssemblyContaining<CreateClassRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateClassSessionRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTrainerRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePlanRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberRequestValidator>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// =============================
// 12) MVC
// =============================
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// =============================
// 13) Razor Pages
// =============================
builder.Services.AddRazorPages();


// =============================
// 14) Swagger
// =============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gym API",
        Version = "v1",
        Description = "Gym Management API — JWT, ETag concurrency, soft delete, paging"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        Description = "Paste JWT token here"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// =============================
// 14) Build app
// =============================
var app = builder.Build();

// =============================
// 15) Seed data
// =============================
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<GymDbContext>();

    await DataSeeder.SeedAsync(sp);
    await ReportSeeder.SeedReportDataAsync(db);
}

// =============================
// 16) Middleware pipeline
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

// =============================
// 17) Run App
// =============================
app.Run();
