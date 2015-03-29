using LynLogger.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    public partial class ShipStatusModel
    {
        private string _src = Properties.Resources.TemplateCustomShipSorter;
        private DateTimeOffset _srcTime = DateTimeOffset.Now;
        public string CustomComparerSource
        {
            get { return _src; }
            set
            {
                _src = value;
                _srcTime = DateTimeOffset.UtcNow;
                RaisePropertyChanged();
            }
        }

        private DateTimeOffset _compileTime;
        private AppDomain _sortDomain;
        private CustomComparerBase<Ship> _customComparer;
        private string _errorMsg;

        public string ErrorMessage
        {
            get { return _errorMsg; }
            set
            {
                _errorMsg = value;
                RaisePropertyChanged();
            }
        }

        public CustomComparerBase<Ship> CustomComparer
        {
            get
            {
                if(_compileTime >= _srcTime) return _customComparer;
                if(_sortDomain != null) {
                    try {
                        AppDomain.Unload(_sortDomain);
                    } catch (AppDomainUnloadedException) { }
                }
                _sortDomain = null;
                _customComparer = null;
                
                _compileTime = DateTimeOffset.UtcNow;
                CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
                if(compiler == null) {
                    ErrorMessage = "行0   列0   错误RT0000 : 当前的.NET运行环境不支持代码编译";
                    return null;
                }

                CompilerParameters parameters = new CompilerParameters() {
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                    IncludeDebugInformation = true
                };
                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    try {
                        string location = assembly.Location;
                        if(!string.IsNullOrEmpty(location)) {
                            parameters.ReferencedAssemblies.Add(location);
                        }
                    } catch (NotSupportedException) {
                        // this happens for dynamic assemblies, so just ignore it. 
                    }
                }

                var compileResult = compiler.CompileAssemblyFromSource(parameters, CustomComparerSource);
                StringBuilder msgBuilder = new StringBuilder();
                foreach(CompilerError msg in compileResult.Errors) {
                    msgBuilder.AppendLine(string.Format("行{2,-3} 列{3,-3} {0}{1,-7}: {4}", msg.IsWarning ? "警告" : "错误", msg.ErrorNumber, msg.Line, msg.Column, msg.ErrorText));
                }
                if(compileResult.Errors.HasErrors) {
                    ErrorMessage = msgBuilder.ToString();
                    return null;
                }

                byte[] bin = System.IO.File.ReadAllBytes(compileResult.PathToAssembly);
                try {
                    System.IO.File.Delete(compileResult.PathToAssembly);
                } catch { }

                var permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                var ourName = Assembly.GetExecutingAssembly().GetName().Name;
                var ourAssemStrongName = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.Evidence.GetHostEvidence<StrongName>()).Where(x => x != null && x.Name != ourName).ToArray();
                var domSetup = new AppDomainSetup() {
                    ApplicationBase = System.IO.Path.GetDirectoryName(typeof(Grabacr07.KanColleWrapper.KanColleClient).Assembly.Location),
                    PrivateBinPath = System.IO.Path.GetDirectoryName(typeof(Utilities.Remoting.BootstrapLoader).Assembly.Location)
                };
                _sortDomain = AppDomain.CreateDomain("Sorting Domain", null, domSetup, permSet, ourAssemStrongName);

                //var assemToFileMapping = AppDomain.CurrentDomain.GetAssemblies().Select(x => { try { return new { Location = x.Location, Name = x.FullName }; } catch { return null; } }).Where(x => x != null).ToDictionary(x => x.Name, x => x.Location);
                //_sortDomain.AssemblyResolve += new XAppDomainAssemblyResolver(assemToFileMapping)._sortDomain_AssemblyResolve;

                var hBsl = Activator.CreateInstanceFrom(_sortDomain, typeof(Utilities.Remoting.BootstrapLoader).Assembly.ManifestModule.FullyQualifiedName, typeof(Utilities.Remoting.BootstrapLoader).FullName);
                var bsl = (Utilities.Remoting.BootstrapLoader)hBsl.Unwrap();
                var bsResult = bsl.BootstrapCustomShipComparer(bin);
                _customComparer = bsResult.Comparer;
                msgBuilder.Append(bsResult.ErrorMessage);
                ErrorMessage = msgBuilder.ToString();

                return _customComparer;
            }
        }

        /*public class XAppDomainAssemblyResolver : MarshalByRefObject
        {
            public Dictionary<string, string> mapping;
            public XAppDomainAssemblyResolver(Dictionary<string, string> d) { mapping = d; }

            public Assembly _sortDomain_AssemblyResolve(object s, ResolveEventArgs e)
            {
                if(mapping.ContainsKey(e.Name)) {
                    return Assembly.LoadFile(mapping[e.Name]);
                }
                return null;
            }
        }*/

        private class CustomComparerWrapper : ComparerBase<Ship>
        {
            private ShipStatusModel parent;
            public CustomComparerWrapper(ShipStatusModel parent) : base("自定义", null) { this.parent = parent; }

            public override int Compare(Ship x, Ship y)
            {
                return DoCompare(x, y);
            }

            private int DoCompare(Ship x, Ship y, bool reEntrant = false)
            {
                try {
                    return parent.CustomComparer == null ? 0 : parent.CustomComparer.Compare(x, y);
                } catch (System.Runtime.Remoting.RemotingException e) {
                    if(!reEntrant) {
                        parent._compileTime = DateTimeOffset.MinValue;
                        return DoCompare(x, y, true);
                    } else {
                        ReportException(e);
                        return 0;
                    }
                } catch (Exception e) {
                    ReportException(e);
                    return 0;
                }
            }

            private void ReportException(Exception e)
            {
                var stack = new System.Diagnostics.StackTrace(e).GetFrame(0);
                var line = stack.GetFileLineNumber();
                var col = stack.GetFileColumnNumber();
                var file = stack.GetFileName();
                parent.ErrorMessage = string.Format("行{0,-3} 列{1,-3} 错误CCE0002: 比较器发生异常，源文件是 {2}，{3}", line, col, file, e.ToString());
            }
        }
    }
}
