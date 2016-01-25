using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SVNCompare.Utility
{
    public enum EFileCompareResult { Ignore, Identical, Different, LeftUnique, RightUnique }

    public static class Comparator
    {
        public static EFileCompareResult CompareFiles(FileInfo sourceFile, string pathSource, string pathTarget, bool checkTargetUniqueFiles)
        {
            // Kreiramo full path na target lokaciji
            string filePathAtTarget = String.Format(@"{0}\{1}", pathTarget, sourceFile.Name);

            // Provjeravamo da fajl postoji na target lokaciji
            if (!File.Exists(filePathAtTarget))
                return (checkTargetUniqueFiles ? EFileCompareResult.RightUnique : EFileCompareResult.LeftUnique);

            if (checkTargetUniqueFiles)
                return EFileCompareResult.Ignore;

            // Onda provjeravamo da im je veličina ista
            FileInfo targetFile = new FileInfo(filePathAtTarget);

            if (targetFile.Length != sourceFile.Length)
                return EFileCompareResult.Different;

            // Na kraju provjeravamo da su stvarno fajlovi isti
            // TODO

            // Stvarno su fajlovi isti
            return EFileCompareResult.Identical;
        }
    }
}
