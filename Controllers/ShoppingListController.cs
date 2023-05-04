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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingListListItemDTO>>> GetShoppingLists(){
      int UserId = 0;
      
      IEnumerable<ShoppingList> shoppingLists = _context.ShoppingLists.Where(x => x.UserId == UserId);
      IEnumerable<ShoppingListListItemDTO> list = shoppingLists.Select(item => new ShoppingListListItemDTO
      {
        Id = item.Id,
        Name = item.Name
      });

      return Ok(list);
    }


    [HttpGet("diets/{ids}")]
    public ActionResult<IEnumerable<ShoppingListItemDTO>> GetShoppingListForDiets(string ids){

      int[] localIds = ids.Split(',').Select(int.Parse).ToArray();
      
      var culture = CultureInfo.CurrentCulture;
      var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
      /*var shoppingList = _context.DietDays
        .Where(dd => localIds.Contains(dd.DietId))
        .SelectMany(dd => _context.DietDishes.Where(d => d.DietDayId == dd.Id && d.Quantity.Contains("1/")))
        .SelectMany(dd => _context.DishIngredients.Where(di => di.DishId == dd.DishId))
        .Select(di => new ShoppingListItemDTO {
          Name = _context.Ingredients.FirstOrDefault(i => i.Id == di.IngredientId).Name,
          Quantity = di.Quantity.Replace(".", decimalSeparator, StringComparison.InvariantCulture),
          PortionName = _context.PortionNames.Where(pn => pn.Id == di.PortionNameId).Select(pn => pn.Name).FirstOrDefault() ?? "na",
          Checked = false
        })
        .ToArray();*/

var shoppingList = _context.DietDays
    .Where(dd => localIds.Contains(dd.DietId))
    .Join(_context.DietDishes, dd => dd.Id, d => d.DietDayId, (dd, d) => new { dd, d })
    .Join(_context.DishIngredients, dd => dd.d.DishId, di => di.DishId, (dd, di) => new { dd.dd, dd.d, di })
    .Select(item => new ShoppingListItemDTO {
        Name = _context.Ingredients.FirstOrDefault(i => i.Id == item.di.IngredientId).Name,
        Quantity = DivideQuantity(item.di.Quantity, item.d.DishId, _context),
        PortionName = _context.PortionNames
            .Where(pn => pn.Id == item.di.PortionNameId)
            .Select(pn => pn.Name)
            .FirstOrDefault() ?? "na",
        Checked = false
    })
    .ToArray();

      var summarizedList = SummarizeShoppingList(shoppingList);

      return Ok(summarizedList);
    }




