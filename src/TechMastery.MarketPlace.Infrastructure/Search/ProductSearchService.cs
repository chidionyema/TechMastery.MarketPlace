using Nest;
using TechMastery.MarketPlace.Application.Models.Search;
namespace TechMastery.MarketPlace.Infrastructure.Search
{
    public class ProductSearchService
    {
        private readonly ElasticClient _elasticClient;

        public ProductSearchService(ElasticClient elasticClient) { 
            _elasticClient = elasticClient;
        }

        public List<ProductSearch> SearchProducts(Application.Models.Search.SearchRequest request)
        {
            var searchResponse = _elasticClient.Search<ProductSearch>(s => s
                .Index("products")
                .Query(q => BuildQuery(request))
                .Sort(sort => BuildSort(request))
                .From(request.PageNumber)
                .Size(request.PageSize)
            );

            return searchResponse.Documents.ToList();
        }

        public void IndexProducts(List<ProductSearch> products)
        {
            _elasticClient.IndexMany(products, "products");
        }

        private QueryContainer BuildQuery(Application.Models.Search.SearchRequest request)
        {
            var query = new BoolQuery();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query.Must.Append(new MultiMatchQuery
                {
                    Fields = Infer.Field<ProductSearch>(f => f.Title),
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

        private SortDescriptor<ProductSearch> BuildSort(Application.Models.Search.SearchRequest request)
        {
            var sortDescriptor = new SortDescriptor<ProductSearch>();

            if (request.SortBy == "price")
            {
                sortDescriptor.Ascending(p => p.Price);
            }

            return sortDescriptor;
        }
    }
}

