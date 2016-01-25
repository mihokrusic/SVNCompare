using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using System.Windows;
using System.IO;
using System.Threading;
using SVNCompare;

namespace SVNCompare
{
    public enum ECompareResult { Identical = 0, Different = 1 }

    public class CompareResultItem
    {
        public CompareItem source { get; set; }
        public CompareItem target { get; set; }
        public ECompareResult result { get; set; }
        public List<String> Log { get; set; }

        public int totalFiles { get; set; }
        public int identicalFiles { get; set; }
        public int differentFiles { get; set; }
        public int leftUniqueFiles { get; set; }
        public int rightUniqueFiles { get; set; }

        public CompareResultItem()
        {
            Log = new List<String>();

            identicalFiles = differentFiles = leftUniqueFiles = rightUniqueFiles = 0;
        }
    }



    public class CompareGroup : IEnumerable<CompareItem>
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
        

        public void UpdateSVN()
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
                            Items[i].lastRevision = result.Revision;
                            Items[i].updateResult = CompareItemSVNUpdateResult.Success;
                        }
                        else
                        {
                            Items[i].lastUpdateMessage = "Failed to update.";
                            Items[i].lastRevision = 0;
                            Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                        }
                    }
                    catch (SvnException se)
                    {
                        Items[i].lastUpdateMessage = se.Message;
                        Items[i].lastRevision = 0;
                        Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                    }
                    catch (UriFormatException ufe)
                    {
                        Items[i].lastUpdateMessage = ufe.Message;
                        Items[i].lastRevision = 0;
                        Items[i].updateResult = CompareItemSVNUpdateResult.Error;
                    }
                }
            }
        }
        

        private void _CompareFolders(DirectoryInfo dir, string subFolder, bool sourceToTarget, ref CompareResultItem result)
        {
            DirectoryInfo[] currentDirectories = dir.GetDirectories("*");
            FileInfo[] currentFiles = dir.GetFiles("*.*");

            // Uspoređujemo fajlove koje smo našli u ovom folderu
            foreach (FileInfo currentFile in currentFiles)
            {
                // Thread.Sleep(500); TODO: za asinkroni test kasnije

                // Compare datoteka
                string sourcePath = (sourceToTarget ? result.source.path : result.target.path) + subFolder;
                string targetPath = (sourceToTarget ? result.target.path : result.source.path) + subFolder;
                EFileCompareResult fileCompareResult = FileComparator.CompareFiles(currentFile, sourcePath, targetPath, !sourceToTarget);

                if (fileCompareResult != EFileCompareResult.Ignore)
                    result.totalFiles++;

                switch (fileCompareResult)
                {
                    case EFileCompareResult.Identical:
                        result.identicalFiles++;
                        result.Log.Add(String.Format("File \"{0}\" is identical.", currentFile.FullName));
                        break;
                    case EFileCompareResult.Different:
                        result.differentFiles++;
                        result.Log.Add(String.Format("File \"{0}\" is different.", currentFile.FullName));
                        break;
                    case EFileCompareResult.Unique:
                        if (sourceToTarget)
                        {
                            result.leftUniqueFiles++;
                            result.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", currentFile.FullName, result.target.path + subFolder));
                        }
                        else
                        {
                            result.rightUniqueFiles++;
                            result.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", currentFile.FullName, result.target.path + subFolder));
                        }
                        break;
                }
            }

            // Idemo po svim folderima i rekurzivno i njih pretražujemo
            foreach (DirectoryInfo dirChild in currentDirectories)
            {
                // TODO: zasad fiksno izbacujemo direktorij .svn
                if (dirChild.Name == ".svn")
                    continue;

                _CompareFolders(dirChild, subFolder + dirChild.Name + @"\", sourceToTarget, ref result);
            }
        }


        public bool Compare(int baseIndex, out List<CompareResultItem> result)
        {
            List<CompareResultItem> compareResults = new List<CompareResultItem>();

            // Dohvaćamo sve fajlove u source direktoriju
            DirectoryInfo sourceRoot = new DirectoryInfo(Items[baseIndex].path);

            // Idemo po svim lokacijama za compare
            for (int i = 0; i < Items.Count; i++)
            {
                // Preskačemo compare base lokacije sa samom sobom
                if (i == baseIndex)
                    continue;

                DirectoryInfo targetRoot = new DirectoryInfo(Items[i].path);

                // Kreiramo result item za usporedbu ove lokacije sa source lokacijom
                CompareResultItem compareResultItem = new CompareResultItem();
                compareResultItem.source = Items[baseIndex];
                compareResultItem.target = Items[i];

                // Uspoređujemo source --> target
                _CompareFolders(sourceRoot, @"\", true, ref compareResultItem);

                // Uspoređujemo target --> source
                _CompareFolders(targetRoot, @"\", false, ref compareResultItem);

                compareResultItem.result = (compareResultItem.identicalFiles != compareResultItem.totalFiles ? ECompareResult.Different : ECompareResult.Identical);
                compareResults.Add(compareResultItem);
            }

            result = compareResults;
            return true;
        }
    }
}
