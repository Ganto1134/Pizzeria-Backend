namespace PizzeriaNino.Models
{
    public class PizzaIngrediente
    {
        public int PizzaId { get; set; }
        public Pizza Pizza { get; set; }

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }
}
