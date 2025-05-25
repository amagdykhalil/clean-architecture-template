using SolutionName.Tests.Common.Layers;

namespace SolutionName.Persistence.Tests.Layers
{
    public class PersistenceLayerDependencyTests : ArchitectureTestsBase
    {
        [Theory]
        [InlineData(ApplicationAssemblyName)]
        [InlineData(DomainAssemblyName)]
        public void Persistence_ShouldDependOn(string AssemblyName)
        {
            AssertHasDependency(PersistenceAssemblyName, AssemblyName);
        }

        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        public void Persistence_ShouldNotDependOn(string AssemblyName)
        {
            AssertNoDependency(PersistenceAssemblyName, AssemblyName);
        }

    }
}

