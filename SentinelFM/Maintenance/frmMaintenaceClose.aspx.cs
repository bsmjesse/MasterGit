using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SentinelFM
{
    public partial class Maintenance_frmMaintenaceClose : SentinelFMBasePage 
    {
        ServerDBVehicle.DBVehicle dbVehicle = new ServerDBVehicle.DBVehicle();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    this.lblVehicleId.Text = Request.QueryString["VehicleId"];
                    this.lblServiceId.Text = Request.QueryString["ServiceId"];
                    DataRow[] drCollections = null;

                    this.txtFromDate.Visible = false;
                    this.txtFromDate.Enabled = false;
                    this.txtMaintenaceValue.Visible = false;
                    this.txtMaintenaceValue.Enabled = false;
                    
                    drCollections = sn.History.DsVehicleDueServices.Tables[0].Select("VehicleId='" + lblVehicleId.Text + "' and ServiceID='" + lblServiceId.Text+"'", "", DataViewRowState.CurrentRows);
                    this.lblMaintenanceType.Text = drCollections[0]["OperationTypeDescription"].ToString();
                    this.lblOperationTypeId.Text = drCollections[0]["OperationTypeId"].ToString();
                    this.lblServiceInterval.Text = drCollections[0]["ServiceInterval"].ToString();
                    this.lblDueValue.Text = drCollections[0]["DueServiceValue"].ToString();

                    if (drCollections[0]["OperationTypeId"].ToString() =="1") 
                    {
                        txtMaintenaceValue.Text =Convert.ToString(Convert.ToInt32(drCollections[0]["DueServiceValue"]) + Convert.ToInt32(this.lblServiceInterval.Text));
                        this.lblCurrentValue.Text = drCollections[0]["CurrentOdo"].ToString(); 
                        this.txtMaintenaceValue.Visible = true;
                    }
                    else if (drCollections[0]["OperationTypeId"].ToString() =="2") //Engine Hours
                    {
                        txtMaintenaceValue.Text =Convert.ToString(Convert.ToInt32(drCollections[0]["DueServiceValue"]) + Convert.ToInt32(this.lblServiceInterval.Text));
                        this.lblCurrentValue.Text = ((int)Math.Floor(Convert.ToDouble(drCollections[0]["CurrentEngHrs"]) / 60)).ToString();
                        this.txtMaintenaceValue.Visible = true;
                    }
                    else if (drCollections[0]["OperationTypeId"].ToString()  == "3") //Time Based
                    {
                        txtFromDate.Text = CalculateNextDateValue(Convert.ToDateTime(drCollections[0]["DueServiceValue"]), Convert.ToInt32(lblServiceInterval.Text)).ToShortDateString() ;
                        this.txtFromDate.Visible = true;
                    }
                }
                catch {}
            }
        }

        protected void optMaintenance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lblOperationTypeId.Text == "1" || lblOperationTypeId.Text == "2")
            {
                this.txtMaintenaceValue.Visible = true;
                this.txtFromDate.Visible = false;
            }
            else if (lblOperationTypeId.Text == "3") //Time Based
            {
                this.txtMaintenaceValue.Visible = false;
                this.txtFromDate.Visible = true;
            }
            
            switch (optMaintenance.SelectedValue)
            {
                case "0":
                    this.txtMaintenaceValue.Enabled = false;
                    this.txtFromDate.Enabled = false;

                    if (lblOperationTypeId.Text == "1" || lblOperationTypeId.Text == "2")
                        txtMaintenaceValue.Text =Convert.ToString( Convert.ToInt32(this.lblDueValue.Text)+Convert.ToInt32(lblServiceInterval.Text)) ;
                    else if (lblOperationTypeId.Text == "3") //Time Based
                        txtFromDate.Text = CalculateNextDateValue(Convert.ToDateTime(this.lblDueValue.Text), Convert.ToInt32(lblServiceInterval.Text)).ToShortDateString() ;
                    break;
                case "1":
                    this.txtMaintenaceValue.Enabled = false;
                    this.txtFromDate.Enabled = false;

                    if (lblOperationTypeId.Text == "1" || lblOperationTypeId.Text == "2")
                        txtMaintenaceValue.Text =Convert.ToString(Convert.ToInt32(this.lblCurrentValue.Text) + Convert.ToInt32(lblServiceInterval.Text));
                    else if (lblOperationTypeId.Text == "3") //Time Based
                        txtFromDate.Text = CalculateNextDateValue(System.DateTime.Now, Convert.ToInt32(lblServiceInterval.Text)).ToShortDateString() ;
                    break;
                case "2":
                    this.txtMaintenaceValue.Enabled = true ;
                    this.txtFromDate.Enabled = true;

                    if (lblOperationTypeId.Text == "1" || lblOperationTypeId.Text == "2")
                        txtMaintenaceValue.Text = Convert.ToString(Convert.ToInt32(this.lblDueValue.Text)+Convert.ToInt32(lblServiceInterval.Text)) ;
                    else if (lblOperationTypeId.Text == "3") //Time Based
                        txtFromDate.Text  = CalculateNextDateValue(Convert.ToDateTime(this.lblDueValue.Text), Convert.ToInt32(lblServiceInterval.Text)).ToShortDateString() ;
                    break;
            }          
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int ServiceValue = 0;

            if (this.lblOperationTypeId.Text != "3")
            {
                if (optMaintenance.SelectedValue != "0")
                {
                    if (!clsUtility.IsNumeric(this.txtMaintenaceValue.Text))
                    {
                        this.lblMessage.Text = "Value should be numeric";
                        return;
                    }
                    if (Convert.ToInt32(lblCurrentValue.Text) > Convert.ToInt32((txtMaintenaceValue.Text)))
                    {
                        this.lblMessage.Text = "New value should be greater than current";
                        return;
                    }
                }

                if (this.lblOperationTypeId.Text == "2")
                    ServiceValue = Convert.ToInt32(txtMaintenaceValue.Text)*60;
                else
                    ServiceValue = Convert.ToInt32(txtMaintenaceValue.Text);
            }
            else
            {
                //if (Convert.ToDateTime(System.DateTime.Now.ToShortDateString())  >= Convert.ToDateTime((this.txtFromDate.Text)))
                //{
                //    this.lblMessage.Text = "New value should be greater than current date";
                //    return;
                //}

                //DateTime dtStart;
                //if (optMaintenance.SelectedValue == "0")
                //    dtStart = Convert.ToDateTime(this.lblDueValue.Text);
                //else
                //    dtStart = Convert.ToDateTime(this.txtFromDate.Text);

                DateTime dtAdj = new DateTime(2000, 1, 1);
                TimeSpan ts = new TimeSpan();
                //DateTime dtNew = new DateTime(); ;
                DateTime dtStart = Convert.ToDateTime(this.txtFromDate.Text);

                ts = dtStart - dtAdj;
                ServiceValue = Convert.ToInt32(ts.TotalDays);

                //if (Convert.ToInt32(this.lblServiceInterval.Text) < 1000)//Weekly
                //    {
                //        Interval = Convert.ToInt32(this.lblServiceInterval.Text) - 100;
                //        AdjDateWeek(ref dtStart, Convert.ToInt16(Interval));
                //        ts = dtStart - dtAdj;
                //        ServiceValue = Convert.ToInt32(ts.TotalDays);
                //    }
                //    else if ((Convert.ToInt32(this.lblServiceInterval.Text) > 1000) && (Convert.ToInt32(this.lblServiceInterval.Text) < 2000)) //Monthly
                //    {
                //        Interval = Convert.ToInt32(this.lblServiceInterval.Text) - 1000;
                //        if (Interval != 32)
                //            dtNew = new DateTime(dtStart.Year, dtStart.Month, Interval);
                //        else
                //            dtNew = LastDayOfMonthFromDateTime(dtStart); ;

                //        ts = dtNew - dtAdj;
                //        ServiceValue = Convert.ToInt32(ts.TotalDays);
                //    }
                //    else if (Convert.ToInt32(this.lblServiceInterval.Text) > 2000)  //Yearly
                //    {
                //        Interval = Convert.ToInt32(this.lblServiceInterval.Text) - 2000;
                //        dtNew = new DateTime(dtStart.Year, Interval, 1); 
                //        if (dtNew < System.DateTime.Now)
                //            dtNew = dtNew.AddYears(1);


                //        ts = dtNew - dtAdj;
                //        ServiceValue = Convert.ToInt32(ts.TotalDays);
                //    }               
            }
                    
            try
            {
                if (objUtil.ErrCheck(dbVehicle.VehicleMaintenancePlanExtended_Close(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt64(lblVehicleId.Text), Convert.ToInt32(lblServiceId.Text), ServiceValue, Convert.ToInt16(this.optMaintenance.SelectedValue), this.txtComments.Text), false))
                    if (objUtil.ErrCheck(dbVehicle.VehicleMaintenancePlanExtended_Close(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt64(lblVehicleId.Text), Convert.ToInt32(lblServiceId.Text), ServiceValue, Convert.ToInt16(this.optMaintenance.SelectedValue), this.txtComments.Text), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " VehicleMaintenancePlanExtended_Close . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }
               // this.lblMessage.Text = "Maintenance plan closed successfully";  
                Response.Write("<script language='javascript'>window.close()</script>");
            }
           catch (Exception Ex)
             {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
             }
        }

        private DateTime AdjDateWeek(ref DateTime dtStart,Int16 DayOfWeek )
        {
            int dayDiff = 0;
            dayDiff = Convert.ToInt16(dtStart.DayOfWeek) - DayOfWeek;
            if (dayDiff > 0)
            {
                dtStart = dtStart.AddDays(7 - dayDiff);
            }
            else if (dayDiff < 0)
            {
                dtStart = dtStart.AddDays(-dayDiff);
            }
            return dtStart;
        }

        public DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        private DateTime  CalculateNextDateValue(DateTime dtStart, Int32 ServiceInterval)
        {
            TimeSpan ts = new TimeSpan();
            DateTime dtNew = new DateTime();
            Int32 Interval = 0;

            if (ServiceInterval < 1000)//Weekly
            {
                Interval = ServiceInterval - 100;
                AdjDateWeek(ref dtStart, Convert.ToInt16(Interval));
            }
            else if (ServiceInterval > 1000 && ServiceInterval < 2000) //Monthly
            {
                Interval = ServiceInterval - 1000;
                if (Interval != 32)
                    dtNew = new DateTime(dtStart.Year, dtStart.Month, Interval);
                else
                    dtNew = LastDayOfMonthFromDateTime(dtStart); ;

                dtStart = dtNew;
            }
            else if (ServiceInterval > 2000)  //Yearly
            {
                Interval = ServiceInterval - 2000;
                dtNew = new DateTime(dtStart.Year, Interval, 1);
                if (dtNew < System.DateTime.Now)
                    dtNew = dtNew.AddYears(1);

                dtStart = dtNew;
            }
            return dtStart;

        }
    }
}
