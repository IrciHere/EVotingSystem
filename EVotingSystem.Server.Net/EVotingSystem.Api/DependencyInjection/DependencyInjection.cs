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

        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IElectionService, ElectionService>();
        services.AddTransient<IVotesService, VotesService>();

        services.AddTransient<IHelperService, HelperService>();
        services.AddTransient<IEncryptionService, EncryptionService>();

        if (withMockEmails)
        {
            services.AddTransient<IEmailsService, MockEmailsService>();
        }
        else
        {
            services.AddTransient<IEmailsService, GmailEmailsService>();
        }

        if (withMockSms)
        {
            services.AddTransient<ISmsService, MockSmsService>();
        }
        else
        {
            services.AddTransient<ISmsService, MockSmsService>();
        }
        
        return services;
    }

    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<IElectionRepository, ElectionRepository>();
        services.AddTransient<IVotesRepository, VotesRepository>();
        
        services.AddTransient<IHelperRepository, HelperRepository>();

        services.AddSingleton<IConfiguration>(_ => configuration);

        services.AddDbContext<EVotingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("EVotingSystemDatabase")));

        return services;
    }
}