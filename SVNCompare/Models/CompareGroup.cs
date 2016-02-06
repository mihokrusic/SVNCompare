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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SVNCompare.Models
{
    public class CompareGroupArguments
    {
        public List<String> IgnoreDirectories { get; set; }
        public List<String> IgnoreFiles { get; set; }

        public CompareGroupArguments()
        {
            IgnoreDirectories = new List<String>();
            IgnoreFiles = new List<String>();
        }
    }


    public class CompareGroup 
    {
        public string Name { get; set; }
        public CompareItem DefaultItem { get; set; }
        public ObservableCollection<CompareItem> Items { get; set; }

        public CompareGroup()
        {
            Items = new ObservableCollection<CompareItem>();
        }
        

        public void UpdateSVN()
        {
            /*//SvnUpdateResult provides info about what happened during a checkout
            SvnUpdateResult result;

            //SvnCheckoutArgs wraps all of the options for the 'svn checkout' function
            SvnUpdateArgs args = new SvnUpdateArgs();

            using (SvnClient client = new SvnClient())
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    try
                    {
                        if (client.Update(Items[i].Path, args, out result))
                        {
                            Items[i].lastUpdateMessage = "Updated to revision " + result.Revision + ".";
                            Items[i].lastRevision = result.Revision;
                            Items[i].SVNUpdateStatus = CompareItemSVNUpdateStatus.Success;
                        }
                        else
                        {
                            Items[i].lastUpdateMessage = "Failed to update.";
                            Items[i].lastRevision = 0;
                            Items[i].SVNUpdateStatus = CompareItemSVNUpdateStatus.Error;
                        }
                    }
                    catch (SvnException se)
                    {
                        Items[i].lastUpdateMessage = se.Message;
                        Items[i].lastRevision = 0;
                        Items[i].SVNUpdateStatus = CompareItemSVNUpdateStatus.Error;
                    }
                    catch (UriFormatException ufe)
                    {
                        Items[i].lastUpdateMessage = ufe.Message;
                        Items[i].lastRevision = 0;
                        Items[i].SVNUpdateStatus = CompareItemSVNUpdateStatus.Error;
                    }
                }
            }*/
        }
        

        private void _CompareFolders(DirectoryInfo dir, string subFolder, bool sourceToTarget, ref CompareResultItem result)
        {
            DirectoryInfo[] currentDirectories = dir.GetDirectories("*");
            FileInfo[] currentFiles = dir.GetFiles("*.*");

            // Idemo po svim folderima i rekurzivno i njih pretražujemo
            foreach (DirectoryInfo dirChild in currentDirectories)
            {
                if (dirChild.Name == ".svn") // TODO
                    continue;

                _CompareFolders(dirChild, Path.Combine(subFolder, dirChild.Name, @"\"), sourceToTarget, ref result);
            }

            // Uspoređujemo fajlove koje smo našli u ovom folderu
            foreach (FileInfo currentFile in currentFiles)
            {
                // Compare datoteka
                string sourcePath = (sourceToTarget ? result.source.Path : result.target.Path) + subFolder;
                string targetPath = (sourceToTarget ? result.target.Path : result.source.Path) + subFolder;
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
                            result.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", currentFile.FullName, result.target.Path + subFolder));
                        }
                        else
                        {
                            result.rightUniqueFiles++;
                            result.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", currentFile.FullName, result.target.Path + subFolder));
                        }
                        break;
                }
            }
        }


        public bool Compare()
        {
            // Dohvaćamo sve fajlove u source direktoriju
            DirectoryInfo sourceRoot = new DirectoryInfo(DefaultItem.Path);

            // Idemo po svim lokacijama za compare
            foreach (CompareItem item in Items)
            {
                // Preskačemo compare base lokacije sa samom sobom
                if (item == DefaultItem)
                    continue;

                DirectoryInfo targetRoot = new DirectoryInfo(item.Path);

                // Kreiramo result item za usporedbu ove lokacije sa source lokacijom
                item.CompareResult.Clear();
                item.CompareResult.source = DefaultItem;
                item.CompareResult.target = item;

                // Uspoređujemo source --> target
                _CompareFolders(sourceRoot, @"\", true, ref item.CompareResult);

                // Uspoređujemo target --> source
                _CompareFolders(targetRoot, @"\", false, ref item.CompareResult);

                item.Status = (item.CompareResult.identicalFiles != item.CompareResult.totalFiles ? CompareItemStatus.Different : CompareItemStatus.Identical);
            }

            return true;
        }
    }
}
