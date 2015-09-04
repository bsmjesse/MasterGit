using System;
using System.Data;
using System.IO;

using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;
 
namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsLandmark.
	/// </summary>
	public class clsLandmark
	{
		private double x=0;
		private double y=0;
		private string refreshFormName="";
		private bool addLandmarkMode=false;
		private bool editLandmarkMode=false;
		private bool landmarkByAddress=true;	
		private string streetAddress="";
		private string landmarkName="";
		private string landmarkDescription="";
		private string contactPhone="";
		private string contactName="";
		private int radius=0;

        //By Devin begin
        private VLF.CLS.Def.Enums.GeozoneType landmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
        private DataTable dsLandmarkDetails = null;
        private DataTable dsLandmarkPointDetails = null;
        public DataTable DsLandmarkPointDetails
        {
            get { return dsLandmarkPointDetails; }
            set { dsLandmarkPointDetails = value; }
        }

        public DataTable DsLandmarkDetails
        {
            get { return dsLandmarkDetails; }
            set { dsLandmarkDetails = value; }
        }
        public VLF.CLS.Def.Enums.GeozoneType LandmarkType
        {
            get { return landmarkType; }
            set { landmarkType = value; }
        }

        //end
		public string LandmarkDescription
		{
			get{return landmarkDescription;}
			set{landmarkDescription = value;}
		}

		public string ContactName
		{
			get{return contactName;}
			set{contactName = value;}
		}

		public string ContactPhone
		{
			get{return contactPhone;}
			set{contactPhone = value;}
		}


		public int Radius
		{
			get{return radius;}
			set{radius = value;}
		}

		public string LandmarkName
		{
			get{return landmarkName;}
			set{landmarkName = value;}
		}

		public string StreetAddress
		{
			get{return streetAddress;}
			set{streetAddress = value;}
		}

		public bool LandmarkByAddress
		{
			get{return landmarkByAddress;}
			set{landmarkByAddress = value;}
		}

		public bool EditLandmarkMode
		{
			get{return editLandmarkMode;}
			set{editLandmarkMode = value;}
		}

		public bool AddLandmarkMode
		{
			get{return addLandmarkMode;}
			set{addLandmarkMode = value;}
		}

		public double X
		{
			get{ return x ; }
			set{ x = value ; }
		}

		public double Y
		{
			get{ return y ; }
			set{ y = value ; }
		}

		public string RefreshFormName
		{
			get{return refreshFormName;}
			set{refreshFormName = value;}
		}

        private DataSet dsWaypoints = null;
        public DataSet DsWaypoints
        {
            get { return dsWaypoints; }
            set { dsWaypoints = value; }
        }

        // Changes for TimeZone Feature start
        public void DgLandmarks_Fill_NewTZ(SentinelFMSession sn)
        {
            try
            {
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                StringReader strrXML = null;
                DataSet dsLandmarks = new DataSet();
                clsUtility objUtil;
                objUtil = new clsUtility(sn);

                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                dbo = new ServerDBOrganization.DBOrganization();

                /*if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
                 */
                DataSet _landmarks = null;

                using (VLF.PATCH.Logic.PatchLandmark pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString))
                {
                    _landmarks = pog.PatchGetLandmarksInfoByOrganizationIdUserId_NewTZ(sn.User.OrganizationId, sn.UserID);

                    if (Util.IsDataSetValid(_landmarks))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_landmarks);
                    }
                }

                if (xml == "")
                {
                    return;
                }


                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);

                sn.DsLandMarks = dsLandmarks;
                // Current Status
                DataColumn dc = new DataColumn("LandmarkId", Type.GetType("System.Int32"));
                dc.DefaultValue = 0;
                sn.DsLandMarks.Tables[0].Columns.Add(dc);
                sn.DsLandmarkPoints = new DataSet();

                foreach (DataRow rowItem in dsLandmarks.Tables[0].Rows)
                {
                    int radius = Convert.ToInt32(rowItem["Radius"]);
                    if (radius == -1 && rowItem["LandmarkName"].ToString() != string.Empty)   // Polygon
                    {
                        VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

                        DataTable dsLandmarkDetails = landPointMgr.GetLandmarkPointSetByLandmarkName(rowItem["LandmarkName"].ToString(), sn.User.OrganizationId).Tables[0];
                        dsLandmarkDetails.TableName = rowItem["LandmarkName"].ToString();
                        sn.DsLandmarkPoints.Tables.Add(dsLandmarkDetails.Copy());
                    }
                }

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString()));
            }
        }
        // Changes for TimeZone Feature end

        public void DgLandmarks_Fill(SentinelFMSession sn)
        {
            try
            {
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                StringReader strrXML = null;
                DataSet dsLandmarks = new DataSet();
                clsUtility objUtil;
                objUtil = new clsUtility(sn);

                string xml = "";
              ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                dbo = new ServerDBOrganization.DBOrganization();

                /*if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
                 */
                DataSet _landmarks = null;
                
                using (VLF.PATCH.Logic.PatchLandmark pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString))
                {
                    _landmarks = pog.PatchGetLandmarksInfoByOrganizationIdUserId(sn.User.OrganizationId, sn.UserID);

                    if (Util.IsDataSetValid(_landmarks))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_landmarks);
                    }
                }

                if (xml == "")
                {
                    return;
                }


                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);

                sn.DsLandMarks = dsLandmarks;
                // Current Status
                DataColumn dc = new DataColumn("LandmarkId", Type.GetType("System.Int32"));
                dc.DefaultValue = 0;
                sn.DsLandMarks.Tables[0].Columns.Add(dc);
                sn.DsLandmarkPoints = new DataSet();
                
                foreach (DataRow rowItem in dsLandmarks.Tables[0].Rows)
                {
                    int radius = Convert.ToInt32(rowItem["Radius"]);
                    if (radius == -1 && rowItem["LandmarkName"].ToString() != string.Empty)   // Polygon
                    {                        
                        VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

                        DataTable dsLandmarkDetails = landPointMgr.GetLandmarkPointSetByLandmarkName(rowItem["LandmarkName"].ToString(), sn.User.OrganizationId).Tables[0];
                        dsLandmarkDetails.TableName = rowItem["LandmarkName"].ToString();
                        sn.DsLandmarkPoints.Tables.Add(dsLandmarkDetails.Copy());                        
                    }                    
                }

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() ));
            }
        }

	}

}


