using System.Collections.Generic;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OmenX.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OmenX.OpenApiFilters
{
    internal class OmenXRoutesDocumentFilter : IDocumentFilter
    {
        private readonly OmenXCheckPointTypeStore _omenXCheckPointTypeStore;

        public OmenXRoutesDocumentFilter(OmenXCheckPointTypeStore omenXCheckPointTypeStore)
        {
            _omenXCheckPointTypeStore = omenXCheckPointTypeStore;
        }


        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var checkListOperation = new OpenApiOperation
            {
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "OmenX" } },
                Summary = "获取所有检查点",
                Description = "在系统定义完检查点之后，会通过这个接口返回，包含请求的路由，标题和描述",
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "OK",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "array",
                                    Description = "检查点详情列表",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["Title"] = new OpenApiSchema { Type = "string" },
                                            ["Description"] = new OpenApiSchema { Type = "string" },
                                            ["Url"] = new OpenApiSchema { Type = "string" }
                                        },
                                        Example = new OpenApiObject
                                        {
                                            ["Title"] = new OpenApiString("慢SQL检查"),
                                            ["Description"] = new OpenApiString("检查是否存在执行时间大于10s的SQL"),
                                            ["Url"] = new OpenApiString("/api/omen-x/slow-sql")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var checklistPathItem = new OpenApiPathItem();
            checklistPathItem.AddOperation(OperationType.Get, checkListOperation);
            swaggerDoc.Paths.Add("/api/omen-x/checklists", checklistPathItem);

            var omeXCheckResultId = typeof(OmeXCheckResult).FullName.Replace(".", "_");
            if (!context.SchemaRepository.Schemas.ContainsKey(omeXCheckResultId))
            {
                var schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["Result"] = new OpenApiSchema { Type = "string" },
                        ["Message"] = new OpenApiSchema { Type = "string" }
                    },
                    Example = new OpenApiObject
                    {
                        ["Result"] = new OpenApiString("Warning"),
                        ["Message"] = new OpenApiString("存在SQL执行时间大于10秒")
                    }
                };
                context.SchemaRepository.Schemas.Add(omeXCheckResultId, schema);
            }

            foreach (var checkPointType in _omenXCheckPointTypeStore.Types)
            {
                var checkPointInfo = checkPointType.GetCustomAttribute<CheckPointMetadataAttribute>();
                var operation = new OpenApiOperation
                {
                    Tags = new List<OpenApiTag> { new OpenApiTag { Name = "OmenX" } },
                    Summary = !string.IsNullOrWhiteSpace(checkPointInfo?.Title)
                        ? checkPointInfo.Title
                        : checkPointType.Name,
                    Description = !string.IsNullOrWhiteSpace(checkPointInfo?.Description)
                        ? checkPointInfo.Description
                        : "",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Description = "检查结果列表",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = omeXCheckResultId
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                var pathItem = new OpenApiPathItem();
                pathItem.AddOperation(OperationType.Post, operation);
                swaggerDoc.Paths.Add($"/api/omen-x/{checkPointType.Name.ToLower()}", pathItem);
            }
        }
    }
}