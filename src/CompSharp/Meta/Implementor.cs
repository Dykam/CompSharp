using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CompSharp.Meta
{
    internal static class Implementor
    {
        private static readonly ConcurrentDictionary<Type, Type> BuildTypes = new ConcurrentDictionary<Type, Type>();

        public static Type BuildImplementation(Type componentType) => BuildTypes.GetOrAdd(componentType, type =>
        {
            var properties =
                from prop in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                let accessors =
                    from accessor in prop.GetAccessors(false)
                    let declaration = prop.GetGetMethod() == accessor
                        ? SyntaxKind.GetAccessorDeclaration
                        : prop.GetSetMethod() == accessor
                            ? SyntaxKind.SetAccessorDeclaration
                            : SyntaxKind.None
                    where declaration != SyntaxKind.None
                    select SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                select SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(prop.PropertyType.FullName), prop.Name) // Type Name {}
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)) // public
                    .AddAccessorListAccessors(accessors.ToArray());

            var @namespace = $"CompSharp.Generated.{componentType.Namespace}";
            var typeName = $"SimpleType_{componentType.Name}";
            var comp = SyntaxFactory.CompilationUnit()
                .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace))
                    .AddMembers(SyntaxFactory.ClassDeclaration(typeName)
                        .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(componentType.FullName)))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddMembers(properties.Cast<MemberDeclarationSyntax>().ToArray())));

            var references = new List<MetadataReference>();
            foreach (var referencedAssembly in componentType.Assembly.GetReferencedAssemblies())
            {
                try
                {
                    TryAdd(references, Assembly.ReflectionOnlyLoad(referencedAssembly.FullName).Location);
                }
                catch
                {
                    // ignored
                }
            }
            TryAdd(references, typeof (object).Assembly.Location);
            TryAdd(references, componentType.Assembly.Location);
            var compilation = CSharpCompilation.Create(@namespace, new[] {comp.SyntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                        Console.Error.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    return assembly.GetType($"{@namespace}.{typeName}");

                }
            }
            throw new Exception($"Failed to implement interface {componentType.FullName}");
        });

/*
        private static void TryASdd(ICollection<MetadataReference> references, MemoryStream ms)
        {
            try
            {
                references.Add(MetadataReference.CreateFromStream(ms));
            }
            catch
            {
                // ignored
            }
        }
*/

        private static void TryAdd(ICollection<MetadataReference> references, string location)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(location));
            }
            catch
            {
                // ignored
            }
        }
    }
}