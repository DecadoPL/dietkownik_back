namespace API.Entities
{
  public class DietDish
  {
    public int Id { get; set; }
    public int DietDayId { get; set; }
    public string Quantity { get; set; }
    public string DishTime { get; set; }
    public int DishId { get; set; }
  }
}