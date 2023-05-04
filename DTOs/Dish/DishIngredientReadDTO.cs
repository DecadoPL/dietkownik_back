namespace API.DTOs
{
  public class DishIngredientReadDTO
  {
    public int Id { get; set; }
    public IngredientDTO Ingredient { get; set; }
    public IngredientPortionDTO Portion { get; set; }
    public string Quantity { get; set; }

  }
}