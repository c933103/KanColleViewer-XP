using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Scavenge
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class ScavengeTargetTypeAttribute : Attribute
    {
        readonly Type targetKeyType;
        readonly Type targetValueType;

        public ScavengeTargetTypeAttribute(Type targetKeyType, Type targetValueType)
        {
            this.targetKeyType = targetKeyType;
            this.targetValueType = targetValueType;
        }

        public Type TargetKeyType
        {
            get { return targetKeyType; }
        }

        public Type TargetValueType
        {
            get { return targetValueType; }
        }
    }
}
