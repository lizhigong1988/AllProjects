using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonLib
{
    public class ServerConfigHeper
    {  
        /// <summary>
        /// 配置文件KEY定义
        /// </summary>
        public enum CONFIG_KEYS
        {
            SERVER_PORT,
            DATABASE_TYPE,
        };
        
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string ReadConfig(CONFIG_KEYS key)
        {
            //读取配置文件中的ip和端口号
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create("Config/ServerConfig.xml", settings);
            xmlDoc.Load(reader);
            reader.Close();
            XmlNode elements = xmlDoc.SelectSingleNode("elements");
            XmlNodeList elementList = elements.ChildNodes;
            foreach (XmlElement elementNode in elementList)
            {
                if (key.ToString() == elementNode.GetAttribute("key"))
                {
                    return elementNode.GetAttribute("value");
                }
            }
            return "";
        }

        /// <summary>
        /// 写配置文件
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static void SetConfig(CONFIG_KEYS key, string value)
        {
            //读取配置文件中的ip和端口号
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create("Config/ServerConfig.xml", settings);
            xmlDoc.Load(reader);
            reader.Close();
            XmlNode elements = xmlDoc.SelectSingleNode("elements");
            XmlNodeList elementList = elements.ChildNodes;
            foreach (XmlElement elementNode in elementList)
            {
                if (key.ToString() == elementNode.GetAttribute("key"))
                {
                    elementNode.SetAttribute("value", value);
                    break;
                }
            }
            xmlDoc.Save("Config/ServerConfig.xml");
        }

    }
}
