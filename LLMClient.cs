using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VisuAIlizer.Services
{
    public class LlmClient
    {
        private readonly HttpClient _client = new HttpClient();
        private const string Endpoint = "http://localhost:11434/api/chat";

        public async Task<string> GenerateMermaidDiagramAsync(object jsonPayload, string mode)
        {
            string systemPrompt = @"
You output ONLY Mermaid diagram code. 
Never output markdown.
Never output explanations.
Never output comments.
Never use ``` fences.
";

            string taskInstruction = mode switch
            {
                "codeflow" => @"
Generate a Mermaid FLOWCHART diagram.
Use: flowchart TD
Nodes = functions/methods
Edges = function calls
",
                "architecture" => @"
Generate a Mermaid CLASS DIAGRAM.
Use: classDiagram
Nodes = classes
Edges = relationships
",
                _ => "Invalid mode"
            };

            string userPrompt = $@"
{taskInstruction}

Project JSON:
{JsonSerializer.Serialize(jsonPayload)}
";

            var requestBody = new
            {
                model = "llama3.1:8b",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                stream = false
            };

            string json = JsonSerializer.Serialize(requestBody);
            var response = await _client.PostAsync(Endpoint,
                new StringContent(json, Encoding.UTF8, "application/json"));

            string body = await response.Content.ReadAsStringAsync();

            var root = JsonDocument.Parse(body);
            string mermaid = root.RootElement
                                 .GetProperty("message")
                                 .GetProperty("content")
                                 .GetString()
                                 .Trim();

            // Remove backticks if model tries to add them
            mermaid = mermaid.Replace("```mermaid", "").Replace("```", "").Trim();

            return mermaid;
        }
    }
}
