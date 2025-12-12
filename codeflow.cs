private void OnCodeFlowClick(object sender, RoutedEventArgs e)
{
    string folder = ProjectPathTextBox.Text;

    var analysis = _projectAnalyzer.AnalyzeProject(folder);

    string json = JsonSerializer.Serialize(analysis, new JsonSerializerOptions
    {
        WriteIndented = true
    });

    WebPanel.NavigateToString(
        $"<html><body style='background:#0e1726;color:white;'>"
        + $"<pre>{System.Net.WebUtility.HtmlEncode(json)}</pre>"
        + "</body></html>"
    );
}
