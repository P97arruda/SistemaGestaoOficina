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
    /// Interaction logic for MecanicosWindow.xaml
    /// </summary>
    public partial class MecanicosWindow : Window
    {
        private NetworkService networkService;

        private ApiService apiService;

        private List<Mecanico> mecanicos;
        public MecanicosWindow()
        {
            InitializeComponent();

            networkService = new NetworkService();

            apiService = new ApiService();

            CarregarEspecialidades();

            LoadMecanicos();
        }

        /// <summary>
        /// Carrega os mecânicos da API.
        /// </summary>
        /// <returns></returns>
        private async Task LoadMecanicos()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro");
                return;
            }

            var response = await apiService.Get<Mecanico>("https://localhost:44390/", "api/mecanicos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            mecanicos = (List<Mecanico>)response.Result;

            listBoxMecanicos.ItemsSource = mecanicos;
        }

        /// <summary>
        /// Carrega as especialidades.
        /// </summary>
        private void CarregarEspecialidades()
        {
            comboBoxEspecialidade.ItemsSource = Enum.GetValues(typeof(Especialidade));
        }

        /// <summary>
        /// Valida os dados do mecânico.
        /// </summary>
        /// <returns></returns>
        private bool ValidaWPF()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) &&
                comboBoxEspecialidade.SelectedItem == null &&
                string.IsNullOrWhiteSpace(txtContacto.Text))
            {
                MessageBox.Show(
                    "Por favor, preencha todos os campos.",
                    "Atenção",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show(
                    "Insira o nome do mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtNome.Focus();
                return false;
            }

            foreach (char c in txtNome.Text)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    MessageBox.Show(
                        "O nome deve conter apenas letras.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtNome.Focus();
                    return false;
                }
            }

            if (comboBoxEspecialidade.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione a especialidade do mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                comboBoxEspecialidade.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContacto.Text))
            {
                MessageBox.Show(
                    "Insira o contacto do mecânico.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtContacto.Focus();
                return false;
            }

            foreach (char c in txtContacto.Text)
            {
                if (!char.IsDigit(c) && c != '+')
                {
                    MessageBox.Show(
                        "O contacto deve conter apenas números.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtContacto.Focus();
                    return false;
                }
            }

            if (txtContacto.Text.Trim().Length < 9)
            {
                MessageBox.Show(
                    "O contacto deve ter pelo menos 9 dígitos.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtContacto.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Permite apenas a introdução do contacto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContacto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "+")
            {
                e.Handled = txtContacto.Text.Contains("+") || txtContacto.CaretIndex != 0;
                return;
            }

            e.Handled = !e.Text.All(char.IsDigit);
        }



        /// <summary>
        /// Permite apenas a introdução de letras.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNome_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || c == ' ');
        }

        /// <summary>
        /// Guarda um novo mecânico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Mecanico mecanico = new Mecanico
            {
                Nome = txtNome.Text.Trim(),
                Especialidade = comboBoxEspecialidade.SelectedItem.ToString(),
                Horario = txtHorario.Text,
                Contacto = txtContacto.Text.Trim(),
                Ativo = true
            };

            var response = await apiService.Post<Mecanico>("https://localhost:44390/", "api/mecanicos", mecanico);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show("Mecânico criado com sucesso.", "Sucesso");

            LoadMecanicos();
            LimparCampos();
        }

        /// <summary>
        /// Limpa os campos do formulário.
        /// </summary>
        private void LimparCampos()
        {
            txtNome.Text = string.Empty;
            comboBoxEspecialidade.SelectedIndex = -1;
            txtContacto.Text = string.Empty;
        }

        /// <summary>
        /// Apaga o mecânico selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnApagar_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado = listBoxMecanicos.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult confirmar = MessageBox.Show(
                "Tem certeza que deseja apagar este mecânico?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.No)
            {
                return;
            }

            var response = await apiService.Delete("https://localhost:44390/", "api/mecanicos/" + mecanicoSelecionado.Id);

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
        "Mecânico apagado com sucesso.",
        "Sucesso",
        MessageBoxButton.OK,
        MessageBoxImage.Information);

            await LoadMecanicos();
        }

        /// <summary>
        /// Abre a janela para editar o mecânico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado = listBoxMecanicos.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EditarMecanicoWindow janela = new EditarMecanicoWindow(mecanicoSelecionado);

            janela.ShowDialog();

            LoadMecanicos();
        }

        /// <summary>
        /// Desativa o mecânico selecionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDesativar_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado =
                listBoxMecanicos.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            mecanicoSelecionado.Ativo = false;

            var response = await apiService.Put<Mecanico>( "https://localhost:44390/","api/mecanicos/" + mecanicoSelecionado.Id,mecanicoSelecionado);

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Mecânico desativado com sucesso.",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadMecanicos();
        }

        /// <summary>
        /// Ativa o mecânico selecionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAtivar_Click(object sender, RoutedEventArgs e)
        {
            Mecanico mecanicoSelecionado = listBoxMecanicos.SelectedItem as Mecanico;

            if (mecanicoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um mecânico.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            mecanicoSelecionado.Ativo = true;

            var response = await apiService.Put<Mecanico>("https://localhost:44390/", "api/mecanicos/" + mecanicoSelecionado.Id, mecanicoSelecionado);

            if (!response.IsSuccess)
            {
                MessageBox.Show(
                    response.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Mecânico ativado com sucesso.",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadMecanicos();
        }
    }
}
