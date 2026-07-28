using SistemaGestaoOficina.WPF.Models;
using SistemaGestaoOficina.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            Cliente cliente = new Cliente
            {
                Nome = txtNome.Text,
                Apelido = txtApelido.Text,
                Contacto = txtContacto.Text,
                NIF = txtNif.Text,
                Email = txtEmail.Text
            };

            var response = await apiService.Post<Cliente>("https://localhost:44390/","api/clientes", cliente);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            MessageBox.Show("Cliente criado com sucesso.", "Sucesso");

            LoadClientes();
            LimparCliente();
        }

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
        private async Task btnApagar_Click(object sender, RoutedEventArgs e)
        {
            Cliente clienteSelecionado = dataGridClientes.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                MessageBox.Show("Selecione um cliente.", "Aviso");
                return;
            }

            MessageBoxResult confirmar = MessageBox.Show("Tem certeza que deseja apagar este clien","Confirmação", 
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
    }
}
