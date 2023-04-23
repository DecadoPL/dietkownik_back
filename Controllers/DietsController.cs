using System;
using System.Collections.Generic;
using System.Globalization;
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
  public class DietsController : BaseApiController
  {    
    private readonly DataContext _context;
    private readonly int tagsTableId = 3;
    public DietsController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DietListItemDTO>>> GetDietsList(){
      int UserId = 0;
      
      IEnumerable<Diet> diets = _context.Diets.Where(x => x.UserId == UserId);
      IEnumerable<DietListItemDTO> dietsList = diets.Select(diet => new DietListItemDTO
      {
        Id = diet.Id,
        Name = diet.Name
      });

      return Ok(dietsList);
    }


    [HttpGet("shoppinglist/{id}")]
    public ActionResult<IEnumerable<ShoppingListItemDTO>> GetShoppingList(int id){
      var culture = CultureInfo.CurrentCulture;
      var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
      var shoppingList = _context.DietDays
        .Where(dd => dd.DietId == id)
        .SelectMany(dd => _context.DietDishes.Where(d => d.DietDayId == dd.Id && d.Quantity.Contains("1/")))
        .SelectMany(dd => _context.DishIngredients.Where(di => di.DishId == dd.DishId))
        .AsEnumerable()
        .GroupBy(di => di.IngredientId)
        .Select(group => new ShoppingListItemDTO {
          Name = _context.Ingredients.FirstOrDefault(i => i.Id == group.Key) != null ? _context.Ingredients.FirstOrDefault(i => i.Id == group.Key).Name : null,
          Quantity = group.Sum(item => Convert.ToDouble(item.PortionQuantity.Replace(".", decimalSeparator, StringComparison.InvariantCulture))).ToString(),
          PortionType = _context.Ingredients
            .Where(i => i.Id == group.Key)
            .Join(
              _context.PortionTypes,
              ingredient => ingredient.PortionTypeId,
              portionType => portionType.Id,
              (ingredient, portionType) => portionType.Name
            )
            .FirstOrDefault() ?? "na"
        })
        .ToArray();
      return Ok(shoppingList);
    }

    [HttpGet("{id}")]
    public ActionResult<DietDTO> GetDiet(int id){
      var dietDTO = _context.Diets
        .Where(d => d.Id == id)
        .Select(d => new DietDTO
        {
          Id = d.Id,
          Name = d.Name,
          Description = d.Description,
          Requirements = _context.DietRequirements
            .Where(dr => dr.Id == d.DietRequirementsId)
            .Select(dr => new DietRequirementsDTO{/////
              Id = dr.Id,
              Name = dr.Name,
              Description = dr.Description,        
              Macro = new MacroDTO{
                Proteins = dr.Proteins,
                Fat = dr.Fat,
                Fibers = dr.Fibers,
                Carbohydrates = dr.Carbohydrates,
                Cholesterol = dr.Cholesterol,
                Kcal = dr.Kcal
              },
              Micro = new MicroDTO{
                VitaminC = dr.VitaminC,
                Calcium = dr.Calcium,
                Iron = dr.Iron,
                Magnesium = dr.Magnesium,  
                Potassium = dr.Potassium,
                Sodium = dr.Sodium,
                VitaminA = dr.VitaminA,
                VitaminB12 = dr.VitaminB12,
                VitaminB6 = dr.VitaminB6,
                VitaminD = dr.VitaminD,
              },

              ProhibitedIngredients = _context.ProhibitedIngredients
                .Where(pi => pi.DietRequirementsId == dr.Id)
                .Select(pi => new ProhibitedIngredientDTO
                {
                  Id = pi.Id,
                  Name = _context.Ingredients
                    .Where(i => i.Id == pi.IngredientId)
                    .Select(i => i.Name)
                    .FirstOrDefault()
                }).ToList(),
              RequiredIngredients = _context.RequiredIngredients
                .Where(ri => ri.DietRequirementsId == dr.Id)
                .Select(ri => new RequiredIngredientDTO
                {
                  Id = ri.Id,
                  Name = _context.Ingredients
                    .Where(i => i.Id == ri.IngredientId)
                    .Select(i => i.Name)
                    .FirstOrDefault()
                }).ToList(),

              ProhibitedTags = _context.ProhibitedTags
                .Where(pt => pt.DietRequirementsId == dr.Id)
                .Select(pt => new ProhibitedTagDTO
                {
                  Id = pt.Id,
                  Name = _context.TagNames
                    .Where(i => i.Id == pt.TagNameId)
                    .Select(i => i.Name)
                    .FirstOrDefault()
                }).ToList(),

              RequiredTags = _context.RequiredTags
                .Where(rt => rt.DietRequirementsId == dr.Id)
                .Select(rt => new RequiredTagDTO
                {
                  Id = rt.Id,
                  Name = _context.TagNames
                    .Where(i => i.Id == rt.TagNameId)
                    .Select(i => i.Name)
                    .FirstOrDefault()
                }).ToList(),
              Hours = _context.DietRequirementsHours
              .Where(x => x.DietRequirementsId == dr.Id)
              .Select(x => x.Hour).ToList()
            }).FirstOrDefault(),
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
            }).ToList(),

          Days = _context.DietDays
            .Where(dd => dd.DietId == d.Id)
            .Select(dd => new DietDayDTO{
              Date = dd.Date,
              Day = DateTime.ParseExact(dd.Date, "d.M.yyyy", null).ToString("dddd"),
              Dishes = _context.DietDishes
                .Where(ddi => ddi.DietDayId == dd.Id)
                .Select(ddi => new DietDishItemDTO
                {
                  Id = ddi.Id,
                  Quantity = ddi.Quantity,
                  Time = ddi.DishTime,
                  Name = _context.Dishes
                    .Where(d => d.Id == ddi.DishId)
                    .Select(d => d.Name).FirstOrDefault(),
                  DishId = ddi.DishId,
                  Macro = new MacroDTO {
                    Carbohydrates = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Carbohydrates).FirstOrDefault(),
                    Cholesterol = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Cholesterol).FirstOrDefault(),
                    Fat = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Fat).FirstOrDefault(),
                    Fibers = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Fibers).FirstOrDefault(),
                    Kcal = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Kcal).FirstOrDefault(),
                    Proteins = _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Proteins).FirstOrDefault()
                  },
                  Micro = new MicroDTO{
                    Calcium =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Calcium).FirstOrDefault(),
                    Iron =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Iron).FirstOrDefault(),
                    Magnesium =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Magnesium).FirstOrDefault(),
                    Potassium =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Potassium).FirstOrDefault(),
                    Sodium =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.Sodium).FirstOrDefault(),
                    VitaminA =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.VitaminA).FirstOrDefault(),
                    VitaminB12 =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.VitaminB12).FirstOrDefault(),
                    VitaminB6 =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.VitaminB6).FirstOrDefault(),
                    VitaminC =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.VitaminC).FirstOrDefault(),
                    VitaminD =  _context.Dishes.Where(d => d.Id == ddi.DishId).Select(d => d.VitaminD).FirstOrDefault(),
                  }
                }).ToList(),
            }).ToList(),
      }).FirstOrDefault();
      return Ok(dietDTO);
    }

    [HttpPost("addDiet")]
    public async Task<ActionResult<DietDTO>> addDiet(DietDTO dietDTO){

      var diet = new Diet{
        Id = 0,
        UserId = 0,
        Description = dietDTO.Description,
        DietRequirementsId = dietDTO.Requirements.Id,      
        Name = dietDTO.Name
      };

      var responseDiets = _context.Diets.Add(diet);
      await _context.SaveChangesAsync();

      
      foreach(DietDayDTO day in dietDTO.Days)
      {
        var da = new DietDay{
          Id = 0,
          Date = day.Date,
          DietId = responseDiets.Entity.Id
        };

        var responseDietDays = _context.DietDays.Add(da);
        await _context.SaveChangesAsync();

        foreach(DietDishItemDTO dish in day.Dishes){
          var di = new DietDish{
            Id = 0,
            DietDayId = responseDietDays.Entity.Id,
            DishTime = dish.Time,
            Quantity = dish.Quantity,
            DishId = dish.DishId
          };
          _context.DietDishes.Add(di);
        }
      }

      if(dietDTO.Tags != null){
        foreach(TagReadDTO tag in dietDTO.Tags){
          var t = new Tag{
            Id = 0,
            ItemId = responseDiets.Entity.Id, 
            TableId = tagsTableId, 
            NameId = _context.TagNames
              .Where(tn => tn.Name == tag.Name)
              .Select(tn => tn.Id)
              .FirstOrDefault()
          };
          _context.Tags.Add(t);
        }
      }

      await _context.SaveChangesAsync();

      return dietDTO;
    }

    
    [HttpPost("updateDiet")]
    public DietDTO UpdateDiet(DietDTO dietDTO){
      var diet = _context.Diets.SingleOrDefault(x => x.Id == dietDTO.Id);
      if(diet != null)
      {
        diet.Id = dietDTO.Id;
        diet.Name = dietDTO.Name;
        diet.Description = dietDTO.Description;
        diet.DietRequirementsId = dietDTO.Requirements.Id;
        diet.UserId = 0;

        // Update tags
        var contextDietTags = _context.Tags
            .Where(x => x.ItemId == dietDTO.Id)
            .ToList();

        // Delete excess tags
        if (dietDTO.Tags.Count() < contextDietTags.Count())
        {
            var tagsToDelete = contextDietTags.Skip(dietDTO.Tags.Count()).ToList();
            _context.Tags.RemoveRange(tagsToDelete);
        }

        // Update or add tags
        for (int i = 0; i < dietDTO.Tags.Count(); i++)
        {
            var dietTag = dietDTO.Tags[i];
            var dietTagNameId = _context.TagNames
                .SingleOrDefault(x => x.Name == dietTag.Name)?.Id;

            if (dietTagNameId == null) // Tag name doesn't exist in database
            {
                var tagNameToAdd = new TagName { Name = dietTag.Name };
                _context.TagNames.Add(tagNameToAdd);
                _context.SaveChanges();
                dietTagNameId = tagNameToAdd.Id;
            }

            if (i < contextDietTags.Count()) // Tag exists in database
            {
                if (contextDietTags[i].NameId != dietTagNameId) // Tag has changed
                {
                    contextDietTags[i].NameId = dietTagNameId.Value;
                    _context.Tags.Update(contextDietTags[i]);
                }
            }
            else // Tag doesn't exist in database
            {
                var tagToAdd = new Tag
                {
                    ItemId = dietDTO.Id,
                    TableId = tagsTableId,
                    NameId = dietTagNameId.Value
                };
                _context.Tags.Add(tagToAdd);
            }
        }

      var days = _context.DietDays
        .Where(x=> x.DietId == diet.Id)
        .ToList();


      for(int i =0; i< dietDTO.Days.Count(); i++)
      {
        var day = dietDTO.Days[i];
        var dayId = _context.DietDays
          .Where(x => x.DietId == diet.Id)
          .Where(x => x.Date == days[i].Date)
          .Select(x => x.Id).FirstOrDefault();

        var dietDishes = _context.DietDishes
        .Where(x=> x.DietDayId == dayId)
        .ToList();

        // Delete excess dishes

        _context.DietDishes.RemoveRange(dietDishes);

        for(int j =0; j< day.Dishes.Count(); j++)
        {
          var dishToAdd = new DietDish
          {      
            DietDayId =  days[i].Id,
            DishId = day.Dishes[j].DishId,
            DishTime = day.Dishes[j].Time,
            Quantity = day.Dishes[j].Quantity   
          };
          _context.DietDishes.Add(dishToAdd);         
        }

        if (i < days.Count()) // exists in database
        {
          if (days[i].Date != day.Date) //  has changed
          {
            days[i].Date = day.Date;
            _context.DietDays.Update(days[i]);
          }
        }

      }

        _context.Update(diet);
        _context.SaveChanges();
      }

      
      return dietDTO;        
    }

    [HttpDelete("deleteDiet/{id}")]
    public ActionResult DeleteDiet(int id){ 
      var diet = _context.Diets.SingleOrDefault(x => x.Id == id);
      if (diet == null) {
        return NotFound();
      }
      var tags = _context.Tags.Where(t => t.TableId == tagsTableId).Where(t => t.ItemId == id);
      _context.Tags.RemoveRange(tags);
      _context.SaveChanges();

      var days = _context.DietDays.Where(d => d.DietId == diet.Id);
      foreach(DietDay day in days){
        var dishes = _context.DietDishes.Where(d => d.DietDayId == day.Id);
        foreach(DietDish dish in dishes){
          var dishIngredients = _context.DishIngredients.Where(d => d.DishId == dish.DishId);
          _context.DishIngredients.RemoveRange(dishIngredients);
          _context.SaveChanges();
        }
      _context.DietDishes.RemoveRange(dishes);
      _context.SaveChanges();
      }
      _context.DietDays.RemoveRange(days);
    _context.SaveChanges();


      _context.Diets.Remove(diet);
      _context.SaveChanges();
      return Ok();
    }
  


  }
}