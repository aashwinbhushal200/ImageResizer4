namespace AnalyzerService.Abstraction
{
    public interface IAnalyzerService
    {
        Task<dynamic> AnalyzeAsync(byte[] image);
    }
}
