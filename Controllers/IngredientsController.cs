using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class IngredientsController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly int tagsTableId = 1;

    public IngredientsController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientListItemDTO>>> GetIngredients(){
      var ingrMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Ingredient, IngredientListItemDTO>()));
      IEnumerable<Ingredient> ingredients = _context.Ingredients.ToList();
      return Ok(ingrMapper.Map<List<IngredientListItemDTO>>(ingredients));
    }
    [HttpGet("searchIngredients/{name}")]
    public ActionResult<IngredientListItemDTO> GetIngredientListItems(string name){      
      var ingredientQuery =
        from i in _context.Ingredients
        where i.Name.ToLower().Contains(name.ToLower())
        select new IngredientListItemDTO
        {
          Id = i.Id,
          Name = i.Name,
        };

      return Ok(ingredientQuery.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<IngredientDTO> GetIngredient(int id){      
      var ingredientQuery = _context.Ingredients
        .Where(i => i.Id == id)
        .Select( i => new IngredientDTO{
          Id = i.Id,
          Name = i.Name,
          Brand = i.Brand,
          Description = i.Description,
          EAN = i.EAN,
          Image = i.Image,
          Macro = new MacroDTO {
            Kcal = i.Kcal,
            Carbohydrates = i.Carbohydrates,
            Fat = i.Fat,
            Fibers = i.Fibers,
            Proteins = i.Proteins,
            Cholesterol = i.Cholesterol
          },
          Micro = new MicroDTO{
            Calcium = i.Calcium,
            Iron = i.Iron,
            Magnesium = i.Magnesium,             
            Potassium = i.Potassium,           
            Sodium = i.Sodium,
            VitaminA = i.VitaminA,
            VitaminB12 = i.VitaminB12,
            VitaminB6 = i.VitaminB6,
            VitaminC = i.VitaminC,
            VitaminD = i.VitaminD
          },
          Tags = _context.Tags
            .Where(t => t.TableId == tagsTableId)
            .Where(t => t.ItemId == i.Id)
            .Select(t => new TagReadDTO{
              Id = _context.TagNames
                .Where(tn => tn.Id == t.NameId)
                .Select(tn => tn.Id)
                .FirstOrDefault(),
              Name = _context.TagNames
                .Where(tn => tn.Id == t.NameId)
                .Select(tn => tn.Name)
                .FirstOrDefault()
            }).ToList(),
          Portions = _context.IngredientPortions
            .Where(p => p.IngredientId == i.Id)
            .Select(p=> new IngredientPortionDTO{
              Id = p.Id,
              Name = _context.PortionNames.Where(x=>x.Id==p.PortionNameId).Select(x=>x.Name).SingleOrDefault(),
              Quantity = p.Quantity
            }).ToList()
        });
  
      return Ok(ingredientQuery.FirstOrDefault());
    }

    [HttpPost("addingredient")]
    public async Task<ActionResult<IngredientDTO>> AddIngredient(IngredientDTO ingredientDTO)
    {

      var ingr = new Ingredient{
        Brand = ingredientDTO.Brand,
        Name = ingredientDTO.Name,
        EAN = ingredientDTO.EAN,
        Image = ingredientDTO.Image,
        Description = ingredientDTO.Description,
        Fat = ingredientDTO.Macro.Fat,
        Carbohydrates = ingredientDTO.Macro.Carbohydrates,
        Proteins = ingredientDTO.Macro.Proteins,
        Cholesterol = ingredientDTO.Macro.Cholesterol,
        Fibers = ingredientDTO.Macro.Fibers,
        Kcal = ingredientDTO.Macro.Kcal,
        Calcium = ingredientDTO.Micro.Calcium,
        Iron = ingredientDTO.Micro.Iron,
        Magnesium = ingredientDTO.Micro.Magnesium,
        Potassium = ingredientDTO.Micro.Potassium,
        Sodium = ingredientDTO.Micro.Sodium,
        VitaminA = ingredientDTO.Micro.VitaminA,
        VitaminB12 = ingredientDTO.Micro.VitaminB12,
        VitaminB6 = ingredientDTO.Micro.VitaminB6,
        VitaminC = ingredientDTO.Micro.VitaminC,
        VitaminD = ingredientDTO.Micro.VitaminD
      };

      _context.Ingredients.Add(ingr);
      await _context.SaveChangesAsync();

      foreach(TagReadDTO tag in ingredientDTO.Tags){
        var t = new Tag{
          Id = 0,
          ItemId = ingr.Id, 
          TableId = tagsTableId, 
          NameId = _context.TagNames
            .Where(tn => tn.Name == tag.Name)
            .Select(tn => tn.Id)
            .FirstOrDefault()
        };
        _context.Tags.Add(t);
      }

      foreach(IngredientPortionDTO portion in ingredientDTO.Portions){
        var p = new IngredientPortion{
          Id = 0,
          PortionNameId = _context.PortionNames.Where(x=>x.Name==portion.Name).Select(x=> x.Id).SingleOrDefault(),
          IngredientId = ingr.Id,
          Quantity = portion.Quantity
        };
        _context.IngredientPortions.Add(p);
      }

      await _context.SaveChangesAsync();
      return Ok(ingredientDTO);
    }


[HttpPost("updateIngredient")]
public IngredientDTO UpdateIngredient(IngredientDTO ingredientDTO)
{
    var ingr = _context.Ingredients.SingleOrDefault(x => x.Id == ingredientDTO.Id);
    if (ingr != null)
    {
        // Update ingredient properties
        ingr.Name = ingredientDTO.Name;
        ingr.Brand = ingredientDTO.Brand;
        ingr.Description = ingredientDTO.Description;
        ingr.EAN = ingredientDTO.EAN;
        ingr.Image = ingredientDTO.Image;
        ingr.Kcal = ingredientDTO.Macro.Kcal;
        ingr.Carbohydrates = ingredientDTO.Macro.Carbohydrates;
        ingr.Fat = ingredientDTO.Macro.Fat;
        ingr.Fibers = ingredientDTO.Macro.Fibers;
        ingr.Proteins = ingredientDTO.Macro.Proteins;
        ingr.Cholesterol = ingredientDTO.Macro.Cholesterol;
        ingr.Calcium = ingredientDTO.Micro.Calcium;
        ingr.Iron = ingredientDTO.Micro.Iron;
        ingr.Magnesium = ingredientDTO.Micro.Magnesium;
        ingr.Potassium = ingredientDTO.Micro.Potassium;
        ingr.Sodium = ingredientDTO.Micro.Sodium;
        ingr.VitaminA = ingredientDTO.Micro.VitaminA;
        ingr.VitaminB12 = ingredientDTO.Micro.VitaminB12;
        ingr.VitaminB6 = ingredientDTO.Micro.VitaminB6;
        ingr.VitaminC = ingredientDTO.Micro.VitaminC;
        ingr.VitaminD = ingredientDTO.Micro.VitaminD;

        // Update tags
        var contextIngrTags = _context.Tags
            .Where(x => x.ItemId == ingredientDTO.Id)
            .ToList();

        // Delete excess tags
        if (ingredientDTO.Tags.Count() < contextIngrTags.Count())
        {
            var tagsToDelete = contextIngrTags.Skip(ingredientDTO.Tags.Count()).ToList();
            _context.Tags.RemoveRange(tagsToDelete);
        }

        // Update or add tags
        for (int i = 0; i < ingredientDTO.Tags.Count(); i++)
        {
            var ingrTag = ingredientDTO.Tags[i];
            var ingrTagNameId = _context.TagNames
                .SingleOrDefault(x => x.Name == ingrTag.Name)?.Id;

            if (ingrTagNameId == null) // Tag name doesn't exist in database
            {
                var tagNameToAdd = new TagName { Name = ingrTag.Name };
                _context.TagNames.Add(tagNameToAdd);
                _context.SaveChanges();
                ingrTagNameId = tagNameToAdd.Id;
            }

            if (i < contextIngrTags.Count()) // Tag exists in database
            {
                if (contextIngrTags[i].NameId != ingrTagNameId) // Tag has changed
                {
                    contextIngrTags[i].NameId = ingrTagNameId.Value;
                    _context.Tags.Update(contextIngrTags[i]);
                }
            }
            else // Tag doesn't exist in database
            {
                var tagToAdd = new Tag
                {
                    ItemId = ingredientDTO.Id,
                    TableId = tagsTableId,
                    NameId = ingrTagNameId.Value
                };
                _context.Tags.Add(tagToAdd);
            }
        }



        var contextIngrPortions = _context.IngredientPortions
            .Where(x => x.IngredientId == ingredientDTO.Id)
            .ToList();


        if (ingredientDTO.Portions.Count() < contextIngrPortions.Count())
        {
            var portionsToDelete = contextIngrPortions.Skip(ingredientDTO.Portions.Count()).ToList();
            _context.IngredientPortions.RemoveRange(portionsToDelete);
        }

        for (int i = 0; i < ingredientDTO.Portions.Count(); i++)
        {
            var portion = ingredientDTO.Portions[i];
            
            if (i < contextIngrPortions.Count()) 
            {
                if (contextIngrPortions[i].Quantity != portion.Quantity)
                {
                    contextIngrPortions[i].Quantity = portion.Quantity;
                    _context.IngredientPortions.Update(contextIngrPortions[i]);
                }
            }
            else
            {
                var portionToAdd = new IngredientPortion
                {
                  Id = portion.Id,
                  IngredientId = ingredientDTO.Id,
                  PortionNameId = _context.PortionNames.Where(x=>x.Name==portion.Name).Select(x=> x.Id).SingleOrDefault(),
                  Quantity = portion.Quantity
                };
                _context.IngredientPortions.Add(portionToAdd);
            }
        }

        _context.Update(ingr);
        _context.SaveChanges();
    }
    return ingredientDTO;
}

    [HttpDelete("deleteIngredient/{id}")]
    public ActionResult DeleteIngredient(int id){ 
      var ingredient = _context.Ingredients.SingleOrDefault(x => x.Id == id);
      if (ingredient == null) {
        return NotFound();
      }
      var tags = _context.Tags.Where(t => t.TableId == tagsTableId).Where(t => t.ItemId == id);
      _context.Tags.RemoveRange(tags);
      _context.SaveChanges();

      var dishIngredients = _context.DishIngredients.Where(i=>i.IngredientId == ingredient.Id);
      _context.DishIngredients.RemoveRange(dishIngredients);
      _context.SaveChanges();

      var portions = _context.IngredientPortions.Where(i=>i.IngredientId == ingredient.Id);
      _context.IngredientPortions.RemoveRange(portions);
      _context.SaveChanges();

      _context.Ingredients.Remove(ingredient);
      _context.SaveChanges();
      return Ok();
    }

    private async Task<bool> ingredientExist(string ingredientName){
      return await _context.Ingredients.AnyAsync(x => x.Name.ToLower() == ingredientName.ToLower());
    }




  }
}