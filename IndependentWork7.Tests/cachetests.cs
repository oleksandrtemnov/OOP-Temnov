using System;
using System.Collections.Generic;
using Xunit;
using IndependentWork7;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldReturnStoredValue()
        {
            var cache = new Cache<int, string>(3);
            cache.Add(1, "One");
            cache.Add(2, "Two");

            Assert.Equal("One", cache.Get(1));
            Assert.Equal("Two", cache.Get(2));
        }

        [Fact]
        public void Count_ShouldReturnCurrentNumberOfElements()
        {
            var cache = new Cache<string, int>(5);
            Assert.Equal(0, cache.Count);

            cache.Add("a", 10);
            cache.Add("b", 20);

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueIfExists_AndFalseOtherwise()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "v1");

            Assert.True(cache.ContainsKey("k1"));
            Assert.False(cache.ContainsKey("k2"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Value");

            Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
        }

        [Fact]
        public void Add_WhenCapacityExceeded_ShouldEvictOldestItem_FIFO()
        {
            var cache = new Cache<string, int>(2);
            cache.Add("first", 1);
            cache.Add("second", 2);

           
            cache.Add("third", 3);

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey("first"));
            Assert.True(cache.ContainsKey("second"));
            Assert.True(cache.ContainsKey("third"));
            Assert.Throws<KeyNotFoundException>(() => cache.Get("first"));
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutEviction()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Old");
            cache.Add(2, "Two");

            cache.Add(1, "New");

            Assert.Equal(2, cache.Count);
            Assert.Equal("New", cache.Get(1));
            Assert.True(cache.ContainsKey(2));
        }
    }
}