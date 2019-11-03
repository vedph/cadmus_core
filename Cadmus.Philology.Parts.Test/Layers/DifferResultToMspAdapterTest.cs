using Cadmus.Philology.Parts.Layers;
using DiffMatchPatch;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class DifferResultToMspAdapterTest
    {
        [Fact]
        public void Adapt_Equal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();
            var ops = adapter.Adapt(new Differ().Diffs("abc", "abc"));
            Assert.Empty(ops);
        }
    }
}
