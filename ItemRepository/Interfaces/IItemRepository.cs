using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemRepository.Interfaces
{
    public interface IItemRepository
    {
        IEnumerable<IItem> GetItems();
        IEnumerable<IItem> GetItemsByItemName(string itemName);
        IItem GetItemById(int id);
        IEnumerable<IItem> GetMaxPricedItems();
        int CreateItem(IItem item);
        void UpdateItem(IItem item);
        void DeleteItem(int itemId);
    }
}
