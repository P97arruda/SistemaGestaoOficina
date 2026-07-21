using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SistemaGestaoOficina.Api.Controllers
{
    public class VeiculosController : ApiController
    {
        SistemaOficinaDataContextDataContext dc = new SistemaOficinaDataContextDataContext("workstation id=SistemaOficina.mssql.somee.com;packet size=4096;user" +
            " id=pliniodev97_SQLLogin_1;pwd=cnzppis3ut;data source=SistemaOficina.mssql.somee.com;persist security " +
            "info=False;initial catalog=SistemaOficina;TrustServerCertificate=True");

        // GET: api/Veiculos
        public List<Veiculo> Get()
        {
            var list = from Veiculo in dc.Veiculos select Veiculo;
            return list.ToList();  
        }

        // GET: api/Veiculos/5
        public IHttpActionResult Get(int id)
        {
           Veiculo veiculo = dc.Veiculos.SingleOrDefault(v => v.Id == id);

            if (veiculo != null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, veiculo));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Veiculos não encontrado"));

        }

        // POST: api/Veiculos
        public IHttpActionResult Post([FromBody]Veiculo novoVeiculo)
        {
            string erro = ValidaVeiculo(novoVeiculo);

            if(erro != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            dc.Veiculos.InsertOnSubmit(novoVeiculo);

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

        // PUT: api/Veiculos/5
        public IHttpActionResult Put(int id, [FromBody]Veiculo veiculoAlterado)
        {
            Veiculo veiculo = dc.Veiculos.FirstOrDefault(v => v.Id == id);

            if (veiculo == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Veiculo não encontrado"));
            }

            string erro = ValidaVeiculo(veiculoAlterado, id);

            if( erro != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,erro));
            }

            veiculo.Matricula = veiculoAlterado.Matricula;
            veiculo.Marca = veiculoAlterado.Marca;
            veiculo.Modelo = veiculoAlterado.Modelo;
            veiculo.Ano = veiculoAlterado.Ano;
            veiculo.Quilometragem = veiculoAlterado.Quilometragem;
            veiculo.Combustivel = veiculoAlterado.Combustivel;
            veiculo.IdCliente = veiculoAlterado.IdCliente;

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

        // DELETE: api/Veiculos/5
        public IHttpActionResult Delete(int id)
        {
           Veiculo veiculo = dc.Veiculos.FirstOrDefault(x => x.Id == id);

            if (veiculo == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Veículo não encontrado"));
            }

            Marcacoe marcacao = dc.Marcacoes.FirstOrDefault(m => m.IdVeiculo == id);

            if (marcacao != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,"O veículo possui marcações."));
            }

            dc.Veiculos.DeleteOnSubmit(veiculo);

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
        /// Valida os dados dos veículos
        /// </summary>
        /// <param name="veiculo"></param>
        /// <param name="IdIgnorado"></param>
        /// <returns></returns>
        private string ValidaVeiculo(Veiculo veiculo, int idIgnorar = 0)
        {
            Veiculo veiculoMatricula = dc.Veiculos.FirstOrDefault(v => v.Matricula == veiculo.Matricula && v.Id != idIgnorar);

            if (veiculoMatricula != null) 
            {
                return "Já existe um veículo com essa matrícula.";
            }

            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.Id == veiculo.IdCliente);

            if(cliente == null)
            {
                return "Cliente não encontrado";
            }

            return null;
        }
    }
}
