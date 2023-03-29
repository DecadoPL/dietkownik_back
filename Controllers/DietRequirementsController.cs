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
  public class DietRequirementsController : BaseApiController
  {
    private readonly DataContext _context;


    public DietRequirementsController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DietRequirementsListItemDTO>>> GetDietRequirementsList(){
      var drMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<DietRequirements, DietRequirementsListItemDTO>()));
      IEnumerable<DietRequirements> dietRequirements = _context.DietRequirements.ToList();
      IEnumerable<DietRequirementsListItemDTO> dietRequirementsDTO = drMapper.Map<List<DietRequirementsListItemDTO>>(dietRequirements);
      return Ok(dietRequirementsDTO);
    }

    [HttpGet("{id}")]
    public ActionResult<DietRequirementsDTO> GetDietRequirements(int id){      
      var drQuery = _context.DietRequirements
        .Where(i => i.Id == id)
        .Select( i => new DietRequirementsDTO{
          Id = i.Id,
          Name = i.Name,
          Description = i.Description,
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
          RequiredTags = _context.RequiredTags
            .Where(rt => rt.DietRequirementsId == i.Id)
            .Select(rt => new RequiredTagDTO{
              Id = _context.TagNames
                .Where(tn => tn.Id == rt.TagNameId)
                .Select(tn => tn.Id)
                .FirstOrDefault(),
              Name = _context.TagNames
                .Where(tn => tn.Id == rt.TagNameId)
                .Select(tn => tn.Name)
                .FirstOrDefault()
            }).ToList(),

          ProhibitedTags = _context.ProhibitedTags
            .Where(pt => pt.DietRequirementsId == i.Id)
            .Select(pt => new ProhibitedTagDTO{
              Id = _context.TagNames
                .Where(tn => tn.Id == pt.TagNameId)
                .Select(tn => tn.Id)
                .FirstOrDefault(),
              Name = _context.TagNames
                .Where(tn => tn.Id == pt.TagNameId)
                .Select(tn => tn.Name)
                .FirstOrDefault()           
            }).ToList(),
          RequiredIngredients = _context.RequiredIngredients
            .Where(t => t.DietRequirementsId == i.Id)
            .Select(t => new RequiredIngredientDTO{
              Id = _context.Ingredients
                .Where(n => n.Id == t.IngredientId)
                .Select(tn => tn.Id)
                .FirstOrDefault(),     
              Name = _context.Ingredients
                .Where(n => n.Id == t.IngredientId)
                .Select(tn => tn.Name)
                .FirstOrDefault()                
            }).ToList(),
          ProhibitedIngredients = _context.ProhibitedIngredients
            .Where(t => t.DietRequirementsId == i.Id)
            .Select(t => new ProhibitedIngredientDTO{
              Id = _context.Ingredients
                .Where(n => n.Id == t.IngredientId)
                .Select(tn => tn.Id)
                .FirstOrDefault(),     
              Name = _context.Ingredients
                .Where(n => n.Id == t.IngredientId)
                .Select(tn => tn.Name)
                .FirstOrDefault()    
            }).ToList(),
        });
  
      return Ok(drQuery.FirstOrDefault());
    }

    [HttpPost("updatedietrequirements")]
    public async Task<ActionResult<DietRequirementsDTO>> UpdateDietRequirements(DietRequirementsDTO dietRequirementsDTO)
    {
      var usrId = 0;

      var dietReq = _context.DietRequirements.SingleOrDefault(x => x.Id == dietRequirementsDTO.Id);
      if(dietReq != null){
        dietReq.Id = dietRequirementsDTO.Id;
        dietReq.Calcium = dietRequirementsDTO.Micro.Calcium;
        dietReq.Carbohydrates = dietRequirementsDTO.Macro.Carbohydrates;
        dietReq.Cholesterol = dietRequirementsDTO.Macro.Cholesterol;
        dietReq.Description = dietRequirementsDTO.Description;
        dietReq.Fat = dietRequirementsDTO.Macro.Fat;
        dietReq.Fibers = dietRequirementsDTO.Macro.Fibers;
        dietReq.Kcal = dietRequirementsDTO.Macro.Kcal;
        dietReq.UserId = usrId;
        dietReq.Iron = dietRequirementsDTO.Micro.Iron;
        dietReq.Magnesium = dietRequirementsDTO.Micro.Magnesium;
        dietReq.Name = dietRequirementsDTO.Name;
        dietReq.Potassium = dietRequirementsDTO.Micro.Potassium;
        dietReq.Proteins = dietRequirementsDTO.Macro.Proteins;
        dietReq.Sodium = dietRequirementsDTO.Micro.Sodium;
        dietReq.VitaminA = dietRequirementsDTO.Micro.VitaminA;
        dietReq.VitaminB12 = dietRequirementsDTO.Micro.VitaminB12;
        dietReq.VitaminB6 = dietRequirementsDTO.Micro.VitaminB6;
        dietReq.VitaminC = dietRequirementsDTO.Micro.VitaminC;
        dietReq.VitaminD = dietRequirementsDTO.Micro.VitaminD;
      }

      var prohibitedTags = _context.ProhibitedTags
        .Where(x=> x.DietRequirementsId == dietReq.Id)
        .ToList();


      if(dietRequirementsDTO.ProhibitedTags.Count() < prohibitedTags.Count())
      {
        var tagsToDelete = prohibitedTags.Skip(dietRequirementsDTO.ProhibitedTags.Count()).ToList();
        _context.ProhibitedTags.RemoveRange(tagsToDelete);
      } 


      for(int i =0; i< dietRequirementsDTO.ProhibitedTags.Count(); i++)
      {
        var prohibitedTag = dietRequirementsDTO.ProhibitedTags[i];
        var prohibitedTagNameId = _context.TagNames
          .SingleOrDefault(x => x.Name == prohibitedTag.Name)?.Id;

        if (i < prohibitedTags.Count()) // Tag exists in database
        {
            if (prohibitedTags[i].TagNameId != prohibitedTagNameId) // Tag has changed
            {
                prohibitedTags[i].TagNameId = prohibitedTagNameId.Value;
                _context.ProhibitedTags.Update(prohibitedTags[i]);
            }
        }
        else // Tag doesn't exist in database
        {
            var tagToAdd = new ProhibitedTag
            {
              
              DietRequirementsId = dietReq.Id,
              TagNameId = prohibitedTagNameId.Value
            };
            _context.ProhibitedTags.Add(tagToAdd);
        }
      }





      var requiredTags = _context.RequiredTags
        .Where(x=> x.DietRequirementsId == dietReq.Id)
        .ToList();


      if(dietRequirementsDTO.RequiredTags.Count() < requiredTags.Count())
      {
        var tagsToDelete = requiredTags.Skip(dietRequirementsDTO.RequiredTags.Count()).ToList();
        _context.RequiredTags.RemoveRange(tagsToDelete);
      } 


      for(int i =0; i< dietRequirementsDTO.RequiredTags.Count(); i++)
      {
        var requiredTag = dietRequirementsDTO.RequiredTags[i];
        var requiredTagNameId = _context.TagNames
          .SingleOrDefault(x => x.Name == requiredTag.Name)?.Id;

        if (i < requiredTags.Count()) // Tag exists in database
        {
            if (requiredTags[i].TagNameId != requiredTagNameId) // Tag has changed
            {
                requiredTags[i].TagNameId = requiredTagNameId.Value;
                _context.RequiredTags.Update(requiredTags[i]);
            }
        }
        else // Tag doesn't exist in database
        {
            var tagToAdd = new RequiredTag
            {
              
              DietRequirementsId = dietReq.Id,
              TagNameId = requiredTagNameId.Value
            };
            _context.RequiredTags.Add(tagToAdd);
        }
      }


      var requiredIngredients = _context.RequiredIngredients
        .Where(x=> x.DietRequirementsId == dietReq.Id)
        .ToList();


      if(dietRequirementsDTO.RequiredIngredients.Count() < requiredIngredients.Count())
      {
        var IngredientsToDelete = requiredIngredients.Skip(dietRequirementsDTO.RequiredIngredients.Count()).ToList();
        _context.RequiredIngredients.RemoveRange(IngredientsToDelete);
      } 


      for(int i =0; i< dietRequirementsDTO.RequiredIngredients.Count(); i++)
      {
        var requiredIngredient = dietRequirementsDTO.RequiredIngredients[i];
        var requiredIngredientId = _context.Ingredients
          .SingleOrDefault(x => x.Id == requiredIngredient.Id)?.Id;

        if (i < requiredIngredients.Count()) // Ingredient exists in database
        {
            if (requiredIngredients[i].IngredientId != requiredIngredientId) // Ingredient has changed
            {
                requiredIngredients[i].IngredientId = requiredIngredientId.Value;
                _context.RequiredIngredients.Update(requiredIngredients[i]);
            }
        }
        else // Ingredient doesn't exist in database
        {
            var IngredientToAdd = new RequiredIngredient
            {
              
              DietRequirementsId = dietReq.Id,
              IngredientId = requiredIngredientId.Value
            };
            _context.RequiredIngredients.Add(IngredientToAdd);
        }
      }

      var prohibitedIngredients = _context.ProhibitedIngredients
        .Where(x=> x.DietRequirementsId == dietReq.Id)
        .ToList();

      if(dietRequirementsDTO.ProhibitedIngredients.Count() < prohibitedIngredients.Count())
      {
        var IngredientsToDelete = prohibitedIngredients.Skip(dietRequirementsDTO.ProhibitedIngredients.Count()).ToList();
        _context.ProhibitedIngredients.RemoveRange(IngredientsToDelete);
      } 

      for(int i =0; i< dietRequirementsDTO.ProhibitedIngredients.Count(); i++)
      {
        var prohibitedIngredient = dietRequirementsDTO.ProhibitedIngredients[i];
        var prohibitedIngredientId = _context.Ingredients
          .SingleOrDefault(x => x.Id == prohibitedIngredient.Id)?.Id;

        if (i < prohibitedIngredients.Count()) // Ingredient exists in database
        {
            if (prohibitedIngredients[i].IngredientId != prohibitedIngredientId) // Ingredient has changed
            {
                prohibitedIngredients[i].IngredientId = prohibitedIngredientId.Value;
                _context.ProhibitedIngredients.Update(prohibitedIngredients[i]);
            }
        }
        else // Ingredient doesn't exist in database
        {
            var IngredientToAdd = new ProhibitedIngredient
            {          
              DietRequirementsId = dietReq.Id,
              IngredientId = prohibitedIngredientId.Value
            };
            _context.ProhibitedIngredients.Add(IngredientToAdd);
        }
      }




      _context.DietRequirements.Update(dietReq);
      await _context.SaveChangesAsync();


      return Ok(dietRequirementsDTO);
    }

    [HttpPost("adddietrequirements")]
    public async Task<ActionResult<DietRequirementsDTO>> AddDietRequirements(DietRequirementsDTO dietRequirementsDTO)
    {
      var usrId = 0;

      var dietReq = new DietRequirements{
        Id = dietRequirementsDTO.Id,
        Calcium = dietRequirementsDTO.Micro.Calcium,
        Carbohydrates = dietRequirementsDTO.Macro.Carbohydrates,
        Cholesterol = dietRequirementsDTO.Macro.Cholesterol,
        Description = dietRequirementsDTO.Description,
        Fat = dietRequirementsDTO.Macro.Fat,
        Fibers = dietRequirementsDTO.Macro.Fibers,
        Kcal = dietRequirementsDTO.Macro.Kcal,
        UserId = usrId,
        Iron = dietRequirementsDTO.Micro.Iron,
        Magnesium = dietRequirementsDTO.Micro.Magnesium,
        Name = dietRequirementsDTO.Name,
        Potassium = dietRequirementsDTO.Micro.Potassium,
        Proteins = dietRequirementsDTO.Macro.Proteins,
        Sodium = dietRequirementsDTO.Micro.Sodium,
        VitaminA = dietRequirementsDTO.Micro.VitaminA,
        VitaminB12 = dietRequirementsDTO.Micro.VitaminB12,
        VitaminB6 = dietRequirementsDTO.Micro.VitaminB6,
        VitaminC = dietRequirementsDTO.Micro.VitaminC,
        VitaminD = dietRequirementsDTO.Micro.VitaminD
      };


      _context.DietRequirements.Add(dietReq);
      await _context.SaveChangesAsync();

      var savedDietRequirements = _context.DietRequirements.SingleOrDefault(x => x.Name == dietRequirementsDTO.Name);

      if (dietRequirementsDTO.RequiredTags != null && dietRequirementsDTO.RequiredTags.Count > 0)
      {
        foreach(RequiredTagDTO rt in dietRequirementsDTO.RequiredTags){
          var t = new RequiredTag{
            Id = 0,
            DietRequirementsId = savedDietRequirements.Id,
            TagNameId = _context.TagNames.SingleOrDefault(y => y.Name == rt.Name).Id
          };
          _context.RequiredTags.Add(t);
        }      
      }

      if (dietRequirementsDTO.ProhibitedTags != null && dietRequirementsDTO.ProhibitedTags.Count > 0)
      {
        foreach(ProhibitedTagDTO pt in dietRequirementsDTO.ProhibitedTags){
          var t = new ProhibitedTag{
            Id = 0,
            DietRequirementsId = savedDietRequirements.Id,
            TagNameId = _context.TagNames.SingleOrDefault(y => y.Name == pt.Name).Id
          };
          _context.ProhibitedTags.Add(t);
        }   
      }

      if (dietRequirementsDTO.RequiredIngredients != null && dietRequirementsDTO.RequiredIngredients.Count > 0)
      {
        foreach(RequiredIngredientDTO ri in dietRequirementsDTO.RequiredIngredients){
          var i = new RequiredIngredient{
            Id = 0,
            DietRequirementsId = savedDietRequirements.Id,
            IngredientId = _context.Ingredients.SingleOrDefault(y => y.Name == ri.Name).Id
          };
          _context.RequiredIngredients.Add(i);
        }   
      }

      if (dietRequirementsDTO.ProhibitedIngredients != null && dietRequirementsDTO.ProhibitedIngredients.Count > 0)
      {
        foreach(ProhibitedIngredientDTO pi in dietRequirementsDTO.ProhibitedIngredients){
          var i = new ProhibitedIngredient{
            Id = 0,
            DietRequirementsId = savedDietRequirements.Id,
            IngredientId = _context.Ingredients.SingleOrDefault(y => y.Name == pi.Name).Id
          };
          _context.ProhibitedIngredients.Add(i);
        }   
      }

      await _context.SaveChangesAsync();

      return Ok(dietRequirementsDTO);
    }


    [HttpDelete("deleteDietRequirements/{id}")]
    public ActionResult DeleteDietRequirements(int id){ 
      var dr = _context.DietRequirements.SingleOrDefault(x => x.Id == id);
      if (dr == null) {
        return NotFound();
      }
      var pi = _context.ProhibitedIngredients.Where(x=> x.DietRequirementsId == dr.Id);
      _context.ProhibitedIngredients.RemoveRange(pi);
      _context.SaveChanges();

      var ri = _context.RequiredIngredients.Where(x=> x.DietRequirementsId == dr.Id);
      _context.RequiredIngredients.RemoveRange(ri);
      _context.SaveChanges();

      var rt = _context.RequiredTags.Where(x=> x.DietRequirementsId == dr.Id);
      _context.RequiredTags.RemoveRange(rt);
      _context.SaveChanges();

      var pt = _context.ProhibitedTags.Where(x=> x.DietRequirementsId == dr.Id);
      _context.ProhibitedTags.RemoveRange(pt);
      _context.SaveChanges();

      _context.DietRequirements.Remove(dr);
      _context.SaveChanges();
      return Ok();
    }


  }
}