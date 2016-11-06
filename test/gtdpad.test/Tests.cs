using System;
using System.Dynamic;
using Xunit;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace gtdpad.test
{
    public class Tests
    {
        IRepository _db;
        TestBootstrapper _bootstrapper;
        Browser _browser;

        public Tests()
        {
            _db = new FakeRepository();
            _bootstrapper = new TestBootstrapper();
            _browser = new Browser(_bootstrapper, defaults: to => to.Accept("application/json"));
        }

        [Fact]
        public void Set_Model_Defaults() 
        {
            var guid = Guid.NewGuid();
            var model1 = ModelExtensions.SetDefaults<Page>(new Page { ID = guid, Title = "Test" });
            var model2 = ModelExtensions.SetDefaults<Page>(new Page { Title = "Test" });

            Assert.True(model1.ID == guid);
            Assert.True(model2.ID != Guid.Empty);
        }

        [Fact]
        public async void Get_Page()
        {
            var req = await _browser.Get("/pages/9bcf9ac4-4256-4070-9553-7e2db1b4c368", with => { with.HttpRequest(); });

            var page = req.Body.DeserializeJson<Page>();

            Assert.Equal(HttpStatusCode.OK, req.StatusCode);
            Assert.Equal(page.ID, new Guid("9bcf9ac4-4256-4070-9553-7e2db1b4c368"));
            Assert.Equal(page.Title, "PAGE A");
            Assert.Equal(page.UserID, new Guid("39f4aa1a-c452-41ef-b0c9-e873274da1b3"));
        }

        [Fact]
        public async void Get_Page_Deep()
        {
            var req = await _browser.Get("/pages/9bcf9ac4-4256-4070-9553-7e2db1b4c368", with => { 
                with.HttpRequest();
                with.Query("deep", "true"); 
            });

            var page = req.Body.DeserializeJson<Page>();

            Assert.Equal(HttpStatusCode.OK, req.StatusCode);
            Assert.Equal(page.ID, new Guid("9bcf9ac4-4256-4070-9553-7e2db1b4c368"));
            Assert.Equal(page.Title, "PAGE A");
        }
    }
}
