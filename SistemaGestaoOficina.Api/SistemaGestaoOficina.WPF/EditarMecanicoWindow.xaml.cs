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
    /// Interaction logic for EditarMecanicoWindow.xaml
    /// </summary>
    public partial class EditarMecanicoWindow : Window
    {
        private ApiService apiService;

        private Mecanico mecanico;

        public EditarMecanicoWindow(Mecanico mecanico)
        {
            InitializeComponent();

            apiService = new ApiService();

            this.mecanico = mecanico;

            CarregarEspecialidades();

            CarregarMecanico();
        }

        /// <summary>
        /// Carrega os dados do mecânico.
        /// </summary>
        private void CarregarMecanico()
        {
            txtId.Text = mecanico.Id.ToString();
            txtNome.Text = mecanico.Nome;
            txtHorario.Text = mecanico.Horario;
            txtContacto.Text = mecanico.Contacto;

            comboBoxEspecialidade.SelectedItem = (Especialidade)Enum.Parse(typeof(Especialidade), mecanico.Especialidade);
        }

        /// <summary>
        /// Valida os dados do mecânico.
        /// </summary>
        /// <returns></returns>
        private bool ValidaWPF()
        {
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

            if (txtNome.Text.Trim().Length < 2)
            {
                MessageBox.Show(
                    "O nome deve ter pelo menos 2 letras.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtNome.Focus();
                return false;
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
        /// Carrega as especialidades.
        /// </summary>
        private void CarregarEspecialidades()
        {
            comboBoxEspecialidade.ItemsSource = Enum.GetValues(typeof(Especialidade));
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
        /// Permite apenas a introdução de números.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContacto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Atualiza os dados do mecânico.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Mecanico mecanicoEditado = new Mecanico
            {
                Id = int.Parse(txtId.Text),
                Nome = txtNome.Text.Trim(),
                Especialidade = comboBoxEspecialidade.SelectedItem.ToString(),
                Horario = txtHorario.Text,
                Contacto = txtContacto.Text.Trim(),
                Ativo = mecanico.Ativo
            };

            var response = await apiService.Put<Mecanico>("https://localhost:44390/", "api/mecanicos/" + mecanicoEditado.Id, mecanicoEditado);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            MessageBox.Show("Mecânico atualizado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();

        }

        /// <summary>
        /// Fecha a janela.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
