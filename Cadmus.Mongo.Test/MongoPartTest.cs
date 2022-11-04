﻿using Cadmus.Core;
using MongoDB.Bson;
using System;
using Xunit;

namespace Cadmus.Mongo.Test
{
    public sealed class MongoPartTest
    {
        [Fact]
        public void ToLayerPartInfo_EmptyLayerPart_FrCount0()
        {
            MongoPart part = new()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = Guid.NewGuid().ToString(),
                TypeId = "some-typeid",
                RoleId = PartBase.FR_PREFIX + "some-roleid",
                Content = BsonDocument.Parse("{\"fragments\": []}")
            };

            LayerPartInfo info = part.ToLayerPartInfo();

            Assert.Equal(0, info.FragmentCount);
        }

        [Fact]
        public void ToLayerPartInfo_NonEmptyLayerPart_FrCount3()
        {
            MongoPart part = new()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = Guid.NewGuid().ToString(),
                TypeId = "some-typeid",
                RoleId = PartBase.FR_PREFIX + "some-roleid",
                Content = BsonDocument.Parse(
                    "{\"fragments\": [ {\"x\":1}, {\"x\":2}, {\"x\":3} ]}")
            };

            LayerPartInfo info = part.ToLayerPartInfo();

            Assert.Equal(3, info.FragmentCount);
        }
    }
}
