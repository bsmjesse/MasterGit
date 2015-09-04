using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Collections;

using VLF.MAP;
using VLF.DAS.Logic;
using System.IO;
using SentinelFM;
using NPOI.HSSF.UserModel;


namespace SentinelFM
{

    public partial class frmLandmarkImport : SentinelFMBasePage
    {
        List<LandmarkDIItem> _InputList = new List<LandmarkDIItem>();
        Dictionary<string, string> _MappedColumnDict = new Dictionary<string, string>();
        protected SentinelFMSession sn = null;
        string instructionMessage = "The data import file should have the following mandatory columns: LandmarkName, CategoryName, Latitude, Longitude, and Radius. The following are optional columns: Description, ContactPersonName, ContactPhoneNumber, Email, Phone, TimeZone, DayLightSaving, AutoAdjustDayLightSaving, and Public.";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if ((sn == null) || (sn.UserName == ""))
                {
                    this.Label1.Text = "Session out. Please login again.";
                    return;
                }
                if (this.IsPostBack == false)
                {
                    this.lblGridCaption.Visible = false;
                }
            }
            catch
            {
                Label1.Text = "Error loading landmarks page";
                return;
            }
        }

        protected void Submit2_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            try
            {
                bool fileOK = false;
                string ext = "";
                if (FileUpload1.HasFile)
                {
                    ext = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
                    if (ext == ".xls") fileOK = true;
                }
                if (fileOK)
                {
                    // parse xls file
                    try
                    {
                        string serverPath = Server.MapPath("~/App_Data/");
                        string fileName = serverPath + sn.User.OrganizationId + "_" + FileUpload1.FileName;
                        try
                        {
                            FileUpload1.PostedFile.SaveAs(fileName);
                        }
                        catch (Exception exc)
                        {
                            Label1.Text = "Error saving uploading file";// +exc.Message;
                            return;
                        }


                        string errorMessage = ParseLandmarksExcel(fileName);
                        if (string.IsNullOrEmpty(errorMessage) == true)
                        {
                            ValidateInputList();
                            ResolveAddressInputList();
                            SaveLandmarksToDatabase();
                        }
                        else
                        {
                            Label1.Text = errorMessage;
                        }

                        DisplayResult();
                    }
                    catch (Exception exc)
                    {
                        Label1.Text = "Error uploading file. Please check the xls file format. " + instructionMessage;// +exc.Message;
                    }
                }
                else Label1.Text = "Files of this type are not supported: *." + ext;
            }
            catch
            {
                Label1.Text = "Error uploading file";
                return;
            }
        }

        private void DisplayResult()
        {
            this.gvErrors.DataSource = this._InputList;
            this.gvErrors.DataBind();
            this.lblGridCaption.Visible = true;
        }

        private string CleanUp(string inputString)
        {
            return inputString.Trim();
        }


        private void ResolveAddressInputList()
        {

            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            DataSet dsGeoCodeEngines = new DataSet();
            DataSet dsMapEngines = new DataSet();
            string xml = "";
            clsUtility objUtil = new clsUtility(sn);

            if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                        VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
                        "GetGeoCodeEnginesInfo. User:" + sn.UserID.ToString() + " Form:clsUser "));
                }
            if (xml != "")
            {
                StringReader strrXML = new StringReader(xml);
                dsGeoCodeEngines.ReadXml(strrXML);
            }

            VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeEngines));
            Resolver.Resolver oneResolver = null;
            string resultedAddress = string.Empty;
            double resultedLatitude = 0.0 , resultedLongitude = 0.0;
            bool isSuccess = false;

            foreach (LandmarkDIItem oneItem in this._InputList)
            {
                if (oneItem.IsValid == true && oneItem.TypedObject != null)
                {
                    try
                    {
                        resultedAddress = string.Empty;
                        oneResolver = new Resolver.Resolver();

                        if (oneItem.ShouldGeocode == false)
                        {
                            isSuccess = oneResolver.StreetAddress(oneItem.TypedObject.Latitude, oneItem.TypedObject.Longitude, ref resultedAddress);

                            if (isSuccess == true)
                            {
                                if (resultedAddress != "No GPS Data")
                                {
                                    oneItem.TypedObject.ResolvedAddress = resultedAddress;
                                }
                                else
                                {
                                    oneItem.IsValid = false;
                                    oneItem.TypedObject.ResolvedAddress = "Unresolved";
                                    oneItem.ErrorMessageList.Add("Address could not be resolved from the coordinate");
                                }
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.TypedObject.ResolvedAddress = "Unresolved";
                                oneItem.ErrorMessageList.Add("Address could not be resolved from the coordinate");
                            }
                        }
                        else
                        {
                            isSuccess = oneResolver.Location(oneItem.Address, ref resultedLatitude, ref resultedLongitude, ref resultedAddress);
                            if (isSuccess == true)
                            {
                                if (resultedLatitude == 0.0 && resultedLongitude == 0.0)
                                {
                                    oneItem.IsValid = false;
                                    oneItem.ErrorMessageList.Add("Coordinate could not be resolved from the address");
                                }
                                else
                                {
                                    oneItem.TypedObject.ResolvedLatitude = resultedLatitude;
                                    oneItem.TypedObject.ResolvedLongitude = resultedLongitude;
                                    oneItem.TypedObject.ResolvedAddress = resultedAddress;
                                }
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.ErrorMessageList.Add("Coordinate could not be resolved from the address");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oneItem.IsValid = false;
                        oneItem.ErrorMessageList.Add("Address resolution failed.");
                    }
                }
            }
        }


        private void ValidateInputList()
        {
            LandmarkDITypedItem oneTypedItem = null;
            string lengthValidationMessageFormat = "'{0}' can not be more than {1} characters";
            string invalidDataValidationMessageFormat = "Invalid data provided for column '{0}'";
            bool? DayLightSavingResult, AutoAdjustDayLightSavingResult, PublicResult;
            short? TimeZoneResult;

            foreach (LandmarkDIItem oneItem in this._InputList)
            {
                if (oneItem.IsValid == true)
                {
                    try
                    {
                        oneTypedItem = new LandmarkDITypedItem();

                        oneTypedItem.CategoryName = oneItem.CategoryName;
                        oneTypedItem.ContactPersonName = oneItem.ContactPersonName;
                        oneTypedItem.ContactPhoneNumber = oneItem.ContactPhoneNumber;
                        oneTypedItem.Description = oneItem.Description;
                        oneTypedItem.Email = oneItem.Email;

                        oneTypedItem.Address = oneItem.Address;
                        oneTypedItem.Phone = oneItem.Phone;
                        oneTypedItem.LandmarkName = oneItem.LandmarkName;

                        oneTypedItem.Latitude = ConvertStringToDouble(oneItem.Latitude).GetValueOrDefault();
                        oneTypedItem.Longitude = ConvertStringToDouble(oneItem.Longitude).GetValueOrDefault();
                        oneTypedItem.Radius = ConvertStringToInt(oneItem.Radius).GetValueOrDefault();

                        if (string.IsNullOrEmpty(oneItem.TimeZone) == false)
                        {
                            TimeZoneResult = ConvertStringToShort(oneItem.TimeZone);
                            if (TimeZoneResult.HasValue == true)
                            {
                                oneTypedItem.TimeZone = TimeZoneResult.Value;
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.TimeZone));
                            }
                        }

                        if (string.IsNullOrEmpty(oneItem.AutoAdjustDayLightSaving) == false)
                        {
                            AutoAdjustDayLightSavingResult = ConvertStringToBool(oneItem.AutoAdjustDayLightSaving);
                            if (AutoAdjustDayLightSavingResult.HasValue == true)
                            {
                                oneTypedItem.AutoAdjustDayLightSaving = AutoAdjustDayLightSavingResult.Value;
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.AutoAdjustDayLightSaving));
                            }
                        }

                        if (string.IsNullOrEmpty(oneItem.DayLightSaving) == false)
                        {
                            DayLightSavingResult = ConvertStringToBool(oneItem.DayLightSaving);
                            if (DayLightSavingResult.HasValue == true)
                            {
                                oneTypedItem.DayLightSaving = DayLightSavingResult.Value;
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.DayLightSaving));
                            }
                        }

                        if (string.IsNullOrEmpty(oneItem.Public) == false)
                        {
                            PublicResult = ConvertStringToBool(oneItem.Public);
                            if (PublicResult.HasValue == true)
                            {
                                oneTypedItem.Public = PublicResult.Value;
                            }
                            else
                            {
                                oneItem.IsValid = false;
                                oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.Public));
                            }
                        }


                        // Validate mandatory fields
                        if (oneTypedItem.Latitude == 0.0 && string.IsNullOrEmpty(oneTypedItem.Address) == true)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.Latitude));
                        }
                        if (oneTypedItem.Longitude == 0.0 && string.IsNullOrEmpty(oneTypedItem.Address) == true)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.Longitude));
                        }
                        if ((oneTypedItem.Radius > 0) == false)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(invalidDataValidationMessageFormat, LandmarkDIColumns.Radius));
                        }

                        // Length validation
                        if (oneTypedItem.LandmarkName.Length > 64)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.LandmarkName, "64"));
                        }
                        if (oneTypedItem.CategoryName.Length > 40)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.CategoryName, "40"));
                        }
                        if (string.IsNullOrEmpty(oneTypedItem.Address) == false && oneTypedItem.Address.Length > 256)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.Address, "256"));
                        }

                        // optional fields - length validation
                        if (_MappedColumnDict.ContainsKey(LandmarkDIColumns.Description) == true &&
                            string.IsNullOrEmpty(oneTypedItem.Description) == false &&
                            oneTypedItem.Description.Length > 100)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.Description, "100"));
                        }
                        if (_MappedColumnDict.ContainsKey(LandmarkDIColumns.ContactPersonName) == true &&
                            string.IsNullOrEmpty(oneTypedItem.ContactPersonName) == false &&
                            oneTypedItem.ContactPersonName.Length > 100)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.ContactPersonName, "100"));
                        }
                        if (_MappedColumnDict.ContainsKey(LandmarkDIColumns.ContactPhoneNumber) == true &&
                            string.IsNullOrEmpty(oneTypedItem.ContactPhoneNumber) == false &&
                            oneTypedItem.ContactPhoneNumber.Length > 20)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.ContactPhoneNumber, "20"));
                        }
                        if (_MappedColumnDict.ContainsKey(LandmarkDIColumns.Email) == true &&
                            string.IsNullOrEmpty(oneTypedItem.Email) == false &&
                            oneTypedItem.Email.Length > 128)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.Email, "128"));
                        }
                        if (_MappedColumnDict.ContainsKey(LandmarkDIColumns.Phone) == true &&
                            string.IsNullOrEmpty(oneTypedItem.Phone) == false &&
                            oneTypedItem.Phone.Length > 55)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.Phone, "55"));
                        }
                        if (string.IsNullOrEmpty(oneTypedItem.Address) == false &&
                            oneTypedItem.Address.Length > 256)
                        {
                            oneItem.IsValid = false;
                            oneItem.ErrorMessageList.Add(string.Format(lengthValidationMessageFormat, LandmarkDIColumns.Address, "256"));
                        }
                    }
                    catch (Exception ex)
                    {
                        oneItem.IsValid = false;
                        oneItem.ErrorMessageList.Add(string.Format("Error occurred while validating data: {0}", ex.Message));
                    }
                    

                    oneItem.TypedObject = oneTypedItem;
                }

            }
        }


        private void SaveLandmarksToDatabase()
        {
            LandmarkDITypedItem oneTypedItem = null;
            int dbResult = 0;
            string errorMessage = null;

            using (ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization())
            {

                foreach (LandmarkDIItem oneItem in this._InputList)
                {

                    if (oneItem.IsValid == true)
                    {
                        oneTypedItem = oneItem.TypedObject;
                        // Save to database
                        try
                        {
                            if (oneItem.ShouldGeocode == true)
                            {
                                // Save ResolvedLatitude, ResolvedLongitude, ResolvedAddress
                                dbResult = orgProxy.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId,
                                                oneTypedItem.ResolvedLatitude, oneTypedItem.ResolvedLongitude, oneTypedItem.LandmarkName,
                                                oneTypedItem.Description, oneTypedItem.ContactPersonName, oneTypedItem.ContactPhoneNumber,
                                                oneTypedItem.Radius, oneTypedItem.Email, oneTypedItem.Phone, oneTypedItem.TimeZone,
                                                oneTypedItem.DayLightSaving, oneTypedItem.AutoAdjustDayLightSaving,
                                                oneTypedItem.ResolvedAddress, oneTypedItem.Public, sn.UserID,
                                                oneTypedItem.CategoryName, ref errorMessage);
                            }
                            else
                            {
                                // Save ResolvedAddress
                                dbResult = orgProxy.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId,
                                                oneTypedItem.Latitude, oneTypedItem.Longitude, oneTypedItem.LandmarkName,
                                                oneTypedItem.Description, oneTypedItem.ContactPersonName, oneTypedItem.ContactPhoneNumber,
                                                oneTypedItem.Radius, oneTypedItem.Email, oneTypedItem.Phone, oneTypedItem.TimeZone,
                                                oneTypedItem.DayLightSaving, oneTypedItem.AutoAdjustDayLightSaving,
                                                oneTypedItem.ResolvedAddress, oneTypedItem.Public, sn.UserID,
                                                oneTypedItem.CategoryName, ref errorMessage);
                            }
                            
                                
                            if (dbResult == (int)VLF.ERRSecurity.InterfaceError.NoError)
                            {
                                oneItem.IsAdded = true;
                            }
                            else
                            {
                                oneItem.IsAdded = false;
                                if ((int)dbResult == (int)VLF.ERRSecurity.InterfaceError.ServerError)
                                {
                                    if (string.IsNullOrEmpty(errorMessage) == false && errorMessage.Contains("IX_vlfLandmark_OrganizationId_LandmarkName") == true)
                                    {
                                        oneItem.ErrorMessageList.Add("Duplicate Landmark");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            oneItem.IsAdded = false;
                            oneItem.ErrorMessageList.Add(string.Format("{0}", ex.Message));
                        }

                    }
                }

            }
            
        }


        private short? ConvertStringToShort(string pInputString)
        {
            short? rvShort = null;
            short outValue = 0;

            bool isSuccess = short.TryParse(pInputString, out outValue);
            if (isSuccess == true)
            {
                rvShort = outValue;
            }

            return rvShort;
        }

        private int? ConvertStringToInt(string pInputString)
        {
            int? rvInt = null;
            int outValue = 0;

            bool isSuccess = int.TryParse(pInputString, out outValue);
            if (isSuccess == true)
            {
                rvInt = outValue;
            }

            return rvInt;
        }

        private double? ConvertStringToDouble(string pInputString)
        {
            double? rvDouble = null;
            double outValue = 0.0;

            bool isSuccess = double.TryParse(pInputString, out outValue);
            if (isSuccess == true)
            {
                rvDouble = outValue;
            }

            return rvDouble;
        }

        private bool? ConvertStringToBool(string pInputString)
        {
            bool? rvBool = null;
            bool outValue = false;

            bool isSuccess = bool.TryParse(pInputString, out outValue);
            if (isSuccess == false)
            {
                //yes in french 'oui', no in french 'aucun'
                if (string.IsNullOrEmpty(pInputString) == false)
                {
                    if (pInputString == "1" || pInputString.ToLower() == "yes" || pInputString.ToLower() == "oui")
                    {
                        rvBool = true;
                    }
                    else if (pInputString == "0" || pInputString.ToLower() == "no" || pInputString.ToLower() == "aucun")
                    {
                        rvBool = false;
                    }
                }
            }
            else
            {
                rvBool = outValue;
            }

            return rvBool;
        }


        private string ParseLandmarksExcel(string path)
        {
            string rvErrorMessage = null;
            _InputList = new List<LandmarkDIItem>();
            List<string> errorList = new List<string>();

            DataTable sourceTable = ReadDataFromExcelUsingNPOI(path);


            DataColumnCollection columns = sourceTable.Columns;
            LandmarkDIItem oneLandmarkDIItem = null;
            string errorMessageFormat = "Missing mandatory column '{0}'";
            string errorMessageLocationInfoFormat = "Either {0} and {1} columns or {2} column must be provided";

            // Mandatory columns
            if (columns.Contains(LandmarkDIColumns.LandmarkName) == false)
            {
                errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.LandmarkName));
            }
            if (columns.Contains(LandmarkDIColumns.CategoryName) == false)
            {
                errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.CategoryName));
            }
            if (columns.Contains(LandmarkDIColumns.Radius) == false)
            {
                errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Radius));
            }

            if (columns.Contains(LandmarkDIColumns.Latitude) == false && 
                columns.Contains(LandmarkDIColumns.Longitude) == false &&
                columns.Contains(LandmarkDIColumns.Address) == false)
            {
                errorList.Add(string.Format(errorMessageLocationInfoFormat, LandmarkDIColumns.Latitude, 
                                                                            LandmarkDIColumns.Longitude, LandmarkDIColumns.Address));
            }
            else if (columns.Contains(LandmarkDIColumns.Address) == false)
            {
                if (columns.Contains(LandmarkDIColumns.Latitude) == false)
                {
                    errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Latitude));
                }
                if (columns.Contains(LandmarkDIColumns.Longitude) == false)
                {
                    errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Longitude));
                }
            }


            if (errorList.Count != 0)
            {
                rvErrorMessage = string.Join(". ", errorList.ToArray());
            }
            else
            {
                foreach (DataColumn oneColumn in columns)
                {
                    this._MappedColumnDict.Add(oneColumn.ColumnName, oneColumn.ColumnName);
                }
            }

            if (string.IsNullOrEmpty(rvErrorMessage) == true)
            {
                string errorMessageForRequiredField = "'{0}' can not be empty";
                string errorMessageForRequiredFieldLocationInfoFormat = "Either {0} and {1} values or {2} value must be provided";
                int rowNo = 2;

                foreach (DataRow oneRow in sourceTable.Rows)
                {
                    oneLandmarkDIItem = new LandmarkDIItem();
                    
                    try
                    {
                        // Mandatory columns
                        if (columns.Contains(LandmarkDIColumns.LandmarkName) == true)
                        {
                            oneLandmarkDIItem.LandmarkName = CleanUp(oneRow[LandmarkDIColumns.LandmarkName].ToString());
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.LandmarkName) == true)
                            {
                                oneLandmarkDIItem.IsValid = false;
                                oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.LandmarkName));
                            }
                        }

                        if (columns.Contains(LandmarkDIColumns.CategoryName) == true)
                        {
                            oneLandmarkDIItem.CategoryName = CleanUp(oneRow[LandmarkDIColumns.CategoryName].ToString());
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.CategoryName) == true)
                            {
                                oneLandmarkDIItem.IsValid = false;
                                oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.CategoryName));
                            }
                        }

                        if (columns.Contains(LandmarkDIColumns.Radius) == true)
                        {
                            oneLandmarkDIItem.Radius = CleanUp(oneRow[LandmarkDIColumns.Radius].ToString());
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.Radius) == true)
                            {
                                oneLandmarkDIItem.IsValid = false;
                                oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Radius));
                            }
                        }

                        if (columns.Contains(LandmarkDIColumns.Address) == true)
                        {
                            oneLandmarkDIItem.Address = CleanUp(oneRow[LandmarkDIColumns.Address].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.Latitude) == true)
                        {
                            oneLandmarkDIItem.Latitude = CleanUp(oneRow[LandmarkDIColumns.Latitude].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.Longitude) == true)
                        {
                            oneLandmarkDIItem.Longitude = CleanUp(oneRow[LandmarkDIColumns.Longitude].ToString());
                        }


                        if (string.IsNullOrEmpty(oneLandmarkDIItem.Latitude) == true && 
                            string.IsNullOrEmpty(oneLandmarkDIItem.Longitude) == true &&
                            string.IsNullOrEmpty(oneLandmarkDIItem.Address) == true)
                        {
                            oneLandmarkDIItem.IsValid = false;
                            oneLandmarkDIItem.ErrorMessageList.Add(
                                string.Format(errorMessageForRequiredFieldLocationInfoFormat, LandmarkDIColumns.Latitude,
                                                                LandmarkDIColumns.Longitude, LandmarkDIColumns.Address));
                        }
                        else if (string.IsNullOrEmpty(oneLandmarkDIItem.Address) == true)
                        {
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.Latitude) == true)
                            {
                                oneLandmarkDIItem.IsValid = false;
                                oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Latitude));
                            }
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.Latitude) == true)
                            {
                                oneLandmarkDIItem.IsValid = false;
                                oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Latitude));
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(oneLandmarkDIItem.Latitude) == true && 
                                string.IsNullOrEmpty(oneLandmarkDIItem.Longitude) == true)
                            {
                                oneLandmarkDIItem.ShouldGeocode = true;
                            }
                            
                        }


                        // Optional columns
                        if (columns.Contains(LandmarkDIColumns.Phone) == true)
                        {
                            oneLandmarkDIItem.Phone = CleanUp(oneRow[LandmarkDIColumns.Phone].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.Public) == true)
                        {
                            oneLandmarkDIItem.Public = CleanUp(oneRow[LandmarkDIColumns.Public].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.TimeZone) == true)
                        {
                            oneLandmarkDIItem.TimeZone = CleanUp(oneRow[LandmarkDIColumns.TimeZone].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.AutoAdjustDayLightSaving) == true)
                        {
                            oneLandmarkDIItem.AutoAdjustDayLightSaving = CleanUp(oneRow[LandmarkDIColumns.AutoAdjustDayLightSaving].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.ContactPersonName) == true)
                        {
                            oneLandmarkDIItem.ContactPersonName = CleanUp(oneRow[LandmarkDIColumns.ContactPersonName].ToString());
                        }

                        if (columns.Contains(LandmarkDIColumns.ContactPhoneNumber) == true)
                        {
                            oneLandmarkDIItem.ContactPhoneNumber = CleanUp(oneRow[LandmarkDIColumns.ContactPhoneNumber].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.DayLightSaving) == true)
                        {
                            oneLandmarkDIItem.DayLightSaving = CleanUp(oneRow[LandmarkDIColumns.DayLightSaving].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.Description) == true)
                        {
                            oneLandmarkDIItem.Description = CleanUp(oneRow[LandmarkDIColumns.Description].ToString());
                        }
                        if (columns.Contains(LandmarkDIColumns.Email) == true)
                        {
                            oneLandmarkDIItem.Email = CleanUp(oneRow[LandmarkDIColumns.Email].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        oneLandmarkDIItem.IsValid = false;
                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format("Error occured while parsing the data: {0}", ex.Message));
                    }

                    oneLandmarkDIItem.RowNumber = rowNo;
                    _InputList.Add(oneLandmarkDIItem);
                    rowNo++;
                }
            }


            return rvErrorMessage;
        }



        //private string ParseLandmarksExcel(string path)
        //{
        //    string rvErrorMessage = null;
        //    _InputList = new List<LandmarkDIItem>();
        //    DataSet dsLandmarks = new DataSet("Landmarks");
        //    List<string> errorList = new List<string>();

        //    // connection string
        //    string connString = String.Format(
        //   "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";", path);

        //    // get data into dataset
        //    using (OleDbConnection conn = new OleDbConnection(connString))
        //    {
        //        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
        //        da.Fill(dsLandmarks, "tblMaster");
        //    }


        //    //DataTable sourceTable = ReadDataFromExcelUsingNPOI(path);


        //    DataColumnCollection columns = dsLandmarks.Tables[0].Columns;
        //    LandmarkDIItem oneLandmarkDIItem = null;
        //    string errorMessageFormat = "Missing mandatory column '{0}'";

        //    // Mandatory columns
        //    if (columns.Contains(LandmarkDIColumns.LandmarkName) == false)
        //    {
        //        errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.LandmarkName));
        //    }
        //    if (columns.Contains(LandmarkDIColumns.CategoryName) == false)
        //    {
        //        errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.CategoryName));
        //    }
        //    if (columns.Contains(LandmarkDIColumns.Latitude) == false)
        //    {
        //        errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Latitude));
        //    }
        //    if (columns.Contains(LandmarkDIColumns.Longitude) == false)
        //    {
        //        errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Longitude));
        //    }
        //    if (columns.Contains(LandmarkDIColumns.Radius) == false)
        //    {
        //        errorList.Add(string.Format(errorMessageFormat, LandmarkDIColumns.Radius));
        //    }

        //    if (errorList.Count != 0)
        //    {
        //        rvErrorMessage = string.Join(". ", errorList.ToArray());
        //    }
        //    else
        //    {
        //        foreach (DataColumn oneColumn in columns)
        //        {
        //            this._MappedColumnDict.Add(oneColumn.ColumnName, oneColumn.ColumnName);
        //        }
        //    }

        //    if (string.IsNullOrEmpty(rvErrorMessage) == true)
        //    {
        //        string errorMessageForRequiredField = "'{0}' can not be empty";

        //        foreach (DataRow oneRow in dsLandmarks.Tables[0].Rows)
        //        {
        //            oneLandmarkDIItem = new LandmarkDIItem();

        //            try
        //            {
        //                // Mandatory columns
        //                if (columns.Contains(LandmarkDIColumns.LandmarkName) == true)
        //                {
        //                    oneLandmarkDIItem.LandmarkName = CleanUp(oneRow[LandmarkDIColumns.LandmarkName].ToString());
        //                    if (string.IsNullOrEmpty(oneLandmarkDIItem.LandmarkName) == true)
        //                    {
        //                        oneLandmarkDIItem.IsValid = false;
        //                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.LandmarkName));
        //                    }
        //                }

        //                if (columns.Contains(LandmarkDIColumns.CategoryName) == true)
        //                {
        //                    oneLandmarkDIItem.CategoryName = CleanUp(oneRow[LandmarkDIColumns.CategoryName].ToString());
        //                    if (string.IsNullOrEmpty(oneLandmarkDIItem.CategoryName) == true)
        //                    {
        //                        oneLandmarkDIItem.IsValid = false;
        //                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.CategoryName));
        //                    }
        //                }

        //                if (columns.Contains(LandmarkDIColumns.Latitude) == true)
        //                {
        //                    oneLandmarkDIItem.Latitude = CleanUp(oneRow[LandmarkDIColumns.Latitude].ToString());
        //                    if (string.IsNullOrEmpty(oneLandmarkDIItem.Latitude) == true)
        //                    {
        //                        oneLandmarkDIItem.IsValid = false;
        //                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Latitude));
        //                    }
        //                }

        //                if (columns.Contains(LandmarkDIColumns.Longitude) == true)
        //                {
        //                    oneLandmarkDIItem.Longitude = CleanUp(oneRow[LandmarkDIColumns.Longitude].ToString());
        //                    if (string.IsNullOrEmpty(oneLandmarkDIItem.Longitude) == true)
        //                    {
        //                        oneLandmarkDIItem.IsValid = false;
        //                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Longitude));
        //                    }
        //                }

        //                if (columns.Contains(LandmarkDIColumns.Radius) == true)
        //                {
        //                    oneLandmarkDIItem.Radius = CleanUp(oneRow[LandmarkDIColumns.Radius].ToString());
        //                    if (string.IsNullOrEmpty(oneLandmarkDIItem.Radius) == true)
        //                    {
        //                        oneLandmarkDIItem.IsValid = false;
        //                        oneLandmarkDIItem.ErrorMessageList.Add(string.Format(errorMessageForRequiredField, LandmarkDIColumns.Radius));
        //                    }
        //                }


        //                // Optional columns
        //                if (columns.Contains(LandmarkDIColumns.Phone) == true)
        //                {
        //                    oneLandmarkDIItem.Phone = CleanUp(oneRow[LandmarkDIColumns.Phone].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.Public) == true)
        //                {
        //                    oneLandmarkDIItem.Public = CleanUp(oneRow[LandmarkDIColumns.Public].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.TimeZone) == true)
        //                {
        //                    oneLandmarkDIItem.TimeZone = CleanUp(oneRow[LandmarkDIColumns.TimeZone].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.AutoAdjustDayLightSaving) == true)
        //                {
        //                    oneLandmarkDIItem.AutoAdjustDayLightSaving = CleanUp(oneRow[LandmarkDIColumns.AutoAdjustDayLightSaving].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.ContactPersonName) == true)
        //                {
        //                    oneLandmarkDIItem.ContactPersonName = CleanUp(oneRow[LandmarkDIColumns.ContactPersonName].ToString());
        //                }

        //                if (columns.Contains(LandmarkDIColumns.ContactPhoneNumber) == true)
        //                {
        //                    oneLandmarkDIItem.ContactPhoneNumber = CleanUp(oneRow[LandmarkDIColumns.ContactPhoneNumber].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.DayLightSaving) == true)
        //                {
        //                    oneLandmarkDIItem.DayLightSaving = CleanUp(oneRow[LandmarkDIColumns.DayLightSaving].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.Description) == true)
        //                {
        //                    oneLandmarkDIItem.Description = CleanUp(oneRow[LandmarkDIColumns.Description].ToString());
        //                }
        //                if (columns.Contains(LandmarkDIColumns.Email) == true)
        //                {
        //                    oneLandmarkDIItem.Email = CleanUp(oneRow[LandmarkDIColumns.Email].ToString());
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                oneLandmarkDIItem.IsValid = false;
        //                oneLandmarkDIItem.ErrorMessageList.Add(string.Format("Error occured while parsing the data: {0}", ex.Message));
        //            }

        //            _InputList.Add(oneLandmarkDIItem);
        //        }
        //    }

 
        //    return rvErrorMessage;
        //}


        public DataTable ReadDataFromExcelUsingNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            if (rows.MoveNext())
            {
                HSSFRow row0 = (HSSFRow)rows.Current;
                for (int i = 0; i < row0.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row0.GetCell(i);

                    dt.Columns.Add(cell.ToString());
                }
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(i);

                    dr[i] = (cell == null) ? "" : cell.ToString();

                }
                dt.Rows.Add(dr);
            }
            return dt;
        } 
    }


    public class LandmarkDIItem
    {
      //        ,[LandmarkName] --1 
      //,[Latitude] -- 2 
      //,[Longitude] -- 3
      //,[Description] -- 4
      //,[ContactPersonName] -- 5
      //,[ContactPhoneNum] -- 6
      //,[Radius] -- 7
      //,[Email] -- 8
      //,[Phone] -- Not there
      //,[TimeZone] -- 9 
      //,[DayLightSaving] -- Not there
      //,[AutoAdjustDayLightSaving] -- Not there
      //,[StreetAddress] -- 10
      //,[Public] -- Not there
      //CategoryName

        public LandmarkDIItem() 
        {
            IsValid = true;
            ErrorMessageList = new List<string>();
        }

        public int RowNumber { get; set; }

        public string LandmarkName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CategoryName { get; set; }
        public string Radius { get; set; }

        public string Description { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string TimeZone { get; set; }
        public string Address { get; set; }
        public string DayLightSaving { get; set; }
        public string AutoAdjustDayLightSaving { get; set; }
        public string Public { get; set; }

        public bool IsValid { get; set; }
        public bool IsAdded { get; set; }
        public List<string> ErrorMessageList { get; set; }
        public LandmarkDITypedItem TypedObject { get; set; }

        public bool ShouldGeocode { get; set; }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                _ErrorMessage = string.Empty;
                if (ErrorMessageList.Count >= 0)
                {
                    _ErrorMessage = String.Join(". ", this.ErrorMessageList.ToArray());
                }

                return _ErrorMessage;
            }
        }

        private string _ActionResult;
        public string ActionResult
        {
            get
            {
                _ActionResult = string.Empty;
                if (this.IsAdded == true)
                {
                    _ActionResult = "Added";
                }
                else
                {
                    _ActionResult = "Failed";
                }

                return _ActionResult;
            }
        }

        private string _ActionResultHtml;
        public string ActionResultHtml
        {
            get
            {
                _ActionResultHtml = string.Empty;
                if (this.IsAdded == true)
                {
                    _ActionResultHtml = "<img border='0' src='../images/ok.gif' title='Added' alt='Added'>";
                }
                else
                {
                    _ActionResultHtml = "<img border='0' src='../images/cancel.gif' title='Failed' alt='Failed'>";
                }

                return _ActionResultHtml;
            }
        }

        private string _SelectedAddress;
        public string SelectedAddress
        {
            get
            {
                if(this.TypedObject != null){
                    _SelectedAddress = this.TypedObject.Address;
                    if (this.ShouldGeocode == false)
                    {
                        _SelectedAddress = this.TypedObject.ResolvedAddress;
                    }
                }
                return _SelectedAddress;
            }
        }

        private string _ResolvedAddress;
        public string ResolvedAddress
        {
            get
            {
                if (this.TypedObject != null)
                {
                    _ResolvedAddress = this.TypedObject.ResolvedAddress;
                }
                return _ResolvedAddress;
            }
        }

        private string _SelectedLatitude;
        public string SelectedLatitude
        {
            get {
                if (this.TypedObject != null)
                {
                    _SelectedLatitude = this.TypedObject.Latitude.ToString();
                    if (this.ShouldGeocode == true)
                    {
                        _SelectedLatitude = this.TypedObject.ResolvedLatitude.ToString();
                    }
                }
                return _SelectedLatitude; 
            }
            set { _SelectedLatitude = value; }
        }

        private string _SelectedLongitude;
        public string SelectedLongitude
        {
            get {
                if (this.TypedObject != null)
                {
                    _SelectedLongitude = this.TypedObject.Longitude.ToString();
                    if (this.ShouldGeocode == true)
                    {
                        _SelectedLongitude = this.TypedObject.ResolvedLongitude.ToString();
                    }
                }
                return _SelectedLongitude; 
            }
            set { _SelectedLongitude = value; }
        }
        
    }


    //[LandmarkName] [varchar](64) NOT NULL,
    //[Latitude] [float] NOT NULL,
    //[Longitude] [float] NOT NULL,
    //[Description] [varchar](100) NULL,
    //[ContactPersonName] [varchar](100) NULL,
    //[ContactPhoneNum] [varchar](20) NULL,
    //[Radius] [int] NOT NULL,
    //[Email] [varchar](128) NULL,
    //[Phone] [varchar](55) NULL,
    //[TimeZone] [int] NULL CONSTRAINT [DF_vlfLandmark_TimeZone]  DEFAULT ((0)),
    //[DayLightSaving] [bit] NULL CONSTRAINT [DF_vlfLandmark_DayLightSaving]  DEFAULT ((0)),
    //[AutoAdjustDayLightSaving] [bit] NULL,
    //[StreetAddress] [varchar](256) NULL,
    //[Public] [bit] NULL CONSTRAINT [DF_vlfLandmark_Public]  DEFAULT ((0)),


    public class LandmarkDITypedItem
    {
        public LandmarkDITypedItem()
        {
        }

        public string LandmarkName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ResolvedLatitude { get; set; }
        public double ResolvedLongitude { get; set; }
        public string CategoryName { get; set; }
        public int Radius { get; set; }

        public string Description { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public short TimeZone { get; set; }
        public string Address { get; set; }
        public string ResolvedAddress { get; set; }
        public bool DayLightSaving { get; set; }
        public bool AutoAdjustDayLightSaving { get; set; }
        public bool Public { get; set; }
    }

    public static class LandmarkDIColumns
    {
        public static string LandmarkName = "LandmarkName";
        public static string Latitude = "Latitude";
        public static string Longitude = "Longitude";
        public static string CategoryName = "CategoryName";
        public static string Radius = "Radius";

        public static string Description = "Description";
        public static string ContactPersonName = "ContactPersonName";
        public static string ContactPhoneNumber = "ContactPhoneNumber";
        public static string Email = "Email";
        public static string Phone = "Phone";

        public static string TimeZone = "TimeZone";
        public static string Address = "Address";
        public static string DayLightSaving = "DayLightSaving";
        public static string AutoAdjustDayLightSaving = "AutoAdjustDayLightSaving";
        public static string Public = "Public";

    }


}
