using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VLF.CLS.Def;

namespace SentinelFM
{
    public class BoxProfileHelper
    {
        public static string ConnectionString { get; set; }

        public static string GetBoxLatestBoxStatus(int boxId)
        {
            string boxStatus = string.Empty;
            string storedProcedure = "[dbo].[aota_GetBoxLatestBoxStatus]";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BoxId", boxId));
                        command.Parameters.Add(new SqlParameter("@MessagetTypeId", (int)Enums.MessageType.BoxStatus));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    boxStatus = (string)reader["CustomProp"];
                                }
                            } //if
                        }//using reader
                    }//using command
                } //using conn
            }
            catch (Exception ex)
            {
   
            }

            return boxStatus;
        }

        public static int GetBoxMainBoardModelByBoxId(int boxId)
        {
            return GetBoxMainBoardModelByBoxStatus(GetBoxLatestBoxStatus(boxId));
        }

        public static int GetBoxMainBoardModelByBoxStatus(string boxStatus)
        {
            string firmwareVersionStr = VLF.CLS.Util.PairFindValue("Firmware Version", boxStatus);
            string mainBoardVersionStr = string.Empty;
            string mainBoardModelStr = string.Empty;

            if (firmwareVersionStr.Length > 9) //0001_tr63.SE7
            {
                mainBoardVersionStr = firmwareVersionStr.Substring(0, 9);
            }

              if (firmwareVersionStr.Length > 13)
              {
                int dlm = firmwareVersionStr.IndexOf('.');
                mainBoardModelStr = firmwareVersionStr.Substring(dlm + 1, 3);
              }

            return GetMainboardModelType(mainBoardModelStr);
        }


        private static int GetMainboardModelType(string mainboardModelStr)
        {
            //BoxStatus MainBoard Model: SE7   
            byte modelTypeId = 0;
            if (mainboardModelStr.Length < 3)
            {
                return 0;   //no device
            }

            string flag = mainboardModelStr.Substring(2, 1);
            try
            {
                modelTypeId = Convert.ToByte(flag);
            }
            catch (Exception)
            {
            }

            return (int)modelTypeId;
        }

        public static string GetBoxModelName(int boxModel)
        {
            string boxModelName = "Unknown";
            switch (boxModel)
            {
                case 2:
                    boxModelName = "SFM2000";
                    break;
                case 3:
                    boxModelName = "SFM3000";
                    break;
                case 5:
                       boxModelName = "SFM5000";
                       break;
                case 7:
                    boxModelName = "SFM7000";
                    break;

                default:
                    boxModelName = "Unsupported Box";
                    break;

            }
            return boxModelName;
        }
        
        public static byte GetCheckSum(string hexEEPROMOffset, byte length, string strBuzzerOutput, byte[] eepromIntialBytes)
        {
            byte[] dataSource = new byte[5];
            
            int intBuzzerOutput = Convert.ToInt32(strBuzzerOutput);
            string hexString2 = hexEEPROMOffset.Substring(2);
            int buzzerOffset = Int32.Parse(hexString2, System.Globalization.NumberStyles.HexNumber);  // + intBuzzerOutput - 1;
            byte[] offsetBytes = BitConverter.GetBytes(buzzerOffset);

            dataSource[0] = offsetBytes[0];
            dataSource[1] = offsetBytes[1];
            dataSource[2] = length;

            for (int i = 0; i < eepromIntialBytes.Length; i++)
            {
                dataSource[3 + i] = eepromIntialBytes[i];
            }

            return DataChecksum(dataSource);
        }

        public static byte DataChecksum(byte[] arrSource)
        {
            byte checksum = 0;
            if (null != arrSource)
            {
                for (int i = 0; i < arrSource.Length; i++)
                    checksum ^= arrSource[i];
                if (checksum == '<' || checksum == '>' || checksum == 0)
                    checksum = 0x40;
            }
            return checksum;
        }

        public static List<clsEEPROMFeature> GetEEPROMCommandList(int boxMode, int boxId, int userId)
        {
            List<clsEEPROMFeature> featureList = new List<clsEEPROMFeature>();          
            string storedProcedure = "[dbo].[usp_GetEEPROMCommandList]";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@boxModel", boxMode));
                        command.Parameters.Add(new SqlParameter("@boxId", boxId));
                        command.Parameters.Add(new SqlParameter("@userId", userId));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    clsEEPROMFeature feature = new clsEEPROMFeature();
                                    feature.FeatureId = Convert.ToInt32(reader["FeatureId"]);
                                    feature.FeatureName = (string)reader["FeatureName"];

                                    featureList.Add(feature);

                                }
                            } //if
                        }//using reader
                    }//using command
                } //using conn
            }
            catch (Exception ex)
            {
            }

            return featureList;
        }


        public static clsEEPROMFeature GetEEPROMFeaturById(int featureId)
        {
            clsEEPROMFeature feature = new clsEEPROMFeature();
            string storedProcedure = "[dbo].[usp_GetEEPROMFeatureById]";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@featureId", featureId));
                          using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    feature.FeatureId = Convert.ToInt32(reader["FeatureId"]);
                                    feature.OffsetInitial = (string)reader["Offset"];
                                    feature.DatalengthInitial = Convert.ToByte(reader["Datalength"]);
                                    feature.DataBuffer = (string)reader["DataBuffer"];
                                }
                            } //if
                        }//using reader
                    }//using command
                } //using conn
            }
            catch (Exception ex)
            {
            }

            return feature;
        }



    }
}
