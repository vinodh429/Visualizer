using System.Collections.Generic;
using System.IO;

namespace VisuAIlizer.Services
{
    public class ProjectAnalyzerService
    {
        private PythonAnalyzerService _python = new PythonAnalyzerService();

        public object AnalyzeProject(string folder)
        {
            var result = new List<object>();

            var pyFiles = Directory.GetFiles(folder, "*.py", SearchOption.AllDirectories);

            foreach (var py in pyFiles)
            {
                var info = _python.AnalyzePythonFile(py);
                result.Add(info);
            }

            return new
            {
                type = "python_project",
                fileCount = pyFiles.Length,
                files = result
            };
        }
    }
}
