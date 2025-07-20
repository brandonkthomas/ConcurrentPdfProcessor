using ConcurrentPdfProcessor.Services;
using ConcurrentPdfProcessor.Models;

namespace ConcurrentPdfProcessor;

// --------------------------------------------------------------------------------------------
/// <summary>
/// Demonstrates concurrency in PDF OCR processing operations
/// Shows how async/await improves performance when processing multiple PDFs
/// </summary>
class Program
{
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        Console.WriteLine("===== Testing PDF OCR Processing using C# Concurrency =====");
        Console.WriteLine("Compares async/await vs blocking approach for I/O operations\n");

        using var pdfService = new PdfService();
        
        //
        // Create sample PDFs to process
        //
        await CreateSamplePdfsAsync(pdfService);
        
        //
        // compare sequential vs concurrent
        //
        await ProcessSequentiallyAsync(pdfService);
        await ProcessConcurrentlyAsync(pdfService);
        await ProcessConcurrentlyWithProgressAsync(pdfService);
        
        Console.WriteLine("\n===== Done. =====\n");
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pdfService"></param>
    /// <returns></returns>
    static async Task CreateSamplePdfsAsync(PdfService pdfService)
    {        
        var pdfFiles = await pdfService.CreateSamplePdfsAsync(5);
        Console.WriteLine($"Created {pdfFiles.Count} sample PDFs\n");
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pdfService"></param>
    /// <returns></returns>
    static async Task ProcessSequentiallyAsync(PdfService pdfService)
    {
        Console.WriteLine("===== Sequential PDF OCR Processing =====");
        Console.WriteLine("Processing PDFs individually (blocking approach)...");
        
        var startTime = DateTime.UtcNow;
        var pdfFiles = Directory.GetFiles(".", "*.pdf");
        
        var results = new List<PdfOcrResult>();
        foreach (var pdfFile in pdfFiles)
        {
            var result = await pdfService.ProcessPdfWithOcrAsync(pdfFile);
            results.Add(result);
            Console.WriteLine($"  > {result}");
        }
        
        var duration = DateTime.UtcNow - startTime;
        Console.WriteLine($"Sequential OCR processing completed in {duration.TotalMilliseconds:F0}ms");
        Console.WriteLine($"Total text extracted: {results.Sum(r => r.ExtractedText.Length)} characters\n");
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pdfService"></param>
    /// <returns></returns>
    static async Task ProcessConcurrentlyAsync(PdfService pdfService)
    {
        Console.WriteLine("===== Concurrent PDF OCR Processing =====");
        Console.WriteLine("Processing all PDFs simultaneously (non-blocking approach)...");
        
        var startTime = DateTime.UtcNow;
        var pdfFiles = Directory.GetFiles(".", "*.pdf");
        
        //
        // Start all OCR processing tasks concurrently
        //
        var tasks = pdfFiles.Select(pdfFile => pdfService.ProcessPdfWithOcrAsync(pdfFile));
        var results = await Task.WhenAll(tasks);
        
        var duration = DateTime.UtcNow - startTime;
        
        //
        // Print results
        //
        foreach (var result in results)
        {
            Console.WriteLine($"  > {result}");
        }
        
        Console.WriteLine($"Concurrent OCR processing completed in {duration.TotalMilliseconds:F0}ms");
        Console.WriteLine($"Total text extracted: {results.Sum(r => r.ExtractedText.Length)} characters\n");
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pdfService"></param>
    /// <returns></returns>
    static async Task ProcessConcurrentlyWithProgressAsync(PdfService pdfService)
    {
        Console.WriteLine("===== Concurrent OCR Processing with Progress Tracking =====");
        Console.WriteLine("Processing PDFs concurrently while tracking progress...");
        
        var pdfFiles = Directory.GetFiles(".", "*.pdf");
        var startTime = DateTime.UtcNow;
        
        //
        // Process PDFs concurrently but track completion as they finish
        //
        var tasks = pdfFiles.Select(async pdfFile =>
        {
            var result = await pdfService.ProcessPdfWithOcrAsync(pdfFile);
            Console.WriteLine($"  > Completed OCR: {Path.GetFileName(pdfFile)}");
            return result;
        });
        
        var results = await Task.WhenAll(tasks);
        var duration = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"All PDFs processed in {duration.TotalMilliseconds:F0}ms");
        Console.WriteLine($"Average time per PDF: {duration.TotalMilliseconds / pdfFiles.Length:F0}ms");
        Console.WriteLine($"Total pages processed: {results.Sum(r => r.PageCount)}");
        Console.WriteLine($"Total text extracted: {results.Sum(r => r.ExtractedText.Length)} characters\n");
    }
}
