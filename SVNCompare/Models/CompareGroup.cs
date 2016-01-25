using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using System.Windows;
using System.IO;

namespace SVNCompare.Models
{
    public enum ECompareResult { Identical = 0, Different = 1 }

    public class CompareResultItem
    {
        public CompareItem source { get; set; }
        public CompareItem target { get; set; }
        public ECompareResult result { get; set; }
        public List<String> Log { get; set; }

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



        public bool Compare(int baseIndex, out List<CompareResultItem> result)
        {
            List<CompareResultItem> compareResults = new List<CompareResultItem>();

            string sourcePath = Items[baseIndex].path;

            // Dohvaćamo sve fajlove u glavnom direktoriju
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            FileInfo[] sourceFiles = sourceDirectory.GetFiles("*.*");

            for (int i = 0; i < Items.Count; i++)
            {
                // Preskačemo compare base lokacije sa samom sobom
                if (i == baseIndex)
                    continue;

                // Kreiramo result item za ovu usporedbu
                CompareResultItem compareResultItem = new CompareResultItem();
                compareResultItem.source = Items[baseIndex];
                compareResultItem.target = Items[i];

                // Reset rezultata
                ECompareResult compareResult = ECompareResult.Identical;

                // Pretražujemo fajlove source --> target
                foreach (FileInfo sourceFile in sourceFiles)
                {
                    string filePathAtTarget = compareResultItem.target.path + @"\" + sourceFile.Name;

                    // Prvo provjeravamo da fajl postoji na target lokaciji
                    if (!File.Exists(filePathAtTarget))
                    {
                        compareResult = ECompareResult.Different;
                        compareResultItem.leftUniqueFiles++;
                        compareResultItem.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", sourceFile.Name, compareResultItem.target.path));
                        continue;
                    }

                    // Onda provjeravamo da im je veličina ista
                    FileInfo targetFile = new FileInfo(filePathAtTarget);

                    if (targetFile.Length != sourceFile.Length)
                    {
                        compareResult = ECompareResult.Different;
                        compareResultItem.differentFiles++;
                        compareResultItem.Log.Add(String.Format("File \"{0}\" is different.", sourceFile.Name));
                        continue;
                    }

                    // Na kraju provjeravamo da su stvarno fajlovi isti

                    // Stvarno su fajlovi isti
                    compareResultItem.identicalFiles++;
                    compareResultItem.Log.Add(String.Format("File \"{0}\" is identical.", sourceFile.Name));
                }

                // Pretražujemo fajlove target --> source
                DirectoryInfo targetDirectory = new DirectoryInfo(Items[i].path);
                FileInfo[] targetFiles = targetDirectory.GetFiles("*.*");
                foreach (FileInfo targetFile in targetFiles)
                {
                    string filePathAtSource = sourcePath + @"\" + targetFile.Name;

                    // Samo provjeravamo da li postoji fajl na source lokaciji, ostalo smo u obrnutom smjeru vec provjerili
                    if (!File.Exists(filePathAtSource))
                    {
                        compareResult = ECompareResult.Different;
                        compareResultItem.rightUniqueFiles++;
                        compareResultItem.Log.Add(String.Format("File \"{0}\" does not exist on path \"{1}\")", targetFile.Name, sourcePath));
                        continue;
                    }
                }

                compareResultItem.result = compareResult;
                compareResults.Add(compareResultItem);

            }

            result = compareResults;
            return true;
        }
    }
}
