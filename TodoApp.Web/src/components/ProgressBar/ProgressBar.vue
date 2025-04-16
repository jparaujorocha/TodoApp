<template>
  <div class="progress-bar-container" :title="`${percentage.toFixed(1)}%`">
    <div class="progress-bar-track">
      <div class="progress-bar-fill" :style="{ width: clampedPercentage + '%' }"></div>
    </div>
    <span class="progress-bar-text">{{ clampedPercentage.toFixed(1) }}%</span>
  </div>
</template>

<script setup>
import { defineProps, computed } from 'vue';

const props = defineProps({
  percentage: {
    type: Number,
    required: true,
    default: 0
  },
});

const clampedPercentage = computed(() => {
    return Math.max(0, Math.min(100, props.percentage));
});
</script>

<style scoped src="./ProgressBar.css"></style>