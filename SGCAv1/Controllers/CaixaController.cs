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
            JsonResult result ;
            if (caixasaque.valorSaque < 0 || caixasaque.valorSaque > 10000)
                return new JsonResult("Não permitido.");
            if (getSaldoSuficiente(caixasaque) < 0)
                return new JsonResult("Não há saldo suficiente");

            List<Nota> totalNotas = sacaNotas(caixasaque);
            if (totalNotas.Count() != 0)
            {
                result = atualizaNotas(caixasaque, totalNotas);
                
            }
            else
            {
                result = new JsonResult("Notas insuficientes");
                
            }
                


            //result = new JsonResult("saque ok!");
            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());//sinaliza o sgca
            //_hub.Clients.All.SendAsync("caixaativoevent", this.getCaixaAtivo());
            _hub.Clients.All.SendAsync("caixaativoevent", result);

            return new JsonResult(result);
        }

        private JsonResult atualizaNotas(CaixaSaque caixasaque, List<Nota> totalNotas)
        {
            var existingCaixa = db.Caixas.Where(x => x.CaixaId == caixasaque.Caixa.CaixaId).FirstOrDefault();
            var existingNota = db.Nota.Where(x => x.CaixaId == caixasaque.Caixa.CaixaId);

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
                ).Where(a => a.CaixaId == caixasaque.Caixa.CaixaId);

            for (int i = 0; i < totalNotas.Count ; i++) {
                if ( data.Where(x => x.NotaValor == totalNotas[i].NotaValor && x.NotaQuantidade >= totalNotas[i].NotaQuantidade) !=null )
                {
                    existingNota.Where(x => x.NotaValor == totalNotas[i].NotaValor).FirstOrDefault().NotaQuantidade -= totalNotas[i].NotaQuantidade;
                    db.SaveChanges();
                }
             }
            return new JsonResult("saque realizado com sucesso.");
        }
        //TODO colocar na tela as notas disponiveis . !!
        private List<Nota> sacaNotas(CaixaSaque caixaSaque)
        {
            List<Nota> totalNotas = new List<Nota>();
            Nota nota;
            int qtdEmCaixa = 0;
            var caixa  = db.Caixas.Where(x => x.CaixaId == caixaSaque.Caixa.CaixaId).FirstOrDefault(); 
            int valor = caixaSaque.valorSaque;
            int[] notasDisponiveis = db.Nota.Where(x => x.CaixaId == caixaSaque.Caixa.CaixaId && x.NotaQuantidade >0).OrderBy(x=>x.NotaValor).Select(x => x.NotaValor).ToArray<int>();
            
            //if (notasDisponiveis.Sum() < caixa.valorSaque)
                //return totalNotas;

            int i;
            for (i = 0; i < notasDisponiveis.Count(); i++)
            {
                if (notasDisponiveis[i] > caixaSaque.valorSaque )
                    break;
            }
            
            i--;
            while ( i>=0 )
            {
                nota = new Nota();
                nota.NotaValor = notasDisponiveis[i];
                nota.NotaQuantidade = valor / notasDisponiveis[i];
                qtdEmCaixa = db.Nota.Where(x => x.CaixaId == caixaSaque.Caixa.CaixaId && x.NotaValor == nota.NotaValor).FirstOrDefault().NotaQuantidade;


                if (nota.NotaQuantidade > qtdEmCaixa || (qtdEmCaixa - nota.NotaQuantidade) < caixa.CaixaQtdCritica)
                {
                    i--;
                    continue;
                }

                valor -= (nota.NotaValor * nota.NotaQuantidade);
                if (nota.NotaQuantidade > 0)
                {
                    
                    totalNotas.Add(nota);
                }
                    
                if (valor % notasDisponiveis[i]  == 0)
                    break;
                i--;
            }
            if (valor > 0)
            {
                caixa.CaixaSituacao = "Inativo";
                db.SaveChanges();
                totalNotas.Clear();//notas disponiveis não podem fechar o valor pedido
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

    }
}
