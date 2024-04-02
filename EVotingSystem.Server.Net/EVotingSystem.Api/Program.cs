using EVotingSystem.Api.Mapper;
using EVotingSystem.Database.Context;
using EVotingSystem.Repositories.Implementations;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Implementations;
using EVotingSystem.Services.Implementations.Helpers;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(EVotingSystemProfile));

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IEmailsService, EmailsService>();
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddTransient<IPasswordEncryptionService, PasswordEncryptionService>();

builder.Services.AddTransient<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IHelperRepository, HelperRepository>();

builder.Services.AddDbContext<EVotingDbContext>();

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
