using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;
using LynLogger.DataStore.Extensions;
using LynLogger.Utilities;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class String : StoragePremitive
    {
        private string value = "";

        public override IEnumerable<TypeIdentifier> Type => CollectionsEx.AsEnumerable(TypeIdentifier.String);
        public string Value => value;

        public String() { }
        public String(string value) { this.value = value; }
        public String(DSReader input) { value = input.ReadString(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }

        public static implicit operator string (String v)
        {
            return v?.Value;
        }

        public static implicit operator String(string v)
        {
            return new String(v);
        }
    }
}
