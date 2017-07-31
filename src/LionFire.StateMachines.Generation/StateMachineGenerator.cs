using CodeGeneration.Roslyn;
using LionFire.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Validation;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Runtime.Loader;

// Tool: http://roslynquoter.azurewebsites.net/

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


            AssemblyLoadContext.Default.Resolving += Default_Resolving;
        }

        private Assembly Default_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            throw new NotImplementedException("Resolving - TEST - " + arg2);
        }

        public class TransitionInfo
        {
            public Enum EnumValue { get; set; }
            public string PresentTense { get; set; }
            public string PastTense { get; set; }
        }
        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (this.stateMachineOptions.HasFlag(StateMachineOptions.NoGeneration)) { return Task.FromResult(new SyntaxList<MemberDeclarationSyntax>()); }

            if (context.ProcessingMember == null) throw new ArgumentNullException("context.ProcessingMember");
            var dClass = (ClassDeclarationSyntax)context.ProcessingMember;
            if (context.SemanticModel == null) throw new Exception("no SemanticModel");
            var sm = context.SemanticModel;
            var typeInfo = context.SemanticModel.GetTypeInfo(dClass);

            //throw new Exception("Location: " + Assembly.GetEntryAssembly().Location);
            //throw new Exception("BaseDir: " + AppContext.BaseDirectory);

            //foreach (var n in typeof(ExecutionState).GetFields().Select(fi => fi.Name))
            //{
            //    Console.WriteLine(" - " + n);
            //    //throw new Exception("------------------ " + n);
            //}
            var allStates = new HashSet<string>(typeof(TState).GetFields().Select(fi => fi.Name));
            var allTransitions = new HashSet<string>(typeof(ExecutionTransition).GetFields().Select(fi => fi.Name));
            var usedStates = new HashSet<string>();
            var usedTransitions = new HashSet<string>();
            //usedTransitions.Add("testtransition");
            foreach (var md in dClass.Members.OfType<MethodDeclarationSyntax>())
            {
                foreach (var s in allTransitions)
                {
                    if (s == "value__") continue;
                    if (md.Identifier.Text.EndsWith(s))
                    {
                        //Console.WriteLine("------------- " + "Found relevant method: TODO " + md.Identifier.Text);
                        //throw new Exception("Found relevant method: TODO " + md.Identifier.Text);
                        if (!usedTransitions.Contains(s))
                        {
                            usedTransitions.Add(s);
                        }
                    }
                }
            }



            //if (typeInfo == null) throw new Exception("Couldn't get TypeInfo");

            //CompilationUnitSyntax cu = SF.ClassDeclaration()
            //       .AddUsings(SF.UsingDirective(SF.IdentifierName("System")))
            //       .AddUsings(SF.UsingDirective(SF.IdentifierName("LionFire.StateMachines")))
            //    ;
            //sm.GetDeclaredSymbol(
            //applyToClass.Ty
            ClassDeclarationSyntax c = SF.ClassDeclaration(dClass.Identifier)
                //.AddModifiers(SF.Token(SyntaxKind.PublicKeyword)) // TODO - 

                .AddModifiers(SF.Token(SyntaxKind.PartialKeyword))
                ;

            //foreach (var used in usedStates)
            //{

            //    var m = SF.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), used);
            //    //m = m.WithBody((BlockSyntax)BlockSyntax.DeserializeFrom(new MemoryStream(UTF8Encoding.UTF8.GetBytes("{}"))));
            //    var block = SF.Block();
            //    m = m.WithBody(block);
            //    c = c.AddMembers(m);
            //}
            foreach (var used in usedTransitions)
            {
                var md = MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.VoidKeyword)),
                            Identifier(used))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                //SingletonList<StatementSyntax>(
                                //    ExpressionStatement(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("stateMachine"),
                                                IdentifierName("ChangeState")))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList<ArgumentSyntax>(
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("ExecutionTransition"),
                                                            IdentifierName("Initialize"))))))))
                                                            //))
                                                            .WithSemicolonToken( Token(SyntaxKind.SemicolonToken))
                ;


                //var m = SF.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), used);
                //m = m.WithBody((BlockSyntax)BlockSyntax.DeserializeFrom(new MemoryStream(UTF8Encoding.UTF8.GetBytes("{}"))));
                //var block = SF.Block();

                ////StateMachine.ChangeState(ExecutionTransition.Initialize);
                //block.AddStatements(
                //    SF.ExpressionStatement(SyntaxFactory.InvocationExpression(
                //            SF.MemberAccessExpression(
                //                SyntaxKind.SimpleMemberAccessExpression,
                //                SF.IdentifierName("StateMachine"),
                //                SF.IdentifierName("ChangeState")
                //            )),
                //        SF.SeparatedList<SyntaxNode>(
                //            //SF.Argument(SF.MemberAccessExpression(SyntaxKind.Enum))
                //            )
                //            )
                //    )
                //    );

                //m = m.WithBody(block);
                c = c.AddMembers(md);
            }

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
