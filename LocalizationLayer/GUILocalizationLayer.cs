using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace LocalizationLayer
{
    public class GUILocalizationLayer
    {
        public static string LocalizeReportData(string data, string lang)
        {
            Const.Culture = new CultureInfo(lang);

            return data.Replace(VLF.CLS.Def.Const.addressNA, Const.InvalidAddress_addressNA)
                       .Replace(VLF.CLS.Def.Const.noGPSData, Const.InvalidAddress_noGPSData)
                       .Replace(VLF.CLS.Def.Const.noValidAddress, Const.InvalidAddress_noValidAddress)
                       .Replace("NoAlarm", Const.AlarmSeverity_NoAlarm)
                       .Replace("Notify", Const.AlarmSeverity_Notify)
                       .Replace("Warning", Const.AlarmSeverity_Warning)
                       .Replace("Critical", Const.AlarmSeverity_Critical);
        }



        public static string LocalizeSeverity(string data, string lang)
        {
            Const.Culture = new CultureInfo(lang);

            return data.Replace("NoAlarm", Const.AlarmSeverity_NoAlarm)
                       .Replace("Notify", Const.AlarmSeverity_Notify)
                       .Replace("Warning", Const.AlarmSeverity_Warning)
                       .Replace("Critical", Const.AlarmSeverity_Critical);
        }

       


        public static string LocalizeAlarmState(string data, string lang)
        {
            Const.Culture = new CultureInfo(lang);

            return data.Replace("New", Const.AlarmState_New )
                       .Replace("Accepted", Const.AlarmState_Accepted )
                       .Replace("Closed", Const.AlarmState_Closed )
                       .Replace("Unknown", Const.AlarmState_Unknown);
        }

        public static string LocalizeAlarms(string alarms, string lang)
        {
            Const.Culture = new CultureInfo(lang);

            // BE CAREFUL OF REPLACE ORDER (e.g. replace "Speeding" before "Speed")

            return alarms.Replace("Alarm", Const.MessageType_Alarm)
                         .Replace("BadSensor", Const.MessageType_BadSensor)
                         .Replace("Coordinate", Const.MessageType_Coordinate)
                         .Replace("Door", Const.MessageType_Door)
                         .Replace("Duration", Const.MessageType_Duration)
                         .Replace("ExtremeAcceleration", Const.MessageType_ExtremeAcceleration)
                         .Replace("ExtremeBraking", Const.MessageType_ExtremeBraking)
                         .Replace("GeoFence", Const.MessageType_GeoFence)
                         .Replace("GPSAntennaOK", Const.MessageType_GPSAntennaOK)
                         .Replace("GPSAntennaOpen", Const.MessageType_GPSAntennaOpen)
                         .Replace("GPSAntennaShort", Const.MessageType_GPSAntennaShort)
                         .Replace("HarshAcceleration", Const.MessageType_HarshAcceleration)
                         .Replace("HarshBraking", Const.MessageType_HarshBraking)
                         .Replace("Idling", Const.MessageType_Idling)
                         .Replace("Ignition", Const.MessageType_Ignition)
                         .Replace("KeyFobArm", Const.MessageType_KeyFobArm)
                         .Replace("KeyFobDisarm", Const.MessageType_KeyFobDisarm)
                         .Replace("KeyFobPanic", Const.MessageType_KeyFobPanic)
                         .Replace("Main Battery", Const.MessageType_Main_Battery)
                         .Replace("Passenger In Seat", Const.MessageType_Passenger_In_Seat)
                         .Replace("SeatBelt", Const.MessageType_SeatBelt)
                         .Replace("Sensor", Const.MessageType_Sensor)
                         .Replace("Speeding", Const.MessageType_Speeding)
                         .Replace("Speed", Const.MessageType_Speed)
                         .Replace("Status", Const.MessageType_Status)
                         .Replace("Trunk", Const.MessageType_Trunk)
                         .Replace("StoredPosition", Const.MessageType_StoredPosition)
                         .Replace("Low", Const.MessageType_Low)
                         .Replace("Off", Const.MessageType_Off)
                         .Replace("On", Const.MessageType_On)
                         .Replace("Open", Const.MessageType_Open)
                         .Replace("Yes", Const.MessageType_Yes)
                         .Replace("In", Const.Alarm_In)
                         .Replace("Out", Const.Alarm_Out)
                         .Replace("DTC codes", Const.Alarm_DTCcodes)
                         .Replace("Tamper Alert", Const.Alarm_TamperAlert)
                         .Replace("Motion Sensor", Const.Alarm_MotionSensor )
                         .Replace("Rear Hopper Door", Const.Alarm_RearHopperDoor)
                         .Replace("Leash Broken", Const.Alarm_LeashBroken)
                         .Replace("Power Connected / Tamper", Const.Alarm_PowerConnected); 
        }

        public static string LocalizeViolations(string violations, string lang)
        {
            Const.Culture = new CultureInfo(lang);

            // BE CAREFUL OF REPLACE ORDER (e.g. replace "Speed Violation" before "Speed")

            return violations.Replace("Extreme Acceleration", Const.Violation_Acc_Extreme)
                             .Replace("Harsh Acceleration", Const.Violation_Acc_Harsh)
                             .Replace("Extreme Braking", Const.Violation_Braking_Extreme)
                             .Replace("Harsh Braking", Const.Violation_Braking_Harsh)
                             .Replace("DURATION", Const.Violation_DURATION)
                             .Replace("Seat Belt Violation", Const.Violation_Seat_Belt)
                             .Replace("Speed Violation", Const.Violation_Speed_Violation)
                             .Replace("Speed", Const.Violation_Speed);
        }

        

        //public static string LocalizeAlarmSeverity(string alarms, string lang)
        //{
        //    Const.Culture = new CultureInfo(lang);

        //    return alarms.Replace("NoAlarm", Const.AlarmSeverity_NoAlarm)
        //                 .Replace("Notify", Const.AlarmSeverity_Notify)
        //                 .Replace("Warning", Const.AlarmSeverity_Warning)
        //                 .Replace("Critical", Const.AlarmSeverity_Critical);
        //}

        public static void GUILocalizeForm(ref HtmlForm frm, string lang)
        {
            if (lang == "en")
                return;

            Control temp;

            foreach (Control ctrl in frm.Controls)
            {
                try
                {
                    temp = ctrl;

                    if (ctrl.HasControls())
                        GUILocalizeObject(ref temp);

                    if (ctrl.GetType() == typeof(Button))
                        GUILocalizeButton(ref temp);

                    else if (ctrl.GetType() == typeof(LinkButton))
                        GUILocalizeLinkButton(ref temp);

                    else if (ctrl.GetType() == typeof(CheckBox))
                        GUILocalizeCheckBox(ref temp);

                    //else if (ctrl.GetType() == typeof(Menu))
                    //    GUILocalizeMenu(ref temp);
                }

                catch
                {
                    continue;
                }
            }
        }

        private static void GUILocalizeObject(ref Control obj)
        {
            Control temp;

            foreach (Control ctrl in obj.Controls)
            {
                try
                {
                    temp = ctrl;

                    if (ctrl.HasControls())
                        GUILocalizeObject(ref temp);

                    if (ctrl.GetType() == typeof(Button))
                        GUILocalizeButton(ref temp);

                    else if (ctrl.GetType() == typeof(LinkButton))
                        GUILocalizeLinkButton(ref temp);

                    else if (ctrl.GetType() == typeof(CheckBox))
                        GUILocalizeCheckBox(ref temp);
                }

                catch
                {
                    continue;
                }
            }
        }

        private static void GUILocalizeButton(ref Control ctrl)
        {
            Button btn = (Button)ctrl;
            int oldWidth, textSize, newWidth;

            if (!btn.Width.IsEmpty)
                oldWidth = (int)btn.Width.Value;

            else
            {
                switch (btn.CssClass)
                {
                    case "Commands":
                        oldWidth = 150;
                        break;

                    case "combutton":
                        oldWidth = 105;
                        break;

                    case "confbutton":
                        oldWidth = 95;
                        break;

                    case "selectedbutton":
                        oldWidth = 95;
                        break;

                    default:
                        return;
                }
            }

            if (!btn.Font.Size.IsEmpty)
                textSize = (int)(btn.Font.Size.Unit.Value * (btn.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1));

            else
            {
                switch (btn.CssClass)
                {
                    case "Commands":
                        textSize = 10;
                        break;

                    case "combutton":
                        textSize = 10;
                        break;

                    case "confbutton":
                        textSize = 11;
                        break;

                    case "selectedbutton":
                        textSize = 11;
                        break;

                    default:
                        return;
                }
            }

            newWidth = (int)(textSize * btn.Text.Length * 0.65 + btn.Text.Length);

            if (newWidth > oldWidth)
                btn.Width = newWidth;
        }

        /*
        private static void GUILocalizeButton(ref Control ctrl)
        {
            Button btn = (Button)ctrl;
            int count; // FORMULA = WIDTH(px) / FONT-SIZE(px)

            if (btn.ToolTip != "")
                return; 

            btn.ToolTip = btn.Text;

            // WIDTH:
            if (!btn.Width.IsEmpty)
                count = (int)btn.Width.Value;

            else
            {
                switch (btn.CssClass)
                {
                    case "Commands":
                        count = 150;
                        break;

                    case "combutton":
                        count = 105;
                        break;

                    case "confbutton":
                        count = 95;
                        break;

                    case "selectedbutton":
                        count = 95;
                        break;

                    default:
                        return;
                }
            }

            // FONT-SIZE:
            if (!btn.Font.Size.IsEmpty)
                count /= (int)(btn.Font.Size.Unit.Value * (btn.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1)); // 1px = 0.75pt

            else
            {
                switch (btn.CssClass)
                {
                    case "Commands":
                        count /= 10;
                        break;

                    case "combutton":
                        count /= 10;
                        break;

                    case "confbutton":
                        count /= 11;
                        break;

                    case "selectedbutton":
                        count /= 11;
                        break;

                    default:
                        return;
                }
            }

            count += 3; // Adjustment

            if (btn.Text.Length > count + 1)
                btn.Text = btn.Text.Substring(0, count) + "...";
        }
        */

        private static void GUILocalizeLinkButton(ref Control ctrl)
        {
            LinkButton lb = (LinkButton)ctrl;
            int oldWidth, textSize, newWidth;

            if (!lb.Width.IsEmpty)
                oldWidth = (int)lb.Width.Value;

            else
                return;

            if (!lb.Font.Size.IsEmpty)
                textSize = (int)(lb.Font.Size.Unit.Value * (lb.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1));

            else
                switch (lb.CssClass)
                {
                    case "mainlink":
                        textSize = 11;
                        break;

                    default:
                        return;
                }

            newWidth = (int)(textSize * lb.Text.Length * 0.65 + lb.Text.Length);

            if (newWidth > oldWidth)
                lb.Width = newWidth;
        }

        /*
        private static void GUILocalizeLinkButton(ref Control ctrl)
        {
            LinkButton lb = (LinkButton)ctrl;
            int count;

            if (lb.ToolTip != "")
                return; 

            lb.ToolTip = lb.Text;

            if (!lb.Width.IsEmpty)
                count = (int)lb.Width.Value;

            else
            {
                return;
            }

            if (!lb.Font.Size.IsEmpty)
                count /= (int)(lb.Font.Size.Unit.Value * (lb.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1));

            else
            {
                switch (lb.CssClass)
                {
                    case "mainlink":
                        count /= 11;
                        break;

                    default:
                        return;
                }
            }

            count += 3;

            if (lb.Text.Length > count + 1)
                lb.Text = lb.Text.Substring(0, count) + "...";
        }
        */

        private static void GUILocalizeCheckBox(ref Control ctrl)
        {
            CheckBox cb = (CheckBox)ctrl;
            int oldWidth, textSize, newWidth;

            if (!cb.Width.IsEmpty)
                oldWidth = (int)cb.Width.Value;

            else
                return;

            if (!cb.Font.Size.IsEmpty)
                textSize = (int)(cb.Font.Size.Unit.Value * (cb.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1));

            else
                switch (cb.CssClass)
                {
                    case "formtext":
                        textSize = 11;
                        break;

                    default:
                        return;
                }

            newWidth = (int)(textSize * cb.Text.Length * 0.65 + cb.Text.Length);

            if (newWidth > oldWidth)
                cb.Width = newWidth;
        }

        /*
        private static void GUILocalizeCheckBox(ref Control ctrl)
        {
            CheckBox cb = (CheckBox)ctrl;
            int count;

            if (cb.ToolTip != "")
                return; 


            cb.ToolTip = cb.Text;

            if (!cb.Width.IsEmpty)
                count = (int)cb.Width.Value;

            else
            {
                return;
            }

            if (!cb.Font.Size.IsEmpty)
                count /= (int)(cb.Font.Size.Unit.Value * (cb.Font.Size.Unit.Type == UnitType.Point ? 0.75 : 1));

            else
            {
                switch (cb.CssClass)
                {
                    case "formtext":
                        count /= 11;
                        break;

                    default:
                        return;
                }
            }

            if (cb.Text.Length > count + 1)
                cb.Text = cb.Text.Substring(0, count) + "...";
        }
        */

        private static void GUILocalizeMenu(ref Control ctrl)
        {
            Menu mnu = (Menu)ctrl;

            foreach (MenuItem mi in mnu.Items)
            {
                if (mi.ToolTip != "")
                    continue;

                mi.ToolTip = mi.Text;

                if (mi.Text.Length > 15)
                    mi.Text = mi.Text.Substring(0, 15) + "...";
            }
        }
    }
}
