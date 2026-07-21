using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;

namespace SistemaGestaoOficina.Api.Controllers
{
    public class ClientesController : ApiController
    {
        SistemaOficinaDataContextDataContext dc = new SistemaOficinaDataContextDataContext("workstation id=SistemaOficina.mssql.somee.com;packet size=4096;user" +
            " id=pliniodev97_SQLLogin_1;pwd=cnzppis3ut;data source=SistemaOficina.mssql.somee.com;persist security " +
            "info=False;initial catalog=SistemaOficina;TrustServerCertificate=True");


        // GET: api/Clientes
        public List<Cliente> Get()
        {
            var list = from Cliente in dc.Clientes select Cliente;
             return list.ToList();

        }

        // GET: api/Clientes/5
        public IHttpActionResult Get(int id)
        {
            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.Id == id);

            if(cliente != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, cliente));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Cliente não encontrado"));

        }

        // POST: api/Clientes
        public IHttpActionResult Post([FromBody]Cliente novoCliente)
        {
            string erro = ValidarCliente(novoCliente);

            if(erro != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            dc.Clientes.InsertOnSubmit(novoCliente);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/Clientes/5
        public IHttpActionResult Put(int id, [FromBody]Cliente clienteAlterado)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.Id == id);

            if (cliente == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,"Cliente não encontrado"));
            }

            string erro = ValidarCliente(clienteAlterado, id);

            if (erro != null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            cliente.Nome = clienteAlterado.Nome;
            cliente.Apelido = clienteAlterado.Apelido;
            cliente.Contacto = clienteAlterado.Contacto;
            cliente.NIF = clienteAlterado.NIF;
            cliente.Email = clienteAlterado.Email;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(
                    Request.CreateErrorResponse(
                        HttpStatusCode.ServiceUnavailable,
                        e));
            }

            return ResponseMessage(
                Request.CreateResponse(HttpStatusCode.OK));

        }

        // DELETE: api/Clientes/5
        public IHttpActionResult Delete(int id)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Cliente não encontrado"));
                }

            }

            Veiculo veiculo = dc.Veiculos.FirstOrDefault(v => v.IdCliente == id);

            if (veiculo != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Não é possivel apagar o cliente por que ele possui veiculo cadastrado")); 
            }


            dc.Clientes.DeleteOnSubmit(cliente);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }


        /// <summary>
        /// Valida os dados do veiculo 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="idIgnorar"></param>
        /// <returns></returns>
        private string ValidarCliente(Cliente cliente, int idIgnorar = 0)
        {
            Cliente clienteContacto = dc.Clientes.FirstOrDefault(c => c.Contacto == cliente.Contacto && c.Id != idIgnorar);

            if (clienteContacto != null)
            {
                return "Já existe um cliente com esse contacto.";
            }

            if (!string.IsNullOrEmpty(cliente.NIF))
            {
                Cliente clienteNif = dc.Clientes.FirstOrDefault(c => c.NIF == cliente.NIF && c.Id != idIgnorar);

                if (clienteNif != null)
                {
                    return "Já existe um cliente com esse NIF.";
                }
            }

            if (!string.IsNullOrEmpty(cliente.Email))
            {
                Cliente clienteEmail = dc.Clientes.FirstOrDefault(c => c.Email == cliente.Email && c.Id != idIgnorar);

                if (clienteEmail != null)
                {
                    return "Já existe um cliente com esse email.";
                }
            }

            return null;
        }
    }
}
