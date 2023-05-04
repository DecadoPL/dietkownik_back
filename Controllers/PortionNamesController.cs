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
  public class PortionNamesController : BaseApiController
  {
    private readonly DataContext _context;

    public PortionNamesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortionNameDTO>>> GetPortionNames(){
      var ptMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<PortionName, PortionNameDTO>()));
      IEnumerable<PortionName> portionTypes = _context.PortionNames.ToList();
      IEnumerable<PortionNameDTO> portionTypesDTO = ptMapper.Map<List<PortionNameDTO>>(portionTypes);
      return Ok(portionTypesDTO);
    }

    [HttpPost("addPortionName")]
    public async Task<ActionResult<PortionNameDTO>> AddPortionName(PortionNameDTO portionNameDTO)
    {
        var newPortionName = new PortionName { Name = portionNameDTO.Name };
        _context.PortionNames.Add(newPortionName);
        await _context.SaveChangesAsync();

        var ptDTO = new PortionNameDTO { Id = newPortionName.Id, Name = newPortionName.Name };
        return Ok(ptDTO);
    }

    [HttpPost("updatePortionName")]
    public async Task<ActionResult<PortionNameDTO>> UpdatePortionName(PortionNameDTO portionNameDTO){
      
    var portionType = _context.PortionNames.Where(x=>x.Id == portionNameDTO.Id).FirstOrDefault();
    if(portionType != null)
    {
      portionType.Name = portionNameDTO.Name;
    }
    
    await _context.SaveChangesAsync();

    var newPortionNameDTO = new PortionNameDTO { Id = portionType.Id, Name = portionType.Name };
    return Ok(newPortionNameDTO);
    }

    [HttpPost("deletePortionName")]   
    public async Task<ActionResult<PortionNameDTO>> DeletePortionName(PortionNameDTO portionNameDTO){   
      _context.Remove(_context.PortionNames.Single(x => x.Id == portionNameDTO.Id));
      await _context.SaveChangesAsync();
      return portionNameDTO;
    }


  }
}