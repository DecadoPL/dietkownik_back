using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class UserDTO
  {
    public int AccountTypeId { get; set; }
    public string Username { get; set; }
    public string Surname { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public string Token {get; set;}
  
  }
    
}