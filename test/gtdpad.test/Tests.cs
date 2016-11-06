using System;
using Xunit;

namespace gtdpad.test
{
    public class Tests
    {
        IRepository _db;

        public Tests()
        {
            _db = new FakeRepository();
        }

        [Fact]
        public void Test1() 
        {
            var model = new Page {
                Title = "Test"
            };

            var model2 = model.SetDefaults<Page>();

            Assert.True(model2.ID != Guid.Empty);
        }
    }
}
