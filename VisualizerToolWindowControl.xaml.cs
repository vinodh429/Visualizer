using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Visualizer
{
    public partial class VisualizerToolWindowControl : UserControl
    {
        private string _projectPath = "";

        public VisualizerToolWindowControl()
        {
            InitializeComponent();
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
    }
}
