<script setup lang="ts">
import { ref } from 'vue'
import { useTodoStore } from '@/stores'

const descricao = ref('')
const todoStore = useTodoStore()

async function addNovaTarefa() {
  try {
    if (descricao.value.trim()) {
      await todoStore.addTodo({ descricao: descricao.value })
      descricao.value = ''
      console.log('Todo adicionado com sucesso!')
    }
  } catch (error) {
    console.error('Erro ao adicionar todo:', error)
  }
}
</script>

<template>
  <form
    @submit.prevent="addNovaTarefa"
    class="flex items-center px-4 bg-gray-900 h-15 rounded-sm border-l-2 border-green-400 mb-3"
  >
    <input
      v-model="descricao"
      placeholder="Adicione um novo item ..."
      type="text"
      class="bg-gray-900 placeholder-gray-500 text-gray-500 font-light focus:outline-none block w-full appearance-none leading-normal py-3 pr-3"
    />

    <button class="text-green-400 text-xs font-semibold focus:outline-none" type="submit">
      ADICIONAR
    </button>
  </form>
</template>
