﻿using Cadmus.Core;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Base class for item index readers.
    /// </summary>
    /// <seealso cref="IItemIndexReader" />
    public abstract class SqlItemIndexReaderBase
    {
        private string _connectionString;
        private ISqlQueryBuilder _queryBuilder;

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        protected string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (_connectionString == value) return;
                _connectionString = value;
                Connection?.Close();
                Connection = null;
            }
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected DbConnection Connection { get; private set; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>Connection.</returns>
        protected abstract DbConnection GetConnection();

        /// <summary>
        /// Gets a new command object.
        /// </summary>
        /// <returns>Command.</returns>
        protected abstract DbCommand GetCommand();

        /// <summary>
        /// Gets the SQL query builder.
        /// </summary>
        /// <returns>SQL query builder.</returns>
        protected abstract ISqlQueryBuilder GetQueryBuilder();

        /// <summary>
        /// Searches for items in the index with the specified query.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>Page of results.</returns>
        /// <exception cref="ArgumentNullException">query or options</exception>
        public DataPage<ItemInfo> SearchItems(string query, PagingOptions options)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (_queryBuilder == null) _queryBuilder = GetQueryBuilder();
            if (_queryBuilder == null) return null;

            var pageAndTot = _queryBuilder.BuildForItem(query, options);

            if (Connection == null)
            {
                Connection = GetConnection();
                Connection.Open();
            }

            DbCommand pageCommand = GetCommand();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            pageCommand.CommandText = pageAndTot.Item1;
            pageCommand.Connection = Connection;

            DbCommand totCommand = GetCommand();
            totCommand.CommandText = pageAndTot.Item2;
            totCommand.Connection = Connection;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            List<ItemInfo> items = new List<ItemInfo>();

            int total = Convert.ToInt32(totCommand.ExecuteScalar());
            if (total == 0)
            {
                return new DataPage<ItemInfo>(
                    options.PageNumber,
                    options.PageSize,
                    0,
                    items);
            }

            using (DbDataReader reader = pageCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    ItemInfo item = new ItemInfo
                    {
                        // for some reason, the type from MySql is GUID here
                        Id = reader.GetValue(reader.GetOrdinal("id")).ToString(),
                        //Id = reader.GetFieldValue<string>(
                        //    reader.GetOrdinal("id")),
                        Title = reader.GetFieldValue<string>(
                            reader.GetOrdinal("title")),
                        Description = reader.GetFieldValue<string>(
                            reader.GetOrdinal("description")),
                        FacetId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("facetId")),
                        GroupId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("groupId")),
                        SortKey = reader.GetFieldValue<string>(
                            reader.GetOrdinal("sortKey")),
                        Flags = reader.GetFieldValue<int>(
                            reader.GetOrdinal("flags")),
                        TimeCreated = reader.GetFieldValue<DateTime>(
                            reader.GetOrdinal("timeCreated")),
                        CreatorId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("creatorId")),
                        TimeModified = reader.GetFieldValue<DateTime>(
                            reader.GetOrdinal("timeModified")),
                        UserId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("userId"))
                    };
                    items.Add(item);
                }
            }
            return new DataPage<ItemInfo>(
                options.PageNumber,
                options.PageSize,
                total, items);
        }

        /// <summary>
        /// Searches for pins in the index with the specified query.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>Page of results.</returns>
        /// <exception cref="ArgumentNullException">query or options</exception>
        public DataPage<DataPinInfo> SearchPins(string query, PagingOptions options)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (_queryBuilder == null) _queryBuilder = GetQueryBuilder();
            if (_queryBuilder == null) return null;

            var pageAndTot = _queryBuilder.BuildForPin(query, options);

            if (Connection == null)
            {
                Connection = GetConnection();
                Connection.Open();
            }

            DbCommand pageCommand = GetCommand();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            pageCommand.CommandText = pageAndTot.Item1;
            pageCommand.Connection = Connection;

            DbCommand totCommand = GetCommand();
            totCommand.CommandText = pageAndTot.Item2;
            totCommand.Connection = Connection;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            List<DataPinInfo> pins = new List<DataPinInfo>();

            int total = Convert.ToInt32(totCommand.ExecuteScalar());
            if (total == 0)
            {
                return new DataPage<DataPinInfo>(
                    options.PageNumber,
                    options.PageSize,
                    0,
                    pins);
            }

            using (DbDataReader reader = pageCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    DataPinInfo pin = new DataPinInfo
                    {
                        ItemId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("itemId")),
                        PartId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("partId")),
                        PartTypeId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("partTypeId")),
                        RoleId = reader.GetFieldValue<string>(
                            reader.GetOrdinal("roleId")),
                        Name = reader.GetFieldValue<string>(
                            reader.GetOrdinal("name")),
                        Value = reader.GetFieldValue<string>(
                            reader.GetOrdinal("value"))
                    };
                    pins.Add(pin);
                }
            }
            return new DataPage<DataPinInfo>(
                options.PageNumber,
                options.PageSize,
                total, pins);
        }

        /// <summary>
        /// Closes the connection to the target database.
        /// </summary>
        public void Close()
        {
            Connection?.Close();
        }
    }
}
