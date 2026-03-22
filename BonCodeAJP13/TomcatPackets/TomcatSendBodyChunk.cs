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
// This is the main container of data. Returned data here will flow into browser
//==============================================================================
//==============================================================================

using System;

namespace BonCodeAJP13.TomcatPackets
{
    public class TomcatSendBodyChunk : TomcatReturn
    {

        #region data Members
        
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "TOMCAT SEND BODY CHUNK RETURN";

        #endregion

        
        #region Properties
        //currently none
        #endregion


        //contructors specific to this type of packet
        #region Constructors

        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public TomcatSendBodyChunk()
        {

        }

        /// <summary>
        /// constructor with data to initialize
        /// </summary>
        public TomcatSendBodyChunk(byte[] content)
        {
            p_ByteStore = content;
            try
            {
                p_PacketLength = content.Length;
                // Check if this is proper SendBodyChunk format (starts with 0x03)
                if (content.Length > 0 && content[0] == 0x03)
                {
                    // Read chunk_length from bytes 1-2 (big-endian uint16)
                    p_UserDataLength = (ushort)((content[1] << 8) | content[2]);
                }
                else
                {
                    // Fall back to legacy calculation for backward compatibility
                    p_UserDataLength = System.Convert.ToUInt16(content.Length - 4);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TomcatSendBodyChunk(byte[]): {ex.Message}");
            }
        }

        /// <summary>
        /// constructor with data to initialize using string as input
        /// </summary>
        public TomcatSendBodyChunk(string strContent)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            //when content is retrieved the first and last 4 bytes are dropped so we are adding space chars
            byte[] content =  encoder.GetBytes("    " + strContent + "    ");

            p_ByteStore = content;
            try
            {
                p_PacketLength = content.Length;
                p_UserDataLength = System.Convert.ToUInt16(content.Length - 4);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TomcatSendBodyChunk(string): {ex.Message}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the user data bytes for SendBodyChunk packets.
        /// For proper SendBodyChunk format (starts with 0x03), user data starts at offset 3 and has length p_UserDataLength.
        /// For legacy format, falls back to base class behavior.
        /// </summary>
        public override byte[] GetUserDataBytes()
        {
            if (p_ByteStore == null)
            {
                System.Diagnostics.Debug.WriteLine("GetUserDataBytes called with null p_ByteStore");
                return new byte[] { };
            }

            // Check if this is proper SendBodyChunk format
            if (p_ByteStore.Length > 0 && p_ByteStore[0] == 0x03)
            {
                // SendBodyChunk format: user data starts at offset 3
                if (p_ByteStore.Length >= 3 + p_UserDataLength)
                {
                    byte[] userArray = new byte[p_UserDataLength];
                    Array.Copy(p_ByteStore, 3, userArray, 0, p_UserDataLength);
                    return userArray;
                }
                else
                {
                    // Return empty array if data is malformed
                    return new byte[] { };
                }
            }
            else
            {
                // Legacy format: use base class behavior
                return base.GetUserDataBytes();
            }
        }

        #endregion
    }
}
