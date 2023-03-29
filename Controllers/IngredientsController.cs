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
          PortionType = _context.PortionTypes
            .Where(pt => pt.Id == i.PortionTypeId)
            .Select(pt => new PortionTypeDTO{
              Id = pt.Id,
              Name = pt.Name
            }).FirstOrDefault(),
          Name = i.Name,
          Brand = i.Brand,
          Description = i.Description,
          EAN = i.EAN,
          PortionQuantity = i.PortionQuantity,
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
            }).ToList()
        });
  
      return Ok(ingredientQuery.FirstOrDefault());
    }

    [HttpPost("addingredient")]
    public async Task<ActionResult<IngredientDTO>> AddIngredient(IngredientDTO ingredientDTO)
    {
      if(await ingredientExist(ingredientDTO.Name)) return BadRequest("Ingredient is already in database");    
      var ingrMapper = new MapperConfiguration(cfg => cfg.CreateMap<IngredientDTO, Ingredient>());
      var ingrMapperResult = ingrMapper.CreateMapper().Map<Ingredient>(ingredientDTO);

      if (ingrMapperResult == null) {
        return BadRequest("Error mapping DTO to entity");
      }

      _context.Ingredients.Add(ingrMapperResult);
      await _context.SaveChangesAsync();

      foreach(TagReadDTO tag in ingredientDTO.Tags){

        var t = new Tag{
          Id = 0,
          ItemId = ingrMapperResult.Id, 
          TableId = tagsTableId, 
          NameId = _context.TagNames
            .Where(tn => tn.Name == tag.Name)
            .Select(tn => tn.Id)
            .FirstOrDefault()
        };
        _context.Tags.Add(t);
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
        ingr.PortionTypeId = ingredientDTO.PortionType.Id;
        ingr.Name = ingredientDTO.Name;
        ingr.Brand = ingredientDTO.Brand;
        ingr.Description = ingredientDTO.Description;
        ingr.EAN = ingredientDTO.EAN;
        ingr.PortionQuantity = ingredientDTO.PortionQuantity;
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

      _context.Ingredients.Remove(ingredient);
      _context.SaveChanges();
      return Ok();
    }

    private async Task<bool> ingredientExist(string ingredientName){
      return await _context.Ingredients.AnyAsync(x => x.Name.ToLower() == ingredientName.ToLower());
    }




  }
}