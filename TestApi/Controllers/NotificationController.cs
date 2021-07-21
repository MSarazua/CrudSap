using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using SAPbobsCOM;
using System.Linq;
using System.Data;
using System.Data.Odbc;
using System.Web.Http;

namespace TestApi.Controllers
{
    public class NotificationController : ApiController
    {
        [HttpPost]
        //Defino la ruta
        //Recordatorio de pago
        [Route("api/RecordatorioPago")]
        public IHttpActionResult RecordatorioPago([FromBody] RequestPendientes requestPendientes)
        {
            //requestPendientes.listDatabases.Split(',').ToList<string>();
            //return Ok(listDatabases.Split(',').ToList<string>());
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    //El where se debe aplicar sí y solo sí, en Sap los clientes tienen el campo personalizado de notificación. 1 = Si, 0 = No
                    string query = "Select a.CardCode, a.CardName, b.E_MailL, a.Balance, db_name() as databases from SBODemoGT.dbo.OCRD a inner join SBODemoGT.dbo.OCPR b on a.CardCode = b.CardCode where b.U_Notificacion_cobro = 1  and a.Balance > 0";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/DetalleNotificacion")]
        public IHttpActionResult DetalleNotificacion([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.DocNum as NoDocumento, a.DocDate as fecha, a.DocDueDate as  fechaVencimiento, a.CardCode as codigo, a.CardName as nombre, a.DocTotal as total, a.PaidSum as pagado, a.doctotal - a.PaidSum as Saldo, b.SlpName as vendedor, DateDiff(day,a.DocDueDate,getdate()) as dias, Case  when DateDiff(day,a.DocDueDate,getdate()) between 0 and 30 then a.doctotal - a.PaidSum else 0 end as CeroTreinta, Case  when DateDiff(day,a.DocDueDate,getdate()) between 31 and 60 then a.doctotal - a.PaidSum else 0 end as TreintaUnoSesenta, Case  when DateDiff(day,a.DocDueDate,getdate()) between 61 and 90 then a.doctotal - a.PaidSum else 0 end as SesentaUnoNoventa, Case  when DateDiff(day,a.DocDueDate,getdate()) > 90 then a.doctotal - a.PaidSum else 0 end as Mayor90 from OINV a inner join OSLP b on a.SlpCode = b.SlpCode where a.doctotal - a.PaidSum  <>0 and a.DocStatus = 'O' and a.CardCode = '" + requestPendientes.CardCode + "'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        [HttpPost]
        [Route("api/DetalleVentas")]
        public IHttpActionResult DetalleVentas([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.DocDate, a.DocDueDate, a.DocNum, a.CardCode, a.CardName, a.DocTotal, db_name() as databases from OINV a where a.DocDate between '" + requestPendientes.fecha2 + "' and '" + requestPendientes.fecha1 + "' and a.CardCode = '" + requestPendientes.CardCode + "'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        public class RequestPendientes
        {
            public string listDatabases { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public string CardCode { get; set; }
            public string fecha1 { get; set; }
            public string fecha2 { get; set; }
        }
    }
}