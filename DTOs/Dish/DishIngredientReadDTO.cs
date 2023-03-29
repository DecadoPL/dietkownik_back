namespace API.DTOs
{
  public class DishIngredientReadDTO
  {
    public int Id { get; set; }
    public IngredientDTO Ingredient { get; set; }
    public PortionTypeDTO PortionType { get; set; }
    public string PortionQuantity { get; set; }
  }
}