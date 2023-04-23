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
  public class ShoppingListController : BaseApiController
  {    
    private readonly DataContext _context;
    public ShoppingListController(DataContext context)
    {
      _context = context;
    }

    [HttpGet("{ids}")]
    public ActionResult<IEnumerable<ShoppingListItemDTO>> GetShoppingListForDiets(string ids){

      int[] localIds = new int[0];

      foreach(string number in ids.Split(',')){
        Array.Resize(ref localIds, localIds.Length + 1);
        localIds[localIds.Length - 1] = int.Parse(number);
      }
          
      var culture = CultureInfo.CurrentCulture;
      var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
      var shoppingList = _context.DietDays
        .Where(dd => localIds.Contains(dd.DietId))
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
            .FirstOrDefault() ?? "na",
          Checked = false
        })
        .ToArray();
      return Ok(shoppingList);
    }


    [HttpPost("saveShoppingList")]
    public async Task<ActionResult<IEnumerable<ShoppingListItemDTO>>> SaveShoppingList(ShoppingListItemDTO[] shoppingListItemDTO)
    {  

      var items = _context.ShoppingListItems.Where(x => x.UserId == 0).ToArray();
      if(items != null) _context.ShoppingListItems.RemoveRange(items);

      foreach(ShoppingListItemDTO item in shoppingListItemDTO){
        var tempItem = new ShoppingListItem{
          UserId = 0,
          Name = item.Name,
          Quantity = item.Quantity,
          PortionType = item.PortionType,
          Checked = item.Checked
        };

        _context.ShoppingListItems.Add(tempItem);

      }
   
      await _context.SaveChangesAsync();
      return Ok(shoppingListItemDTO);
    }




    [HttpGet("user/{userId}")]
    public ActionResult<IEnumerable<ShoppingListItemDTO>> GetShoppingList(int userId){
          
      var culture = CultureInfo.CurrentCulture;
      var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
      var shoppingList = _context.ShoppingListItems
        .Where(x => x.UserId == userId)
        .ToArray();

      return Ok(shoppingList);

    }









  }
}