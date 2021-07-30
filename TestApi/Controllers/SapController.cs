using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web;
using System.Net;
using System.Data;
using System.Data.Odbc;
using System.Net.Http;
using System.Web.Http;
using TestApi.Utils;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using SAPbobsCOM;
using System.Web.Http.Cors;

namespace TestApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SapController : ApiController
    {
        #region Variables
        //Instancia de la clase que devolverá la respuesta
        CallResponse responseCall = new CallResponse();
        #endregion

        //CREAR ARTÍCULOS
        //Función pública tipo respuesta HTTP
        [HttpPost]
        //Defino la ruta
        [Route("api/CreateItem")]
        public IHttpActionResult CreateItem([FromBody] ItemDetails itemDetails)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(itemDetails.dbname, itemDetails.userSap, itemDetails.userSapPass);

            SAPbobsCOM.Items oItems;
            oItems = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            oItems.ItemCode = itemDetails.ItemCode;
            oItems.ItemName = itemDetails.ItemName;
            int status = oItems.Add();

            //Compruebo si el guardado se ha realizado correctamente
            if (status == 0)
            {
                responseCall.RespCode = "00";
                responseCall.Description = "Guardado correctamente";
            }
            else
            {
                responseCall.RespCode = "99";
                responseCall.Description = company.GetLastErrorDescription().ToString();

            }
            return Ok(responseCall);
        }

                                //EDITAR ARTPICULOS
        //Función pública tipo respuesta HTTP
        [HttpPost]
        //Defino la ruta
        [Route("api/UpdateItem")]
        public IHttpActionResult UpdateItem(ItemDetails itemDetails)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(itemDetails.dbname, itemDetails.userSap, itemDetails.userSapPass);

            SAPbobsCOM.Items oItems;
            oItems = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);

            if (oItems.GetByKey(itemDetails.ItemCode))
            {
                oItems.ItemCode = itemDetails.ItemCode;
                oItems.ItemName = itemDetails.ItemName;
                int status = oItems.Update();

                //Compruebo si el guardado se ha realizado correctamente
                if (status == 0)
                {
                    responseCall.RespCode = "00";
                    responseCall.Description = "Artículo modificado correctamente";
                }
                else
                {
                    responseCall.RespCode = "99";
                    responseCall.Description = company.GetLastErrorDescription().ToString();

                }
            }
            else
            {
                responseCall.RespCode = "90";
                responseCall.Description = "El artículo no se encuentra en SAP";

            }

            return Ok(responseCall);
        }

                            //MOSTRAR ARTÍCULOS
        [HttpPost]
        [Route("api/getItems")]
        public HttpResponseMessage getItems([FromBody] RequestArticulos requestArticulos)
        {
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select ItemName, ItemCode from OITM";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

                               //TRAER CLIENTES
        [HttpPost]
        [Route("api/getClientes")]
        public HttpResponseMessage getClientes([FromBody] RequestArticulos requestArticulos)
        {
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select * from ORDR";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

                                //CENTROS DE COSTO
        [HttpPost]
        [Route("api/CentrosCosto")]
        public HttpResponseMessage getCentrosCosto([FromBody] RequestArticulos requestArticulos)
        {
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select * from OPRC";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

                                //LLAMADA A MIS CAMPOS DE SAP
        public class ItemDetails
        {
            public string ItemName { get; set; }
            public string ItemCode { get; set; }
            public string BasePrice { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public string dbname { get; set; }

        }

        public class RequestArticulos
        {
            public string listDatabases { get; set; }
            public string server { get; set; }
        }

        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }

    }
}