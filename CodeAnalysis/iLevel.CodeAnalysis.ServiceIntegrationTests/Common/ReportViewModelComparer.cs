using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CodeAnalysisService.Models;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    class ReportViewModelComparer : IEqualityComparer<ReportViewModel>
    {
        public bool Equals(ReportViewModel x, ReportViewModel y)
        {
            Type typeOfComparingObject = typeof(ReportViewModel);
            foreach (PropertyInfo propInfo in typeOfComparingObject.GetProperties())
            {
                var xVal = typeOfComparingObject.GetProperty(propInfo.Name).GetValue(x);
                var yVal = typeOfComparingObject.GetProperty(propInfo.Name).GetValue(y);
                if (!xVal.Equals(yVal))
                    return false;
            }
            return true;
        }

        public int GetHashCode(ReportViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
