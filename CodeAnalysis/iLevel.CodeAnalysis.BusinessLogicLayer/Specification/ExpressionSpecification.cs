using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Specification
{
    class ExpressionSpecification : ISpecification
    {
        public bool IsStatisfiedBy(ReportDTO report)
        {
            return false;
        }
    }
}
