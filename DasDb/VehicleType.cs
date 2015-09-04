using System;
using VLF.ERR;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfVehicleType table.
	/// </summary>
	public class VehicleType : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public VehicleType(SQLExecuter sqlExec) : base ("vlfVehicleType",sqlExec)
		{
		}
		/// <summary>
		/// Add new vehicle type.
		/// </summary>
		/// <param name="vehicleTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddVehicleType(string vehicleTypeName)
		{
			AddNewRow("VehicleTypeId","VehicleTypeName",vehicleTypeName,"vehicle type");
		}	
		/// <summary>
		/// Deletes exist vehicle type by name.
		/// </summary>
		/// <param name="vehicleTypeName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteVehicleType(string vehicleTypeName)
		{
			return DeleteRowsByStrField("VehicleTypeName",vehicleTypeName, "vehicle type");
		}
		/// <summary>
		/// Delete exist vehicle type by Id
		/// Throws exception in case of wrong result (see TblGenInterfaces class).
		/// </summary>
		/// <param name="vehicleTypeId"></param> 
		public int DeleteVehicleType(short vehicleTypeId)
		{
			return DeleteVehicleType( GetTypeById(vehicleTypeId) );
		}
		/// <summary>
		/// Retrieves record count of "vlfVehicleType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("VehicleTypeId");
			}
		}
		/// <summary>
		/// Retrieves max record index from "vlfVehicleType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("VehicleTypeId");
			}
		}
		/// <summary>
		/// Retrieves box hardware type name by id from "vlfVehicleType" table
		/// </summary>
		/// <param name="vehicleTypeId"></param>
		/// <returns>name</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetTypeById(short vehicleTypeId)
		{
			return GetFieldValueByRowId("VehicleTypeId",vehicleTypeId,"VehicleTypeName","vehicle type");
		}
	}
}
