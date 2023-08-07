using System;
namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
	public interface IStorageProvider
	{
        Task CreateBlobFolderStructureAsync(string username, string artifactType);

    }
}

