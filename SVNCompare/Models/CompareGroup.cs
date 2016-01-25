using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using System.Windows;

namespace SVNCompare.Models
{
    class CompareGroup: IEnumerable<CompareItem>
    {
        public List<CompareItem> Items;
        public IEnumerator<CompareItem> GetEnumerator() { return Items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version of the method
            return this.GetEnumerator();
        }

        public CompareGroup()
        {
            Items = new List<CompareItem>();
        }

        public void SVNUpdate()
        {
            //SvnUpdateResult provides info about what happened during a checkout
            SvnUpdateResult result;

            //SvnCheckoutArgs wraps all of the options for the 'svn checkout' function
            SvnUpdateArgs args = new SvnUpdateArgs();

            //path is the path where the local working copy will end up
            string path;

            using (SvnClient client = new SvnClient())
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    try
                    {
                        path = Items[i].path;

                        if (client.Update(path, args, out result))
                        {
                            Items[i].lastUpdateMessage = "Updated to revision " + result.Revision + ".";
                            Items[i].updateResult = CompareItemSVNUpdateResult.Success;
                        }
                        else
                        {
                            Items[i].lastUpdateMessage = "Failed to update.";
                            Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                        }
                    }
                    catch (SvnException se)
                    {
                        Items[i].lastUpdateMessage = se.Message;
                        Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                    }
                    catch (UriFormatException ufe)
                    {
                        Items[i].lastUpdateMessage = ufe.Message;
                        Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                    }
                }
            }
        }
    }
}
