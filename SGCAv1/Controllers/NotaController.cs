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

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public NotaController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        //api method to get the entity details
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select NotaId, NotaQuantidade, NotaValor, CaixaId from dbo.Nota";
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
        public JsonResult Post(Nota Nota)
        {
            string query = @"insert into dbo.Nota (NotaQuantidade, NotaValor, CaixaId) values (" + Nota.NotaQuantidade
                + @", " + Nota.NotaValor + @", "+ Nota.CaixaID + @")";
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
        public JsonResult Put(Nota Nota)
        {
            string query = @"update dbo.Nota set NotaQuantidade = " + Nota.NotaQuantidade +
                @", NotaValor = " + Nota.NotaValor +
                @", CaixaID = "+ Nota.CaixaID +
                @" where Notaid = " + Nota.NotaID;
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
            string query = @"delete from dbo.Nota where NotaId = " + id;
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
    }
}
