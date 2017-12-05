using iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BusinessLogicLayer
{
    public class CustomSolution : CustomSolutionFactory
    {
        private Solution _solution;

        public Solution Solution { get { return _solution; } set { _solution = value; } }

        public IEnumerable<Project> Projects { get { return _solution.Projects; } }

        public CustomSolution(Solution solution)
        {
            _solution = solution;
        }
    }
}
