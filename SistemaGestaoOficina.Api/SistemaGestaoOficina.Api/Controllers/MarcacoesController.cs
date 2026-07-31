using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SistemaGestaoOficina.Api.Controllers
{
    public class MarcacoesController : ApiController
    {

        SistemaOficinaDataContextDataContext dc = new SistemaOficinaDataContextDataContext("workstation id=SistemaOficina.mssql.somee.com;packet size=4096;user" +
            " id=pliniodev97_SQLLogin_1;pwd=cnzppis3ut;data source=SistemaOficina.mssql.somee.com;persist security " +
            "info=False;initial catalog=SistemaOficina;TrustServerCertificate=True");

        // GET: api/Marcacoes
        public List<Marcacoe> Get()
        {
            var list = from Marcacoe in dc.Marcacoes select Marcacoe;
            return list.ToList();
        }

        // GET: api/Marcacoes/5
        public IHttpActionResult Get(int id)
        {
            Marcacoe marcacao = dc.Marcacoes.SingleOrDefault(m => m.Id == id);

            if (marcacao != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,marcacao));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,"Marcação não encontrada"));
        }

        // POST: api/Marcacoes
        public IHttpActionResult Post([FromBody]Marcacoe novaMarcacao)
        {
            string erro = ValidaMarcacao(novaMarcacao);

            if (erro != null)
            {
                return ResponseMessage(
                    Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            Mecanico mecanico = dc.Mecanicos.FirstOrDefault(
                m => m.Id == novaMarcacao.IdMecanico);

            if (mecanico == null)
            {
                return ResponseMessage(
                    Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        "Mecânico não encontrado."));
            }

            if (mecanico.Ativo == false)
            {
                return ResponseMessage(
                    Request.CreateResponse(
                        HttpStatusCode.Conflict,
                        "Não é possível criar uma marcação para um mecânico desativado."));
            }

            dc.Marcacoes.InsertOnSubmit(novaMarcacao);

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

      

        // PUT: api/Marcacoes/5
        public IHttpActionResult Put(int id, [FromBody]Marcacoe marcacaoAlterada)
        {
            Marcacoe marcacao = dc.Marcacoes.FirstOrDefault(m => m.Id == id);

            if (marcacao == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Marcação não encontrada"));
            }

            if (marcacao.Estado != "Pendente")
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Apenas marcações pendentes podem ser alteradas."));
            }

            string erro = ValidaMarcacao(marcacaoAlterada, id);

            if (erro != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            marcacao.IdCliente = marcacaoAlterada.IdCliente;
            marcacao.IdVeiculo = marcacaoAlterada.IdVeiculo;
            marcacao.IdMecanico = marcacaoAlterada.IdMecanico;
            marcacao.TipoServico = marcacaoAlterada.TipoServico;
            marcacao.DataHora = marcacaoAlterada.DataHora;
            marcacao.Estado = marcacaoAlterada.Estado;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(
                Request.CreateResponse(HttpStatusCode.OK));

        }

        // DELETE: api/Marcacoes/5
        public IHttpActionResult Delete(int id)
        {
            Marcacoe marcacao = dc.Marcacoes.FirstOrDefault(m => m.Id == id);

            if (marcacao == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,"Marcação não encontrada"));
            }

            if (marcacao.Estado != "Pendente")
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,"Apenas marcações pendentes podem ser apagadas."));
            }

            Reparacoe reparacao = dc.Reparacoes.FirstOrDefault(r => r.IdMarcacao == id);

            if (reparacao != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,"A marcação possui uma reparação."));
            }

            dc.Marcacoes.DeleteOnSubmit(marcacao);

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
        /// Valida dos da marcação 
        /// </summary>
        /// <param name="marcacao"></param>
        /// <param name="idIgnorar"></param>
        /// <returns></returns>
        private string ValidaMarcacao(Marcacoe marcacao, int idIgnorar = 0)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.Id == marcacao.IdCliente);

            if (cliente == null)
            {
                return "Cliente não encontrado.";
            }

            var veiculoComMarcacaoPendente = dc.Marcacoes.FirstOrDefault(m => m.IdVeiculo == marcacao.IdVeiculo && m.Estado == "Pendente" && m.Id != idIgnorar);

            if (veiculoComMarcacaoPendente != null)
            {
                return "Este veículo já possui uma marcação pendente.";
            }


            Veiculo veiculo = dc.Veiculos.FirstOrDefault(v => v.Id == marcacao.IdVeiculo);

            if (veiculo == null)
            {
                return "Veículo não encontrado.";
            }

            Mecanico mecanico = dc.Mecanicos.FirstOrDefault(m => m.Id == marcacao.IdMecanico);

            if (mecanico == null)
            {
                return "Mecânico não encontrado.";
            }

            if (veiculo.IdCliente != marcacao.IdCliente)
            {
                return "O veículo não pertence ao cliente informado.";
            }

            if (!mecanico.Ativo)
            {
                return "Não é possível atribuir uma marcação a um mecânico inativo.";
            }

            Marcacoe marcacaoExistente = dc.Marcacoes.FirstOrDefault(m => m.IdMecanico == marcacao.IdMecanico && m.DataHora == marcacao.DataHora &&
                m.Id != idIgnorar);

            if (marcacaoExistente != null)
            {
                return "Já existe uma marcação para esse mecânico nesta data e hora.";
            }

            return null;
        }
    }
}
