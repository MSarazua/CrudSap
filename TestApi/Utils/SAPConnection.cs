using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApi.Utils
{
    public class SAPConnection
    {
        public SAPbobsCOM.Company OpenConnection()
        {
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany.Server = "PRUEBASTUSAP";
            oCompany.CompanyDB = "SBO_IDEACODEX";
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            oCompany.DbUserName = "sa";
            oCompany.DbPassword = "Soporte@2021";
            oCompany.UserName = "manager";
            oCompany.Password = "Soporte@21";
            oCompany.UseTrusted = true;
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;

            //Inicio la conexión
            oCompany.Connect();
            return oCompany;
        }
    }
}