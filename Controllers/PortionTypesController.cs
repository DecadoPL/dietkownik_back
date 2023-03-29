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
  public class PortionTypesController : BaseApiController
  {
    private readonly DataContext _context;

    public PortionTypesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortionTypeDTO>>> GetPortionTypes(){
      var ptMapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<PortionType, PortionTypeDTO>()));
      IEnumerable<PortionType> portionTypes = _context.PortionTypes.ToList();
      IEnumerable<PortionTypeDTO> portionTypesDTO = ptMapper.Map<List<PortionTypeDTO>>(portionTypes);
      return Ok(portionTypesDTO);
    }

    [HttpPost("addPortionType")]
    public async Task<ActionResult<PortionTypeDTO>> AddPortionType(PortionTypeDTO portionTypeDTO)
    {
        var newPortionType = new PortionType { Name = portionTypeDTO.Name };
        _context.PortionTypes.Add(newPortionType);
        await _context.SaveChangesAsync();

        var ptDTO = new PortionTypeDTO { Id = newPortionType.Id, Name = newPortionType.Name };
        return Ok(ptDTO);
    }

    [HttpPost("updatePortionType")]
    public async Task<ActionResult<PortionTypeDTO>> UpdatePortionType(PortionTypeDTO portionTypeDTO){
      
    var portionType = _context.PortionTypes.Where(x=>x.Id == portionTypeDTO.Id).FirstOrDefault();
    if(portionType != null)
    {
      portionType.Name = portionTypeDTO.Name;
    }
    
    await _context.SaveChangesAsync();

    var newPortionTypeDTO = new PortionTypeDTO { Id = portionType.Id, Name = portionType.Name };
    return Ok(newPortionTypeDTO);
    }

    [HttpPost("deletePortionType")]   
    public async Task<ActionResult<PortionTypeDTO>> DeletePortionType(PortionTypeDTO portionTypeDTO){   
      _context.Remove(_context.PortionTypes.Single(x => x.Id == portionTypeDTO.Id));
      await _context.SaveChangesAsync();
      return portionTypeDTO;
    }


  }
}