using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Xml;
using SharpSvn;
using System.Collections.ObjectModel;

namespace SVNModels
{
    public class CompareGroups: _BaseModel
    {
        public ObservableCollection<CompareGroup> Groups { get; set; }


        public CompareGroups()
        {
            Groups = new ObservableCollection<CompareGroup>();
        }


        public void LoadFromXML(string xmlPath)
        {
            Console.WriteLine("Loading data from XML");
            Groups.Clear();

            XmlDocument xmlDoc = new XmlDocument();
            
            try
            {
                xmlDoc.Load(xmlPath);
                XmlNodeList xmlGroups = xmlDoc.SelectNodes("/groups/group");

                // Dohvaćamo grupe
                foreach (XmlNode xmlGroup in xmlGroups)
                {
                    CompareGroup newGroup = new CompareGroup();
                    newGroup.Name = xmlGroup.SelectSingleNode("name").InnerText;

                    // Dohvaćamo iteme
                    XmlNodeList xmlItems = xmlGroup.SelectNodes("items/item");

                    foreach (XmlNode xmlItem in xmlItems)
                    {
                        CompareItem newItem = new CompareItem();
                        newItem.Group = newGroup;
                        newItem.Name = xmlItem.SelectSingleNode("name").InnerText;
                        newItem.Path = xmlItem.SelectSingleNode("path").InnerText;

                        XmlNode defaultItem = xmlItem.SelectSingleNode("default");
                        newItem.Default = (defaultItem != null) && (Convert.ToBoolean(defaultItem.InnerText));

                        if (newItem.Default)
                        {
                            newGroup.SetDefaultItem(newItem);

                            newItem.Status = CompareItemStatus.Base;
                        }
                        else
                        {
                            newItem.Status = CompareItemStatus.Unknown;
                        }

                        newGroup.Items.Add(newItem);
                    }

                    Groups.Add(newGroup);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public void Compare()
        {
            foreach (CompareGroup group in Groups)
            {
                group.Compare();
            }
        }
    }
}
