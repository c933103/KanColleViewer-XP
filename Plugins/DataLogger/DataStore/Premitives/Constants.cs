using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.Premitives
{
    public enum TypeIdentifier
    {
        /// <summary>
        /// Indicates that the respective field is a byte array, its value consists of an unsigned integer that indicates its length, and the blob data.
        /// </summary>
        Blob = 0,

        /// <summary>
        /// Indicates that the respective field is a signed integer, its value should be read with the Read7bInt method.
        /// </summary>
        Int = 1,

        /// <summary>
        /// Indicates that the respective field is an unsigned integer, its value should be read with the Read7bUInt method.
        /// </summary>
        UInt = 2,

        /// <summary>
        /// Indicates that the respective field is an double-precision floating point number, its value should be read with the ReadDouble method.
        /// </summary>
        Double = 3,

        /// <summary>
        /// Indicates that the respective field is an double-precision floating point number, its value should be read with the ReadDouble method.
        /// </summary>
        Decimal = 4,

        /// <summary>
        /// Indicates that the respective field is a string, its value should be read with the ReadString method.
        /// </summary>
        String = 5,

        /// <summary>
        /// Indicates that the respective field is a list of values, the data field begins with a type identifier, followed by item count, followed by the data of each item.
        /// </summary>
        List = 6,

        /// <summary>
        /// Indicates that the respective field is a dictionary, the data field begins with two type identifiers(key and value respectively), followed by item count, followed by the key and value of each pair.
        /// </summary>
        Dictionary = 7,

        /// <summary>
        /// Indicates that the respective field is a compound type, the data field begins with a field count, followed by each field. Each field is made up of a field identifier, which is an UInt and only significant to the compound type, followed by its type identifier, followed by the value.
        /// </summary>
        Compound = 8,

        /// <summary>
        /// Indicates that the respective field has no value.
        /// </summary>
        Null = 9,

        /// <summary>
        /// Represents a generic type with no data field.
        /// </summary>
        Undefined = 127
    }
}
