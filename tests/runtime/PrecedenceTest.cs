using System;

using NUnit.Framework;

using Katahdin.Grammars.Precedences;

namespace Katahdin.Tests
{
    [TestFixture]
    public class PrecedenceTest
    {
        [Test]
        public void Basic()
        {
            Precedence a = new Precedence("a");
            Precedence b = new Precedence("b");
            
            Precedence.SetPrecedence(a, b, Relation.Lower);
            
            Assert.IsTrue(a.IsLowerThan(b, false));
            Assert.IsTrue(a.IsLowerThan(b, true));
            
            Assert.IsFalse(b.IsLowerThan(a, false));
            Assert.IsFalse(b.IsLowerThan(a, true));
        }

        [Test]
        public void Equal()
        {
            Precedence a = new Precedence("a");
            Precedence b = new Precedence("b");
            Precedence c = new Precedence("c");
            
            Precedence.SetPrecedence(a, b, Relation.Lower);
            Precedence.SetPrecedence(c, b, Relation.Equal);

            Assert.IsTrue(c.IsLowerThan(b, true));
            Assert.IsFalse(c.IsLowerThan(b, false));

            Assert.IsTrue(b.IsLowerThan(c, true));
            Assert.IsFalse(c.IsLowerThan(b, false));
            
            Assert.IsTrue(a.IsLowerThan(c, true));
            Assert.IsTrue(a.IsLowerThan(c, false));
        }

        [Test]
        public void Higher()
        {
            Precedence a = new Precedence("a");
            Precedence b = new Precedence("b");
            Precedence c = new Precedence("c");
            
            Precedence.SetPrecedence(a, b, Relation.Lower);
            Precedence.SetPrecedence(c, b, Relation.Higher);
            
            Assert.IsTrue(a.IsLowerThan(c, true));
            Assert.IsTrue(a.IsLowerThan(c, false));
            
            Assert.IsTrue(b.IsLowerThan(c, true));
            Assert.IsTrue(b.IsLowerThan(c, false));
            
            Assert.IsFalse(c.IsLowerThan(a, true));
            Assert.IsFalse(c.IsLowerThan(a, false));
            
            Assert.IsFalse(c.IsLowerThan(b, true));
            Assert.IsFalse(c.IsLowerThan(b, false));
        }
    }
}
