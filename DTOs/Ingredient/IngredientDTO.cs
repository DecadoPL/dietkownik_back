namespace API.DTOs
{
  public class IngredientDTO
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Image { get; set; }
    public string Brand { get; set; }
    public string EAN { get; set; }
    public string Description { get; set; }
    public MacroDTO Macro { get; set; }
    public MicroDTO Micro { get; set; }
    public List<TagReadDTO> Tags { get; set; }
    public List<IngredientPortionDTO> Portions { get; set; }
  }
}