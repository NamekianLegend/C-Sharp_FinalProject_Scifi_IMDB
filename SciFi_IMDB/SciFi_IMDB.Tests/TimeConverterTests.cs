
using SciFi_IMDB.Services;

namespace SciFi_IMDB.Tests
{
    [TestClass]
    public class TimeConverterTests
    {
        private MillisecondsToTimeConverter _converter;

        [TestInitialize]
        public void Setup()
        {
            _converter = new MillisecondsToTimeConverter();
        }

        [TestMethod]
        [DataRow(120, "2:00:00")]  // Standard 2 hour movie
        [DataRow(90, "1:30:00")]   // 1.5 hour movie
        [DataRow(45, "45:00")]     // Less than an hour (MM:SS format)
        public void Convert_ValidMinutes_ReturnsCorrectString(int input, string expected)
        {
            var result = _converter.Convert(input, typeof(string), null, null);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Convert_ZeroOrNegative_ReturnsNA()
        {
            var zeroResult = _converter.Convert(0, typeof(string), null, null);
            var negativeResult = _converter.Convert(-10, typeof(string), null, null);
            Assert.AreEqual("N/A", zeroResult);
            Assert.AreEqual("N/A", negativeResult);
        }

        [TestMethod]
        public void Convert_InvalidType_ReturnsDefault()
        {
            var result = _converter.Convert("NotANumber", typeof(string), null, null);
            Assert.AreEqual("00:00", result);
        }

        [TestMethod]
        public void Convert_NullInput_ReturnsDefault()
        {
            var result = _converter.Convert(null, typeof(string), null, null);
            Assert.AreEqual("00:00", result);
        }
    }
}
