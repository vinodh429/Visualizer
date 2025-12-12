using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using System.Threading.Tasks;

namespace Visualizer
{
    public partial class VisualizerToolWindowControl : UserControl
    {
        private string _projectPath = "";

        public VisualizerToolWindowControl()
        {
            InitializeComponent();
            InitializeWebViewAsync();

        }

        private void OnSelectProjectClick(object sender, RoutedEventArgs e)
        {
            // Try to get active VS project first
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                if (dte?.Solution?.Projects?.Count > 0)
                {
                    var project = dte.Solution.Projects.Item(1);
                    _projectPath = Path.GetDirectoryName(project.FullName);

                    ProjectPathTextBox.Text = _projectPath;
                    return;
                }
            }
            catch { }

            // fallback: user selects manually
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _projectPath = dialog.SelectedPath;
                ProjectPathTextBox.Text = _projectPath;
            }
        }

        private void OnCodeFlowClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_projectPath))
            {
                MessageBox.Show("Select a project first!");
                return;
            }

            MessageBox.Show("Code Flow generation triggered!");
        }

        private void OnArchitectureClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_projectPath))
            {
                MessageBox.Show("Select a project first!");
                return;
            }

            MessageBox.Show("Architecture Diagram generation triggered!");
        }

        private async void InitializeWebViewAsync()
{
    try
    {
        // Required for VSIX – WebView2 cannot use default temp directory
        string dataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VisuAILizer",
            "WebView2");

        Directory.CreateDirectory(dataFolder);

        var env = await CoreWebView2Environment.CreateAsync(null, dataFolder);

        await WebPanel.EnsureCoreWebView2Async(env);

        // Load a blank HTML shell where diagrams will appear
        string html = @"
            <html>
            <head>
                <style>
                    body {
                        margin: 0;
                        background-color: #0A0F14;
                        color: white;
                        font-family: Segoe UI;
                    }
                </style>
            </head>
            <body>
                <div id='diagramArea'>
                    <h3 style='color:#88D7E0;margin-top:20px;text-align:center;'>
                        Diagram will appear here...
                    </h3>
                </div>
            </body>
            </html>";

        WebPanel.NavigateToString(html);
    }
    catch (Exception ex)
    {
        System.Windows.MessageBox.Show("WebView2 init failed: " + ex.Message);
    }
}

private async Task SetDiagramHtml(string htmlContent)
{
    if (WebPanel?.CoreWebView2 == null)
        return;

    string escaped = htmlContent
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\r", "")
        .Replace("\n", "\\n");

    string js = $@"
        document.getElementById('diagramArea').innerHTML = ""{escaped}"";
    ";

    await WebPanel.CoreWebView2.ExecuteScriptAsync(js);
}
    }
}
