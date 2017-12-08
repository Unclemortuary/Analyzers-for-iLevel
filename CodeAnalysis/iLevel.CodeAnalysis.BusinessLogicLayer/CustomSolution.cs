using System.Linq;
using iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BusinessLogicLayer
{
    public class CustomSolution
    {
        private Solution _solution;

        public Solution Solution { get { return _solution; } set { _solution = value; } }

        public virtual IEnumerable<Project> Projects { get { return _solution.Projects; } }

        public ProjectId ProjectId { get { return _solution.Projects.First().Id; } }

        public CustomSolution(Solution solution)
        {
            _solution = solution;
        }
    }
}
