/*
 *  Copyright (c) 2011 by Bilal Soylu
 *  Bilal Soylu licenses this file to You under the 
 *  Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

/*************************************************************************
 * Description: IIS-to-Tomcat connector                                  *
 * Author:      Bilal Soylu <bilal.soylu[at]gmail.com>                   *
 * Version:     1.0                                                      *
 *************************************************************************/

//==============================================================================
// This is an abstract class for Packages Send and Received from Tomcat
//==============================================================================
// We refer to packages send as Server packets (see seperate folder)
// We refer to packages received from tomcat  as Tomcat packets (see seperate folder)
//==============================================================================

using System;
using System.Text;


namespace BonCodeAJP13
{
    /// <summary>
    /// abstract class, Represents the base class for all BonCodeAJP13 Server packets, don't create an instance of this class directly
    /// </summary>
    public abstract class BonCodeAJP13Packet
    {
        #region data Members

        protected byte p_PacketType;    
        protected UInt16 p_UserDataLength=0;
        protected int p_PacketLength=0;
        protected byte[] p_ByteStore = null; //declare storage for packet data. it is empty by default
        protected const string p_PACKET_DESCRIPTION = "GENERIC"; //this constant will be overriden with derived classes

        #endregion

        #region Properties

        /// <summary>
        /// Length of the user specific data bytes in packet. This excludes any control bytes.
        /// </summary>
        public virtual ushort Length
        {
            get { return p_UserDataLength; }
        }

        /// <summary>
        /// packet type, see BonCodeAJP13ServerPacketType enumeration.
        /// </summary>
        public byte PacketType
        {
            get { return p_PacketType; }
        }

        /// <summary>
        /// packet ID, for future use
        /// </summary>
        public byte PacketID
        {
            get { return 0; }  //not using this right now
        }

  

        /// <summary>
        /// The Length of user data in the packet. This is not the same as the overall length of package.
        /// Use PacketLength for overall length.
        /// </summary>
        public UInt16 UserDataLength
        {
            get { return p_UserDataLength; }
        }

