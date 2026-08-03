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
    /// Interaction logic for EditarClientesWindow.xaml
    /// </summary>
    public partial class EditarClientesWindow : Window
    {
        private ApiService apiService;

        private Cliente cliente;

        private List<Veiculo> veiculos;
        public EditarClientesWindow(Cliente cliente)
        {
            InitializeComponent();

            apiService = new ApiService();

            this.cliente = cliente;

            CarregarCliente(cliente);

            LoadVeiculos();

        }

        #region Metodos 

        /// <summary>
        /// Carrega os dados do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        private void CarregarCliente(Cliente cliente)
        {
            txtId.Text = cliente.Id.ToString();
            txtNome.Text = cliente.Nome;
            txtApelido.Text = cliente.Apelido;
            txtContacto.Text = cliente.Contacto;
            txtNif.Text = cliente.NIF;
            txtEmail.Text = cliente.Email;
        }

        /// <summary>
        /// Carrega os veículos do cliente.
        /// </summary>
        private async void LoadVeiculos()
        {
            var response = await apiService.Get<Veiculo>(
                "https://localhost:44390/",
                "api/veiculos");

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            List<Veiculo> todosVeiculos =
                (List<Veiculo>)response.Result;

            veiculos = todosVeiculos
                .Where(v => v.IdCliente == cliente.Id)
                .ToList();

            dataGridVeiculos.ItemsSource = veiculos;
        }


        /// <summary>
        /// Valida os dados do cliente.
        /// </summary>
        /// <returns></returns>
        private bool ValidaWPF()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) &&
                string.IsNullOrWhiteSpace(txtApelido.Text) &&
                string.IsNullOrWhiteSpace(txtContacto.Text) &&
                string.IsNullOrWhiteSpace(txtNif.Text) &&
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show(
                    "Por favor, preencha os campos.",
                    "Atenção",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show(
                    "Insira o nome do cliente.",
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

            if (string.IsNullOrWhiteSpace(txtApelido.Text))
            {
                MessageBox.Show(
                    "Insira o apelido do cliente.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtApelido.Focus();
                return false;
            }

            foreach (char c in txtApelido.Text)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    MessageBox.Show(
                        "O apelido deve conter apenas letras.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtApelido.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtContacto.Text))
            {
                MessageBox.Show(
                    "Insira o contacto do cliente.",
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

            if (txtContacto.Text.Length < 9)
            {
                MessageBox.Show(
                    "O contacto deve ter pelo menos 9 dígitos.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtContacto.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtNif.Text))
            {
                foreach (char c in txtNif.Text)
                {
                    if (!char.IsDigit(c))
                    {
                        MessageBox.Show(
                            "O NIF deve conter apenas números.",
                            "Erro",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        txtNif.Focus();
                        return false;
                    }
                }

                if (txtNif.Text.Length != 9)
                {
                    MessageBox.Show(
                        "O NIF deve ter exatamente 9 dígitos.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtNif.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();

                int posArroba = email.IndexOf("@");
                int ultimoPonto = email.LastIndexOf(".");

                if (posArroba <= 0)
                {
                    MessageBox.Show(
                        "Email inválido. Exemplo: nome@gmail.com",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtEmail.Focus();
                    return false;
                }

                if (ultimoPonto < posArroba + 2)
                {
                    MessageBox.Show(
                        "Email inválido. Falta o domínio.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtEmail.Focus();
                    return false;
                }

                string extensao = email.Substring(ultimoPonto + 1);

                if (extensao.Length < 2)
                {
                    MessageBox.Show(
                        "A extensão do email deve ter pelo menos 2 letras.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtEmail.Focus();
                    return false;
                }

                foreach (char c in extensao)
                {
                    if (!char.IsLetter(c))
                    {
                        MessageBox.Show(
                            "A extensão do email deve conter apenas letras.",
                            "Erro",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        txtEmail.Focus();
                        return false;
                    }
                }

                if (email.Contains(" "))
                {
                    MessageBox.Show(
                        "O email não pode conter espaços.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        #endregion


        /// <summary>
        /// Atualiza os dados do cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Cliente cliente = new Cliente
            {
                Id = int.Parse(txtId.Text),
                Nome = txtNome.Text.Trim(),
                Apelido = txtApelido.Text.Trim(),
                Contacto = txtContacto.Text.Trim(),
                NIF = txtNif.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            var response = await apiService.Put<Cliente>(
                "https://localhost:44390/",
                "api/clientes/" + cliente.Id,
                cliente);


            if (!response.IsSuccess)
            {
                if (response.Message.Contains("NIF"))
                {
                    MessageBox.Show(
                        "Já existe um cliente com este NIF.",
                        "NIF duplicado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else if (response.Message.Contains("Contacto") ||
                         response.Message.Contains("contacto"))
                {
                    MessageBox.Show(
                        "Já existe um cliente com este contacto.",
                        "Contacto duplicado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else if (response.Message.Contains("Email") ||
                         response.Message.Contains("email"))
                {
                    MessageBox.Show(
                        "Já existe um cliente com este email.",
                        "Email duplicado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        response.Message,
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                return;
            }

            MessageBox.Show("Cliente atualizado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();

        }

        /// <summary>
        /// Permite apenas a introdução de números.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNif_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Permite apenas a introdução do contacto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContacto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Text == "+")
            {
                e.Handled = textBox.Text.Length > 0;
                return;
            }

            e.Handled = !e.Text.All(char.IsDigit);
        }

        /// <summary>
        /// Permite apenas a introdução de letras
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNome_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || c == ' ');
        }

        /// <summary>
        /// Abre a janela para editar o veículo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditarVeiculo_Click(object sender, RoutedEventArgs e)
        {
            Veiculo veiculoSelecionado = dataGridVeiculos.SelectedItem as Veiculo;

            if (veiculoSelecionado == null)
            {
                MessageBox.Show(
                    "Selecione um veículo.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EditarVeiculoWindow janela = new EditarVeiculoWindow(veiculoSelecionado);

            janela.ShowDialog();

            LoadVeiculos();
        }

        /// <summary>
        /// Apaga o veículo selecionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnApagarVeiculo_Click(object sender, RoutedEventArgs e)
        {
            Veiculo veiculoSelecionado = dataGridVeiculos.SelectedItem as Veiculo;

            if (veiculoSelecionado == null)
            {
                MessageBox.Show("Selecione um veículo.", "Aviso");
                return;
            }

            MessageBoxResult confirmar = MessageBox.Show(
                "Tem certeza que deseja apagar este veículo?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.No)
            {
                return;
            }

            var response = await apiService.Delete("https://localhost:44390/", "api/veiculos/" + veiculoSelecionado.Id);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            MessageBox.Show("Veículo apagado com sucesso.", "Sucesso");

            LoadVeiculos();
        }
    }
}
