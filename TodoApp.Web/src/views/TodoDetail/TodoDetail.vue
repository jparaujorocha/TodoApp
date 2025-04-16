<template>
  <div class="todo-detail-view">
    <router-link to="/" class="back-link">&larr; Back to List</router-link>

    <div v-if="loading" class="loading-indicator">Loading details...</div>

    <div v-if="error" class="error-message">
      <p>⚠️ Could not load item details: {{ error }}</p>
    </div>

    <div v-if="item && !loading" class="item-details-card">
      <h2>{{ item.title }} <span class="category-chip">{{ item.category }}</span></h2>

      <div class="detail-section">
        <strong>ID:</strong> {{ item.id }}
      </div>

      <div class="detail-section">
        <strong>Description:</strong>
        <p class="description-text">{{ item.description || 'No description provided.' }}</p>
      </div>

      <div class="detail-section">
        <strong>Status:</strong>
        <span :class="['status-badge', item.isCompleted ? 'status-completed' : 'status-incomplete']">
          {{ item.isCompleted ? 'Completed' : 'In Progress' }}
        </span>
      </div>

      <div class="detail-section progress-section">
        <strong>Overall Progress:</strong>
        <ProgressBar :percentage="latestAccumulatedPercentage" />
      </div>

      <div class="detail-section" v-if="sortedProgressions.length > 0">
        <strong>Progress History:</strong>
        <ul class="progress-history-list">
            <li v-for="(prog, index) in sortedProgressions" :key="index" class="progress-history-item">
                <span class="history-date">{{ formatDate(prog.date) }}</span>
                <span class="history-percent"> Added: +{{ prog.percent }}% </span>
                <span class="history-total">(Total: {{ prog.accumulatedPercent.toFixed(1) }}%)</span>
                <ProgressBar :percentage="prog.accumulatedPercent" />
            </li>
        </ul>
      </div>
       <p v-else class="no-progress-message">No progress has been registered for this item yet.</p>

    </div>
  </div>
</template>

<script setup>
import ProgressBar from '@/components/ProgressBar/ProgressBar.vue'; // Updated path
import { useTodoDetail } from '@/composables/useTodoDetail';

const {
    item,
    loading,
    error,
    sortedProgressions,
    latestAccumulatedPercentage,
    formatDate
} = useTodoDetail();

</script>

<style scoped src="./TodoDetail.css"></style>