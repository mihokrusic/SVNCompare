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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SVNModels
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

        public CompareItem DefaultItem { get; private set; }

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
                string sourcePath = (sourceToTarget ? result.Source.Path : result.Target.Path) + subFolder;
                string targetPath = (sourceToTarget ? result.Target.Path : result.Source.Path) + subFolder;
                EFileCompareResult fileCompareResult = FileComparator.CompareFiles(currentFile, sourcePath, targetPath, !sourceToTarget);

                if (fileCompareResult != EFileCompareResult.Ignore)
                    result.TotalFiles++;

                switch (fileCompareResult)
                {
                    case EFileCompareResult.Identical:
                        result.IdenticalFiles++;
                        result.FileResults.Add(
                            new CompareFileResult() { 
                                Status = CompareFileStatus.Identical,
                                FileLeft = Path.Combine(sourcePath, currentFile.Name),
                                FileRight = Path.Combine(targetPath, currentFile.Name)
                            }
                        );
                        break;
                    case EFileCompareResult.Different:
                        result.DifferentFiles++;
                        result.FileResults.Add(
                            new CompareFileResult()
                            {
                                Status = CompareFileStatus.Different,
                                FileLeft = Path.Combine(sourcePath, currentFile.Name),
                                FileRight = Path.Combine(targetPath, currentFile.Name)
                            }
                        );
                        break;
                    case EFileCompareResult.Unique:
                        if (sourceToTarget)
                        {
                            result.LeftUniqueFiles++;
                            result.FileResults.Add(
                                new CompareFileResult()
                                {
                                    Status = CompareFileStatus.LeftUnique,
                                    FileLeft = Path.Combine(sourcePath, currentFile.Name),
                                    FileRight = ""
                                }
                            );
                        }
                        else
                        {
                            result.RightUniqueFiles++;
                            result.FileResults.Add(
                                new CompareFileResult()
                                {
                                    Status = CompareFileStatus.RightUnique,
                                    FileLeft = "",
                                    FileRight = Path.Combine(targetPath, currentFile.Name)
                                }
                            );
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
                item.CompareResult.Source = DefaultItem;
                item.CompareResult.Target = item;

                // Uspoređujemo source --> target
                _CompareFolders(sourceRoot, @"\", true, ref item.CompareResult);

                // Uspoređujemo target --> source
                _CompareFolders(targetRoot, @"\", false, ref item.CompareResult);

                item.Status = (item.CompareResult.IdenticalFiles != item.CompareResult.TotalFiles ? CompareItemStatus.Different : CompareItemStatus.Identical);
            }

            return true;
        }

        public void SetDefaultItem(CompareItem newDefaultItem)
        {
            DefaultItem = newDefaultItem;

            foreach (CompareItem item in Items)
            {
                item.Status = (item == DefaultItem ? CompareItemStatus.Base : CompareItemStatus.Unknown);
                item.CompareResult.Clear();
            }
        }
    }
}
