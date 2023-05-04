namespace API.Entities
{
  public class DishIngredient
  {
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public int DishId { get; set; }
    public int PortionNameId { get; set; }
    public string Quantity { get; set; }
  
  }
}