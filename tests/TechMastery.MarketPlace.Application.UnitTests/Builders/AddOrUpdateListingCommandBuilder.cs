using Microsoft.AspNetCore.Http.Internal;
using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    // Builder for AddOrUpdateListing command
    internal class AddOrUpdateListingCommandBuilder
    {
        private AddOrUpdateListing _command = new AddOrUpdateListing();

        internal static AddOrUpdateListingCommandBuilder Create()
        {
            return new AddOrUpdateListingCommandBuilder();
        }

        internal AddOrUpdateListingCommandBuilder WithCategory(Category category)
        {
            _command.Category = new CategoryDto { CategoryId = category.CategoryId, Name = category.Name};
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithUploadAssets(int count)
        {
            var artifacts = new List<ProductAssetDto>();
            for (int i = 0; i < count; i++)
            {
                artifacts.Add(new ProductAssetDto
                {
                    FormFile = new FormFile(new MemoryStream(), 0, 0, $"mockFormFile{i}", $"mockFileName{i}"),
                    ArtifactType = new ProductArtifactTypeEnum()
                });
            }
            _command.UploadAssets = artifacts;
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithDependencies(int count)
        {
            var dependencies = new List<ProductDependencyDto>();
            for (int i = 0; i < count; i++)
            {
                dependencies.Add(new ProductDependencyDto
                {
                    Name = $"Dependency {i + 1}",
                    Version = $"{i + 1}.0",
                    DependencyType = (ProductDependencyTypeEnum)new Random().Next(1, Enum.GetValues(typeof(ProductDependencyTypeEnum)).Length)
                });
            }
            _command.Dependencies = dependencies;
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoTags()
        {
            _command.Tags = null;
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoDependencies()
        {
            _command.Dependencies = null;
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoUploadAssets()
        {
            _command.UploadAssets = null;
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithInvalidCategory(Guid categoryId)
        {
            _command.Category = new CategoryDto { CategoryId = categoryId, Name = "Invalid Category" };
            return this;
        }

        internal AddOrUpdateListing Build()
        {
            return _command;
        }
    }
}

