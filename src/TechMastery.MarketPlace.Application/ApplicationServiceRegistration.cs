using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using TechMastery.MarketPlace.Application.Validators;

namespace TechMastery.MarketPlace.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IValidator<CartItemDto>, CartItemDtoValidator>();

            return services;
        }
    }
}
