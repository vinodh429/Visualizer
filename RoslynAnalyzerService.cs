using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisuAIlizer.Services
{
    public class RoslynAnalyzerService
    {
        private bool _registered = false;

        public RoslynAnalyzerService()
        {
            EnsureMSBuildRegistered();
        }

        private void EnsureMSBuildRegistered()
        {
            if (_registered) return;

            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (instances.Length == 0)
                throw new Exception("No MSBuild instances found — VS Build Tools missing.");

            MSBuildLocator.RegisterInstance(instances[0]);
            _registered = true;
        }

        public async Task<List<object>> AnalyzeProjectAsync(string path)
        {
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("MSBuildWorkspace ERROR: " + e.Diagnostic.Message);
            };

            Solution solution = null;

            if (path.EndsWith(".sln"))
                solution = await workspace.OpenSolutionAsync(path);
            else if (path.EndsWith(".csproj"))
            {
                var proj = await workspace.OpenProjectAsync(path);
                solution = proj.Solution;
            }
            else
                throw new Exception("Select a .sln or .csproj file.");

            var projectsList = new List<object>();

            foreach (var proj in solution.Projects)
            {
                var projObj = new
                {
                    ProjectName = proj.Name,
                    Classes = new List<object>()
                };

                foreach (var doc in proj.Documents)
                {
                    var root = await doc.GetSyntaxRootAsync();
                    var model = await doc.GetSemanticModelAsync();
                    if (root == null || model == null) continue;

                    var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                    foreach (var cls in classes)
                    {
                        var sym = model.GetDeclaredSymbol(cls);

                        var clsObj = new
                        {
                            Class = sym?.ToDisplayString(),
                            Methods = new List<object>()
                        };

                        var methods = cls.DescendantNodes().OfType<MethodDeclarationSyntax>();

                        foreach (var method in methods)
                        {
                            var mSym = model.GetDeclaredSymbol(method);
                            var calls = method.DescendantNodes()
                                .OfType<InvocationExpressionSyntax>()
                                .Select(inv => model.GetSymbolInfo(inv).Symbol?.ToDisplayString())
                                .Where(x => x != null)
                                .ToList();

                            ((List<object>)clsObj.Methods).Add(new
                            {
                                Method = mSym?.ToDisplayString(),
                                Calls = calls
                            });
                        }

                        ((List<object>)projObj.Classes).Add(clsObj);
                    }
                }

                projectsList.Add(projObj);
            }

            return projectsList;
        }
    }
}
