using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SentinelFM
{
    /// <summary>
    /// This class for represent EEPROP Feature in the system.
    /// </summary>
    public class clsEEPROMFeature
    {
        //--properties
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureParameter { get; set; }
        public int BoxModel { get; set; }

        public int OutputIndex { get; set; }       //output cable index, e.g: Buzzer=2
        public string OffsetInitial { get; set; }   //read from db
        public int IntOffsetInitial { get; set; }   //read from db in int

        public string Offset { get; set; }          //to fill Offset text box in Popup Window.

        public byte DatalengthInitial { get; set; }     //without checksum byte, read from DB
        public byte Datalength { get; set; }            //with CheckSum

        public string DataBuffer { get; set; }  //From DB
        public byte[] DataBytes { get; set; }   //without checksum byte
        public string DataText { get; set; }    //to fill the Data Text Area in Popup Window.

        private byte checkSum;
        public byte CheckSum { get; set; }

        //Methods:
        public clsEEPROMFeature()
        {
            //Initial 
        }

        public clsEEPROMFeature(int boxModel)
        {
            this.BoxModel = boxModel;
            RefreshDataLength(this.BoxModel);
        }

        public void RefreshDataLength(int boxModel)
        {
            this.BoxModel = boxModel;
            Datalength = DatalengthInitial;

            switch (BoxModel)
            {
                case 2:
                    Datalength = DatalengthInitial;
                    break;
                case 3:
                case 7:
                    Datalength = (byte)(DatalengthInitial + 1);
                    break;
            }
        }
        
        public string RefreshOffset()
        {
            //After OutputIndex changed, call this method.
            string hexOffset = OffsetInitial.Substring(2);
            int buzzerOffset = Int32.Parse(hexOffset, System.Globalization.NumberStyles.HexNumber) + OutputIndex - 1;

            this.Offset = String.Format("0x{0:X}", buzzerOffset);
            return Offset;
        }

        public byte GenerateCheckSum()
        {
            int dataByteLen = DataBytes.Length;
            byte[] dataSource = new byte[2 + 1 + dataByteLen];  //

            string hexOffset = Offset.Substring(2);  //0x2C8==>2C8
            int buzzerOffset = Int32.Parse(hexOffset, System.Globalization.NumberStyles.HexNumber);

            byte[] offsetBytes = BitConverter.GetBytes(buzzerOffset);

            dataSource[0] = offsetBytes[0];
            dataSource[1] = offsetBytes[1];
            dataSource[2] = Datalength;

            for (int i = 0; i < DataBytes.Length; i++)
            {
                dataSource[3 + i] = DataBytes[i];
            }

            return BoxProfileHelper.DataChecksum(dataSource);
        }

        public int ProcessBuzzer()
        {
            RefreshOffset();
           
            const string BUFFER_DELIMITER = " ";
          
            switch (BoxModel)
            {
                case 2:
                    Datalength = DatalengthInitial;
                    DataText = DataBuffer;
                    break;
                case 3:
                case 7:
                    DataBytes = ParseDataBuffer();
                    CheckSum = GenerateCheckSum();

                    DataText = BitConverter.ToString(DataBytes).Replace("-", BUFFER_DELIMITER);
                    DataText = DataText + BUFFER_DELIMITER + String.Format("{0:X}", CheckSum);
                    Datalength = (byte)(DatalengthInitial + 1);
                    break;
            }

            return 0;
        }

        public int Populate()
        {
            string hexOffset = OffsetInitial.Substring(2);
            int buzzerOffset = Int32.Parse(hexOffset, System.Globalization.NumberStyles.HexNumber);// + OutputIndex - 1;

            this.Offset = String.Format("0x{0:X}", buzzerOffset);
   
            const string BUFFER_DELIMITER = " ";
             switch (BoxModel)
            {
                case 2:
                    Datalength = DatalengthInitial;
                    DataText = DataBuffer;
                    break;
                case 3:
                case 7:
                    DataBytes = ParseDataBuffer();
                    DataText = BitConverter.ToString(DataBytes).Replace("-", BUFFER_DELIMITER);
                    CheckSum = GenerateCheckSum();
                    DataText = DataText + BUFFER_DELIMITER + String.Format("{0:X}", CheckSum);
                    Datalength = (byte)(DatalengthInitial+1);
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Build DataBytes for 2000,3000, 7000 Box.
        /// </summary>
        /// <returns>Data Bytes Array</returns>
        public byte[] ParseDataBuffer()
        {
            // DataBuffer = "48 EA A7";
            string[] hexValuesSplit = DataBuffer.Split(' ');
            int lenWithoutCheckSum = hexValuesSplit.Length;
            byte[] dataWithoutCheckSum = new byte[lenWithoutCheckSum];

            for (int i = 0; i < lenWithoutCheckSum; i++)
            {
                dataWithoutCheckSum[i] = (byte)Convert.ToInt32(hexValuesSplit[i], 16);
            }
            
            return dataWithoutCheckSum;
        }


    } //class
}//namespace