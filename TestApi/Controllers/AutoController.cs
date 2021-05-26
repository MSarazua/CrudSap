using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Odbc;
using TestApi.Utils;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using SAPbobsCOM;

namespace TestApi.Controllers
{
    public class AutoController : ApiController
    {
        #region Variables
        //Instancia de la clase que devolverá la respuesta
        CallResponse responseCall = new CallResponse();
        #endregion

        // GET api/<controller>/5
        [HttpGet]
        //Defino la ruta
        [Route("api/Pendientes")]
        public HttpResponseMessage getAutoPendientes()
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=SBO_IDEACODEX;uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select WddCode, Status, UserSign, Remarks from OWDD where Status = 'W'";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        //EDITAR AUTORIZACIONES
        //Función pública tipo respuesta HTTP
        [HttpPost]
        //Defino la ruta
        [Route("api/Autorizar")]
        public IHttpActionResult UpdateItem(Authorizations authorizations)
        { //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection();

            //ApprovalRequestsService approvalSrv = company.GetCompanyService(ServiceTypes.ApprovalRequestsService) as ApprovalRequestsService;

            SAPbobsCOM.CompanyService oCompanyService = company.GetCompanyService();
            SAPbobsCOM.ApprovalRequestsService approvalSrv = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ApprovalRequestsService);
            //SAPbobsCOM.HolidayService oHolidayService = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.HolidayService)
            //SAPbobsCOM.Holiday oHoliday = oHolidayService.GetDataInterface(SAPbobsCOM.HolidayServiceDataInterfaces.hsHoliday);
            ApprovalRequestParams oParams = approvalSrv.GetDataInterface(ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams) as ApprovalRequestParams;
            oParams.Code = authorizations.WddCode;
            ApprovalRequest oData = approvalSrv.GetApprovalRequest(oParams);

            //Agregar una autorización
            oData.ApprovalRequestDecisions.Add();
            oData.ApprovalRequestDecisions.Item(0).ApproverUserName = "manager";
            oData.ApprovalRequestDecisions.Item(0).ApproverPassword = "Soporte@21";
            oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardApproved;
            oData.ApprovalRequestDecisions.Item(0).Remarks = "Autorizado desde el api";

            //Actualizar la autorización 
            approvalSrv.UpdateRequest(oData);
            return Ok(responseCall);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        public class Authorizations
        {
            public int WddCode { get; set;  }

        }

        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }
    }
}