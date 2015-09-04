<%@ WebService Language="C#" Class="Validation" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
//using VLF3.Common.Presentation.Controls;
//using VLF3.Common.Presentation.Validation.Implementation;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Validation  : System.Web.Services.WebService {

//    public Validation()
//    {

//        //Uncomment the following line if using designed components 
//        //InitializeComponent(); 
//    }

//    [WebMethod]
//    public string GeoNavInputValidation(short id, short controlType, bool isRequired, string value)
//    {
//        string message = "OK";
//        try
//        {
//            if (!Enum.IsDefined(typeof(ControlEnums.GeoNavControlTypes), controlType))
//            {
//                message = string.Format("Control type [{0}] is invalid", controlType);
//                return message;
//            }
//            ControlEnums.GeoNavControlTypes geoNav = (ControlEnums.GeoNavControlTypes)controlType;
//            if (isRequired)
//            {
//                if (string.IsNullOrEmpty(value))
//                {
//                    message = "Value cannot be null or empty";
//                    return message;
//                }

//            }
//            return (GeoNavInputValidator.Validate(value, geoNav, out message)) ? "OK" : message;
//        }
//        catch (Exception exc)
//        {
//            message = exc.Message;
//        }
//        return message;
//    }

}

