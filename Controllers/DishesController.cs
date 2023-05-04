using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class DishesController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly int tagsTableId = 2;

    public DishesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishListItemDTO>>> GetDishesList(){
      var dishesMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Dish, DishListItemDTO>()));
      IEnumerable<Dish> dishes = _context.Dishes.ToList();
      IEnumerable<DishListItemDTO> dishesList = dishesMapper.Map<List<DishListItemDTO>>(dishes);
      return Ok(dishesList);
    }

    [HttpGet("searchDishes/{name}")]
    public ActionResult<DishListItemDTO> GetDishListItems(string name){      
      var dishQuery =
        from i in _context.Dishes
        where i.Name.ToLower().Contains(name.ToLower())
        select new DishListItemDTO
        {
          Id = i.Id,
          Name = i.Name,
        };

      return Ok(dishQuery.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<DishReadDTO> GetDish(int id){      
      var dishDTO = _context.Dishes
        .Where(d => d.Id == id)
        .Select(d => new DishReadDTO
        {
          Id = d.Id,
          Name = d.Name,
          Image = d.Image,
          Portions = d.Portions,
          Description = d.Description,
          Recipe = d.Recipe,
          CookingTime = d.CookingTime,
          Ingredients = _context.DishIngredients
            .Where(di => di.DishId == d.Id)
            .Select(di => new DishIngredientReadDTO
            {
              Id = di.Id,
              Ingredient = _context.Ingredients
                .Where(ingr => ingr.Id == di.IngredientId)
                .Select(ingr => new IngredientDTO{
                  Id = ingr.Id,
                  Brand = ingr.Brand,
                  Description = ingr.Description,
                  EAN = ingr.EAN,
                  Image = ingr.Image,
                  Name = ingr.Name,
                  Macro = new MacroDTO {
                    Carbohydrates = ingr.Carbohydrates,
                    Cholesterol = ingr.Cholesterol,
                    Fat = ingr.Fat,
                    Fibers = ingr.Fibers,
                    Kcal = ingr.Kcal,
                    Proteins = ingr.Proteins
                  },
                  Micro = new MicroDTO{
                    Calcium = ingr.Calcium,
                    Iron = ingr.Iron,
                    Magnesium = ingr.Magnesium,
                    Potassium = ingr.Potassium,
                    Sodium = ingr.Sodium,
                    VitaminA = ingr.VitaminA,
                    VitaminB12 = ingr.VitaminB12,
                    VitaminB6 = ingr.VitaminB6,
                    VitaminC = ingr.VitaminC,
                    VitaminD = ingr.VitaminD
                  },

                  Tags = _context.Tags
                    .Where(t => t.TableId == tagsTableId)
                    .Where(t => t.ItemId == ingr.Id)
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
                    .Where(p => p.IngredientId == ingr.Id)
                    .Select(p=> new IngredientPortionDTO{
                      Id = p.PortionNameId,
                      Name = _context.PortionNames.Where(x=>x.Id==p.PortionNameId).Select(x=>x.Name).SingleOrDefault(),
                      Quantity = p.Quantity
                    }).ToList()

                }).FirstOrDefault(),
                Portion = _context.DishIngredients
                  .Where(p => p.IngredientId == di.IngredientId)
                  .Where(x => x.DishId == di.DishId)
                  .Select(p => new IngredientPortionDTO{
                      Id = p.PortionNameId,
                      Name = _context.PortionNames.Where(x=>x.Id==p.PortionNameId).Select(x=>x.Name).SingleOrDefault(),
                      Quantity = p.Quantity
                    }).FirstOrDefault(),
                Quantity = _context.DishIngredients
                  .Where(x => x.IngredientId == di.IngredientId)
                  .Where(x => x.DishId == di.DishId)
                  .Select(x => x.Quantity).FirstOrDefault()
            }).ToList(),
          Macro = new MacroDTO{
            Proteins = d.Proteins,
            Carbohydrates = d.Carbohydrates,
            Cholesterol = d.Cholesterol,
            Fat = d.Fat,
            Fibers = d.Fibers,
            Kcal = d.Kcal},
          Micro = new MicroDTO{
            Calcium = d.Calcium,
            Iron = d.Iron,
            Magnesium = d.Magnesium,
            Potassium = d.Potassium,
            Sodium = d.Sodium,
            VitaminA = d.VitaminA,
            VitaminB12 = d.VitaminB12,
            VitaminB6 = d.VitaminB6,
            VitaminC = d.VitaminC,
            VitaminD = d.VitaminD},
          Tags = _context.Tags
            .Where(t => t.TableId == tagsTableId)
            .Where(t => t.ItemId == d.Id)
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
          
        })
        .FirstOrDefault();

      return Ok(dishDTO);
    }

    [HttpPost("addDish")]
    public async Task<ActionResult<DishSaveDTO>> AddDish(DishSaveDTO dishSaveDTO)
    {
      var dishIngrMapper = new MapperConfiguration(cfg => cfg.CreateMap<DishIngredientSaveDTO, DishIngredient>());
      var dishIngrMapperResult = dishIngrMapper.CreateMapper().Map<List<DishIngredient>>(dishSaveDTO.Ingredients);

      if (dishIngrMapperResult == null) {
        return BadRequest("Error mapping DTO to entity");
      }

      var dish = new Dish{
        Id = 0,
        Calcium = dishSaveDTO.Micro.Calcium,
        Carbohydrates = dishSaveDTO.Macro.Carbohydrates,
        Cholesterol = dishSaveDTO.Macro.Cholesterol,
        CookingTime = dishSaveDTO.CookingTime,
        Description = dishSaveDTO.Description,
        Fat = dishSaveDTO.Macro.Fat,
        Fibers = dishSaveDTO.Macro.Fibers,
        Image = dishSaveDTO.Image,
        Iron = dishSaveDTO.Micro.Iron,
        Kcal = dishSaveDTO.Macro.Kcal,
        Magnesium = dishSaveDTO.Micro.Magnesium,
        Name = dishSaveDTO.Name,
        Portions = dishSaveDTO.Portions,
        Potassium = dishSaveDTO.Micro.Potassium,
        Proteins = dishSaveDTO.Macro.Proteins,
        Recipe = dishSaveDTO.Recipe,
        Sodium = dishSaveDTO.Micro.Sodium,
        VitaminA = dishSaveDTO.Micro.VitaminA,
        VitaminB12 = dishSaveDTO.Micro.VitaminB12,
        VitaminB6 = dishSaveDTO.Micro.VitaminB6,
        VitaminC = dishSaveDTO.Micro.VitaminC,
        VitaminD = dishSaveDTO.Micro.VitaminD
      };

      var response = _context.Dishes.Add(dish);
      await _context.SaveChangesAsync();

      foreach (var di in dishIngrMapperResult) {
        di.DishId = response.Entity.Id;//_context.Dishes.Where(x=>x.)
        _context.DishIngredients.Add(di);
      }

      foreach(TagReadDTO tag in dishSaveDTO.Tags){

        var t = new Tag{
          Id = 0,
          ItemId = response.Entity.Id, 
          TableId = tagsTableId, 
          NameId = _context.TagNames
            .Where(tn => tn.Name == tag.Name)
            .Select(tn => tn.Id)
            .FirstOrDefault()
        };
        _context.Tags.Add(t);
      }

      await _context.SaveChangesAsync();
      return Ok(dishSaveDTO);
    }

    private async Task<bool> dishExist(string dishName){
      return await _context.Dishes.AnyAsync(x => x.Name.ToLower() == dishName.ToLower());
    }

    [HttpPost("updateDish")]
    public DishSaveDTO UpdateDish(DishSaveDTO dishSaveDTO){
      var dish = _context.Dishes.SingleOrDefault(x => x.Id == dishSaveDTO.Id);
      if(dish != null)
      {
        dish.Id = dishSaveDTO.Id;
        dish.Calcium = dishSaveDTO.Micro.Calcium;
        dish.Carbohydrates = dishSaveDTO.Macro.Carbohydrates;
        dish.Cholesterol = dishSaveDTO.Macro.Cholesterol;
        dish.CookingTime = dishSaveDTO.CookingTime;
        dish.Description = dishSaveDTO.Description;
        dish.Fat = dishSaveDTO.Macro.Fat;
        dish.Fibers = dishSaveDTO.Macro.Fibers;
        dish.Image = dishSaveDTO.Image;
        dish.Iron = dishSaveDTO.Micro.Iron;
        dish.Kcal = dishSaveDTO.Macro.Kcal;
        dish.Magnesium = dishSaveDTO.Micro.Magnesium;
        dish.Name = dishSaveDTO.Name;
        dish.Portions = dishSaveDTO.Portions;
        dish.Potassium = dishSaveDTO.Micro.Potassium;
        dish.Proteins = dishSaveDTO.Macro.Proteins;
        dish.Recipe = dishSaveDTO.Recipe;
        dish.Sodium = dishSaveDTO.Micro.Sodium;
        dish.VitaminA = dishSaveDTO.Micro.VitaminA;
        dish.VitaminB12 = dishSaveDTO.Micro.VitaminB12;
        dish.VitaminB6 = dishSaveDTO.Micro.VitaminB6;
        dish.VitaminC = dishSaveDTO.Micro.VitaminC;
        dish.VitaminD = dishSaveDTO.Micro.VitaminD;

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DishIngredientSaveDTO, DishIngredient>());
        
        var contextDishIngredients = _context.DishIngredients
            .Where(x => x.DishId == dishSaveDTO.Id)
            .ToList();

        if (dishSaveDTO.Ingredients.Count() < contextDishIngredients.Count())
        {
            var dishIngredientsToDelete = contextDishIngredients.Skip(dishSaveDTO.Ingredients.Count()).ToList();
            _context.DishIngredients.RemoveRange(dishIngredientsToDelete);
        }

        for (int i = 0; i < dishSaveDTO.Ingredients.Count(); i++)
        {
            var ingr = dishSaveDTO.Ingredients[i];



            if (i < contextDishIngredients.Count()){
              contextDishIngredients[i].IngredientId = ingr.IngredientId;
              contextDishIngredients[i].PortionNameId = ingr.PortionNameId;
              contextDishIngredients[i].Quantity = ingr.Quantity;
              _context.DishIngredients.Update(contextDishIngredients[i]);
            }else{
              var ingrToAdd = new DishIngredient{
                DishId = ingr.DishId,
                IngredientId = ingr.IngredientId,
                PortionNameId = ingr.PortionNameId,
                Quantity = ingr.Quantity,
              };

              _context.DishIngredients.Add(ingrToAdd);
              //var dishMapperResult = mapper.CreateMapper().Map<DishIngredient>(ingr);
              //  _context.DishIngredients.Add(dishMapperResult);
            }
        }

        var contextIngrTags = _context.Tags
            .Where(x => x.ItemId == dishSaveDTO.Id)
            .ToList();

        // Delete excess tags
        if (dishSaveDTO.Tags.Count() < contextIngrTags.Count())
        {
            var tagsToDelete = contextIngrTags.Skip(dishSaveDTO.Tags.Count()).ToList();
            _context.Tags.RemoveRange(tagsToDelete);
        }

        // Update or add tags
        for (int i = 0; i < dishSaveDTO.Tags.Count(); i++)
        {
            var ingrTag = dishSaveDTO.Tags[i];
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
                    ItemId = dishSaveDTO.Id,
                    TableId = tagsTableId,
                    NameId = ingrTagNameId.Value
                };
                _context.Tags.Add(tagToAdd);
            }
        }

        _context.Update(dish);
        _context.SaveChanges();
      }
      return dishSaveDTO;
    }

    [HttpDelete("deleteDish/{id}")]
    public ActionResult DeleteDish(int id){ 
      var dish = _context.Dishes.SingleOrDefault(x => x.Id == id);
      if (dish == null) {
        return NotFound();
      }
      var tags = _context.Tags.Where(t => t.TableId == tagsTableId).Where(t => t.ItemId == id);
      _context.Tags.RemoveRange(tags);
      _context.SaveChanges();

      var dishIngredients = _context.DishIngredients.Where(d => d.DishId == dish.Id);
      _context.DishIngredients.RemoveRange(dishIngredients);
      _context.SaveChanges();

      var dietDishes = _context.DietDishes.Where(d => d.DishId == dish.Id);
      _context.DietDishes.RemoveRange(dietDishes);
      _context.SaveChanges();

      _context.Dishes.Remove(dish);
      _context.SaveChanges();
      return Ok();
    }


  }
}