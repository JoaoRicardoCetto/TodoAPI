<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
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
