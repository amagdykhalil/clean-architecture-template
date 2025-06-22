using System.Reflection;

namespace SolutionName.UnitTests.SolutionNamehitectureTests
{
    public class SolutionNamehitectureTests
    {
        private const string APIAssemblyName = "SolutionName.API";
        private const string ApplicationAssemblyName = "SolutionName.Application";
        private const string DomainAssemblyName = "SolutionName.Domain";
        private const string InfrastructureAssemblyName = "SolutionName.Infrastructure";
        private const string PersistenceAssemblyName = "SolutionName.Persistence";

        private static Assembly GetAssembly(string assemblyName) =>
            Assembly.Load(assemblyName);

        private void AssertNoDependency(string sourceLayer, string targetLayer)
        {
            var sourceAssembly = GetAssembly(sourceLayer);
            var references = sourceAssembly.GetReferencedAssemblies().Select(a => a.Name);
            Assert.DoesNotContain(targetLayer, references);
        }

        private void AssertHasDependency(string sourceLayer, string targetLayer)
        {
            var sourceAssembly = GetAssembly(sourceLayer);
            var references = sourceAssembly.GetReferencedAssemblies().Select(a => a.Name);
            Assert.Contains(targetLayer, references);
        }

        // --- API Layer ---

        [Theory]
        [InlineData(ApplicationAssemblyName)]
        [InlineData(PersistenceAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        public void API_ShouldDependOn(string assemblyName)
        {
            AssertHasDependency(APIAssemblyName, assemblyName);
        }

        // --- Application Layer ---

        [Theory]
        [InlineData(DomainAssemblyName)]
        public void Application_ShouldDependOn(string assemblyName)
        {
            AssertHasDependency(ApplicationAssemblyName, assemblyName);
        }

        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        [InlineData(PersistenceAssemblyName)]
        public void Application_ShouldNotDependOn(string assemblyName)
        {
            AssertNoDependency(ApplicationAssemblyName, assemblyName);
        }

        // --- Infrastructure Layer ---

        [Theory]
        [InlineData(ApplicationAssemblyName)]
        [InlineData(DomainAssemblyName)]
        public void Infrastructure_ShouldDependOn(string assemblyName)
        {
            AssertHasDependency(InfrastructureAssemblyName, assemblyName);
        }

        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(PersistenceAssemblyName)]
        public void Infrastructure_ShouldNotDependOn(string assemblyName)
        {
            AssertNoDependency(InfrastructureAssemblyName, assemblyName);
        }

        // --- Domain Layer ---

        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        [InlineData(PersistenceAssemblyName)]
        [InlineData(ApplicationAssemblyName)]
        public void Domain_ShouldNotDependOn(string assemblyName)
        {
            AssertNoDependency(DomainAssemblyName, assemblyName);
        }

        // --- Persistence Layer ---

        [Theory]
        [InlineData(ApplicationAssemblyName)]
        [InlineData(DomainAssemblyName)]
        public void Persistence_ShouldDependOn(string assemblyName)
        {
            AssertHasDependency(PersistenceAssemblyName, assemblyName);
        }

        [Theory]
        [InlineData(APIAssemblyName)]
        [InlineData(InfrastructureAssemblyName)]
        public void Persistence_ShouldNotDependOn(string assemblyName)
        {
            AssertNoDependency(PersistenceAssemblyName, assemblyName);
        }
    }
}
