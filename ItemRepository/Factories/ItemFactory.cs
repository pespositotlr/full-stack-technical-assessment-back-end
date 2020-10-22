using ItemRepository.Interfaces;
using ItemRepository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemRepository.Factories
{
    public class ItemFactory
    {

        private readonly IConfiguration _config;

        public ItemFactory(IConfiguration config)
        {
            _config = config;
        }

        public IItem GetItem()
        {
            int itemType = Convert.ToInt32(_config["ItemType"]);

            switch (itemType)
            {
                case 1:
                    return new ConcreteItem();
                default:
                    return new ConcreteItem();
            }
            
        }
    }
}
