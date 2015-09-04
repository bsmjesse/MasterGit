using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace VLF.DAS.Logic
{
   public partial class Box
   {
      /// <summary>
      /// Find message pairs where a message is missing
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>HTML table string</returns>
      public string GetMissingMessages(int boxId, DateTime dtFrom, DateTime dtTo)
      {
         StringBuilder html = new StringBuilder("<table>");
         SqlDataReader drMessages = null;
         int counter = 0, resultCounter = 0;
         string hr = "<tr><td><hr /></td></tr>", tr_open = "<tr><td>", tr_close = "</td></tr>";

         try
         {
            drMessages = this.sqlExec.SQLExecuteGetDataReader(
               String.Format("SELECT [vlfMsgInHst].[BoxId], [OriginDateTime], [BoxMsgInTypeId], [CustomProp] FROM [vlfMsgInHst] WHERE [BoxMsgInTypeId] IN (2, 25, 73) AND [OriginDateTime] between '{0}' AND '{1}' AND [vlfMsgInHst].BoxId = {2}",
               dtFrom, dtTo, boxId));

            StateRow storedRowIgnition = null;
            StateRow storedRowPto = null;
            StateRow storedRowIdling = null;

            html.AppendFormat("{0}----- DATA :: Box: {1} From: {2} To: {3} -----{4}", tr_open, boxId, dtFrom, dtTo, tr_close);
            html.AppendLine(hr);

            if (drMessages == null)
            {
               html.AppendFormat("{0}----- No data with given parameters -----{1}", tr_open, tr_close);
               return html.ToString();
            }

            if (drMessages.HasRows)
            {
               while (drMessages.Read())
               {
                  StateRow currentRow = new StateRow(drMessages[0], drMessages[1], drMessages[2], drMessages[3]);

                  switch (currentRow.MessageId)
                  {
                     case VLF.CLS.Def.Enums.MessageType.Sensor:
                     case VLF.CLS.Def.Enums.MessageType.SensorExtended:
                        switch (currentRow.SensorId)
                        {
                           case SensorType.Ignition:
                              if (storedRowIgnition != null && storedRowIgnition.SensorState == currentRow.SensorState)
                              {
                                 resultCounter++;
                                 html.AppendLine(tr_open + storedRowIgnition.Print() + tr_close);
                                 html.AppendLine(tr_open + currentRow.Print() + tr_close);
                                 html.AppendLine(hr);
                              }
                              storedRowIgnition = currentRow;
                              break;
                           case SensorType.PTO:
                              if (storedRowPto != null && storedRowPto.SensorState == currentRow.SensorState)
                              {
                                 resultCounter++;
                                 html.AppendLine(tr_open + storedRowPto.Print() + tr_close);
                                 html.AppendLine(tr_open + currentRow.Print() + tr_close);
                                 html.AppendLine(hr);
                              }
                              storedRowPto = currentRow;
                              break;
                        }
                        break;
                     case VLF.CLS.Def.Enums.MessageType.Idling:
                        if (storedRowIdling != null)
                        {
                           if ((storedRowIdling.Duration == 0 && currentRow.Duration == 0) ||
                              (storedRowIdling.Duration > 0 && currentRow.Duration > 0))
                           {
                              resultCounter++;
                              html.AppendLine(tr_open + storedRowIdling.Print() + tr_close);
                              html.AppendLine(tr_open + currentRow.Print() + tr_close);
                              html.AppendLine(hr);
                           }
                        }
                        storedRowIdling = currentRow;
                        break;
                  }
                  counter++;
               }
            }
            html.AppendFormat("{0}---------- SCAN RESULT: {1} -----------{2}", tr_open, resultCounter, tr_close);
         }
         catch (Exception exc)
         {
            html.AppendFormat("{0}Row: {1} Error: {2} Stack: {3}{4}", tr_open, counter, exc.Message, exc.StackTrace, tr_close);
         }
         finally
         {
            //drMessages.Close();
         }
         html.AppendLine("</table>");
         return html.ToString();
      }
   }

   /// <summary>
   /// Parsing row data into fields
   /// </summary>
   public class StateRow
   {
      const string SNS_NUM = "SENSOR_NUM", SNS_STATE = "SENSOR_STATUS", IDLE_DURATION = "IDLE_DURATION", SENSOR_DURATION = "SENSOR_DURATION";
      public int BoxId;
      public VLF.CLS.Def.Enums.MessageType MessageId;
      public SensorType SensorId;
      public int Duration;
      public string Message;
      public DateTime MessageDate;
      public SensorSignal SensorState;

      private StateRow()
      {
      }

      public StateRow(object boxid, object dt, object msgid, object msg)
      {
         string state = "", sSensor = "", sDuration = "";
         this.BoxId = Convert.ToInt32(boxid);
         this.MessageDate = Convert.ToDateTime(dt);
         this.MessageId = (VLF.CLS.Def.Enums.MessageType)Convert.ToInt32(msgid);
         this.Message = msg.ToString();
         CLS.Json json = new CLS.Json(this.Message);
         sSensor = json[SNS_NUM];
         state = json[SNS_STATE];

         switch (this.MessageId)
         {
            case VLF.CLS.Def.Enums.MessageType.Sensor:
            case VLF.CLS.Def.Enums.MessageType.SensorExtended:
               sDuration = json[SENSOR_DURATION];
               if (!String.IsNullOrEmpty(sDuration))
               {
                  this.Duration = Convert.ToInt32(sDuration);
               }
               if (!String.IsNullOrEmpty(sSensor))
               {
                  this.SensorId = (SensorType)Convert.ToInt32(sSensor);
                  if (!String.IsNullOrEmpty(state))
                  {
                     if (this.SensorId == SensorType.Ignition || this.SensorId == SensorType.PTO)
                     {
                        this.SensorState = state == SensorSignal.ON.ToString() ? SensorSignal.ON : SensorSignal.OFF;
                     }
                  }
               }
               break;
            case VLF.CLS.Def.Enums.MessageType.Idling:
               sDuration = json[IDLE_DURATION];
               if (!String.IsNullOrEmpty(sDuration))
               {
                  this.Duration = Convert.ToInt32(sDuration);
               }
               break;
            default:
               break;
         }
      }

      /// <summary>
      /// Prints the row
      /// </summary>
      /// <returns></returns>
      public string Print()
      {
         switch (this.MessageId)
         {
            case VLF.CLS.Def.Enums.MessageType.Sensor:
            case VLF.CLS.Def.Enums.MessageType.SensorExtended:
               return String.Format("Date: {0} :: Msg Type: {1} :: Sensor: {2} :: State: {3} :: Msg [{4}]",
                  this.MessageDate, this.MessageId, this.SensorId, this.SensorState.ToString(), this.Message);
            case VLF.CLS.Def.Enums.MessageType.Idling:
               return String.Format("Date: {0} :: Msg Type: {1} :: Duration: {2} :: Msg [{3}]",
                  this.MessageDate, this.MessageId, this.Duration, this.Message);
            default:
               return "";
         }
      }
   }

   public enum SensorSignal
   {
      ON,
      OFF
   }

   public enum SensorType
   {
      Ignition = 3,
      PTO = 8
   }
}
