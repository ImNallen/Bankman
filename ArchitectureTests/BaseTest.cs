using System.Reflection;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Domain.IMarker).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(Application.IMarker).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.IMarker).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Api.IMarker).Assembly;
}
