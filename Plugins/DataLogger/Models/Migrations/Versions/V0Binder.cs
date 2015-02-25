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
            typeMap.Add("LynLogger.Models.ShipHistory", "LynLogger.Models.Migrations.Versions.V0.ShipHistory");
            typeMap.Add("System.Collections.Generic.Dictionary`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[LynLogger.Models.ShipHistory, LynLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]", "System.Collections.Generic.Dictionary`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[LynLogger.Models.Migrations.Versions.V0.ShipHistory, LynLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");
            typeMap.Add("System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[LynLogger.Models.ShipHistory, LynLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]", "System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[LynLogger.Models.Migrations.Versions.V0.ShipHistory, LynLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");

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
