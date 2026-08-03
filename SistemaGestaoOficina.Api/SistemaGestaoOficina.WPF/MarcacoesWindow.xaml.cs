using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SistemaGestaoOficina.WPF
{
    /// <summary>
    /// Interaction logic for MarcacoesWindow.xaml
    /// </summary>
    public partial class MarcacoesWindow : Window
    {
        private NetworkService networkService;

        private ApiService apiService;

        private List<Marcacao> Marcacoes;

        private List<Cliente> Clientes;

        private List<Veiculo> Veiculos;

        private List<Mecanico> Mecanicos;
        public MarcacoesWindow()
        {
            InitializeComponent();

            networkService = new NetworkService();

            apiService = new ApiService();

            Marcacoes = new List<Marcacao>();

            Clientes = new List<Cliente>();

            Veiculos = new List<Veiculo>();

            Mecanicos = new List<Mecanico>();

            CarregarClientes();

        }

        // <summary>
        /// Mostra as marcações e atualiza o total.
        /// </summary>
        /// <param name="lista"></param>
        private void MostrarMarcacoes(List<Marcacao> lista)
        {
            listBoxMarcacoes.ItemsSource = null;

            listBoxMarcacoes.ItemsSource = lista;

            lblTotalMarcacoes.Text = "Total: " + lista.Count + " marcações";

            listBoxDetalhesMarcacao.ItemsSource = null;
        }


        /// <summary>
        /// Carrega todas as marcações da API.
        /// </summary>
        private async Task CarregarMarcacoes()
        {
            var response = await apiService.Get<Marcacao>("https://localhost:44390/", "api/marcacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            List<Marcacao> todasMarcacoes =
                (List<Marcacao>)response.Result;

            Marcacoes = todasMarcacoes
                .Select(m =>
                {
                    Cliente cliente = Clientes.FirstOrDefault(c => c.Id == m.IdCliente);

                    Veiculo veiculo = Veiculos.FirstOrDefault(v => v.Id == m.IdVeiculo);

                    Mecanico mecanico = Mecanicos.FirstOrDefault(me => me.Id == m.IdMecanico);

                    if (cliente != null)
                    {
                        m.NomeCliente = cliente.Nome + " " + cliente.Apelido;
                    }

                    if (veiculo != null)
                    {
                        m.MarcaVeiculo = veiculo.Marca;

                        m.ModeloVeiculo = veiculo.Modelo;

                        m.Matricula = veiculo.Matricula;
                    }

                    if (mecanico != null)
                    {
                        m.NomeMecanico = mecanico.Nome;
                    }

                    return m;
                })
                .ToList();

            MostrarMarcacoes(Marcacoes);
        }

        /// <summary>
        /// Carrega os mecânicos da API
        /// </summary>
        private async Task CarregarMecanicos()
        {
            var response = await apiService.Get<Mecanico>("https://localhost:44390/", "api/mecanicos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Mecanicos = (List<Mecanico>)response.Result;

            CarregarMarcacoes();
        }
        /// <summary>
        /// Carrega os veículos da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarVeiculos()
        {
            var response = await apiService.Get<Veiculo>("https://localhost:44390/", "api/veiculos");

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

            CarregarMecanicos();
        }

        /// <summary>
        /// Carrega os clientes da API.
        /// </summary>
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
        /// Mostra os detalhes da marcação selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxMarcacoes_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            Marcacao marcacaoSelecionada = listBoxMarcacoes.SelectedItem as Marcacao;

            if (marcacaoSelecionada == null)
            {
                return;
            }

            List<string> detalhes = new List<string>();

            detalhes.Add(
                "Cliente: " + marcacaoSelecionada.NomeCliente);

            detalhes.Add(
                "Veículo: " +
                marcacaoSelecionada.MarcaVeiculo + " | " +
                marcacaoSelecionada.ModeloVeiculo + " | " +
                marcacaoSelecionada.Matricula);

            detalhes.Add(
                "Mecânico: " + marcacaoSelecionada.NomeMecanico);

            detalhes.Add(
                "Tipo de serviço: " + marcacaoSelecionada.TipoServico);

            detalhes.Add(
                "Data e hora: " + marcacaoSelecionada.DataHora.ToString("dd/MM/yyyy HH:mm"));

            detalhes.Add(
                "Estado: " + marcacaoSelecionada.Estado);

            listBoxDetalhesMarcacao.ItemsSource = null;

            listBoxDetalhesMarcacao.ItemsSource = detalhes;
        }

        /// <summary>
        /// Pesquisa marcações.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPesquisarMarcacao_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisarMarcacao.Text))
            {
                MostrarMarcacoes(Marcacoes);

                return;
            }

            string pesquisa =
                txtPesquisarMarcacao.Text
                    .ToLower()
                    .Trim();

            List<Marcacao> filtradas = Marcacoes
                .Where(m =>
                    m.NomeCliente.ToLower().Trim().Contains(pesquisa) ||
                    m.Matricula.ToLower().Trim().Contains(pesquisa) ||
                    m.TipoServico.ToLower().Trim().Contains(pesquisa))
                .ToList();

            MostrarMarcacoes(filtradas);
        }
    }
}
