using ItemRepository.Interfaces;
using System;

namespace ItemRepository.Models
{
    public class ConcreteItem : IItem
    {
        public int Id { get; set; }

        public string ItemName { get; set; }
        
        public int Cost { get; set; }
    }
}
