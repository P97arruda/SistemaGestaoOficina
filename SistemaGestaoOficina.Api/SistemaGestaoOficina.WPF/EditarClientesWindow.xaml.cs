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
        public EditarClientesWindow(Cliente cliente)
        {
            InitializeComponent();

            apiService = new ApiService();

            CarregarCliente(cliente);

        }

        private void CarregarCliente(Cliente cliente)
        {
            txtId.Text = cliente.Id.ToString();
            txtNome.Text = cliente.Nome;
            txtApelido.Text = cliente.Apelido;
            txtContacto.Text = cliente.Contacto;
            txtNif.Text = cliente.NIF;
            txtEmail.Text = cliente.Email;
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            Cliente cliente = new Cliente
            {
                Id = int.Parse(txtId.Text),
                Nome = txtNome.Text,
                Apelido = txtApelido.Text,
                Contacto = txtContacto.Text,
                NIF = txtNif.Text,
                Email = txtEmail.Text
            };

            var response = await apiService.Put<Cliente>(
                "https://localhost:44390/",
                "api/clientes/" + cliente.Id,
                cliente);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.Message, "Erro");
                return;
            }

            MessageBox.Show("Cliente atualizado com sucesso.", "Sucesso");

            
        }
    }
}
