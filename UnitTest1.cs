using System.IO;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Xunit;
using Xunit.Abstractions;

namespace OpenApiInspector
{
    
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;
        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void RouteSegmentsMustBePlural()
        {
            var ruleSet = new OpenApiValidationRuleSet();
            ruleSet.Add(OpenApiPathRules.RouteSegmentsMustBePlural);
            var inspector = new OpenApiInspector(ruleSet);
            var walker = new OpenApiWalker(inspector);
            OpenApiDiagnostic diagnostic;
            var document = new OpenApiStreamReader().Read(File.OpenRead(".\\petstore.yaml"), out diagnostic);
            walker.Walk(document);
            foreach (var error in inspector.Errors)
            {
                output.WriteLine(error.ToString());
            }
            
        }

        [Fact]
        public void PropertiesMustBePascalCased()
        {
            var ruleSet = new OpenApiValidationRuleSet();
            ruleSet.Add(OpenApiPropertyRules.PropertiesMustBePascalCased);
            var inspector = new OpenApiInspector(ruleSet);
            var walker = new OpenApiWalker(inspector);
            OpenApiDiagnostic diagnostic;
            var document = new OpenApiStreamReader().Read(File.OpenRead(".\\petstore.yaml"), out diagnostic);
            walker.Walk(document);
            foreach (var error in inspector.Errors)
            {
                output.WriteLine(error.ToString());
            }
            
        }
    }

}
