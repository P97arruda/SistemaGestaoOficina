using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SistemaGestaoOficina.Api.Controllers
{
    public class ReparacoesController : ApiController
    {
        SistemaOficinaDataContextDataContext dc = new SistemaOficinaDataContextDataContext("workstation id=SistemaOficina.mssql.somee.com;packet size=4096;user" +
           " id=pliniodev97_SQLLogin_1;pwd=cnzppis3ut;data source=SistemaOficina.mssql.somee.com;persist security " +
           "info=False;initial catalog=SistemaOficina;TrustServerCertificate=True");


        // GET: api/Reparacoes
        public List<Reparacoe> Get()
        {
            var list = from Reparacoe in dc.Reparacoes select Reparacoe;
            return list.ToList();
        }

        // GET: api/Reparacoes/5
        public IHttpActionResult Get(int id)
        {
            Reparacoe reparacao = dc.Reparacoes.SingleOrDefault(x => x.Id == id);

            if(reparacao != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, reparacao));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Reparação não encontrada"));
        }

        // POST: api/Reparacoes
        public IHttpActionResult Post([FromBody]Reparacoe novaReparacao)
        {
            string erro = ValidaReparacao(novaReparacao);

            if (erro != null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            dc.Reparacoes.InsertOnSubmit(novaReparacao);

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

        // PUT: api/Reparacoes/5
        public IHttpActionResult Put(int id, [FromBody]Reparacoe reparacaoAlterada)
        {
            Reparacoe reparacao = dc.Reparacoes.FirstOrDefault(x => x.Id == id);

            if (reparacao == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Reparação não encontrada"));
            }

            if (reparacao.Concluida)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "A reparação concluída não pode ser alterada."));
            }


            string erro = ValidaReparacao(reparacaoAlterada, id);

            if (erro != null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            reparacao.IdMarcacao = reparacaoAlterada.IdMarcacao;
            reparacao.Pecas = reparacaoAlterada.Pecas;
            reparacao.CustoTotal = reparacaoAlterada.CustoTotal;
            reparacao.DataInicio = reparacaoAlterada.DataInicio;
            reparacao.DataFim = reparacaoAlterada.DataFim;
            reparacao.Concluida = reparacaoAlterada.Concluida;
            reparacao.TipoServico = reparacaoAlterada.TipoServico;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable,e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));

        }

        // DELETE: api/Reparacoes/5
        public IHttpActionResult Delete(int id)
        {
            Reparacoe reparacao = dc.Reparacoes.FirstOrDefault(x => x.Id == id);

            if (reparacao == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Reparação não encontrada"));
            }

            if (reparacao.Concluida)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Não é possível apagar uma reparação concluída."));
            }

            dc.Reparacoes.DeleteOnSubmit(reparacao);

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
        ///  Valida dados da reparação
        /// </summary>
        /// <param name="reparacao"></param>
        /// <param name="idIgnorar"></param>
        /// <returns></returns>
        private string ValidaReparacao(Reparacoe reparacao, int idIgnorar = 0)
        {
            Marcacoe marcacao = dc.Marcacoes.FirstOrDefault(m => m.Id == reparacao.IdMarcacao);

            if (marcacao == null)
            {
                return "Marcação não encontrada";
            }

            Reparacoe reparacaoExistente = dc.Reparacoes.FirstOrDefault(r =>
                r.IdMarcacao == reparacao.IdMarcacao &&
                r.Id != idIgnorar);

            if (reparacaoExistente != null)
            {
                return "Esta marcação já possui uma reparação";
            }

            return null;
        }
    }
}
