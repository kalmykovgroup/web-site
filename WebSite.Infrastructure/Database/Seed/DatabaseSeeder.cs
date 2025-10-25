using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Database.Seed
{
    public static class DatabaseSeeder
    {
        public static void SeedCategories(AppDbContext context)
        {
            // Проверяем, есть ли уже данные
            if (context.Categories.Any())
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Сантехника",
                    Description = "Смесители, трубы, фитинги и аксессуары",
                    Color = "#3b82f6",
                    IconUrl = "/images/categories/santehnika/icon.svg",
                    IconAlt = "Сантехника", 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 0,

                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Электрика",
                    Description = "Провода, розетки, выключатели, светильники",
                    Color = "#f59e0b",
                    IconUrl = "/images/categories/elektrika/icon.svg",
                    IconAlt = "Электрика",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 1,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Лакокрасочные",
                    Description = "Краски, лаки, эмали, кисти и валики",
                    Color = "#10b981",
                    IconUrl = "/images/categories/lakokrasochnye/icon.svg",
                    IconAlt = "Лакокрасочные",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 11,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Для кухни",
                    Description = "Посуда, кухонные принадлежности",
                    Color = "#f97316",
                    IconUrl = "/images/categories/kuhnya/icon.svg",
                    IconAlt = "Для кухни",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 2,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Уборка",
                    Description = "Швабры, ведра, щетки, инвентарь",
                    Color = "#8b5cf6",
                    IconUrl = "/images/categories/uborka/icon.svg",
                    IconAlt = "Уборка",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 3,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Бытовая химия",
                    Description = "Моющие средства, порошки, гели",
                    Color = "#06b6d4",
                    IconUrl = "/images/categories/bytovaya-himiya/icon.svg",
                    IconAlt = "Бытовая химия",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 4,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Школьные товары",
                    Description = "Тетради, ручки, рюкзаки, канцелярия",
                    Color = "#6366f1",
                    IconUrl = "/images/categories/shkolnye/icon.svg",
                    IconAlt = "Школьные товары",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 5,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Канцелярия",
                    Description = "Бумага, папки, офисные принадлежности",
                    Color = "#059669",
                    IconUrl = "/images/categories/kancelyariya/icon.svg",
                    IconAlt = "Канцелярия",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 6,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Зарядки и батарейки",
                    Description = "Зарядные устройства, батарейки, кабели",
                    Color = "#ef4444",
                    IconUrl = "/images/categories/zaryadki/icon.svg",
                    IconAlt = "Зарядки и батарейки",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 7,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Товары для праздника",
                    Description = "Свечи, новогодние украшения, детские игрушки",
                    Color = "#ec4899",
                    IconUrl = "/images/categories/prazdnik/icon.svg",
                    IconAlt = "Товары для праздника",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 12,
                    Images = new List<CategoryImage>
                    {
                    }
                },
                // Новые категории
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Инструменты",
                    Description = "Молотки, отвертки, ключи, измерительные приборы",
                    Color = "#f97316",
                    IconUrl = "/images/categories/instrumenty/icon.svg",
                    IconAlt = "Инструменты", 
                    BackgroundUrl = "/images/categories/instrumenty/preview.jpg",
                    BackgroundAlt = "Prewiew инструменты",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = 9,
                    Images = new List<CategoryImage>
                    {
                        new CategoryImage { Url = "/images/categories/instrumenty/image1.jpg", Alt = "Инструменты 1", Order = 1 },
                        new CategoryImage { Url = "/images/categories/instrumenty/image2.jpg", Alt = "Инструменты 2", Order = 2 },
                        new CategoryImage { Url = "/images/categories/instrumenty/image3.jpg", Alt = "Инструменты 3", Order = 3 }
                    }
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Электроинструмент",
                    Description = "Дрели, шуруповерты, пилы, болгарки",
                    Color = "#3b82f6",
                    IconUrl = "/images/categories/elektroinstrument/icon.svg",
                    IconAlt = "Электроинструмент",
                    Order = 10,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Садовый инвентарь",
                    Description = "Лопаты, грабли, секаторы, садовые принадлежности",
                    Color = "#22c55e",
                    IconUrl = "/images/categories/sad/icon.svg",
                    IconAlt = "Садовый инвентарь",
                    Order = 8,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Images = new List<CategoryImage>
                    {
                    }
                }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
