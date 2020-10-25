using ItemRepository.Interfaces;
using ItemRepository.Models;
using ItemRepository.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ItemRepository.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private IItemRepository _itemRepository;

        [TestInitialize]
        public void Initialize()
        {
            var myConfiguration = new Dictionary<string, string>
                {
                    {"ItemType", "1"},
                    {"AllowedHosts", "*"}
                };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _itemRepository = new XmlItemRepository(configuration);
        }
        
        [TestMethod]
        public void TestGetItems()
        {
            Assert.IsNotNull(_itemRepository.GetItems());
        }

        [TestMethod]
        public void TestGetItemsByItemName()
        {
            var name = "ITEM 1";
            var item1s = _itemRepository.GetItemsByItemName(name);
            Assert.IsNotNull(item1s);
            Assert.AreEqual(item1s.FirstOrDefault().ItemName, name);
        }

        [TestMethod]
        public void TestGetItemById()
        {
            var name = "ITEM 1";
            var item1 = _itemRepository.GetItemById(1);
            Assert.IsNotNull(item1);
            Assert.AreEqual(item1.ItemName, name);
        }

        [TestMethod]
        public void TestGetItemById_NonExistantItem()
        {
            var item0 = _itemRepository.GetItemById(0);
            Assert.IsNull(item0);
        }

        [TestMethod]
        public void TestGetMaxPricedItems()
        {
            var items = _itemRepository.GetMaxPricedItems();
            Assert.IsTrue(items.Any());
            var item1 = items.Where(x => x.ItemName == "ITEM 1");
            Assert.IsTrue(item1.Count() == 1);
            Assert.AreEqual(item1.FirstOrDefault().Cost, 250);
        }       

        [TestMethod]
        public void TestCreateItem()
        {
            ConcreteItem newItem = new ConcreteItem();
            newItem.ItemName = "ITEM 999";
            newItem.Cost = 999;

            _itemRepository.CreateItem(newItem);

            var items = _itemRepository.GetItems();

            Assert.IsNotNull(items.FirstOrDefault(x => x.ItemName == newItem.ItemName));
        }

        [TestMethod]
        public void TestUpdateItem()
        {
            ConcreteItem itemToUpdate = new ConcreteItem();
            itemToUpdate.Id = 1;
            itemToUpdate.ItemName = "ITEM 1";
            itemToUpdate.Cost = 77777;

            _itemRepository.UpdateItem(itemToUpdate);

            var item = _itemRepository.GetItemById(itemToUpdate.Id);

            Assert.AreEqual(item.Cost, itemToUpdate.Cost);
            Assert.AreEqual(item.ItemName, itemToUpdate.ItemName);
        }

        [TestMethod]
        public void TestDeleteItem()
        {
            ConcreteItem itemToAdd = new ConcreteItem();
            itemToAdd.ItemName = "ITEM 555";
            itemToAdd.Cost = 555;

            var newId = _itemRepository.CreateItem(itemToAdd);

            Assert.IsTrue(newId > 0);

            var items = _itemRepository.GetItems();

            var newItem = items.FirstOrDefault(x => x.ItemName == itemToAdd.ItemName && x.Cost == 555);

            Assert.IsNotNull(newItem);

            _itemRepository.DeleteItem(newItem.Id);

            items = _itemRepository.GetItems();

            Assert.IsNull(items.FirstOrDefault(x => x.ItemName == itemToAdd.ItemName && x.Cost == 555));
        }
    }
}
