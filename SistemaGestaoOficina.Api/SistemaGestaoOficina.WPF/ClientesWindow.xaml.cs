using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaGestaoOficina.WPF
{
    /// <summary>
    /// Interaction logic for ClientesWindow.xaml
    /// </summary>
    public partial class ClientesWindow : Window
    {
        #region Atributos 

        private NetworkService networkService;

        private ApiService apiService;

        private List<Cliente> clientes;

        #endregion

        public ClientesWindow()
        {
            InitializeComponent();

            networkService = new NetworkService();

            apiService = new ApiService();

            LoadClientes();

        }

        #region Métodos
        /// <summary>
        /// 
        /// </summary>
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
                MessageBox.Show(response.Message, "Erro");

                return;
            }

            clientes = (List<Cliente>)response.Result;

            dataGridClientes.ItemsSource = clientes;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LimparCliente()
        {
            txtNome.Text = string.Empty;
            txtApelido.Text = string.Empty;
            txtContacto.Text = string.Empty;
            txtNif.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }

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
                string email = txtEmail.Text;

                int posArroba = email.IndexOf("@");

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

                string depoisArroba = email.Substring(posArroba + 1);

                if (!depoisArroba.Contains("."))
                {
                    MessageBox.Show(
                        "Email inválido. Falta o domínio.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtEmail.Focus();
                    return false;
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

                if (txtApelido.Text.Trim().Length < 2)
                {
                    MessageBox.Show(
                        "O apelido deve ter pelo menos 2 letras.",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    txtApelido.Focus();
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && txtEmail.Text.Contains(" "))
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

        #region Btn
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaWPF())
            {
                return;
            }

            Cliente cliente = new Cliente
            {
                Nome = txtNome.Text.Trim(),
                Apelido = txtApelido.Text.Trim(),
                Contacto = txtContacto.Text.Trim(),
                NIF = txtNif.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            var response = await apiService.Post<Cliente>("https://localhost:44390/", "api/clientes", cliente);

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

            MessageBox.Show("Cliente criado com sucesso.", "Sucesso");

            LoadClientes();
            LimparCliente();
        }

        #region Validaçao
        private void txtNif_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

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

        private void txtNome_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || c == ' ');
        }


        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            Cliente clienteSelecionado = dataGridClientes.SelectedItem as Cliente;


            if (clienteSelecionado == null)
            {
                MessageBox.Show("Selecione um cliente.", "Aviso");
                return;
            }

            EditarClientesWindow janela = new EditarClientesWindow(clienteSelecionado);

            janela.ShowDialog();

            LoadClientes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async void btnApagar_Click(object sender, RoutedEventArgs e)
        {
            Cliente clienteSelecionado = dataGridClientes.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                MessageBox.Show("Selecione um cliente.", "Aviso");
                return;
            }

            MessageBoxResult confirmar = MessageBox.Show("Tem certeza que deseja apagar este clien", "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.No)
            {
                return;
            }

            var response = await apiService.Delete("https://localhost:44390/", "api/clientes/" + clienteSelecionado.Id);

            if (!response.IsSuccess)
            {
                MessageBox.Show("Não é possível apagar este cliente porque possui um ou mais veículos associados.",
                    "Cliente não pode ser apagado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Cliente apagado com sucesso.", "Sucesso");

            LoadClientes();
        }
        #endregion

        private void btnAdicionarCarro_Click(object sender, RoutedEventArgs e)
        {
            AdicionarVeiculoWindow adicionarVeiculoWindow = new AdicionarVeiculoWindow();

            adicionarVeiculoWindow.ShowDialog();

            LoadClientes();

        }
    }
}
