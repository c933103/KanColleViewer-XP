using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Utilities.Remoting
{
    public class BootstrapLoader : MarshalByRefObject
    {
        public CustomShipComparerBootstrapResult BootstrapCustomShipComparer(byte[] assemblyBinary)
        {
            var result = new CustomShipComparerBootstrapResult();
            var msgBuilder = new StringBuilder();
            string comparer = null;
            Assembly assembly;
            try {
                assembly = AppDomain.CurrentDomain.Load(assemblyBinary);
                comparer = assembly.GetCustomAttribute<ComparerClassAttribute>()?.ClassName;
            } catch(Exception e) {
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                msgBuilder.Append("行0   列0   错误RT0001 : 无法加载程序集，发生异常 ");
                msgBuilder.AppendLine(e.ToString());
                result.ErrorMessage = msgBuilder.ToString();
                return result;
            }
            if(comparer == null) {
                result.ErrorMessage = "行0   列0   错误CCE0000: 程序集上找不到 ComparerClass 属性";
                return result;
            }

            try {
                result.Comparer = assembly.GetType(comparer).GetConstructor(new Type[0]).Invoke(new object[0]) as CustomComparerBase<Ship>;
                if(result.Comparer == null) {
                    msgBuilder.Append("行0   列0   错误CCE0001: 程序集的 ComparerClass 属性上指定的比较器 ");
                    msgBuilder.Append(comparer);
                    msgBuilder.AppendLine(" 不是 LynLogger.CustomComparerBase<LynLogger.Models.Ship> 的派生类");
                    result.ErrorMessage = msgBuilder.ToString();
                }
                return result;
            } catch(Exception e) {
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                msgBuilder.Append("行0   列0   错误RT0002 : 构建比较器时发生异常 ");
                msgBuilder.AppendLine(e.ToString());
                result.ErrorMessage = msgBuilder.ToString();
                return result;
            }
        }
    }
    
    public class CustomShipComparerBootstrapResult : MarshalByRefObject
    {
        public CustomComparerBase<Ship> Comparer { get; set; }
        public string ErrorMessage { get; set; }
    }
}
