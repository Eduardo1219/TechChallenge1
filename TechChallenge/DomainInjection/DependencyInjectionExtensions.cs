using Domain.Contact.Repository;
using Domain.Contact.Service;
using Domain.Region.Repository;
using Domain.Region.Service;
using Infraestructure.Context;
using Infraestructure.Repository.ContactsRepository;
using Infraestructure.Repository.RegionRepository;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge1.DomainInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureContext(services, configuration);
            ConfigureContacts(services);
            ConfigureRegions(services);

            return services;
        }

        public static void ConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TechChallengeContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));
        }

        public static void ConfigureContacts(this IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactService, ContactService>();
        }

        public static void ConfigureRegions(this IServiceCollection services)
        {
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IRegionService, RegionService>();
        }
    }
}
