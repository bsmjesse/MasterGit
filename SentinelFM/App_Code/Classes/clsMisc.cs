using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;


/// <summary>
/// Summary description for clsMisc
/// </summary>
public class clsMisc
{

    private Int32 confVehiclesSelectedGridPage = 0;
    public Int32 ConfVehiclesSelectedGridPage
    {
        get { return confVehiclesSelectedGridPage; }
        set { confVehiclesSelectedGridPage = value; }
    }

   private DataSet  confdsvehicles = null;
   public DataSet ConfDsvehicles
    {
       get { return confdsvehicles; }
       set { confdsvehicles = value; }
    }


    private DataSet confdsusers = null;
    public DataSet ConfDsUsers
    {
       get { return confdsusers; }
       set { confdsusers = value; }
    }


    private DataSet dsSearch = null;
    public DataSet DsSearch
    {
        get { return dsSearch; }
        set { dsSearch = value; }
    }

    private DataSet dsReportSelectedFleets = null;
    public DataSet DsReportSelectedFleets
    {
        get { return dsReportSelectedFleets; }
        set { dsReportSelectedFleets = value; }
    }


    private DataSet dsReportAllFleets = null;
    public DataSet DsReportAllFleets
    {
        get { return dsReportAllFleets; }
        set { dsReportAllFleets = value; }
    }


   private DataTable confdtlandmarks = null;
   public DataTable ConfDtLandmarks
    {
       get { return confdtlandmarks; }
       set { confdtlandmarks = value; }
    }

    private Int32 confUsersSelectedGridPage = 0;
    public Int32 ConfUsersSelectedGridPage
    {
       get { return confUsersSelectedGridPage; }
       set { confUsersSelectedGridPage = value; }
    }

    private Int32 landmarkSelectedGridPage = 0;
    public Int32 LandmarkSelectedGridPage
    {
       get { return landmarkSelectedGridPage; }
       set { landmarkSelectedGridPage = value; }
    }


    private Dictionary<int, int> dashBoardFilter = new Dictionary<int, int>();
    public Dictionary<int, int> DashBoardFilter
    {
        get { return dashBoardFilter; }
        set { dashBoardFilter = value; }
    }

    private DataSet dsBoxExtraInfo = null;
    public DataSet DsBoxExtraInfo
    {
        get { return dsBoxExtraInfo; }
        set { dsBoxExtraInfo = value; }
    }

    private DataSet dsAlarmSeverity = null;
    public DataSet DsAlarmSeverity
    {
        get { return dsAlarmSeverity; }
        set { dsAlarmSeverity = value; }
    }

    private DataSet confdsgroupsgrid = null;
    public DataSet ConfDsGroupsGrid
    {
        get { return confdsgroupsgrid; }
        set { confdsgroupsgrid = value; }
    }

    private DataSet confdscontrolsgrid = null;
    public DataSet ConfDsControlsGrid
    {
        get { return confdscontrolsgrid; }
        set { confdscontrolsgrid = value; }
    }

    private DataTable dtgrid = null;
    public DataTable DtGrid
    {
        get { return dtgrid; }
        set { dtgrid = value; }
    }

    private DataSet confdsusergroupcontrolsettings = null;
    public DataSet ConfDsUserGroupControlSettings
    {
        get { return confdsusergroupcontrolsettings; }
        set { confdsusergroupcontrolsettings = value; }
    }

    private DataSet confdsusergroupreportsettings = null;
    public DataSet ConfDsUserGroupReportSettings
    {
        get { return confdsusergroupreportsettings; }
        set { confdsusergroupreportsettings = value; }
    }

    private DataSet confdsusergroupcommandsettings = null;
    public DataSet ConfDsUserGroupCommandSettings
    {
        get { return confdsusergroupcommandsettings; }
        set { confdsusergroupcommandsettings = value; }
    }

    static public void cboHoursFill(ref DropDownList cbo)
    {
        ListItem ls;
        //12 AM
        ls = new ListItem();
        ls.Text = "12" + " AM";
        ls.Value = "0";
        cbo.Items.Add(ls);
        for (int i = 1; i < 12; i++)
        {
            ls = new ListItem();
            if (i < 10)
            {
                ls.Text = "0" + i.ToString() + " AM";
                ls.Value = "0" + i.ToString();
            }
            else
            {
                ls.Text = i.ToString() + " AM";
                ls.Value = i.ToString();
            }
            cbo.Items.Add(ls);
        }


        //12 PM
        ls = new ListItem();
        ls.Text = "12" + " PM";
        ls.Value = "12";
        cbo.Items.Add(ls);
        //---------

        int nextValue = 0;

        for (int i = 1; i < 12; i++)
        {
            ls = new ListItem();
             if (i < 10)
                ls.Text ="0"+ i.ToString() + " PM";
             else
                 ls.Text =  i.ToString() + " PM";

            nextValue = i + 12;
            ls.Value = nextValue.ToString();
            cbo.Items.Add(ls);
        }
    }
    public clsMisc()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}
