namespace ConcurrentPdfProcessor.Models;

/// <summary>
/// Stores OCR results from PDF processing
/// </summary>
public class PdfOcrResult
{
    public string FileName { get; set; } = string.Empty;
    public string ExtractedText { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public int ThreadId { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    public override string ToString()
    {
        return $"PDF: {FileName} | Pages: {PageCount} | Text Length: {ExtractedText.Length} chars | Time: {ProcessingTime.TotalMilliseconds:F0}ms | Thread: {ThreadId}";
    }
} 