private void ShowMermaidDiagram(string mermaid)
{
    string escaped = System.Net.WebUtility.HtmlEncode(mermaid);

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

    function render() {{
      const code = document.getElementById('src').innerText;
      try {{
        mermaid.parse(code);
        document.getElementById('view').innerHTML =
          '<div class=""mermaid"">' + code + '</div>';
        mermaid.init(undefined, document.querySelectorAll('.mermaid'));
      }} catch (err) {{
        // 🔥 AUTO SEND ERROR TO HOST
        window.chrome.webview.postMessage({{
          type: 'mermaid_error',
          error: err.message,
          code: code
        }});
      }}
    }}
  </script>
</head>

<body style='margin:0;background:#0e1726;color:white;padding:16px'
      onload='render()'>
  <pre id='src' style='display:none'>{escaped}</pre>
  <div id='view'></div>
</body>
</html>";

    WebPanel.NavigateToString(html);
}
