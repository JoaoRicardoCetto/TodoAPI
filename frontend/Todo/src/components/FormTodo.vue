<script setup lang="ts">
import { ref } from 'vue'
import { useTodoStore } from '@/stores/useTodoStore'

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
  <form @submit.prevent="addNovaTarefa" class="flex items-center h-15 rounded-sm mb-3">
    <div class="flex flex-row items-center bg-[#cacdda] rounded-lg mb-2 px-2 py-2 w-full gap-2">
      <input
        v-model="descricao"
        placeholder="Adicione uma nova tarefa aqui ..."
        type="text"
        class="flex-1 min-w-0 text-[#000000] text-base placeholder-[#211b15] px-2 py-1 rounded-sm appearance-none bg-transparent outline-none"
      />
      <button
        class="w-fit text-[#009439] px-2 py-1 text-base font-semibold hover:cursor-pointer appearance-none rounded-l-lg rounded-r-lg border-l-4 border-l-[#34236e] border-r-4 border-r-[#de0025]"
        type="submit"
      >
        ADICIONAR
      </button>
    </div>
  </form>
</template>
