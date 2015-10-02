using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VLF.DAS.DB;
using System.IO;


namespace LocalizationLayer
{
    public class ServerLocalizationLayer
    {

        private string connectionString = "";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public ServerLocalizationLayer(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
        }


        public void LocalizationData(string lang, string KeyField, string FieldName, string FieldGroup,  ref DataSet dsData)
        {
            //VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
            //DataSet dsLocalized = new DataSet();

            //dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang);
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



           VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
           DataSet dsLocalized = new DataSet();

           dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang);
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
            //VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
            //DataSet dsLocalized = new DataSet();

            //dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
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

           VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
           DataSet dsLocalized = new DataSet();

           dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
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
            //VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
            //DataSet dsLocalized = new DataSet();

            //dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
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


           VLF.DAS.DB.Localization dbLocal = new VLF.DAS.DB.Localization(connectionString);
           DataSet dsLocalized = new DataSet();

           dsLocalized = dbLocal.GetGuiLocalization(FieldGroup, lang, BoxHWTypeID);
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
