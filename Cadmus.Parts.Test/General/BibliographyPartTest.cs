using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class BibliographyPartTest
    {
        private static BibliographyPart GetPart(int count)
        {
            BibliographyPart part = new BibliographyPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another"
            };
            for (int i = 0; i < count; i++)
            {
                part.Entries.Add(new BibEntry
                {
                    Key = $"e{i + 1}",
                    Authors = new[]
                    {
                        new BibAuthor
                        {
                            FirstName = "John",
                            LastName = "D" + new string((char)('a' + i), 2)
                        }
                    },
                    Title = $"Title {i}",
                    PlacePub = "Somewhere",
                    YearPub = 2020
                });
            }

            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            BibliographyPart part = GetPart(2);

            string json = TestHelper.SerializePart(part);
            CategoriesPart part2 = TestHelper.DeserializePart<CategoriesPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            Assert.Equal(2, part.Entries.Count);
        }

        [Fact]
        public void GetDataPins_NoEntries_Ok()
        {
            BibliographyPart part = GetPart(0);

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal("tot-count", pin.Name);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("0", pin.Value);
        }

        [Fact]
        public void GetDataPins_SingleEntryAllPinFields_Ok()
        {
            BibliographyPart part = GetPart(0);

            BibEntry entry = new BibEntry
            {
                Key = "heller & pomeroy 2020",
                // type (1)
                TypeId = "book-chapter",
                // authors (2)
                Authors = new[]
                {
                    new BibAuthor
                    {
                        FirstName = "Steven",
                        LastName = "Heller",
                    },
                    new BibAuthor
                    {
                        FirstName = "Karen",
                        LastName = "Pomeroy"
                    }
                },
                // title (1)
                Title = "A Survey: Perì Theôn",
                // contributors (1)
                Contributors = new[]
                {
                    new BibAuthor
                    {
                        FirstName = "Homer",
                        LastName = "Simpson",
                        RoleId = "ed"
                    }
                },
                // container (1)
                Container = "Theology, Today!",
                // keywords (1)
                Keywords = new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "gods"
                    }
                },
                PlacePub = "New York",
                YearPub = 2020
            };
            part.Entries.Add(entry);

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Equal(9, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("1", pin.Value);

            // key
            pin = pins.Find(p => p.Name == "key");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("heller & pomeroy 2020", pin.Value);

            // authors + contributors
            Assert.Equal(3, pins.Count(p => p.Name == "author"));
            Assert.NotNull(pins.Find(
                p => p.Name == "author" && p.Value == "heller"));
            Assert.NotNull(pins.Find(
                p => p.Name == "author" && p.Value == "pomeroy"));
            Assert.NotNull(pins.Find(
                p => p.Name == "author" && p.Value == "simpson"));

            // title
            Assert.Equal(1, pins.Count(p => p.Name == "title"));
            Assert.Equal("a survey peri theon", pins.Find(
                p => p.Name == "title").Value);

            Assert.Equal(1, pins.Count(p => p.Name == "container"));
            Assert.Equal("theology today", pins.Find(
                p => p.Name == "container").Value);

            // keyword
            Assert.Equal(1, pins.Count(p => p.Name == "keyword.eng"));
            Assert.Equal("gods", pins.Find(
                p => p.Name == "keyword.eng").Value);

            // type-X-count
            pin = pins.Find(p => p.Name == "type-book-chapter-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("1", pin.Value);
        }

        [Fact]
        public void GetDataPins_MultipleEntries_Ok()
        {
            BibliographyPart part = GetPart(0);

            BibEntry book = new BibEntry
            {
                Key = "heller & pomeroy 2020",
                TypeId = "book",
                Authors = new[]
                {
                    new BibAuthor
                    {
                        FirstName = "Steven",
                        LastName = "Heller",
                    },
                    new BibAuthor
                    {
                        FirstName = "Karen",
                        LastName = "Pomeroy"
                    }
                },
                Title = "Design Literacy: Understanding Graphic Design",
                PlacePub = "New York",
                YearPub = 2020
            };
            part.Entries.Add(book);

            BibEntry paper = new BibEntry
            {
                Key = "Fusi 2018",
                TypeId = "journal-paper",
                Authors = new[]
                {
                    new BibAuthor
                    {
                        FirstName = "Daniele",
                        LastName = "Fusi"
                    }
                },
                Title = "Sailing for a Second Navigation: " +
                    "Paradigms in Producing Digital Content",
                Container = "SemRom",
                Number = "n.s.7",
                YearPub = 2018,
                FirstPage = 213,
                LastPage = 276,
                Keywords = new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "scholarly digital edition"
                    }
                }
            };
            part.Entries.Add(paper);

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Equal(12, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("2", pin.Value);

            Assert.Equal(2, pins.Count(p => p.Name == "key"));
            Assert.Equal(3, pins.Count(p => p.Name == "author"));
            Assert.Equal(2, pins.Count(p => p.Name == "title"));
            Assert.Equal(1, pins.Count(p => p.Name == "container"));
            Assert.Equal(1, pins.Count(p => p.Name == "keyword.eng"));
        }
    }
}
