using System.Reflection;

namespace SolutionName.Tests.Common.Layers
{
    /// <summary>
    /// Base class for architecture tests that verify layer dependencies and assembly references.
    /// </summary>
    public abstract class ArchitectureTestsBase
    {
        protected const string APIAssemblyName = "SolutionName.API";
        protected const string ApplicationAssemblyName = "SolutionName.Application";
        protected const string DomainAssemblyName = "SolutionName.Domain";
        protected const string InfrastructureAssemblyName = "SolutionName.Infrastructure";
        protected const string PersistenceAssemblyName = "SolutionName.Persistence";

        protected static Assembly GetAssembly(string layerName) =>
               layerName switch
               {
                   APIAssemblyName => Assembly.Load(APIAssemblyName),
                   ApplicationAssemblyName => Assembly.Load(ApplicationAssemblyName),
                   DomainAssemblyName => Assembly.Load(DomainAssemblyName),
                   InfrastructureAssemblyName => Assembly.Load(InfrastructureAssemblyName),
                   PersistenceAssemblyName => Assembly.Load(PersistenceAssemblyName),
                   _ => throw new ArgumentException($"Invalid assembly name: {layerName}")
               };

        protected void AssertNoDependency(string sourceLayer, string targetLayer)
        {
            var sourceAsm = GetAssembly(sourceLayer);
            var referenced = sourceAsm.GetReferencedAssemblies().Select(a => a.Name);
            Assert.DoesNotContain(targetLayer, referenced);
        }

        protected void AssertHasDependency(string sourceLayer, string targetLayer)
        {
            var sourceAsm = GetAssembly(sourceLayer);
            var referenced = sourceAsm.GetReferencedAssemblies().Select(a => a.Name);
            Assert.Contains(targetLayer, referenced);
        }
    }
}


