using Cadmus.Core.Layers;
using Cadmus.Parts.Layers;
using DiffMatchPatch;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Cadmus.Core.Test.Layers
{
    public sealed class AnonLayerPartTest
    {
        private readonly diff_match_patch _differ;
        private readonly IEditOperationDiffAdapter<YXEditOperation> _adapter;
        private readonly JsonSerializerOptions _jsonOptions;

        public AnonLayerPartTest()
        {
            _differ = new diff_match_patch();
            _adapter = new YXEditOperationDiffAdapter();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private string SerializePart(IPart part) =>
            JsonSerializer.Serialize(part, part.GetType(), _jsonOptions);

        private AnonLayerPart DeserializeAnonLayerPart(string json) =>
            (AnonLayerPart)JsonSerializer.Deserialize(json,
                typeof(AnonLayerPart), _jsonOptions);

        private IList<YXEditOperation> GetOperations(string a, string b)
        {
            List<Diff> diffs = _differ.diff_main(a, b);
            return _adapter.Adapt(diffs);
        }

        #region GetFragmentHints
        [Fact]
        public void GetFragmentHints_Equ_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "alpha beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.1
            LayerHint fr = fragments[0];
            Assert.Equal("1.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
        }

        [Fact]
        public void GetFragmentHints_EquMovedCoincident_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "alpha x beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1"),
                    new AnonFragment("1.2")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Equal(2, fragments.Count);
            // 1.1
            LayerHint fr = fragments[0];
            Assert.Equal("1.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
            // 1.2
            fr = fragments[1];
            Assert.Equal("1.2", fr.Location);
            Assert.Equal(2, fr.ImpactLevel);
            Assert.Equal("mov 1.2 1.3", fr.PatchOperation);
            Assert.NotNull(fr.Description);
        }

        [Fact]
        public void GetFragmentHints_EquMovedOverlap_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "alpha x beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1"),
                    new AnonFragment("1.2-2.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Equal(2, fragments.Count);
            // 1.1
            LayerHint fr = fragments[0];
            Assert.Equal("1.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
            // 1.2-2.1
            fr = fragments[1];
            Assert.Equal("1.2-2.1", fr.Location);
            Assert.Equal(1, fr.ImpactLevel);
            Assert.Null(fr.PatchOperation);
            Assert.NotNull(fr.Description);
        }

        [Fact]
        public void GetFragmentHints_DelNoOverlap_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("2.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 2.1
            LayerHint fr = fragments[0];
            Assert.Equal("2.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
        }

        [Fact]
        public void GetFragmentHints_DelOverlapping_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1-1.2")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.1-1.2
            LayerHint fr = fragments[0];
            Assert.Equal("1.1-1.2", fr.Location);
            Assert.Equal(1, fr.ImpactLevel);
            Assert.Equal(operations[1].ToString(), fr.EditOperation);
            Assert.Null(fr.PatchOperation);
        }

        [Fact]
        public void GetFragmentHints_DelCoincident_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta", "beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.1
            LayerHint fr = fragments[0];
            Assert.Equal("1.1", fr.Location);
            Assert.Equal(2, fr.ImpactLevel);
            Assert.Equal(operations[0].ToString(), fr.EditOperation);
            Assert.Equal("del 1.1", fr.PatchOperation);
        }

        [Fact]
        public void GetFragmentHints_MovNoOverlap_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta gamma",
                              "alpha gamma beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("2.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 2.1
            LayerHint fr = fragments[0];
            Assert.Equal("2.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
            Assert.Null(fr.PatchOperation);
        }

        [Fact]
        public void GetFragmentHints_MovOverlap_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta gamma",
                              "alpha gamma beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1-1.2")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.1-1.2
            LayerHint fr = fragments[0];
            Assert.Equal("1.1-1.2", fr.Location);
            Assert.Equal(1, fr.ImpactLevel);
            Assert.Equal(operations[1].ToString(), fr.EditOperation);
            Assert.Null(fr.PatchOperation);
        }

        [Fact]
        public void GetFragmentHints_MovCoincident_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta gamma",
                              "alpha gamma beta");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.2")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.2
            LayerHint fr = fragments[0];
            Assert.Equal("1.2", fr.Location);
            Assert.Equal(2, fr.ImpactLevel);
            Assert.Equal(operations[1].ToString(), fr.EditOperation);
            Assert.Equal("mov 1.2 1.3", fr.PatchOperation);
        }

        [Fact]
        public void GetFragmentHints_RepNoOverlap_Ok()
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta",
                              "alpha x");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment("1.1")
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            // 1.1
            LayerHint fr = fragments[0];
            Assert.Equal("1.1", fr.Location);
            Assert.Equal(0, fr.ImpactLevel);
            Assert.Null(fr.EditOperation);
            Assert.Null(fr.PatchOperation);
        }

        [Theory]
        [InlineData("1.2")]
        [InlineData("1.1-2.1")]
        public void GetFragmentHints_RepOverlapOrCoincident_Ok(string loc)
        {
            IList<YXEditOperation> operations =
                GetOperations("alpha beta",
                              "alpha x");
            AnonLayerPart part = new AnonLayerPart
            {
                Fragments = new List<AnonFragment>
                {
                    new AnonFragment(loc)
                }
            };

            IList<LayerHint> fragments = part.GetFragmentHints(operations);

            Assert.Single(fragments);
            LayerHint fr = fragments[0];
            Assert.Equal(loc, fr.Location);
            Assert.Equal(1, fr.ImpactLevel);
            Assert.Equal(operations[1].ToString(), fr.EditOperation);
            Assert.Null(fr.PatchOperation);
        }
        #endregion

        #region ApplyPatches
        [Fact]
        public void ApplyPatches_Empty_Unchanged()
        {
            TokenTextLayerPart<CommentLayerFragment> part =
                new TokenTextLayerPart<CommentLayerFragment>();
            string json = SerializePart(part);
            AnonLayerPart anon = DeserializeAnonLayerPart(json);

            string json2 = anon.ApplyPatches(json, Array.Empty<string>());

            Assert.Equal(json, json2);
        }
        #endregion
    }
}
