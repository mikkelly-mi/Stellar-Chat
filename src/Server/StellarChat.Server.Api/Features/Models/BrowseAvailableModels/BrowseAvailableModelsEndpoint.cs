﻿namespace StellarChat.Server.Api.Features.Models.BrowseAvailableModels;

internal sealed class BrowseAvailableModelsEndpoint : IEndpoint
{
    public void Expose(IEndpointRouteBuilder endpoints)
    {
        var models = endpoints.MapGroup("/models").WithTags("Models");

        models.MapGet("", async ([AsParameters] BrowseAvailableModels query, IMediator mediator) => IEndpoint.Select(await mediator.Send(query)))
            .Produces<AvailableModelsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Retrieves all available models from various providers based on the API keys provided by the user.",
                Description = $"This endpoint fetches a list of available models from various providers based on the API keys provided by the user.\n" +
                    $"\nIt retrieves a combined list of models from all providers, such as OpenAI, Ollama, etc., for which the user has provided API keys." +
                    $"\n### Query Parameters:\n" +
                    $"- **provider** (required with filter): The name of the provider to filter models. Example values: `openai`, `google`, etc.\n" +
                    $"- **filter** (required with provider): The type of models to filter. Example values: `audio`, `completions`, `embeddings`, `images`.\n"
            });
    }

    public void Register(IServiceCollection services, IConfiguration configuration) { }

    public void Use(IApplicationBuilder app) { }
}
