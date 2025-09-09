using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace TranslatePDF.Services
{
    public class TranslateService
    {

        private readonly string openAPIKey = "";
        public string ExtractTextFromPdf(string filePath)
        {
            using var pdfReader = new PdfReader(filePath);
            using var pdfDoc = new PdfDocument(pdfReader);
            var strategy = new SimpleTextExtractionStrategy();

            string text = "";
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
            }
            return text;
        }
        public async Task<string> TranslateToHindi(string text)
        {
            var prompt = "You are a professional translator. Translate the following text from English to Hindi. Keep the meaning, tone, and context intact. Do not translate proper nouns (like company names, person names, or technical terms). If the text contains lists, tables, or formatting, preserve the same structure in the translated version. If the sentence can have multiple interpretations, choose the most natural one in Hindi.";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAPIKey}");

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new object[]
                {
                    new { role = "system", content = prompt },
                    new { role = "user", content = text }
                }
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
            response.EnsureSuccessStatusCode(); // throw if failed

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new Newtonsoft.Json.JsonTextReader(reader);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            dynamic result = serializer.Deserialize<dynamic>(jsonReader);

            string translated = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            return translated ?? "[Translation failed]";
        }

    }
}
