using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// SQL server database manager.
    /// </summary>
    /// <seealso cref="IDbManager" />
    public sealed class MsSqlDbManager : IDbManager
    {
        private readonly string _csTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDbManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string with placeholder
        /// <c>{0}</c> for the database name.</param>
        /// <exception cref="ArgumentNullException">connectionString</exception>
        public MsSqlDbManager(string connectionString)
        {
            _csTemplate = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Gets the connection string to the database with the specified
        /// name from the specified template, where the database name placeholder
        /// is represented by the string <c>{0}</c>.
        /// </summary>
        /// <param name="template">The connection string template with placeholder
        /// at the database name value.</param>
        /// <param name="name">The database name, or null or empty to avoid
        /// setting the database name at all in the connection string.</param>
        /// <returns>The connection string.</returns>
        public static string GetConnectionString(string template, string name)
        {
            Regex dbRegex = new Regex("Database=[^;]+;", RegexOptions.IgnoreCase);

            return string.IsNullOrEmpty(name)
                ? dbRegex.Replace(template, "")
                : string.Format(template, name);
        }

        /// <summary>
        /// Executes the specified set of commands against the database.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <param name="commands">The SQL commands array.</param>
        /// <exception cref="ArgumentNullException">database</exception>
        public void ExecuteCommands(string database, params string[] commands)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            using (SqlConnection connection = new SqlConnection(
                GetConnectionString(_csTemplate, database)))
            {
                connection.Open();
                foreach (string command in commands.Where(
                    c => !string.IsNullOrWhiteSpace(c)))
                {
                    foreach (string single in Regex.Split(command, @"[\r\n;]+GO\b"))
                    {
                        if (string.IsNullOrWhiteSpace(single)) continue;
                        SqlCommand cmd = new SqlCommand(single, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the specified database exists.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <returns>true if exists, else false</returns>
        public bool Exists(string database)
        {
            using (SqlConnection connection = new SqlConnection(
                GetConnectionString(_csTemplate, "master")))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand($"SELECT DB_ID('{database}')", connection);
                object result = cmd.ExecuteScalar();
                connection.Close();
                return result != DBNull.Value;
            }
        }

        /// <summary>
        /// Removes the database with the specified name.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <exception cref="ArgumentNullException">database</exception>
        public void RemoveDatabase(string database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            if (Exists(database))
                ExecuteCommands(null, $"DROP DATABASE [{database}]");
        }

        /// <summary>
        /// Creates the database with the specified name.
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

            ExecuteCommands("master",
                $"CREATE DATABASE [{name}]", $"USE [{name}]",
                schema, seed);
        }

        /// <summary>
        /// Clears the database by removing all the rows and resetting autonumber.
        /// </summary>
        /// <exception cref="ArgumentNullException">database</exception>
        public void ClearDatabase(string database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            // https://stackoverflow.com/questions/155246/how-do-you-truncate-all-tables-in-a-database-using-tsql
            ExecuteCommands(database,
                $"USE [{database}]",
                "EXEC sp_MSForEachTable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"\n" +
                "EXEC sp_MSForEachTable \"DELETE FROM ?\"\n" +
                "EXEC sp_MSForEachTable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
        }
    }
}
