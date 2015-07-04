using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace LynLogger.Extensions.RawDataWrapper
{
    static class RawDataWrapperExtension
    {
        private static readonly ConcurrentDictionary<Type, Delegate> accessor = new ConcurrentDictionary<Type, Delegate>();

        public static T GetRawData<T>(this RawDataWrapper<T> rdw)
        {
            try {
                Delegate compiledFunc;
                if (accessor.TryGetValue(typeof(T), out compiledFunc)) {
                    return (T)compiledFunc.DynamicInvoke(rdw);
                }

                var paramRdw = Expression.Parameter(typeof(RawDataWrapper<T>));
                var func = Expression.Lambda<Func<RawDataWrapper<T>, T>>(
                                Expression.Property(paramRdw, typeof(RawDataWrapper<T>).GetProperty("RawData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)), 
                            paramRdw).Compile();

                accessor[typeof(T)] = func;
                return func(rdw);
            } catch(Exception e) {
                File.AppendAllText("lynlogger-error.log", e.ToString());
                throw;
            }
        }
    }
}
