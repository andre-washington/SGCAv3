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

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaixaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CaixaController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        //[HttpGet("{id}")]
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
