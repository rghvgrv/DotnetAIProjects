using Microsoft.AspNetCore.Mvc;
using TranslatePDF.Services;

namespace TranslatePDF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadFileController : ControllerBase
    {
        public readonly TranslateService _translateService;
        public readonly UploadFileService _uploadFileService;
        public UploadFileController(TranslateService translateService, UploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
            _translateService = translateService;
        }
        [HttpGet("Test")]
        public string Test()
        {
            return "Hello";
        }

        [HttpGet("UploadFileToLocal")]
        public async Task<string> UploadFileToLocalAsync()
        {
            string filePath = @"D:\Dotnet_Projects\TranslatePDF\Resources\Test.pdf";

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            bool isFileReadable = _uploadFileService.CheckAndAbleToReadTheFile(filePath);
            if (!isFileReadable)
            {
                return "File is not readable";
            }

            // 1. Extract text
            string extractedText = _translateService.ExtractTextFromPdf(filePath);

            // 2. Translate text
            string translatedText = await _translateService.TranslateToHindi(extractedText);

            // 3. Create new PDF with translated text (same name, with "_translated")
            string originalFileName = Path.GetFileNameWithoutExtension(filePath);
            string translatedPath = Path.Combine(uploadsFolder, $"{originalFileName}_translated.pdf");

            _uploadFileService.CreatePdfWithText(translatedPath, translatedText);

            // 4. Return translated file path
            return translatedPath;
        }
    }
}
