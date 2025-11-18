using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gym.Application.Auth;
using Gym.Application.Classes;
using Gym.Application.Classes.Validators;
using Gym.Application.DTOs.Members;
using Gym.Application.DTOs.Plans;
using Gym.Application.Mapping;
using Gym.Application.Members;
using Gym.Application.Plans;
using Gym.Application.Trainers;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Auth;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//  DbContext 
builder.Services.AddDbContext<GymDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  IdentityCore (API) 
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

//  JWT default 
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

    });

//  DI 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<TrainerService>();

// Service
builder.Services.AddScoped<ClassService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateClassRequestValidator>();


//  MVC + FluentValidation 
builder.Services.AddControllers();

// v11+ auto-validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// scan validators 
builder.Services.AddValidatorsFromAssemblyContaining<CreatePlanRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberRequestValidator>();

//  Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Gym API",
        Version     = "v1",
        Description = "Gym Management API — JWT, ETag concurrency, soft delete, paging"
    });

    // HTTP Bearer (JWT)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat= "JWT",
        In          = ParameterLocation.Header,
        Description = "Paste access token here (no 'Bearer' prefix)."
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

//  SEED roles + admin
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

//  Pipeline 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.DocumentTitle = "Gym API Docs";
        o.DisplayOperationId();
        o.DisplayRequestDuration();
    });
}

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
