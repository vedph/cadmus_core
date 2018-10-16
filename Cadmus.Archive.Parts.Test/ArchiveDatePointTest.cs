using Xunit;

namespace Cadmus.Archive.Parts.Test
{
    public sealed class ArchiveDatePointTest
    {
        [Fact]
        public void SortValue_YearOnly_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 1923
            };

            Assert.Equal(1923 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_YearAbout_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 1923,
                Approximation = ApproximationType.About
            };

            Assert.Equal(1923 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_YearMonth_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 1923,
                Month = 2
            };

            Assert.Equal(1923 * 365 + 2 * 30, point.SortValue);
        }

        [Fact]
        public void SortValue_YearMonthDay_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 1923,
                Month = 2,
                Day = 3
            };

            Assert.Equal(1923 * 365 + 2 * 30 + 3, point.SortValue);
        }

        [Fact]
        public void SortValue_Century_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century
            };

            Assert.Equal(1950 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturyAbout_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.About
            };

            Assert.Equal(1950 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturyBeginning_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.Beginning
            };

            Assert.Equal(1910 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturyFirstHalf_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.FirstHalf
            };

            Assert.Equal(1925 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturyHalf_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.Mid
            };

            Assert.Equal(1950 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturySecondHalf_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.SecondHalf
            };

            Assert.Equal(1975 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_CenturyEnd_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 20,
                ValueType = DateValueType.Century,
                Approximation = ApproximationType.End
            };

            Assert.Equal(1990 * 365, point.SortValue);
        }

        [Fact]
        public void SortValue_Decade_Ok()
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                Value = 192,
                ValueType = DateValueType.Decade
            };

            Assert.Equal(1925 * 365, point.SortValue);
        }
    }
}
