private void ShowMermaidDiagram(string mermaid)
{
    string escapedMermaid = System.Net.WebUtility.HtmlEncode(mermaid);

    string html = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'/>
  <script src='https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js'></script>
  <script>
    mermaid.initialize({{
        startOnLoad: false,
        theme: 'dark'
    }});

    function renderDiagram() {{
        const code = document.getElementById('src').innerText;

        try {{
            mermaid.parse(code);   // 🔴 SYNTAX CHECK
            document.getElementById('diagram').innerHTML = '<div class=""mermaid"">' + code + '</div>';
            mermaid.init(undefined, document.querySelectorAll('.mermaid'));
        }} catch (err) {{
            document.getElementById('diagram').innerHTML = `
                <div style='color:#ff6b6b; font-family:Consolas;'>
                    <h3>⚠ Mermaid syntax error</h3>
                    <pre>${{err.message}}</pre>
                </div>
            `;
        }}
    }}
  </script>
</head>
<body style='margin:0;background:#0e1726;color:white;padding:16px;' onload='renderDiagram()'>
  <pre id='src' style='display:none;'>{escapedMermaid}</pre>
  <div id='diagram'></div>
</body>
</html>";

    WebPanel.NavigateToString(html);
}
