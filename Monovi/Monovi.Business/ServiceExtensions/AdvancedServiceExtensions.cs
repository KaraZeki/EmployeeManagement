using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Monovi.Business.IServices;
using Monovi.Business.Services;
using Monovi.DataAccess.Context;
using Monovi.DataAccess.Repository;
using Monovi.Model.Concrete;

namespace Monovi.Business.ServiceExtensions
{
    public static class AdvancedServiceExtensions
    {

        public static void AddCustomDbContext(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<MonoviDbContext>(
                    options => options.UseSqlServer(connectionString));
        }

        public static void AddContainerWithDependencies(this IServiceCollection services)
        {
            #region Core Layer Dependencies
            services.AddTransient<IGenericRepository<User>, GenericRepository<User>>();
            services.AddTransient<IGenericRepository<Role>, GenericRepository<Role>>();
            #endregion

            #region Service Extensions
            services.AddScoped<IUserService, UserService>();
            #endregion
        }
    }
}
