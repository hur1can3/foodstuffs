import trimAndCapitalize from '../filters/trimAndCapitalize';
import Recipe from '../models/recipe';
import applicationInfoApi from '../models/applicationInfoApi';
import recipeApi from '../models/recipeApi';
import sortTypes from '../models/recipeSearchSortTypes';
import webApiCallbacks from '../models/webApiCallbacks';

export default {
  fetchApplicationInfo(context) {
    applicationInfoApi.fetchApplicationInfo(
      (data) => {
        context.commit('setApplicationName', data.applicationName);
        context.commit('setUserName', data.userName);
        webApiCallbacks.setRequestVerificationToken(data.antiforgeryToken);
      },
      (response) => {
        webApiCallbacks.onFailure(context, response);
      },
    );
  },

  fetchRecipes(context, postbackId) {
    recipeApi.listRecipes(
      context.state.recipesSearchParameters,
      data => webApiCallbacks.onFetchListSuccess(context, data, postbackId),
      response => webApiCallbacks.onFailure(context, response),
    );
  },

  deleteRecipe(context, recipe) {
    context.dispatch('clearMessages');
    recipeApi.deleteRecipe(
      recipe,
      data => webApiCallbacks.onSuccess(context, data),
      response => webApiCallbacks.onFailure(context, response),
    );
  },

  saveRecipe(context, recipe) {
    context.dispatch('clearMessages');

    if (recipe.id === undefined || recipe.id < 1) {
      recipeApi.createRecipe(
        recipe,
        data => webApiCallbacks.onSuccess(context, data),
        response => webApiCallbacks.onFailure(context, response),
      );
    } else {
      recipeApi.updateRecipe(
        recipe,
        data => webApiCallbacks.onSuccess(context, data),
        response => webApiCallbacks.onFailure(context, response),
      );
    }
  },

  setCurrentRecipe(context, recipe) {
    context.dispatch('addRecipeToRecents', context.getters.currentRecipe);
    context.commit('setCurrentRecipe', recipe || new Recipe());
  },

  setRecipeName(context, { recipe, value }) {
    context.commit('setRecipeName', { recipe, value });
  },

  setRecipeIngredients(context, { recipe, value }) {
    context.commit('setRecipeIngredients', { recipe, value });
  },

  setRecipeDirections(context, { recipe, value }) {
    context.commit('setRecipeDirections', { recipe, value });
  },

  setRecipePrepTimeMinutes(context, { recipe, value }) {
    context.commit('setRecipePrepTimeMinutes', { recipe, value });
  },

  setRecipeCookTimeMinutes(context, { recipe, value }) {
    context.commit('setRecipeCookTimeMinutes', { recipe, value });
  },

  selectRecipe(context, recipe) {
    context.dispatch('clearMessages');
    context.dispatch('setCurrentRecipe', recipe);
  },

  addRecipeToRecents(context, recipe) {
    if (!context.getters.recipesList.includes(recipe)) {
      return;
    }

    const recentRecipeIds = context.state.recentRecipeIds.slice();
    const recentRecipeIndex = recentRecipeIds.indexOf(recipe.id);

    if (recentRecipeIndex > -1) {
      recentRecipeIds.splice(recentRecipeIndex, 1);
    }
    if (recipe.id > 0) {
      recentRecipeIds.unshift(recipe.id);
    }
    if (recentRecipeIds.length > 3) {
      recentRecipeIds.pop();
    }
    context.commit('setRecentRecipeIds', recentRecipeIds);
  },

  addCategoryToRecipe(context, { recipe, categoryName }) {
    const cleanedCategoryName = trimAndCapitalize(categoryName);

    const categoryDoesNotExist = recipe.categories
      .map(value => value.toUpperCase())
      .indexOf(categoryName.toUpperCase()) < 0;

    if (categoryDoesNotExist && cleanedCategoryName.length > 0) {
      context.commit('addCategoryToRecipe', { recipe, cleanedCategoryName });
    }
  },

  removeCategoryFromRecipe(context, { recipe, categoryName }) {
    const categoryIndex = recipe.categories.indexOf(categoryName);

    if (categoryIndex > -1) {
      context.commit('removeCategoryFromRecipe', { recipe, categoryIndex });
    }
  },

  setRecipesList(context, data) {
    context.commit('setRecipesList', data.items);
    context.commit('setRecipesListTotalCount', data.totalCount);
  },

  setRecipesSearchParametersNameSearch(context, nameSearch) {
    context.commit('setRecipesSearchParametersNameSearch', nameSearch);
  },

  setRecipesSearchParametersCategorySearch(context, categorySearch) {
    context.commit('setRecipesSearchParametersCategorySearch', categorySearch);
  },

  setRecipesSearchParametersPage(context, page) {
    context.commit('setRecipesSearchParametersPage', page);
  },

  setRecipesSearchParametersTake(context, take) {
    context.commit('setRecipesSearchParametersTake', take);
  },

  cycleSelectedNameSortType(context) {
    const currentSortId = sortTypes
      .indexOf(sortTypes
        .filter(type => type.name === context.state.recipesSearchParameters.sort)[0]);
    const newSortId = (currentSortId + 1) % sortTypes.length;
    const newSortName = sortTypes[newSortId].name;
    context.commit('setRecipesSearchParametersSort', newSortName);
    context.dispatch('fetchRecipes');
  },

  clearMessages(context) {
    context.commit('setMessageIsError', false);
    context.commit('setFieldsInError', []);
    context.commit('setMessages', []);
  },
};
