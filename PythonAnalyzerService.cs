using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace VisuAIlizer.Services
{
    public class PythonAnalyzerService
    {
        public object AnalyzePythonFile(string filePath)
{
    // Get installed VSIX location
    var extensionRoot = Path.GetDirectoryName(
        typeof(VisuAIlizer.VisualizerToolWindowControl).Assembly.Location
    );

    string script = Path.Combine(extensionRoot, "ExternalScripts", "py_ast_extractor.py");

    if (!File.Exists(script))
    {
        throw new FileNotFoundException("Python AST script not found at: " + script);
    }

    var psi = new ProcessStartInfo
    {
        FileName = "python",
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
