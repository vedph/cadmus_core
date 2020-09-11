﻿using System.Collections.Generic;
using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class DataPinBuilderTest
    {
        private static void AssertPinIds(IPart part, DataPin pin)
        {
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
        }

        [Fact]
        public void GetPins_Empty_0()
        {
            DataPinBuilder builder = new DataPinBuilder();

            Assert.Empty(builder.Build(new MockPart()));
        }

        [Fact]
        public void Increase_WithoutTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Increase("alpha", false);
            for (int i = 0; i < 3; i++) builder.Increase("beta", false);

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);
            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);
        }

        [Fact]
        public void Increase_WithTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Increase("alpha");
            for (int i = 0; i < 3; i++) builder.Increase("beta");

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);
            Assert.Equal(2 + 1, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);

            pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("4", pin.Value);
        }

        [Fact]
        public void Increase_With0NoTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Increase("alpha", false);
            for (int i = 0; i < 3; i++) builder.Increase("beta", false);
            // gamma is never found, so Increase is not invoked

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part, "gamma");
            Assert.Equal(3, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);

            pin = pins.Find(p => p.Name == "gamma-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("0", pin.Value);
        }

        [Fact]
        public void Set_WithoutTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Set("alpha", 1, false);
            builder.Set("beta", 3, false);

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);
            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);
        }

        [Fact]
        public void Set_WithTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Set("alpha", 1);
            builder.Set("beta", 3);

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);
            Assert.Equal(2 + 1, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);

            pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("4", pin.Value);
        }

        [Fact]
        public void Update_WithTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Update(new[] { "alpha", "beta", "beta", null }, true, "tag-");

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);

            Assert.Equal(2 + 1, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tag-alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "tag-beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("2", pin.Value);

            pin = pins.Find(p => p.Name == "tag-tot-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("4", pin.Value);
        }

        [Fact]
        public void Update_WithoutTotal_Ok()
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Update(new[] { "alpha", "beta", "beta", null }, false, "tag-");

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);

            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tag-alpha-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("1", pin.Value);

            pin = pins.Find(p => p.Name == "tag-beta-count");
            Assert.NotNull(pin);
            AssertPinIds(part, pin);
            Assert.Equal("2", pin.Value);
        }

        [Fact]
        public void AddValue_MultipleWithSameKey_Once()
        {
            DataPinBuilder builder = new DataPinBuilder();

            const string text = "The álpha!";
            builder.AddValue("alpha", text);
            builder.AddValue("alpha", text);

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);

            Assert.Single(pins);
            Assert.Equal("alpha", pins[0].Name);
            Assert.Equal(text, pins[0].Value);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AddValue_Unfiltered_Ok(bool preserveDigits)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());
            builder.AddValue("alpha", " The  álpha1 is here! ", null, true,
                preserveDigits);

            IPart part = new MockPart();
            List<DataPin> pins = builder.Build(part);

            Assert.Single(pins);
            Assert.Equal("alpha", pins[0].Name);
            Assert.Equal(preserveDigits
                ? "the alpha1 is here" : "the alpha is here", pins[0].Value);
        }

        [Theory]
        [InlineData(new object[] { false, "Héllo 1!" }, "Héllo 1!")]
        [InlineData(new object[] { true, "Héllo 1!" }, "hello")]
        [InlineData(new object[] { "Héllo 1! ", true, "Báz!", false, " @" },
            "Héllo 1! baz @")]
        public void ApplyFilter_Ok(object[] filtersAndValues, string expected)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());

            string actual = builder.ApplyFilter(false, filtersAndValues);

            Assert.Equal(expected, actual);
        }
    }

    internal sealed class MockPart : PartBase
    {
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new List<DataPin>();
        }
    }
}