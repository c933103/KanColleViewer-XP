using System;
using System.Collections.Generic;
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
        protected virtual IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency { get { return null; } }

        [NonSerialized]
        private Dictionary<string, HashSet<string>> _propertyChangePath;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        private Lazy<string[]> allProperties;

        protected NotificationSourceObject() { Deserialized(new StreamingContext()); }

        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            allProperties = new Lazy<string[]>(() => {
                return GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(info => info.Name).ToArray();
            });
            _propertyChangePath = new Dictionary<string, HashSet<string>>();
            if(PropertyDependency == null) return;

            var _propertyDependency = new Dictionary<string, HashSet<string>>();
            foreach(var dpPair in PropertyDependency) {                     //Build a preliminary dependency graph
                var name = ResolveTargetName(dpPair.Key);
                if(name == null) continue;
                if(!_propertyDependency.ContainsKey(name)) {
                    _propertyDependency[name] = new HashSet<string>();
                }
                var dps = _propertyDependency[name];
                foreach(var dp in dpPair.Value) {
                    var dpName = ResolveTargetName(dp);
                    if(dpName == null) continue;
                    dps.Add(dpName);
                }
            }

            foreach(var dpPair in _propertyDependency) {                    //Resolve all depencency of one particular property
                var closed = new HashSet<string>();
                var open = new LinkedList<string>();
                open.AddLast(dpPair.Key);
                while(open.First != null) {
                    var visit = open.First.Value;
                    open.RemoveFirst();
                    if(!closed.Add(visit)) continue;
                    if(!_propertyDependency.ContainsKey(visit)) continue;
                    foreach(var dp in _propertyDependency[visit]) {
                        open.AddLast(dp);
                    }
                }
                foreach(var source in closed) {                             //Flip the dependency list to form the lookup table
                    if(!_propertyChangePath.ContainsKey(source)) {
                        _propertyChangePath[source] = new HashSet<string>();
                        _propertyChangePath[source].Add(source);
                    }
                    _propertyChangePath[source].Add(dpPair.Key);
                }
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;
            if(property == null) {
                RaiseMultiPropertyChanged();
                return;
            }

            if(!_propertyChangePath.ContainsKey(property)) {
                handler(this, new PropertyChangedEventArgs(property));
            } else {
                foreach(var dp in _propertyChangePath[property]) {
                    handler(this, new PropertyChangedEventArgs(dp));
                }
            }
        }

        protected void RaiseMultiPropertyChanged(params string[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                foreach(var prop in allProperties.Value) {
                    handler(this, new PropertyChangedEventArgs(prop));
                }
            } else {
                DoRaiseMultiPropertyChanged(properties, handler);
            }
        }

        protected void RaisePropertyChanged(params Expression<Func<object>>[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                RaiseMultiPropertyChanged();
                return;
            }
            DoRaiseMultiPropertyChanged(properties.Select(ResolveTargetName).Where(x => x != null), handler);
        }

        private void DoRaiseMultiPropertyChanged(IEnumerable<string> properties, PropertyChangedEventHandler handler)
        {
            var affected = new HashSet<string>();
            foreach(var source in properties) {
                if(!_propertyChangePath.ContainsKey(source)) {
                    affected.Add(source);
                } else {
                    foreach(var dp in _propertyChangePath[source]) {
                        affected.Add(dp);
                    }
                }
            }
            foreach(var dp in affected) {
                handler(this, new PropertyChangedEventArgs(dp));
            }
        }

        private string ResolveTargetName(Expression<Func<object>> prop)
        {
            var expr = prop.Body;
            while(expr is UnaryExpression) {
                expr = ((UnaryExpression)expr).Operand;
            }
            if(expr is MemberExpression) {
                var targetMember = ((MemberExpression)expr).Member;
                if(targetMember.MemberType == MemberTypes.Property) {
                    return targetMember.Name;
                }
            }
            return null;
        }

        private string ResolveTargetName(Expression<Func<object, object>> prop)
        {
            var expr = prop.Body;
            while(expr is UnaryExpression) {
                expr = ((UnaryExpression)expr).Operand;
            }
            if(expr is MemberExpression) {
                var targetMember = ((MemberExpression)expr).Member;
                if(targetMember.MemberType == MemberTypes.Property) {
                    return targetMember.Name;
                }
            }
            return null;
        }
    }
}
