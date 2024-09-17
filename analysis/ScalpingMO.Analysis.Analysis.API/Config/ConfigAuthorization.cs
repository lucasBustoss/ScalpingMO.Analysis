using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ScalpingMO.Analysis.Analysis.API.Config.Models;
using System.Text;

namespace ScalpingMO.Analysis.Analysis.API.Config
{
    public static class ConfigAuthorization
    {
        public static void AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("JwtBearerTokenSettings");
            services.Configure<ConfigAppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<ConfigAppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthorization();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = true;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JwtBearerTokenSettings:Audience"],
                    ValidIssuer = configuration["JwtBearerTokenSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]))
                };
            });
        }

        public static void UseAuthConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
