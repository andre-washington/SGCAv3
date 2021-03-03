using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SGCAv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SGCAController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SGCAController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("getsignalstatus")]
        public JsonResult GetSignalStatus()
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
                using SqlCommand myCommand = new SqlCommand(query, myCon);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                myCon.Close();
            }
            
            List<Dictionary<string, object>> caixas;
            List<List<Dictionary<string, object>>> lista = new List<List<Dictionary<string, object>>>();
            Dictionary<string, object> info;

            var caixaIds = table.AsEnumerable().GroupBy(r => r.Field<int>("CaixaId"));

            foreach (var group in caixaIds)
            {
                caixas =  new List<Dictionary<string, object>>();
                
                foreach (DataRow rr in group)
                {
                    info = new Dictionary<string, object>();
                    info.Add("CaixaId", rr.Field <int>( "CaixaId"));
                    info.Add("CaixaQtdCritica", rr.Field<int>("CaixaQtdCritica"));
                    info.Add("CaixaSituacao", rr.Field<string>("CaixaSituacao"));
                    info.Add("NotaQuantidade", rr.Field<int?>("NotaQuantidade") !=null? rr.Field<int>("NotaQuantidade") : 0);
                    info.Add("NotaValor", rr.Field<int?>("NotaValor")!= null? rr.Field<int>("NotaValor") : 0);
                    caixas.Add(info);
                }
                lista.Add( caixas);   
            }
            return new JsonResult(lista);
        }
    }
}
