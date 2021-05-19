using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApi.Utils
{
    public class SAPConnection
    {
        // String companyDbInfo, String sapUserInfo, String sapPasswordInfo 
        public SAPbobsCOM.Company OpenConnection()
        {
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany.Server = "PRUEBASTUSAP";
            oCompany.SLDServer = "52.167.130.68:30010";
            oCompany.CompanyDB = "SBO_IDEACODEX";
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            oCompany.DbUserName = "sa";
            oCompany.DbPassword = "Soporte@2021";
            oCompany.UserName = "manager";
            oCompany.Password = "Soporte@21";
            oCompany.LicenseServer = "52.167.130.68:30000";
            oCompany.UseTrusted = false;
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;

            //Inicio la conexión
            oCompany.Connect();
            return oCompany;
        }
    }
}