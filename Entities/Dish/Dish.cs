namespace API.Entities
{
  public class Dish
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Image { get; set; }
    public string Portions { get; set; }
    public string Description { get; set; }
    public string Recipe{get;set;}
    public string CookingTime { get; set; }
    public string Kcal { get; set; }
    public string Proteins { get; set; }
    public string Carbohydrates { get; set; }
    public string Fat { get; set; }
    public string Fibers { get; set; }
    public string Cholesterol { get; set; }
    public string Potassium { get; set; }
    public string Sodium { get; set; }
    public string VitaminA { get; set; }
    public string VitaminC { get; set; }
    public string VitaminB6 { get; set; }
    public string Magnesium { get; set; }
    public string VitaminD { get; set; }
    public string VitaminB12 { get; set; }
    public string Calcium { get; set; }
    public string Iron { get; set; }
  }
}