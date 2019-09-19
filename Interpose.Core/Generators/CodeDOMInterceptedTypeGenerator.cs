using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using Interpose.Core.Proxies;
using Microsoft.CSharp;

namespace Interpose.Core.Generators
{
    public sealed class CodeDOMInterceptedTypeGenerator : InterceptedTypeGenerator
    {
        public static readonly InterceptedTypeGenerator Instance = new CodeDOMInterceptedTypeGenerator();

        private static readonly CodeDomProvider provider = new CSharpCodeProvider();
        private static readonly CodeGeneratorOptions options = new CodeGeneratorOptions() { BracingStyle = "C" };
        private static readonly CodeTypeReference proxyTypeReference = new CodeTypeReference(typeof(IInterceptionProxy));
        private static readonly CodeTypeReference interceptorTypeReference = new CodeTypeReference(typeof(IInterceptor));
        private static readonly CodeTypeReference handlerTypeReference = new CodeTypeReference(typeof(IInterceptionHandler));
        private static readonly Assembly proxyAssembly = typeof(IInterceptionProxy).Assembly;
        private static readonly Type interfaceProxyType = typeof(InterfaceProxy);

        private void GenerateConstructors(CodeTypeDeclaration targetClass, Type baseType, IEnumerable<ConstructorInfo> constructors)
        {
            foreach (var constructor in constructors)
            {
                var c = new CodeConstructor();
                targetClass.Members.Add(c);

                c.Attributes = MemberAttributes.Final | MemberAttributes.Override;

                foreach (var parameter in constructor.GetParameters())
                {
                    c.Parameters.Add(new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name));
                }

                if (baseType == interfaceProxyType)
                {
                    c.Attributes |= MemberAttributes.Public;
                }
                else
                {
                    if ((constructor.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
                    {
                        c.Attributes |= MemberAttributes.Public;
                    }
                    else if ((constructor.Attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
                    {
                        c.Attributes |= MemberAttributes.FamilyOrAssembly;
                    }
                    else if ((constructor.Attributes & MethodAttributes.Family) == MethodAttributes.Family)
                    {
                        c.Attributes |= MemberAttributes.Family;
                    }
                    else if ((constructor.Attributes & MethodAttributes.FamANDAssem) == MethodAttributes.FamANDAssem)
                    {
                        c.Attributes |= MemberAttributes.FamilyAndAssembly;
                    }
                }

                foreach (var p in constructor.GetParameters())
                {
                    c.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(p.Name));
                }
            }
        }

        private void GenerateMethods(CodeTypeDeclaration targetClass, Type baseType, IEnumerable<MethodInfo> methods)
        {
            if (methods.Any() == true)
            {
                var finalizeMethod = methods.First().DeclaringType.GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    if (method == finalizeMethod)
                    {
                        continue;
                    }

                    if (method.IsSpecialName == true)
                    {
                        continue;
                    }

                    var m = new CodeMemberMethod();
                    m.Name = method.Name;
                    m.ReturnType = new CodeTypeReference(method.ReturnType);

                    if (baseType != interfaceProxyType)
                    {
                        m.Attributes = MemberAttributes.Override;
                    }

                    targetClass.Members.Add(m);

                    foreach (var parameter in method.GetParameters())
                    {
                        m.Parameters.Add(new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name));
                    }

                    if ((method.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
                    {
                        m.Attributes |= MemberAttributes.Public;
                    }
                    else if ((method.Attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
                    {
                        m.Attributes |= MemberAttributes.FamilyOrAssembly;
                    }
                    else if ((method.Attributes & MethodAttributes.Family) == MethodAttributes.Family)
                    {
                        m.Attributes |= MemberAttributes.Family;
                    }
                    else if ((method.Attributes & MethodAttributes.FamANDAssem) == MethodAttributes.FamANDAssem)
                    {
                        m.Attributes |= MemberAttributes.FamilyAndAssembly;
                    }

                    if (baseType == interfaceProxyType)
                    {
                        m.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    }

                    var currentMethod = new CodeVariableDeclarationStatement(typeof(MethodInfo), "currentMethod", new CodeCastExpression(typeof(MethodInfo), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(MethodBase)), "GetCurrentMethod"))));
                    var getType = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance"), "GetType");
                    var getMethod = new CodeMethodInvokeExpression(getType, "GetMethod", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("currentMethod"), "Name"));
                    var originalMethod = new CodeVariableDeclarationStatement(typeof(MethodInfo), "originalMethod", getMethod);

                    m.Statements.Add(currentMethod);
                    m.Statements.Add(originalMethod);

                    var ps = new CodeExpression[] { new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance"), new CodeVariableReferenceExpression("originalMethod") }.Concat(method.GetParameters().Select(x => new CodeVariableReferenceExpression(x.Name))).ToArray();
                    var arg = new CodeVariableDeclarationStatement(typeof(InterceptionArgs), "args", new CodeObjectCreateExpression(typeof(InterceptionArgs), ps));

                    m.Statements.Add(arg);

                    var handler = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "handler"), "Invoke"), new CodeVariableReferenceExpression("args"));
                    m.Statements.Add(handler);

                    var comparison = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Handled"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(method.ReturnType != typeof(void)));
                    CodeConditionStatement @if = null;

                    if (method.ReturnType != typeof(void))
                    {
                        if (baseType != interfaceProxyType)
                        {
                            @if = new CodeConditionStatement(comparison, new CodeMethodReturnStatement(new CodeCastExpression(method.ReturnType, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Result"))));

                            m.Statements.Add(@if);
                            m.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), method.Name, method.GetParameters().Select(x => new CodeArgumentReferenceExpression(x.Name)).ToArray())));
                        }
                        else
                        {
                            @if = new CodeConditionStatement(comparison, new CodeMethodReturnStatement(new CodeCastExpression(method.ReturnType, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Result"))));

                            m.Statements.Add(@if);
                            m.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeCastExpression(method.DeclaringType, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance")), m.Name), method.GetParameters().Select(x => new CodeArgumentReferenceExpression(x.Name)).ToArray())));
                        }
                    }
                    else
                    {
                        if (baseType != interfaceProxyType)
                        {
                            @if = new CodeConditionStatement(comparison, new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), method.Name, method.GetParameters().Select(x => new CodeArgumentReferenceExpression(x.Name)).ToArray())));

                            m.Statements.Add(@if);
                        }
                        else
                        {
                            @if = new CodeConditionStatement(comparison, new CodeMethodReturnStatement(new CodeCastExpression(method.ReturnType, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Result"))));

                            m.Statements.Add(@if);
                            m.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeCastExpression(method.DeclaringType, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance")), m.Name), method.GetParameters().Select(x => new CodeArgumentReferenceExpression(x.Name)).ToArray())));
                        }
                    }
                }
            }
        }

        private void GenerateProperties(CodeTypeDeclaration targetClass, Type baseType, IEnumerable<PropertyInfo> properties)
        {
            var interceptorProperty = new CodeMemberProperty();
            interceptorProperty.Name = "Interceptor";
            interceptorProperty.Type = interceptorTypeReference;
            interceptorProperty.Attributes |= MemberAttributes.Private;
            interceptorProperty.PrivateImplementationType = proxyTypeReference;
            interceptorProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "interceptor")));
            targetClass.Members.Add(interceptorProperty);

