<script setup lang="ts">
import { defineProps, ref, nextTick } from 'vue'
import ModalDelete from './ModalDelete.vue'
import IconButton from './IconButton.vue'
import CheckIcon from './CheckIcon.vue'
import EditIcon from './EditIcon.vue'
import DeleteIcon from './DeleteIcon.vue'
import type { itemTodo } from '@/api/itemType'
import { useTodoStore } from '@/stores/useTodoStore'

const props = defineProps<{
  todo: itemTodo
}>()

const todoStore = useTodoStore()
const isCompleted = ref(props.todo.completo)
const showModal = ref(false)
const isEditing = ref(false)
const descricaoEdit = ref(props.todo.descricao)
const inputRef = ref<HTMLInputElement | null>(null)

function onCheckClick() {
  isCompleted.value = !isCompleted.value
  todoStore.updateToggleTodo(props.todo.id, isCompleted.value)
}

function onDeleteClick() {
  showModal.value = true
}

function onUpdateClick() {
  if (!isEditing.value) {
    descricaoEdit.value = props.todo.descricao
    isEditing.value = true
    nextTick(() => {
      inputRef.value?.focus()
    })
  } else {
    isEditing.value = false
    descricaoEdit.value = props.todo.descricao
  }
}

async function onSaveClick() {
  const newDesc = descricaoEdit.value.trim()
  if (!newDesc) return
  if (newDesc === props.todo.descricao) {
    isEditing.value = false
    return
  }
  await todoStore.updateTodo(props.todo.id, { descricao: newDesc })
  isEditing.value = false
}

function handleExcluir() {
  todoStore.deleteTodo(props.todo.id)
  showModal.value = false
}

function handleCancelar() {
  showModal.value = false
}
</script>

<template>
  <div class="bg-[#cacdda] rounded-lg relative">
    <div class="flex items-center px-4 py-3 border-b border-gray-400 last:border-b-0">
      <div class="flex items-center justify-center mr-2">
        <IconButton
          :extraClass="{ 'text-gray-600': !isCompleted, 'text-green-400': isCompleted }"
          @click="onCheckClick"
        >
          <CheckIcon class="w-5 h-5" />
        </IconButton>
      </div>

      <div class="w-full">
        <template v-if="isEditing">
          <div class="flex items-center gap-2">
            <input
              ref="inputRef"
              v-model="descricaoEdit"
              type="text"
              class="w-full px-2 py-1 border rounded focus:outline-none"
            />
            <button
              class="bg-[#009439] text-white px-3 py-1 rounded font-semibold hover:cursor-pointer"
              @click="onSaveClick"
            >
              Salvar
            </button>
          </div>
        </template>
        <template v-else>
          <input
            type="text"
            placeholder="Digite a sua tarefa"
            :value="props.todo.descricao"
            readonly
            class="placeholder-gray-500 text-gray-700 font-normal focus:outline-none block w-full appearance-none leading-normal mr-3"
          />
        </template>
      </div>

      <div class="ml-auto flex items-center justify-center">
        <IconButton extraClass="group ml-3" @click="onUpdateClick">
          <EditIcon class="h-4 w-4 text-gray-500 group-hover:text-blue-500 transition-colors duration-200" />
        </IconButton>
      </div>

      <div class="ml-auto flex items-center justify-center">
        <IconButton extraClass="group ml-3" @click="onDeleteClick">
          <DeleteIcon class="h-4 w-4 text-gray-500 group-hover:text-[#de0025] transition-colors duration-200" />
        </IconButton>
      </div>
    </div>
    <ModalDelete
      v-if="showModal"
      :mensagem="`Deseja realmente excluir a tarefa: '${props.todo.descricao}'?`"
      :id="props.todo.id"
      @excluir="handleExcluir"
      @cancelar="handleCancelar"
    />
  </div>
</template>
