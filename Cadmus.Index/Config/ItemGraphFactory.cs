using Cadmus.Graph;
using Fusi.Tools.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Cadmus.Index.Config;

/// <summary>
/// Item graph factory.
/// </summary>
/// <seealso cref="ComponentFactory" />
public sealed class ItemGraphFactory : ComponentFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// The name of the connection string property to be supplied
    /// in POCO option objects (<c>ConnectionString</c>).
    /// </summary>
    public const string CONNECTION_STRING_NAME = "ConnectionString";

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemIndexFactory"/>
    /// class.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="connectionString">The general connection string to
    /// supply to any component requiring an option named
    /// <see cref="CONNECTION_STRING_NAME"/> (=<c>ConnectionString</c>),
    /// when this option is not specified in its configuration.</param>
    public ItemGraphFactory(IHost host, string connectionString) : base(host)
    {
        _connectionString = connectionString ??
            throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Overrides the options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="section">The section.</param>
    protected override void OverrideOptions(object options,
          IConfigurationSection? section)
    {
        Type optionType = options.GetType();

        // if we have a default connection AND the options type
        // has a ConnectionString property, see if we should supply a value
        // for it
        PropertyInfo? property;
        if (_connectionString != null &&
            (property = optionType.GetProperty(CONNECTION_STRING_NAME)) != null)
        {
            // here we can safely discard the returned object as it will
            // be equal to the input options, which is not null
            SupplyProperty(optionType, property, options, _connectionString);
        }
    }

    /// <summary>
    /// Configures the container services to use components from
    /// the assemblies specified by <paramref name="additionalAssemblies"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="additionalAssemblies">The optional additional
    /// assemblies.</param>
    /// <exception cref="ArgumentNullException">container or part type
    /// provider</exception>
    public static void ConfigureServices(IServiceCollection services,
        params Assembly[] additionalAssemblies)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        Assembly[] assemblies = additionalAssemblies ?? Array.Empty<Assembly>();

        foreach (Type it in new[]
        {
            typeof(IGraphRepository),
        })
        {
            foreach (Type t in GetAssemblyConcreteTypes(assemblies, it))
            {
                services.AddTransient(it, t);
            }
        }
    }

    /// <summary>
    /// Gets the graph repository from <c>graph:repository</c> if any.
    /// </summary>
    /// <returns>Graph repository.</returns>
    public IGraphRepository? GetGraphRepository() =>
        GetComponent<IGraphRepository>("graph:repository", false);
}
