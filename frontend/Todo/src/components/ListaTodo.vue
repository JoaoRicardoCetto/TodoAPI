<script setup lang="ts">
import ItemToDo from './ItemTodo.vue'
import { useTodoStore } from '@/stores'

const todoStore = useTodoStore()
</script>

<template>
  <div class="space-y-2">
    <ItemToDo v-for="todo in todoStore.getTodos" :key="todo.id" :todo="todo" />
  </div>
</template>