        /// <summary>
        /// Overall Length of bytes in packet. If you need to only get user data size use UserDataLength
        /// </summary>
        public int PacketLength
        {
            get { return p_PacketLength; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Generic empty constructor 
        /// </summary>
        public BonCodeAJP13Packet()
        {

        }

        /// <summary>
        /// Initialize the packet from the buffer, it sets the essential properties of the packet like type. Each derived class will define details. 
        /// </summary>
        public BonCodeAJP13Packet(byte[] buffer)
        {

            if (buffer != null && buffer.Length >= BonCodeAJP13Consts.MIN_BONCODEAJP13_PACKET_LENGTH && buffer.Length <= BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH)
            {
                try
                {
                    p_UserDataLength = System.Convert.ToUInt16(buffer.Length - 4);
                }
                catch { }
            }
            else
            {
                // throw an exeption to mark reason
                throw new Exception("Invalid BonCodeAJP13 Packet received. Wrong byte length or null buffer.");
            }

        }

        /// <summary>
        /// Initialize the packet from string, it sets the essential properties of the packet like type. 
        /// Each derived class will define details. 
        /// </summary>
        public BonCodeAJP13Packet(string content)
        {
            if (content != null)
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

                byte[] buffer = encoder.GetBytes(content);

                if (buffer.Length >= BonCodeAJP13Consts.MIN_BONCODEAJP13_PACKET_LENGTH && buffer.Length <= BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH)
                {
                    try
                    {
                        p_UserDataLength = System.Convert.ToUInt16(buffer.Length - 4);
                    }
                    catch { }
                }
                else
                {
                    // throw an exeption to mark reason
                    throw new Exception("Invalid BonCodeAJP13 Packet received. Wrong byte length.");
                }
            }
            else
            {
                // throw an exeption to mark reason
                throw new Exception("No content. Invalid BonCodeAJP13 Packet received. Zero byte length.");
            }
            

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the raw data of the packet in an array of bytes
        /// </summary>
        public virtual byte[] GetDataBytes()
        {
            return p_ByteStore;
        }




        /// <summary>
        /// Returns the string data (content) of package
        /// optionally an ecoding paramater can be passed. Default is 8 for UTF 8
        /// </summary>
        public virtual string GetDataString(int intEncoding=8)
        {
            //return the user data packet as string. This returns the whole store.
            return System.Text.Encoding.UTF8.GetString(p_ByteStore, 0, p_ByteStore.Length);   
   
        }

        /// <summary>
        /// Returns the raw user data of the packet without control bytes. Returns empty array if no user data is present.        
        /// </summary>
        public virtual byte[] GetUserDataBytes()
        {
            if (p_ByteStore.Length > 3)
            {
                byte[] userArray = new byte[p_ByteStore.Length - 4];
                Array.Copy(p_ByteStore, 3, userArray, 0, p_ByteStore.Length - 4);                
                return userArray;
            }
            else
            {
                //return empty marker if no user data
                return new byte[] {};
            }
            
        }

        /// <summary>
        /// Return string of user data enclosed in packet. This is different based on each packet type     
        /// </summary>
        public virtual string GetUserDataString()
        {
            //generically the first three bytes and the last byte are control bytes and we will not return them
            //byte[0]=type, byte[1,2] = string length, last byte (0x00) = string terminator
            if (p_ByteStore.Length > 3)
            {
                
                return System.Text.Encoding.UTF8.GetString(p_ByteStore, 3, p_ByteStore.Length - 4);
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Initialize the packet's header in the raw data. This is a sample stub, each outbound packet type will write its own
        /// version of this function.
        /// </summary>
        protected int WriteBonCodeAJP13Header(byte[] data)
        {
            int pos = 0;
            // Writing the packet Header .... 
            // ============================================================
            pos = SetByte(data, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START, pos);
            pos = SetByte(data, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2, pos);
            pos = SetUInt16(data, p_UserDataLength, pos);
            pos = SetByte(data, p_PacketType, pos);
                      
            return pos;
        }


        #region Byte

        /// <summary>
        /// Set the byte value in the array starting from the pos index 
        /// </summary>
        protected static int SetByte(byte[] data, byte value, int pos)
        {
            data[pos] = value;
            return pos + 1;
        }

        /// <summary>
        /// Get the byte value from the array starting from the pos index
        /// </summary>
        protected static int GetByte(byte[] data, ref byte value, int pos)
        {
            value = data[pos];
            return pos + 1;
        }
        #endregion

        #region Int16
        /// <summary>
        /// Set the Int16 value in the array starting from the pos index 
        /// Using unsigned integers only range from 0 to 65,535
        /// </summary>
        protected static int SetInt16(byte[] data, UInt16 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt16)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array .... Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(UInt16);
        }

        /// <summary>
        /// Get the Int16 value from the array starting from the pos index 
        /// Using unsigned integers only range from 0 to 65,535
        /// </summary>
        protected static int GetInt16(byte[] data, ref UInt16 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt16)];
            Array.Copy(data, pos, valueData, 0, sizeof(UInt16));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToUInt16(valueData, 0);
            return pos + sizeof(UInt16);
        }
        #endregion

        #region Int32
        /// <summary>
        /// Set the Int32 value in the array starting from the pos index 
        /// These are signed values -2,147,483,648 to 2,147,483,647
        /// </summary>
        protected static int SetInt32(byte[] data, Int32 value, int pos)
        {
            byte[] valueData = new byte[sizeof(Int32)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(Int32);
        }

        /// <summary>
        /// Get the Int32 value from the array starting from the pos index 
        /// These are signed values -2,147,483,648 to 2,147,483,647
        /// </summary>
        protected static int GetInt32(byte[] data, ref Int32 value, int pos)
        {
            byte[] valueData = new byte[sizeof(Int32)];
            Array.Copy(data, pos, valueData, 0, sizeof(Int32));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToInt32(valueData, 0);
            return pos + sizeof(Int32);
        }
        #endregion

        #region Int64
        /// <summary>
        /// Set the Int64 value in the array starting from the pos index 
        /// Signed 64 bit integer -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        /// </summary>
        protected static int SetInt64(byte[] data, Int64 value, int pos)
        {
            byte[] valueData = new byte[sizeof(Int64)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(Int64);
        }

        /// <summary>
        /// Get the Int64 value from the array starting from the pos index 
        /// Signed 64 bit integer -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        /// </summary>
        protected static int GetInt64(byte[] data, ref Int64 value, int pos)
        {
            byte[] valueData = new byte[sizeof(Int64)];
            Array.Copy(data, pos, valueData, 0, sizeof(Int64));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToInt64(valueData, 0);
            return pos + sizeof(Int64);
        }

        #endregion

        #region UInt16
        /// <summary>
        /// Set the UInt16 value in the array starting from the pos index 
        /// </summary>
        protected static int SetUInt16(byte[] data, UInt16 Value, int Pos)
        {
            byte[] valueData = new byte[sizeof(UInt16)];
            valueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, Pos, valueData.Length);
            return Pos + sizeof(UInt16);
        }

        /// <summary>
        /// Get the UInt16 value from the array starting from the pos index 
        /// </summary>
        protected static int GetUInt16(byte[] data, ref UInt16 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt16)];
            Array.Copy(data, pos, valueData, 0, sizeof(UInt16));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToUInt16(valueData, 0);
            return pos + sizeof(UInt16);
        }
        #endregion

