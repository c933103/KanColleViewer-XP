using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.Serialization
{
    public abstract partial class AbstractDSSerializable<T>
    {
        private static Func<T, LinkedList<object>, Premitives.StoragePremitive> GenerateSerializer(MemberInfo field, Type constructionType)
        {
            ParameterExpression paramInput = Expression.Parameter(typeof(T), "input");
            ParameterExpression paramPath = Expression.Parameter(typeof(LinkedList<object>), "path");
            var prop = typeof(ExpressionGenerator<>).MakeGenericType(constructionType).GetProperty(nameof(ExpressionGenerator<T>.Serializer));

            Expression body = Expression.Call(Expression.MakeMemberAccess(null, prop), prop.PropertyType.GetMethod("Invoke"), Expression.Convert(Expression.MakeMemberAccess(paramInput, field), constructionType), paramPath);
            return Expression.Lambda<Func<T, LinkedList<object>, Premitives.StoragePremitive>>(body, paramInput, paramPath).Compile();
        }

        private static Action<T, Premitives.StoragePremitive, LinkedList<object>> GenerateDeserializer(MemberInfo field, Type constructionType, Type fieldType)
        {
            ParameterExpression paramObj = Expression.Parameter(typeof(T), "object");
            ParameterExpression paramInfo = Expression.Parameter(typeof(Premitives.StoragePremitive), "info");
            ParameterExpression paramPath = Expression.Parameter(typeof(LinkedList<object>), "path");
            var prop = typeof(ExpressionGenerator<>).MakeGenericType(constructionType).GetProperty(nameof(ExpressionGenerator<T>.Deserializer));

            return Expression.Lambda<Action<T, Premitives.StoragePremitive, LinkedList<object>>>(Expression.Assign(Expression.MakeMemberAccess(paramObj, field), Expression.Convert(Expression.Call(Expression.MakeMemberAccess(null, prop), prop.PropertyType.GetMethod("Invoke"), paramInfo, paramPath), fieldType)), paramObj, paramInfo, paramPath).Compile();
        }
    }

    internal static class ConstantExpressions
    {
        internal static readonly Expression NullConstant = Expression.Constant(null, typeof(object));
        internal static readonly Expression NullSerialization = Expression.Constant(null, typeof(Premitives.StoragePremitive));
        internal static readonly Expression UlongZero = Expression.Constant(0uL, typeof(ulong));
        internal static readonly Expression UlongOne = Expression.Constant(1uL, typeof(ulong));
    }

    public static class ExpressionGenerator<T>
    {
        private static Func<T, LinkedList<object>, Premitives.StoragePremitive> _serializer;
        private static Func<Premitives.StoragePremitive, LinkedList<object>, T> _deserializer;
        private static Type _premitive;

        public static Func<T, LinkedList<object>, Premitives.StoragePremitive> Serializer
        {
            get
            {
                return _serializer ?? (_serializer = GenerateSerializer());
            }
        }

        public static Func<Premitives.StoragePremitive, LinkedList<object>, T> Deserializer
        {
            get
            {
                return _deserializer ?? (_deserializer = GenerateDeserializer());
            }
        }

        public static Type PremitiveType
        {
            get
            {
                return _premitive ?? (_premitive = InferPremitiveType());
            }
        }

        private static Func<T, LinkedList<object>, Premitives.StoragePremitive> GenerateSerializer()
        {
            ParameterExpression input = Expression.Parameter(typeof(T), "input");
            ParameterExpression path = Expression.Parameter(typeof(LinkedList<object>), "path");
            var constructionType = typeof(T);
            var premitiveType = PremitiveType;
            Expression body;
            if(typeof(Premitives.StoragePremitive).IsAssignableFrom(constructionType)) {
                body = input;
            } else if(typeof(IDSSerializable).IsAssignableFrom(constructionType)) {
                body = Expression.Call(Expression.Convert(input, typeof(IDSSerializable)), typeof(IDSSerializable).GetMethod(nameof(IDSSerializable.GetSerializationInfo)), path);
            } else if(premitiveType == typeof(Premitives.UnsignedInteger)) {
                Expression conversion;
                if(constructionType == typeof(bool)) {
                    conversion = Expression.Condition(input, ConstantExpressions.UlongOne, ConstantExpressions.UlongZero);
                } else {
                    conversion = Expression.Convert(input, typeof(ulong));
                }
                body = Expression.New(typeof(Premitives.UnsignedInteger).GetConstructor(new Type[] { typeof(ulong) }), conversion);
            } else if(premitiveType == typeof(Premitives.SignedInteger)) {
                body = Expression.New(typeof(Premitives.SignedInteger).GetConstructor(new Type[] { typeof(long) }), Expression.Convert(input, typeof(long)));
            } else if(premitiveType == typeof(Premitives.DsDecimal)) {
                body = Expression.New(typeof(Premitives.DsDecimal).GetConstructor(new Type[] { typeof(decimal) }), Expression.Convert(input, typeof(decimal)));
            } else if(premitiveType == typeof(Premitives.DsDouble)) {
                body = Expression.New(typeof(Premitives.DsDouble).GetConstructor(new Type[] { typeof(double) }), Expression.Convert(input, typeof(double)));
            } else if(premitiveType == typeof(Premitives.DsString)) {
                body = Expression.New(typeof(Premitives.DsString).GetConstructor(new Type[] { typeof(string) }), Expression.Convert(input, typeof(string)));
            } else if(premitiveType == typeof(Premitives.Blob)) {
                body = Expression.New(typeof(Premitives.Blob).GetConstructor(new Type[] { typeof(byte[]) }), Expression.Convert(input, typeof(byte[])));
            } else {
                //TODO Dictionary and List.
                throw new NotImplementedException();
            }
            return Expression.Lambda<Func<T, LinkedList<object>, Premitives.StoragePremitive>>(Expression.Condition(Expression.Equal(Expression.Convert(input, typeof(object)), ConstantExpressions.NullConstant), ConstantExpressions.NullSerialization, Expression.Convert(body, typeof(Premitives.StoragePremitive))), input, path).Compile();
        }

        private static Func<Premitives.StoragePremitive, LinkedList<object>, T> GenerateDeserializer()
        {
            ParameterExpression paramInfo = Expression.Parameter(typeof(Premitives.StoragePremitive), "info");
            ParameterExpression paramPath = Expression.Parameter(typeof(LinkedList<object>), "path");
            var constructionType = typeof(T);
            var premitiveType = PremitiveType;
            Expression body;

            if(typeof(IDSSerializable).IsAssignableFrom(constructionType)) {
                body = Expression.New(constructionType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(Premitives.StoragePremitive), typeof(LinkedList<object>) }, null), paramInfo, paramPath);
            } else if(premitiveType == typeof(Premitives.UnsignedInteger)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.UnsignedInteger)), nameof(Premitives.UnsignedInteger.Value));
            } else if(premitiveType == typeof(Premitives.SignedInteger)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.SignedInteger)), nameof(Premitives.SignedInteger.Value));
            } else if(premitiveType == typeof(Premitives.DsDecimal)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.DsDecimal)), nameof(Premitives.DsDecimal.Value));
            } else if(premitiveType == typeof(Premitives.DsDouble)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.DsDouble)), nameof(Premitives.DsDouble.Value));
            } else if(premitiveType == typeof(Premitives.DsString)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.DsString)), nameof(Premitives.DsString.Value));
            } else if(premitiveType == typeof(Premitives.Blob)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.Blob)), nameof(Premitives.Blob.Data));
            } else {
                //TODO Dictionary and List.
                throw new NotImplementedException();
            }

            Expression conversion;
            if (constructionType == typeof(bool)) {
                conversion = Expression.NotEqual(body, ConstantExpressions.UlongZero);
            } else {
                conversion = Expression.Convert(body, constructionType);
            }
            return Expression.Lambda<Func<Premitives.StoragePremitive, LinkedList<object>, T>>(Expression.Convert(Expression.Condition(Expression.Equal(Expression.Convert(paramInfo, typeof(object)), ConstantExpressions.NullConstant), Expression.Default(constructionType), conversion), typeof(T)), paramInfo, paramPath).Compile();
        }

        private static Type InferPremitiveType()
        {
            var fieldType = typeof(T);
            if(fieldType.IsEnum) fieldType = fieldType.GetEnumUnderlyingType();

            if(fieldType == typeof(string)) return typeof(Premitives.DsString);
            if(fieldType == typeof(byte[])) return typeof(Premitives.Blob);
            if(fieldType == typeof(decimal)) return typeof(Premitives.DsDecimal);
            if(fieldType == typeof(decimal?)) return typeof(Premitives.DsDecimal);

            if(fieldType == typeof(double) || fieldType == typeof(float)) return typeof(Premitives.DsDouble);
            if(fieldType == typeof(double?) || fieldType == typeof(float?)) return typeof(Premitives.DsDouble);

            if(fieldType == typeof(sbyte) || fieldType == typeof(short) || fieldType == typeof(int) || fieldType == typeof(long)) return typeof(Premitives.SignedInteger);
            if(fieldType == typeof(sbyte?) || fieldType == typeof(short?) || fieldType == typeof(int?) || fieldType == typeof(long?)) return typeof(Premitives.SignedInteger);

            if(fieldType == typeof(byte) || fieldType == typeof(ushort) || fieldType == typeof(uint) || fieldType == typeof(ulong) || fieldType == typeof(bool)) return typeof(Premitives.UnsignedInteger);
            if(fieldType == typeof(byte?) || fieldType == typeof(ushort?) || fieldType == typeof(uint?) || fieldType == typeof(ulong?) || fieldType == typeof(bool?)) return typeof(Premitives.UnsignedInteger);

            if(typeof(IDSSerializable).IsAssignableFrom(fieldType)) return typeof(Premitives.StoragePremitive);

            //TODO Dictionary and List
            throw new NotImplementedException();
        }
    }
}
