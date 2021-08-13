using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TestApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PaymentPlanController : ApiController
    {
        

        [HttpPost]
        //Defino la ruta
        [Route("api/InsertPayment")]
        public IHttpActionResult InsertPayment([FromBody] RequestPayment requestPayment)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable itemsData;
                OdbcCommand cmd;
                int lastId;

                using (OdbcConnection conn = new OdbcConnection(@"Driver={SQL Server};Server=" + requestPayment.server + ";Database=" + requestPayment.listDatabases + ";uid=sa;pwd=Soporte@2021"))
                {
                    conn.Open();
                    string querySelect = "select max(convert(int, RIGHT (DocNum, 10))) + 1 as DocNum from " + requestPayment.listDatabases + ".dbo.tbl_contrato";
                    cmd = new OdbcCommand(querySelect, conn);
                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    lastId = Convert.ToInt32(cmd.ExecuteScalar());
                    da.Fill(ds, "Items");
                    conn.Close();
                }

                string connectionString = "Data Source = " + requestPayment.server + "; Initial Catalog = " + requestPayment.listDatabases + "; Integrated Security = False; User ID = sa; Password = Soporte@2021; MultipleActiveResultSets = True; Connect Timeout = 300;";
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "BEGIN INSERT INTO tbl_contrato(DocNum, CardCode, DocDate, ItemCode, DbSap, CCost, Precio_Apto, Enganche_Apto, ItemCode_Par1, Precio_Par1, Enganche_Par1, ItemCode_Par2, Precio_Par2, Enganche_Par2, Total_Venta, Total_Enganche) VALUES (" + lastId + ",'" + requestPayment.CardCode + "', '" + requestPayment.DocDate + "', '" + requestPayment.ItemCode + "', '" + requestPayment.DbSap + "', '" + requestPayment.CCost + "', " + requestPayment.Precio_Apto + ", " + requestPayment.Enganche_Apto + ", '" + requestPayment.ItemCode_Par1 + "', " + requestPayment.Precio_Par1 + ", " + requestPayment.Enganche_Par1 + ", '" + requestPayment.ItemCode_Par2 + "', " + requestPayment.Precio_Par2 + ", " + requestPayment.Enganche_Par2 + ", " + requestPayment.Total_Venta + ", " + requestPayment.Total_Enganche + ") END";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                for (int i = 1; i <= requestPayment.Share; i++)
                {
                    string querydetail = "INSERT INTO tbl_datella_contrato(DocNum, CodeFee, DateFee, TotalFee, FeeApto, FeePar_1, FeePar_2) VALUES(" + lastId + ", '" + requestPayment.codeFeeNum + i + "', '" + requestPayment.DateFee + "', " + requestPayment.TotalFee + ", " + requestPayment.FeeApto + ", " + requestPayment.FeePar_1 + ", " + requestPayment.FeePar_2 + ")";
                    SqlCommand commandDetail = new SqlCommand(querydetail, connection);
                    connection.Open();
                    commandDetail.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                return Ok(e);
            }
            return Ok("Encabezado y detalle registrados");
        }

        public class RequestPayment
        {
            public string listDatabases { get; set; }
            public string server { get; set; }
            public string CardCode { get; set; }
            public string DocDate { get; set; }
            public string ItemCode { get; set; }
            public string DbSap { get; set; }
            public string CCost { get; set; }
            public double Precio_Apto { get; set; }
            public double Enganche_Apto { get; set; }
            public string ItemCode_Par1 { get; set; }
            public double Precio_Par1 { get; set; }
            public double Enganche_Par1 { get; set; }
            public string ItemCode_Par2 { get; set; }
            public double Precio_Par2 { get; set; }
            public double Enganche_Par2 { get; set; }
            public double Total_Venta { get; set; }
            public double Total_Enganche { get; set; }
            public int Share { get; set; }

            //Detalle de contrato
            public int DocNum { get; set; }
            public string CodeFee { get; set; }
            public string DateFee { get; set; }
            public int TotalFee { get; set; }
            public int FeeApto { get; set; }
            public int FeePar_1 { get; set; }
            public int FeePar_2 { get; set; }
            public string codeFeeNum { get; set; }
        }
    }
}