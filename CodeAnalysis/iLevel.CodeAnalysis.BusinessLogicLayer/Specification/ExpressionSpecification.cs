using System;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Specification
{
    public class ExpressionSpecification : ISpecification
    {
        private Func<ReportDTO, bool> _expression;

        public ExpressionSpecification(Func<ReportDTO, bool> expression)
        {
            _expression = expression;
        }

        public bool IsStatisfiedBy(ReportDTO report)
        {
            return _expression(report);
        }
    }
}
