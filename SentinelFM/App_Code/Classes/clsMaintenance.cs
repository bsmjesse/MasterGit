using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for clsMaintenance
/// </summary>
public class clsMaintenance
{
    private int selectedFleetID = 0;
    public int SelectedFleetID
    {
        get { return selectedFleetID; }
        set { selectedFleetID = value; }
    }

    private int selectedVehicleID = 0;
    public int SelectedVehicleID
    {
        get { return selectedVehicleID; }
        set { selectedVehicleID = value; }
    }

    private string  selectedServiceOption = "";
    public string  SelectedServiceOption
    {
        get { return selectedServiceOption; }
        set { selectedServiceOption = value; }
    }

    private DataSet dsDTCcodes;
    public DataSet DsDTCcodes
    {
        get { return dsDTCcodes; }
        set { dsDTCcodes = value; }
    }

    public clsMaintenance()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}
