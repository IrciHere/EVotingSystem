using System.Text;
using EVotingSystem.Api.Mapper;
using EVotingSystem.Database.Context;
using EVotingSystem.Repositories.Implementations;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Implementations;
using EVotingSystem.Services.Implementations.Emails;
using EVotingSystem.Services.Implementations.Helpers;
using EVotingSystem.Services.Implementations.Sms;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

IConfigurationRoot configuration = configurationBuilder.Build();

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-Voting System API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    { 
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT", 
        Name = "Authorization", 
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAutoMapper(typeof(EVotingSystemProfile));

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddTransient<IElectionService, ElectionService>();
builder.Services.AddTransient<IVotesService, VotesService>();
builder.Services.AddTransient<IEmailsService, MockEmailsService>();
builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.AddTransient<IPasswordEncryptionService, PasswordEncryptionService>();

builder.Services.AddTransient<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IHelperRepository, HelperRepository>();
builder.Services.AddTransient<IElectionRepository, ElectionRepository>();
builder.Services.AddTransient<IVotesRepository, VotesRepository>();

builder.Services.AddSingleton<IConfiguration>(_ => configuration);

builder.Services.AddDbContext<EVotingDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("EVotingSystemDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
