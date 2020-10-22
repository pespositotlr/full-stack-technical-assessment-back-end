using ItemRepository.Interfaces;
using ItemRepository.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ItemRepository.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private IItemRepository itemRepository;

        [TestInitialize]
        public void Initialize()
        {
            itemRepository = new XmlItemRepository();
        }

        [TestMethod]
        public void TestGetItems()
        {
            Assert.IsNotNull(itemRepository.GetItems());
        }

        [TestMethod]
        public void TestGetItemsByItemName()
        {
            var name = "ITEM 1";
            var item1s = itemRepository.GetItemsByItemName(name);
            Assert.IsNotNull(item1s);
            Assert.AreEqual(item1s.FirstOrDefault().ItemName, name);
        }

        [TestMethod]
        public void TestGetItemById()
        {

        }

        [TestMethod]
        public void TestGetMaxPricedItems()
        {
            
        }       

        [TestMethod]
        public void TestCreateItem()
        {

        }

        [TestMethod]
        public void TestUpdateItem()
        {

        }

        [TestMethod]
        public void TestDeleteItem()
        {

        }
    }
}
