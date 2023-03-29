namespace API.DTOs
{
  public class DietDishItemDTO
  {
    public int Id { get; set; }
    public string Quantity { get; set; }
    public string Time { get; set; }
    public string Name { get; set; }
    public MacroDTO Macro { get; set; }
    public MicroDTO Micro { get; set; }
    public int DishId { get; set; }
    public List<TagReadDTO> Tags { get; set; }

  }
}