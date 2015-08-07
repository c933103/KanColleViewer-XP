using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Extensions;

namespace LynLogger.DataStore.IO
{
    public class DSReader : BinaryReader
    {
        public DSReader(Stream input, bool leaveOpen = false) : base(input, Encoding.Unicode, leaveOpen) { }
        
        public long Read7bInt()
        {
            var input = BaseStream;
            long val = 0;
            int i, b = 0;
            for (i = 0; i < 9; i++) {
                b = input.ReadByte();
                if (b < 0)
                    throw new EndOfStreamException();

                long l = b & 0x7F;
                val |= l << (7 * i);

                if ((b & 0x80) == 0) break;
            }
            if ((i < 8) && ((val & (1L << (7 * i + 6))) != 0)) {
                val |= -1L << (7 * i + 7);
            } else if (i == 9) {
                val |= 1L << 63;
            }

            return val;
        }

        public ulong Read7bUInt()
        {
            var input = BaseStream;
            ulong val = 0;
            int i, b = 0;
            for (i = 0; i < 9; i++) {
                b = input.ReadByte();
                if (b < 0)
                    throw new EndOfStreamException();

                ulong l = (ulong)(b & 0x7F);
                val |= l << (7 * i);

                if ((b & 0x80) == 0) break;
            }
            if (i == 9) {
                val |= 1UL << 63;
            }

            return val;
        }

        public override string ReadString()
        {
            var input = BaseStream;
            var length = Read7bUInt();
            var data = new byte[(int)length * 2];

            input.ReadFully(data);
            return Encoding.Unicode.GetString(data);
        }
    }
}
