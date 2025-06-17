<script setup lang="ts">
defineProps({
  loading: {
    type: Boolean,
    default: false
  }
})
</script>


<template>
  <div>
    <a-spin :spinning="loading" size="small">
      <slot></slot>
    </a-spin>
  </div>
</template>

