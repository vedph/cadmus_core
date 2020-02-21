using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>No pins.</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return Enumerable.Empty<DataPin>();
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
        /// <exception cref="ArgumentNullException">null location</exception>
        public string DeleteFragmentsAtIntegral(string json, string location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            if (Fragments == null) return json;

            JObject doc = JObject.Parse(json);
            JArray frr = (JArray)doc["fragments"];

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

            List<AnonFragment> frr = new List<AnonFragment>();
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

            List<LayerHint> hints = new List<LayerHint>();

            foreach (AnonFragment fr in Fragments)
            {
                TokenTextLocation frLoc = TokenTextLocation.Parse(fr.Location);
                LayerHint hint = new LayerHint
                {
                    Location = fr.Location
                };
                hints.Add(hint);

                foreach (YXEditOperation operation in operations)
                {
                    TokenTextLocation opLoc = TokenTextLocation.Parse(
                        operation.OldLocation);

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
                                else hint.ImpactLevel = 1;
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
                                else hint.ImpactLevel = 1;
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
                                    && o != operation).OldLocation;
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
