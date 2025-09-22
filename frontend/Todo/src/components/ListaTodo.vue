<script setup lang="ts">
import ItemTodo from './ItemTodo.vue'
import { useTodoStore } from '@/stores'

const todoStore = useTodoStore()
</script>

<template>
  <div class="space-y-2">
    <ItemTodo v-for="todo in todoStore.getTodos" :key="todo.id" :todo="todo" />
    <div class="bg-gray-300 rounded-sm"></div>
  </div>
</template>
