using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace TranslatePDF.Services
{
    public class UploadFileService
    {
        public bool CheckAndAbleToReadTheFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    Console.WriteLine("PDF is readable");
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("You don't have permission to read this file.");
                return false;
            }
            catch (IOException)
            {
                Console.WriteLine("File is locked or not accessible.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        public void CreatePdfWithText(string filePath, string text)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Content().Text(text, TextStyle.Default.FontSize(12));
                });
            }).GeneratePdf(filePath);
        }
    }
}
