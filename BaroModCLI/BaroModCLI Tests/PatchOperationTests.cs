using System.Xml.Linq;
using BaroModCLI.PatchOperations;
using Xunit;

namespace BaroModCLI_Tests
{
    public class PatchOperationTests
    {
        [Theory]
        [InlineData("TestData/Add/Output.xml", "TestData/Add/Diff.xml")]
        [InlineData("TestData/Add/Attribute/Output.xml", "TestData/Add/Attribute/Diff.xml")]
        [InlineData("TestData/Add/Attribute/Cleanup/Output.xml", "TestData/Add/Attribute/Cleanup/Diff.xml")]
        [InlineData("TestData/Remove/Output.xml", "TestData/Remove/Diff.xml")]
        [InlineData("TestData/Edit/Output.xml", "TestData/Edit/Diff.xml")]
        [InlineData("TestData/All/Output.xml", "TestData/All/Diff.xml")]
        [InlineData("TestData/Edit/Override/Local/Output.xml",
                    "TestData/Edit/Override/Local/Diff.xml")]
        [InlineData("TestData/Edit/Override/All/Output.xml",
                    "TestData/Edit/Override/All/Diff.xml")]
        [InlineData("TestData/Edit/Override/Local/Multiple/Output.xml",
                    "TestData/Edit/Override/Local/Multiple/Diff.xml")]
        [InlineData("TestData/Edit/Override/Root/Output.xml",
                    "TestData/Edit/Override/Root/Diff.xml")]
        [InlineData("TestData/Remove/Cleanup/Output.xml",
                    "TestData/Remove/Cleanup/Diff.xml")]
        [InlineData("TestData/Remove/Cleanup/SecondLevel/Output.xml",
                    "TestData/Remove/Cleanup/SecondLevel/Diff.xml")]
        [InlineData("TestData/Remove/Multiple/Output.xml",
                    "TestData/Remove/Multiple/Diff.xml")]
        public void TestApplyAll(string outputPath, string diffPath)
        {
            XDocument outputData = XDocument.Load(outputPath);
            XDocument diff = XDocument.Load(diffPath);

            XDocument testData = ApplyPatchOperation.ApplyAll(diff);

            Assert.True(XNode.DeepEquals(outputData.Root, testData.Root),
                        $"{nameof(outputPath)}\n\n{outputData}\n\ndoes not match file produced by {nameof(diffPath)}\n\n{testData}");
        }
    }
}
