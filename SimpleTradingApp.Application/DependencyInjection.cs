using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleTradingApp.Application.ServiceContracts;
using SimpleTradingApp.Application.Services;
using SimpleTradingApp.Application.Validators;

namespace SimpleTradingApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IAccountsService, AccountsService>();
        services.AddValidatorsFromAssemblyContaining<AddAccountDtoValidator>();

        return services;
    }
}
