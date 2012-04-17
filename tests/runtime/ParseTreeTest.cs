using System;

using NUnit.Framework;

namespace Katahdin.Tests
{
    [TestFixture]
    public class ParseTreeTest
    {
        [Test]
        public void ExtendingFields()
        {
            ParseTree a = new ParseTree();
            a.Fields["a"] = "A";
            a.Fields["b"] = "B";
            a.Fields["c"] = "C";
            
            Assert.IsTrue(a.Fields.ContainsKey("a"));
            Assert.IsTrue(a.Fields.ContainsKey("b"));
            Assert.IsTrue(a.Fields.ContainsKey("c"));
            
            ParseTree b = new ParseTree();
            b.ExtendFields(a);
            b.Fields["d"] = "D";
            
            Assert.IsTrue(b.Fields.ContainsKey("a"));
            Assert.IsTrue(b.Fields.ContainsKey("b"));
            Assert.IsTrue(b.Fields.ContainsKey("c"));
            Assert.IsTrue(b.Fields.ContainsKey("d"));
        }
    }
}
