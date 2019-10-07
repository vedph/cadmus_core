using System.Collections.Generic;
using Xunit;

namespace Cadmus.Archive.Parts.Test
{
    public sealed class ArchiveDateParserTest
    {
        #region Null
        [Fact]
        public void Parse_Empty_Null()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("");

            Assert.Equal(0, dates.Count);
        }

        [Fact]
        public void Parse_SenzaData_Null()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("senza data");

            Assert.Equal(0, dates.Count);
        }

        [Fact]
        public void Parse_Sd_Null()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("s.d.");

            Assert.Equal(0, dates.Count);
        }
        #endregion

        #region Centuries
        [Fact]
        public void Parse_CenturyRight_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeft_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyFullRight_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("secolo XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyFullLeft_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("XX secolo");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightBeginning_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("inizio sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Beginning, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightFirstHalf_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("I metà sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.FirstHalf, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightMid_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("metà sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Mid, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightSecondHalf_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("II metà sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.SecondHalf, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightEnd_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("fine sec. XX");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.End, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightIn_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("sec. XX in.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Beginning, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyRightEx_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("sec. XX ex.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.End, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftBeginning_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("inizio XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Beginning, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftFirstHalf_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("I metà XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.FirstHalf, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftMid_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("metà XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Mid, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftSecondHalf_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("II metà XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.SecondHalf, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftEnd_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("fine XX sec.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.End, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftIn_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("XX sec. in.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.Beginning, date.A.Approximation);
        }

        [Fact]
        public void Parse_CenturyLeftEx_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("XX sec. ex.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(20, date.A.Value);
            Assert.Equal(DateValueType.Century, date.A.ValueType);
            Assert.Equal(ApproximationType.End, date.A.Approximation);
        }
        #endregion

        #region Decades
        [Fact]
        public void Parse_DecadeNumeric_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("anni '10");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(191, date.A.Value);
            Assert.Equal(DateValueType.Decade, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DecadeFullNumeric_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("anni 1910");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(191, date.A.Value);
            Assert.Equal(DateValueType.Decade, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DecadeAlphabetic_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("anni dieci");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(191, date.A.Value);
            Assert.Equal(DateValueType.Decade, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }
        #endregion

        #region YMD Style N Dash
        [Fact]
        public void Parse_YmdStyleNSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleN1DigitSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-5-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YStyleNSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970]-05-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-[05]-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-[30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970-05]-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-[05-30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970-05-30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepDashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970-05]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepDashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970]-05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepDashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-[05]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YStyleNSepDashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepDashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970-05-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepDashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca 1970-05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_YStyleNSepDashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("circa 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region YMD Style N Slash
        [Fact]
        public void Parse_YmdStyleNSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/05/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleN1DigitSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/5/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970]/05/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/[05]/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/05/[30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970/05]/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/[05/30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970/05/30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepSlashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970/05]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepSlashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970]/05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepSlashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970/[05]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleNSepSlashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970/05/30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleNSepSlashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca 1970/05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region YMD Style S
        [Fact]
        public void Parse_YmdStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag. 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmAltNameStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970 lu.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(7, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970], mag. 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [mag.] 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag. [30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, mag.] 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [mag. 30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, mag. 30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleSInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970], mag.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleSInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [mag.]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleSInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, mag.]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleSAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970, mag. 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleSAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970, mag.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region YMD Style F
        [Fact]
        public void Parse_YmdStyleF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970], maggio 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [maggio] 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio [30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, maggio] 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [maggio 30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, maggio 30]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleFInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970], maggio");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleFInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, [maggio]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleFInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[1970, maggio]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmdStyleFAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970, maggio 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_YmStyleFAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 1970, maggio");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region DMY Style N Dash
        [Fact]
        public void Parse_DmyStyleNSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleN1DigitSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-5-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepDash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-05-[1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-[05]-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30]-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-[05-1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30-05]-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30-05-1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepDashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[05-1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepDashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("05-[1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepDashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[05]-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepDashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 30-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepDashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca 30-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region DMY Style N Slash
        [Fact]
        public void Parse_DmyStyleNSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30/05/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleN1DigitSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30/5/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepSlash_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("05/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30/05/[1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30/[05]/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30]/05/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30/[05/1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30/05]/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30/05/1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepSlashInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[05/1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepSlashInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("05/[1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepSlashInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[05]/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleNSepSlashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 30/05/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleNSepSlashAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca 05/1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region DMY Style S
        [Fact]
        public void Parse_DmyStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyAltNameStyleS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("lu. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(7, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 mag. [1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 [mag.] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30] mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 [mag. 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30 mag.] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30 mag. 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleSInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("mag. [1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleSInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[mag.] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleSInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[mag. 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleSAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 30 mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleSAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region DMY Style F
        [Fact]
        public void Parse_DmyStyleF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 maggio [1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 [maggio] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30] maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 [maggio 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30 maggio] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFInferredYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[30 maggio 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.True(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleFInferredY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("maggio [1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleFInferredM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[maggio] 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleFInferredYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("[maggio 1970]");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.True(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.True(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_DmyStyleFAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. 30 maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(30, date.A.Day);
            Assert.False(date.A.IsDayInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }

        [Fact]
        public void Parse_MyStyleFAbout_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ca. maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.False(date.A.IsYearInferred);
            Assert.Equal(5, date.A.Month);
            Assert.False(date.A.IsMonthInferred);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.About, date.A.Approximation);
        }
        #endregion

        #region Terminus ante
        [Fact]
        public void Parse_AnteY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 1970-05-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 1970-05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteDMY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 30-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteMY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteYShortMonthD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 1970, mag. 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteYFullMonthD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 1970, maggio 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteDShortMonthY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 30 mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_AnteDFullMonthY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("ante 30 maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.A);
            Assert.NotNull(date.B);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(30, date.B.Day);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }
        #endregion

        #region Terminus post
        [Fact]
        public void Parse_PostY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 1970-05-30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 1970-05");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostDMY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 30-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostMY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostYShortMonthD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 1970, mag. 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostYFullMonthD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 1970, maggio 30");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostDShortMonthY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 30 mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }

        [Fact]
        public void Parse_PostDFullMonthY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("post 30 maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.Null(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
        }
        #endregion

        #region Ranges
        [Fact]
        public void Parse_RangeYDashY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970 - 1980");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(1980, date.B.Value);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_RangeYSlashY_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970 / 1980");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(1980, date.B.Value);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_RangeYMDashYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05 - 1970-06");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_RangeYMDDashYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30 - 1970-06-28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_RangeYMSlashYM_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05 / 1970-06");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_RangeYMDSlashYMD_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30 / 1970-06-28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_BetweenRange_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("tra il 1970-05-30 e il 1970-06-28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }
        #endregion

        #region Shortened ranges
        [Fact]
        public void Parse_ShortRangeYmdMdN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30 / 06-28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmdDN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30 / 31");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmMN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05 / 06");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDmN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30-05 / 28-06-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 / 31-05-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmMN_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("05 / 06-1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmdMdS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag. 30 / giu. 28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmdDS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag. 30 / 31");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmMS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, mag. / giu.");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDmS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 mag. / 28 giu. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 / 31 mag. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmMS_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("mag. / giu. 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmdMdF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio 30 / giugno 28");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmdDF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio 30 / 31");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeYmMF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970, maggio / giugno");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDmF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 maggio / 28 giugno 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(28, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmyDF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("30 / 31 maggio 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(5, date.B.Month);
            Assert.Equal(31, date.B.Day);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }

        [Fact]
        public void Parse_ShortRangeDmMF_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("maggio / giugno 1970");

            Assert.Equal(1, dates.Count);
            ArchiveDate date = dates[0];
            Assert.True(date.IsRange);
            Assert.NotNull(date.B);
            Assert.NotNull(date.A);
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(1970, date.B.Value);
            Assert.Equal(6, date.B.Month);
            Assert.Equal(DateValueType.Year, date.A.ValueType);
            Assert.Equal(ApproximationType.None, date.A.Approximation);
            Assert.Equal(DateValueType.Year, date.B.ValueType);
            Assert.Equal(ApproximationType.None, date.B.Approximation);
        }
        #endregion

        [Fact]
        public void Parse_Multiple_Ok()
        {
            ArchiveDateParser parser = new ArchiveDateParser();

            IList<ArchiveDate> dates = parser.Parse("1970-05-30; 1973-06-01");

            Assert.Equal(2, dates.Count);

            ArchiveDate date = dates[0];
            Assert.Equal(1970, date.A.Value);
            Assert.Equal(5, date.A.Month);
            Assert.Equal(30, date.A.Day);

            date = dates[1];
            Assert.Equal(1973, date.A.Value);
            Assert.Equal(6, date.A.Month);
            Assert.Equal(1, date.A.Day);
        }
    }
}
