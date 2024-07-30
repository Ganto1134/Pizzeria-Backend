namespace PizzeriaNino.Models
{
    public class PizzaCreateViewModel
    {
        public string Nome { get; set; }
        public string Foto { get; set; }
        public decimal Prezzo { get; set; }
        public int Tempo { get; set; }
        public List<int> IngredientiIds { get; set; } = new List<int>();
    }
}
