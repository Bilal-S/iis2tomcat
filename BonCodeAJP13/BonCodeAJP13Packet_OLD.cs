//==========================
// Packet Structure ......
//==========================
// Start Bytes
// Data Length
// Data
//==========================

using System;
using System.Collections.Generic;
using System.Text;
using BonCodeAJP13;


namespace BonCodeAJP13Namespace
{
    /// <summary>
    /// abstract class, Represents the base class for all BonCodeAJP13 Server Packets, don't create an instance of this class directly
    /// </summary>
    public abstract class BonCodeAJP13Packet
    {
        #region Data Members

        protected byte m_PacketType;    
        protected UInt16 m_UserDataLength;
        //protected UInt16 m_SessionID;

        public const byte START_POS = 0;
        public const byte USERDATALENGTH_POS = 2;
        public const byte PACKET_TYPE_POS = 4;        

        #endregion

        #region Properties

        /// <summary>
        /// Length of the Packet in bytes.
        /// </summary>
        public virtual ushort Length
        {
            get { return m_UserDataLength; }
        }

        /// <summary>
        /// Packet type, see BonCodeAJP13ServerPacketType enumeration.
        /// </summary>
        public byte PacketType
        {
            get { return m_PacketType; }
        }

        /// <summary>
        /// Packet ID, each packet has a unique ID provided in the file: BonCodeAJP13Enums.cs
        /// </summary>
        public byte PacketID
        {
            get { return 0; }  //not using this right now
        }

  

        /// <summary>
        /// The Length of the data in the packet.
        /// </summary>
        public UInt16 UserDataLength
        {
            get { return m_UserDataLength; }
        }

        #endregion

        #region Constructors

        //generic constructor
        public BonCodeAJP13Packet()
        {

        }

        /// <summary>
        /// Initialize the packet from the buffer, this is the normal case, it sets the essential properties of the packet like type 
        /// </summary>
        public BonCodeAJP13Packet(byte[] Buffer)
        {

            if (Buffer.Length >= BonCodeAJP13Consts.MIN_BONCODEAJP13_PACKET_LENGTH && Buffer.Length <= BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH)
            {
       
            }
            else
            {
                // throw an exeption here ... stating the error !! 
                throw new Exception("Invalid BonCodeAJP13 Packet recieved. Wrong byte length.");
            }

        }

        #endregion

        #region Methods



        /// <summary>
        /// Returns the raw data of the packet in an array of bytes
        /// </summary>
        public abstract byte[] GetDataBytes();

        /// <summary>
        /// Initialize the packet's header in the raw data.
        /// </summary>
        protected int WriteBonCodeAJP13Header(byte[] Data)
        {
            int Pos = 0;
            // Writing the packet Header .... never change the order of the lines here ... 
            // ============================================================
            Pos = SetByte(Data, BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_START, Pos);
            Pos = SetByte(Data, BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_START2, Pos);
            Pos = SetUInt16(Data, m_UserDataLength, Pos);
            Pos = SetByte(Data, m_PacketType, Pos);
           
            
            
            return Pos;
        }




        #region Byte

        /// <summary>
        /// Set the byte value in the array starting from the position Pos 
        /// </summary>
        protected static int SetByte(byte[] Data, byte Value, int Pos)
        {
            Data[Pos] = Value;
            return Pos + 1;
        }

        /// <summary>
        /// Get the byte value from the array starting from the position Pos 
        /// </summary>
        protected static int GetByte(byte[] Data, ref byte Value, int Pos)
        {
            Value = Data[Pos];
            return Pos + 1;
        }
        #endregion

        #region Int16
        /// <summary>
        /// Set the Int16 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetInt16(byte[] Data, Int16 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int16)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array .... Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(Int16);
        }

