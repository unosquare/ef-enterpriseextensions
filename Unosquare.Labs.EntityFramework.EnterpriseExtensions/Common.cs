using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions
{
    public static class Common
    {
        public static string DynamicProxiesNamespace = "System.Data.Entity.DynamicProxies";

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
