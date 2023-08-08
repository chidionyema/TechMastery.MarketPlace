using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
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
            var searchResponse = _elasticClient.Search<ProductSearch>(s => s
                .Index(_indexName)
                .Query(q => BuildQuery(request))
                .Sort(sort => BuildSort(request))
                .From(request.PageNumber)
                .Size(request.PageSize)
            );

            return searchResponse.Documents.ToList();
        }

        public async Task IndexProducts(List<ProductSearch> products)
        {
            await _elasticClient.IndexManyAsync(products, _indexName);
        }

        private QueryContainer BuildQuery(ProductSearchRequest request)
        {
            var query = new BoolQuery();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query.Must.Append(new MultiMatchQuery
                {
                    Fields = new[] { Infer.Field<ProductSearch>(f => f.Title), Infer.Field<ProductSearch>(f => f.Description) },
                    Query = request.Query
                });
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                query.Must.Append(new TermQuery { Field = Infer.Field<ProductSearch>(p => p.Category), Value = request.Category });
            }

            if (request.MinPrice.HasValue && request.MaxPrice.HasValue)
            {
                query.Must.Append(new NumericRangeQuery
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
