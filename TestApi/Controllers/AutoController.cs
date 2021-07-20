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
using System.Web.Http.Cors;

namespace TestApi.Controllers
{
    [EnableCors(origins: "*", headers:"*", methods:"*")]
    public class AutoController : ApiController
    {
        #region Variables
        //Instancia de la clase que devolverá la respuesta
        CallResponse responseCall = new CallResponse();
        #endregion
        [HttpPost]
        //Pendientes
        [Route("api/Pendiente")]
        public IHttpActionResult getAutoPendiente([FromBody] RequestPendientes requestPendientes)
        {
            requestPendientes.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, b.WddCode, b.Remarks, a.DocTotal, b.status, x.compnyName, c.Name, m.USER_CODE, a.DocCur, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join OUSR m on m.USERID = b.UserSign  left join OSLP n on n.SlpCode = a.SlpCode, OADM x where b.Status = 'W'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        //Autorizar uno
        [HttpPost]
        [Route("api/Autorizar")]
        public IHttpActionResult UpdateItem(Authorizations authorizations)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection();

            SAPbobsCOM.CompanyService oCompanyService = company.GetCompanyService();
            SAPbobsCOM.ApprovalRequestsService approvalSrv = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ApprovalRequestsService);
            ApprovalRequestParams oParams = approvalSrv.GetDataInterface(ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams) as ApprovalRequestParams;
            oParams.Code = authorizations.WddCode;
            ApprovalRequest oData = approvalSrv.GetApprovalRequest(oParams);

            //Agregar una autorización
            oData.ApprovalRequestDecisions.Add();
            oData.ApprovalRequestDecisions.Item(0).ApproverUserName = "manager";
            oData.ApprovalRequestDecisions.Item(0).ApproverPassword = "Soporte@20";
            oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardApproved;
            oData.ApprovalRequestDecisions.Item(0).Remarks = authorizations.Remarks;

            //Actualizar la autorización 
            approvalSrv.UpdateRequest(oData);
            return Ok(responseCall);
        }

