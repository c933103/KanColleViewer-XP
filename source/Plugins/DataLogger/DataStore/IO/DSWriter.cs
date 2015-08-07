using LynLogger.DataStore.Premitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.IO
{
    public class DSWriter : BinaryWriter
    {
        public DSWriter(Stream output) : base(output, Encoding.Unicode) { }

        public void Write7b(ulong val)
        {
            var output = BaseStream;
            for (int i = 0; i < 9; i++) {
                byte thisByte = (byte)(val & 0x7F);
                val >>= 7;
                if (val != 0) {
                    thisByte |= 0x80;
                    output.WriteByte(thisByte);
                } else {
                    output.WriteByte(thisByte);
                    break;
                }
            }
        }

        public void Write7b(long val)
        {
            var output = BaseStream;
            for (int i = 0; i < 9; i++) {
                byte thisByte = (byte)(val & 0x7F);
                val >>= 7;
                if (((val != 0) && (val != -1)) || ((thisByte & 0x40) != (val & 0x40))) {
                    thisByte |= 0x80;
                    output.WriteByte(thisByte);
                } else {
                    output.WriteByte(thisByte);
                    break;
                }
            }
        }
        
        public void Write7bInt(long val) { Write7b(val); }
        public void Write7bUInt(ulong val) { Write7b(val); }

        public override void Write(string val)
        {
            var output = BaseStream;
            byte[] encoded = Encoding.Unicode.GetBytes(val);
            System.Diagnostics.Trace.Assert(encoded.Length == val.Length * 2);

            Write7bUInt((uint)val.Length);
            output.Write(encoded, 0, encoded.Length);
        }

        public void Write(IEnumerable<TypeIdentifier> identifiers)
        {
            foreach(var id in identifiers) {
                Write7bUInt((uint)id);
            }
        }
    }
}
