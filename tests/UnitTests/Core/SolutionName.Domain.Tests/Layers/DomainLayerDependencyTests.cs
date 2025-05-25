using SolutionName.Tests.Common.Layers;

namespace SolutionName.Domain.Tests.Layers
{
    public class DomainLayerDependencyTests : ArchitectureTestsBase
    {
        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        [InlineData(PersistenceAssemblyName)]
        [InlineData(ApplicationAssemblyName)]
        public void Domain_ShouldNotDependOn(string AssemblyName)
        {
            AssertNoDependency(DomainAssemblyName, AssemblyName);
        }

    }
}

