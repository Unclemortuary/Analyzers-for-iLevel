using System.Collections.Generic;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using CodeAnalysisService.Models;

namespace CodeAnalysisService.Infrastructure
{
    public interface IMapper
    {
        IEnumerable<SourceFileDTO> ToSourceFileDTO(Dictionary<string, string> uploadedSources);
        IEnumerable<ReportViewModel> ToReportViewModel(IEnumerable<ReportDTO> reportDTO);
    }

    class Mapper : IMapper
    {
        public IEnumerable<SourceFileDTO> ToSourceFileDTO(Dictionary<string, string> uploadedSources)
        {
            List<SourceFileDTO> result = new List<SourceFileDTO>();
            foreach (var file in uploadedSources)
            {
                result.Add(new SourceFileDTO { Name = file.Key, Text = file.Value });
            }
            return result;
        }

        public IEnumerable<ReportViewModel> ToReportViewModel(IEnumerable<ReportDTO> reportDTO)
        {
            List<ReportViewModel> result = new List<ReportViewModel>();
            foreach (var dto in reportDTO)
            {
                result.Add(new ReportViewModel
                {
                    FileName = dto.FileName,
                    Locatin = dto.Location,
                    Severety = dto.Severety,
                    Message = dto.Message,
                    AnalyzerID = dto.AnalyzerID
                });
            }
            return result;
        }
    }
}