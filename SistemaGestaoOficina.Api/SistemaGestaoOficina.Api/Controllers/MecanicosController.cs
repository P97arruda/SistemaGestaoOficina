using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SistemaGestaoOficina.Api.Controllers
{
    public class MecanicosController : ApiController
    {
        SistemaOficinaDataContextDataContext dc = new SistemaOficinaDataContextDataContext("workstation id=SistemaOficina.mssql.somee.com;packet size=4096;user" +
            " id=pliniodev97_SQLLogin_1;pwd=cnzppis3ut;data source=SistemaOficina.mssql.somee.com;persist security " +
            "info=False;initial catalog=SistemaOficina;TrustServerCertificate=True");

        // GET: api/Mecanicos
        public List<Mecanico> Get()
        {
            var list = from Mecanico in dc.Mecanicos select Mecanico;
            return list.ToList();
        }

        // GET: api/Mecanicos/5
        public IHttpActionResult Get(int id)
        {
            Mecanico mecanico = dc.Mecanicos.SingleOrDefault(m => m.Id == id);

            if(mecanico != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, mecanico));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Mecanico não encontrado"));
        }

        // POST: api/Mecanicos
        public IHttpActionResult Post([FromBody]Mecanico novoMecanico)
        {
            string erro = ValidaMecanico(novoMecanico);

            if (erro != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, erro));
            }

            dc.Mecanicos.InsertOnSubmit(novoMecanico);

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

        // PUT: api/Mecanicos/5
        public IHttpActionResult Put(int id, [FromBody]Mecanico mecanicoAlterado)
        {
            Mecanico mecanico = dc.Mecanicos.FirstOrDefault(m => m.Id == id);

            if (mecanico == null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Mecanico não foi encontrado"));
            }

            string erro = ValidaMecanico(mecanicoAlterado, id);

            if (erro != null) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,erro));
            }

            mecanico.Nome = mecanicoAlterado.Nome;
            mecanico.Especialidade = mecanicoAlterado.Especialidade;
            mecanico.Horario = mecanicoAlterado.Horario;
            mecanico.Contacto = mecanicoAlterado.Contacto;
            mecanico.Ativo = mecanicoAlterado.Ativo;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage (Request.CreateResponse(HttpStatusCode.OK));
        }

        // DELETE: api/Mecanicos/5
        public IHttpActionResult Delete(int id)
        {
            Mecanico mecanico = dc.Mecanicos.FirstOrDefault(m => m.Id == id);

            if (mecanico == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Mecânico não encontrado"));
            }

            Marcacoe marcacao = dc.Marcacoes.FirstOrDefault(m => m.IdMecanico == id);

            if (marcacao != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "O mecânico possui marcações."));
            }

            dc.Mecanicos.DeleteOnSubmit(mecanico);

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
        /// Valida os dados do mecânico.
        /// <param name="mecanico"></param>
        /// <param name="idIgnorar"></param>
        /// <returns></returns>
        private string ValidaMecanico(Mecanico mecanico, int idIgnorar = 0)
        {
            Mecanico mecanicoContacto = dc.Mecanicos.FirstOrDefault(m => m.Contacto == mecanico.Contacto && m.Id != idIgnorar);

            if (mecanicoContacto != null)
            {
                return "Já existe um mecânico com esse contacto.";
            }

            Mecanico mecanicoNome = dc.Mecanicos.FirstOrDefault(m => m.Nome == mecanico.Nome && m.Id != idIgnorar);

            if (mecanicoNome != null)
            {
                return "Já existe um mecânico com esse nome.";
            }

            Cliente clienteContacto = dc.Clientes.FirstOrDefault(c => c.Contacto == mecanico.Contacto);

            if (clienteContacto != null)
            {
                return "Já existe um cliente com esse contacto.";
            }


            return null;
        } 
    }
}
