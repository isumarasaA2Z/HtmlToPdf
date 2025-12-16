using HtmlToPdf.core.Interfaces;

namespace HtmlToPdf.core.Services
{
    public class TemplateLoader : ITemplateLoader
    {
        private readonly string _templatesPath;

        public TemplateLoader(string templatesPath = null)
        {
            _templatesPath = templatesPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

            // Create templates directory if it doesn't exist
            if (!Directory.Exists(_templatesPath))
            {
                Directory.CreateDirectory(_templatesPath);

                // Create default template file
                var defaultTemplatePath = Path.Combine(_templatesPath, "default.html");
                if (!File.Exists(defaultTemplatePath))
                {
                    File.WriteAllText(defaultTemplatePath, GetDefaultTemplate());
                }
            }
        }

        public async Task<string> LoadTemplateAsync(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                return GetDefaultTemplate();
            }

            // Ensure .html extension
            if (!templateName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                templateName += ".html";
            }

            var templatePath = Path.Combine(_templatesPath, templateName);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template '{templateName}' not found at path: {templatePath}");
            }

            return await File.ReadAllTextAsync(templatePath);
        }

        public string GetDefaultTemplate()
        {
            return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Purchase Order Quotation</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { text-align: center; margin-bottom: 30px; }
        h1 { text-align: center; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f4f4f4; }
        .signature-section { margin-top: 60px; text-align: center; border-top: 2px solid #333; padding-top: 20px; }
    </style>
</head>
<body>
    <div class=""header"">
        #{image.logo}#
    </div>

    <h1>Purchase Order Quotation</h1>
    <p>This Purchase Order <strong>#{text.orderNO}#</strong> contains the following data:</p>

    <table>
        <tr>
            <th>Order No</th>
            <th>Line No</th>
            <th>Part No</th>
            <th>Quantity</th>
        </tr>
        #{table.Order}#
    </table>

    <h2>Delivery Locations:</h2>
    <ul>
        <li>No: 5, Jayamalapauara, Gampola</li>
        <li>No: 10 Kandy Road, Peradeniya</li>
        <li>No: 55 KCC, Kandy</li>
    </ul>

    <p>Order Reference: <strong>#{text.orderno1}#</strong></p>

    <div class=""signature-section"">
        <p><strong>Authorized Signature:</strong></p>
        #{image.signature}#
    </div>
</body>
</html>";
        }
    }
}
