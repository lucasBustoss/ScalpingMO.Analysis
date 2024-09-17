
using ScalpingMO.Analysis.Analysis.API.Config;
using ScalpingMO.Analysis.Analysis.API.Repositories.Login;
using ScalpingMO.Analysis.Analysis.API.Repositories.Users;

namespace ScalpingMO.Analysis.Analysis.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddApiConfiguration();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddJwtConfig(builder.Configuration);

            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();

            var app = builder.Build();

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            app.UseApiConfiguration(app.Environment);
            app.MapControllers();

            app.Run();
        }
    }
}
