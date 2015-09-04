using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Organization Vehicle Service Type
   /// </summary>
   public partial class OrganizationVehicleServiceType : TblTwoPrimaryKeys
   {
      private const string MainTable = "vlfOrganizationVehicleServiceType";

      /// <summary>
      /// OrganizationVehicleServiceType Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public OrganizationVehicleServiceType(SQLExecuter sqlExec) : base(MainTable, sqlExec)
      {
      }
   }
}
