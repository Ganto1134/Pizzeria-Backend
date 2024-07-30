namespace PizzeriaNino.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<PizzaIngrediente> PizzaIngredienti { get; set; }
    }
}
