using Core.Model.Actions.Responder;
using Core.Model.Actions.Steps;
using FoodStuffs.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace FoodStuffs.Model.Actions.Recipes
{
    public class ConvertRecipesToListItems : AbstractActionStep
    {
        private List<IRecipeViewModel> _recipesContext;
        private List<IRecipeListItem> _listContext;

        public ConvertRecipesToListItems(List<IRecipeViewModel> recipesContext, List<IRecipeListItem> listContext)
        {
            _recipesContext = recipesContext;
            _listContext = listContext;
        }

        protected override void PerformStep(IActionResponder respond)
        {
            _listContext.Clear();

            _listContext.AddRange(_recipesContext.Select(recipe => new RecipeListItem
            {
                Categories = recipe.Categories,
                Id = recipe.Id,
                Name = recipe.Name
            }));
        }
    }
}