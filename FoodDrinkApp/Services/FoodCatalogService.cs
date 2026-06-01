using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

public static class FoodCatalogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly List<FoodItem> LocalFallbackItems =
    [
        new()
        {
            Name = "Berry Yogurt Bowl",
            Category = "Breakfast",
            Description = "Greek yogurt with mixed berries, oats, and a small drizzle of honey.",
            Calories = 340,
            Protein = 24,
            Carbs = 42,
            Fat = 8,
            AllergyNote = "Contains dairy and gluten.",
            Tags = "healthy breakfast yogurt berries"
        },
        new()
        {
            Name = "Avocado Toast with Eggs",
            Category = "Breakfast",
            Description = "Sourdough toast topped with smashed avocado, poached eggs, and chilli flakes.",
            Calories = 420,
            Protein = 20,
            Carbs = 38,
            Fat = 22,
            AllergyNote = "Contains eggs and gluten.",
            Tags = "breakfast avocado eggs toast"
        },
        new()
        {
            Name = "Chicken Brown Rice Box",
            Category = "Lunch",
            Description = "Grilled chicken breast with brown rice, spinach, cucumber, and lemon dressing.",
            Calories = 520,
            Protein = 38,
            Carbs = 58,
            Fat = 14,
            AllergyNote = "No common allergens recorded.",
            Tags = "meal prep protein lunch"
        },
        new()
        {
            Name = "Mediterranean Falafel Wrap",
            Category = "Lunch",
            Description = "Falafel, hummus, pickled vegetables, and tahini in a wholewheat wrap.",
            Calories = 480,
            Protein = 16,
            Carbs = 56,
            Fat = 20,
            AllergyNote = "Contains sesame and gluten.",
            Tags = "vegetarian lunch falafel wrap"
        },
        new()
        {
            Name = "Trail Mix Energy Bites",
            Category = "Snack",
            Description = "No-bake bites made with oats, peanut butter, dark chocolate chips, and flaxseed.",
            Calories = 220,
            Protein = 8,
            Carbs = 26,
            Fat = 10,
            AllergyNote = "Contains peanuts and gluten.",
            Tags = "snack energy healthy trail mix"
        },
        new()
        {
            Name = "Grilled Salmon with Vegetables",
            Category = "Dinner",
            Description = "Atlantic salmon fillet with roasted asparagus, baby potatoes, and dill sauce.",
            Calories = 550,
            Protein = 42,
            Carbs = 34,
            Fat = 24,
            AllergyNote = "Contains fish.",
            Tags = "dinner seafood salmon protein"
        },
        new()
        {
            Name = "Tomato Wholegrain Pasta",
            Category = "Dinner",
            Description = "Wholegrain pasta with tomato sauce, basil, and roasted vegetables.",
            Calories = 610,
            Protein = 18,
            Carbs = 92,
            Fat = 16,
            AllergyNote = "Contains gluten.",
            Tags = "vegetarian dinner pasta"
        },
        new()
        {
            Name = "Iced Matcha Latte",
            Category = "Drink",
            Description = "Matcha, milk, and ice. A lower-sugar version is recommended.",
            Calories = 180,
            Protein = 8,
            Carbs = 22,
            Fat = 6,
            AllergyNote = "Contains dairy unless plant-based milk is selected.",
            Tags = "drink caffeine matcha latte"
        },
        new()
        {
            Name = "Mango Passion Fruit Smoothie",
            Category = "Drink",
            Description = "Fresh mango, passion fruit, banana, and coconut water blended with ice.",
            Calories = 210,
            Protein = 4,
            Carbs = 48,
            Fat = 2,
            AllergyNote = "No common allergens recorded.",
            Tags = "drink smoothie tropical vegan"
        },
        new()
        {
            Name = "Kung Pao Chicken (宫保鸡丁)",
            Category = "Lunch",
            Description = "Diced chicken with peanuts, dried chillies, and Sichuan peppercorns in a savoury sauce.",
            Calories = 480,
            Protein = 35,
            Carbs = 28,
            Fat = 22,
            AllergyNote = "Contains peanuts and soy.",
            Tags = "Chinese Sichuan chicken spicy lunch"
        },
        new()
        {
            Name = "Xiaolongbao (小笼包)",
            Category = "Snack",
            Description = "Steamed soup dumplings filled with minced pork and a savoury broth.",
            Calories = 360,
            Protein = 16,
            Carbs = 42,
            Fat = 14,
            AllergyNote = "Contains gluten and pork.",
            Tags = "Chinese dumpling dim sum snack"
        },
        new()
        {
            Name = "Bubble Milk Tea (珍珠奶茶)",
            Category = "Drink",
            Description = "Classic Taiwanese milk tea with chewy tapioca pearls.",
            Calories = 320,
            Protein = 4,
            Carbs = 56,
            Fat = 8,
            AllergyNote = "Contains dairy. Tapioca pearls may pose a choking risk for young children.",
            Tags = "drink bubble tea taiwanese boba"
        },
        new()
        {
            Name = "Malatang (麻辣烫)",
            Category = "Dinner",
            Description = "Spicy Sichuan-style hot pot with assorted vegetables, tofu, and meat in a numbing broth.",
            Calories = 550,
            Protein = 28,
            Carbs = 45,
            Fat = 26,
            AllergyNote = "May contain soy, sesame, and peanut. Ask about broth ingredients.",
            Tags = "Chinese Sichuan spicy hotpot dinner"
        },
        new()
        {
            Name = "Egg Fried Rice (蛋炒饭)",
            Category = "Lunch",
            Description = "Classic home-style fried rice with egg, spring onion, and a touch of sesame oil.",
            Calories = 420,
            Protein = 14,
            Carbs = 58,
            Fat = 14,
            AllergyNote = "Contains eggs and soy. Gluten-free when made with tamari.",
            Tags = "Chinese rice egg lunch fried"
        },
        new()
        {
            Name = "Douhua (豆花)",
            Category = "Snack",
            Description = "Silky soft tofu pudding served with a light ginger syrup or savoury soy dressing.",
            Calories = 180,
            Protein = 10,
            Carbs = 24,
            Fat = 5,
            AllergyNote = "Contains soy.",
            Tags = "Chinese tofu dessert snack sweet"
        }
    ];

    private static List<FoodItem> cachedItems = new(LocalFallbackItems);

    public static bool LastLoadUsedMockApi { get; private set; }

    public static async Task<IReadOnlyList<FoodItem>> SearchAsync(string? query)
    {
        var items = await GetAllAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return items.OrderBy(item => item.Name).ToList();
        }

        var normalised = query.Trim();
        return items
            .Where(item =>
                item.Name.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Tags.Contains(normalised, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Name)
            .ToList();
    }

    public static async Task<FoodItem?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodItem>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                    JsonOptions);

                if (item is not null)
                {
                    return item;
                }
            }
            catch
            {
                // Fall back to the last loaded cache below.
            }
        }

        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    public static async Task<FoodItem> AddAsync(FoodItem item)
    {
        if (MockApiConfig.IsConfigured)
        {
            var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item, JsonOptions);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<FoodItem>(JsonOptions);
            if (created is not null)
            {
                cachedItems.Add(created);
                return created;
            }
        }

        cachedItems.Add(item);
        return item;
    }

    private static async Task<IReadOnlyList<FoodItem>> GetAllAsync()
    {
        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            return cachedItems;
        }

        try
        {
            var items = await HttpClient.GetFromJsonAsync<List<FoodItem>>(MockApiConfig.EndpointUrl, JsonOptions);
            if (items is { Count: > 0 })
            {
                cachedItems = items;
                LastLoadUsedMockApi = true;
                return cachedItems;
            }
        }
        catch
        {
            // Keep the app usable during demos even if the network is unavailable.
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }
}
