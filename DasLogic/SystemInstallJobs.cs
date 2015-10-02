using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Xml;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
   public partial class SystemConfig
   {
      /// <summary>
      /// Add new Install Job
      /// </summary>
      /// <param name="xmlData"></param>
      /// <param name="description"></param>
      /// <param name="status"></param>
      /// <param name="modified"></param>
      /// <returns></returns>
      public int InstallJob_Add(string xmlData, string description, string status, DateTime modified)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(xmlData);
         int boxId;
         XmlNode nodeBoxId = doc.DocumentElement.SelectSingleNode("Debug/BoxID");
         if (nodeBoxId == null)
            throw new ArgumentException("Box Id Node not found");

         if (!Int32.TryParse(nodeBoxId.InnerText, out boxId))
            throw new XmlException("Invalid box Id");

         XmlNode nodeInstaller = doc.DocumentElement.SelectSingleNode("Installer/Name");
         if (nodeInstaller == null)
            throw new XmlException("Installer Node not found");

         InstallJob job = new InstallJob(this.sqlExec);
         return job.AddJob(boxId, modified, status, nodeInstaller.InnerText, description, xmlData);
      }

      /// <summary>
      /// Get all install jobs
      /// </summary>
      /// <returns></returns>
      public DataSet InstallJob_GetAll()
      {
         InstallJob job = new InstallJob(this.sqlExec);
         return job.GetRows("JobID, BoxId, LastModified, Status, Installer, Description");
      }

      /// <summary>
      /// Get install job
      /// </summary>
      /// <returns></returns>
      public DataSet InstallJob_Get(int jobId)
      {
         InstallJob job = new InstallJob(this.sqlExec);
         return job.GetRowsByFilter("XMLData", "InstallJobs", "WHERE JobId = @jobId", new SqlParameter("@jobId", jobId));
      }
   }
}
