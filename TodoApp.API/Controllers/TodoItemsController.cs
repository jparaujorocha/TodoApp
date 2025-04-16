using System;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.API.Controllers
{
    public class TodoItemsController : ApiControllerBase
    {
        private readonly ITodoListService _todoListService;

        public TodoItemsController(ITodoListService todoListService)
        {
            _todoListService = todoListService;
        }

        [HttpGet]
        public IActionResult GetAllItems()
        {
            try
            {
                var items = _todoListService.GetAllItems();
                return Success(items);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetItemById(int id)
        {
            try
            {
                var item = _todoListService.GetItemById(id);
                return Success(item);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _todoListService.GetAllCategories();
                return Success(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] TodoItemRequest request)
        {
            try
            {
                var createdItem = _todoListService.AddItemAndReturn(request);
                return Created(createdItem, nameof(GetItemById), new { id = createdItem.Id });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, [FromBody] TodoItemRequest request)
        {
            try
            {
                var updatedItem = _todoListService.UpdateItemAndReturn(id, request);
                return Success(updatedItem);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveItem(int id)
        {
            try
            {
                _todoListService.RemoveItem(id);
                return Success();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/progressions")]
        public IActionResult RegisterProgression(int id, [FromBody] ProgressionRequest request)
        {
            try
            {
                var updatedItem = _todoListService.RegisterProgressionAndReturn(id, request);
                return Success(updatedItem);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("print")]
        public IActionResult PrintItems()
        {
            try
            {
                string output = _todoListService.GetPrintOutput();
                return Success(new { output });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}