<script setup lang="ts">
import FormTodo from './components/FormTodo.vue'
import ListaTodo from './components/ListaTodo.vue'

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
  <div class="min-h-screen flex flex-col items-center justify-center bg-[#e2e3eb] px-4">
    <div class="bg-[#e2e3eb] mb-8 flex justify-center w-fit rounded-3xl">
      <img src="@/assets/logo-leds.png" alt="Logo" class="h-16 w-auto" />
    </div>
    <div class="w-full max-w-md flex flex-col items-center">
      <FormTodo class="w-full" />
      <div class="w-full mt-2">
        <template v-if="isLoading">
          <div class="flex justify-center items-center h-24">
            <img src="@/assets/spinner.svg" alt="Loading..." class="w-8 h-8 animate-spin" />
          </div>
        </template>
        <template v-else>
          <ListaTodo v-if="todoStore.getTodos.length > 0" />
          <div v-else class="text-center font-semibold text-lg text-[#211b15]">
            Você ainda não adicionou nenhuma tarefa.
          </div>
        </template>
      </div>
    </div>
  </div>
</template>
