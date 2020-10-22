using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ItemRepository.Models;
using System.Linq;
using System.Net.Http;
using FullStackTechnicalAssessment.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FullStackTechnicalAssessment.Tests
{
    [TestClass]
    public class WebServiceTests
    {
        [TestMethod]
        public async Task TestGetItems()
        {
            var factory = new WebApplicationFactory<Startup>();
            var Client = factory.CreateClient();
            var result = await Client.GetAsync("/api/GetItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            var content = result.Content;
            var data = await content.ReadAsStringAsync();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public async Task TestGetMaxPricedItems()
        {
            var factory = new WebApplicationFactory<Startup>();
            var Client = factory.CreateClient();
            var result = await Client.GetAsync("/api/GetMaxPricedItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

            var content = result.Content;
            var data = await content.ReadAsStringAsync();

            var items = (List<ConcreteItem>)JsonConvert.DeserializeObject(data, typeof(List<ConcreteItem>));
            Assert.AreEqual(items.FirstOrDefault(x => x.ItemName == "ITEM 1").Cost, 250);
        }

        [TestMethod]
        public async Task TestGetMaxPriceByItemName()
        {
            var factory = new WebApplicationFactory<Startup>();
            var Client = factory.CreateClient();
            var result = await Client.GetAsync("/api/GetMaxPriceByItemName/ITEM 1");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

            var content = result.Content;
            var data = await content.ReadAsStringAsync();
            Assert.AreEqual(Convert.ToString(250), data);
        }

        [TestMethod]
        public async Task TestGetMaxPriceByItemNameForNonExistantName()
        {
            var factory = new WebApplicationFactory<Startup>();
            var Client = factory.CreateClient();
            var result = await Client.GetAsync("/api/GetMaxPriceByItemName/ITEM 88888888");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateItem()
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            ItemViewModel item = new ItemViewModel();
            item.Id = 999;
            item.Cost = 555;
            item.ItemName = "Unit Test Item";

            var stringContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var createResult = await client.PostAsync("/api/CreateItem", stringContent);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, createResult.StatusCode);
            
            var getResult = await client.GetAsync("/api/GetItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, getResult.StatusCode);
            var content = getResult.Content;
            var data = await content.ReadAsStringAsync();

            var items = (List<ConcreteItem>)JsonConvert.DeserializeObject(data, typeof(List<ConcreteItem>));
            Assert.IsTrue(items.Any(x => x.Id == item.Id && x.Cost == item.Cost && x.ItemName == item.ItemName));
        }

        [TestMethod]
        public async Task TestUpdateItem()
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            ItemViewModel item = new ItemViewModel();
            item.Id = 1;
            item.Cost = 999999;
            item.ItemName = "ITEM 1";

            var stringContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var updateResult = await client.PutAsync("/api/UpdateItem", stringContent);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, updateResult.StatusCode);

            var getResult = await client.GetAsync("/api/GetItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, getResult.StatusCode);
            var content = getResult.Content;
            var data = await content.ReadAsStringAsync();
            Assert.IsNotNull(data);

            var items = (List<ConcreteItem>)JsonConvert.DeserializeObject(data, typeof(List<ConcreteItem>));

            var updatedItem = items.FirstOrDefault(x => x.Id == item.Id);

            Assert.IsNotNull(updatedItem);
            Assert.AreEqual(updatedItem.Cost, item.Cost);
            Assert.AreEqual(updatedItem.ItemName, item.ItemName);
        }

        [TestMethod]
        public async Task TestUpdateNonExistantItem()
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            ItemViewModel item = new ItemViewModel();
            item.Id = 77777777;
            item.Cost = 4;
            item.ItemName = "ITEM 77777777";

            var stringContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var updateResult = await client.PutAsync("/api/UpdateItem", stringContent);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, updateResult.StatusCode);

        }

        [TestMethod]
        public async Task TestDeleteItem()
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            ItemViewModel item = new ItemViewModel();
            item.Id = 999;
            item.Cost = 555;
            item.ItemName = "Unit Test Item";

            //Create an item
            var stringContentCreate = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var createResult = await client.PostAsync("/api/CreateItem", stringContentCreate);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, createResult.StatusCode);

            //Check that item has been created
            var getResult1 = await client.GetAsync("/api/GetItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, getResult1.StatusCode);
            var content1 = getResult1.Content;
            var data1 = await content1.ReadAsStringAsync();

            var items1 = (List<ConcreteItem>)JsonConvert.DeserializeObject(data1, typeof(List<ConcreteItem>));
            Assert.IsNotNull(items1.Any(x => x.Id == item.Id && x.Cost == item.Cost && x.ItemName == item.ItemName));

            //Delete that item
            var stringContentDelete = new StringContent(JsonConvert.SerializeObject(item.Id), Encoding.UTF8, "application/json");
            var deleteResult = await client.DeleteAsync("/api/DeleteItem/" + stringContentDelete);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, deleteResult.StatusCode);

            //See that item has been deleted
            var getResult2 = await client.GetAsync("/api/GetItems");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, getResult2.StatusCode);
            var content2 = getResult2.Content;
            var data2 = await content2.ReadAsStringAsync();

            var items2 = (List<ConcreteItem>)JsonConvert.DeserializeObject(data2, typeof(List<ConcreteItem>));
            Assert.IsFalse(items2.Any(x => x.Id == item.Id && x.Cost == item.Cost && x.ItemName == item.ItemName));

        }

        [TestMethod]
        public async Task TestDeleteNonExistantItem()
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            //Delete that item
            var stringContentDelete = new StringContent(JsonConvert.SerializeObject(1111111), Encoding.UTF8, "application/json");
            var deleteResult = await client.PutAsync("/api/DeleteItem", stringContentDelete);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, deleteResult.StatusCode);

        }
    }
}
