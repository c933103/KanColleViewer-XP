using LynLogger.DataStore.IO;
using LynLogger.DataStore.Premitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.Utilities;

namespace LynLogger.DataStore.Extensions
{
    static class PremitiveExtensions
    {
        public static void Serialize(this StoragePremitive p, DSWriter w)
        {
            if(p == null) {
                w.Write(CollectionsEx.AsEnumerable(TypeIdentifier.Null));
            } else {
                p.SerializeNonNull(w);
            }
        }
    }
}
