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
        [HttpPost]
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
            oItems.BarCode = itemDetails.BarCodes;
            //oItems.ItemsGroupCode = itemDetails.ItemsGroupCode;
            oItems.Manufacturer = itemDetails.Manufacturer;

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
                oItems.BarCode = itemDetails.BarCodes;
                //oItems.ItemsGroupCode = itemDetails.ItemsGroupCode;
                oItems.Manufacturer = itemDetails.Manufacturer;

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
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select a.ItemName, a.ItemCode, b.ItmsGrpNam, c.WhsCode, c.OnHand, a.CodeBars, db_name() as databases, d.FirmName from OITM a inner join OITB b on a.ItmsGrpCod = b.ItmsGrpCod left join OITW c on c.ItemCode = a.ItemCode left join OMRC d on d.FirmCode = a.FirmCode";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        [HttpPost]
        [Route("api/getOneItem")]
        public HttpResponseMessage getOneItem([FromBody] RequestArticulos requestArticulos)
        {
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select a.ItemName, a.ItemCode, b.ItmsGrpNam, c.WhsCode, c.OnHand, a.CodeBars, db_name() as databases, d.FirmName, a.FirmCode from OITM a inner join OITB b on a.ItmsGrpCod = b.ItmsGrpCod left join OITW c on c.ItemCode = a.ItemCode left join OMRC d on d.FirmCode = a.FirmCode where a.ItemCode = '" + requestArticulos.ItemCode + "'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        //Grupo de artículos
        [HttpPost]
        [Route("api/grupoArticulos")]
        public HttpResponseMessage getGrupoArticulos([FromBody] RequestArticulos requestArticulos)
        {
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select ItmsGrpCod, ItmsGrpNam, db_name() as databases from OITB";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        //Mostrar Fabricantes
        [HttpPost]
        [Route("api/getFabricante")]
        public HttpResponseMessage getFabricante([FromBody] RequestArticulos requestArticulos)
        {
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select FirmCode, FirmName, db_name() as databases from OMRC";
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
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select a.CardCode, a.CardName, b.GroupName, a.Address, a.E_Mail, a.Phone1, a.Phone2, a.Balance, c.ListName, d.PymntGroup, a.LicTradNum, db_name() as databases, a.CardType, case a.CardType when 'S' then 'Proveedor' when 'C' then 'Cliente' when 'L' then 'Lead' end from OCRD a inner join OCRG b on b.GroupCode = a.GroupCode left join OPLN c on c.ListNum = a.ListNum left join OCTG d on d.GroupNum = a.GroupNum";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        //Traer un solo cliente
        [HttpPost]
        [Route("api/getOneClient")]
        public HttpResponseMessage getOneClient([FromBody] RequestArticulos requestArticulos)
        {
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
                {
                    string query = "Select a.CardCode, a.CardName, b.GroupName, a.Address, a.E_Mail, a.Phone1, a.Phone2, a.Balance, c.ListName, d.PymntGroup, a.LicTradNum, db_name() as databases, a.CardType, case a.CardType when 'S' then 'Proveedor' when 'C' then 'Cliente' when 'L' then 'Lead' end from OCRD a inner join OCRG b on b.GroupCode = a.GroupCode left join OPLN c on c.ListNum = a.ListNum left join OCTG d on d.GroupNum = a.GroupNum where a.CardCode = '" + requestArticulos.CardCode + "'";
                    cmd = new OdbcCommand(query, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    da.Fill(ds, "Items");
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        [HttpPost]
        [Route("api/CreateClient")]
        public IHttpActionResult CreateClient([FromBody] RequestClientes requestClientes)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(requestClientes.dbname, requestClientes.userSap, requestClientes.userSapPass);

            SAPbobsCOM.BusinessPartners vBP;
            vBP = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            vBP.CardCode = requestClientes.CardCode;
            vBP.CardName = requestClientes.CardName;
            vBP.FederalTaxID = requestClientes.FederalTaxID;
            //vBP.CardType = requestClientes.CardType;
            vBP.Address = requestClientes.Address;
            vBP.EmailAddress = requestClientes.EmailAddress;
            vBP.Phone1 = requestClientes.Phone1;
            vBP.Phone2 = requestClientes.Phone2;
            vBP.PriceListNum = requestClientes.ListNum;
            int status = vBP.Add();

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

        //Modificar Cientes
        [HttpPost]
        //Defino la ruta
        [Route("api/UpdateClient")]
        public IHttpActionResult UpdateClient(RequestClientes requestClientes)
        {
            //Inicializo mi conexión a SAP
            SAPConnection conncetion = new SAPConnection();
            SAPbobsCOM.Company company = conncetion.OpenConnection(requestClientes.dbname, requestClientes.userSap, requestClientes.userSapPass);

            SAPbobsCOM.BusinessPartners vBP;
            vBP = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

            if (vBP.GetByKey(requestClientes.CardCode))
            {
                vBP.CardCode = requestClientes.CardCode;
                vBP.CardName = requestClientes.CardName;
                vBP.FederalTaxID = requestClientes.FederalTaxID;
                //vBP.CardType = requestClientes.CardType;
                vBP.Address = requestClientes.Address;
                vBP.EmailAddress = requestClientes.EmailAddress;
                vBP.Phone1 = requestClientes.Phone1;
                vBP.Phone2 = requestClientes.Phone2;
                vBP.PriceListNum = requestClientes.ListNum;

                int status = vBP.Update();

                //Compruebo si el guardado se ha realizado correctamente
                if (status == 0)
                {
                    responseCall.RespCode = "00";
                    responseCall.Description = "Cliente modificado correctamente";
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
                responseCall.Description = "El cliente no se encuentra en SAP";

            }

            return Ok(responseCall);
        }

        //CENTROS DE COSTO
        [HttpPost]
        [Route("api/CentrosCosto")]
        public HttpResponseMessage getCentrosCosto([FromBody] RequestArticulos requestArticulos)
        {
            SAPConnection conncetion = new SAPConnection();
            requestArticulos.listDatabases.Split(',').ToList<string>();
            DataSet ds = new DataSet();
            DataTable itemsData;
            OdbcCommand cmd;

            foreach (var item in requestArticulos.listDatabases.Split(',').ToList<string>())
            {
                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestArticulos.server + ";Database=" + item + ";uid="+  conncetion.sqlServerSa +";pwd=" + conncetion.sqlServerPwd))
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
            public double QuantityOnStock { get; set; }
            public string BarCodes { get; set; }
            public int ItemsGroupCode { get; set; }
            public int Manufacturer { get; set; }
        }

        public class RequestArticulos
        {
            public string listDatabases { get; set; }
            public string server { get; set; }
            public string ItemCode { get; set; }
            public string CardCode { get; set; }
        }

        public class CallResponse
        {
            public string RespCode { get; set; }
            public string Description { get; set; }
        }

        public class RequestClientes
        {
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public string CardType { get; set; }
            public int ListNum { get; set; }
            public string FederalTaxID { get; set; }
            public string Address { get; set; }
            public string EmailAddress { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string userSap { get; set; }
            public string userSapPass { get; set; }
            public string dbname { get; set; }
        }
    }
}