using DesafioDesktop.Models;
using DesafioDesktop.Services;
using DesafioDesktop.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesafioDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ProdutoService produtoService = new ProdutoService();
        ObservableCollection<Produto> produtosLocal;

        public MainWindow()
        {
            InitializeComponent();
            Iniciar();
        }

        private async void Iniciar()
        {
            try
            {
                produtosLocal = new ObservableCollection<Produto>(await produtoService.ObterTodosProdutos());

                if (produtosLocal.Count == 0)
                {
                    List<Produto> produtosApi = await produtoService.ObterProdutosApi();
                    await produtoService.SalvarProdutos(produtosApi);
                    produtosLocal = new ObservableCollection<Produto>(await produtoService.ObterTodosProdutos());
                }

                dgProdutos.ItemsSource = produtosLocal.Where(x => x.Excluir != true);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro no carregamento incial da aplicação\n{ex.Message}");
            }
        }

        private void BtnNovo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CadastroProdutoView cadProd = new CadastroProdutoView();
                cadProd.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao abrir tela de cadastro produto\n{ex.Message}");
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgProdutos.SelectedItem != null)
                {
                    Produto produto = (Produto)dgProdutos.SelectedItem;
                    CadastroProdutoView cadProd = new CadastroProdutoView(produto);
                    cadProd.Show();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao abrir tela de cadastro produto\n{ex.Message}");
            }
        }

        private async void BtnExcluir_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgProdutos.SelectedItem != null)
                {
                    Produto produto = (Produto)dgProdutos.SelectedItem;
                    produto.Excluir = true;

                    await produtoService.AtualizarProdutos(produto);

                    await AtualizarGrid();

                    MessageBox.Show($"Produto excluido");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao excluir produto\n{ex.Message}");
            }
        }

        private async void BtnSinc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SincronizarProdutos();

                MessageBox.Show($"Sincronizado com sucesso");
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao sincronizar produtos\n{ex.Message}");
            }
        }

        private async Task SincronizarProdutos()
        {
            await produtoService.SincronizacaoProdutos();
        }

        private async Task AtualizarGrid()
        {
            produtosLocal = new ObservableCollection<Produto>(await produtoService.ObterTodosProdutos());
            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtosLocal.Where(x => x.Excluir != true);
        }
    }
}
