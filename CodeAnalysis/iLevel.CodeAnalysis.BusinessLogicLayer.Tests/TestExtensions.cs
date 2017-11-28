using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Extensions
{
    public static class TestExtensions
    {
        public static SourceText SourceText(this SourceText text)
        {
            return text;
        }
    }
}
