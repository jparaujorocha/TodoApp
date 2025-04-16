import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import todoService from '@/services/todoService';
import { formatLocalDateTime } from '@/utils/formatters';

export function useTodoDetail() {
    const route = useRoute();
    const router = useRouter();
    const item = ref(null);
    const loading = ref(true);
    const error = ref(null);

    const fetchItem = async () => {
      loading.value = true;
      error.value = null;
      try {
        const id = parseInt(route.params.id);
        if (isNaN(id)) {
            throw new Error("Invalid Todo Item ID in URL.");
        }
        const response = await todoService.getItemById(id);
        if (!response.data) {
            throw new Error(`Item with ID ${id} not found.`);
        }
        item.value = response.data;
      } catch (err) {
         error.value = err.message || "An unknown error occurred while fetching item details.";
         console.error("Fetch Item Detail Error:", err);
      } finally {
        loading.value = false;
      }
    };

    const sortedProgressions = computed(() => {
        if (!item.value || !Array.isArray(item.value.progressions)) return [];
        return [...item.value.progressions].sort((a, b) => new Date(a.date) - new Date(b.date));
    });

    const latestAccumulatedPercentage = computed(() => {
      if (!sortedProgressions.value || sortedProgressions.value.length === 0) {
        return 0;
      }
      return sortedProgressions.value[sortedProgressions.value.length - 1].accumulatedPercent;
    });

    const formatDate = (dateString) => {
        return formatLocalDateTime(dateString);
    };

    onMounted(() => {
      fetchItem();
    });

    return {
        item,
        loading,
        error,
        sortedProgressions,
        latestAccumulatedPercentage,
        formatDate
    };
}