            foreach (var property in properties)
            {
                var p = new CodeMemberProperty();
                p.Name = property.Name;
                p.Type = new CodeTypeReference(property.PropertyType);

                if (baseType != interfaceProxyType)
                {
                    p.Attributes = MemberAttributes.Override;
                }

                targetClass.Members.Add(p);

                if (property.CanRead == true)
                {
                    p.HasGet = true;

                    if ((property.GetMethod.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
                    {
                        p.Attributes |= MemberAttributes.Public;
                    }
                    else if ((property.GetMethod.Attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
                    {
                        p.Attributes |= MemberAttributes.FamilyOrAssembly;
                    }
                    else if ((property.GetMethod.Attributes & MethodAttributes.Family) == MethodAttributes.Family)
                    {
                        p.Attributes |= MemberAttributes.Family;
                    }
                    else if ((property.GetMethod.Attributes & MethodAttributes.FamANDAssem) == MethodAttributes.FamANDAssem)
                    {
                        p.Attributes |= MemberAttributes.FamilyAndAssembly;
                    }

                    if (baseType == interfaceProxyType)
                    {
                        p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    }

                    var currentMethod = new CodeVariableDeclarationStatement(typeof(MethodInfo), "currentMethod", new CodeCastExpression(typeof(MethodInfo), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(MethodBase)), "GetCurrentMethod"))));
                    var getType = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance"), "GetType");
                    var getMethod = new CodeMethodInvokeExpression(getType, "GetMethod", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("currentMethod"), "Name"));
                    var originalMethod = new CodeVariableDeclarationStatement(typeof(MethodInfo), "originalMethod", getMethod);

                    p.GetStatements.Add(currentMethod);
                    p.GetStatements.Add(originalMethod);

                    var ps = new CodeExpression[] { new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance"), new CodeVariableReferenceExpression("originalMethod") };
                    var arg = new CodeVariableDeclarationStatement(typeof(InterceptionArgs), "args", new CodeObjectCreateExpression(typeof(InterceptionArgs), ps));

                    p.GetStatements.Add(arg);

                    var handler = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "handler"), "Invoke"), new CodeVariableReferenceExpression("args"));
                    p.GetStatements.Add(handler);

