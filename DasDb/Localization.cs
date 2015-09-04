using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{
   public class Localization 
    {
       private string connectionString = null;

       public Localization(string connectionString)
       {
           this.connectionString = connectionString;
       }


        public DataSet GetGuiLocalization(string FieldGroup, string Lang)
        {
            DataSet dsResult = null;
			try
			{
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT   LocalizationID , LocalName , FieldID, EngName , FieldGroup , Lang FROM  vlfGUILocalization ";
                    sql += " WHERE FieldGroup='" + FieldGroup + "' and Lang='" + Lang + "'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        // Create a new adapter and initialize it with the new SQL command
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        // Create a new data set
                        dsResult = new DataSet();

                        // It is time to update the data set with information from the data adapter
                        sda.Fill(dsResult, "GuiLocalization");
                    }
                }
             }
			
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve GUI Localization Info for Group:" + FieldGroup + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
            return dsResult;
        }




       


        public DataSet GetGuiLocalization(string FieldGroup, string Lang,short BoxHWTypeID)
        {
            DataSet dsResult = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT     vlfGUILocalization.LocalizationID, vlfGUILocalization.LocalName, vlfGUILocalization.FieldID, vlfGUILocalization.EngName, ";
                    sql += "  vlfGUILocalization.FieldGroup, vlfGUILocalization.Lang, vlfGuiHWTypeLocalization.BoxHwTypeID, vlfGuiHWTypeLocalization.LocalAction ";
                    sql += " FROM         vlfGUILocalization INNER JOIN  vlfGuiHWTypeLocalization ON vlfGUILocalization.LocalizationID = vlfGuiHWTypeLocalization.LocalizationID ";
                    sql += " WHERE FieldGroup='" + FieldGroup + "' and Lang='" + Lang + "' and BoxHwTypeID=" + BoxHWTypeID;

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                       
                        // Create a new adapter and initialize it with the new SQL command
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        // Create a new data set
                        dsResult = new DataSet();

                        // It is time to update the data set with information from the data adapter
                        sda.Fill(dsResult, "GuiLocalization");
                    }
                }
               }
               catch (Exception objException)
               {
                   string prefixMsg = "Unable to retrieve GUI Localization Info for Group:" + FieldGroup + ". ";
                   throw new DASException(prefixMsg + " " + objException.Message);
               }
               return dsResult;
        }



        public void LocalizationData(string lang, string KeyField, string FieldName, string FieldGroup, ref DataSet dsData)
        {
           //DataSet dsLocalized = new DataSet();
           //dsLocalized = GetGuiLocalization(FieldGroup, lang);
           //dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           //dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           //dsData.Tables[1].TableName = "GUILocalization";
           //dsData.Relations.Add("Localization",
           //                     dsData.Tables["Localized"].Columns[KeyField],
           //                     dsData.Tables["GUILocalization"].Columns["FieldID"],
           //                     false);
           //dsData.Tables["Localized"].Columns.Add("LocalName", typeof(string), "Max(Child.LocalName)");
           //dsData.Tables["Localized"].Columns.Remove(FieldName);
           //dsData.Tables["Localized"].Columns["LocalName"].ColumnName = FieldName;
           //dsData.Relations.Remove("Localization");
           //dsData.Tables.Remove("GUILocalization");




           DataSet dsLocalized = new DataSet();
           dsLocalized = GetGuiLocalization(FieldGroup, lang);
           dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           dsData.Tables[1].TableName = "GUILocalization";
           dsData.Relations.Add("Localization",
                                dsData.Tables[0].Columns[KeyField],
                                dsData.Tables["GUILocalization"].Columns["FieldID"],
                                false);
           dsData.Tables[0].Columns.Add("LocalName", typeof(string), "Max(Child.LocalName)");
           dsData.Tables[0].Columns.Remove(FieldName);
           dsData.Tables[0].Columns["LocalName"].ColumnName = FieldName;
           dsData.Relations.Remove("Localization");
           dsData.Tables.Remove("GUILocalization");
        }



        public void LocalizationData(string lang, string KeyField, string FieldName, string FieldGroup, short BoxHWTypeID, ref DataSet dsData)
        {
           //DataSet dsLocalized = new DataSet();
           //dsLocalized = GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
           //dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           //dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           //dsData.Tables[1].TableName = "GUILocalization";
           //dsData.Relations.Add("Localization",
           //                     dsData.Tables["Localized"].Columns[KeyField],
           //                     dsData.Tables["GUILocalization"].Columns["FieldID"],
           //                     false);
           //dsData.Tables["Localized"].Columns.Add("LocalName", typeof(string), "Max(Child.LocalName)");
           //dsData.Tables["Localized"].Columns.Remove(FieldName);
           //dsData.Tables["Localized"].Columns["LocalName"].ColumnName = FieldName;
           //dsData.Relations.Remove("Localization");
           //dsData.Tables.Remove("GUILocalization");


           DataSet dsLocalized = new DataSet();
           dsLocalized = GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
           dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           dsData.Tables[1].TableName = "GUILocalization";
           dsData.Relations.Add("Localization",
                                dsData.Tables[0].Columns[KeyField],
                                dsData.Tables["GUILocalization"].Columns["FieldID"],
                                false);
           dsData.Tables[0].Columns.Add("LocalName", typeof(string), "Max(Child.LocalName)");
           dsData.Tables[0].Columns.Remove(FieldName);
           dsData.Tables[0].Columns["LocalName"].ColumnName = FieldName;
           dsData.Relations.Remove("Localization");
           dsData.Tables.Remove("GUILocalization");
        }


        public void LocalizationDataAction(string lang, string KeyField, string LocalAction, string FieldGroup, short BoxHWTypeID, ref DataSet dsData)
        {
           
           //DataSet dsLocalized = new DataSet();

           //dsLocalized = GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
           //dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           //dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           //dsData.Tables[1].TableName = "GUILocalization";
           //dsData.Relations.Add("Localization",
           //                     dsData.Tables["Localized"].Columns[KeyField],
           //                     dsData.Tables["GUILocalization"].Columns["FieldID"],
           //                     false);
           //dsData.Tables["Localized"].Columns.Add("LocalName", typeof(string), "Max(Child.LocalAction)");
           //dsData.Tables["Localized"].Columns.Remove(LocalAction);
           //dsData.Tables["Localized"].Columns["LocalName"].ColumnName = LocalAction;
           //dsData.Relations.Remove("Localization");
           //dsData.Tables.Remove("GUILocalization");


           DataSet dsLocalized = new DataSet();

           dsLocalized = GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
           dsData.Tables.Add(dsLocalized.Tables[0].Copy());
           dsLocalized.Dispose();
           //dsData.Tables[0].TableName = "Localized";
           dsData.Tables[1].TableName = "GUILocalization";
           dsData.Relations.Add("Localization",
                                dsData.Tables[0].Columns[KeyField],
                                dsData.Tables["GUILocalization"].Columns["FieldID"],
                                false);
           dsData.Tables[0].Columns.Add("LocalName", typeof(string), "Max(Child.LocalAction)");
           dsData.Tables[0].Columns.Remove(LocalAction);
           dsData.Tables[0].Columns["LocalName"].ColumnName = LocalAction;
           dsData.Relations.Remove("Localization");
           dsData.Tables.Remove("GUILocalization");
        }

    }
}
