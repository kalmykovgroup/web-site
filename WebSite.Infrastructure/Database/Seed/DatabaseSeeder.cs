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

        public static void SeedSpecialOffers(AppDbContext context)
        {
            // Проверяем, есть ли уже данные
            if (context.SpecialOffers.Any())
                return;

            var specialOffers = new List<SpecialOffer>
            {
                // 1. Летняя распродажа (OVERLAY раскладка)
                new SpecialOffer
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                    Type = "PercentageDiscount",
                    Title = "Летняя распродажа",
                    Description = "Скидки до 50% на все товары для дачи и сада",
                    ImageUrl = "https://images.unsplash.com/photo-1416339442236-8ceb164046f8?w=800&h=600&fit=crop",
                    ImageAlt = "Летняя распродажа",
                    DisplayConfig = @"{
                        ""backgroundColor"": ""#FF6B6B"",
                        ""textColor"": ""#FFFFFF"",
                        ""accentColor"": ""#FF8E53"",
                        ""size"": ""large"",
                        ""showGradient"": true,
                        ""timerPosition"": ""overlay"",
                        ""layout"": ""overlay"",
                        ""overlayConfig"": {
                            ""contentPosition"": ""bottom"",
                            ""contentAlign"": ""left"",
                            ""overlayOpacity"": 0.7,
                            ""overlayColor"": ""0, 0, 0""
                        }
                    }",
                    Timer = @"{
                        ""type"": ""countdown"",
                        ""endDate"": ""2025-12-31T23:59:59Z"",
                        ""showDays"": true,
                        ""showHours"": true,
                        ""showMinutes"": true,
                        ""showSeconds"": false
                    }",
                    Order = 1,
                    IsActive = true,
                    Metadata = @"{""discountPercent"": 50}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // 2. Акция 2+1 (IMAGE_WITH_CONTENT раскладка)
                new SpecialOffer
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
                    Type = "BuyGet",
                    Title = "Акция 2+1",
                    Description = "Купите два товара и получите третий в подарок",
                    ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=800&h=600&fit=crop",
                    ImageAlt = "Акция 2+1",
                    DisplayConfig = @"{
                        ""backgroundColor"": ""#4ECDC4"",
                        ""textColor"": ""#2C3E50"",
                        ""accentColor"": ""#44A08D"",
                        ""size"": ""medium"",
                        ""showGradient"": false,
                        ""timerPosition"": ""bottom"",
                        ""layout"": ""imageWithContent"",
                        ""imageWithContentConfig"": {
                            ""imagePosition"": ""left"",
                            ""imageWidth"": 40
                        }
                    }",
                    Timer = @"{
                        ""type"": ""dateRange"",
                        ""startDate"": ""2025-01-01T00:00:00Z"",
                        ""endDate"": ""2025-12-31T23:59:59Z""
                    }",
                    Order = 2,
                    IsActive = true,
                    Metadata = @"{""buyQuantity"": 2, ""getQuantity"": 1}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // 3. Ограниченная партия (OVERLAY center)
                new SpecialOffer
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440003"),
                    Type = "LimitedOffer",
                    Title = "Ограниченная партия",
                    Description = "Специальная цена на инструменты. Торопитесь, количество ограничено!",
                    ImageUrl = "https://images.unsplash.com/photo-1581091226825-a6a2a5aee158?w=800&h=600&fit=crop",
                    ImageAlt = "Ограниченная партия инструментов",
                    DisplayConfig = @"{
                        ""backgroundColor"": ""#F7B731"",
                        ""textColor"": ""#2C3E50"",
                        ""accentColor"": ""#FD7272"",
                        ""size"": ""medium"",
                        ""showGradient"": false,
                        ""timerPosition"": ""overlay"",
                        ""layout"": ""overlay"",
                        ""overlayConfig"": {
                            ""contentPosition"": ""center"",
                            ""contentAlign"": ""center"",
                            ""overlayOpacity"": 0.6,
                            ""overlayColor"": ""247, 183, 49""
                        }
                    }",
                    Timer = @"{
                        ""type"": ""countdown"",
                        ""endDate"": ""2025-06-30T23:59:59Z"",
                        ""showDays"": true,
                        ""showHours"": true,
                        ""showMinutes"": true,
                        ""showSeconds"": true
                    }",
                    Order = 3,
                    IsActive = true,
                    Metadata = @"{""itemsLeft"": 15, ""totalItems"": 50}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // 4. Скидка 1000₽ (IMAGE_WITH_CONTENT справа)
                new SpecialOffer
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440004"),
                    Type = "FixedDiscount",
                    Title = "Скидка 1000₽",
                    Description = "На все электроинструменты при покупке от 5000₽",
                    ImageUrl = "https://images.unsplash.com/photo-1572981779307-38b8cabb2407?w=800&h=600&fit=crop",
                    ImageAlt = "Скидка на электроинструменты",
                    DisplayConfig = @"{
                        ""backgroundColor"": ""#6C5CE7"",
                        ""textColor"": ""#FFFFFF"",
                        ""accentColor"": ""#A29BFE"",
                        ""size"": ""large"",
                        ""showGradient"": true,
                        ""timerPosition"": ""bottom"",
                        ""layout"": ""imageWithContent"",
                        ""imageWithContentConfig"": {
                            ""imagePosition"": ""right"",
                            ""imageWidth"": 50
                        }
                    }",
                    Timer = @"{""type"": ""none""}",
                    Order = 4,
                    IsActive = true,
                    Metadata = @"{""discountAmount"": 1000, ""originalPrice"": 5000}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // 5. Новинки (IMAGE_ONLY)
                new SpecialOffer
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440005"),
                    Type = "NewArrival",
                    Title = "Новинки поступления",
                    Description = "Свежие товары для строительства и ремонта уже в продаже",
                    ImageUrl = "https://images.unsplash.com/photo-1590595906931-81f04f0ccebb?w=800&h=600&fit=crop",
                    ImageAlt = "Новинки",
                    DisplayConfig = @"{
                        ""backgroundColor"": ""#00B894"",
                        ""textColor"": ""#FFFFFF"",
                        ""accentColor"": ""#00CEC9"",
                        ""size"": ""large"",
                        ""showGradient"": false,
                        ""layout"": ""imageOnly""
                    }",
                    Timer = @"{
                        ""type"": ""dateRange"",
                        ""startDate"": ""2025-01-01T00:00:00Z"",
                        ""endDate"": ""2025-12-31T23:59:59Z""
                    }",
                    Order = 5,
                    IsActive = true,
                    Metadata = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.SpecialOffers.AddRange(specialOffers);
            context.SaveChanges();
        }
    }
}