        #region UInt32
        /// <summary>
        /// Set the UInt32 value in the array starting from the pos index 
        /// </summary>
        protected static int SetUInt32(byte[] data, UInt32 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt32)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(UInt32);
        }

        /// <summary>
        /// Get the UInt32 value from the array starting from the pos index 
        /// </summary>
        protected static int GetUInt32(byte[] data, ref UInt32 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt32)];
            Array.Copy(data, pos, valueData, 0, sizeof(UInt32));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToUInt32(valueData, 0);
            return pos + sizeof(UInt32);
        }

        #endregion

        #region UInt64
        /// <summary>
        /// Set the UInt64 value in the array starting from the pos index 
        /// </summary>
        protected static int SetUInt64(byte[] data, UInt64 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt64)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(UInt64);
        }

        /// <summary>
        /// Get the UInt64 value from the array starting from the pos index 
        /// </summary>
        protected static int GetUInt64(byte[] data, ref UInt64 value, int pos)
        {
            byte[] valueData = new byte[sizeof(UInt64)];
            Array.Copy(data, pos, valueData, 0, sizeof(UInt64));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToUInt64(valueData, 0);
            return pos + sizeof(UInt64);
        }

        #endregion

        #region Double
        /// <summary>
        /// Set the Double value in the array starting from the pos pos 
        /// </summary>
        protected static int SetDouble(byte[] data, double value, int pos)
        {
            byte[] valueData = new byte[sizeof(double)];
            valueData = BitConverter.GetBytes(value);
            // flipping the Array ....Big Endian .. 
            valueData = FlipArray(valueData);
            Array.Copy(valueData, 0, data, pos, valueData.Length);
            return pos + sizeof(double);
        }

        /// <summary>
        /// Get the Double value from the array starting from the pos index 
        /// </summary>
        protected static int GetDouble(byte[] data, ref double value, int pos)
        {
            byte[] valueData = new byte[sizeof(double)];
            Array.Copy(data, pos, valueData, 0, sizeof(double));

            // flipping the Array .... Big Endian ..
            valueData = FlipArray(valueData);
            value = BitConverter.ToDouble(valueData, 0);
            return pos + sizeof(double);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Set the DateTime value in the array starting from the pos index 
        /// </summary>
        protected static int SetDateTime(byte[] data, DateTime value, int pos)
        {
            return SetInt64(data, value.ToBinary(), pos);
        }

        /// <summary>
        /// Get the DateTime value from the array starting from the pos index 
        /// </summary>
        protected static int GetDateTime(byte[] data, ref DateTime value, int pos)
        {
            Int64 longValue = 0;
            int tempPos = GetInt64(data, ref longValue, pos);
            value = DateTime.FromBinary(longValue);
            return tempPos;
        }
        #endregion

        #region String
        /// <summary>
        /// Set the String value in the array starting from the pos index 
        /// String will be transmitted prefixed by length UInt16 in bytes and terminated by zero byte
        /// </summary>
        protected static int SetString(byte[] data, string value="", int pos=-1)
        {
            if (value == null) value = "";

            if (data != null && pos > -1 )
            {
                // ASCIIEncoding encodingLib = new System.Text.ASCIIEncoding();
                UTF8Encoding encodingLib = new System.Text.UTF8Encoding();
                int valueByteSize = encodingLib.GetByteCount(value);
                byte[] valueData = new byte[valueByteSize + 3]; // need space for character counter at the beginning of array and termination at end
                SetUInt16(valueData, (UInt16)valueByteSize, 0); //first two bytes set the length of the string

                byte[] temp = new byte[valueByteSize];
                temp = encodingLib.GetBytes(value);

                Array.Copy(temp, 0, valueData, 2, temp.Length);
                //the last byte of valueData is allways zero based on our initial declaration. This indicates the terminator byte as well. Copy this to the main byte array
                Array.Copy(valueData, 0, data, pos, valueData.Length);
                return pos + valueByteSize + 3; //we added three more characters/bytes than passed in
            }
            else
            {
                //we cannot write to null data reference or invalid pos, return zero pos or same pos as passed
                return pos == -1 ? 0:pos;
            }
        }

        /// <summary>
        /// Set the String value in the array starting from the pos index 
        /// The length prefix will be omitted.
        /// </summary>
        protected static int SetSimpleString(byte[] data, string value, int pos)
        {
            
            //We use UTF 8 encoding for any string conversion
            UTF8Encoding encodingLib = new System.Text.UTF8Encoding();
            byte[] stringBytes = new byte[value.Length];
            stringBytes = encodingLib.GetBytes(value);
                        
            return SetSimpleByteArray(data,stringBytes,pos); 
        }

        /// <summary>
        /// Get the String value from the array starting from the pos index 
        /// String length is first two bytes from starting pos, then terminated by zero char
        /// </summary>
        protected static int GetString(byte[] data, ref string value, int pos)
        {
            UInt16 realLength = 0;
            GetUInt16(data, ref realLength, pos);
            //ASCIIEncoding encodingLib = new System.Text.ASCIIEncoding();
            UTF8Encoding encodingLib = new System.Text.UTF8Encoding();
            value = encodingLib.GetString(data, pos + 2, realLength);
            return pos + realLength + 3; //string is terminated with zero char which needs to be skipped as well thus +3
        }
        #endregion

        #region byte[]
        /// <summary>
        /// Set the byte array in the data array starting from the pos index
        /// Assumed that first two bytes contain length indicator and will be stored before actual array of bytes data.
        /// </summary>
        protected static int SetByteArray(byte[] data, byte[] value, int pos)
        {
            byte[] lengthOfArrayStore = new byte[2];
            SetUInt16(lengthOfArrayStore, (UInt16)value.Length, 0);

            Array.Copy(lengthOfArrayStore, 0, data, pos, 2);
            Array.Copy(value, 0, data, pos + 2, value.Length);
            return pos + value.Length + 2;

        }

        /// <summary>
        /// Set the byte array in the data array starting from the pos index
        /// Bytes appended without length indicator
        /// </summary>
        protected static int SetSimpleByteArray(byte[] data, byte[] value, int pos)
        {           
            Array.Copy(value, 0, data, pos, value.Length);
            return pos + value.Length;

        }

        /// <summary>
        /// Get the byte array value from the data array starting from the pos index
        /// The length of bytes to be returned is determined by first two bytes read from pos
        /// </summary>
        protected static int GetByteArray(byte[] data, ref byte[] value, int pos)
        {
            UInt16 arrayLength = 0; //assumed that first two bytes contain length of byte array to be returned
            GetUInt16(data, ref arrayLength, pos);

            value = new byte[arrayLength];
            Array.Copy(data, pos + 2, value, 0, arrayLength);
            return pos + arrayLength + 2;

        }

        /// <summary>
        /// Get the byte array value from the data array starting from the pos index
        /// The length of bytes to return is passed
        /// </summary>
        protected static int GetSimpleByteArray(byte[] data, ref byte[] value, int pos, int length)
        {
            UInt16 arrayLength = System.Convert.ToUInt16(length);             

            value = new byte[arrayLength];
            Array.Copy(data, pos, value, 0, arrayLength);
            return pos + arrayLength ;

        }

        #endregion



        /// <summary>
        /// Returns a string of the packet contents. This includes header data. Generic form.
        /// </summary>
        public virtual string PrintPacket()
        {
            string strPck;
            strPck =  "\r\n begin of packet ====> \r\nType:" + p_ByteStore[0].ToString();
            if (p_ByteStore.Length > 2)
            {
                strPck = strPck + "\r\nUser Data High:" + p_ByteStore[1].ToString();
                strPck = strPck + "\r\nUser Data Low:" + p_ByteStore[2].ToString();
                strPck = strPck + "\r\n";
                if (p_ByteStore.Length > 4)
                {
                    strPck = strPck + GetUserDataString();
                }
            }
            else
            {
                strPck = strPck + System.Text.Encoding.UTF8.GetString(p_ByteStore, 0, p_ByteStore.Length);
            }
            
            strPck = strPck  + "\r\n <==== end of packet";
            return strPck;


        }

        /// <summary>
        /// Returns header information about a packet
        /// </summary>
        public virtual string PrintPacketHeader()
        {
            string strPckHead="";
            strPckHead = " " + p_ByteStore.Length.ToString() + " bytes";    


            return strPckHead;
        }



        /// <summary>
        /// Flips the byte array to send it using the big endian convention
        /// </summary>
        protected static byte[] FlipArray(byte[] data)
        {
            byte[] flippedData = new byte[data.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                flippedData[data.GetUpperBound(0) - i] = data[i];
            }
            return flippedData;
        }

        /// <summary>
        /// Analyze the provided buffer and returns suitable packet instance. This is a stub 
        /// </summary>
        public static BonCodeAJP13Packet GetPacket(byte[] Buffer)
        {
            // First of all we need to check that this is a valid packet
            // then we will return the suitable packet instance, otherwise null will be returnd.           

            return null;
        }

        

        #endregion

    }
}


