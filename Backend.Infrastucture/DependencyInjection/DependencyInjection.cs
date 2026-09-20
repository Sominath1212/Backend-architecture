using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Application.Services;
using Backend.Infrastructure.Identity;
using Backend.Infrastructure.Persistence.Context;
using Backend.Infrastructure.Persistence.Repositories;
using Backend.Infrastructure.Services;
using Backend.Infrastucture.Services.FileStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Identity
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICandidateProfileService, CandidateProfileService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IExperienceService, ExperienceService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IRecruiterService, RecruiterService>();
            services.AddScoped<IJobService, JobService>();
            return services;
        }
    } 
}