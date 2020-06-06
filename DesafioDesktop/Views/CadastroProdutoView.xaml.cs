using DesafioDesktop.Models;
using DesafioDesktop.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesafioDesktop.Views
{
    /// <summary>
    /// Interaction logic for CadastroProdutoView.xaml
    /// </summary>
    public partial class CadastroProdutoView : Window
    {
        readonly ProdutoService produtoService = new ProdutoService();

        Produto produtoEdit;

        public CadastroProdutoView()
        {
            InitializeComponent();
        }

        public CadastroProdutoView(Produto produto)
        {
            InitializeComponent();
            produtoEdit = produto;
            PreencheCampos();
        }

        private void PreencheCampos()
        {
            txtEstoque.Text = produtoEdit.Estoque.ToString();
            txtNome.Text = produtoEdit.Nome;
            txtPreco.Text = produtoEdit.Preco.ToString();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    SalvarProduto();


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao cadastrar produto\n{ex.Message}");
            }
        }

        private async void SalvarProduto()
        {
            if (produtoEdit == null)
            {
                Produto produto = new Produto
                {
                    Id = await produtoService.ObterId(),
                    Nome = txtNome.Text,
                    Preco = Convert.ToDecimal(txtPreco.Text),
                    Estoque = Convert.ToInt32(txtEstoque.Text)
                };

                await produtoService.SalvarProduto(produto);
            }
            else
            {
                produtoEdit.Alterado = true;
                produtoEdit.Nome = txtNome.Text;
                produtoEdit.Preco = Convert.ToDecimal(txtPreco.Text);
                produtoEdit.Estoque = Convert.ToInt32(txtEstoque.Text);

                await produtoService.AtualizarProdutos(produtoEdit);
            }

            await (Owner as MainWindow).AtualizarGrid();
            MessageBox.Show($"Produto salvo");
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("Insira o nome");
                return false;
            }

            if (string.IsNullOrEmpty(txtPreco.Text))
            {
                MessageBox.Show("Insira o preço");
                return false;
            }

            if (string.IsNullOrEmpty(txtEstoque.Text))
            {
                MessageBox.Show("Insira o estoque");
                return false;
            }

            return true;
        }
    }
}
