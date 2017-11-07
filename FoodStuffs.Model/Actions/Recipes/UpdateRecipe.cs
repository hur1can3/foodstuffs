﻿using FoodStuffs.Model.Actions.Core.Responder;
using FoodStuffs.Model.Actions.Core.Steps;
using FoodStuffs.Model.Interfaces.Domain;
using FoodStuffs.Model.Interfaces.Services.Data;
using FoodStuffs.Model.Interfaces.Services.DateTime;
using FoodStuffs.Model.Queries;
using FoodStuffs.Model.Validation.Core;
using FoodStuffs.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace FoodStuffs.Model.Actions.Recipes
{
    public class UpdateRecipe : ActionStep
    {
        public UpdateRecipe(IFoodStuffsData data, IDateTimeService now, RecipeViewModel viewModel, int userIdId)
        {
            _data = data;
            _now = now;
            _userId = userIdId;
            _viewModel = viewModel;
        }

        protected override void PerformStep(IActionResponder respond)
        {
            var savedRecipe = _data.Recipes.Stored.GetById(_viewModel.Id);

            if (savedRecipe == null)
            {
                respond.WithValidationErrors($"RecipeId: {_viewModel.Id}",
                    new ValidationError("Recipe does not exist."));
                return;
            }

            savedRecipe.ModifiedOn = _now.Moment;
            savedRecipe.ModifiedByUserId = _userId;
            savedRecipe.CookTimeMinutes = _viewModel.CookTimeMinutes;
            savedRecipe.PrepTimeMinutes = _viewModel.PrepTimeMinutes;
            savedRecipe.Directions = _viewModel.Directions;
            savedRecipe.Ingredients = _viewModel.Ingredients;
            savedRecipe.Name = _viewModel.Name;

            CleanupCategories(savedRecipe);

            AddCategoriesAndCategoryRecipes(savedRecipe);
            _data.SaveChanges();
        }

        private readonly IFoodStuffsData _data;
        private readonly IDateTimeService _now;
        private readonly int _userId;
        private readonly RecipeViewModel _viewModel;

        private static IEnumerable<ICategory> FindUnusedCategories(IRecipe recipe, IEnumerable<ICategoryRecipe> unusedCategoryRecipes)
        {
            var categories = unusedCategoryRecipes.Select(cr => cr.Category);

            foreach (var category in categories)
            {
                if (category.CategoryRecipe.All(cr => cr.RecipeId == recipe.Id))
                {
                    yield return category;
                }
            }
        }

        private static IEnumerable<ICategoryRecipe> FindUnusedCategoryRecipes(IRecipe recipe, RecipeViewModel viewModel)
        {
            var newCategoryNames = viewModel.Categories.Select(c => c.ToUpper().Trim()).ToList();

            var unusedCategoryRecipes =
                recipe.CategoryRecipe.Where(cr => !newCategoryNames.Contains(cr.Category.Name.ToUpper().Trim()));

            return unusedCategoryRecipes;
        }

        private void AddCategoriesAndCategoryRecipes(IRecipe recipe)
        {
            foreach (var viewModelCategory in _viewModel.Categories)
            {
                var existingCategory = _data.Categories.Stored.GetByName(viewModelCategory) ?? CreateCategory(viewModelCategory);

                var existingCategoryRecipe = _data.CategoryRecipes.Stored.GetById(recipe.Id, existingCategory.Id);

                if (existingCategoryRecipe == null)
                {
                    CreateCategoryRecipe(recipe, existingCategory);
                }
            }
        }

        private void CleanupCategories(IRecipe recipe)
        {
            var unusedCategoryRecipes = FindUnusedCategoryRecipes(recipe, _viewModel).ToList();
            var unusedCategories = FindUnusedCategories(recipe, unusedCategoryRecipes).ToList();

            _data.CategoryRecipes.RemoveRange(unusedCategoryRecipes);
            _data.Categories.RemoveRange(unusedCategories);
        }

        private ICategory CreateCategory(string viewModelCategory)
        {
            var existingCategory = _data.Categories.New;
            existingCategory.Name = viewModelCategory;
            _data.Categories.Add(existingCategory);
            return existingCategory;
        }

        private void CreateCategoryRecipe(IRecipe recipe, ICategory existingCategory)
        {
            var categoryRecipe = _data.CategoryRecipes.New;
            categoryRecipe.RecipeId = recipe.Id;
            categoryRecipe.CategoryId = existingCategory.Id;
            _data.CategoryRecipes.Add(categoryRecipe);
        }
    }
}