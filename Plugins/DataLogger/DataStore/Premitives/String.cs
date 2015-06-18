using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;
using LynLogger.DataStore.Extensions;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class String : StoragePremitive
    {
        private string value = "";

        public override IEnumerable<TypeIdentifier> Type => Collections.AsEnumerable(TypeIdentifier.String);
        public string Value => value;

        public String() { }
        public String(string value) { this.value = value; }
        public String(DSReader input) { value = input.ReadString(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }

        public static explicit operator string (String v)
        {
            return v?.Value;
        }
    }
}
