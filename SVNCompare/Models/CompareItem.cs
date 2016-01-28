using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNCompare
{
    public enum CompareItemSVNUpdateResult { None = 0, Success = 1, Error = 2 }

    public class CompareItem
    {
        public int position { get; private set; } // TODO: ne potrebna veza sa UI, ukloniti
        public string path { get; private set; }

        public long lastRevision { get; set; } 
        public string lastUpdateMessage { get; set; }
        public CompareItemSVNUpdateResult updateResult { get; set; } 

        public CompareItem(string path, int position)
        {
            this.path = path;
            this.position = position;
            this.updateResult = CompareItemSVNUpdateResult.None;
        }
    }
}
