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
    class Blob : StoragePremitive
    {
        public override IEnumerable<TypeIdentifier> Type => Collections.AsEnumerable(TypeIdentifier.Blob);

        private byte[] data;
        public byte[] Data => (byte[])data.Clone();

        public Blob() { data = new byte[0]; }
        public Blob(byte[] data) { this.data = (byte[])data.Clone(); }
        public Blob(byte[] buf, int start, int len)
        {
            data = new byte[len];
            Array.Copy(buf, start, data, 0, len);
        }
        public Blob(DSReader input)
        {
            data = new byte[(int)input.Read7bUInt()];
            input.ReadFully(data);
        }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt((uint)data.Length);
            output.Write(data);
        }
    }
}
