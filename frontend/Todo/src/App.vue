<script setup lang="ts">
import SpinnerTodo from './components/SpinnerTodo.vue'
import FormTodo from './components/FormTodo.vue'
import ListaTodo from './components/ListaTodo.vue'
import VazioTodo from './components/VazioTodo.vue'
import LogoLeds from './components/LogoLeds.vue'

import './assets/main.css'
import { onMounted, ref } from 'vue'
import { useTodoStore } from '@/stores/useTodoStore'

const todoStore = useTodoStore()
const isLoading = ref(false)

onMounted(async () => {
  isLoading.value = true

  await todoStore.fetchTodos()

  setTimeout(() => {
    isLoading.value = false
  }, 3000)
})
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-[#211b15] px-4">
    <LogoLeds />
    <div class="w-full max-w-md flex flex-col items-center">
      <FormTodo class="w-full" />
      <div class="w-full mt-2">
        <template v-if="isLoading">
          <SpinnerTodo />
        </template>
        <template v-else>
          <ListaTodo v-if="todoStore.getTodos.length > 0" />
          <VazioTodo v-else />
        </template>
      </div>
    </div>
  </div>
</template>
