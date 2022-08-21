using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;

namespace Cadmus.Mongo
{
    /// <summary>
    /// A base class for Mongo consumer code.
    /// </summary>
    public abstract class MongoConsumerBase
    {
        private string _currentConnString;

        /// <summary>
        /// The Mongo client. This gets created by <see cref="EnsureClientCreated(string)"/>
        /// and cached until the received connection string changes.
        /// </summary>
        protected MongoClient Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConsumerBase"/> class.
        /// </summary>
        protected MongoConsumerBase()
        {
            // camel case everything:
            // https://stackoverflow.com/questions/19521626/mongodb-convention-packs/19521784#19521784
            ConventionPack pack = new()
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("camel case", pack, _ => true);
        }

        /// <summary>
        /// Gets the name of the database from the specified source.
        /// </summary>
        /// <param name="source">The source (connection string).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">source</exception>
        protected string GetDatabaseName(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new MongoUrl(source).DatabaseName;
        }

        /// <summary>
        /// Ensures that <see cref="Client"/> is created for the specified
        /// source.
        /// </summary>
        /// <param name="source">The source (connection string).</param>
        /// <exception cref="ArgumentNullException">source</exception>
        protected void EnsureClientCreated(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (Client != null && _currentConnString == source) return;

            Client = new MongoClient(source);
            _currentConnString = source;
        }
    }
}
