using AnalyzerService.Abstraction;

namespace AnalyzerService
{
    public class ComputerVisionAnalyzerService : IAnalyzerService
    {
       
        public async Task<dynamic> AnalyzeAsync(byte[] image)
        {
            throw new NotImplementedException();
        }
    }
}
