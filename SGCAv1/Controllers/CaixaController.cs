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

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaixaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IHubContext<SgcaHub> _hub;
        public CaixaController(IConfiguration configuration, IHubContext<SgcaHub> hub)
        {
            _configuration = configuration;
            _hub = hub;
        }

        //api method to get the entity details
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select CaixaId, CaixaQtdCritica, CaixaSituacao from dbo.Caixa";
            DataTable table = getDataTable(query);
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Caixa caixa)
        {
            string query = @"insert into dbo.Caixa (CaixaQtdCritica, CaixaSituacao) values (" + caixa.CaixaQtdCritica
                + @",'" + caixa.CaixaSituacao + @"')";
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
            
           SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            
            
            return new JsonResult("added successfully");
        }

        [HttpPut]
        public JsonResult Put(Caixa caixa)
        {
            string query = @"update dbo.Caixa set CaixaQtdCritica = " + caixa.CaixaQtdCritica +
                @", CaixaSituacao = '" + caixa.CaixaSituacao + @"' where Caixaid = " + caixa.CaixaID;
            DataTable table = getDataTable(query);

            SGCAController sgca = new SGCAController(_configuration);
            _hub.Clients.All.SendAsync("transfercaixadata", sgca.GetSignalStatus());
            
            return new JsonResult("Updated successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"delete from dbo.Caixa where CaixaId = " + id;
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
            return new JsonResult("Deleted successfully");
        }
        
        [Route("{id}/GetStatus")]
        //[HttpGet("{id}")]
        public JsonResult GetStatus(int id)
        {
            string query = @"select c.CaixaId, c.CaixaQtdCritica, c.CaixaSituacao,
            n.NotaQuantidade, n.NotaValor
            from dbo.Caixa c
                join dbo.Nota n on n.CaixaId = c.CaixaId where c.CaixaId = " + id;
            DataTable table = getDataTable(query);
            return new JsonResult(table );
        }
        
        [Route("getstatus")]
        
        public JsonResult GetStatus()
        {
            string query = @"select c.CaixaId, c.CaixaQtdCritica, c.CaixaSituacao,
            n.NotaQuantidade, n.NotaValor
            from dbo.Caixa c
                left join dbo.Nota n on n.CaixaId = c.CaixaId ";
            DataTable table = getDataTable(query);

            return new JsonResult(table);
        }

        [Route("saque")]
        public JsonResult saque(Caixa caixa) {
            if (caixa.valorSaque < 0 || caixa.valorSaque > 10000)
                return new JsonResult(" Não permitido.");
            if (getSaldoSuficiente(caixa) < 0)
                return new JsonResult("Não há saldo suficiente");

            List<Nota> totalNotas = sacaNotas(caixa);
            atualizaNotas(totalNotas);

            DataTable table = getDataTable(@"select CaixaId from dbo.Caixa where CaixaSituacao like '%ok%' ");
                        
            table = getDataTable(@"select n.NotaQuantidade as quantidade from nota n where caixaId ="+
                caixa.CaixaID + @" and n.NotaValor = 2");

            //calcNotas(caixa.valorSaque); 


            _hub.Clients.All.SendAsync("caixaativoevent", this.getCaixaAtivo());
            return new JsonResult(totalNotas);
        }

        private void atualizaNotas(List<Nota> totalNotas)
        {
            throw new NotImplementedException();
        }

        private List<Nota> sacaNotas(Caixa caixa)
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
            while ( i>=0)
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

        private int getSaldoSuficiente(Caixa caixa)
        {
            DataTable table = getDataTable(@"select sum(n.NotaValor* n.NotaQuantidade) as montante 
                    from nota n where n.CaixaId =" +
                    caixa.CaixaID );
            int saldo = table.Rows[0].Field<int?>("montante") !=null? 
                table.Rows[0].Field<int>("montante") - caixa.valorSaque
                : -1 ;
            
            return saldo;
        }

        [Route("getcaixaativo")]
        public JsonResult getCaixaAtivo()
        {
            string query = @"select CaixaId from dbo.Caixa where CaixaSituacao like '%ok%' ";
            DataTable table = getDataTable(query);

            return new JsonResult(table);
            
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
