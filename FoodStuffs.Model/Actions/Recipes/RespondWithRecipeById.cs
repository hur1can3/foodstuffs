﻿using Core.Model.Actions.Responder;
using Core.Model.Actions.Steps;
using FoodStuffs.Model.Interfaces.Services.Data;
using System.Linq;

namespace FoodStuffs.Model.Actions.Recipes
{
    public class RespondWithRecipeById : ActionStep

    {
        public RespondWithRecipeById(IFoodStuffsData data, int id)
        {
            _data = data;
            _id = id;
        }

        protected override void PerformStep(IActionResponder respond)
        {
            respond.WithData(_data.Recipes.Stored.FirstOrDefault(recipe => recipe.Id == _id));
        }

        private readonly IFoodStuffsData _data;
        private readonly int _id;
    }
}