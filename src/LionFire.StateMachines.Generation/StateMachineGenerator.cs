using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Validation;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LionFire.StateMachines.Generation
{
    public class StateMachineGenerator : ICodeGenerator
    {
        AttributeData attributeData;
        private StateMachineOptions stateMachineOptions;

        public StateMachineGenerator(AttributeData attributeData)
        {
            Requires.NotNull(attributeData, nameof(attributeData));
            this.attributeData = attributeData;
            this.stateMachineOptions = (StateMachineOptions)attributeData.ConstructorArguments[0].Value;
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (this.stateMachineOptions.HasFlag(StateMachineOptions.NoGeneration)) { return Task.FromResult(new SyntaxList<MemberDeclarationSyntax>()); }

            if (context.ProcessingMember == null) throw new ArgumentNullException("context.ProcessingMember");
            var applyToClass = (ClassDeclarationSyntax)context.ProcessingMember;
            if (context.SemanticModel == null) throw new Exception("no SemanticModel");
            var sm = context.SemanticModel;
            var typeInfo = context.SemanticModel.GetTypeInfo(applyToClass);
            //if (typeInfo == null) throw new Exception("Couldn't get TypeInfo");

            //CompilationUnitSyntax cu = SF.ClassDeclaration()
            //       .AddUsings(SF.UsingDirective(SF.IdentifierName("System")))
            //       .AddUsings(SF.UsingDirective(SF.IdentifierName("LionFire.StateMachines")))
            //    ;
            //sm.GetDeclaredSymbol(
            //applyToClass.Ty
            ClassDeclarationSyntax c = SF.ClassDeclaration(applyToClass.Identifier)
                //.AddModifiers(SF.Token(SyntaxKind.PublicKeyword)) // TODO - 

                .AddModifiers(SF.Token(SyntaxKind.PartialKeyword))
                ;

            var m = SF.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "GenTest");
            //m = m.WithBody((BlockSyntax)BlockSyntax.DeserializeFrom(new MemoryStream(UTF8Encoding.UTF8.GetBytes("{}"))));
            var block = SF.Block();
            m = m.WithBody(block);
            c = c.AddMembers(m);


            //NamespaceDeclarationSyntax ns = SF.NamespaceDeclaration(SF.IdentifierName(typeInfo.Type.ContainingNamespace.Name));
            //cu = cu.AddMembers(ns);

            //ClassDeclarationSyntax c = SF.ClassDeclaration(typeInfo.Type.Name)
            //    //.AddModifiers(SF.Token(SyntaxKind.PrivateKeyword))
            //    .AddModifiers(SF.Token(SyntaxKind.PartialKeyword))
            //    ;
            //ns = ns.AddMembers(c);

            var results = SyntaxFactory.List<MemberDeclarationSyntax>();
            results = results.Add(c);
            return Task.FromResult(results);



            //// Our generator is applied to any class that our attribute is applied to.



            //ns = ns.AddMembers(c);


            //// Apply a suffix to the name of a copy of the class.
            ////var copy = applyToClass .WithIdentifier(SyntaxFactory.Identifier(applyToClass.Identifier.ValueText));

            //applyToClass

            //// Return our modified copy. It will be added to the user's project for compilation.
            //results = results.Add(copy);
            //return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results);
        }
    }

}
