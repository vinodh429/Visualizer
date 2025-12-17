private async Task AutoFixMermaidAsync(string brokenMermaid, string error)
{
    StatusText.Text = "Fixing diagram automatically…";

    string fixPrompt = $@"
The following Mermaid diagram has a SYNTAX ERROR.

ERROR MESSAGE:
{error}

BROKEN MERMAID:
{brokenMermaid}

TASK:
- Fix the Mermaid syntax
- Output ONLY valid Mermaid
- No markdown
- No explanations
- One diagram only
";

    var fixedMermaid = await _llm.GenerateMermaidDiagramAsync(
        new { brokenMermaid, error },
        "fix"
    );

    fixedMermaid = fixedMermaid
        .Replace("```mermaid", "")
        .Replace("```", "")
        .Trim();

    _lastMermaid = fixedMermaid;

    ShowMermaidDiagram(fixedMermaid);
}
