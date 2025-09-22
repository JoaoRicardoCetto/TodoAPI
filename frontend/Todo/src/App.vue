<script setup lang="ts">
import SpinnerTodo from './components/SpinnerTodo.vue'
import FormTodo from './components/FormTodo.vue'
import ListaTodo from './components/ListaTodo.vue'
import VazioTodo from './components/VazioTodo.vue'
import './assets/main.css'
import { onMounted, ref } from 'vue'
import { useTodoStore } from '@/stores'

const todoStore = useTodoStore()
const isLoading = ref(false)

onMounted(async () => {
  isLoading.value = true

  await todoStore.fetchTodos()
  const storeTodos = todoStore.getTodos
  console.log('TESTEEE', storeTodos)
  setTimeout(() => {
    isLoading.value = false
  }, 3000)
})
</script>

<template>
  <body class="bg-gray-800">
    <div class="px-3 py-10 md:px-10">
      <div class="w-full sm:w-2/3 lg:w-3/2 mx-auto">
        <SpinnerTodo v-if="isLoading" />
        <template v-else>
          <FormTodo />
          <ListaTodo />
          <VazioTodo v-if="!todoStore.getTodos.length" />
        </template>
      </div>
    </div>
  </body>
</template>
