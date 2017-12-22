using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayerTests.Common
{
    public class CustomSourceText : SourceText
    {
        public CustomSourceText() { }

        public override char this[int position] => default(char);

        public override Encoding Encoding => default(Encoding);

        public override int Length => default(int);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { }
    }
}
