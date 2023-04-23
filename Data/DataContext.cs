using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions options) : base(options)
    {
      Database.EnsureCreated();
    }
    
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<Diet> Diets { get; set; }
    public DbSet<DietDish> DietDishes { get; set; }
    public DbSet<DietRequirements> DietRequirements { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishIngredient> DishIngredients { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<PortionType> PortionTypes { get; set; }
    public DbSet<ProhibitedIngredient> ProhibitedIngredients { get; set; }
    public DbSet<RequiredIngredient> RequiredIngredients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagName> TagNames { get; set; }
    public DbSet<DietDay> DietDays { get; set; }
    public DbSet<RequiredTag> RequiredTags { get; set; }
    public DbSet<ProhibitedTag> ProhibitedTags { get; set; }
    public DbSet<DietRequirementsHour> DietRequirementsHours { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

  }
}