using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    // Builder for AddOrUpdateListing command
    internal class AddOrUpdateListingCommandBuilder
    {
        private AddListingCommand _command = new("product name", "product desc", 1.0m, 1.0m, new CategoryDto());

        internal static AddOrUpdateListingCommandBuilder Create()
        {
            return new AddOrUpdateListingCommandBuilder();
        }

        internal AddOrUpdateListingCommandBuilder WithCategory(Category category)
        {
            _command = _command with { Category = new CategoryDto { CategoryId = category.Id, Name = category.Name } };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithLanguage(Guid languageId)
        {
            _command = _command with { LanguageIds = new List<Guid> { languageId} };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithFramework(Guid frameworkId)
        {
            _command = _command with { FrameworkIds = new List<Guid> { frameworkId } };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithPlatform(Guid platformId)
        {
            _command = _command with { PlatformIds = new List<Guid> { platformId } };
            return this;
        }


        internal AddOrUpdateListingCommandBuilder WithUploadAssets(int count)
        {
            var artifacts = new List<ProductAssetDto>();
            for (int i = 0; i < count; i++)
            {
                artifacts.Add(new ProductAssetDto
                {
                    BlobUrl = "blob",
                    ArtifactType = new ProductArtifactTypeEnum()
                });
            }
            _command = _command with { UploadAssets = artifacts };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoTags()
        {
            _command = _command with { Tags = null };
            return this;
        }
        internal AddOrUpdateListingCommandBuilder WithNoUploadAssets()
        {
            _command = _command with { UploadAssets = null };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithInvalidCategory(Guid categoryId)
        {
            _command = _command with { Category = new CategoryDto { CategoryId = categoryId, Name = "Invalid Category" } };
            return this;
        }

        internal AddListingCommand Build()
        {

            return _command;
        }
    }
}
