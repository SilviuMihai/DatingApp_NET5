using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<DataContext>(options => 
            { options.UseSqlite(config.GetConnectionString("DefaultConnection")); });
            services.AddScoped<ITokenService,TokenService>();
            //services.AddScoped<IUserRepository,UserRepository>(); using unit of work pattern
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<LogUserActivity>();
            //services.AddScoped<ILikesRepository,LikesRepository>(); using unit of work pattern
            //services.AddScoped<IMessageRepository,MessageRepository>(); using unit of work pattern
            services.AddScoped<IUnitOfWork,UnitOfWork>(); // unit of work
            services.AddSingleton<PresenceTracker>();
            //services.AddScoped<IPhotoManagement,PhotoRepository>(); //using unit of work pattern
            services.AddHttpContextAccessor();
            return services;
        }
    }
}