        /// <summary>
        /// Get the Int16 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetInt16(byte[] Data, ref Int16 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int16)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(Int16));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToInt16(ValueData, 0);
            return Pos + sizeof(Int16);
        }
        #endregion

        #region Int32
        /// <summary>
        /// Set the Int32 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetInt32(byte[] Data, Int32 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int32)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(Int32);
        }

        /// <summary>
        /// Get the Int32 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetInt32(byte[] Data, ref Int32 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int32)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(Int32));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToInt32(ValueData, 0);
            return Pos + sizeof(Int32);
        }
        #endregion

        #region Int64
        /// <summary>
        /// Set the Int64 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetInt64(byte[] Data, Int64 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int64)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(Int64);
        }

        /// <summary>
        /// Get the Int64 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetInt64(byte[] Data, ref Int64 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(Int64)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(Int64));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToInt64(ValueData, 0);
            return Pos + sizeof(Int64);
        }

        #endregion

        #region UInt16
        /// <summary>
        /// Set the UInt16 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetUInt16(byte[] Data, UInt16 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt16)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(UInt16);
        }

        /// <summary>
        /// Get the UInt16 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetUInt16(byte[] Data, ref UInt16 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt16)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(UInt16));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToUInt16(ValueData, 0);
            return Pos + sizeof(UInt16);
        }
        #endregion

        #region UInt32
        /// <summary>
        /// Set the UInt32 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetUInt32(byte[] Data, UInt32 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt32)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(UInt32);
        }

        /// <summary>
        /// Get the UInt32 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetUInt32(byte[] Data, ref UInt32 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt32)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(UInt32));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToUInt32(ValueData, 0);
            return Pos + sizeof(UInt32);
        }

        #endregion

        #region UInt64
        /// <summary>
        /// Set the UInt64 value in the array starting from the position Pos 
        /// </summary>
        protected static int SetUInt64(byte[] Data, UInt64 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt64)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(UInt64);
        }

        /// <summary>
        /// Get the UInt64 value from the array starting from the position Pos 
        /// </summary>
        protected static int GetUInt64(byte[] Data, ref UInt64 Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(UInt64)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(UInt64));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToUInt64(ValueData, 0);
            return Pos + sizeof(UInt64);
        }

        #endregion

        #region Double
        /// <summary>
        /// Set the Double value in the array starting from the position Pos 
        /// </summary>
        protected static int SetDouble(byte[] Data, double Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(double)];
            ValueData = BitConverter.GetBytes(Value);
            // flipping the Array ....Big Endian .. 
            ValueData = FlipArray(ValueData);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length);
            return Pos + sizeof(double);
        }

        /// <summary>
        /// Get the Double value from the array starting from the position Pos 
        /// </summary>
        protected static int GetDouble(byte[] Data, ref double Value, int Pos)
        {
            byte[] ValueData = new byte[sizeof(double)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(double));

            // flipping the Array .... Big Endian ..
            ValueData = FlipArray(ValueData);
            Value = BitConverter.ToDouble(ValueData, 0);
            return Pos + sizeof(double);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Set the DateTime value in the array starting from the position Pos 
        /// </summary>
        protected static int SetDateTime(byte[] Data, DateTime Value, int Pos)
        {
            return SetInt64(Data, Value.ToBinary(), Pos);
        }

        /// <summary>
        /// Get the DateTime value from the array starting from the position Pos 
        /// </summary>
        protected static int GetDateTime(byte[] Data, ref DateTime Value, int Pos)
        {
            Int64 LongValue = 0;
            int TempPos = GetInt64(Data, ref LongValue, Pos);
            Value = DateTime.FromBinary(LongValue);
            return TempPos;
        }
        #endregion

        #region String
        /// <summary>
        /// Set the String value in the array starting from the position Pos 
        /// String will be transmitted prefixed by length in bytes and terminated by zero byte
        /// </summary>
        protected static int SetString(byte[] Data, string Value, int Pos, int StrLength)
        {

            byte[] ValueData = new byte[Value.Length + 2];
            SetUInt16(ValueData, (UInt16)Value.Length, 0); //first set the length of the string
            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            byte[] temp = new byte[Value.Length];
            temp = encoding.GetBytes(Value);

            Array.Copy(temp, 0, ValueData, 2, temp.Length);
            Array.Copy(ValueData, 0, Data, Pos, ValueData.Length); //terminate the string with zero
            return Pos + Value.Length + 2;
        }

        /// <summary>
        /// Get the String value from the array starting from the position Pos 
        /// </summary>
        protected static int GetString(byte[] Data, ref string Value, int Pos, int StrLength)
        {
            UInt16 StrRealLength = 0;
            GetUInt16(Data, ref StrRealLength, Pos);
            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Value = encoding.GetString(Data, Pos + 2, StrRealLength);
            return Pos + StrRealLength + 2;
        }
        #endregion

        #region byte[]
        /// <summary>
        /// Set the byte array in the Data array starting from the position Pos 
        /// </summary>
        protected static int SetByteArray(byte[] Data, byte[] Value, int Pos, int ArrayLength)
        {
            byte[] LengthArray = new byte[2];
            SetUInt16(LengthArray, (UInt16)Value.Length, 0);

            Array.Copy(LengthArray, 0, Data, Pos, 2);
            Array.Copy(Value, 0, Data, Pos + 2, Value.Length);
            return Pos + Value.Length + 2;

        }

        /// <summary>
        /// Get the byte array value from the Data array starting from the position Pos 
        /// </summary>
        protected static int GetByteArray(byte[] Data, ref byte[] Value, int Pos, int ArrayLength)
        {
            UInt16 RealLenght = 0;
            GetUInt16(Data, ref RealLenght, Pos);

            Value = new byte[RealLenght];
            Array.Copy(Data, Pos + 2, Value, 0, RealLenght);
            return Pos + RealLenght + 2;

        }
        #endregion



        /// <summary>
        /// Returns a string of the packet's properties and their values
        /// </summary>
        public virtual string PrintPacket()
        {
            string strPck;
            strPck = "PacketType:     " + m_PacketType.ToString() + "\r\n" +                        
                     "UserDataLength: " + m_UserDataLength.ToString() + "\r\n";

            return strPck;


        }

        /// <summary>
        /// Flips the array to send it using the big endian convention
        /// </summary>
        protected static byte[] FlipArray(byte[] Data)
        {
            byte[] FlippedData = new byte[Data.GetLength(0)];

            for (int i = 0; i < Data.GetLength(0); i++)
            {
                FlippedData[Data.GetUpperBound(0) - i] = Data[i];
            }
            return FlippedData;
        }

        /// <summary>
        /// Analyze the provided buffer and returns suitable packet instance
        /// </summary>
        public static BonCodeAJP13Packet GetPacket(byte[] Buffer)
        {
            // First of all we need to check that this is a valid packet
            // then we will return the suitable packet instance, otherwise null will be returnd.

            if (Buffer.Length < BonCodeAJP13Consts.MIN_BONCODEAJP13_PACKET_LENGTH)
            {
                return null;
            }
            if (!(Buffer[0] == BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_START && Buffer[Buffer.Length - 1] == BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_END))
            {
                return null;
            }

            byte PacketType = Buffer[BonCodeAJP13Packet.PACKET_TYPE_POS];


            if (PacketType == BonCodeAJP13ServerPacketType.SERVER_FORWARD_REQUEST)
            {

                if (Buffer.Length <= BonCodeAJP13Consts.MAX_BONCODEAJP13_PACKET_LENGTH && Buffer.Length >= BonCodeAJP13Consts.MIN_BONCODEAJP13_PACKET_LENGTH)
                            return null; // need to return new forward request new BonCodeAJP13GET(Buffer);
                        else
                            return null;

               

            }
            else if (PacketType == BonCodeAJP13PacketType.BONCODEAJP13_RESPONSE)
            {

                switch (PacketID)
                {
                    case BonCodeAJP13PacketID.SERVLETRESPONSE_ID:
                        if (Buffer.Length <= BonCodeAJP13ServletResponse.SERVLETRESPONSE_MAX_LENGTH && Buffer.Length >= BonCodeAJP13ServletResponse.SERVLETRESPONSE_MIN_LENGTH)
                            return new BonCodeAJP13ServletResponse(Buffer);
                        else
                            return null;

                }

            }
            else if (PacketType == BonCodeAJP13PacketType.BONCODEAJP13_EVENT)
            {

                switch (PacketID)
                {
                }

            }
            else if (PacketType == BonCodeAJP13PacketType.BONCODEAJP13_TWO_WAY_PACKET)
            {

                switch (PacketID)
                {
                    case BonCodeAJP13PacketID.DATALENGTHINITIALIZER_ID:
                        if (Buffer.Length == BonCodeAJP13DataLengthInitializer.DATALENGTHINITIALIZER_LENGTH)
                            return new BonCodeAJP13DataLengthInitializer(Buffer);
                        else
                            return null;
                }

            }
            else
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// Analyze the provided buffer and returns a collection of packets represented by the buffer
        /// </summary>
        public static BonCodeAJP13PacketCollection GetPackets(byte[] Buffer)
        {
            BonCodeAJP13PacketCollection AnalyzedResponses = new BonCodeAJP13PacketCollection();

            if (Buffer != null)
            {
                int start = 0;
                for (int i = 0; i < Buffer.Length; i++)
                {
                    if (Buffer[start] == BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_START)
                    {
                        UInt16 UserDataLength = 0;
                        GetUInt16(Buffer, ref UserDataLength, start + USERDATALENGTH_POS);
                        i = start + UserDataLength + 8;

                        // here we need to truncate the buffer and analyze the packet.
                        if (Buffer[i] == BonCodeAJP13PacketFormat.BONCODEAJP13_PACKET_END)
                        {

                            byte[] NewPacket = new byte[i + 1 - start];
                            Array.Copy(Buffer, start, NewPacket, 0, i + 1 - start);

                            BonCodeAJP13Packet packet = BonCodeAJP13Packet.GetPacket(NewPacket);
                            if (packet != null)
                            {
                                AnalyzedResponses.Add(packet);
                            }
                            else
                            {
                                return null;
                            }

                            start = i + 1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return AnalyzedResponses;
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}


