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
    class DsString : StoragePremitive
    {
        private string value = "";

        public override IEnumerable<TypeIdentifier> Type => CollectionsEx.AsEnumerable(TypeIdentifier.String);
        public string Value => value;

        public DsString() { }
        public DsString(string value) { this.value = value; }
        public DsString(DSReader input) { value = input.ReadString(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }

        public static implicit operator string (DsString v)
        {
            return v?.Value;
        }

        public static implicit operator DsString(string v)
        {
            return new DsString(v);
        }
    }
}
