using Newtonsoft.Json;

namespace PizzeriaNino.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Foto { get; set; }
        public decimal Prezzo { get; set; }
        public int Tempo { get; set; }

        [JsonIgnore]
        public virtual ICollection<PizzaIngrediente> PizzaIngredienti { get; set; }
    }
}
