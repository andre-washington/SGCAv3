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
            
            //JsonResult signalStatus = GetSignalStatus();
            //_hub.Clients.All.SendAsync("transfercaixadata", signalStatus);
            return new JsonResult("added successfully");
        }

        [HttpPut]
        public JsonResult Put(Caixa caixa)
        {
            string query = @"update dbo.Caixa set CaixaQtdCritica = " + caixa.CaixaQtdCritica +
                @", CaixaSituacao = '" + caixa.CaixaSituacao + @"' where Caixaid = " + caixa.CaixaID;
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
            return new JsonResult(table );
        }
        [Route("GetStatus")]
        
        public JsonResult GetStatus()
        {
            string query = @"select c.CaixaId, c.CaixaQtdCritica, c.CaixaSituacao,
            n.NotaQuantidade, n.NotaValor
            from dbo.Caixa c
                left join dbo.Nota n on n.CaixaId = c.CaixaId ";
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
            
            return new JsonResult(table);
        }


        
    }
}
