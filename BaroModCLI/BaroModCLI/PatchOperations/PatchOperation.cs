using System;
using System.Xml.Linq;

namespace BaroModCLI.PatchOperations
{
    public abstract class PatchOperation : IDisposable
    {
        protected readonly XDocument Document;
        protected readonly XElement Patch;

        protected PatchOperation(XElement patch, XDocument document)
        {
            Patch = patch;
            Document = document;
        }

        public void Dispose()
        {
        }

        public abstract XDocument Apply();
    }
}
