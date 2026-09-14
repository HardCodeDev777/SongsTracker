using dotenv.net;
using SongsTracker.Data;

namespace SongsTracker.Extensions.ServicesExtensions;

public static class EnvServiceExtensions
{
    public static EnvData AddEnvService(this WebApplicationBuilder builder)
    {
        if (File.Exists(".env"))
        {
            DotEnv.Load();
        }
        var envData = new EnvData(
            Environment.GetEnvironmentVariable("TG_API_KEY")!,
            long.Parse(Environment.GetEnvironmentVariable("ALLOWED_TG_ID")!)
            );

        builder.Services.AddSingleton(envData);
        return envData;
    }
}
