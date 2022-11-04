using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Anonymous fragments layer part. This is used to manage fragments at
    /// their JSON level, by just reading their locations and eventually
    /// editing the JSON document representing them.
    /// </summary>
    /// <remarks>This class is used to handle in a generalized way all the
    /// fragments, independently from their type, for what concerns the
    /// layers reconciliation procedures, which only require to know the
    /// location of each fragment.</remarks>
    /// <seealso cref="PartBase" />
    public sealed class AnonLayerPart : PartBase
    {
        /// <summary>
        /// Gets or sets the fragments.
        /// </summary>
        public List<AnonFragment> Fragments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonLayerPart"/> class.
        /// </summary>
        public AnonLayerPart()
        {
            Fragments = new();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>No pins.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem? item = null)
        {
            return Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>No data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>();
        }

        /// <summary>
        /// Deletes all the non-range fragments at the integral location
        /// specified by the given Y,X coordinates. Fragments including this
        /// location but with a larger extent (ranges) are not deleted;
        /// fragments included by this location with a smaller extent (with
        /// at/run) are deleted.
        /// </summary>
        /// <param name="json">The JSON code representing the serialized
        /// layer part.</param>
        /// <param name="location">The location with Y and X coordinates.
        /// This must represent a single point, not a range.</param>
        /// <returns>The JSON code representing the serialized layer part,
        /// edited to remove the matching fragments.</returns>
        /// <exception cref="ArgumentNullException">json or location</exception>
        public string DeleteFragmentsAtIntegral(string json, string location)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            if (Fragments == null) return json;

            JObject doc = JObject.Parse(json);
            JArray frr = (JArray)doc["fragments"]!;

            TokenTextLocation refLoc = TokenTextLocation.Parse(location);

            for (int i = frr.Count - 1; i > -1; i--)
            {
                TokenTextLocation loc =
                    TokenTextLocation.Parse(Fragments[i].Location);

                if (!loc.IsRange && loc.A.Y == refLoc.A.Y && loc.A.X == refLoc.A.X)
                    frr.RemoveAt(i);
            }

            return doc.ToString(Formatting.None);
        }

        /// <summary>
        /// Applies the specified patch operations to this fragment.
        /// </summary>
        /// <param name="json">The JSON code used to instantiate this layer
        /// part.</param>
        /// <param name="patches">The patches operations. These can be
        /// either <c>del &lt;coords&gt;</c> or
        /// <c>mov &lt;oldCoords&gt; &lt;newCoords&gt;</c>.</param>
        /// <returns>The updated JSON code representing this layer part.</returns>
        /// <exception cref="ArgumentNullException">json or patches</exception>
        public string ApplyPatches(string json, IList<string> patches)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            if (Fragments == null) return json;

            JObject doc = JObject.Parse(json);
            JArray frr = (JArray)doc["fragments"]!;

            foreach (string patch in patches
                .OrderBy(p => p, new PatchOperationComparer()))
            {
                // split operation into tokens. Currently we have only:
                // -del coords
                // -mov oldCoords newCoords
                // so at least 2 tokens are required
                string[] opTokens = patch.Split(' ');
                if (opTokens.Length < 2
                    || (opTokens[0] != "del" && opTokens[0] != "mov")
                    || (opTokens[0] == "mov" && opTokens.Length != 3))
                {
                    continue;
                }

                // find index of fragment in array by its location
                AnonFragment? fragment =
                    Fragments.Find(fr => fr.Location == opTokens[1]);
                if (fragment == null) continue;
                int frIndex = Fragments.IndexOf(fragment);

                // apply operation to JSON
                switch (opTokens[0])
                {
                    case "del":
                        frr.RemoveAt(frIndex);
                        Fragments.RemoveAt(frIndex);
                        break;

                    case "mov":
                        JToken fr = frr[frIndex];
                        fr["location"] = opTokens[2];
                        Fragments[frIndex].Location = opTokens[2];
                        break;
                }
            }

            return doc.ToString(Formatting.None);
        }

        /// <summary>
        /// Gets the non-range fragments whose extent is equal to or less than
        /// that specified by the given Y/X coordinates.
        /// </summary>
        /// <param name="location">The location with Y and X coordinates.
        /// This must represent a single point, not a range.</param>
        /// <exception cref="ArgumentNullException">null location</exception>
        /// <returns>Fragments list, empty if none matches.</returns>
        public IList<AnonFragment> GetFragmentsAtIntegral(string location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            List<AnonFragment> frr = new();
            TokenTextLocation refLoc = TokenTextLocation.Parse(location);

            if (Fragments != null)
            {
                for (int i = Fragments.Count - 1; i > -1; i--)
                {
                    TokenTextLocation loc =
                        TokenTextLocation.Parse(Fragments[i].Location);
                    if (!loc.IsRange && loc.A.Y == refLoc.A.Y && loc.A.X == refLoc.A.X)
                        frr.Add(Fragments[i]);
                }
            }

            return frr;
        }

        /// <summary>
        /// Gets all the fragments ovlerapping the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The fragments</returns>
        /// <exception cref="ArgumentNullException">location</exception>
        public IList<AnonFragment> GetFragmentsAt(string location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));

            if (Fragments == null) return new List<AnonFragment>();

            TokenTextLocation requestedLoc = TokenTextLocation.Parse(location);

            return (from fr in Fragments
                    where TokenTextLocation.Parse(fr.Location).Overlaps(requestedLoc)
                    select fr).ToList();
        }

        /// <summary>
        /// Gets the fragments hints for the specified list of editing
        /// operations on the base text.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <returns>The hints list, one hint per fragment.</returns>
        /// <exception cref="ArgumentNullException">operations</exception>
        public IList<LayerHint> GetFragmentHints(
            IList<YXEditOperation> operations)
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));

            List<LayerHint> hints = new();

            foreach (AnonFragment fr in Fragments)
            {
                TokenTextLocation frLoc = TokenTextLocation.Parse(fr.Location);
                LayerHint hint = new()
                {
                    Location = fr.Location
                };
                hints.Add(hint);

                foreach (YXEditOperation operation in operations)
                {
                    TokenTextLocation opLoc = TokenTextLocation.Parse(
                        operation.OldLocation!);

                    bool isOverlap = frLoc.Overlaps(opLoc);
                    bool isCoincident = !frLoc.IsRange
                        && frLoc.A.Y == opLoc.A.Y
                        && frLoc.A.X == opLoc.A.X;

                    switch (operation.Operator)
                    {
                        case YXEditOperation.EQU:
                            if (isOverlap
                                && operation.OldLocation != operation.Location)
                            {
                                hint.EditOperation = operation.ToString();
                                hint.Description = $"text \"{operation.Value}\" moved";

                                if (isCoincident)
                                {
                                    hint.ImpactLevel = 2;
                                    hint.PatchOperation =
                                        $"mov {operation.OldLocation} {operation.Location}";
                                }
                                else
                                {
                                    hint.ImpactLevel = 1;
                                }
                            }
                            break;

                        case YXEditOperation.DEL:
                            if (isOverlap)
                            {
                                hint.EditOperation = operation.ToString();
                                hint.Description =
                                    $"text \"{operation.Value}\" deleted";
                                if (isCoincident)
                                {
                                    hint.ImpactLevel = 2;
                                    hint.PatchOperation =
                                        $"del {operation.OldLocation}";
                                }
                                else
                                {
                                    hint.ImpactLevel = 1;
                                }
                            }
                            break;

                        case YXEditOperation.MVD:
                            if (isCoincident)
                            {
                                hint.EditOperation = operation.ToString();
                                hint.Description =
                                    $"text \"{operation.Value}\" moved";
                                hint.ImpactLevel = 2;
                                string newLoc = operations.First(o =>
                                    o.GroupId == operation.GroupId
                                    && o != operation).OldLocation!;
                                hint.PatchOperation =
                                    $"mov {operation.Location} {newLoc}";
                                break;
                            }
                            if (isOverlap)
                            {
                                hint.EditOperation = operation.ToString();
                                hint.Description =
                                    $"text \"{operation.Value}\" moved";
                                hint.ImpactLevel = 1;
                            }
                            break;

                        case YXEditOperation.REP:
                            if (isOverlap)
                            {
                                hint.EditOperation = operation.ToString();
                                hint.Description =
                                    $"text \"{operation.OldValue}\" " +
                                    $"replaced with \"{operation.Value}\"";
                                hint.ImpactLevel = 1;
                            }
                            break;

                        // no hint for ins/mvi as relocations are already
                        // processed for equ
                    }
                }
            }
            return hints;
        }
    }

    internal sealed class PatchOperationComparer : IComparer<string>
    {
        private readonly Regex _opCmdRegex;

        public PatchOperationComparer()
        {
            _opCmdRegex = new Regex(@"^[^\s]+");
        }

        public int Compare(string? a, string? b)
        {
            string aCmd = _opCmdRegex.Match(a ?? "").Value;
            string bCmd = _opCmdRegex.Match(b ?? "").Value;

            // operations are only del and mov, and del must come first
            if (aCmd != bCmd)
            {
                return aCmd == "del" ? -1 : 1;
            }

            // if operations are equal, compare by location
            TokenTextLocation aLoc = TokenTextLocation.Parse(a!);
            TokenTextLocation bLoc = TokenTextLocation.Parse(b!);

            return aLoc.CompareTo(bLoc);
        }
    }

    /// <summary>
    /// An anonymous fragment, used in an <see cref="AnonLayerPart"/> to
    /// represent just the location of each fragment.
    /// </summary>
    public sealed class AnonFragment
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonFragment"/> class.
        /// </summary>
        public AnonFragment()
        {
            Location = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonFragment"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <exception cref="ArgumentNullException">location</exception>
        public AnonFragment(string location)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Location;
        }
    }
}
