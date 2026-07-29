using SistemaGestaoOficina.WPF.Enums;
using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SistemaGestaoOficina.WPF
{
    /// <summary>
    /// Interaction logic for AdicionarVeiculoWindow.xaml
    /// </summary>
    public partial class AdicionarVeiculoWindow : Window
    {
        private ApiService apiService;

        private NetworkService networkService;

        private List<Cliente> clientes;

        public AdicionarVeiculoWindow()
        {
            InitializeComponent();

            apiService = new ApiService();

            networkService = new NetworkService();

            CarregarEnums();
            CarregarAnos();
            LoadClientes();

        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarEnums()
        {
            comboBoxMarca.ItemsSource = Enum.GetValues(typeof(MarcaVeiculos));
            comboBoxCombustivel.ItemsSource = Enum.GetValues(typeof(Combustivel));
        }

        private void CarregarAnos()
        {
            for (int ano = 2026; ano >= 2000; ano--)
            {
                comboBoxAno.Items.Add(ano);
            }

            comboBoxAno.SelectedItem = DateTime.Now.Year;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async void LoadClientes()
        {
            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message, "Erro");
                return;
            }

            var response = await apiService.Get<Cliente>("https://localhost:44390/", "api/clientes");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Error");
                return;
            }

            clientes = (List<Cliente>)response.Result;

            comboBoxCliente.ItemsSource = clientes;

            comboBoxCliente.DisplayMemberPath = "NomeCompleto";

            comboBoxCliente.SelectedValuePath = "Id";
        }

        private bool ValidaWPF()
        {
            if (comboBoxCliente.SelectedItem == null &&
                comboBoxMarca.SelectedItem == null &&
                string.IsNullOrWhiteSpace(txtModelo.Text) &&
                string.IsNullOrWhiteSpace(txtMatricula.Text) &&
                comboBoxAno.SelectedItem == null &&
                comboBoxCombustivel.SelectedItem == null &&
                string.IsNullOrWhiteSpace(txtQuilometragem.Text))
            {
                MessageBox.Show(
                    "Por favor, preencha todos os campos.",
                    "Atenção",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (comboBoxCliente.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione o cliente do veículo.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                comboBoxCliente.Focus();
                return false;
            }

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

            if (txtMatricula.Text.Trim().Length < 4)
            {
                MessageBox.Show(
                    "A matrícula deve ter pelo menos 4 caracteres.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtMatricula.Focus();
                return false;
            }

            if (txtMatricula.Text.Trim().Length > 10)
            {
                MessageBox.Show(
                    "A matrícula não pode ter mais de 10 caracteres.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtMatricula.Focus();
                return false;
            }

            foreach (char c in txtMatricula.Text)
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

            return true;
        }




        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Veiculo viculo = new Veiculo
            {
                Matricula = txtMatricula.Text.Trim(),
                Marca = comboBoxMarca.SelectedItem.ToString(),
                Modelo = txtModelo.Text.Trim(),
                Ano = (int)comboBoxAno.SelectedItem,
                Quilometragem = int.Parse(txtQuilometragem.Text),
                Combustivel = comboBoxCombustivel.SelectedItem.ToString(),
                IdCliente = (int)comboBoxCliente.SelectedValue
            };

            var response = await apiService.Post<Veiculo>("https://localhost:44390/", "api/veiculos", viculo);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message,"Erro", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }

            MessageBox.Show("Veículo criado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void txtQuilometragem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
