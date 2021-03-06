﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LynLogger.DataStore.Serialization
{
    public abstract partial class AbstractDSSerializable<T>
    {
        private Dictionary<ulong, HandlerInfo> DiscoverFields()
        {
            Dictionary<ulong, HandlerInfo> r = new Dictionary<ulong, HandlerInfo>();

            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(SerializeAttribute))).ToList();

            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.CanRead && x.CanWrite && x.CustomAttributes.Any(a => a.AttributeType == typeof(SerializeAttribute))).ToList();

            foreach(var field in fields) {
                var attr = field.GetCustomAttribute<SerializeAttribute>();
                var constructionType = attr.ConstructionType ?? field.FieldType;
                r[attr.FieldId] = new HandlerInfo(attr.DepthLimit, GenerateSerializer(field, constructionType), GenerateDeserializer(field, constructionType, field.FieldType));
            }

            foreach(var prop in properties) {
                var attr = prop.GetCustomAttribute<SerializeAttribute>();
                var constructionType = attr.ConstructionType ?? prop.PropertyType;
                r[attr.FieldId] = new HandlerInfo(attr.DepthLimit, GenerateSerializer(prop, constructionType), GenerateDeserializer(prop, constructionType, prop.PropertyType));
            }

            return r;
        }
    }
}
