using System;

using NUnit.Framework;

using Katahdin.Collections;

namespace Katahdin.Tests
{
    [TestFixture]
    public class MultisetTest
    {
        [Test]
        public void Count()
        {
            Multiset<string> multiset = new Multiset<string>();
            
            Assert.AreEqual(0, multiset.Count);
            
            multiset.Add("a");
            multiset.Add("a");
            
            Assert.AreEqual(2, multiset.Count);
            
            multiset.Remove("a");
            
            Assert.AreEqual(1, multiset.Count);
            
            multiset.Add("b");
            
            Assert.AreEqual(2, multiset.Count);
            
            multiset.Remove("a");
            multiset.Remove("b");
            
            Assert.AreEqual(0, multiset.Count);
        }
        
        [Test]
        public void Contains()
        {
            Multiset<string> multiset = new Multiset<string>();

            multiset.Add("a");

            Assert.IsTrue(multiset.Contains("a"));
            
            multiset.Add("a");
            
            Assert.IsTrue(multiset.Contains("a"));
            
            multiset.Remove("a");
            multiset.Remove("a");
            
            Assert.IsFalse(multiset.Contains("a"));
        }
    }
}
