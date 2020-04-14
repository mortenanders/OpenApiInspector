using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Xunit;
using Xunit.Abstractions;

namespace OpenApiInspector
{

    public class UnitTest1
    {
        private readonly ITestOutputHelper output;
        private readonly OpenApiInspector inspector;
        private readonly OpenApiValidationRuleSet ruleSet;
        private readonly OpenApiDocument document;
        private readonly OpenApiWalker walker;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            output = testOutputHelper;
            ruleSet = new OpenApiValidationRuleSet();
            inspector = new OpenApiInspector(ruleSet);
            walker = new OpenApiWalker(inspector);
            OpenApiDiagnostic diagnostic;
            document = new OpenApiStreamReader().Read(File.OpenRead(".\\petstore.yaml"), out diagnostic);
        }
        [Fact]
        public void RouteRules()
        {
            ruleSet.Add(OpenApiPathRules.RouteSegmentsMustBePlural);
            ruleSet.Add(OpenApiPathRules.RouteParametersMustBePascalcased);
            ruleSet.Add(OpenApiPathRules.RoutesMustBeLowercased);
            ruleSet.Add(OpenApiPathRules.RoutesMustNotUseDelimiters);
            walker.Walk(document);
            DisplayErrors();

        }

        [Fact]
        public void PropertiesMustBePascalCased()
        {
            ruleSet.Add(OpenApiPropertyRules.PropertiesMustBePascalCased);
            walker.Walk(document);
            DisplayErrors();

        }

        [Fact]
        public void MustUseCollectionEnvelope()
        {
            ruleSet.Add(OpenApiResponseRules.MustUseCollectionEnvelope);
            walker.Walk(document);
            DisplayErrors();
        }

        public void DisplayErrors()
        {
            foreach (var error in inspector.Errors)
            {
                output.WriteLine(error.ToString());
            }
        }
    }

}
