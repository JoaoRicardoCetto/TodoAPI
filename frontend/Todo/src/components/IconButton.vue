<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  // allows passing string, array or object class bindings
  extraClass: { type: [String, Object, Array], default: '' },
  title: { type: String, default: '' },
})

const emit = defineEmits(['click'])

function onClick(e: Event) {
  emit('click', e)
}
</script>

<template>
  <button
    :class="['focus:outline-none hover:cursor-pointer', extraClass]"
    @click="onClick"
    :title="title"
  >
    <slot />
  </button>
</template>

<style scoped>
/* keep styles minimal; visuals are controlled by passed classes */
</style>
