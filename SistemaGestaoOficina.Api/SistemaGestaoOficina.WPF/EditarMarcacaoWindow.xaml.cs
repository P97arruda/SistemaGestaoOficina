using SistemaGestaoOficina.WPF.Enums;
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
    /// Interaction logic for EditarMarcacaoWindow.xaml
    /// </summary>
    public partial class EditarMarcacaoWindow : Window
    {
        #region Atributos

        private Marcacao marcacao;

        private NetworkService networkService;

        private ApiService apiService;

        private List<Veiculo> Veiculos;

        private List<Mecanico> Mecanicos;

        private List<Marcacao> Marcacoes;

        private List<Cliente> clientes;

        #endregion

        public EditarMarcacaoWindow(Marcacao marcacao)
        {
            InitializeComponent();

            this.marcacao = marcacao;

            networkService = new NetworkService();

            apiService = new ApiService();

            Veiculos = new List<Veiculo>();

            Mecanicos = new List<Mecanico>();

            Marcacoes = new List<Marcacao>();

            clientes = new List<Cliente>();

            CarregarDados();
        }

        private void CarregarDados()
        {
            comboBoxTipoServico.ItemsSource = Enum.GetValues(typeof(TipoServico));

            comboBoxTipoServico.SelectedItem = Enum.Parse(typeof(TipoServico), marcacao.TipoServico);

            calendarMarcacao.SelectedDate = marcacao.DataHora.Date;

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            CarregarClientes();

            CarregarVeiculos();

            CarregarMecanicos();

            CarregarMarcacoes();
        }

        private async Task CarregarMarcacoes()
        {
            var response = await apiService.Get<Marcacao>("https://localhost:44390/", "api/marcacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Marcacoes = (List<Marcacao>)response.Result;

            CarregarHorariosDisponiveis();
        }

        private void CarregarHorariosDisponiveis()
        {
            if (calendarMarcacao.SelectedDate == null)
            {
                return;
            }

            if (comboBoxMecanico.SelectedItem == null)
            {
                return;
            }

            List<string> horarios = new List<string>();

            horarios.Add("09:00");
            horarios.Add("10:00");
            horarios.Add("11:00");
            horarios.Add("12:00");
            horarios.Add("14:00");
            horarios.Add("15:00");
            horarios.Add("16:00");
            horarios.Add("17:00");

            DateTime dataSelecionada =
                calendarMarcacao.SelectedDate.Value;

            Mecanico mecanicoSelecionado =
                (Mecanico)comboBoxMecanico.SelectedItem;

            horarios = horarios
                .Where(h => !Marcacoes.Any(m =>
                    m.Id != marcacao.Id &&
                    m.IdMecanico == mecanicoSelecionado.Id &&
                    m.DataHora.Date == dataSelecionada.Date &&
                    m.DataHora.ToString("HH:mm") == h))
                .ToList();

            if (dataSelecionada.Date == DateTime.Today)
            {
                horarios = horarios
                    .Where(h =>
                        DateTime.Parse(h).TimeOfDay > DateTime.Now.TimeOfDay)
                    .ToList();
            }

            listBoxHorarios.ItemsSource = horarios;

            listBoxHorarios.SelectedItem =
                marcacao.DataHora.ToString("HH:mm");
        }

        private async Task CarregarMecanicos()
        {
            var response = await apiService.Get<Mecanico>("https://localhost:44390/", "api/mecanicos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,
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

            comboBoxMecanico.SelectedItem = Mecanicos
                .FirstOrDefault(m => m.Id == marcacao.IdMecanico);
        }

        private async Task CarregarVeiculos()
        {
            var response = await apiService.Get<Veiculo>("https://localhost:44390/", "api/veiculos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            List<Veiculo> todosVeiculos =
                (List<Veiculo>)response.Result;

            Veiculos = todosVeiculos
                .Where(v => v.IdCliente == marcacao.IdCliente)
                .ToList();

            comboBoxVeiculo.ItemsSource = Veiculos;

            comboBoxVeiculo.DisplayMemberPath = "Matricula";

            comboBoxVeiculo.SelectedItem = Veiculos
                .FirstOrDefault(v => v.Id == marcacao.IdVeiculo);
        }

        private async Task CarregarClientes()
        {
            var response = await apiService.Get<Cliente>("https://localhost:44390/", "api/clientes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            clientes = (List<Cliente>)response.Result;

            Cliente cliente = clientes
                .FirstOrDefault(c => c.Id == marcacao.IdCliente);

            if (cliente != null)
            {
                txtCliente.Text =
                    cliente.Nome + " " + cliente.Apelido;
            }
        }

        private void comboBoxMecanico_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            CarregarHorariosDisponiveis();
        }

        private void calendarMarcacao_SelectedDatesChanged(object sender,SelectionChangedEventArgs e)
        {
            CarregarHorariosDisponiveis();
        }

        private bool ValidaWPF()
        {
            if (comboBoxVeiculo.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione um veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (comboBoxMecanico.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (comboBoxTipoServico.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione o tipo de serviço.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (listBoxHorarios.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione um horário.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            if (calendarMarcacao.SelectedDate.Value.DayOfWeek == DayOfWeek.Saturday ||
                calendarMarcacao.SelectedDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show(
                    "Não é possível marcar ao fim de semana.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            string horario = listBoxHorarios.SelectedItem.ToString();

            int hora = Convert.ToInt32(horario.Split(':')[0]);

            int minuto = Convert.ToInt32(horario.Split(':')[1]);

            DateTime dataHora = new DateTime(
                calendarMarcacao.SelectedDate.Value.Year,
                calendarMarcacao.SelectedDate.Value.Month,
                calendarMarcacao.SelectedDate.Value.Day,
                hora,
                minuto,
                0);

            Veiculo veiculoSelecionado =
                (Veiculo)comboBoxVeiculo.SelectedItem;

            Mecanico mecanicoSelecionado =
                (Mecanico)comboBoxMecanico.SelectedItem;

            marcacao.IdVeiculo = veiculoSelecionado.Id;

            marcacao.IdMecanico = mecanicoSelecionado.Id;

            marcacao.TipoServico =
                comboBoxTipoServico.SelectedItem.ToString();

            marcacao.DataHora = dataHora;

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
    "Id: " + marcacao.Id +
    "\nIdCliente: " + marcacao.IdCliente +
    "\nIdVeiculo: " + marcacao.IdVeiculo +
    "\nEstado: " + marcacao.Estado);

            var response = await apiService.Put( "https://localhost:44390/", "api/marcacoes/" + marcacao.Id, marcacao);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Marcação atualizada com sucesso.",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
