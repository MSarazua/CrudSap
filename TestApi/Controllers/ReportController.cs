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
                                    //Gráficas por empresa
        [HttpPost]
        [Route("api/QuantityAutho")]
        public IHttpActionResult QuantityAutho([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select * from (select count(a.DocEntry), sum(a.DocTotalSy) as DocTotal_MS from ODRF a left join OWDD b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' ) as b ( cantidad, monto)";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CompanyAutho")]
        public IHttpActionResult CompanyAutho([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select x.compnyName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode,OADM x where b.Status = 'W' Group By x.CompnyName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CategoryAutho")]
        public IHttpActionResult CategoryAutho([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select f.AcctName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry,OADM x where b.Status = 'W' Group By f.AcctName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        [Route("api/ProveedorAutho")]
        public IHttpActionResult ProveedorAutho([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.CardName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' Group By a.CardName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }

            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CompradorAutho")]
        public IHttpActionResult CompradorAutho([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select n.SlpName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode,OADM x where b.Status = 'W' Group By n.SlpName";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

                                    //Gráfica por varias empresas
        [HttpPost]
        [Route("api/QuantityAuthos")]
        public IHttpActionResult QuantityAuthos([FromBody] RequestPendientes requestPendientes)
        {
            requestPendientes.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select * from (select count(a.DocEntry), sum(a.DocTotalSy) as DocTotal_MS from ODRF a left join OWDD b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' ) as b ( cantidad, monto)";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CompanyAuthos")]
        public IHttpActionResult CompanyAuthos([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select x.compnyName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode,OADM x where b.Status = 'W' Group By x.CompnyName";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CategoryAuthos")]
        public IHttpActionResult CategoryAuthos([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select f.AcctName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry,OADM x where b.Status = 'W' Group By f.AcctName";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/ProveedorAuthos")]
        public IHttpActionResult ProveedorAuthos([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.CardName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join WDD1 o on b.WddCode = o.WddCode ,OADM x where b.Status = 'W' Group By a.CardName";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/CompradorAuthos")]
        public IHttpActionResult CompradorAuthos([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPendientes.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select n.SlpName, sum(a.DocTotalSy) as Doctotal_MS from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join WDD1 o on b.WddCode = o.WddCode left join OSLP n on n.SlpCode = a.SlpCode,OADM x where b.Status = 'W' Group By n.SlpName";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/EstadoResultados")]
        public IHttpActionResult EstadoResultados([FromBody] RequestEstadoResultados requestEstadoResultados)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

               using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestEstadoResultados.server + ";Database=CRM_MODUS;uid=sa;pwd=Soporte@2021"))
                {
                    string query = "SELECT * FROM [dbo].[fnEstadoResultado] ('" + requestEstadoResultados.fechaIni + "','" + requestEstadoResultados.fechaFin + "','" + requestEstadoResultados.fechaInicial + "','" + requestEstadoResultados.fechaFinal + "', '" + requestEstadoResultados.Nivel + "') GO";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                } 
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/BalanceGeneral")]
        public IHttpActionResult BalanceGeneral([FromBody] RequestEstadoResultados requestEstadoResultados)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;    
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestEstadoResultados.server + ";Database=" + requestEstadoResultados.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "SELECT * FROM [dbo].[fnBalanceGeneral] ('" + requestEstadoResultados.fechaIni + "','" + requestEstadoResultados.fechaFin + "','" + requestEstadoResultados.fechaInicial + "','" + requestEstadoResultados.fechaFinal + "', '" + requestEstadoResultados.Nivel + "') GO";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds); 
        }

        [HttpPost]
        [Route("api/Inventario")]
        public IHttpActionResult Inventario([FromBody] RequestEstadoResultados requestEstadoResultados)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestEstadoResultados.server + ";Database=DATA;uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.ItemCode as CodigoArtículo, a.ItemName as Descripcion, a.OnHand as CantidadDisponible, b.PriceList as NoLista, b.Price as Precio, c.ListName as NombreLista, sum(	case when d.WhsCode = '01' Then d.OnHand else 0 end ) as BodegaCentral, sum(	case when d.WhsCode = '02' Then d.OnHand else 0 end) as BodegaQuetzaltenango, sum(	case when d.WhsCode = '03' Then d.OnHand else 0 end) as BodegaHuehuetenango from [Tecno].[dbo].[OITM] a left join [Tecno].[dbo].[ITM1] b on a.ItemCode = b.ItemCode left join [Tecno].[dbo].[OPLN] c on b.PriceList = c.ListNum left join [Tecno].[dbo].[OITW] d on d.ItemCode = a.ItemCode left join [Tecno].[dbo].[OWHS] e on e.WhsCode = d.WhsCode where b.PriceList = 1 and d.WhsCode in ('01', '02', '03') Group By a.ItemCode, a.ItemName, a.OnHand, b.PriceList, b.Price, c.ListName";
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
            public string server { get; set; }
        }

        public class RequestEstadoResultados
        {
            public string listDatabases { get; set; }
            public string fechaIni { get; set; }
            public string fechaFin { get; set; }
            public string fechaInicial { get; set; }
            public string fechaFinal { get; set; }
            public string Nivel { get; set; }
            public string server { get; set; }
            public string codigo { get; set; }
            public string descripcion { get; set; }
        }
    }
}