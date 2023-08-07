using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    // Builder for Category entity
    internal class CategoryBuilder
    {
        private Category _category = new Category();

        internal static CategoryBuilder Create()
        {
            return new CategoryBuilder();
        }

        internal CategoryBuilder WithName(string name)
        {
            _category.SetName(name);
            return this;
        }

        internal Category Build()
        {
            return _category;
        }
    }
}

