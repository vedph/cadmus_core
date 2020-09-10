using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cadmus.Core
{
    /// <summary>
    /// Helper class used when building <see cref="DataPin"/>'s from lists.
    /// </summary>
    /// <remarks>Often, when creating data pins from parts having lists of
    /// objects, it is useful to create pins which count the count of each
    /// value found in the list. For instance, say you have a list of objects
    /// with a <c>Tag</c> string property; your list has 4 objects with
    /// tag <c>alpha</c>, <c>beta</c>, null, and <c>beta</c>. You might want
    /// to build pins named like <c>tag-alpha-count</c>=1, <c>tag-beta-count</c>=2,
    /// and <c>tag-tot-count</c>=4, which provide the count for each value
    /// of the tag property (except for null), plus the total count of the
    /// objects. This helper class can be used for this purposes, thus reducing
    /// the amount of code required for adding such counts.</remarks>
    public sealed class DataPinBuilder
    {
        private readonly Dictionary<string, int> _counts;
        private readonly Dictionary<string, HashSet<string>> _values;
        private string _totKey;

        /// <summary>
        /// Gets or sets key to be used for naming the total counts.
        /// The default value is <c>tot</c>; this value can be empy but
        /// not null.
        /// </summary>
        public string TotalKey {
            get { return _totKey; }
            set { _totKey = value ?? ""; }
        }

        /// <summary>
        /// Gets or sets the suffix to be appended to keys representing counts.
        /// The default value is <c>-count</c>.
        /// </summary>
        public string CountKeySuffix { get; set; }

        /// <summary>
        /// Gets the optional filter used in adding text values.
        /// </summary>
        public IDataPinTextFilter Filter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPinBuilder"/> class.
        /// </summary>
        /// <param name="filter">The optional text filter to be used on request.
        /// </param>
        public DataPinBuilder(IDataPinTextFilter filter = null)
        {
            Filter = filter;
            _counts = new Dictionary<string, int>();
            _values = new Dictionary<string, HashSet<string>>();
            _totKey = "tot";
            CountKeySuffix = "-count";
        }

        /// <summary>
        /// Resets all the counts and values.
        /// </summary>
        public void Reset()
        {
            _counts.Clear();
            _values.Clear();
        }

        #region Set
        /// <summary>
        /// Sets the count value for the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no count will be set.</param>
        /// <param name="value">The value.</param>
        /// <param name="includeTotal">True to also set the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Set(string key, int value, bool includeTotal = true,
            string prefix = null)
        {
            if (includeTotal)
            {
                string k = (prefix ?? "") + _totKey + (CountKeySuffix ?? "");
                if (_counts.ContainsKey(k)) _counts[k] += value;
                else _counts[k] = value;
            }

            if (key == null) return;

            _counts[(prefix ?? "")
                + key
                + (CountKeySuffix ?? "")] = value;
        }

        /// <summary>
        /// Sets the count value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="includeTotal">True to also set the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Set(bool key, int value, bool includeTotal = true,
            string prefix = null)
            => Set(key ? "1" : "0", value, includeTotal, prefix);

        /// <summary>
        /// Sets the count value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="includeTotal">True to also set the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Set(int key, int value, bool includeTotal = true,
            string prefix = null)
            => Set(key.ToString(CultureInfo.InvariantCulture), value,
                includeTotal, prefix);

        /// <summary>
        /// Sets the count value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="includeTotal">True to also set the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Set(double key, int value, bool includeTotal = true,
            string prefix = null)
            => Set(key.ToString(CultureInfo.InvariantCulture), value,
                includeTotal, prefix);
        #endregion

        #region Increase
        private void IncreaseTotal(string prefix)
        {
            string k = (prefix ?? "") + _totKey + (CountKeySuffix ?? "");
            if (!_counts.ContainsKey(k)) _counts[k] = 1;
            else _counts[k]++;
        }

        /// <summary>
        /// Increases the count for the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no count will be increased.</param>
        /// <param name="includeTotal">True to also increase the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Increase(string key, bool includeTotal = true,
            string prefix = null)
        {
            if (includeTotal) IncreaseTotal(prefix);

            if (key == null) return;

            string k = (prefix ?? "") + key + (CountKeySuffix ?? "");
            if (!_counts.ContainsKey(k)) _counts[k] = 1;
            else _counts[k]++;
        }

        /// <summary>
        /// Increases the count for the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no count will be changed.</param>
        /// <param name="includeTotal">True to also increase the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Increase(bool key, bool includeTotal = true,
            string prefix = null)
            => Increase(key ? "1" : "0", includeTotal, prefix);

        /// <summary>
        /// Increases the count for the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no count will be changed.</param>
        /// <param name="includeTotal">True to also increase the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Increase(int key, bool includeTotal = true,
            string prefix = null)
            => Increase(key.ToString(CultureInfo.InvariantCulture),
                includeTotal, prefix);

        /// <summary>
        /// Increases the count for the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no count will be changed.</param>
        /// <param name="includeTotal">True to also increase the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        public void Increase(double key, bool includeTotal = true,
            string prefix = null)
            => Increase(key.ToString(CultureInfo.InvariantCulture),
                includeTotal, prefix);
        #endregion

        #region Update
        /// <summary>
        /// Updates the counts for all the received <paramref name="keys"/>,
        /// prefixing and suffixing each of them as requested.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="includeTotal">True to also update the count under
        /// the corresponding total key.</param>
        /// <param name="prefix">The optional key prefix.</param>
        /// <exception cref="ArgumentNullException">keys</exception>
        public void Update(IEnumerable<string> keys, bool includeTotal = true,
            string prefix = null)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            int total = 0;
            foreach (string key in keys)
            {
                if (key != null) Increase(key, includeTotal, prefix);
                else if (includeTotal) IncreaseTotal(prefix);
                total++;
            }
        }
        #endregion

        #region AddValue
        /// <summary>
        /// Adds the specified pin value.
        /// </summary>
        /// <param name="key">The key. If null, no value will be added.</param>
        /// <param name="value">The pin value. Nothing is added if value is null.
        /// <param name="prefix">The optional key prefix.</param>
        /// <param name="filter">True to apply filtering.</param>
        /// <param name="filterOptions">The options for the filter.</param>
        /// </param>
        public void AddValue(string key, string value, string prefix = null,
            bool filter = false, object filterOptions = null)
        {
            if (key == null || value == null) return;

            string k = (prefix ?? "") + key;
            if (!_values.ContainsKey(k)) _values[k] = new HashSet<string>();

            _values[k].Add(filter && Filter != null
                ? Filter.Apply(value, filterOptions) : value);
        }

        /// <summary>
        /// Adds the specified pin value.
        /// </summary>
        /// <param name="key">The key. If null, no value will be added.</param>
        /// <param name="value">The pin value.</param>
        /// <param name="prefix">The optional key prefix.</param>
        /// <param name="filter">True to apply filtering.</param>
        /// <param name="filterOptions">The options for the filter.</param>
        public void AddValue(string key, bool value, string prefix = null,
            bool filter = false, object filterOptions = null)
            => AddValue(key, value ? "1" : "0", prefix, filter, filterOptions);

        /// <summary>
        /// Adds the specified pin value.
        /// </summary>
        /// <param name="key">The key. If null, no value will be added.</param>
        /// <param name="value">The pin value.</param>
        /// <param name="prefix">The optional key prefix.</param>
        /// <param name="filter">True to apply filtering.</param>
        /// <param name="filterOptions">The options for the filter.</param>
        public void AddValue(string key, int value, string prefix = null,
            bool filter = false, object filterOptions = null)
            => AddValue(key, value.ToString(CultureInfo.InvariantCulture),
                prefix, filter, filterOptions);

        /// <summary>
        /// Adds the specified pin value.
        /// </summary>
        /// <param name="key">The key. If null, no value will be added.</param>
        /// <param name="value">The pin value.</param>
        /// <param name="prefix">The optional key prefix.</param>
        /// <param name="filter">True to apply filtering.</param>
        /// <param name="filterOptions">The options for the filter.</param>
        public void AddValue(string key, double value, string prefix = null,
            bool filter = false, object filterOptions = null)
            => AddValue(key, value.ToString(CultureInfo.InvariantCulture),
                prefix, filter, filterOptions);

        /// <summary>
        /// Adds all the values under the specified key.
        /// </summary>
        /// <param name="key">The key. If null, no value will be added.</param>
        /// <param name="values">The values.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="filter">True to apply filtering.</param>
        /// <param name="filterOptions">The options for the filter.</param>
        public void AddValues(string key, IEnumerable<string> values,
            string prefix = null, bool filter = false, object filterOptions = null)
        {
            if (key == null) return;
            foreach (string value in values) AddValue(key, value, prefix, filter,
                filterOptions);
        }
        #endregion

        /// <summary>
        /// Applies or not the filter to the specified array of objects,
        /// including a mix of booleans and strings. Every occurrence of the
        /// boolean value toggles filtering; every string is appended,
        /// eventually filtered if filtering is on at that position.
        /// </summary>
        /// <remarks>You can use this function when you want to build a string
        /// where some portions should be filtered using <see cref="Filter"/>,
        /// and some others not.</remarks>
        /// <param name="options">The options for the filter.</param>
        /// <param name="filtersAndValues">The filters and values array.</param>
        /// <returns>The resulting string.</returns>
        public string ApplyFilter(object options, params object[] filtersAndValues)
        {
            StringBuilder sb = new StringBuilder();
            bool on = false;

            for (int i = 0; i < filtersAndValues.Length; i++)
            {
                if (filtersAndValues[i] is bool onOff)
                {
                    on = onOff;
                }
                else
                {
                    string value = Convert.ToString(filtersAndValues[i]) ?? "";
                    sb.Append(on && Filter != null
                        ? Filter.Apply(value, options) : value);
                }
            }

            return sb.ToString();
        }

        private DataPin CreateDataPin(IPart part, string name, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new DataPin
            {
                ItemId = part?.ItemId,
                PartId = part?.Id,
                RoleId = part?.RoleId,
                Name = name,
                Value = value
            };
        }

        private HashSet<string> AdjustZeroKeys(string[] zeroKeys)
        {
            HashSet<string> keys = new HashSet<string>();

            foreach (string key in zeroKeys)
            {
                if (!string.IsNullOrEmpty(CountKeySuffix)
                    && !key.EndsWith(CountKeySuffix, StringComparison.Ordinal))
                {
                    keys.Add(key + CountKeySuffix);
                }
                else keys.Add(key);
            }

            return keys;
        }

        /// <summary>
        /// Builds the pins from the current data.
        /// </summary>
        /// <param name="part">The part the pins refer to, or null
        /// (for fragments).</param>
        /// <param name="zeroKeys">An optional array of count keys which should
        /// be emitted for counts even they are equal to 0, and thus eventually
        /// not present in this builder.</param>
        /// <returns>Pins.</returns>
        public List<DataPin> Build(IPart part, params string[] zeroKeys)
        {
            List<DataPin> pins = new List<DataPin>();

            // counts
            foreach (string key in _counts.Keys)
            {
                pins.Add(CreateDataPin(
                    part, key,
                    _counts[key].ToString(CultureInfo.InvariantCulture)));
            }

            // zero-counts
            if (zeroKeys.Length > 0)
            {
                HashSet<string> zk = AdjustZeroKeys(zeroKeys);
                foreach (string key in zk.Except(_counts.Keys))
                    pins.Add(CreateDataPin(part, key, "0"));
            }

            // values
            foreach (string key in _values.Keys)
            {
                foreach (string value in _values[key])
                    pins.Add(CreateDataPin(part, key, value));
            }

            return pins;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Join(" ", from kvp in _counts
                                    orderby kvp.Key
                                    select $"{kvp.Key}={kvp.Value}");
        }
    }
}
