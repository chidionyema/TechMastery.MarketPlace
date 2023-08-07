using TechMastery.MarketPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TechMastery.MarketPlace.Persistence.Seed
{
    public static class CategoryDataSeeder
    {
        public static void SeedInitialCategories(ModelBuilder modelBuilder)
        {
            var categories = new[]
                {
                    new Category("Frontend", CategoryIds.FrontendCategoryId),
                    new Category("Backend", CategoryIds.BackendCategoryId),
                    new Category("DevOps", CategoryIds.DevOpsCategoryId),
                    new Category("Database", CategoryIds.DatabaseCategoryId),
                    new Category("Machine Learning", CategoryIds.MachineLearningCategoryId),
                };

            modelBuilder.Entity<Category>().HasData(categories);
        }

        public static void SeedCategoryDependencies(ModelBuilder modelBuilder)
        {
            var dependencies = new List<CategoryDependency>
    {
        // Frontend Dependencies
        CreateCategoryDependency(CategoryIds.FrontendCategoryId, "React", "17.0.2", CategoryDependencyTypeEnum.Framework),
        CreateCategoryDependency(CategoryIds.FrontendCategoryId, "Angular", "12.0.3", CategoryDependencyTypeEnum.Framework),
        // Add more frontend dependencies

        // Backend Dependencies
        CreateCategoryDependency(CategoryIds.BackendCategoryId, ".NET Core", "6.0", CategoryDependencyTypeEnum.Framework),
        CreateCategoryDependency(CategoryIds.BackendCategoryId, "Node.js", "14.17.6", CategoryDependencyTypeEnum.Framework),
        // Add more backend dependencies

        // DevOps Dependencies
        CreateCategoryDependency(CategoryIds.DevOpsCategoryId, "Docker", "20.10.8", CategoryDependencyTypeEnum.Tool),
        CreateCategoryDependency(CategoryIds.DevOpsCategoryId, "Kubernetes", "1.21.3", CategoryDependencyTypeEnum.Tool),
        // Add more DevOps dependencies

        // Database Dependencies
        CreateCategoryDependency(CategoryIds.DatabaseCategoryId, "PostgreSQL", "13.4", CategoryDependencyTypeEnum.Tool),
        CreateCategoryDependency(CategoryIds.DatabaseCategoryId, "MongoDB", "5.0.2", CategoryDependencyTypeEnum.Tool),
        // Add more Database dependencies

        // Machine Learning Dependencies
        CreateCategoryDependency(CategoryIds.MachineLearningCategoryId, "TensorFlow", "2.7.0", CategoryDependencyTypeEnum.Framework),
        CreateCategoryDependency(CategoryIds.MachineLearningCategoryId, "PyTorch", "1.9.1", CategoryDependencyTypeEnum.Framework),
        // Add more Machine Learning dependencies
    };

            modelBuilder.Entity<CategoryDependency>().HasData(dependencies);
}


        private static CategoryDependency CreateCategoryDependency(Guid categoryId, string name, string version, CategoryDependencyTypeEnum dependencyType)
        {
            return new CategoryDependency(categoryId, name, version, dependencyType);
        }

    }

    public static class CategoryIds
    {
        public static Guid FrontendCategoryId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000001");
        public static Guid BackendCategoryId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000002");
        public static Guid DevOpsCategoryId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000003");
        public static Guid DatabaseCategoryId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000004");
        public static Guid MachineLearningCategoryId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000005");
        // Add more properties as needed for other categories
    }
}
