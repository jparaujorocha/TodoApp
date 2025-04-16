import { ref, onMounted } from 'vue';
import todoService from '@/services/todoService';
import { getLocalDateTimeStringForInput } from '@/utils/formatters';

export function useTodoList() {
    const items = ref([]);
    const categories = ref([]);
    const loading = ref(true);
    const error = ref(null);

    const showAddItemModal = ref(false);
    const showEditModal = ref(false);
    const showProgressModal = ref(false);

    const newItem = ref({ title: '', description: '', category: '' });
    const editItemData = ref({ id: null, title: '', description: '', category: '' });
    const progressItemId = ref(null);
    const newProgression = ref({ date: getLocalDateTimeStringForInput(), percent: null });

    const clearError = () => {
        error.value = null;
    };

    const fetchItems = async () => {
      loading.value = true;
      clearError();
      try {
        const response = await todoService.getAllItems();
        items.value = response.data.sort((a,b) => a.id - b.id);
      } catch (err) {
        error.value = `Failed to fetch items: ${err.message}`;
        console.error("Fetch Items Error:", err);
      } finally {
        loading.value = false;
      }
    };

    const fetchCategories = async () => {
      try {
        const response = await todoService.getCategories();
        categories.value = response.data;
      } catch (err) {
        console.error("Failed to fetch categories:", err);
        error.value = `Failed to fetch categories: ${err.message}`;
      }
    };

    const addItem = async () => {
        if (!newItem.value.title || !newItem.value.category) {
            error.value = "Title and Category are required fields.";
            return;
        }
        clearError();
        try {
          await todoService.addItem(newItem.value);
          showAddItemModal.value = false;
          newItem.value = { title: '', description: '', category: '' };
          await fetchItems();
        } catch (err) {
          error.value = `Failed to add item: ${err.message}`;
          console.error("Add Item Error:", err);
        }
    };

    const openEditModal = (item) => {
        const latestProgress = item.progressions?.length > 0
            ? item.progressions.reduce((maxProg, current) => Math.max(maxProg, current.accumulatedPercent), 0)
            : 0;

        if(latestProgress > 50) {
             error.value = "Cannot edit an item with more than 50% progress.";
             setTimeout(clearError, 5000);
             return;
        }
      editItemData.value = {
          id: item.id,
          title: item.title,
          description: item.description,
          category: item.category
       };
      showEditModal.value = true;
    };

    const updateItem = async () => {
        clearError();
        if (!editItemData.value.id) {
            error.value = "No item selected for update.";
            return;
        }
        try {
          const payload = {
              title: editItemData.value.title,
              description: editItemData.value.description,
              category: editItemData.value.category
          };
          await todoService.updateItem(editItemData.value.id, payload);
          showEditModal.value = false;
          await fetchItems();
        } catch (err) {
          error.value = `Failed to update item: ${err.message}`;
          console.error("Update Item Error:", err);
        }
    };

     const confirmDelete = (id) => {
        const itemToDelete = items.value.find(i => i.id === id);
        if (!itemToDelete) return;

         const latestProgress = itemToDelete.progressions?.length > 0
            ? itemToDelete.progressions.reduce((maxProg, current) => Math.max(maxProg, current.accumulatedPercent), 0)
            : 0;

        if (latestProgress > 50) {
             error.value = "Cannot delete an item with more than 50% progress.";
             setTimeout(clearError, 5000);
             return;
        }

      if (confirm(`Are you sure you want to delete item #${id}? This action cannot be undone.`)) {
        deleteItem(id);
      }
    };


    const deleteItem = async (id) => {
        clearError();
        try {
          await todoService.deleteItem(id);
          await fetchItems();
        } catch (err) {
          error.value = `Failed to delete item: ${err.message}`;
          console.error("Delete Item Error:", err);
        }
    };

    const openProgressModal = (id) => {
        const item = items.value.find(i => i.id === id);
        if (item?.isCompleted) {
            error.value = "Cannot add progress to a completed item.";
            setTimeout(clearError, 5000);
            return;
        }
        progressItemId.value = id;
        newProgression.value = { date: getLocalDateTimeStringForInput(), percent: null };
        showProgressModal.value = true;
    };

    const registerProgress = async () => {
        if(!newProgression.value.date || newProgression.value.percent === null || newProgression.value.percent <= 0 || newProgression.value.percent > 100){
            error.value = 'Please provide a valid future date/time and a percentage value between 0.1 and 100.';
            return;
        }
         clearError();
        if (!progressItemId.value) {
             error.value = "No item selected for progress registration.";
             return;
        }
        try {
            const progressionData = {
                date: new Date(newProgression.value.date).toISOString(),
                percent: parseFloat(newProgression.value.percent)
            };
            await todoService.registerProgression(progressItemId.value, progressionData);
            showProgressModal.value = false;
            await fetchItems();
        } catch (err) {
             error.value = `Failed to register progression: ${err.message}`;
             console.error("Register Progression Error:", err);
        }
    };

    onMounted(async () => {
      await fetchCategories();
      await fetchItems();
    });

    return {
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
    };
}