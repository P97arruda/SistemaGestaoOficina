using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System;
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
    /// Interaction logic for ReparacoesWindow.xaml
    /// </summary>
    public partial class ReparacoesWindow : Window
    {
        #region Atributos

        private NetworkService networkService;

        private ApiService apiService;

        private List<Reparacao> Reparacoes;

        private List<Marcacao> Marcacoes;

        private List<Mecanico> Mecanicos;

        private List<Mecanico> MecanicosDaReparacao;

        private List<Veiculo> Veiculos;

        private List<string> PecasDaReparacao;

        private List<Cliente> Clientes;

        private List<Marcacao> TodasMarcacoes;

        #endregion

        public ReparacoesWindow()
        {
            InitializeComponent();

            networkService = new NetworkService();

            apiService = new ApiService();

            Reparacoes = new List<Reparacao>();

            Marcacoes = new List<Marcacao>();

            Mecanicos = new List<Mecanico>();

            Veiculos = new List<Veiculo>();

            Clientes = new List<Cliente>();

            TodasMarcacoes = new List<Marcacao>();

            MecanicosDaReparacao = new List<Mecanico>();

            PecasDaReparacao = new List<string>();

            CarregarClientes();

            CarregarVeiculos();

            CarregarMecanicos();

            CarregarMarcacoes();

            CarregarReparacoes();

        }

        /// <summary>
        /// Carrega os mecânicos ativos da API.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private async Task CarregarMecanicos()
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

            var response = await apiService.Get<Mecanico>("https://localhost:44390/","api/mecanicos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            List<Mecanico> todosMecanicos =
                (List<Mecanico>)response.Result;

            Mecanicos = todosMecanicos
                .Where(m => m.Ativo)
                .ToList();

            comboBoxMecanico.ItemsSource = Mecanicos;

            comboBoxMecanico.DisplayMemberPath = "Nome";
        }

        /// <summary>
        /// Carrega as marcações pendentes da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarMarcacoes()
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

            var response = await apiService.Get<Marcacao>("https://localhost:44390/","api/marcacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            TodasMarcacoes = (List<Marcacao>)response.Result;

            Marcacoes = TodasMarcacoes
                .Where(m => m.Estado == "Pendente").Select(m =>
                {
                    Veiculo veiculo = Veiculos.FirstOrDefault(v => v.Id == m.IdVeiculo);

                    Cliente cliente = Clientes.FirstOrDefault(c => c.Id == m.IdCliente);

                    if (veiculo != null)
                    {
                        m.MarcaVeiculo = veiculo.Marca;

                        m.ModeloVeiculo = veiculo.Modelo;

                        m.Matricula = veiculo.Matricula;
                    }

                    if (cliente != null)
                    {
                        m.NomeCliente = cliente.Nome + " " + cliente.Apelido;
                    }

                    return m;
                })
                .ToList();

            comboBoxMarcacao.ItemsSource = null;

            comboBoxMarcacao.ItemsSource = Marcacoes;

            CarregarReparacoesEmCurso();
        }



        /// <summary>
        /// Carrega as reparações da API.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarReparacoes()
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

            var response = await apiService.Get<Reparacao>(
                "https://localhost:44390/",
                "api/reparacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            List<Reparacao> todasReparacoes =
                (List<Reparacao>)response.Result;

            Reparacoes = todasReparacoes
                .Select(r =>
                {
                    Marcacao marcacao = TodasMarcacoes.FirstOrDefault(m => m.Id == r.IdMarcacao);

                    if (marcacao != null)
                    {
                        Cliente cliente = Clientes.FirstOrDefault(c => c.Id == marcacao.IdCliente);

                        Veiculo veiculo = Veiculos.FirstOrDefault(v => v.Id == marcacao.IdVeiculo);

                        if (cliente != null)
                        {
                            r.NomeCliente =cliente.Nome + " " + cliente.Apelido;
                        }

                        if (veiculo != null)
                        {
                            r.Matricula = veiculo.Matricula;

                            r.ModeloVeiculo = veiculo.Modelo;
                        }
                    }

                    return r;
                })
                .ToList();

            CarregarReparacoesConcluidas();
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
        }

        /// <summary>
        /// Carrega o mecânico da marcação selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMarcacao_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            Marcacao marcacaoSelecionada = comboBoxMarcacao.SelectedItem as Marcacao;

            if (marcacaoSelecionada == null)
            {
                return;
            }

            Mecanico mecanico = Mecanicos.FirstOrDefault(m => m.Id == marcacaoSelecionada.IdMecanico);

            MecanicosDaReparacao.Clear();

            if (mecanico != null)
            {
                MecanicosDaReparacao.Add(mecanico);
            }

            listBoxMecanicos.ItemsSource = null;

            listBoxMecanicos.ItemsSource = MecanicosDaReparacao;

            listBoxMecanicos.DisplayMemberPath = "Nome";


            datePickerInicio.SelectedDate = marcacaoSelecionada.DataHora.Date;

            datePickerInicio.IsEnabled = false;

            datePickerFim.SelectedDate = null;
        }

        /// <summary>
        /// Carrega as reparações concluídas.
        /// </summary>
        private void CarregarReparacoesConcluidas()
        {
            List<Reparacao> reparacoesConcluidas = Reparacoes.Where(r => r.Concluida).ToList();

            listBoxConcluidas.ItemsSource = null;

            listBoxConcluidas.ItemsSource = reparacoesConcluidas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidaWPF()
        {
            bool output = true;

            if (comboBoxMarcacao.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Selecione uma marcação.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (PecasDaReparacao.Count == 0)
            {
                MessageBox.Show(
                    "Adicione pelo menos uma peça.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (MecanicosDaReparacao.Count == 0)
            {
                MessageBox.Show(
                    "Adicione pelo menos um mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (datePickerInicio.SelectedDate == null)
            {
                MessageBox.Show(
                    "A data de início é obrigatória.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (datePickerFim.SelectedDate == null)
            {
                MessageBox.Show(
                    "A data de fim é obrigatória.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (datePickerFim.SelectedDate.Value <
                datePickerInicio.SelectedDate.Value)
            {
                MessageBox.Show(
                    "A data de fim não pode ser anterior à data de início.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return output;
        }

        private void btnAdicionarPeca_Click(object sender, RoutedEventArgs e)
        {
           if (string.IsNullOrWhiteSpace(txtPeca.Text))
    {
                MessageBox.Show(
                    "Introduza o nome da peça.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtPeca.Focus();
                return;
            }

            if (txtPeca.Text.Trim().Length < 3 || txtPeca.Text.Trim().Length > 100)
            {
                MessageBox.Show(
                    "O nome da peça deve ter entre 3 e 100 caracteres.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtPeca.Focus();
                return;
            }

            foreach (char c in txtPeca.Text)
            {
                if (!char.IsLetterOrDigit(c) &&
                    c != ' ' &&
                    c != '-' &&
                    c != '/')
                {
                    MessageBox.Show(
                        "O nome da peça contém caracteres inválidos.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtPeca.Focus();
                    return;
                }
            }

            decimal valor;

            bool valorValido = decimal.TryParse(txtValorPeca.Text.Replace(".", ","), out valor);

            if (!valorValido || valor <= 0)
            {
                MessageBox.Show(
                    "Insira um valor válido maior que zero.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtValorPeca.Focus();
                return;
            }

            string peca = txtPeca.Text.Trim() + " | " + valor.ToString("0.00") + "€";

            PecasDaReparacao.Add(peca);

            listBoxPecas.ItemsSource = null;

            listBoxPecas.ItemsSource = PecasDaReparacao;

            AtualizarTotal();

            txtPeca.Text = string.Empty;

            txtValorPeca.Text = string.Empty;

            txtPeca.Focus();
        }

        /// <summary>
        /// Remove uma peça da reparação.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoverPeca_Click(object sender, RoutedEventArgs e)
        {
            string pecaSelecionada = listBoxPecas.SelectedItem as string;

            if (pecaSelecionada == null)
            {
                MessageBox.Show(
                    "Selecione uma peça.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            PecasDaReparacao.Remove(pecaSelecionada);

            listBoxPecas.ItemsSource = null;

            listBoxPecas.ItemsSource = PecasDaReparacao;

            AtualizarTotal();
        }

        /// <summary>
        /// Calcula o custo total das peças da reparação.
        /// </summary>
        private void AtualizarTotal()
        {
            decimal total = 0;

            foreach (string peca in PecasDaReparacao)
            {
                string[] partes = peca.Split('|');

                if (partes.Length == 2)
                {
                    string valorTexto = partes[1]
                        .Replace("€", "")
                        .Trim();

                    decimal valor;

                    if (decimal.TryParse(valorTexto, out valor))
                    {
                        total += valor;
                    }
                }
            }

            txtCustoTotal.Text = total.ToString("0.00") + "€";
        }

        private async Task CarregarClientes()
        {
            var response = await apiService.Get<Cliente>("https://localhost:44390/", "api/clientes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

             Clientes = (List<Cliente>)response.Result;
        }

        /// <summary>
        /// Carrega as marcações em curso.
        /// </summary>
        private void CarregarReparacoesEmCurso()
        {
            listBoxEmCurso.ItemsSource = null;

            listBoxEmCurso.ItemsSource = Marcacoes;
        }

        private void btnAdicionarMecanico_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado = comboBoxMecanico.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Mecanico mecanicoExistente = MecanicosDaReparacao.FirstOrDefault(m => m.Id == mecanicoSelecionado.Id);

            if (mecanicoExistente != null)
            {
                MessageBox.Show(
                    "Este mecânico já foi adicionado.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MecanicosDaReparacao.Add(mecanicoSelecionado);

            listBoxMecanicos.ItemsSource = null;

            listBoxMecanicos.ItemsSource = MecanicosDaReparacao;

            listBoxMecanicos.DisplayMemberPath = "Nome";
        }

        private void btnRemoverMecanico_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado = listBoxMecanicos.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MecanicosDaReparacao.Remove(mecanicoSelecionado);

            listBoxMecanicos.ItemsSource = null;

            listBoxMecanicos.ItemsSource = MecanicosDaReparacao;

            listBoxMecanicos.DisplayMemberPath = "Nome";
        }

        /// <summary>
        /// Permite apenas letras.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPeca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || c == ' ');
        }

        /// <summary>
        /// Permite apenas números.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtValorPeca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsDigit(c) || c == ',' || c == '.');
        }

        /// <summary>
        /// Conclui e guarda a reparação na API.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Marcacao marcacaoSelecionada = comboBoxMarcacao.SelectedItem as Marcacao;

            decimal custoTotal;

            bool custoValido = decimal.TryParse(txtCustoTotal.Text.Replace("€", "").Trim(),out custoTotal);

            if (!custoValido || custoTotal <= 0)
            {
                MessageBox.Show(
                    "O custo total deve ser maior que zero.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Reparacao reparacao = new Reparacao();

            reparacao.IdMarcacao = marcacaoSelecionada.Id;

            reparacao.Pecas = string.Join(", ", PecasDaReparacao);

            reparacao.CustoTotal = custoTotal;

            reparacao.DataInicio = datePickerInicio.SelectedDate.Value;

            reparacao.DataFim = datePickerFim.SelectedDate.Value;

            reparacao.Concluida = true;

            reparacao.TipoServico = marcacaoSelecionada.TipoServico;

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

            var responseReparacao = await apiService.Post("https://localhost:44390/", "api/reparacoes", reparacao);

            if (!responseReparacao.IsSuccess)
            {
                MessageBox.Show(
                    responseReparacao.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            marcacaoSelecionada.Estado = "Concluída";

            var responseMarcacao = await apiService.Put("https://localhost:44390/","api/marcacoes/" + marcacaoSelecionada.Id,marcacaoSelecionada);

            if (!responseMarcacao.IsSuccess)
            {
                MessageBox.Show(
                    responseMarcacao.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Reparação concluída com sucesso.",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await CarregarReparacoes();

            await CarregarMarcacoes();

            LimparCampos();
        }

        /// <summary>
        /// Limpa os campos da reparação.
        /// </summary>
        private void LimparCampos()
        {
            txtPeca.Text = string.Empty;

            txtValorPeca.Text = string.Empty;

            txtCustoTotal.Text = string.Empty;

            datePickerFim.SelectedDate = null;

            PecasDaReparacao.Clear();

            MecanicosDaReparacao.Clear();

            listBoxPecas.ItemsSource = null;

            listBoxMecanicos.ItemsSource = null;
        }
    }
}
