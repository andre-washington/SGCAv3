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
        public JsonResult Post(Nota nota)
        {
            string query = @"insert into dbo.Nota (NotaQuantidade, NotaValor, CaixaId) values (" + nota.NotaQuantidade
                + @", " + nota.NotaValor + @", "+ nota.CaixaID + @")";
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
        public JsonResult Put(Nota nota)
        {
            string query = @"update dbo.Nota set NotaQuantidade = " + nota.NotaQuantidade +
                @", NotaValor = " + nota.NotaValor +
                @", CaixaID = "+ nota.CaixaID +
                @" where Notaid = " + nota.NotaID;
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
