using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LynLogger.Models
{
    [Serializable]
    public abstract class NotificationSourceObject : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        private Lazy<string[]> allProperties;

        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            allProperties = new Lazy<string[]>(() => {
                return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(info => info.Name).ToArray();
            });
        }

        protected NotificationSourceObject()
        {
            allProperties = new Lazy<string[]>(() => {
                return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(info => info.Name).ToArray();
            });
        }

        protected void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            handler(this, new PropertyChangedEventArgs(property));
        }

        protected void RaiseMultiPropertyChanged(params string[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                properties = allProperties.Value;
            }
            foreach(var prop in properties) {
                handler(this, new PropertyChangedEventArgs(prop));
            }
        }

        protected void RaisePropertyChanged(params Expression<Func<object, object>>[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                RaiseMultiPropertyChanged();
                return;
            }
            foreach(var prop in properties) {
                var expr = prop.Body;
                while(expr is UnaryExpression) {
                    expr = ((UnaryExpression)expr).Operand;
                }
                if(expr is MemberExpression) {
                    var targetMember = ((MemberExpression)expr).Member;
                    if(targetMember.MemberType == MemberTypes.Property) {
                        handler(this, new PropertyChangedEventArgs(targetMember.Name));
                    }
                }
            }
        }
    }
}
