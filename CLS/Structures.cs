using System;

namespace VLF.CLS.Def.Structures
{
    #region DAS Section
    public struct VehicInfo
    {
        public string vinNum;			// char 20
        public int makeModelId;
        public short vehicleTypeId;
        public string stateProvince;	// char 50
        public short modelYear;
        public string color;			// char 20	(null is allowed)
        public string description;	// char 100	(null is allowed)
        public double costPerMile;
        public short iconTypeId;
        public string email;
        public string phone;
        public int timeZone;
        public short dayLightSaving;
        public short formatType;
        public short notify;
        public short warning;
        public short critical;
        public short autoAdjustDayLightSaving;
        // 4 new fields
        public string field1;
        public string field2;
        public string field3;
        public string field4;
        public short maintenance;
        public string _class;
        // Changes for TimeZone Feature start
        public float timeZoneNew;
        // Changes for TimeZone Feature end
    }
    public struct VehicAssign
    {
        public string licensePlate;	// char 20
        public int boxId;
        public Int64 vehicleId;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct DriverInfo
    {
        public int driverId;
        public string firstName;
        public string lastName;
        public string license;
        public string classLicense;
        public DateTime licenseIssued;
        public DateTime licenseExpired;
        public int orgId;
        public char gender;
        public short height;
        public string homePhone;
        public string cellPhone;
        public string additionalPhone;
        public string smsPwd;
        public string smsid;
        public string email;
        public string address;
        public string city;
        public string zipcode;
        public string state;
        public string country;
        public string description;
    }

    /// <remarks>
    /// Does not include UserId,UserName
    /// Those fields should be processed separately
    /// </remarks>
    public struct UserInfo
    {
        public int userId;
        public string username;
        public string password;		// char 30
        public string hashpassword;		// char 30
        public string personId;		// char 30
        public int organizationId;
        public string pin;			// char 20
        public string description;	// char 300
        public DateTime expiredDate;
        public string userStatus;
    }
    public struct PersonContactInfo
    {
        public string address;		// char 100 (null is allowed)
        public string city;			// char 50	(null is allowed)
        public string stateProvince;	// char 50	(null is allowed)
        public string country;		// char 50	(null is allowed)
        public string phoneNo1;		// char 20	(null is allowed)
        public string phoneNo2;		// char 20	(null is allowed)
        public string cellNo;			// char 20	(null is allowed)
    }
    public struct PersonInfoStruct
    {
        public string personId;		// char 30
        public string driverLicense;	// char 20
        public string firstName;		// char 50
        public string lastName;		// char 50
        public string middleName;		// char 50	(null is allowed)
        public DateTime birthday;
        public PersonContactInfo userContactInfo;// (null is allowed)
        public DateTime licenseExpDate;// (null is allowed)
        public string licenseEndorsements;// char 50	(null is allowed)
        public float height;			// (null is allowed)
        public float weight;			// (null is allowed)
        public string gender;			// char 10	(null is allowed)
        public string eyeColor;		// char 30	(null is allowed)
        public string hairColor;		// char 30	(null is allowed)
        public string idMarks;		// char 100	(null is allowed)
        public string certifications;	// char 100	(null is allowed)
        public string description;	// char 100	(null is allowed)
    }
    /// <summary>
    /// Storing info about hardware type output
    /// </summary>
    public struct HardwareOutput
    {
        public short outputID;
        public string outputName;
        public string outputAction;
    }
    /// <summary>
    /// Storing info about hardware type sensor
    /// </summary>
    public struct HardwareSensor
    {
        public short sensorID;
        public string sensorName;
        public string sensorAction;
        public short sensorAlarmOn;
        public short sensorAlarmOff;
    }

    public struct OrganizationPeripheralFormset
    {
        public uint organizationPeripheralFormsetId;
        public uint organizationId;
        public ushort peripheralTypeId;
        public string description;      // char 120
        public string title;            // char 15
        public short gmtOffset;
        public bool autoDaylightSavings;
        public bool blankInMotion;
    }

    public struct PeripheralForm
    {
        public uint peripheralFormId;
        public ushort peripheralTypeId;
        public string description;      // char 120
        public ushort identifier;
        public byte modality;            // char 15
        public byte version;
        public bool isThirdParty;
        public byte[] bin;
    }

    #endregion DAS Section
}
