using System;
using System.Collections.Generic;
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
        private AddListingCommand _command = new AddListingCommand();

        internal static AddOrUpdateListingCommandBuilder Create()
        {
            return new AddOrUpdateListingCommandBuilder();
        }

        internal AddOrUpdateListingCommandBuilder WithCategory(Category category)
        {
            _command = _command with { Category = new CategoryDto { CategoryId = category.CategoryId, Name = category.Name } };
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

        internal AddOrUpdateListingCommandBuilder WithDependencies(int count)
        {
            var dependencies = new List<ProductDependencyDto>();
            for (int i = 0; i < count; i++)
            {
                dependencies.Add(new ProductDependencyDto
                {
                    Name = $"Dependency {i + 1}",
                    Version = $"{i + 1}.0",
                    DependencyType = (DependencyTypeEnum)new Random().Next(1, Enum.GetValues(typeof(DependencyTypeEnum)).Length)
                });
            }
            _command = _command with { Dependencies = dependencies };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoTags()
        {
            _command = _command with { Tags = null };
            return this;
        }

        internal AddOrUpdateListingCommandBuilder WithNoDependencies()
        {
            _command = _command with { Dependencies = null };
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
