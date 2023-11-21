using System;
using System.Data.Common;

namespace Cadmus.Index;

/// <summary>
/// Extensions for <see cref="DbDataReader"/>.
/// </summary>
public static class DbDataReaderExtensions
{
    /// <summary>
    /// Gets the value at <paramref name="columnIndex"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <returns>Value.</returns>
    public static T? GetValue<T>(this DbDataReader reader, int columnIndex)
    {
        return reader.IsDBNull(columnIndex)
            ? default : (T)reader.GetValue(columnIndex);
    }

    /// <summary>
    /// Gets the value at column named <paramref name="columnName"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>Value.</returns>
    /// <exception cref="ArgumentNullException">columnName</exception>
    public static T? GetValue<T>(this DbDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(columnName);

        int index = reader.GetOrdinal(columnName);
        return reader.IsDBNull(index) ? default : (T)reader.GetValue(index);
    }
}
