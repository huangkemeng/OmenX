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
            if (!context.SchemaRepository.Schemas.ContainsKey(typeof(OmeXCheckResult).FullName))
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
                context.SchemaRepository.Schemas.Add(typeof(OmeXCheckResult).FullName, schema);
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
                                                Id = typeof(OmeXCheckResult).FullName
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