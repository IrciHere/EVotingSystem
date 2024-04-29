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
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem.Api.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, bool withMockEmails = false, bool withMockSms = false)
    {
        services.AddAutoMapper(typeof(EVotingSystemProfile));

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IElectionService, ElectionService>();
        services.AddScoped<IVotesService, VotesService>();

        services.AddScoped<IHelperService, HelperService>();
        services.AddScoped<IEncryptionService, EncryptionService>();

        if (withMockEmails)
        {
            services.AddScoped<IEmailsService, MockEmailsService>();
        }
        else
        {
            services.AddScoped<IEmailsService, GmailEmailsService>();
        }

        if (withMockSms)
        {
            services.AddScoped<ISmsService, MockSmsService>();
        }
        else
        {
            services.AddScoped<ISmsService, SmsapiSmsService>();
        }
        
        return services;
    }

    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IElectionRepository, ElectionRepository>();
        services.AddScoped<IVotesRepository, VotesRepository>();
        
        services.AddScoped<IHelperRepository, HelperRepository>();

        services.AddSingleton<IConfiguration>(_ => configuration);

        services.AddDbContext<EVotingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("EVotingSystemDatabase")));

        return services;
    }
}