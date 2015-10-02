using System;
using System.Data ;
using VLF.CLS;
using System.IO;

using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsGeoZone.
	/// </summary>
	public class clsGeoZone
	{
		private string description="";
		private string name="";
		private string oldname="";
		private short direction=0;
		private short severity=0;
		private bool addMode=false;
		private bool editMode=false;
		private bool optMap=false;
	    private DataSet dsGeoZone=null;
		private DataSet dsGeoDetails =new DataSet() ;
		private DataSet dsVehicleGeoZone=new DataSet();
		private DataSet dsUnAssVehicleGeoZoneClone=new DataSet();
		private DataSet dsAssVehicleGeoZoneClone=new DataSet();
		private DataSet dsUnAssVehicleGeoZone=null;
		private Int32 geozoneId=0;
		private bool setGeoZone=false;
		private string selectLayer="trans";
		private int selectedFleetId=0;
		private Int64 selectedVehicleId=0;
		private VLF.CLS.Def.Enums.GeozoneType geozoneTypeId=VLF.CLS.Def.Enums.GeozoneType.Rectangle;
		public const string GeoZoneSync="Sync";
		public const string GeoZoneFailed="Failed";
		public const string AddPendingGeoZone="Add Pending";
		public const string DeletePendingGeoZone="Delete Pending";
		public const string FailedAddPendingGeoZone="Failed-Add Geozone";
		public const string FailedDeletePendingGeoZone="Failed-Delete Geozone";
		private string imgPath="";
		private bool isAssigned=false;
		private bool isGeoZoneComplete=false;
		private bool showEditGeoZoneTable=false;
        

		public bool IsGeoZoneComplete
		{
			get{return isGeoZoneComplete;}
			set{isGeoZoneComplete = value;}
		}


		public bool ShowEditGeoZoneTable
		{
			get{return showEditGeoZoneTable;}
			set{showEditGeoZoneTable = value;}
		}

		public VLF.CLS.Def.Enums.GeozoneType GeozoneTypeId
		{
			get{return geozoneTypeId;}
			set{geozoneTypeId = value;}
		}


		public DataSet DsGeoDetails
		{
			get{return dsGeoDetails;}
			set{dsGeoDetails = value;}
		}

		public bool IsAssigned
		{
			get{return isAssigned;}
			set{isAssigned = value;}
		}

		public string ImgPath
		{
			get{return imgPath;}
			set{imgPath = value;}
		}


		private string imgConfPath="";

		public string ImgConfPath
		{
			get{return imgConfPath;}
			set{imgConfPath = value;}
		}

		public int SelectedFleetId
		{
			get{return selectedFleetId;}
			set{selectedFleetId = value;}
		}


		public Int64 SelectedVehicleId
		{
			get{return selectedVehicleId;}
			set{selectedVehicleId = value;}
		}



		public string SelectLayer
		{
			get{return selectLayer;}
			set{selectLayer = value;}
		}

		public bool SetGeoZone
		{
			get{return setGeoZone;}
			set{setGeoZone = value;}
		}

		public Int32 GeozoneId
		{
			get{return geozoneId;}
			set{geozoneId = value;}
		}

		
		public DataSet DsUnAssVehicleGeoZone
		{
			get{return dsUnAssVehicleGeoZone;}
			set{dsUnAssVehicleGeoZone = value;}
		}


		public DataSet DsAssVehicleGeoZoneClone
		{
			get{return dsAssVehicleGeoZoneClone;}
			set{dsAssVehicleGeoZoneClone = value;}
		}

		public DataSet DsUnAssVehicleGeoZoneClone
		{
			get{return dsUnAssVehicleGeoZoneClone;}
			set{dsUnAssVehicleGeoZoneClone = value;}
		}

		public DataSet DsGeoZone
		{
			get{return dsGeoZone;}
			set{dsGeoZone = value;}
		}

    

		public DataSet DsVehicleGeoZone
		{
			get{return dsVehicleGeoZone;}
			set{dsVehicleGeoZone = value;}
		}

		public bool OptMap
		{
			get{return optMap;}
			set{optMap = value;}
		}

		public bool AddMode
		{
			get{return addMode;}
			set{addMode = value;}
		}

		public bool EditMode
		{
			get{return editMode;}
			set{editMode = value;}
		}

		public string Description
		{
			get{return description;}
			set{description = value;}
		}

		public string Name
		{
			get{return name;}
			set{name = value;}
		}


		public string OldName
		{
			get{return oldname;}
			set{oldname = value;}
		}

		public short Direction
		{
			get{return direction;}
			set{direction = value;}
		}

		public short Severity
		{
			get{return severity;}
			set{severity = value;}
		}

		public clsGeoZone()
		{
			dsVehicleGeoZone.Tables.Add();  

			DataColumn VehicleId = new DataColumn("VehicleId",Type.GetType("System.Int64"));
			VehicleId.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(VehicleId);

			DataColumn GeozoneNo = new DataColumn("GeozoneNo",Type.GetType("System.Int32"));
			GeozoneNo.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(GeozoneNo);

			DataColumn SeverityId = new DataColumn("SeverityId",Type.GetType("System.Int32"));
			SeverityId.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(SeverityId);

			DataColumn OrganizationId = new DataColumn("OrganizationId",Type.GetType("System.Int32"));
			OrganizationId.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(OrganizationId);

			DataColumn GeozoneId = new DataColumn("GeozoneId",Type.GetType("System.Int32"));
			GeozoneId.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(GeozoneId);


			DataColumn GeozoneName = new DataColumn("GeozoneName",Type.GetType("System.String"));
			GeozoneName.DefaultValue=""; 
			dsVehicleGeoZone.Tables[0].Columns.Add(GeozoneName);


			DataColumn colType = new DataColumn("Type",Type.GetType("System.Int32"));
			colType.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(colType);

			DataColumn Description = new DataColumn("Description",Type.GetType("System.String"));
			Description.DefaultValue=""; 
			dsVehicleGeoZone.Tables[0].Columns.Add(Description);

			DataColumn BoxId = new DataColumn("BoxId",Type.GetType("System.Int32"));
			BoxId.DefaultValue=-1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(BoxId);


			DataColumn Assigned = new DataColumn("Assigned",Type.GetType("System.Boolean"));
			Assigned.DefaultValue =false;
			dsVehicleGeoZone.Tables[0].Columns.Add(Assigned);
			

			DataColumn Severity = new DataColumn("Severity",Type.GetType("System.String"));
			Severity.DefaultValue=""; 
			dsVehicleGeoZone.Tables[0].Columns.Add(Severity);
			
			DataColumn Status = new DataColumn("Status",Type.GetType("System.String"));
			Status.DefaultValue=""; 
			dsVehicleGeoZone.Tables[0].Columns.Add(Status);


			DataColumn GeozoneType = new DataColumn("GeozoneType",Type.GetType("System.Int16"));
			GeozoneType.DefaultValue=1; 
			dsVehicleGeoZone.Tables[0].Columns.Add(GeozoneType);
			

			
			
			dsGeoDetails.Tables.Add();   

			 
			DataColumn colLatitude = new DataColumn("Latitude",Type.GetType("System.Double"));
			colLatitude.DefaultValue=0; 
			dsGeoDetails.Tables[0].Columns.Add(colLatitude);

			DataColumn colLongitude = new DataColumn("Longitude",Type.GetType("System.Double"));
			colLongitude.DefaultValue=0; 
			dsGeoDetails.Tables[0].Columns.Add(colLongitude);


			DataColumn colSequenceNum = new DataColumn("SequenceNum",Type.GetType("System.Int32"));
			colSequenceNum.DefaultValue=0; 
			dsGeoDetails.Tables[0].Columns.Add(colSequenceNum);

			
		}

        // Changes for TimeZone Feature start

        public void DsGeoZone_Fill_NewTZ(SentinelFMSession sn)
        {
            clsUtility objUtil = new clsUtility(sn);
            StringReader strrXML = null;
            DataSet dsGeoZone = new DataSet();
            DataSet dsGeoZoneDetails;

            string xml = "";
            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            /*if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesWithStatus(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesWithStatus(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                {
                   sn.GeoZone.DsGeoZone = null;
                }
            */
            DataSet _geozones = null;
            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            using (VLF.PATCH.Logic.PatchOrganizationGeozone pog = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString))
            {
                _geozones = pog.PatchGetOrganizationGeozonesWithStatus_NewTZ(sn.User.OrganizationId, sn.UserID);

                if (Util.IsDataSetValid(_geozones))
                {
                    xml = clsXmlUtil.GetXmlIncludingNull(_geozones);
                }
            }


            if (xml == "")
            {
                sn.GeoZone.DsGeoZone = null;
                return;
            }



            strrXML = new StringReader(xml);
            dsGeoZone.ReadXml(strrXML);


            // Show DirectionName
            DataColumn dc = new DataColumn();
            dc.ColumnName = "DirectionName";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsGeoZone.Tables[0].Columns.Add(dc);




            short enumId = 0;
            string[] Sen = new string[1];


            foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
            {

                enumId = Convert.ToInt16(rowItem["Type"]);
                rowItem["DirectionName"] = Enum.GetName(typeof(VLF.CLS.Def.Enums.GeoZoneDirection), (VLF.CLS.Def.Enums.GeoZoneDirection)enumId);


                if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(rowItem["GeoZoneId"]), ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(rowItem["GeoZoneId"]), ref xml), true))
                    {
                        continue;
                    }

                if (xml == "")
                    continue;

                dsGeoZoneDetails = new DataSet();
                strrXML = new StringReader(xml);
                dsGeoZoneDetails.ReadXml(strrXML);
                dsGeoZoneDetails.Tables[0].TableName = rowItem["GeoZoneId"].ToString();
                sn.GeoZone.DsGeoDetails.Tables.Add(dsGeoZoneDetails.Tables[0].Copy());
            }

            sn.GeoZone.DsGeoZone = dsGeoZone;

        }

        // Changes for TimeZone Feature end


        public  void DsGeoZone_Fill(SentinelFMSession sn)
		{
                clsUtility objUtil = new clsUtility(sn);
                StringReader strrXML = null;
                DataSet dsGeoZone = new DataSet();
                DataSet dsGeoZoneDetails;
            
                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                /*if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesWithStatus(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesWithStatus(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                       sn.GeoZone.DsGeoZone = null;
                    }
                */
                DataSet _geozones = null;
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                using (VLF.PATCH.Logic.PatchOrganizationGeozone pog = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString))
                {
                    _geozones = pog.PatchGetOrganizationGeozonesWithStatus(sn.User.OrganizationId, sn.UserID);

                    if (Util.IsDataSetValid(_geozones))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_geozones);
                    }
                }

                
                if (xml == "")
                {
                   sn.GeoZone.DsGeoZone = null;
                    return;
                }



                strrXML = new StringReader(xml);
                dsGeoZone.ReadXml(strrXML);


                // Show DirectionName
                DataColumn dc = new DataColumn();
                dc.ColumnName = "DirectionName";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsGeoZone.Tables[0].Columns.Add(dc);




                short enumId = 0;
                string[] Sen = new string[1];


                foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                {

                    enumId = Convert.ToInt16(rowItem["Type"]);
                    rowItem["DirectionName"] = Enum.GetName(typeof(VLF.CLS.Def.Enums.GeoZoneDirection), (VLF.CLS.Def.Enums.GeoZoneDirection)enumId);


                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(rowItem["GeoZoneId"]), ref xml), false))
                        if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(rowItem["GeoZoneId"]), ref xml), true))
                        {
                            continue;
                        }

                    if (xml == "")
                        continue;

                    dsGeoZoneDetails = new DataSet();
                    strrXML = new StringReader(xml);
                    dsGeoZoneDetails.ReadXml(strrXML);
                    dsGeoZoneDetails.Tables[0].TableName = rowItem["GeoZoneId"].ToString();
                   sn.GeoZone.DsGeoDetails.Tables.Add(dsGeoZoneDetails.Tables[0].Copy());
                }

               sn.GeoZone.DsGeoZone = dsGeoZone;

        }
	}
}
