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
using SGCAv1.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaixaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private IHubContext<SgcaHub> _hub;
        SgcaContext db = new SgcaContext();
        public CaixaController(IConfiguration configuration, IHubContext<SgcaHub> hub, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            _hub = hub;
        }

        //api method to get the entity details
        [HttpGet]
        public JsonResult Get()
        {
            IEnumerable<Caixa> lista = from p in db.Caixas select p;

            return new JsonResult(lista);
        }

        [HttpPost]
        public JsonResult Post(Caixa caixa)
        {
            db.Caixas.Add(caixa);
            db.SaveChanges();
                        
            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            
            return new JsonResult("added successfully");
        }

        [HttpPut]
        public JsonResult Put(Caixa caixa)
        {
            var existingCaixa = db.Caixas.Where(x => x.CaixaId == caixa.CaixaId).FirstOrDefault();
                if (existingCaixa != null)
                {
                    existingCaixa.CaixaQtdCritica = caixa.CaixaQtdCritica;
                    existingCaixa.CaixaSituacao = caixa.CaixaSituacao;
                    db.SaveChanges();
                }            

            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            
            return new JsonResult("Updated successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            db.Entry(new Caixa { CaixaId = id }).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            db.SaveChanges();
            return new JsonResult("Deleted successfully");
        }
        
        [Route("{id}/GetStatus")]
        //[HttpGet("{id}")]
        public JsonResult GetStatus(int id)
        {
            var data = db.Caixas.Join(
                db.Nota,
                caixa => caixa.CaixaId,
                nota => nota.CaixaId,
                (caixa, nota) => new
                {
                    CaixaId = caixa.CaixaId,
                    NotaId = nota.NotaId,
                    CaixaQtdCritica = caixa.CaixaQtdCritica,
                    CaixaSituacao = caixa.CaixaSituacao,
                    NotaQuantidade = nota.NotaQuantidade,
                    NotaValor = nota.NotaValor
                }
                ).Where(a => a.CaixaId == id);
           
            return new JsonResult(data);
        }
        
        [Route("getstatus")]
        
        public JsonResult GetStatus()
        {
            var data = db.Caixas.Join(
                db.Nota,
                caixa => caixa.CaixaId,
                nota => nota.CaixaId,
                (caixa, nota) => new
                {
                    CaixaId = caixa.CaixaId,
                    NotaId = nota.NotaId,
                    CaixaQtdCritica = caixa.CaixaQtdCritica,
                    CaixaSituacao = caixa.CaixaSituacao,
                    NotaQuantidade = nota.NotaQuantidade,
                    NotaValor = nota.NotaValor
                }
                );

            return new JsonResult(data);
        }

        [Route("saque")]
        public JsonResult saque(CaixaSaque caixasaque) {
            if (caixasaque.valorSaque < 0 || caixasaque.valorSaque > 10000)
                return new JsonResult(" Não permitido.");
            if (getSaldoSuficiente(caixasaque) < 0)
                return new JsonResult("Não há saldo suficiente");

            List<Nota> totalNotas = sacaNotas(caixasaque); 
            atualizaNotas(totalNotas); //fazer

            //DataTable table = getDataTable(@"select CaixaId from dbo.Caixa where CaixaSituacao like '%ok%' ");
                        
            //table = getDataTable(@"select n.NotaQuantidade as quantidade from nota n where caixaId ="+
                //caixasaque.Caixa.CaixaId + @" and n.NotaValor = 2");

            //calcNotas(caixa.valorSaque); 

            _hub.Clients.All.SendAsync("caixaativoevent", this.getCaixaAtivo());
            return new JsonResult(totalNotas);
        }

        private void atualizaNotas(List<Nota> totalNotas)
        {
            throw new NotImplementedException();
        }

        private List<Nota> sacaNotas(CaixaSaque caixa)
        {
            int valor = caixa.valorSaque;
            int[] notas = { 2, 5, 10, 20, 50};
            List<Nota> totalNotas = new List<Nota>();
            int i;
            for (i = 0; i<=4 ; i++)
            {
                if (notas[i] > caixa.valorSaque )
                    break;
            }
            Nota n;
            i--;
            while ( i>=0 )
            {
                n = new Nota();
                n.NotaValor = notas[i];
                n.NotaQuantidade = valor / notas[i];
                valor -= (n.NotaValor * n.NotaQuantidade);
                if (n.NotaQuantidade>0)
                    totalNotas.Add(n);
                if (valor % notas[i]  == 0)
                    break;
                i--;
            }
            return totalNotas;
        }

        private int getSaldoSuficiente(CaixaSaque caixa)
        {
            var result = db.Nota.Where(t => t.CaixaId == caixa.Caixa.CaixaId).Sum(i => (i.NotaQuantidade * i.NotaValor));
            int saldo = result != null ? result - caixa.valorSaque : -1;
            return saldo;
        }

        [Route("getcaixaativo")]
        public JsonResult getCaixaAtivo()
        {
            IEnumerable<Caixa> lista = from p in db.Caixas where p.CaixaSituacao.Equals("Ativo") select p ;

            return new JsonResult(lista);
            
        }

        private DataTable getDataTable(string query)
        {
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SGCACon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return table;
        }
    }
}
