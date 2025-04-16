<template>
  <li :class="['todo-item', { completed: item.isCompleted }]">
    <div class="item-content">
      <h3>
        {{ item.title }}
        <span class="category-tag">({{ item.category }})</span>
      </h3>
      <p v-if="item.description" class="description">{{ item.description }}</p>
      <div v-if="item.progressions && item.progressions.length > 0" class="progress-section">
         <ProgressBar :percentage="latestAccumulatedPercentage" />
         <details class="progress-details">
           <summary>View Progress History</summary>
           <ul>
             <li v-for="(prog, index) in sortedProgressions" :key="index" class="progress-entry">
               <span class="progress-date">{{ formatDate(prog.date) }}</span>:
               <span class="progress-percent">{{ prog.percent }}%</span>
               (Total: {{ prog.accumulatedPercent.toFixed(1) }}%)
               <ProgressBar :percentage="prog.accumulatedPercent" />
             </li>
           </ul>
         </details>
      </div>
      <p v-else class="no-progress">No progress registered yet.</p>
    </div>
    <div class="item-actions">
      <button @click="viewDetails" title="View Details">👁️</button>
      <button @click="editItem" :disabled="!canUpdate" title="Edit Item">✏️</button>
      <button @click="openProgressModal" :disabled="item.isCompleted" title="Register Progress">📊</button>
      <button @click="deleteItem" :disabled="!canDelete" title="Delete Item">🗑️</button>
    </div>
  </li>
</template>

<script setup>
import { defineProps, defineEmits, computed } from 'vue';
import { useRouter } from 'vue-router';
import ProgressBar from '@/components/ProgressBar/ProgressBar.vue'; // Updated path
import { formatLocalDateTime } from '@/utils/formatters';

const props = defineProps({
  item: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['edit', 'delete', 'registerProgress']);
const router = useRouter();

const sortedProgressions = computed(() => {
    return Array.isArray(props.item.progressions)
        ? [...props.item.progressions].sort((a, b) => new Date(a.date) - new Date(b.date))
        : [];
});

const latestAccumulatedPercentage = computed(() => {
  if (!sortedProgressions.value || sortedProgressions.value.length === 0) {
    return 0;
  }
  return sortedProgressions.value[sortedProgressions.value.length - 1].accumulatedPercent;
});

const canUpdate = computed(() => {
    return latestAccumulatedPercentage.value <= 50;
});

const canDelete = computed(() => {
    return latestAccumulatedPercentage.value <= 50;
});

const editItem = () => {
  if (canUpdate.value) {
      emit('edit', props.item);
  }
};

const deleteItem = () => {
    if(canDelete.value) {
        emit('delete', props.item.id);
    }
};

const openProgressModal = () => {
    if (!props.item.isCompleted) {
        emit('registerProgress', props.item.id);
    }
};

const viewDetails = () => {
    router.push({ name: 'TodoDetail', params: { id: props.item.id } });
};

const formatDate = (dateString) => {
    return formatLocalDateTime(dateString);
};
</script>

<style scoped src="./TodoItem.css"></style>