// helper function to divide the quantity by the denominator in the format "1/5", "1/3", etc.
private static  string DivideQuantity(string quantity, int dishId, DataContext context)
{
    var totalQuantity = context.Dishes.Where(d => d.Id == dishId).Select( d => d.Portions).FirstOrDefault();

    var result = decimal.Parse(quantity) / decimal.Parse(totalQuantity);
    return result.ToString().Replace(".", ",", StringComparison.InvariantCulture);
}


    public ShoppingListItemDTO[] SummarizeShoppingList(ShoppingListItemDTO[] shoppingList) {
      var summarizedList = new List<ShoppingListItemDTO>();
      var sortedList = shoppingList.OrderBy(s => s.Name).ThenBy(s => s.PortionName);

      var currentName = "";
      var currentPortionName = "";
      var currentQuantity = 0.0;

      foreach (var item in sortedList) {
        if (item.Name != currentName || item.PortionName != currentPortionName) {
          if (!string.IsNullOrEmpty(currentName)) {
            summarizedList.Add(new ShoppingListItemDTO {
              Name = currentName,
              Quantity = currentQuantity.ToString(),
              PortionName = currentPortionName,
              Checked = false
            });
          }

          currentName = item.Name;
          currentPortionName = item.PortionName;
          currentQuantity = Convert.ToDouble(item.Quantity);
        } else {
          currentQuantity += Convert.ToDouble(item.Quantity);
        }
      }

      if (!string.IsNullOrEmpty(currentName)) {
        summarizedList.Add(new ShoppingListItemDTO {
          Name = currentName,
          Quantity = currentQuantity.ToString(),
          PortionName = currentPortionName,
          Checked = false
        });
      }

      return summarizedList.ToArray();
    }

    [HttpPost("saveShoppingList")]
    public async Task<ActionResult<ShoppingListDTO>> SaveShoppingList(ShoppingListDTO shoppingListDTO)
    {  
      ShoppingList list;
      if(shoppingListDTO.Id != 0){
        list = _context.ShoppingLists.Where(x => x.Id == shoppingListDTO.Id).FirstOrDefault();
        list.Name = shoppingListDTO.Name;
        _context.ShoppingLists.Update(list);
        var items = _context.ShoppingListItems.Where(x => x.ShoppingListId == shoppingListDTO.Id).ToArray();
        if(items != null) _context.ShoppingListItems.RemoveRange(items);
      }else{
        var existingShoppingListId = _context.ShoppingLists.Where(x => x.Name == shoppingListDTO.Name).Select(x => x.Id).FirstOrDefault();
        if( existingShoppingListId != 0){
          list = _context.ShoppingLists.Where(x => x.Id == existingShoppingListId).FirstOrDefault();
          list.Name = shoppingListDTO.Name;
          _context.ShoppingLists.Update(list);
        }else{
          list = new ShoppingList{Name = shoppingListDTO.Name, UserId = 0};
          _context.ShoppingLists.Add(list);
        }

      }
      _context.SaveChanges();  

      _context.ShoppingListItems.RemoveRange(_context.ShoppingListItems.Where(x=>x.ShoppingListId == list.Id).ToList());

      foreach(ShoppingListItemDTO item in shoppingListDTO.Items){
        var tempItem = new ShoppingListItem{
          ShoppingListId = list.Id,
          Name = item.Name,
          Quantity = item.Quantity,
          PortionName = item.PortionName,
          Checked = item.Checked
        };

        _context.ShoppingListItems.Add(tempItem);

      }
   
      await _context.SaveChangesAsync();
      return Ok(shoppingListDTO);
    }




    [HttpGet("{Id}")]
    public ActionResult<IEnumerable<ShoppingListDTO>> GetShoppingList(int Id){
          
      var shoppingList = _context.ShoppingLists
        .Where(x => x.Id == Id)
        .Select(x => new ShoppingListDTO {
          Id = x.Id,
          Name = x.Name,
          Items = _context.ShoppingListItems
            .Where(i => i.ShoppingListId == x.Id)
            .Select(i => new ShoppingListItemDTO{
              Name = i.Name,
              PortionName = i.PortionName,
              Checked = i.Checked,
              Quantity = i.Quantity
            }).ToList() 
        }).FirstOrDefault();

      return Ok(shoppingList);

    }

    [HttpDelete("deleteShoppingListItem/{id}/{name}/{portionName}")]   
    public async Task<ActionResult<ShoppingListListItemDTO>> DeleteShoppingListItem(int id, string name, string portionName){   
      var itemToRemove = _context.ShoppingListItems
        .Where(x=>x.ShoppingListId == id)
        .Where(x=>x.Name == name)
        .Where(x=>x.PortionName == portionName)
        .FirstOrDefault();
      _context.ShoppingListItems.Remove(itemToRemove);

      await _context.SaveChangesAsync();
      return Ok();
    } 

    [HttpDelete("deleteShoppingList/{id}")]   
    public async Task<ActionResult> DeleteShoppingList(int id){   
      _context.ShoppingLists.Remove(_context.ShoppingLists.Single(x => x.Id == id));
      _context.ShoppingListItems.RemoveRange(_context.ShoppingListItems.Where(x=>x.ShoppingListId == id).ToList());

      await _context.SaveChangesAsync();
      return Ok();
    } 

  }
}