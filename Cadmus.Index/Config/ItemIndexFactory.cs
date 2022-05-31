using Cadmus.Graph;
using Fusi.Tools.Config;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System;
using System.Reflection;

namespace Cadmus.Index.Config
{
    /// <summary>
    /// A factory for <see cref="IItemIndexWriter"/>'s and
    /// <see cref="IItemIndexReader"/>'s. This factory relies on
    /// a configuration rooted at the <c>index</c> section, with
    /// properties <c>writer</c> and <c>reader</c>, both including an
    /// objects with an <c>id</c> and an eventual <c>options</c> object.
    /// </summary>
    /// <seealso cref="ComponentFactoryBase" />
    public sealed class ItemIndexFactory : ComponentFactoryBase
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
        /// <param name="container">The container.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionString">The general connection string to
        /// supply to any component requiring an option named
        /// <see cref="CONNECTION_STRING_NAME"/> (=<c>ConnectionString</c>),
        /// when this option is not specified in its configuration.</param>
        public ItemIndexFactory(Container container,
            IConfiguration configuration,
            string connectionString) : base(container, configuration)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        private static object SupplyProperty(Type optionType,
            PropertyInfo property, object options, object defaultValue)
        {
            // if options have been loaded, supply if not specified
            if (options != null)
            {
                string value = (string)property.GetValue(options);
                if (string.IsNullOrEmpty(value))
                    property.SetValue(options, defaultValue);
            }
            // else create empty options and supply it
            else
            {
                options = Activator.CreateInstance(optionType);
                if (options != null)
                    property.SetValue(options, defaultValue);
            }

            return options;
        }

        /// <summary>
        /// Does the custom configuration.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="component">The component.</param>
        /// <param name="section">The section.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="optionType">Type of the option.</param>
        /// <returns>True if custom configuration logic applied.</returns>
        protected override bool DoCustomConfiguration<T>(T component,
            IConfigurationSection section, TypeInfo targetType, Type optionType)
        {
            // get the options if specified
            object options = section?.Get(optionType);

            // if we have a default connection AND the options type
            // has a ConnectionString property, see if we should supply a value
            // for it
            PropertyInfo property;
            if (_connectionString != null &&
                (property = optionType.GetProperty(CONNECTION_STRING_NAME)) != null)
            {
                options = SupplyProperty(optionType, property, options,
                    _connectionString);
            }

            // apply options if any
            if (options != null)
            {
                targetType.GetMethod("Configure").Invoke(component,
                    new[] { options });
            }

            return true;
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
        /// <param name="container">The container.</param>
        /// <param name="additionalAssemblies">The optional additional
        /// assemblies.</param>
        /// <exception cref="ArgumentNullException">container or part type
        /// provider</exception>
        public static void ConfigureServices(Container container,
            params Assembly[] additionalAssemblies)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            // https://simpleinjector.readthedocs.io/en/latest/advanced.html?highlight=batch#batch-registration
            Assembly[] assemblies = additionalAssemblies ?? Array.Empty<Assembly>();

            container.Collection.Register<IItemIndexWriter>(assemblies);
            container.Collection.Register<IItemIndexReader>(assemblies);
            container.Collection.Register<IGraphRepository>(assemblies);
        }

        /// <summary>
        /// Gets the item index writer if any.
        /// </summary>
        /// <param name="graphSql">The optional SQL code to seed the index
        /// database with preset data for the graph.</param>
        /// <returns>Item index writer or null.</returns>
        public IItemIndexWriter GetItemIndexWriter(string graphSql = null)
        {
            IItemIndexWriter writer = GetComponent<IItemIndexWriter>(
                Configuration["index:writer:id"],
                "index:writer:options",
                false);

            writer.InitContext = graphSql;
            return writer;
        }

        /// <summary>
        /// Gets the item index reader if any.
        /// </summary>
        /// <returns>Item index reader or null.</returns>
        public IItemIndexReader GetItemIndexReader()
        {
            return GetComponent<IItemIndexReader>(
                Configuration["index:reader:id"],
                "index:reader:options",
                false);
        }

        /// <summary>
        /// Gets the graph repository if any.
        /// </summary>
        /// <returns>Graph repository or null.</returns>
        public IGraphRepository GetGraphRepository()
        {
            return GetComponent<IGraphRepository>(
                Configuration["graph:repository:id"],
                "graph:repository:options",
                false);
        }
    }
}
