using Cadmus.Graph;
using Fusi.Tools.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Cadmus.Index.Config;

/// <summary>
/// A factory for <see cref="IItemIndexWriter"/>'s and
/// <see cref="IItemIndexReader"/>'s. This factory relies on
/// a configuration rooted at the <c>index</c> section, with
/// properties <c>writer</c> and <c>reader</c>, both including an
/// objects with an <c>id</c> and an eventual <c>options</c> object.
/// </summary>
public sealed class ItemIndexFactory : ComponentFactory
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
    public ItemIndexFactory(IHost host, string connectionString) : base(host)
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
        ArgumentNullException.ThrowIfNull(services);

        // https://simpleinjector.readthedocs.io/en/latest/advanced.html?highlight=batch#batch-registration
        Assembly[] assemblies = additionalAssemblies ?? Array.Empty<Assembly>();

        foreach (Type it in new[]
        {
            typeof(IItemIndexWriter),
            typeof(IItemIndexReader),
            // TODO: remove with GetGraphRepository
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
    /// Gets the item index writer if any.
    /// </summary>
    /// <param name="graphSql">The optional SQL code to seed the index
    /// database with preset data for the graph.</param>
    /// <returns>Item index writer or null.</returns>
    public IItemIndexWriter? GetItemIndexWriter(string? graphSql = null)
    {
        IItemIndexWriter? writer = GetComponent<IItemIndexWriter>(
            "index:writer", false);

        if (writer != null) writer.InitContext = graphSql;
        return writer;
    }

    /// <summary>
    /// Gets the item index reader if any.
    /// </summary>
    /// <returns>Item index reader or null.</returns>
    public IItemIndexReader? GetItemIndexReader() =>
        GetComponent<IItemIndexReader>("index:reader", false);

    /// <summary>
    /// Gets the graph repository if any.
    /// </summary>
    /// <returns>Graph repository or null.</returns>
    [Obsolete("Use ItemGraphFactory for GetGraphRepository")]
    public IGraphRepository? GetGraphRepository() =>
        GetComponent<IGraphRepository>("graph:repository", false);
}
