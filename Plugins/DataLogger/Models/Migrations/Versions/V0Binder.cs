using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Migrations.Versions
{
    class V0Binder : SerializationBinder
    {
        private static readonly Dictionary<string, string> typeMap;

        static V0Binder()
        {
            typeMap = new Dictionary<string, string>();
            typeMap.Add("LynLogger.Models.DataStore", "LynLogger.Models.Migrations.Versions.V0.DataStore");
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if(typeMap.ContainsKey(typeName)) {
                typeName = typeMap[typeName];
            }

            return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
        }
    }
}
