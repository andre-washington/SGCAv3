using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using SGCAv1.Models;
//using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using SGCAv1.HubConfig;

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private IHubContext<SgcaHub> _hub;
        SgcaContext db = new SgcaContext();
        public NotaController(IConfiguration configuration, IWebHostEnvironment env,  IHubContext<SgcaHub> hub)
        {
            _configuration = configuration;
            _env = env;
            _hub = hub;
        }

        //api method to get the entity details
        [HttpGet]
        public JsonResult Get()
        {
            IEnumerable<Notum> lista = from p in db.Nota select p;

            return new JsonResult(lista);
        }

        [HttpPost]
        public JsonResult Post(Notum nota)
        {
            db.Nota.Add(nota);
            db.SaveChanges();

            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            return new JsonResult("added successfully");
        }

        [HttpPut]
        public JsonResult Put(Notum nota)
        {
           var existingNota = db.Nota.Where(x => x.NotaId == nota.NotaId).FirstOrDefault();
            if (existingNota != null)
            {
                existingNota.NotaQuantidade = nota.NotaQuantidade;
                existingNota.NotaValor = nota.NotaValor;
                //existingNota.CaixaID = nota.CaixaID;
                db.SaveChanges();
            }

            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            
            return new JsonResult("Updated successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            db.Entry(new Notum { NotaId = id }).State =  Microsoft.EntityFrameworkCore.EntityState.Deleted;
            db.SaveChanges();
            return new JsonResult("Deleted successfully");
        }
    }
}
