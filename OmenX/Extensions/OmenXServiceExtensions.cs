using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OmenX.Contracts;
using OmenX.OpenApiFilters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace OmenX.Extensions
{
    public static class OmenXServiceExtensions
    {
        private static readonly OmenXCheckPointTypeStore OmenXCheckPointTypeStore = new OmenXCheckPointTypeStore();

        public static IServiceCollection AddOmenX(this IServiceCollection services, params Assembly[] assemblies)
        {
            var assembly = Assembly.GetCallingAssembly();
            if (!assemblies.Contains(assembly))
            {
                ScanCheckPoint(services, assembly);
            }

            foreach (var assemblyItem in assemblies)
            {
                ScanCheckPoint(services, assemblyItem);
            }

            services.TryAddSingleton(OmenXCheckPointTypeStore);
            return services;
        }

        public static void AddOmenXApiDoc(this SwaggerGenOptions options)
        {
            options.SwaggerDoc("omen-x", new OpenApiInfo
            {
                Title = "OmenX API",
                Version = "v1",
                Description = @"
## 系统功能
提供分布式检查点（Checkpoint）的自动化验证能力，覆盖:
- 数据库健康状态
- 服务连通性
- 关键业务指标
- 自定义业务规则检查

## 使用场景
- **运维监控**：实时系统健康度检测
- **CI/CD**：部署后自动化验证
- **故障排查**：快速定位问题层级
`",
            });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "omen-x")
                {
                    return apiDesc.RelativePath.Contains("/omen-x/");
                }

                return true;
            });
            options.DocumentFilter<OmenXRoutesDocumentFilter>(OmenXCheckPointTypeStore);
        }

        public static void UseOmenXApiDoc(this SwaggerUIOptions options)
        {
            options.SwaggerEndpoint("/swagger/omen-x/swagger.json", "OmenX API");
        }

        public static T UseOmenX<T>(this T app) where T : IApplicationBuilder
        {
            var reqList = new List<(string Url, string Title, string Description)>();
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var checkPoints = scope.ServiceProvider.GetServices<IOmenXCheckPoint>();
                if (checkPoints != null)
                {
                    foreach (var checkPoint in checkPoints)
                    {
                        var url = $"/api/omen-x/{checkPoint.GetType().Name.ToLower()}";
                        var type = checkPoint.GetType();
                        var metadata = type.GetCustomAttribute<CheckPointMetadataAttribute>();
                        reqList.Add((url, metadata?.Title ?? type.Name, metadata?.Description ?? ""));
                        app.Map(url, builder =>
                        {
                            builder.Use(async (ctx, next) =>
                            {
                                if (!ctx.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                                {
                                    ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                                    return;
                                }

                                var checkResult = new OmeXCheckPointContext();
                                await checkPoint.CheckAsync(checkResult);
                                ctx.Response.ContentType = "application/json";
                                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(checkResult.CheckResults));
                            });
                        });
                    }
                }
            }

            app.Map("/api/omen-x/checklists", builder =>
            {
                builder.Use(async (ctx, next) =>
                {
                    if (!ctx.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        return;
                    }

                    ctx.Response.ContentType = "application/json";
                    await ctx.Response.WriteAsync(
                        JsonConvert.SerializeObject(reqList.Select(x => new { x.Url, x.Title, x.Description })));
                });
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "omenxroot", "assets")),
                RequestPath = "/assets"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "omenxroot")),
                RequestPath = "/omenx-ui"
            });
            app.Map("/omenx-ui", builder =>
            {
                builder.Run(async context =>
                {
                    string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    string indexPath = Path.Combine(binPath, "omenxroot", "index.html");

                    if (!File.Exists(indexPath))
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Page not found");
                        return;
                    }

                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(indexPath);
                });
            });
            return app;
        }

        private static void ScanCheckPoint(IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t =>
                t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IOmenXCheckPoint)));
            foreach (var type in types)
            {
                services.AddScoped(typeof(IOmenXCheckPoint), type);
                OmenXCheckPointTypeStore.Types.Add(type);
            }
        }
    }
}