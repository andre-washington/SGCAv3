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
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            foreach (DataRow dr in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            List<Dictionary<string, object>> caixas;
            List<List<Dictionary<string, object>>> lista = new List<List<Dictionary<string, object>>>();
            Dictionary<string, object> info;

            var caixaIds = table.AsEnumerable().GroupBy(r => r.Field<int>("CaixaId"));

            foreach (var group in caixaIds)
            {
                
                caixas =  new List<Dictionary<string, object>>();
                //Console.WriteLine("Caixa ID: {0}", group.Key);
                //Console.WriteLine("CaixaQtdCritica   CaixaSituacao  NotaQuantidade  NotaValor");
                //int rowNum = 0;
                foreach (DataRow rr in group)
                {
                    info = new Dictionary<string, object>();
                    info.Add("CaixaId", rr.Field <int>( "CaixaId"));
                    info.Add("CaixaQtdCritica", rr.Field<int>("CaixaQtdCritica"));
                    info.Add("CaixaSituacao", rr.Field<string>("CaixaSituacao"));
                    info.Add("NotaQuantidade", rr.Field<int>("NotaQuantidade"));
                    info.Add("NotaValor", rr.Field<int>("NotaValor"));
                    //Console.WriteLine("{0}- {1}   {2} {3} {4}", ++rowNum, rr.Field<int>("CaixaQtdCritica")
                    //    , rr.Field<string>("CaixaSituacao")
                    //    , rr.Field<int>("NotaQuantidade")
                    //    , rr.Field<int>("NotaValor"));
                    caixas.Add(info);
                }
                lista.Add( caixas);
                
            }



            return new JsonResult(lista);
        }
    }
}
