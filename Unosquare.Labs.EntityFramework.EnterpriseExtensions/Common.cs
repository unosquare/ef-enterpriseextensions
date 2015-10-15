using System;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    /// <summary>
    /// Common types and constants
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Dynamic Proxies namespace
        /// </summary>
        public static string DynamicProxiesNamespace = "System.Data.Entity.DynamicProxies";

        /// <summary>
        /// Primitive Types
        /// </summary>
        public static Type[] PrimitiveTypes =
        {
            typeof (string),
            typeof (DateTime),
            typeof (bool),
            typeof (byte),
            typeof (sbyte),
            typeof (char),
            typeof (decimal),
            typeof (double),
            typeof (float),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
            typeof (short),
            typeof (ushort),
            typeof (DateTime?),
            typeof (bool?),
            typeof (byte?),
            typeof (sbyte?),
            typeof (char?),
            typeof (decimal?),
            typeof (double?),
            typeof (float?),
            typeof (int?),
            typeof (uint?),
            typeof (long?),
            typeof (ulong?),
            typeof (short?),
            typeof (ushort?),
            typeof (Guid),
            typeof (Guid?)
        };
    }
}
