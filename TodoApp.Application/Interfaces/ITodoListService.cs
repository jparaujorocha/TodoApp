using System;
using System.Collections.Generic;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Interfaces
{
    public interface ITodoListService
    {
        List<TodoItemResponse> GetAllItems();
        TodoItemResponse GetItemById(int id);
        List<string> GetAllCategories();
        int GetNextId();
        int AddItem(TodoItemRequest request);
        void UpdateItem(int id, TodoItemRequest request);
        void RegisterProgression(int id, ProgressionRequest request);
        string GetPrintOutput();
        TodoItemResponse RegisterProgressionAndReturn(int id, ProgressionRequest request);

        void RemoveItem(int id);
        TodoItemResponse AddItemAndReturn(TodoItemRequest request);
        TodoItemResponse UpdateItemAndReturn(int id, TodoItemRequest request);
    }
}