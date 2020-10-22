using ItemRepository.Factories;
using ItemRepository.Interfaces;
using ItemRepository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ItemRepository.Repositories
{
    public class XmlItemRepository : IItemRepository
    {
        private readonly string itemDataStorePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DataStores/ItemsDataStore.xml");
        private List<IItem> _items;
        private ItemFactory _itemFactory;
        private IConfiguration _config;

        public XmlItemRepository(IConfiguration config)
        {
            _config = config;
            _itemFactory = new ItemFactory(_config);
            _items = GetItemsFromDataStore();
        }

        public IEnumerable<IItem> GetItems()
        {
            return _items;
        }
        public IEnumerable<IItem> GetItemsByItemName(string itemName)
        {
            return _items.Where(x => x.ItemName == itemName);
        }
        public IItem GetItemById(int id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IItem> GetMaxPricedItems()
        {
            return _items.GroupBy(x => x.ItemName, (key, g) => g.OrderByDescending(e => e.Cost).FirstOrDefault());
        }
        
        public void CreateItem(IItem item)
        {                        
            XDocument doc = XDocument.Load(itemDataStorePath);
            XElement itemElement = new XElement("item");

            itemElement.Add(new XElement("id", getNewItemId()));
            itemElement.Add(new XElement("cost", item.Cost));
            itemElement.Add(new XElement("item_name", item.ItemName));

            doc.Element("root").Add(itemElement);

            doc.Save(itemDataStorePath);
        }

        private int getNewItemId()
        {
            return _items.Max(x => x.Id) + 1;
        }

        public void UpdateItem(IItem item)
        {
            XDocument doc = XDocument.Load(itemDataStorePath);

            var existingItem = doc.Root.Elements("item")
            .Where(x => Convert.ToInt32(x.Element("id").Value) == item.Id)
            .Single();

            existingItem.Element("item_name").Value = item.ItemName;
            existingItem.Element("cost").Value = Convert.ToString(item.Cost);

            doc.Save(itemDataStorePath);
        }

        public void DeleteItem(int itemId)
        {
            XDocument doc = XDocument.Load(itemDataStorePath);

            doc.Root.Elements("item")
            .Where(x => Convert.ToInt32(x.Element("id").Value) == itemId)
            .Remove();

            doc.Save(itemDataStorePath);
        }

        private List<IItem> GetItemsFromDataStore()
        {
            var itemsFromDataStore = new List<IItem>();

            XDocument doc = XDocument.Load(itemDataStorePath);
            foreach (var itemNode in doc.Root.Elements("item"))
            {
                IItem item = _itemFactory.GetItem();

                item.Id = Convert.ToInt32(itemNode.Element("id").Value);
                item.Cost = Convert.ToInt32(itemNode.Element("cost").Value);
                item.ItemName = Convert.ToString(itemNode.Element("item_name").Value);

                itemsFromDataStore.Add(item);
            }

            return itemsFromDataStore;
        }

    }
}
