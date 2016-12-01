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

using BonCodeAJP13.TomcatPackets;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BonCodeAJP13
{
    /// <summary>
    /// Used to hold a collection of Received packets. 
    /// Can contain multiple tomcat or server packets.
    /// </summary>
    public class BonCodeAJP13PacketCollection : IEnumerable<BonCodeAJP13Packet>
    {
        // instance store
        private List<BonCodeAJP13Packet> packets = new List<BonCodeAJP13Packet>();

        // enumeration links
        IEnumerator IEnumerable.GetEnumerator()
        {           
            return ((IEnumerable<BonCodeAJP13Packet>)packets).GetEnumerator();
        }

        public IEnumerator<BonCodeAJP13Packet> GetEnumerator() {
            return this.packets.GetEnumerator();
        }

        // getters and setters
        public int Count { get { return (packets.Count); } }

        public BonCodeAJP13Packet this[int index]
        {
            get
            {
                return ((BonCodeAJP13Packet)packets[index]);
            }
            set
            {
                packets[index] = value;
            }
        }

      
        // public methods to collection
        public void Add(BonCodeAJP13Packet value)
        {
            packets.Add(value);
        }

        public int IndexOf(BonCodeAJP13Packet value)
        {
            return (packets.IndexOf(value));
        }

        public void Insert(int index, BonCodeAJP13Packet value)
        {
            packets.Insert(index, value);
        }

        public void Remove(BonCodeAJP13Packet value)
        {
            packets.Remove(value);
        }

        public void Clear()
        {
            packets.Clear();
        }

        public bool Contains(BonCodeAJP13Packet value)
        {
            // If value is not of type BonCodeAJP13Packet, this will return false.
            return (packets.Contains(value));
        }

        // used to convert to array so that all references are resolved before looping in foreach loops
        public IEnumerable<TomcatReturn> ToArray()
        {
            return (Array.ConvertAll(packets.ToArray(), item => (TomcatReturn)item));
        }
    }
}
