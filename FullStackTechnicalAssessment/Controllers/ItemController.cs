using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullStackTechnicalAssessment.ViewModels;
using ItemRepository;
using ItemRepository.Factories;
using ItemRepository.Interfaces;
using ItemRepository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FullStackTechnicalAssessment.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IItemRepository _itemRepository;
        private ItemFactory _itemFactory;

        public ItemController(IConfiguration config, IItemRepository itemRepository)
        {
            _config = config;
            _itemRepository = itemRepository;
            _itemFactory = new ItemFactory(_config);
        }

        /// <summary>
        /// Gets all items in the data store
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetItems")]
        public IActionResult GetItems()
        {
            var items = _itemRepository.GetItems();

            if (items.Any())
            {
                return Ok(items);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets one item from the data store
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetItem/{itemId}")]
        public IActionResult GetItem(int itemId)
        {
            var item = _itemRepository.GetItemById(itemId);

            if (item != null)
            {
                return Ok(item);
            }

            return NotFound();
        }

        /// <summary>
        /// Returns a list of max prices of items grouped by item name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetMaxPricedItems")]
        public IActionResult GetMaxPricedItems()
        {
            var items = _itemRepository.GetItems();

            if (items.Any())
            {
                return Ok(_itemRepository.GetMaxPricedItems());
            }

            return NotFound();
        }

        /// <summary>
        /// Takes as an input an item name and returns the max price for it
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetMaxPriceByItemName/{itemName}")]
        public IActionResult GetMaxPriceByItemName(string itemName)
        {
            var items = _itemRepository.GetItemsByItemName(itemName);

            if (items.Any())
            {
                return Ok(items.Select(x => x.Cost).Max());
            }

            return NotFound();
        }

        /// <summary>
        /// Allows the user to create data in the data store
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IItem> CreateItem([FromBody]ItemCreateRequest item)
        {

            if (String.IsNullOrEmpty(item.ItemName))
                return BadRequest();

            IItem newItem = _itemFactory.GetItem();
            newItem.ItemName = item.ItemName;
            newItem.Cost = item.Cost;

            _itemRepository.CreateItem(newItem);

            return Ok();
        }

        /// <summary>
        /// Allows the user to update data in the data store
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IItem> UpdateItem([FromBody]ItemViewModel item)
        {
            var originalItem = _itemRepository.GetItemById(item.Id);

            if (originalItem != null)
            {
                IItem newItem = _itemFactory.GetItem();
                newItem.Id = item.Id;
                newItem.ItemName = item.ItemName;
                newItem.Cost = item.Cost;

                _itemRepository.UpdateItem(newItem);
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Allows the user to delete data in the data store
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("DeleteItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IItem> DeleteItem([FromBody]int itemId)
        {
            var originalItem = _itemRepository.GetItemById(itemId);

            if (originalItem != null)
            {
                _itemRepository.DeleteItem(itemId);
                return Ok();
            }

            return BadRequest();
        }
    }
}
