using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// MySql database manager.
    /// </summary>
    /// <seealso cref="IDbManager" />
    public sealed class MySqlDbManager : IDbManager
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDbManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentNullException">connectionString</exception>
        public MySqlDbManager(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="name">The name.</param>
        /// <returns>The connection string.</returns>
        public static string GetConnectionString(string template, string name)
        {
            return Regex.Replace(template,
                    "Database=[^;]+;", $"Database={name};",
                    RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Executes the specified set of commands against the database.
        /// </summary>
        /// <param name="database">The database or null to use the default.</param>
        /// <param name="commands">The SQL commands array.</param>
        public void ExecuteCommands(string database, params string[] commands)
        {
            using (MySqlConnection connection = new MySqlConnection(
                database == null ?
                _connectionString :
                GetConnectionString(_connectionString, database)))
            {
                connection.Open();
                foreach (string command in commands.Where(
                    c => !string.IsNullOrWhiteSpace(c)))
                {
                    foreach (string single in Regex.Split(command, @"[\r\n\s;]+"))
                    {
                        if (string.IsNullOrWhiteSpace(single)) continue;
                        MySqlCommand cmd = new MySqlCommand(single, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the specified database exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>true if exists, else false</returns>
        public bool Exists(string name)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA " +
                    $"WHERE SCHEMA_NAME = '{name}'", connection);
                object result = cmd.ExecuteScalar();
                connection.Close();
                return result != DBNull.Value;
            }
        }

        /// <summary>
        /// Removes the database with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public void RemoveDatabase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (Exists(name))
                ExecuteCommands(null, $"DROP DATABASE `{name}`");
        }

        /// <summary>
        /// Creates the database with the specified name, or clears it if it exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schema">The optional schema script to be executed only when the
        /// database is created.</param>
        /// <param name="seed">The optional seed script to be executed.</param>
        /// <param name="identityTablesToReset">if not null, a list of table names
        /// whose identity should be reset before executing the seed script.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public void CreateDatabase(string name, string schema, string seed,
            IList<string> identityTablesToReset = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (Exists(name))
            {
                ClearDatabase(name);
                ExecuteCommands(null, $"USE [{name}]", seed);
            }
            else
            {
                ExecuteCommands(null, $"CREATE DATABASE [{name}]",
                    $"USE [{name}]", schema, seed);
            }
        }

        private IList<string> GetTableNames(MySqlConnection connection, string name)
        {
            List<string> tables = new List<string>();
            MySqlCommand cmd = new MySqlCommand(
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                $"WHERE table_schema='{name}';", connection);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            return tables;
        }

        /// <summary>
        /// Clears the database by removing all the rows and resetting autonumber.
        /// </summary>
        public void ClearDatabase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            // https://stackoverflow.com/questions/1912813/truncate-all-tables-in-a-mysql-database-in-one-command
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand($"USE `{name}`", connection);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SET FOREIGN_KEY_CHECKS=0";
                cmd.ExecuteNonQuery();

                foreach (string table in GetTableNames(connection, name))
                {
                    cmd.CommandText = $"TRUNCATE TABLE `{table}`";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SET FOREIGN_KEY_CHECKS=1";
            }
        }
    }
}