                    var comparison = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Handled"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
                    var @if = new CodeConditionStatement(comparison, new CodeMethodReturnStatement(new CodeCastExpression(property.PropertyType, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Result"))));

                    p.GetStatements.Add(@if);

                    if (baseType != interfaceProxyType)
                    {
                        p.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), property.Name)));
                    }
                    else
                    {
                        p.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeCastExpression(property.DeclaringType, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance")), property.Name)));
                    }
                }

                if (property.CanWrite == true)
                {
                    p.HasSet = true;

                    var ps = new CodeExpression[]
                    {
                        new CodeThisReferenceExpression(),
                        new CodeCastExpression(typeof(MethodInfo), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(MethodBase)), "GetCurrentMethod"))),
                        new CodeVariableReferenceExpression("value")
                    };

                    var arg = new CodeVariableDeclarationStatement(typeof(InterceptionArgs), "args", new CodeObjectCreateExpression(typeof(InterceptionArgs), ps));

                    p.SetStatements.Add(arg);

                    var handler = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "handler"), "Invoke"), new CodeVariableReferenceExpression("args"));
                    p.SetStatements.Add(handler);

                    var comparison = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("args"), "Handled"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
                    var @if = new CodeConditionStatement(comparison, new CodeMethodReturnStatement());

                    p.SetStatements.Add(@if);

                    if (baseType != interfaceProxyType)
                    {
                        p.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), property.Name), new CodeVariableReferenceExpression("value")));
                    }
                    else
                    {
                        p.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeCastExpression(property.DeclaringType, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "instance")), property.Name), new CodeVariableReferenceExpression("value")));
                    }
                }
            }
        }

        private void GenerateFields(CodeTypeDeclaration targetClass, Type baseType, Type handlerType, Type interceptorType)
        {
            if (handlerType != null)
            {
                var handlerField = new CodeMemberField();
                handlerField.Attributes = MemberAttributes.FamilyOrAssembly;
                handlerField.Name = "handler";
                handlerField.Type = handlerTypeReference;
                handlerField.InitExpression = (handlerType != null) ? new CodeObjectCreateExpression(handlerType) : null;
                targetClass.Members.Add(handlerField);
            }

            var interceptorField = new CodeMemberField();
            interceptorField.Attributes = MemberAttributes.FamilyOrAssembly;
            interceptorField.Name = "interceptor";
            interceptorField.Type = interceptorTypeReference;
            interceptorField.InitExpression = (interceptorType != null) ? new CodeObjectCreateExpression(interceptorType) : null;
            targetClass.Members.Add(interceptorField);
        }

        private void AddReferences(CompilerParameters parameters, Type baseType, Type handlerType, Type interceptorType, Type[] additionalInterfaceTypes)
        {
            parameters.ReferencedAssemblies.Add(string.Concat(proxyAssembly.GetName().Name, Path.GetExtension(proxyAssembly.CodeBase)));
            parameters.ReferencedAssemblies.Add(string.Concat(baseType.Assembly.GetName().Name, Path.GetExtension(baseType.Assembly.CodeBase)));

            if (handlerType != null)
            {
                parameters.ReferencedAssemblies.Add(string.Concat(handlerType.Assembly.GetName().Name, Path.GetExtension(handlerType.Assembly.CodeBase)));
            }

            if (interceptorType != null)
            {
                parameters.ReferencedAssemblies.Add(string.Concat(interceptorType.Assembly.GetName().Name, Path.GetExtension(interceptorType.Assembly.CodeBase)));
            }

            foreach (var additionalInterfaceType in additionalInterfaceTypes)
            {
                parameters.ReferencedAssemblies.Add(string.Concat(additionalInterfaceType.Assembly.GetName().Name, Path.GetExtension(additionalInterfaceType.Assembly.CodeBase)));
            }
        }

        private IEnumerable<PropertyInfo> GetProperties(Type baseType, Type[] additionalInterfaceTypes)
        {
            if (baseType != interfaceProxyType)
            {
                return (baseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => ((x.CanRead == true) && (x.GetMethod.IsVirtual == true)) || ((x.CanWrite == true) && (x.SetMethod.IsVirtual == true))).Concat(additionalInterfaceTypes.SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(y => ((y.CanRead == true) && (y.GetMethod.IsVirtual == true)) || ((y.CanWrite == true) && (y.SetMethod.IsVirtual == true))))).Distinct().ToList());
            }
            else
            {
                return (additionalInterfaceTypes.SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(y => ((y.CanRead == true) && (y.GetMethod.IsVirtual == true)) || ((y.CanWrite == true) && (y.SetMethod.IsVirtual == true)))).Distinct().ToList());
            }
        }

        private IEnumerable<MethodInfo> GetMethods(Type baseType, Type[] additionalInterfaceTypes)
        {
            if (baseType != interfaceProxyType)
            {
                return (baseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsFinal == false && (x.IsVirtual == true || x.IsAbstract == true)).Concat(additionalInterfaceTypes.SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(y => y.IsVirtual == true))).Distinct().ToList());
            }
            else
            {
                return (additionalInterfaceTypes.SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(y => y.IsFinal == false && (y.IsVirtual == true || y.IsAbstract == true))).Distinct().ToList());
            }
        }

        private IEnumerable<ConstructorInfo> GetConstructors(Type baseType)
        {
            return (baseType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
        }

        public override Type Generate(IInterceptor interceptor, Type baseType, Type handlerType, params Type[] additionalInterfaceTypes)
        {
            var properties = this.GetProperties(baseType, additionalInterfaceTypes);
            var methods = this.GetMethods(baseType, additionalInterfaceTypes);
            var constructors = this.GetConstructors(baseType);

            var targetClass = new CodeTypeDeclaration(string.Concat(baseType.Name, "_Dynamic"));
            targetClass.IsClass = baseType.IsClass;
            targetClass.TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Serializable;
            targetClass.BaseTypes.Add((baseType.IsInterface == false) ? baseType : typeof(object));
            targetClass.BaseTypes.Add(proxyTypeReference.BaseType);

            foreach (var additionalInterfaceType in additionalInterfaceTypes)
            {
                targetClass.BaseTypes.Add(additionalInterfaceType);
            }

            var samples = new CodeNamespace(baseType.Namespace);
            samples.Imports.Add(new CodeNamespaceImport(typeof(string).Namespace));
            samples.Types.Add(targetClass);

            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(samples);

            this.GenerateFields(targetClass, baseType, handlerType, (interceptor != null) ? interceptor.GetType() : null);

            this.GenerateConstructors(targetClass, baseType, constructors);

            this.GenerateMethods(targetClass, baseType, methods);

            this.GenerateProperties(targetClass, baseType, properties);

            var builder = new StringBuilder();

            using (var sourceWriter = new StringWriter(builder))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }

            var parameters = new CompilerParameters() { GenerateInMemory = true };

            this.AddReferences(parameters, baseType, handlerType, (interceptor != null) ? interceptor.GetType() : null, additionalInterfaceTypes);

            var results = provider.CompileAssemblyFromDom(parameters, targetUnit);

            if (results.Errors.HasErrors == true)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, results.Errors.OfType<object>()));
            }

            return (results.CompiledAssembly.GetTypes().First());
        }
    }
}
