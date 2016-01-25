using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SVNCompare
{
    public enum EFileCompareResult { Ignore, Identical, Different, Unique }

    public static class FileComparator
    {
        public static EFileCompareResult CompareFiles(FileInfo sourceFile, string pathSource, string pathTarget, bool checkOnlyUnique)
        {
            // Kreiramo full path na target lokaciji
            string filePathAtTarget = String.Format(@"{0}\{1}", pathTarget, sourceFile.Name);

            // Provjeravamo da fajl postoji na target lokaciji
            if (!File.Exists(filePathAtTarget))
                return EFileCompareResult.Unique;

            if (checkOnlyUnique)
                return EFileCompareResult.Ignore;

            // Onda provjeravamo da im je veličina ista
            FileInfo targetFile = new FileInfo(filePathAtTarget);

            if (targetFile.Length != sourceFile.Length)
                return EFileCompareResult.Different;

            // Na kraju provjeravamo da su stvarno fajlovi isti
            // TODO: nedostaje dio za provjeru da su stvarno fajlovi razliciti ili isti ako imaju istu velicinu

            // Stvarno su fajlovi isti
            return EFileCompareResult.Identical;
        }
    }
}
