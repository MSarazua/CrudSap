using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApi.Utils
{
    public class SAPConnection
    {

        /// Global variable that is constant.
        public string sqlServerSa = "sa";
        public string sqlServerPwd= "sapmv@2018@";
        public string SLDServer = "10.0.0.4:40000";
        public string Server = "SRVSAP";
        public string LicenseServer = "localhost:30000";


        public SAPbobsCOM.Company OpenConnection(string companyDb = "SBO_DemoGT", string Usersap = "manager", string PassSap = "manager")
        {
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany.Server = Server;
            oCompany.SLDServer = SLDServer;
            oCompany.CompanyDB = companyDb;
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            oCompany.DbUserName = sqlServerSa;
            oCompany.DbPassword = sqlServerPwd;
            oCompany.UserName = Usersap;
            oCompany.Password = PassSap;
            oCompany.LicenseServer = LicenseServer;
            oCompany.UseTrusted = false;
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;

            //Inicio la conexión
            oCompany.Connect();
            return oCompany;
        }
    }
}