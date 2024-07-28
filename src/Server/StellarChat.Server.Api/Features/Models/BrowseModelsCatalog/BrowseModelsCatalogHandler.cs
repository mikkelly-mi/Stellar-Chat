﻿using Microsoft.Extensions.Caching.Memory;
using StellarChat.Server.Api.Features.Models.BrowseModelsCatalog.Catalogs;
using System.Collections.Concurrent;
using System.Text;

namespace StellarChat.Server.Api.Features.Models.BrowseModelsCatalog;

internal sealed class BrowseModelsCatalogHandler : IQueryHandler<BrowseModelsCatalog, ModelCatalogResponse?>
{
    private readonly IEnumerable<IModelCatalog> _modelCatalogs;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeProvider _clock;


    private readonly ConcurrentBag<string> _activeCacheKeys = [];

    public BrowseModelsCatalogHandler(IEnumerable<IModelCatalog> modelCatalogs, IMemoryCache memoryCache, TimeProvider clock)
    {
        _modelCatalogs = modelCatalogs;
        _memoryCache = memoryCache;
        _clock = clock;
    }

    public async ValueTask<ModelCatalogResponse?> Handle(BrowseModelsCatalog query, CancellationToken cancellationToken)
    {
        var cacheKey = GenerateCacheKey(query);
        _activeCacheKeys.Add(cacheKey);

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);

            var models = await FetchModelsFromProvider(query, cancellationToken) ?? [];
            var now = _clock.GetLocalNow();

            return new ModelCatalogResponse
            {
                Models = models,
                LastFetched = now
            };
        });
    }

    private async Task<IEnumerable<ModelCatalog>> FetchModelsFromProvider(BrowseModelsCatalog query, CancellationToken cancellationToken)
    {
        var availableModels = new List<ModelCatalog>();

        foreach (var provider in _modelCatalogs)
        {
            var providerName = provider.ProviderName;
            var models = await provider.FetchModelsAsync(query, cancellationToken);

            if (query.Provider is not null
                && query.Provider.Equals(providerName, StringComparison.OrdinalIgnoreCase)
                && query.Filter is not null)
            {
                models = provider.FilterModels(query.Filter, models);
            }

            availableModels.AddRange(models);
        }
        return availableModels;
    }

    private string GenerateCacheKey(BrowseModelsCatalog query)
    {
        var keyBuilder = new StringBuilder("ModelCatalog");

        if (query.Provider != null) keyBuilder.Append($"_{query.Provider}");
        if (query.Filter != null) keyBuilder.Append($"_{query.Filter}");

        var cacheKey = keyBuilder.ToString();

        return cacheKey;
    }
}
