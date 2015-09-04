using System;
using System.Collections.Generic;
using System.Text;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.CLS.Def;
using VLF.CLS;

namespace VLF.DAS.Logic
{
   /// <summary>
   ///      the same content as vlfPeripheralTypes
   /// </summary>
   public enum PeripheralType : int
   {
      Null           =   1,
      Reefer         =   2,
      Netistix       =   3,
      MDT            =   4,
      Garmin         =   5,
//      Blackberry     =   6,
      NotAssigned    =   6
   }

   public class Peripheral : Das
   {
       bool _usingHistory = false;
        DB.Peripheral _currentAssignments = null;
        DB.PeripheralHistory _history = null;

        #region General Interfaces

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
      public Peripheral(string connectionString, bool usingHistory)
            : base(connectionString)
        {
            _usingHistory = usingHistory;

            if (usingHistory)
               _history = new VLF.DAS.DB.PeripheralHistory(sqlExec);
            else
                _currentAssignments = new VLF.DAS.DB.Peripheral(sqlExec);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }

        #endregion

      /// <summary>
      ///         assigns the peripheral to the box 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="when"></param>
      /// <param name="peripheralType"></param>
      /// <param name="peripheralId"></param>
      /// <param name="MajorVersion"></param>
      /// <param name="MinorVersion"></param>
      /// <returns></returns>
      public int PeripheralAssignment(int boxId, DateTime when, int peripheralType, string peripheralId, int MajorVersion, int MinorVersion)
      {
         Util.BTrace(Util.ERR0, "-- PeripheralAssignment -> boxid={0}, dtWhen={1}, type={2}, pid={3}", boxId, when, peripheralType, peripheralId);
         if (!_usingHistory)
             if (peripheralType > (int)PeripheralType.Null && peripheralType < (int)PeripheralType.NotAssigned)
                   return _currentAssignments.PeripheralAssignment(boxId, when, peripheralType, 
                                                                  peripheralId, MajorVersion, MinorVersion);
             else
               Util.BTrace(Util.ERR0, "-- PeripheralAssignment -> peripheral type not known {0}", peripheralType);

         return (-1);
      }

      public void UpdateGarminDescription(int garminId, string Description)
      {
          if (!_usingHistory)
                _currentAssignments.UpdateRow("SET Description = @description WHERE GarminId=@id", "dbo.vlfGarmin", 
                        new SqlParameter("@description", Description), new SqlParameter("@id", garminId));
      }

      public void UpdateMdtDescription(int MdtId, string Description)
      {
         if (!_usingHistory)
            _currentAssignments.UpdateRow("SET Description = @description WHERE MdtId=@id", "dbo.vlfMdt",
                    new SqlParameter("@description", Description), new SqlParameter("@id", MdtId));
      }
/*
      public int GetPeripheralId(int boxid, int typeId, out int garminOrMdtId )
      {
         if (typeId == (int)PeripheralType.Garmin)
            _currentAssignments.
      }
*/
   }
}
