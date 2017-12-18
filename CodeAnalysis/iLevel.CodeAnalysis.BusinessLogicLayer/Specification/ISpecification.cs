using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Specification
{
    public interface ISpecification
    {
        bool IsStatisfiedBy(ReportDTO report);
    }
}
