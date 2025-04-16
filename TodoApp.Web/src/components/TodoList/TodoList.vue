<template>
  <div class="todo-list-container">
    <div class="list-header">
      <h2>Current Tasks</h2>
      <button @click="showAddItemModal = true">✨ Add New Item</button>
    </div>

    <div v-if="loading" class="loading-indicator">Loading items...</div>

    <div v-if="error" class="error-message">
        <p>⚠️ {{ error }}</p>
        <button @click="clearError">Dismiss</button>
    </div>

    <ul v-if="!loading && items.length > 0" class="todo-list">
      <TodoItem
        v-for="item in items"
        :key="item.id"
        :item="item"
        @edit="openEditModal"
        @delete="confirmDelete"
        @registerProgress="openProgressModal"
      />
    </ul>
    <p v-if="!loading && items.length === 0 && !error" class="no-items-message">No items yet. Add one!</p>

    <div v-if="showAddItemModal" class="modal-overlay" @click.self="showAddItemModal = false">
        <div class="modal-content">
          <h3>Add New Todo Item</h3>
          <form @submit.prevent="addItem">
              <label for="add-title">Title:</label>
              <input id="add-title" type="text" v-model="newItem.title" placeholder="Title" required>
              <label for="add-desc">Description:</label>
              <textarea id="add-desc" v-model="newItem.description" placeholder="Description (optional)"></textarea>
              <label for="add-cat">Category:</label>
              <select id="add-cat" v-model="newItem.category" required>
                  <option disabled value="">Select Category</option>
                  <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
              </select>
              <div class="modal-actions">
                  <button type="submit">Add Item</button>
                  <button type="button" @click="showAddItemModal = false" class="cancel-button">Cancel</button>
              </div>
          </form>
       </div>
    </div>

    <div v-if="showEditModal" class="modal-overlay" @click.self="showEditModal = false">
       <div class="modal-content">
         <h3>Edit Todo Item (ID: {{ editItemData.id }})</h3>
          <form @submit.prevent="updateItem">
              <label for="edit-title">Title:</label>
              <input id="edit-title" type="text" v-model="editItemData.title" placeholder="Title" required disabled>
              <label for="edit-desc">Description:</label>
              <textarea id="edit-desc" v-model="editItemData.description" placeholder="Description"></textarea>
              <label for="edit-cat">Category:</label>
              <select id="edit-cat" v-model="editItemData.category" required disabled>
                  <option disabled value="">Select Category</option>
                  <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
              </select>
              <div class="modal-actions">
                <button type="submit">Update Description</button>
                <button type="button" @click="showEditModal = false" class="cancel-button">Cancel</button>
              </div>
          </form>
       </div>
    </div>

     <div v-if="showProgressModal" class="modal-overlay" @click.self="showProgressModal = false">
        <div class="modal-content">
          <h3>Register Progress for Item #{{ progressItemId }}</h3>
          <form @submit.prevent="registerProgress">
              <label for="progDate">Date & Time:</label>
              <input id="progDate" type="datetime-local" v-model="newProgression.date" required>
              <label for="progPercent">Percentage (0.1 - 100):</label>
              <input id="progPercent" type="number" v-model.number="newProgression.percent" placeholder="Percent (0.1-100)" step="0.1" min="0.1" max="100" required>
               <div class="modal-actions">
                 <button type="submit">Register</button>
                 <button type="button" @click="showProgressModal = false" class="cancel-button">Cancel</button>
               </div>
          </form>
        </div>
    </div>

  </div>
</template>

<script setup>
import TodoItem from '@/components/TodoItem/TodoItem.vue'; // Updated path
import { useTodoList } from '@/composables/useTodoList';

const {
    items,
    categories,
    loading,
    error,
    showAddItemModal,
    showEditModal,
    showProgressModal,
    newItem,
    editItemData,
    progressItemId,
    newProgression,
    clearError,
    addItem,
    openEditModal,
    updateItem,
    confirmDelete,
    openProgressModal,
    registerProgress
} = useTodoList();

</script>

<style scoped src="./TodoList.css"></style>