namespace APCapstoneProject.Service
{
    public class EmailTemplateRenderer
    {
        private readonly string _templatesPath;

        public EmailTemplateRenderer(string templatesPath)
        {
            _templatesPath = templatesPath;
        }

        public async Task<string> RenderAsync(string templateName, IDictionary<string, string?> tokens)
        {
            var path = Path.Combine(_templatesPath, templateName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Email template not found: {path}");

            var html = await File.ReadAllTextAsync(path);
            foreach (var kv in tokens)
            {
                html = html.Replace("{{" + kv.Key + "}}", kv.Value ?? string.Empty);
            }
            return html;
        }
    }
}
