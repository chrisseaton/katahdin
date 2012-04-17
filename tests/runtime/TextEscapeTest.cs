using System;

using NUnit.Framework;

using Katahdin;

namespace Katahdin.Tests
{
    [TestFixture]
    public class TextEscapeTest
    {
        [Test]
        public void Escape()
        {
            Assert.AreEqual("abc", TextEscape.Escape("abc"));
            Assert.AreEqual("\\\"", TextEscape.Escape("\""));
            Assert.AreEqual("\\n", TextEscape.Escape("\n"));
            Assert.AreEqual("\\\\", TextEscape.Escape("\\"));
            Assert.AreEqual("\\0", TextEscape.Escape("\0"));
        }
        
        [Test]
        public void Unescape()
        {
            Assert.AreEqual("abc", TextEscape.Unescape("abc"));
            Assert.AreEqual("\"", TextEscape.Unescape("\\\""));
            Assert.AreEqual("\n", TextEscape.Unescape("\\n"));
            Assert.AreEqual("\\", TextEscape.Unescape("\\\\"));
        }
    }
}
