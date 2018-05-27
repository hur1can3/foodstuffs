import Recipe from '../models/recipe';
import RecipesSearchParameters from '../models/recipesSearchParameters';

export default {
  applicationName: '',
  currentRecipe: new Recipe(),
  messages: [],
  fieldsInError: [],
  isError: false,
  recipesList: [],
  recipesListTotalCount: 0,
  recentRecipeIds: [],
  recipesSearchParameters: new RecipesSearchParameters(),
};
