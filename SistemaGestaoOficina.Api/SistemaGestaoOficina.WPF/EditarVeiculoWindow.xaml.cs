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
    /// Interaction logic for EditarVeiculoWindow.xaml
    /// </summary>
    public partial class EditarVeiculoWindow : Window
    {
        private ApiService apiService;

        private Veiculo veiculo;
        public EditarVeiculoWindow(Veiculo veiculo)
        {
            InitializeComponent();

            apiService = new ApiService();

            this.veiculo = veiculo;

            CarregarEnums();

            CarregarAnos();

            CarregarVeiculo();

        }


        /// <summary>
        /// Carrega os dados do veículo.
        /// </summary>
        private void CarregarVeiculo()
        {
            txtId.Text = veiculo.Id.ToString();
            txtModelo.Text = veiculo.Modelo;
            txtMatricula.Text = veiculo.Matricula;
            txtQuilometragem.Text = veiculo.Quilometragem.ToString();

            comboBoxMarca.SelectedItem =
                (MarcaVeiculos)Enum.Parse(
                    typeof(MarcaVeiculos),
                    veiculo.Marca);

            comboBoxCombustivel.SelectedItem =
                (Combustivel)Enum.Parse(
                    typeof(Combustivel),
                    veiculo.Combustivel);

            comboBoxAno.SelectedItem = veiculo.Ano;
        }




        /// <summary>
        /// Carrega os anos disponíveis.
        /// </summary>
        private void CarregarAnos()
        {
            for (int ano = 2026; ano >= 2000; ano--)
            {
                comboBoxAno.Items.Add(ano);
            }
        }


        /// <summary>
        /// Carrega as marcas e os combustíveis.
        /// </summary>
        private void CarregarEnums()
        {
            comboBoxMarca.ItemsSource = Enum.GetValues(typeof(MarcaVeiculos));

            comboBoxCombustivel.ItemsSource = Enum.GetValues(typeof(Combustivel));
        }

        /// <summary>
        /// Permite apenas a introdução de números.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQuilometragem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Valida os dados do veículo
        /// </summary>
        /// <returns></returns>
        private bool ValidaWPF()
        {
            if (string.IsNullOrWhiteSpace(txtMatricula.Text))
            {
                MessageBox.Show(
                    "Insira a matrícula do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtMatricula.Focus();
                return false;
            }

            if (comboBoxMarca.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione a marca do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                comboBoxMarca.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtModelo.Text))
            {
                MessageBox.Show(
                    "Insira o modelo do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtModelo.Focus();
                return false;
            }

            if (comboBoxAno.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione o ano do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                comboBoxAno.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtQuilometragem.Text))
            {
                MessageBox.Show(
                    "Insira a quilometragem do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtQuilometragem.Focus();
                return false;
            }

            int quilometragem;

            if (!int.TryParse(txtQuilometragem.Text, out quilometragem))
            {
                MessageBox.Show(
                    "A quilometragem deve conter apenas números.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtQuilometragem.Focus();
                return false;
            }

            if (quilometragem < 0)
            {
                MessageBox.Show(
                    "A quilometragem não pode ser negativa.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtQuilometragem.Focus();
                return false;
            }

            if (comboBoxCombustivel.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione o combustível do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                comboBoxCombustivel.Focus();
                return false;
            }

            string matricula = txtMatricula.Text.Trim();

            if (matricula.Length < 4)
            {
                MessageBox.Show(
                    "A matrícula deve ter pelo menos 4 caracteres.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtMatricula.Focus();
                return false;
            }

            if (matricula.Length > 10)
            {
                MessageBox.Show(
                    "A matrícula não pode ter mais de 10 caracteres.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtMatricula.Focus();
                return false;
            }

            foreach (char c in matricula)
            {
                if (!char.IsLetterOrDigit(c) && c != '-')
                {
                    MessageBox.Show(
                        "A matrícula só pode conter letras, números e hífen.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtMatricula.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Atualiza os dados do veículo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Veiculo veiculoEditado = new Veiculo
            {
                Id = int.Parse(txtId.Text),
                Matricula = txtMatricula.Text.Trim(),
                Marca = comboBoxMarca.SelectedItem.ToString(),
                Modelo = txtModelo.Text.Trim(),
                Ano = (int)comboBoxAno.SelectedItem,
                Quilometragem = int.Parse(txtQuilometragem.Text),
                Combustivel = comboBoxCombustivel.SelectedItem.ToString(),
                IdCliente = veiculo.IdCliente
            };

            var response = await apiService.Put<Veiculo>(
                "https://localhost:44390/",
                "api/veiculos/" + veiculoEditado.Id,
                veiculoEditado);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            MessageBox.Show("Veículo atualizado com sucesso.", "Sucesso");

            Close();
        }

        /// <summary>
        /// Fecha a janela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
    }
}
