using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace VisuAIlizer.Services
{
    public class PythonAnalyzerService
    {
        public object AnalyzePythonFile(string filePath)
        {
            string exe = "python"; // assumes python is in PATH
            string script = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "py_ast_extractor.py");

            if (!File.Exists(script))
                throw new FileNotFoundException("Missing script: " + script);

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"\"{script}\" \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = Process.Start(psi);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return JsonSerializer.Deserialize<object>(output);
        }
    }
}