        // Pendientes
        [HttpPost]
        [Route("api/Pendientes")]
        public IHttpActionResult getAutoPendientes([FromBody] RequestPendientes requestPendientes)
        {
            requestPendientes.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal, a.DocTotalSy as Doctotal_MS, b.status, c.Name, x.compnyName, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, p.FileName, p.FileExt, p.Line, p.srcPath, p.trgtPath, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry, OADM x where b.Status = 'W'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        // Documentos autorizados
        [HttpPost]
        [Route("api/DocumentosAutorizados")]
        public IHttpActionResult getDocumentosAutorizados([FromBody] RequestPendientes requestPendientes)
        {
            requestPendientes.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal, a.DocTotalSy as Doctotal_MS, b.status, c.Name, x.compnyName, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, p.FileName, p.FileExt, p.Line, p.srcPath, p.trgtPath, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry, OADM x where b.Status = 'Y'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        // Documentos autorizados
        [HttpPost]
        [Route("api/DocumentosRechazados")]
        public IHttpActionResult getDocumentosRechazados([FromBody] RequestPendientes requestPendientes)
        {
            requestPendientes.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestPendientes.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + item + ";uid=sa;pwd=Soporte@2021"))
                {
                    string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal, a.DocTotalSy as Doctotal_MS, b.status, c.Name, x.compnyName, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, p.FileName, p.FileExt, p.Line, p.srcPath, p.trgtPath, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry, OADM x where b.Status = 'N'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Ok(ds);
        }

        // Detalle de Autorización
        [HttpPost]
        [Route("api/detalleUnaAuto")]
        public IHttpActionResult getDetalleUnaAuto([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;
            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal, a.DocTotalSy as Doctotal_MS, b.status, c.Name, x.compnyName, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, f.acctcode, q.WTCode, q.WTAmnt, a.VatSum, a.VatSumSy, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join ATC1 p on p.absentry = a.AtcEntry left join DRF5 q on p.AbsEntry = a.DocEntry,OADM x where b.Status = 'W' and  b.WddCode=" + requestPendientes.WddCode + "";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        // Archivos
        [HttpPost]
        [Route("api/archivos")]
        public IHttpActionResult getArchivos([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;
            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select p.FileName, p.FileExt, p.srcPath, p.trgtPath from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join ATC1 p on p.absentry = a.AtcEntry,OADM x where b.Status = 'W' and  b.WddCode=" + requestPendientes.WddCode + "";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        //Cuentas
        [HttpPost]
        [Route("api/cuentas")]
        public IHttpActionResult getCuentas([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;
            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.Segment_0 +' ' + a.AcctName as NombreCuenta, b.AcctCode +' ' + b.AcctName, c.AcctCode +' ' + c.AcctName, d.AcctCode +' ' + d.AcctName, convert(varchar(1),e.GroupMask) +' ' + e.AcctName from OACT a left join OACT b on a.FatherNum = b.AcctCode left join OACT c on b.FatherNum = c.AcctCode left join OACT d on c.FatherNum = d.AcctCode left join OACT e on d.FatherNum = e.AcctCode where a.Segment_0 = '" + requestPendientes.Segment_0 + "'";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        //Documentos rechazados impuestos
        [HttpPost]
        [Route("api/retencion")]
        public IHttpActionResult getRetencion([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal as Doctotal_ML, a.DocTotalSy as Doctotal_MS, b.status, x.compnyName, c.Name, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, p.WTCode, p.WTAmnt, a.VatSum, a.VatSumSy, f.acctcode, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join DRF5 p on p.AbsEntry = a.DocEntry,OADM x where  b.Status = 'N' and  b.WddCode=" + requestPendientes.WddCode + "";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        //Documentos autorizados impuestos
        [HttpPost]
        [Route("api/retencionAutorizados")]
        public IHttpActionResult getRetencionAutorizados([FromBody] RequestPendientes requestPendientes)
        {
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=" + requestPendientes.listDatabases + ";uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select a.docentry, a.DocNum, a.DocStatus, a.DocDate, a.CardCode, a.CardName, e.dscription, f.segment_0, f.acctname, e.price, b.WddCode, b.Remarks, a.DocTotal as Doctotal_ML, a.DocTotalSy as Doctotal_MS, b.status, x.compnyName, c.Name, m.USER_CODE, e.OcrCode, e.OcrCode2, e.OcrCode3, e.OcrCode4, e.OcrCode5, a.Comments, n.SlpName, a.DocCur, e.TrgetEntry, p.WTCode, p.WTAmnt, a.VatSum, a.VatSumSy, f.acctcode, db_name() as databases from ODRF a left join OWDD  b on a.DocEntry = b.DraftEntry left join OWST c on c.WstCode = b.CurrStep left join DRF1 e on e.docentry = a.docentry left join OACT f on f.acctcode = e.acctcode left join WDD1 o on b.WddCode = o.WddCode left join OUSR m on m.USERID = o.UserID left join OSLP n on n.SlpCode = a.SlpCode left join DRF5 p on p.AbsEntry = a.DocEntry,OADM x where  b.Status = 'Y' and  b.WddCode=" + requestPendientes.WddCode + "";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Ok(ds);
        }

        //EDITAR AUTORIZACIONES
        //Función pública tipo respuesta HTTP
        [HttpPost]
        //Defino la ruta
        [Route("api/Autorizaciones")]
        public IHttpActionResult UpdateItems([FromBody] Authorizations authorizations)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(authorizations.dbname, authorizations.userSap, authorizations.userSapPass);

            SAPbobsCOM.CompanyService oCompanyService = company.GetCompanyService();
            SAPbobsCOM.ApprovalRequestsService approvalSrv = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ApprovalRequestsService);
            ApprovalRequestParams oParams = approvalSrv.GetDataInterface(ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams) as ApprovalRequestParams;
            oParams.Code = authorizations.WddCode;
            ApprovalRequest oData = approvalSrv.GetApprovalRequest(oParams);

            //Agregar una autorización
            oData.ApprovalRequestDecisions.Add();
            oData.ApprovalRequestDecisions.Item(0).ApproverUserName = authorizations.userSap;
            oData.ApprovalRequestDecisions.Item(0).ApproverPassword = authorizations.userSapPass;
            //Autorizar
            if (authorizations.Status == "Y")
            {
                oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardApproved;
            }else
                if (authorizations.Status == "N")
            {
                oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardNotApproved;
            }
            oData.ApprovalRequestDecisions.Item(0).Remarks = authorizations.Remarks;

            //Actualizar la autorización 
            approvalSrv.UpdateRequest(oData);
            return Ok(authorizations);
        }

        [HttpPost]
        //Defino la ruta
        [Route("api/AutorizacionesMasivas")]
        public IHttpActionResult AutorizacionesMasivas([FromBody] AuthorizationsM authorizationsm)
        {
            //return Ok (authorizationsm.WddCode[1]);
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(authorizationsm.dbname, authorizationsm.userSap, authorizationsm.userSapPass);

            SAPbobsCOM.CompanyService oCompanyService = company.GetCompanyService();
            SAPbobsCOM.ApprovalRequestsService approvalSrv = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ApprovalRequestsService);
            ApprovalRequestParams oParams = approvalSrv.GetDataInterface(ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams) as ApprovalRequestParams;

            for (int i = 0; i < authorizationsm.WddCode.Length; i++)
            {
                oParams.Code = authorizationsm.WddCode[i];
                ApprovalRequest oData = approvalSrv.GetApprovalRequest(oParams);

                //Agregar una autorización
                oData.ApprovalRequestDecisions.Add();
                oData.ApprovalRequestDecisions.Item(0).ApproverUserName = authorizationsm.userSap;
                oData.ApprovalRequestDecisions.Item(0).ApproverPassword = authorizationsm.userSapPass;
                //Autorizar
                if (authorizationsm.Status == "Y")
                {
                    oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardApproved;
                }
                else
                    if (authorizationsm.Status == "N")
                {
                    oData.ApprovalRequestDecisions.Item(0).Status = BoApprovalRequestDecisionEnum.ardNotApproved;
                }
                oData.ApprovalRequestDecisions.Item(0).Remarks = authorizationsm.Remarks;
                //Actualizar la autorización 

                approvalSrv.UpdateRequest(oData);
            }

            return Ok(responseCall);
            //return Json(new { status = "error", message = "error creating customer" }); 
        }

        // GET api/<controller>/5
        [HttpGet]
        //Defino la ruta
        [Route("api/OpenDoc")]
        public HttpResponseMessage OpenDoc()
        {
            DataSet ds = new DataSet();
            OdbcCommand cmd;

            using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=PRUEBASTUSAP;Database=SBO_IDEACODEX;uid=sa;pwd=Soporte@2021"))
            {
                string query = "Select DocEntry, DocStatus, ObjType, CardName, Comments from ODRF where DocStatus = 'O'";
                cmd = new OdbcCommand(query, conn);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(ds, "Items");
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        [HttpPost]
        //Defino la ruta
        [Route("api/CreateDocument")]
        public IHttpActionResult CreateDocument(createDocs createdocs)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection();
            SAPbobsCOM.Documents draft;

            draft = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            draft.GetByKey(createdocs.DocEntry);
            draft.Comments = createdocs.Comments;
            draft.EDocStatus = (EDocStatusEnum)BoBoeStatus.boes_Created;
            //draft.DocumentStatus = BoStatus.bost_Close;
            draft.Update();

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

        public class createDocs
        {
            public int DocEntry { get; set; }
            public string Comments { get; set; }
        }
        public class Authorizations
        {
            public int WddCode { get; set; }
            public string Remarks { get; set; }
            public string Status { get; set;  }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public string dbname { get; set; }
        }
        public class AuthorizationsM
        {
            public string Remarks { get; set; }
            public string Status { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public string dbname { get; set; }
            public int[] WddCode { get; set; }
        }
        public class Connection
        {
            public string dbname { get; set; }
            public string userSap { get; set; }
            public string passSap { get; set; }
        }
        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }
        public class RequestPendientes
        {
            public string listDatabases { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public int WddCode { get; set; }
            public int Segment_0 { get; set; }
            
        }
    }
}