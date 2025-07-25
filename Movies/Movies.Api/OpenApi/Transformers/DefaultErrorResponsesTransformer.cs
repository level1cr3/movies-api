﻿using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Movies.Api.OpenApi.Transformers;

public class DefaultErrorResponsesTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations.Values))
        {
            operation.Responses.TryAdd(StatusCodes.Status500InternalServerError.ToString(), new OpenApiResponse
            {
                Description = "Internal Server Error",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/problem+json"] = new OpenApiMediaType
                    {
                        Schema = CreateProblemDetailsSchema()
                    }
                },
            });
        }

        return Task.CompletedTask;
    }


    private static OpenApiSchema CreateProblemDetailsSchema()
    {
        return new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new OpenApiSchema { Type = "string", Nullable = true },
                ["title"] = new OpenApiSchema { Type = "string", Nullable = true },
                ["status"] = new OpenApiSchema { Type = "integer", Format = "int32", Nullable = true },
                ["detail"] = new OpenApiSchema { Type = "string", Nullable = true },
                ["instance"] = new OpenApiSchema { Type = "string", Nullable = true },
                ["traceId"] = new OpenApiSchema { Type = "string", Nullable = true },
                ["method"] = new OpenApiSchema { Type = "string", Nullable = true }
            }
        };
    }

}