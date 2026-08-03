using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SistemaGestaoOficina.WPF
{
    /// <summary>
    /// Interaction logic for VeiculosWindow.xaml
    /// </summary>
    public partial class VeiculosWindow : Window
    {
        private NetworkService networkService;

        private ApiService apiService;

        private List<Cliente> Clientes;

        private List<Veiculo> Veiculos;

        private List<Marcacao> Marcacoes;

        private List<Reparacao> Reparacoes;

        public VeiculosWindow()
        {
            InitializeComponent();
            
            networkService = new NetworkService();

            apiService = new ApiService();

            Clientes = new List<Cliente>();

            Veiculos = new List<Veiculo>();

            Marcacoes = new List<Marcacao>();

            Reparacoes = new List<Reparacao>();

            CarregarClientes();
        }

        /// <summary>
        /// Carrega os clientes da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarClientes()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(
                    connection.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            var response = await apiService.Get<Cliente>("https://localhost:44390/", "api/clientes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Clientes = (List<Cliente>)response.Result;

            CarregarVeiculos();
        }

        /// <summary>
        /// Carrega os veículos da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarVeiculos()
        {
            var response = await apiService.Get<Veiculo>("https://localhost:44390/","api/veiculos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Veiculos = (List<Veiculo>)response.Result;

            CarregarMarcacoes();
        }

        /// <summary>
        /// Carrega as marcações da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarMarcacoes()
        {
            var response = await apiService.Get<Marcacao>( "https://localhost:44390/","api/marcacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Marcacoes = (List<Marcacao>)response.Result;

            CarregarReparacoes();
        }

        /// <summary>
        /// Carrega as reparações da API.
        /// </summary>
        private async Task CarregarReparacoes()
        {
            var response = await apiService.Get<Reparacao>("https://localhost:44390/", "api/reparacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Reparacoes = (List<Reparacao>)response.Result;

            MostrarVeiculos(Veiculos);
        }

        /// <summary>
        /// Mostra os veículos e atualiza o total.
        /// </summary>
        /// <param name="veiculos"></param>
        private void MostrarVeiculos(List<Veiculo> lista)
        {
            listBoxVeiculos.ItemsSource = null;

            listBoxVeiculos.ItemsSource = lista;

            lblTotalVeiculos.Text =
                "Total: " + lista.Count + " veículos";

            listBoxDetalhesVeiculo.ItemsSource = null;

            listBoxHistoricoReparacoes.ItemsSource = null;
        }

        /// <summary>
        /// Mostra os detalhes e o histórico do veículo selecionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxVeiculos_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            Veiculo veiculoSelecionado =listBoxVeiculos.SelectedItem as Veiculo;

            if (veiculoSelecionado == null)
            {
                return;
            }

            Cliente cliente = Clientes.FirstOrDefault(c => c.Id == veiculoSelecionado.IdCliente);

            List<string> detalhes = new List<string>();

            if (cliente != null)
            {
                detalhes.Add("Cliente: " + cliente.Nome + " " + cliente.Apelido);
            }

            detalhes.Add("Marca: " + veiculoSelecionado.Marca);

            detalhes.Add("Modelo: " + veiculoSelecionado.Modelo);

            detalhes.Add("Matrícula: " + veiculoSelecionado.Matricula);

            detalhes.Add("Ano: " + veiculoSelecionado.Ano);

            detalhes.Add("Combustível: " + veiculoSelecionado.Combustivel);

            detalhes.Add("Quilometragem: " + veiculoSelecionado.Quilometragem + " km");

            listBoxDetalhesVeiculo.ItemsSource = null;

            listBoxDetalhesVeiculo.ItemsSource = detalhes;

            CarregarHistoricoReparacoes(veiculoSelecionado);
        }

        /// <summary>
        /// Carrega o histórico de reparações do veículo.
        /// </summary>
        /// <param name="veiculo"></param>
        private void CarregarHistoricoReparacoes(Veiculo veiculo)
        {
            List<string> historico = new List<string>();

            List<Marcacao> marcacoesVeiculo = Marcacoes.Where(m => m.IdVeiculo == veiculo.Id).ToList();

            foreach (Marcacao marcacao in marcacoesVeiculo)
            {
                List<Reparacao> reparacoesMarcacao = Reparacoes.Where(r => r.IdMarcacao == marcacao.Id).ToList();

                foreach (Reparacao reparacao in reparacoesMarcacao)
                {
                    historico.Add("Matrícula: " + veiculo.Matricula);

                    string[] partesPecas = reparacao.Pecas.Split(',');

                    string nomePecas = string.Empty;

                    foreach (string peca in partesPecas)
                    {
                        string[] partes = peca.Split('|');

                        if (partes.Length > 0)
                        {
                            nomePecas += partes[0].Trim() + ", ";
                        }
                    }

                    historico.Add(
                        "Peças: " +
                        nomePecas.TrimEnd(',', ' '));

                    historico.Add(
                        "Data início: " + reparacao.DataInicio.ToString("dd/MM/yyyy"));

                    if (reparacao.DataFim.HasValue)
                    {
                        historico.Add("Data fim: " + reparacao.DataFim.Value.ToString("dd/MM/yyyy"));
                    }

                    historico.Add(string.Empty);
                }
            }

            listBoxHistoricoReparacoes.ItemsSource = null;

            listBoxHistoricoReparacoes.ItemsSource = historico;
        }

        private void btnPesquisarVeiculo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisarVeiculo.Text))
            {
                MostrarVeiculos(Veiculos);

                return;
            }

            string pesquisa = txtPesquisarVeiculo.Text.ToLower().Trim();

            List<Veiculo> filtrados = Veiculos.Where(v =>
                    v.Matricula.ToLower().Contains(pesquisa) ||
                    v.Modelo.ToLower().Contains(pesquisa) ||
                    v.Marca.ToLower().Contains(pesquisa))
                .ToList();

            MostrarVeiculos(filtrados);
        }
    }
}
