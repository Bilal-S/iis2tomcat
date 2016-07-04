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
                p_UserDataLength = System.Convert.ToUInt16(content.Length - 4);
            }
            catch
            {
                //do nothing for now
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
            catch
            {
                //do nothing for now
            }
        } 

        #endregion

        #region Methods
        //no specific method for this class
        #endregion
    }
}
