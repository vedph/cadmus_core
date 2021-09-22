using MySql.Data.MySqlClient;
using System;
using Xunit;

namespace Cadmus.Index.Sql.Test
{
    public sealed class SqlSelectBuilderTest
    {
        private static SqlSelectBuilder GetBuilder()
            => new SqlSelectBuilder(() => new MySqlCommand());

        private static readonly string NL = Environment.NewLine;

        [Fact]
        public void AddWhat_Once_Ok()
        {
            SqlSelectBuilder builder = GetBuilder();
            builder.AddWhat("id, name");
            builder.AddFrom("person");

            string sql = builder.Build();

            Assert.Equal($"SELECT id, name{NL}FROM person", sql);
        }

        [Fact]
        public void AddWhat_Twice_Ok()
        {
            SqlSelectBuilder builder = GetBuilder();
            builder.AddWhat("id, name");
            builder.AddWhat("sex");
            builder.AddFrom("person");

            string sql = builder.Build();

            Assert.Equal($"SELECT id, name, sex{NL}FROM person", sql);
        }

        [Fact]
        public void AddWhat_DifferentSlots_Ok()
        {
            SqlSelectBuilder builder = GetBuilder();
            builder.AddWhat("id, name");
            builder.AddWhat("COUNT(id)", slotId: "c");
            builder.AddFrom("person", slotId: "*");

            string sql = builder.Build();
            Assert.Equal($"SELECT id, name{NL}FROM person", sql);

            sql = builder.Build("c");
            Assert.Equal($"SELECT COUNT(id){NL}FROM person", sql);
        }
    }
}
