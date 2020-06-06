using DesafioDesktop.Data;
using DesafioDesktop.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesafioDesktop.Services
{
    public class ProdutoService
    {
        readonly DesafioHiperContext context;

        public ProdutoService()
        {
            context = new DesafioHiperContext();
        }

        public Task<List<Produto>> ObterTodosProdutos()
        {
            return context.Produtos.ToListAsync();

        }

        public async Task SincronizacaoProdutos()
        {
            List<Produto> produtos = await context.Produtos.Where(x => x.Alterado == true || x.Excluir == true || x.Id < 0).ToListAsync();

            string ProdutosJson = JsonConvert.SerializeObject(produtos);

            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://localhost:44372/api/Sincronizacao/Produto")
            {
                Content = new StringContent(ProdutosJson, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = httpClient.SendAsync(request).Result;

            var jsonString = await response.Content.ReadAsStringAsync();
            List<Produto> produtosApi = JsonConvert.DeserializeObject<List<Produto>>(jsonString);

            await AtualizarTabela(produtosApi);
        }

        public Task<int> SalvarProdutos(List<Produto> produtos)
        {
            context.Produtos.AddRangeAsync(produtos);
            return context.SaveChangesAsync();
        }

        public Task<int> SalvarProduto(Produto produto)
        {
            context.Produtos.AddAsync(produto);
            return context.SaveChangesAsync();
        }

        public async Task<long> ObterId()
        {
            long id = await context.Produtos.MinAsync(x => x.Id);

            if (id > 0)
            {
                id = -1;
                return id;
            }

            return id - 1;
        }

        public Task<int> AtualizarTabela(List<Produto> produtos)
        {
            context.Produtos.RemoveRange(context.Produtos);
            context.Produtos.AddRangeAsync(produtos);
            return context.SaveChangesAsync();
        }

        public async Task<int> AtualizarProdutos(Produto produto)
        {

            Produto prodEdit = await ObterProdutoPorId(produto.Id);

            prodEdit.Nome = produto.Nome;
            prodEdit.Preco = produto.Preco;
            prodEdit.Estoque = produto.Estoque;
            prodEdit.Alterado = produto.Alterado;
            prodEdit.Excluir = produto.Excluir;

            return await context.SaveChangesAsync();
        }

        public Task<Produto> ObterProdutoPorId(long idProduto)
        {
            return context.Produtos.FirstOrDefaultAsync(x => x.Id == idProduto);
        }

        public async Task<List<Produto>> ObterProdutosApi()
        {
            List<Produto> produtos = new List<Produto>();

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44372/api/Produto");
            if (response.IsSuccessStatusCode)
            {
                string produtoJson = await response.Content.ReadAsStringAsync();

                produtos = JsonConvert.DeserializeObject<List<Produto>>(produtoJson);

            }

            return produtos;
        }
    }
}
