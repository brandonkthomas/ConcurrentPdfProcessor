using ConcurrentPdfProcessor.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace ConcurrentPdfProcessor.Services;

// --------------------------------------------------------------------------------------------
/// <summary>
/// Creates PDFs and simulates OCR processing (no 3rd party dependencies required)
/// </summary>
public class PdfService : IDisposable
{
    private readonly Random _random = new();

    public PdfService()
    {
        // nothing to do here (no native dependencies required for simulation)
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a simple PDF with text content
    /// </summary>
    public async Task<string> CreateSamplePdfAsync(
        string fileName, 
        string content, 
        int pageCount = 1)
    {
        var filePath = $"{fileName}.pdf";
        
        using var writer = new PdfWriter(filePath);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        for (int page = 1; page <= pageCount; page++)
        {
            var pageContent = $"Page {page}\n\n{content}\n\nGenerated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            
            var paragraph = new Paragraph(pageContent)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.LEFT);
            
            document.Add(paragraph);
            
            if (page < pageCount)
            {
                document.Add(new AreaBreak());
            }
        }

        await Task.CompletedTask; // Simulate async operation
        return filePath;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Simulates OCR processing on a PDF file and extracts text
    /// This simulates the I/O-bound nature of OCR operations
    /// </summary>
    public async Task<PdfOcrResult> ProcessPdfWithOcrAsync(string pdfPath)
    {
        var startTime = DateTime.UtcNow;
        var threadId = Environment.CurrentManagedThreadId;
        var fileName = Path.GetFileName(pdfPath);
        
        Console.WriteLine($"  Starting OCR for {fileName} on thread {threadId}");
        
        try
        {
            //
            // Simulate PDF reading and page extraction
            //
            var pageCount = await GetPdfPageCountAsync(pdfPath);
            
            //
            // Simulate OCR processing time (I/O-bound operation)
            //
            var ocrDelay = 300 + _random.Next(400); // 300-700ms per PDF
            await Task.Delay(ocrDelay);
            
            //
            // Simulate text extraction from each page
            //
            var extractedText = await SimulateTextExtractionAsync(fileName, pageCount);
            
            var result = new PdfOcrResult
            {
                FileName = fileName,
                ExtractedText = extractedText,
                PageCount = pageCount,
                ProcessingTime = DateTime.UtcNow - startTime,
                ThreadId = threadId
            };
            
            Console.WriteLine($"  Completed OCR for {fileName} on thread {threadId}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error processing {fileName}: {ex.Message}");
            return new PdfOcrResult
            {
                FileName = fileName,
                ExtractedText = $"Error: {ex.Message}",
                PageCount = 0,
                ProcessingTime = DateTime.UtcNow - startTime,
                ThreadId = threadId
            };
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the page count of a PDF file
    /// </summary>
    private async Task<int> GetPdfPageCountAsync(string pdfPath)
    {
        //
        // Simulate async file reading (not done in this demo to simplify)
        //
        await Task.Delay(50 + _random.Next(100));
        
        using var pdfDocument = new PdfDocument(new PdfReader(pdfPath));
        return pdfDocument.GetNumberOfPages();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Simulates text extraction from PDF pages
    /// </summary>
    private async Task<string> SimulateTextExtractionAsync(
        string fileName, 
        int pageCount)
    {
        var extractedText = new List<string>();
        
        for (int page = 1; page <= pageCount; page++)
        {
            //
            // Simulate OCR processing time per page
            //
            await Task.Delay(100 + _random.Next(200));
            
            var pageText = $"Page {page} from {fileName}: This is simulated OCR text extracted from the PDF document. " +
                          $"The content includes various characters and formatting that would be recognized by a real OCR engine. " +
                          $"Processing time and accuracy are simulated for demonstration purposes.";
            
            extractedText.Add(pageText);
        }
        
        return string.Join("\n\n", extractedText);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates multiple sample PDFs with different content
    /// </summary>
    public async Task<List<string>> CreateSamplePdfsAsync(int count = 5)
    {
        var pdfFiles = new List<string>();
        var sampleTexts = new[]
        {
            "This is a sample document about C# concurrency patterns and async programming. The Task-based Asynchronous Pattern (TAP) provides a powerful way to handle I/O-bound operations efficiently.",
            "The Task-based Asynchronous Pattern (TAP) is the recommended approach for async operations in .NET. It allows threads to be freed during I/O operations, improving overall application performance.",
            "Concurrency allows multiple operations to run simultaneously without blocking threads. This is different from parallelism, which uses multiple threads for CPU-bound work.",
            "Async and await keywords make it easy to write non-blocking code in C#. They yield control back to the caller while waiting for I/O operations to complete.",
            "Performance improvements from concurrency can be dramatic for I/O-bound operations. PDF processing and OCR are perfect examples of operations that benefit from concurrent execution."
        };

        for (int i = 1; i <= count; i++)
        {
            var fileName = $"sample-document-{i}";
            var content = sampleTexts[(i - 1) % sampleTexts.Length];
            var pageCount = 1 + (i % 3); // 1-3 pages
            
            var pdfPath = await CreateSamplePdfAsync(fileName, content, pageCount);
            pdfFiles.Add(pdfPath);
        }

        return pdfFiles;
    }

    public void Dispose()
    {
        // nothing to do here
    }
} 