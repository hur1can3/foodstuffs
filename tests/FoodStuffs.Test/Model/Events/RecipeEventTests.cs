using FoodStuffs.Model.Data.Models;
using FoodStuffs.Model.Data.Queries;
using FoodStuffs.Model.Events.Recipes;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FoodStuffs.Test.Model.Events
{
    public class RecipeEventTests
    {
        [Fact]
        public async Task GetRecipeFound()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new GetRecipe.Handler(data)
                    .Handle(new GetRecipe.Request(11));

                Assert.True(result.IsSuccess);
                Assert.Equal(11, result.Value.Id);
                Assert.Equal("Recipe1", result.Value.Name);
                Assert.Equal("ing", result.Value.Ingredients);
                Assert.Equal("dir", result.Value.Directions);
                Assert.Equal(21, result.Value.CookTimeMinutes);
                Assert.Equal(2, result.Value.PrepTimeMinutes);
                Assert.Equal("11", result.Value.CreatedBy);
                Assert.Equal(Deps.DateTimeServiceEarly.Moment, result.Value.CreatedOn);
                Assert.Equal("12", result.Value.ModifiedBy);
                Assert.Equal(Deps.DateTimeServiceLate.Moment, result.Value.ModifiedOn);
                Assert.Equal(2, result.Value.Categories.Count());
            }
        }

        [Fact]
        public async Task GetRecipeNotFound()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new GetRecipe.Handler(data)
                    .Handle(new GetRecipe.Request(1000));

                Assert.True(result.IsFailed);
            }
        }

        [Fact]
        public async Task ListRecipes()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, null, null, true, 2, 1));

                Assert.True(result.IsSuccess);
                Assert.Equal(1, result.Value.Count);
                Assert.Equal(3, result.Value.TotalCount);
                Assert.Equal(2, result.Value.Page);
                Assert.Equal(1, result.Value.Take);
                Assert.Contains("Recipe2", result.Value.Items.Select(r => r.Name));
                Assert.Contains(12, result.Value.Items.Select(r => r.Id));
                Assert.Contains("Category1", result.Value.Items.SelectMany(r => r.Categories));
            }
        }

        [Fact]
        public async Task ListRecipesWithoutPaging()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, null, null, false, 0, 0));

                Assert.True(result.IsSuccess);
                Assert.Equal(3, result.Value.Count);
                Assert.Equal(3, result.Value.TotalCount);
                Assert.Contains("Recipe2", result.Value.Items.Select(r => r.Name));
                Assert.Contains(12, result.Value.Items.Select(r => r.Id));
                Assert.Contains("Category1", result.Value.Items.SelectMany(r => r.Categories));
            }
        }

        [Fact]
        public async Task ListRecipesSortDesc()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, null, "nameDesc", true, 1, 1));

                Assert.True(result.IsSuccess);
                Assert.Equal(1, result.Value.Count);
                Assert.Equal(3, result.Value.TotalCount);
                Assert.Equal(1, result.Value.Page);
                Assert.Equal(1, result.Value.Take);
                Assert.Contains("Recipe3", result.Value.Items.Select(r => r.Name));
            }
        }

        [Fact]
        public async Task ListRecipesSortAscend()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                await data.Recipes.Add(new Recipe { Name = "ANewRecipe" });

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, null, "name", true, 1, 1));

                Assert.True(result.IsSuccess);
                Assert.Equal(1, result.Value.Count);
                Assert.Equal(4, result.Value.TotalCount);
                Assert.Equal(1, result.Value.Page);
                Assert.Equal(1, result.Value.Take);
                Assert.Contains("ANewRecipe", result.Value.Items.Select(r => r.Name));
            }
        }

        [Fact]
        public async Task ListRecipesNameSearch()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request("recipe2", null, null, true, 1, 2));

                Assert.True(result.IsSuccess);
                Assert.Equal(1, result.Value.Count);
                Assert.Equal(1, result.Value.TotalCount);
                Assert.Equal(1, result.Value.Page);
                Assert.Equal(2, result.Value.Take);
                Assert.Contains("Recipe2", result.Value.Items.Select(r => r.Name));
            }
        }

        [Fact]
        public async Task ListRecipesCategorySearch()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, "cat", null, true, 1, 4));

                Assert.True(result.IsSuccess);
                Assert.Equal(2, result.Value.Count);
                Assert.Equal(2, result.Value.TotalCount);
                Assert.Equal(1, result.Value.Page);
                Assert.Equal(4, result.Value.Take);
                Assert.Contains("Recipe1", result.Value.Items.Select(r => r.Name));
                Assert.Contains("Recipe2", result.Value.Items.Select(r => r.Name));
                Assert.DoesNotContain("Recipe3", result.Value.Items.Select(r => r.Name));
            }
        }

        [Fact]
        public async Task ListRecipesNoneFoundByNameSearch()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request("nothing matches", null, null, true, 1, 2));

                Assert.True(result.IsSuccess);
                Assert.Equal(0, result.Value.Count);
                Assert.Equal(0, result.Value.TotalCount);
            }
        }

        [Fact]
        public async Task ListRecipesNoneFoundByCategorySearch()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new ListRecipes.Handler(data)
                    .Handle(new ListRecipes.Request(null, "nothing matches", null, true, 1, 2));

                Assert.True(result.IsSuccess);
                Assert.Equal(0, result.Value.Count);
                Assert.Equal(0, result.Value.TotalCount);
            }
        }

        [Fact]
        public async Task DeleteRecipeFound()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new DeleteRecipe.Handler(data)
                    .Handle(new DeleteRecipe.Request(11));

                var maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(11));

                Assert.True(result.IsSuccess);
                Assert.True(maybeRecipe.HasNoValue);
                Assert.Equal(11, result.Value.Id);
            }
        }

        [Fact]
        public async Task DeleteRecipeNotFound()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new DeleteRecipe.Handler(data)
                    .Handle(new DeleteRecipe.Request(1000));

                var maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(1000));

                Assert.True(result.IsFailed);
                Assert.True(maybeRecipe.HasNoValue);
            }

        }

        [Fact]
        public async Task SaveNewRecipe()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var result = await new SaveRecipe.Handler(data)
                    .Handle(new SaveRecipe.Request(0, "New", "New", "New", null, 20, new[] { "Category2", "Category3", "Category4" }));

                Assert.True(result.IsSuccess);
                Assert.True(result.Value.Id > 0);

                var maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(result.Value.Id));
                Assert.True(maybeRecipe.HasValue);
                Assert.Equal(Deps.DateTimeServiceLate.Moment, maybeRecipe.Value.CreatedOn);
                Assert.Equal(Deps.DateTimeServiceLate.Moment, maybeRecipe.Value.ModifiedOn);
                Assert.DoesNotContain("Category1", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category2", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category3", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category4", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));

                maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(12));
                Assert.True(maybeRecipe.HasValue);
                Assert.Contains("Category1", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category2", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category3", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category4", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
            }

        }

        [Fact]
        public async Task SaveExistingRecipe()
        {
            using (var context = Deps.FoodStuffsContext())
            {
                var data = context.Seed().FoodStuffsData();

                var maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(11));
                Assert.True(maybeRecipe.HasValue);

                var result = await new SaveRecipe.Handler(data)
                    .Handle(new SaveRecipe.Request(11, "New", "New", "New", null, 20, new[] { "Category2", "Category3", "Category4" }));

                Assert.True(result.IsSuccess);
                Assert.Equal(11, result.Value.Id);

                maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(11));
                Assert.True(maybeRecipe.HasValue);
                Assert.Equal(Deps.DateTimeServiceEarly.Moment, maybeRecipe.Value.CreatedOn);
                Assert.Equal(Deps.DateTimeServiceLate.Moment, maybeRecipe.Value.ModifiedOn);
                Assert.DoesNotContain("Category1", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category2", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category3", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.Contains("Category4", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));

                maybeRecipe = await data.Recipes.Get(new RecipesByIdWithCategoriesSpecification(12));
                Assert.True(maybeRecipe.HasValue);
                Assert.Contains("Category1", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category2", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category3", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
                Assert.DoesNotContain("Category4", maybeRecipe.Value.CategoryRecipe.Select(cr => cr.Category.Name));
            }
        }
    }
}
