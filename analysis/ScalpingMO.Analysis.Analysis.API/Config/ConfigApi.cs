using ScalpingMO.Analysis.Analysis.API.Middlewares;
using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Analysis.API.Config
{
    public static class ConfigApi
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }); ;

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });


            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<InternalErrorHandlingMiddleware>();
            app.UseCors("AllowAll");

            app.UseAuthConfiguration();
            // Temporariamente desabilitado
            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
