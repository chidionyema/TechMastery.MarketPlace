using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Models.Search;

namespace TechMastery.MarketPlace.Infrastructure.Search
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ILogger<ProductSearchService> _logger;
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;

        public ProductSearchService(ILogger<ProductSearchService> logger, IElasticClient elasticClient, string indexName)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
            _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
        }

        public List<ProductSearch> SearchProducts(ProductSearchRequest request)
        {
            if (request == null)
            {
                _logger.LogError("SearchProducts: Request is null.");
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var query = BuildQuery(request);
                var sortDescriptor = BuildSort(request);

                var searchResponse = _elasticClient.Search<ProductSearch>(s => s
                     .Index(_indexName)
                     .Query(q => query)
                     .Sort(sort => sortDescriptor)
                     .From(request.PageNumber)
                     .Size(request.PageSize)
                 );

                return searchResponse.Documents?.ToList() ?? new List<ProductSearch>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProducts: An error occurred while searching products.");
                return new List<ProductSearch>();
            }
        }

        public async Task IndexProducts(List<ProductSearch> products)
        {
            if (products == null || products.Count == 0)
            {
                _logger.LogWarning("IndexProducts: Empty or null products list.");
                return;
            }

            try
            {
                await _elasticClient.IndexManyAsync(products, _indexName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IndexProducts: An error occurred while indexing products.");
            }
        }

        private QueryContainer BuildQuery(ProductSearchRequest request)
        {
            var query = new BoolQuery();

            void AppendQuery(QueryContainer q) => (query.Must ??= new List<QueryContainer>()).Append(q);

            if (!string.IsNullOrEmpty(request.Query))
            {
                AppendQuery(new MultiMatchQuery
                {
                    Fields = new[] { Infer.Field<ProductSearch>(f => f.Title), Infer.Field<ProductSearch>(f => f.Description) },
                    Query = request.Query
                });
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                AppendQuery(new TermQuery { Field = Infer.Field<ProductSearch>(p => p.Category), Value = request.Category });
            }

            if (request.MinPrice.HasValue && request.MaxPrice.HasValue)
            {
                AppendQuery(new NumericRangeQuery
                {
                    Field = Infer.Field<ProductSearch>(p => p.Price),
                    GreaterThanOrEqualTo = (double)request.MinPrice.Value,
                    LessThanOrEqualTo = (double)request.MaxPrice.Value
                });
            }

            return query;
        }

        private SortDescriptor<ProductSearch> BuildSort(ProductSearchRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("BuildSort: Request is null.");
                return null;
            }

            var sortDescriptor = new SortDescriptor<ProductSearch>();

            if (request.SortBy == "price")
            {
                sortDescriptor.Ascending(p => p.Price);
            }

            // You can add more sorting criteria based on request.SortBy here...

            return sortDescriptor;
        }
    }
}
