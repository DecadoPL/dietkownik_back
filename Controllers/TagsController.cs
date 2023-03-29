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
  public class TagsController : BaseApiController
  {
    private readonly DataContext _context;

    public TagsController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagReadDTO>>> GetTags(){
      var Mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Tag, TagReadDTO>()));
      IEnumerable<Tag> tags = _context.Tags.ToList();
      IEnumerable<TagReadDTO> tagsDTO = Mapper.Map<List<TagReadDTO>>(tags);
      return Ok(tagsDTO);
    }

    [HttpGet("names")]
    public async Task<ActionResult<IEnumerable<TagReadDTO>>> GetTagsNames(){
      var Mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<TagName, TagReadDTO>()));
      IEnumerable<TagName> tagNames = _context.TagNames.ToList();
      IEnumerable<TagReadDTO> tagsDTO = Mapper.Map<List<TagReadDTO>>(tagNames);
      return Ok(tagsDTO);
    }

    [HttpGet("{ItemId}/{TableId}")]
    public async Task<ActionResult<IEnumerable<TagReadDTO>>> GetItemTags(int ItemId, int TableId){
      var query = _context.Tags
        .Where(t => t.TableId == TableId)
        .Where(t => t.ItemId == ItemId)
        .Select(t => new TagReadDTO{
          Id = t.Id,
          Name = _context.TagNames
            .Where(tn => tn.Id == t.NameId)
            .Select(tn => tn.Name)
            .FirstOrDefault()
        });
        
      return Ok(query.ToList());
    }

    [HttpPost("addTagName")]
    public async Task<ActionResult<String>> AddTagName(TagReadDTO tagReadDTO)
    {  
      _context.TagNames.Add(new TagName{Id=0,Name=tagReadDTO.Name});
      await _context.SaveChangesAsync();
      return Ok(tagReadDTO);
    }

    [HttpPost("updateTagName")]
    public async Task<ActionResult<TagReadDTO>> UpdateTagName(TagReadDTO tagReadDTO){
      
    var tagRead = _context.TagNames.Where(x=>x.Id == tagReadDTO.Id).FirstOrDefault();
    if(tagRead != null)
    {
      tagRead.Name = tagReadDTO.Name;
    }
    
    await _context.SaveChangesAsync();

    var newtagReadDTO = new TagReadDTO { Id = tagRead.Id, Name = tagRead.Name };
    return Ok(newtagReadDTO);
    }

    [HttpPost("deleteTagName")]   
    public async Task<ActionResult<TagReadDTO>> DeleteTagName(TagReadDTO tagReadDTO){   
      _context.Remove(_context.TagNames.Single(x => x.Id == tagReadDTO.Id));
      await _context.SaveChangesAsync();
      return tagReadDTO;
    }































    [HttpPost("addTags")]
    public async Task<ActionResult<IEnumerable<TagSaveDTO>>> AddTags(TagSaveDTO[] tags)
    {

      foreach(TagSaveDTO tag in tags){
        if(await tagExist(tag)){
        }else{
          _context.Tags.Add(new Tag{Id=0,ItemId = tag.ItemId,
           NameId = tag.NameId, TableId = tag.TableId});
        }
      }
      return Ok(tags);
    }

    private async Task<bool> tagNameExist(string tagName){
      return await _context.TagNames.AnyAsync(t => t.Name.ToLower() == tagName.ToLower());
    }

    private async Task<bool> tagExist(TagSaveDTO tag){
      return await _context.Tags.AnyAsync(
          t => (t.TableId == tag.TableId)
          && (t.ItemId == tag.ItemId)
          && (t.NameId == tag.NameId) 
        );
    }

  }
}