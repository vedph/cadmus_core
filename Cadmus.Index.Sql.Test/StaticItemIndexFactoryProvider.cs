using Cadmus.Index.Config;
using Fusi.Microsoft.Extensions.Configuration.InMemoryJson;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System;
using System.Reflection;

namespace Cadmus.Index.Sql.Test
{
    public sealed class StaticItemIndexFactoryProvider :
        IItemIndexFactoryProvider
    {
        private readonly string _connectionString;

        public StaticItemIndexFactoryProvider(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        public ItemIndexFactory GetFactory(string profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            // build the container for seeders
            Assembly[] indexAssemblies = new[]
            {
                // Cadmus.Index.Sql
                typeof(MySqlItemIndexWriter).Assembly
            };

            Container container = new Container();

            ItemIndexFactory.ConfigureServices(
                container,
                indexAssemblies);

            container.Verify();

            // load seed configuration
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddInMemoryJson(profile);
            var configuration = builder.Build();

            return new ItemIndexFactory(
                container,
                configuration,
                _connectionString);
        }
    }
}
