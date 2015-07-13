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


// Since we are using a DLL we cannot use built in functionality of .net to manage 
// settings/properties for the connector. We need to roll our own.
// This class manages persistence to setting file.
// It reads the settings from XML file:
/* SAMPLE SETTING FILE: BonCodeAJP13.settings
 
<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <Port>8009</Port>
  <Server>localhost</Server>   
  <MaxConnections>250</MaxConnections>
  <FlushThreshold>0</FlushThreshold>
  <Server>localhost</Server>   
  <LogLevel>1</LogLevel> 
</Settings>
  
 */


using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Web.Caching;



namespace BonCodeAJP13.Config
{
    class BonCodeAJP13SettingProvider : SettingsProvider
    {

        #region Constants
        const string XMLROOT = "Settings";
        #endregion

        #region Instance data

        // private XmlDocument p_settingsXmlDoc = null;

        #endregion

        #region Properties

        public override string ApplicationName
        {
            get
            {
                return (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            }
            set
            {
                // Do nothing.
            }
        }

        public override string Name
        {
            get { return "BonCodeAJP13SettingProvider"; }
        }

        private XmlDocument SettingsXML
        {
            get
            {
                XmlDocument settingsXmlDoc = new XmlDocument();
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one that is empty.
                if (System.Web.HttpRuntime.Cache["xmlsettingfile"] == null)
                {
                    try
                    {
                        settingsXmlDoc.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        //create new blank doc in memory to return to process
                        XmlDeclaration dec = settingsXmlDoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        settingsXmlDoc.AppendChild(dec);

                        XmlNode nodeRoot = default(XmlNode);

                        nodeRoot = settingsXmlDoc.CreateNode(XmlNodeType.Element, XMLROOT, "");
                        settingsXmlDoc.AppendChild(nodeRoot);
                    }

                    System.Web.HttpRuntime.Cache.Insert("xmlsettingfile", settingsXmlDoc);

                }
                else
                {
                    //return from cache and convert
                    settingsXmlDoc = (XmlDocument)System.Web.HttpRuntime.Cache["xmlsettingfile"];
                }

                return settingsXmlDoc;
            }
        }
        #endregion //properties



        #region Methods
        //specialized init
        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(this.ApplicationName, col);
        }

        /// <summary>
        /// Get path in which the properties XML file should be located. This is where the .dll is located itself.           
        /// If dll is in GAC this will switch to windows\system32 dir
        /// </summary>
        public virtual string GetAppSettingsPath()
        {

            return BonCodeAJP13Logger.GetAssemblyDirectory();
        }

        /// <summary>
        /// Determine file name containing properties. This is in XML file. Should return: BonCodeAJP13.settings            
        /// </summary>
        public virtual string GetAppSettingsFilename()
        {
            //Used to determine the filename to store the settings
            return ApplicationName + ".settings";
        }

        /// <summary>
        /// Get the collection of properties available.            
        /// </summary>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            //Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {

                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        /// <summary>
        /// Get a singular property value from XML file. If not found return default from code.            
        /// </summary>
        private string GetValue(SettingsProperty setting)
        {
            string ret = "";

            try
            {
                ret = SettingsXML.SelectSingleNode(XMLROOT + "/" + setting.Name).InnerText;
            }

            catch (Exception)
            {
                //return default value if possible otherwise return blank string
                if ((setting.DefaultValue != null))
                {
                    ret = setting.DefaultValue.ToString();
                }
            }

            return ret;
        }


        //we do not need to set properties at this point
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            throw new NotImplementedException();
        }
        #endregion  //methods
    }
}
