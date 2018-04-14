using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using Interpose.Core.Proxies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Interpose.Core.Generators
{
    public sealed class RoslynInterceptedTypeGenerator : InterceptedTypeGenerator
    {
        internal static readonly InterceptedTypeGenerator Instance = new RoslynInterceptedTypeGenerator();

        private static readonly Type interfaceProxyType = typeof(InterfaceProxy);

        private string GetVisibility(MethodAttributes attributes)
        {
            var visibility = string.Empty;

            if ((attributes & MethodAttributes.Public) == MethodAttributes.Public)
            {
                visibility = "public";
            }
            else if ((attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
            {
                visibility = "protected internal";
            }
            else if ((attributes & MethodAttributes.Family) == MethodAttributes.Family)
            {
                visibility = "protected";
            }
            else if ((attributes & MethodAttributes.FamANDAssem) == MethodAttributes.FamANDAssem)
            {
                visibility = "internal";
            }

            return visibility;
        }

        private string GetReturnType(Type type)
        {
            if (type == typeof(void))
            {
                return "void";
            }
            else
            {
                return type.FullName;
            }
        }

        private string GetMoreRelaxedVisibility(MethodAttributes? attributes1, MethodAttributes? attributes2)
        {
            if (attributes1 == null)
            {
                return this.GetVisibility(attributes2.Value);
            }
            else if (attributes2 == null)
            {
                return this.GetVisibility(attributes1.Value);
            }

            if (attributes1 > attributes2)
            {
                return this.GetVisibility(attributes1.Value);
            }
            else if (attributes2 > attributes1)
            {
                return this.GetVisibility(attributes2.Value);
            }
            else
            {
                return this.GetVisibility(attributes1.Value);
            }
        }

        private void BuildConstructors(Type baseType, string typeName, StringBuilder builder, bool isInterface)
        {
            var constructors = this.GetConstructors(baseType);

            //constructors
            foreach (var ctor in constructors)
            {
                var visibility = this.GetVisibility(ctor.Attributes);

                builder.AppendFormat("{0} {1}({2}) : base({3}) {{ }}\r\n", visibility, typeName, string.Join(", ", ctor.GetParameters().Select(x => x.ParameterType.FullName + " " + x.Name)), string.Join(", ", ctor.GetParameters().Select(x => x.Name)));
            }

            if (isInterface == true)
            {
                builder.AppendFormat("public {0}({1} interceptor, {2} handler, object target): base(interceptor, handler, target) {{ }}\r\n", typeName, typeof(IInterceptor).FullName, typeof(IInterceptionHandler).FullName);
            }

        }

        public override Type Generate(IInterceptor interceptor, Type baseType, Type handlerType, params Type[] additionalInterfaceTypes)
        {
            if (interceptor == null)
            {
                throw new ArgumentNullException(nameof(interceptor));
            }

            if ((handlerType != null) && (typeof(IInterceptionHandler).IsAssignableFrom(handlerType) == false))
            {
                throw new ArgumentException($"Handler type is not an {typeof(IInterceptionHandler)}", nameof(handlerType));
            }

            if ((handlerType != null) && (handlerType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, new ParameterModifier[0]) == null))
            {
                throw new ArgumentException($"Handler type {handlerType} does not have a public parameterless constructor");
            }

            if ((baseType == typeof(InterfaceProxy)) || (baseType == null))
            {
                if (additionalInterfaceTypes.Any(x => x.IsInterface == false))
                {
                    throw new ArgumentNullException("The target interface is missing", nameof(additionalInterfaceTypes));
                }
            }

            if (baseType == null)
            {
                baseType = typeof(object);
            }

            var isInterface = (baseType == interfaceProxyType);
            var types = new[] { baseType, handlerType }.Concat(additionalInterfaceTypes).Distinct();
            var typeName = ((isInterface) ? additionalInterfaceTypes.First().Name : baseType.Name) + "_Dynamic";

            var builder = new StringBuilder();

            foreach (var @namespace in types.Where(x => x != null).Select(x => x.Namespace).Distinct().OrderBy(x => x))
            {
                builder.AppendFormat("using {0};\r\n", @namespace);
            }

            //default namespaces
            builder.AppendFormat("using System;\r\n");
            builder.AppendFormat("using System.Linq;\r\n");
            builder.AppendFormat("using System.Reflection;\r\n");

            builder.AppendFormat("namespace {0} {{\r\n", baseType.Namespace);

            this.BuildClass(builder, typeName, baseType, additionalInterfaceTypes, handlerType);

            this.BuildConstructors(baseType, typeName, builder, isInterface);

            this.BuildMethods(baseType, additionalInterfaceTypes, builder, isInterface);

            this.BuildEvents(baseType, additionalInterfaceTypes, builder, isInterface);

            this.BuildProperties(baseType, additionalInterfaceTypes, builder, isInterface);

            builder.Append("}\r\n");        //class

            builder.Append("}\r\n");        //namespace

            var syntaxTree = CSharpSyntaxTree.ParseText(builder.ToString());
            var assemblyName = Path.GetRandomFileName();

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(this.GetType().Assembly.Location),
                MetadataReference.CreateFromFile(baseType.Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Queryable).Assembly.Location),    //System.Linq.Queryable
                MetadataReference.CreateFromFile(typeof(IQueryable<>).Assembly.Location), //System.Linq.Expressions
                MetadataReference.CreateFromFile(typeof(Lazy<,>).Assembly.Location),      //System.Runtime
                MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),       //System.Private.CoreLib
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),      //System.Console
                MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.ObjectModel").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)
            };

            if (handlerType != null)
            {
                references.Add(MetadataReference.CreateFromFile(handlerType.Assembly.Location));
            }

            references.AddRange((additionalInterfaceTypes ?? new Type[0]).Select(type => MetadataReference.CreateFromFile(type.GetTypeInfo().Assembly.Location)));

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success == true)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetExportedTypes().First();

                    return type;
                }
                else
                {
                    var failures = result.Diagnostics.Where(diagnostic => (diagnostic.IsWarningAsError == true) || (diagnostic.Severity == DiagnosticSeverity.Error));
                    var errorMessage = string.Join(Environment.NewLine, failures.Select(x => x.GetMessage()));
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private void BuildClass(StringBuilder builder, string typeName, Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            var isInterface = (baseType == interfaceProxyType);

            builder.AppendFormat("public class {0} : {1} ", typeName, baseType.Name);

            if (isInterface == true)
            {
                builder.AppendFormat(", {0} ", typeof(IInterceptionProxy).FullName);
            }

            foreach (var additionalInterfaceType in additionalInterfaceTypes)
            {
                builder.AppendFormat(", {0} ", additionalInterfaceType.FullName);
            }

            builder.Append("{\r\n");

            if (isInterface == true)
            {
                builder.AppendFormat("{0} {1}.{2} {{ get {{ return this.interceptor; }} }}\r\n", typeof(IInterceptor).FullName, typeof(IInterceptionProxy).FullName, nameof(IInterceptionProxy.Interceptor));
                builder.AppendFormat("object {0}.{1} {{ get {{ return this.target; }} }}\r\n", typeof(IInterceptionProxy).FullName, nameof(IInterceptionProxy.Target));
            }

            if (isInterface == false)
            {
                if (handlerType != null)
                {
                    builder.AppendFormat("private {0} handler = new {1}();\r\n", typeof(IInterceptionHandler).FullName, handlerType.FullName);
                }
            }
        }

        private void BuildProperties(Type baseType, Type[] additionalInterfaceTypes, StringBuilder builder, bool isInterface)
        {
            var properties = this.GetProperties(baseType, additionalInterfaceTypes);

            //properties
            foreach (var prop in properties.Where(x => ((x.GetMethod != null) && (x.GetMethod.IsVirtual == true)) || ((x.SetMethod != null) && (x.SetMethod.IsVirtual == true))))
            {
                var hasSetter = (prop.SetMethod != null);
                var hasGetter = (prop.GetMethod != null);

                var returnType = this.GetReturnType(prop.PropertyType);
                var visibility = this.GetMoreRelaxedVisibility(prop.SetMethod?.Attributes, prop.GetMethod?.Attributes);

                builder.AppendFormat("{0} {1} {2} {3} {{\r\n", visibility, (isInterface == true ? string.Empty : "override"), prop.PropertyType.FullName, prop.Name);

                foreach (var method in new[] { prop.SetMethod, prop.GetMethod })
                {
                    if (method == null)
                    {
                        continue;
                    }

                    var isSet = method.Name.StartsWith("set_");
                    var setterVisibility = this.GetVisibility(method.Attributes);

                    if (setterVisibility != visibility)
                    {
                        builder.AppendFormat("{0} ", setterVisibility);
                    }

                    if (isSet == true)
                    {
                        builder.Append("set {\r\n");
                    }
                    else
                    {
                        builder.Append("get {\r\n");
                    }

                    if (isInterface == false)
                    {
                        //virtual method
                        builder.Append("var self = this;\r\n");
                    }
                    else
                    {
                        //interface
                        builder.AppendFormat("var self = ({0}) this.target;\r\n", method.DeclaringType.FullName);
                    }

                    builder.AppendFormat("{0} args = null;\r\n", typeof(InterceptionArgs).FullName);

                    if (isSet == true)
                    {
                        if (isInterface == true)
                        {
                            builder.AppendFormat("args = new {0}(self, this.target.GetType().GetMethod(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType).ToArray()), () => {{ return self.{1} = value; }}, new object [] {{ value }});\r\n", typeof(InterceptionArgs).FullName, prop.Name);
                        }
                        else
                        {
                            builder.AppendFormat("args = new {0}(self, (MethodInfo) MethodBase.GetCurrentMethod(), () => {{ return base.{1} = value; }}, new object [] {{ value }});\r\n", typeof(InterceptionArgs).FullName, prop.Name);
                        }
                    }
                    else
                    {
                        if (isInterface == true)
                        {
                            builder.AppendFormat("args = new {0}(self, this.target.GetType().GetMethod(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType).ToArray()), () => {{ return self.{1}; }}, new object [] {{ }});\r\n", typeof(InterceptionArgs).FullName, prop.Name);
                        }
                        else
                        {
                            builder.AppendFormat("args = new {0}(self, (MethodInfo) MethodBase.GetCurrentMethod(), () => {{ return base.{1}; }} , new object [] {{ }});\r\n", typeof(InterceptionArgs).FullName, prop.Name);
                        }
                    }

                    builder.Append("this.handler.Invoke(args);\r\n");

                    if (isSet == false)
                    {
                        builder.AppendFormat("if (args.Handled == true) {{ return ({0}) args.Result; }}\r\n", prop.PropertyType.FullName);
                        builder.AppendFormat("return {0}.{1};\r\n", (isInterface == true ? "self" : "base"), prop.Name);
                    }
                    else
                    {
                        builder.Append("if (args.Handled == true) {{ return; }}\r\n");
                        builder.AppendFormat("{0}.{1} = value;\r\n", (isInterface == true ? "self" : "base"), prop.Name);
                    }

                    builder.Append("}\r\n");
                }

                builder.Append("}\r\n");   //property
            }
        }

        private void BuildEvents(Type baseType, Type[] additionalInterfaceTypes, StringBuilder builder, bool isInterface)
        {
            var events = this.GetEvents(baseType, additionalInterfaceTypes);

            foreach (var evt in events)
            {
                var visibility = GetMoreRelaxedVisibility(evt.AddMethod.Attributes, null);

                if (isInterface == true)
                {
                    builder.AppendFormat("{0} event {1} {2} {{ add {{ (this.target as {3}).{2} += value; }} remove {{ (this.target as {3}).{2} -= value; }} }}", visibility, evt.EventHandlerType.FullName, evt.Name, evt.DeclaringType);
                }
            }
        }

        private void BuildMethods(Type baseType, Type[] additionalInterfaceTypes, StringBuilder builder, bool isInterface)
        {
            var methods = this.GetMethods(baseType, additionalInterfaceTypes);

            //methods
            foreach (var method in methods.Where(x => x.IsVirtual == true))
            {
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                {
                    //skip properties
                    continue;
                }

                var visibility = this.GetVisibility(method.Attributes);
                var returnType = this.GetReturnType(method.ReturnType);

                //TODO: add attributes
                //TODO: support generic methods
                builder.AppendFormat("{0} {1} {2} {3}({4}) {{\r\n", visibility, (isInterface == true ? string.Empty : "override"), returnType, method.Name, string.Join(", ", method.GetParameters().Select(x => x.ParameterType.FullName + " " + x.Name)));

                if (isInterface == false)
                {
                    //virtual method
                    builder.Append("var self = this;\r\n");
                }
                else
                {
                    //interface
                    builder.AppendFormat("var self = ({0}) this.target;\r\n", method.DeclaringType.FullName);
                }

                builder.AppendFormat("{0} args = null;\r\n", typeof(InterceptionArgs).FullName);

                if (isInterface == true)
                {
                    if (method.ReturnType != typeof(void))
                    {
                        builder.AppendFormat("args = new {0}(self, this.target.GetType().GetMethod(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType).ToArray()), () => {{ return self.{1}({2}); }}, new object [] {{ {2} }});\r\n", typeof(InterceptionArgs).FullName, method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                    }
                    else
                    {
                        builder.AppendFormat("args = new {0}(self, this.target.GetType().GetMethod(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType).ToArray()), () => {{ self.{1}({2}); return null; }}, new object [] {{ {2} }});\r\n", typeof(InterceptionArgs).FullName, method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                    }
                }
                else
                {
                    if (method.ReturnType != typeof(void))
                    {
                        builder.AppendFormat("args = new {0}(self, (MethodInfo) MethodBase.GetCurrentMethod(), () => {{ return base.{1}({2}); }}, new object [] {{ {2} }});\r\n", typeof(InterceptionArgs).FullName, method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                    }
                    else
                    {
                        builder.AppendFormat("args = new {0}(self, (MethodInfo) MethodBase.GetCurrentMethod(), () => {{ base.{1}({2}); return null; }}, new object [] {{ {2} }});\r\n", typeof(InterceptionArgs).FullName, method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                    }
                }

                builder.Append("this.handler.Invoke(args);\r\n");

                if (method.ReturnType != typeof(void))
                {
                    builder.AppendFormat("if (args.Handled == true) {{ return ({0}) args.Result; }}\r\n", method.ReturnType.FullName);
                    builder.AppendFormat("return {0}.{1}({2});\r\n", (isInterface == true ? "self" : "base"), method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                }
                else
                {
                    builder.Append("if (args.Handled == true) {{ return; }}\r\n");
                    builder.AppendFormat("{0}.{1}({2});\r\n", (isInterface == true ? "self" : "base"), method.Name, string.Join(", ", method.GetParameters().Select(x => x.Name)));
                }

                builder.Append("}\r\n");    //method
            }

        }

        private IEnumerable<ConstructorInfo> GetConstructors(Type baseType)
        {
            return baseType.GetConstructors();
        }

        private static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            if ((type == typeof(object)) || (type == null))
            {
                return Enumerable.Empty<MethodInfo>();
            }

            return type.GetTypeInfo().DeclaredMethods.Where(x => (x.IsStatic == false) || (x.IsSpecialName == false)).Concat(GetMethods(type.BaseType));
        }

        private static IEnumerable<EventInfo> GetEvents(Type type)
        {
            if ((type == typeof(object)) || (type == null))
            {
                return Enumerable.Empty<EventInfo>();
            }

            return type.GetTypeInfo().DeclaredEvents.Concat(GetEvents(type.BaseType)).Concat(type.GetInterfaces().SelectMany(x => GetEvents(x)));
        }

        private IEnumerable<EventInfo> GetEvents(Type baseType, Type[] additionalInterfaceTypes)
        {
            return GetEvents(baseType).Concat(additionalInterfaceTypes.SelectMany(x => GetEvents(x))).Distinct();
        }

        private IEnumerable<MethodInfo> GetMethods(Type baseType, Type[] additionalInterfaceTypes)
        {
            return GetMethods(baseType).Concat(additionalInterfaceTypes.SelectMany(x => GetMethods(x))).Distinct();
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if ((type == typeof(object)) || (type == null))
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            return type.GetTypeInfo().DeclaredProperties.Concat(GetProperties(type.BaseType));
        }

        private IEnumerable<PropertyInfo> GetProperties(Type baseType, Type[] additionalInterfaceTypes)
        {
            return GetProperties(baseType).Concat(additionalInterfaceTypes.SelectMany(x => GetProperties(x))).Distinct();
        }
    }
}
