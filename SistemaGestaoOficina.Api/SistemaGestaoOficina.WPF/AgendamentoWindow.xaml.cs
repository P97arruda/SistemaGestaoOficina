using SistemaGestaoOficina.WPF.Enums;
using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SistemaGestaoOficina.WPF
{
    /// <summary>
    /// Interaction logic for AgendamentoWindow.xaml
    /// </summary>
    public partial class AgendamentoWindow : Window
    {
        private NetworkService networkService;

        private ApiService apiService;

        private List<Veiculo> Veiculos;

        private List<Mecanico> Mecanicos;

        private List<Marcacao> Marcacoes;


        private Cliente cliente;
        public AgendamentoWindow(Cliente cliente)
        {
            InitializeComponent();

            this.cliente = cliente;

            networkService = new NetworkService();

            apiService = new ApiService();

            Veiculos = new List<Veiculo>();

            Marcacoes = new List<Marcacao>();

            Mecanicos = new List<Mecanico>();

            CarregarCliente();
            CarregarVeiculos();
            CarregarTiposServico();
            CarregarMecanicos();
            CarregarHorarios();
            CarregarMarcacoes();

        }

        /// <summary>
        /// 
        /// </summary>
        private async void CarregarCliente()
        {
            txtCliente.Text = cliente.Nome + " " + cliente.Apelido;
        }

        /// <summary>
        /// 
        /// </summary>
        private async void CarregarVeiculos()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var response = await apiService.Get<Veiculo>(
                "https://localhost:44390/",
                "api/veiculos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Veiculo> todosVeiculos =
                (List<Veiculo>)response.Result;

            Veiculos = todosVeiculos
                .Where(v => v.IdCliente == cliente.Id)
                .ToList();

            comboBoxVeiculo.ItemsSource = Veiculos;
        }

        /// <summary>
        /// 
        /// </summary>
        private async void CarregarMecanicos()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var response = await apiService.Get<Mecanico>(
                "https://localhost:44390/",
                "api/mecanicos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Mecanico> todosMecanicos =
                (List<Mecanico>)response.Result;

            Mecanicos = todosMecanicos
                .Where(m => m.Ativo)
                .ToList();

            comboBoxMecanico.ItemsSource = Mecanicos;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarTiposServico()
        {
            comboBoxTipoServico.ItemsSource =
                Enum.GetValues(typeof(TipoServico));
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarHorarios()
        {
            List<string> horarios = new List<string>();

            horarios.Add("09:00");
            horarios.Add("10:00");
            horarios.Add("11:00");
            horarios.Add("12:00");
            horarios.Add("14:00");
            horarios.Add("15:00");
            horarios.Add("16:00");
            horarios.Add("17:00");

            listBoxHorarios.ItemsSource = horarios;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendarMarcacao_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendarMarcacao.SelectedDate == null)
            {
                return;
            }

            DateTime data = calendarMarcacao.SelectedDate.Value;

            if (data.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show(
                    "Não é possível fazer marcações ao domingo.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                calendarMarcacao.SelectedDate = null;

                listBoxHorarios.ItemsSource = null;

                return;
            }

            CarregarHorariosDisponiveis();
        }

        /// <summary>
        /// 
        /// </summary>
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
        }

        /// <summary>
        /// 
        /// </summary>
        private async void CarregarMarcacoes()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var response = await apiService.Get<Marcacao>(
                "https://localhost:44390/",
                "api/marcacoes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Marcacoes = (List<Marcacao>)response.Result;

            CarregarHorariosDisponiveis();
        }


        private void comboBoxMecanico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregarHorariosDisponiveis();
        }

        private bool ValidaWPF()
        {
            if (comboBoxVeiculo.SelectedItem == null)
            {
                MessageBox.Show("Selecione um veículo.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (comboBoxMecanico.SelectedItem == null)
            {
                MessageBox.Show("Selecione um mecânico.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (comboBoxTipoServico.SelectedItem == null)
            {
                MessageBox.Show("Selecione o tipo de serviço.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (calendarMarcacao.SelectedDate == null)
            {
                MessageBox.Show("Selecione uma data.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (listBoxHorarios.SelectedItem == null)
            {
                MessageBox.Show("Selecione um horário.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Veiculo veiculo = (Veiculo)comboBoxVeiculo.SelectedItem;

            Mecanico mecanico = (Mecanico)comboBoxMecanico.SelectedItem;

            string horario = listBoxHorarios.SelectedItem.ToString();

            int hora = Convert.ToInt32(horario.Split(':')[0]);

            int minuto = Convert.ToInt32(horario.Split(':')[1]);

            DateTime data = calendarMarcacao.SelectedDate.Value;

            DateTime dataHora = new DateTime(
                data.Year,
                data.Month,
                data.Day,
                hora,
                minuto,
                0);

            Marcacao marcacao = new Marcacao();

            marcacao.IdCliente = cliente.Id;

            marcacao.IdVeiculo = veiculo.Id;

            marcacao.IdMecanico = mecanico.Id;

            marcacao.TipoServico =
                comboBoxTipoServico.SelectedItem.ToString();

            marcacao.DataHora = dataHora;

            marcacao.Estado = "Pendente";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            var response = await apiService.Post(
                "https://localhost:44390/",
                "api/marcacoes",
                marcacao);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show("Marcação criada com sucesso!",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.Close();
        }
    }
}
