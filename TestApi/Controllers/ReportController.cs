using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAPbobsCOM;

namespace TestApi.Controllers
{
    public class ReportController : ApiController
    {
        [HttpPost]
        //Defino la ruta
        //Autorizaciones pendientes y monto
        [Route("api/QuantityAutho")]
        public IHttpActionResult QuantityAutho([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select * from (select count(a.DocEntry), sum(a.DocTotalSy) as DocTotal_MS from ODRF a left join OWDD b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' ) as b ( cantidad, monto)";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        //Defino la ruta
        //Autorizaciones pendientes y monto
        [Route("api/CompanyAutho")]
        public IHttpActionResult CompanyAutho([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select x.compnyName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode,OADM x where b.Status = 'W' Group By x.CompnyName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        //Defino la ruta
        //Autorizaciones por Categoria  y Monto
        [Route("api/CategoryAutho")]
        public IHttpActionResult CategoryAutho([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select f.AcctName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry,OADM x where b.Status = 'W' Group By f.AcctName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        //Defino la ruta
        //Autorizaciones por proveedor  y Monto
        [Route("api/ProveedorAutho")]
        public IHttpActionResult ProveedorAutho([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.CardName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' Group By a.CardName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        //Defino la ruta
        //Autorizaciones por Comprador  y Monto
        [Route("api/CompradorAutho")]
        public IHttpActionResult CompradorAutho([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select n.SlpName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode,OADM x where b.Status = 'W' Group By n.SlpName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        public class RequestPendientes
        {
            public string listDatabases { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public int WddCode { get; set; }

        }
    }
}