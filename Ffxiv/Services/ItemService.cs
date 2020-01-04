using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ffxiv.Common;
using Ffxiv.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ffxiv.Services
{
    public class ItemService
    {
        private readonly DatabaseService _databaseService;
        private readonly IFfxivService _ffxivService;

        public ItemService(IFfxivService ffxivService, DatabaseService databaseService)
        {
            _ffxivService = ffxivService;
            _databaseService = databaseService;
        }

        private async Task<Item> GetItemFromApi(string id, ILogger logger)
        {
            try
            {
                var data = await _ffxivService.GetItem(id);
                var dataDictionary = (IDictionary<string, dynamic>)data;
                var item = new Item();
                item.Id = data.ID;
                item.Name = data.Name;
                item.LocalizedNames = GetLocalizedNames(data);
                item.Recipes = dataDictionary.TryGetNonNullValue("Recipes", out _)
                                   ? ((List<dynamic>)data.Recipes)
                                     .Select(r => new Recipe { Id = r.ID })
                                     .ToList()
                                   : null;
                item.ItemKind = new ItemKind
                                {
                                    Id = data.ItemKind.ID,
                                    Name = data.ItemKind.Name,
                                    LocalizedNames = GetLocalizedNames(data.ItemKind)
                                };
                item.ClassJobCategory = dataDictionary.TryGetNonNullValue("ClassJobCategory", out _)
                                            ? new ClassJobCategory
                                              {
                                                  Name = data.ClassJobCategory.Name,
                                                  LocalizedNames = GetLocalizedNames(data.ClassJobCategory)
                                              }
                                            : null;

                return item;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        private async Task<Recipe> GetRecipeFromApi(long id, ILogger logger)
        {
            try
            {
                var data = await _ffxivService.GetRecipe(id.ToString());
                var recipe = new Recipe
                             {
                                 Id = data.ID,
                                 Name = data.Name,
                                 LocalizedNames = GetLocalizedNames(data),
                                 ResultAmount = data.AmountResult,
                                 Ingredients = GetIngredients(data, logger)
                             };
                return recipe;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        private List<Ingredient> GetIngredients(IDictionary<string, dynamic> data, ILogger logger)
        {
            try
            {
                var ingredients = new List<Ingredient>();

                for (var i = 0; i < 10; i++)
                {
                    if (data[$"ItemIngredient{i}"] == null)
                        continue;

                    var ingredient = new Ingredient
                                     {
                                         Amount = data[$"AmountIngredient{i}"],
                                         ItemId = data[$"ItemIngredient{i}"].ID,
                                         IsCrystal = data[$"ItemIngredient{i}"].ItemUICategory.Name == "Crystal"
                                     };

                    ingredients.Add(ingredient);
                }

                return ingredients;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        private static LocalizedNames GetLocalizedNames(dynamic data)
        {
            return new LocalizedNames { De = data.Name_de, En = data.Name_en, Fr = data.Name_fr, Ja = data.Name_ja };
        }

        public async Task<Item> AddItemToDatabase(string id, ILogger logger, bool isEndProduct = true)
        {
            try
            {
                var item = await GetItemFromApi(id, logger);
                item.IsEndProduct = isEndProduct;

                List<Item> ingredients;

                if (item.Recipes != null)
                {
                    item.Recipes = (await Task.WhenAll(item.Recipes
                                                           .Select(async r => await GetRecipeFromApi(r.Id, logger))))
                        .ToList();

                    ingredients = (await Task.WhenAll(item.Recipes
                                                          .SelectMany(r => r.Ingredients)
                                                          .Select(async i => await AddItemToDatabase(i.ItemId.ToString(), logger, false)))).ToList();
                }

                await _databaseService.UpsertItem(item);

                logger.LogInformation($"Added item: {JsonConvert.SerializeObject(item)}");

                return item;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task RebuildData(List<string> initialIds, ILogger logger)
        {
            await _databaseService.RemoveAll();

            await Task.WhenAll(initialIds.Select(id => AddItemToDatabase(id, logger)));
        }
    }
}