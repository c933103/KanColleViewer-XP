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
        private static Func<T, Premitives.StoragePremitive> GenerateSerializer(MemberInfo field, Type constructionType)
        {
            ParameterExpression paramInput = Expression.Parameter(typeof(T), "input");
            var prop = typeof(ExpressionGenerator<>).MakeGenericType(constructionType).GetProperty("Serializer");
            return Expression.Lambda<Func<T, Premitives.StoragePremitive>>(Expression.Call(Expression.MakeMemberAccess(null, prop), prop.PropertyType.GetMethod("Invoke"), Expression.Convert(Expression.MakeMemberAccess(paramInput, field), constructionType)), paramInput).Compile();
        }

        private static Action<T, Premitives.StoragePremitive, LinkedList<object>> GenerateDeserializer(MemberInfo field, Type constructionType, Type fieldType)
        {
            ParameterExpression paramObj = Expression.Parameter(typeof(T), "object");
            ParameterExpression paramInfo = Expression.Parameter(typeof(Premitives.StoragePremitive), "info");
            ParameterExpression paramPath = Expression.Parameter(typeof(LinkedList<object>), "path");
            var prop = typeof(ExpressionGenerator<>).MakeGenericType(constructionType).GetProperty("Deserializer");

            return Expression.Lambda<Action<T, Premitives.StoragePremitive, LinkedList<object>>>(Expression.Assign(Expression.MakeMemberAccess(paramObj, field), Expression.Convert(Expression.Call(Expression.MakeMemberAccess(null, prop), prop.PropertyType.GetMethod("Invoke"), paramInfo, paramPath), fieldType)), paramObj, paramInfo, paramPath).Compile();
        }
    }

    internal static class ConstantExpressions
    {
        internal static readonly Expression NullConstant = Expression.Constant(null, typeof(object));
        internal static readonly Expression NullSerialization = Expression.Convert(NullConstant, typeof(Premitives.StoragePremitive));
    }

    public static class ExpressionGenerator<T>
    {
        private static Func<T, Premitives.StoragePremitive> _serializer;
        private static Func<Premitives.StoragePremitive, LinkedList<object>, T> _deserializer;
        private static Type _premitive;

        public static Func<T, Premitives.StoragePremitive> Serializer
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
                return _premitive ?? (_premitive = InferPremitiveType(typeof(T)));
            }
        }

        private static Func<T, Premitives.StoragePremitive> GenerateSerializer()
        {
            ParameterExpression input = Expression.Parameter(typeof(T), "input");
            var constructionType = typeof(T);
            var premitiveType = PremitiveType;
            Expression body;
            if(typeof(Premitives.StoragePremitive).IsAssignableFrom(constructionType)) {
                body = input;
            } else if(typeof(IDSSerializable).IsAssignableFrom(constructionType)) {
                body = Expression.Call(Expression.Convert(input, typeof(IDSSerializable)), typeof(IDSSerializable).GetMethod("GetSerializationInfo"));
            } else if(premitiveType == typeof(Premitives.UnsignedInteger)) {
                body = Expression.New(typeof(Premitives.UnsignedInteger).GetConstructor(new Type[] { typeof(ulong) }), Expression.Convert(input, typeof(ulong)));
            } else if(premitiveType == typeof(Premitives.SignedInteger)) {
                body = Expression.New(typeof(Premitives.SignedInteger).GetConstructor(new Type[] { typeof(long) }), Expression.Convert(input, typeof(long)));
            } else if(premitiveType == typeof(Premitives.Decimal)) {
                body = Expression.New(typeof(Premitives.Decimal).GetConstructor(new Type[] { typeof(decimal) }), Expression.Convert(input, typeof(decimal)));
            } else if(premitiveType == typeof(Premitives.Double)) {
                body = Expression.New(typeof(Premitives.Double).GetConstructor(new Type[] { typeof(double) }), Expression.Convert(input, typeof(double)));
            } else if(premitiveType == typeof(Premitives.String)) {
                body = Expression.New(typeof(Premitives.String).GetConstructor(new Type[] { typeof(string) }), Expression.Convert(input, typeof(string)));
            } else if(premitiveType == typeof(Premitives.Blob)) {
                body = Expression.New(typeof(Premitives.Blob).GetConstructor(new Type[] { typeof(byte[]) }), Expression.Convert(input, typeof(byte[])));
            } else {
                //TODO Dictionary and List.
                throw new NotImplementedException();
            }
            return Expression.Lambda<Func<T, Premitives.StoragePremitive>>(Expression.Condition(Expression.Equal(Expression.Convert(input, typeof(object)), ConstantExpressions.NullConstant), ConstantExpressions.NullSerialization, Expression.Convert(body, typeof(Premitives.StoragePremitive))), input).Compile();
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
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.UnsignedInteger)), "Value");
            } else if(premitiveType == typeof(Premitives.SignedInteger)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.SignedInteger)), "Value");
            } else if(premitiveType == typeof(Premitives.Decimal)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.Decimal)), "Value");
            } else if(premitiveType == typeof(Premitives.Double)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.Double)), "Value");
            } else if(premitiveType == typeof(Premitives.String)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.String)), "Value");
            } else if(premitiveType == typeof(Premitives.Blob)) {
                body = Expression.PropertyOrField(Expression.Convert(paramInfo, typeof(Premitives.Blob)), "Data");
            } else {
                //TODO Dictionary and List.
                throw new NotImplementedException();
            }

            return Expression.Lambda<Func<Premitives.StoragePremitive, LinkedList<object>, T>>(Expression.Convert(Expression.Condition(Expression.Equal(Expression.Convert(paramInfo, typeof(object)), ConstantExpressions.NullConstant), Expression.Default(constructionType), Expression.Convert(body, constructionType)), typeof(T)), paramInfo, paramPath).Compile();
        }

        private static Type InferPremitiveType(Type fieldType)
        {
            if(fieldType.IsEnum) fieldType = fieldType.GetEnumUnderlyingType();

            if(fieldType == typeof(string)) return typeof(Premitives.String);
            if(fieldType == typeof(byte[])) return typeof(Premitives.Blob);
            if(fieldType == typeof(decimal)) return typeof(Premitives.Decimal);

            if(fieldType == typeof(double) || fieldType == typeof(float)) return typeof(Premitives.Double);

            if(fieldType == typeof(sbyte) ||fieldType == typeof(short) ||fieldType == typeof(int) ||fieldType == typeof(long)) return typeof(Premitives.SignedInteger);
            if(fieldType == typeof(byte) ||fieldType == typeof(ushort) ||fieldType == typeof(uint) ||fieldType == typeof(ulong)) return typeof(Premitives.UnsignedInteger);

            if(typeof(IDSSerializable).IsAssignableFrom(fieldType)) return typeof(Premitives.StoragePremitive);

            //TODO Dictionary and List
            throw new NotImplementedException();
        }
    }
}
