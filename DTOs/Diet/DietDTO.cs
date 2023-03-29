namespace API.DTOs
{
    public class DietDTO
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DietRequirementsDTO Requirements { get; set; }
    public List<DietDayDTO> Days { get; set; }
    public List<TagReadDTO> Tags { get; set; }
    }
}

