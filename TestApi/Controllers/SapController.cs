using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using TestApi.Utils;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace TestApi.Controllers
{
    public class SapController : ApiController
    {
        //Instancia de la clase que devolverá la respuesta
        CallResponse responseCall = new CallResponse();
        //Función pública tipo respuesta HTTP
        [HttpPost]
        //Defino la ruta
        [Route("api/CreateItem")]
        public IHttpActionResult CreateItem(ItemDetails itemDetails)
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

       

        //public IHttpActionResult Ok(CallResponse callResponse)
        //{
        //  return Ok(callResponse);
        //}

        public class ItemDetails
        {
            public string ItemName { get; set; }
            public string ItemCode { get; set; }
            public string BasePrice { get; set; }

        }

        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }
    }
}