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
            Delegate compiledFunc;
            if (!accessor.TryGetValue(typeof(T), out compiledFunc) || !(compiledFunc is Func<RawDataWrapper<T>, T>)) {
                compiledFunc = Delegate.CreateDelegate(typeof(Func<RawDataWrapper<T>, T>), null, typeof(RawDataWrapper<T>).GetProperty("RawData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetMethod);
                accessor.TryAdd(typeof(T), compiledFunc);
            }

            return ((Func<RawDataWrapper<T>, T>)compiledFunc)(rdw);
        }
    }
}
