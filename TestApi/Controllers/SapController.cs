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

namespace TestApi.Controllers
{
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
            SAPbobsCOM.Company company = conncetion.OpenConnection();

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
            SAPbobsCOM.Company company = conncetion.OpenConnection();

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
        [HttpGet]
        [Route("api/getItems")]
        public HttpResponseMessage getItems()
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=SBO_IDEACODEX;uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select ItemName, ItemCode from OITM";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

                                //MOSTRAR PRECIOS
        [HttpGet]
        [Route("api/getPrice")]
        public HttpResponseMessage getPrice()
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=SBO_IDEACODEX;uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select Price, ItemCode, Discount from OSPP";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, ds);
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

        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }

    }
}