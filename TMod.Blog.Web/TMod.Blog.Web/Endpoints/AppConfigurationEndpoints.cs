using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TMod.Blog.Web.Endpoints
{
    public static class AppConfigurationEndpoints
    {
        public static WebApplication MapLocalConfigurationEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/configurations")
                .WithName("LocalConfigurations")
                .WithDisplayName("本地配置读取终结点")
                .WithSummary("本地配置读取终结点")
                .WithDescription("本地配置读取终结点，包含了读取当前应用 appsettings.json 文件的接口")
                .WithTags("LocalConfigurations");

            group.MapGet("/{configKey}", Results<NotFound, Ok<string>> ([FromRoute] string configKey, [FromServices] IConfiguration configuration) =>
            {
                string? value = configuration.GetValue<string>(configKey);
                if ( string.IsNullOrEmpty(value) )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(value);
            });
            return app;
        }
    }
}
