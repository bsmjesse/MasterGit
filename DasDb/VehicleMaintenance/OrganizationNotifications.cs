using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Organization Vehicle Service Notifications
   /// </summary>
   public partial class OrganizationNotifications : TblTwoPrimaryKeys
   {
      private const string MainTable = "vlfOrganizationNotifications";

      /// <summary>
      /// Organization Vehicle Service Notifications Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public OrganizationNotifications(SQLExecuter sqlExec) : base(MainTable, sqlExec)
      {
      }


   }
}
