namespace DesafioDesktop.Models
{
    public class Produto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public bool? Alterado { get; set; }
        public bool? Excluir { get; set; }
    }
}
