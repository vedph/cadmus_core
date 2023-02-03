using Cadmus.Core.Storage;
using Fusi.Tools.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cadmus.Core.Config;

/// <summary>
/// A factory for <see cref="IItemBrowser"/>'s. This factory relies on
/// a configuration rooted at the <c>browsers</c> section, including an
/// array of item browsers objects, each with an <c>id</c> and an eventual
/// <c>options</c> object.
/// </summary>
public sealed class ItemBrowserFactory : ComponentFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// The name of the connection string property to be supplied
    /// in POCO option objects (<c>ConnectionString</c>).
    /// </summary>
    public const string CONNECTION_STRING_NAME = "ConnectionString";

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemBrowserFactory"/>
    /// class.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="connectionString">The general connection string to
    /// supply to any component requiring an option named
    /// <see cref="CONNECTION_STRING_NAME"/> (=<c>ConnectionString</c>),
    /// when this option is not specified in its configuration.</param>
    public ItemBrowserFactory(IHost host, string connectionString) : base(host)
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
    /// <c>Cadmus.Core</c> and <c>Cadmus.Seed</c>, plus the assemblies
    /// specified by <paramref name="additionalAssemblies"/>.
    /// This is just a helper method: at any rate, the configuration of
    /// the container is external to the VSM factory. You could use this
    /// method as a model and create your own, or call this method to
    /// register the components from these two assemblies, and then
    /// further configure the container, or add more assemblies when
    /// calling this via <paramref name="additionalAssemblies"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="partTypeProvider">The part type provider.</param>
    /// <param name="additionalAssemblies">The optional additional
    /// assemblies.</param>
    /// <exception cref="ArgumentNullException">container or part type
    /// provider</exception>
    public static void ConfigureServices(IServiceCollection services,
        IPartTypeProvider partTypeProvider,
        params Assembly[] additionalAssemblies)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (partTypeProvider is null)
            throw new ArgumentNullException(nameof(partTypeProvider));

        // https://simpleinjector.readthedocs.io/en/latest/advanced.html?highlight=batch#batch-registration
        Assembly[] assemblies = additionalAssemblies ?? Array.Empty<Assembly>();

        services.AddSingleton(partTypeProvider);

        foreach (Type it in new[]
        {
            typeof(IItemSortKeyBuilder),
            typeof(IItemBrowser),
        })
        {
            foreach (Type t in GetAssemblyConcreteTypes(assemblies, it))
            {
                services.AddTransient(it, t);
            }
        }
    }

    /// <summary>
    /// Gets the IDs of all the item browsers defined in the factory
    /// configuration.
    /// </summary>
    /// <returns>Array of IDs.</returns>
    public string[] GetItemBrowserIds()
    {
        IList<ComponentFactoryConfigEntry> entries =
            ComponentFactoryConfigEntry.ReadComponentEntries(
            Configuration, "browsers");

        return (from e in entries select e.Tag).ToArray();
    }

    /// <summary>
    /// Gets the optional item sort key builder from
    /// <c>seed/itemSortKeyBuilder</c>. If not specified, the
    /// <see cref="StandardItemSortKeyBuilder"/> will be used.
    /// </summary>
    /// <returns>Item sort key builder.</returns>
    public IItemBrowser? GetItemBrowser(string id)
    {
        IList<ComponentFactoryConfigEntry> entries =
            ComponentFactoryConfigEntry.ReadComponentEntries(
            Configuration, "browsers");
        ComponentFactoryConfigEntry? entry =
            entries.FirstOrDefault(e => e.Tag == id);
        if (entry == null) return null;

        int i = entries.IndexOf(entry);
        return GetComponent<IItemBrowser>(
            id,
            $"browsers:{i}:options",
            false);
    }
}
