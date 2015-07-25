using System;
using System.Collections.Concurrent;
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
    public abstract class NotificationSourceObject<T> : INotifyPropertyChanged
        where T : NotificationSourceObject<T>
    {
        private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> _propertyChangePath = null;
        private static Lazy<string[]> _allProperties;

        //protected virtual IReadOnlyDictionary<Expression<Func<T, object>>, IReadOnlyCollection<Expression<Func<T, object>>>> PropertyDependencyExpr => null;
        protected virtual IReadOnlyDictionary<string, IReadOnlyCollection<string>> PropertyDependency => null;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected NotificationSourceObject() { Deserialized(new StreamingContext()); }

        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            if (_allProperties == null) {
                System.Threading.Interlocked.CompareExchange(ref _allProperties, new Lazy<string[]>(() => {
                    return GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(info => info.Name).ToArray();
                }), null);
            }

            if (_propertyChangePath != null) return;
            var pdNames = PropertyDependency;
            if(pdNames == null) {
                /*if (PropertyDependencyExpr == null)*/ return;

                /*var rwNames = new Dictionary<string, IReadOnlyCollection<string>>();
                foreach(var kv in PropertyDependencyExpr) {
                    rwNames[ResolveTargetName(kv.Key)] = kv.Value.Select(x => ResolveTargetName(x)).ToList();
                }
                pdNames = rwNames;*/
            }
            
            var _cp = new Dictionary<string, HashSet<string>>();
            var closed = new HashSet<string>();
            var open = new LinkedList<string>();
            foreach (var dpPair in PropertyDependency) {                    //Resolve all depencency of one particular property
                if (dpPair.Key == null) continue;

                closed.Clear(); open.Clear();
                open.AddLast(dpPair.Key);
                while(open.First != null) {
                    var visit = open.First.Value;
                    open.RemoveFirst();
                    if(!closed.Add(visit)) continue;
                    if(!PropertyDependency.ContainsKey(visit)) continue;
                    foreach(var dp in PropertyDependency[visit]) {
                        if(dp != null) open.AddLast(dp);
                    }
                }
                foreach(var source in closed) {                             //Flip the dependency list to form the lookup table
                    if(!_cp.ContainsKey(source)) {
                        _cp[source] = new HashSet<string>();
                        _cp[source].Add(source);
                    }
                    _cp[source].Add(dpPair.Key);
                }
            }
            
            System.Threading.Interlocked.CompareExchange(ref _propertyChangePath, _cp.ToDictionary(x => x.Key, x => x.Value.ToArray() as IReadOnlyCollection<string>), null);
        }

        protected void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;
            if(property == null) {
                RaiseMultiPropertyChanged();
                return;
            }

            if(_propertyChangePath?.ContainsKey(property) == true) {
                foreach(var dp in _propertyChangePath[property]) {
                    handler(this, new PropertyChangedEventArgs(dp));
                }
            } else {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        protected void RaiseMultiPropertyChanged(params string[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                foreach(var prop in _allProperties.Value) {
                    handler(this, new PropertyChangedEventArgs(prop));
                }
            } else {
                DoRaiseMultiPropertyChanged(properties, handler);
            }
        }

        /*protected void RaiseMultiPrropertyChangedExpr(params Expression<Func<T, object>>[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler == null) return;

            if(properties.Length == 0) {
                foreach(var prop in _allProperties.Value) {
                    handler(this, new PropertyChangedEventArgs(prop));
                }
                return;
            }
            DoRaiseMultiPropertyChanged(properties.Select(ResolveTargetName).Where(x => x != null), handler);
        }*/

        private void DoRaiseMultiPropertyChanged(IEnumerable<string> properties, PropertyChangedEventHandler handler)
        {
            var affected = new HashSet<string>();
            foreach(var source in properties) {
                if(_propertyChangePath?.ContainsKey(source) == true) {
                    foreach(var dp in _propertyChangePath[source]) {
                        affected.Add(dp);
                    }
                } else {
                    affected.Add(source);
                }
            }
            foreach(var dp in affected) {
                handler(this, new PropertyChangedEventArgs(dp));
            }
        }

        /*private string ResolveTargetName(Expression<Func<T, object>> prop)
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
        }*/
    }
}
