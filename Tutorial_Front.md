# 📝 Todo App - Vue 3 + TypeScript + Vite

Uma aplicação de lista de tarefas (Todo) moderna construída com Vue 3, TypeScript, Vite, Tailwind CSS e Pinia para gerenciamento de estado.

## 🚀 Tutorial Completo - Copie e Cole para Desenvolver

Este tutorial foi criado para pessoas que querem desenvolver este projeto copiando e colando código. Siga os passos abaixo para criar sua própria aplicação Todo do zero!

### 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **Node.js** (versão 18 ou superior) - [Download aqui](https://nodejs.org/)
- **npm** ou **yarn** (vem com o Node.js)
- **Editor de código** (recomendado: VS Code)

### 🛠️ Passo 1: Criando o Projeto

Abra o terminal e execute os comandos abaixo:

```bash
# Criar novo projeto Vue com Vite
npm create vue@latest meu-todo-app

# Navegar para a pasta do projeto
cd meu-todo-app

# Instalar dependências
npm install
```

### 📦 Passo 2: Instalando Dependências Necessárias

Cole e execute no terminal:

```bash
# Instalar Tailwind CSS
npm install -D tailwindcss@latest postcss@latest autoprefixer@latest

# Instalar Pinia para gerenciamento de estado
npm install pinia

# Instalar Axios para requisições HTTP
npm install axios

# Instalar dependências do Tailwind CSS
npm install @tailwindcss/postcss
```

### ⚙️ Passo 3: Configuração do Tailwind CSS

**3.1. Criar arquivo `tailwind.config.js`:**

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

**3.2. Criar arquivo `postcss.config.js`:**

```javascript
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
```

### 🎨 Passo 4: Configurando Estilos CSS

**4.1. Substitua o conteúdo de `src/assets/main.css`:**

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

* {
  box-sizing: border-box;
}
```

### 🔧 Passo 5: Configuração do Vite

**5.1. Substitua o conteúdo de `vite.config.ts`:**

```typescript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  }
})
```

### 📁 Passo 6: Estrutura de Tipos

**6.1. Crie a pasta `src/api` e o arquivo `src/api/itemType.ts`:**

```typescript
export interface itemTodo {
  id: string
  descricao: string
  completo: boolean
}
```

### 🏪 Passo 7: Gerenciamento de Estado com Pinia

**7.1. Crie a pasta `src/stores` e o arquivo `src/stores/index.ts`:**

```typescript
import { defineStore } from 'pinia'
import axios from 'axios'
import type { itemTodo } from '../api/itemType'

const entityTodoApi = 'http://localhost:5132/api/Todo'

export const useTodoStore = defineStore('todo', {
  state: () => ({
    todos: [] as itemTodo[],
  }),

  getters: {
    getTodos: (state) => state.todos,
  },

  actions: {
    async fetchTodos() {
      try {
        const response = await axios.get<itemTodo[]>(`${entityTodoApi}`)
        this.todos = response.data
      } catch (error) {
        console.error('There was an error!', error)
      }
    },

    async addTodo(data: { descricao: string }) {
      try {
        const response = await axios.post<itemTodo>(`${entityTodoApi}`, {
          ...data,
          completo: false,
        })
        this.todos.push(response.data)
      } catch (error) {
        console.error('Erro ao adicionar todo:', error)
      }
    },

    async updateTodo(id: string, data: Partial<itemTodo>) {
      try {
        const payload = Object.fromEntries(
          Object.entries(data).filter(([, v]) => v !== undefined),
        ) as Partial<itemTodo>

        if (Object.keys(payload).length === 0) return

        await axios.put<itemTodo>(`${entityTodoApi}/${id}`, payload)
        const todo = this.todos.find((todo) => todo.id === id)
        if (todo) Object.assign(todo, payload)
      } catch (error) {
        console.error('Erro ao atualizar todo:', error)
      }
    },

    async updateToggleTodo(id: string, completo: boolean) {
      try {
        await axios.patch(`${entityTodoApi}/${id}/toggle`, { completo })
        const todo = this.todos.find((todo) => todo.id === id)
        if (todo) todo.completo = completo
      } catch (error) {
        console.error('Erro ao atualizar toggle do todo:', error)
      }
    },

    async deleteTodo(id: string) {
      try {
        await axios.delete(`${entityTodoApi}/${id}`)
        this.todos = this.todos.filter((todo) => todo.id !== id)
      } catch (error) {
        console.error('Erro ao deletar todo:', error)
      }
    },
  },
})
```

### 🧩 Passo 8: Criando os Componentes

**8.1. Crie a pasta `src/components` e os seguintes arquivos:**

**`src/components/LogoLeds.vue`:**

```vue
<template>
  <div class="flex items-center justify-center mb-8">
    <img 
      src="@/assets/logo-leds.png" 
      alt="Logo LEDS" 
      class="h-16 w-auto"
    />
  </div>
</template>
```

**`src/components/SpinnerTodo.vue`:**

```vue
<template>
  <div class="flex justify-center items-center py-8">
    <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-white"></div>
  </div>
</template>
```

**`src/components/VazioTodo.vue`:**

```vue
<template>
  <div class="text-center py-8">
    <div class="text-gray-400 text-lg mb-2">📝</div>
    <p class="text-gray-400">Nenhuma tarefa encontrada</p>
    <p class="text-gray-500 text-sm">Adicione uma nova tarefa acima</p>
  </div>
</template>
```

**`src/components/FormTodo.vue`:**

```vue
<script setup lang="ts">
import { ref } from 'vue'
import { useTodoStore } from '@/stores'

const todoStore = useTodoStore()
const descricao = ref('')
const isSubmitting = ref(false)

const handleSubmit = async () => {
  if (!descricao.value.trim() || isSubmitting.value) return
  
  isSubmitting.value = true
  
  try {
    await todoStore.addTodo({ descricao: descricao.value.trim() })
    descricao.value = ''
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <form @submit.prevent="handleSubmit" class="w-full">
    <div class="flex gap-2">
      <input
        v-model="descricao"
        type="text"
        placeholder="Digite uma nova tarefa..."
        class="flex-1 px-4 py-3 rounded-lg border border-gray-600 bg-gray-800 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        :disabled="isSubmitting"
      />
      <button
        type="submit"
        :disabled="!descricao.trim() || isSubmitting"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
      >
        {{ isSubmitting ? '...' : 'Adicionar' }}
      </button>
    </div>
  </form>
</template>
```

**`src/components/ListaTodo.vue`:**

```vue
<script setup lang="ts">
import { useTodoStore } from '@/stores'
import ItemTodo from './ItemTodo.vue'

const todoStore = useTodoStore()
</script>

<template>
  <div class="space-y-2">
    <ItemTodo
      v-for="todo in todoStore.getTodos"
      :key="todo.id"
      :todo="todo"
    />
  </div>
</template>
```

**`src/components/ModalDelete.vue`:**

```vue
<script setup lang="ts">
import type { itemTodo } from '@/api/itemType'

interface Props {
  todo: itemTodo
}

defineProps<Props>()

const emit = defineEmits<{
  confirm: []
  cancel: []
}>()
</script>

<template>
  <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
    <div class="bg-gray-800 rounded-lg p-6 max-w-sm mx-4">
      <h3 class="text-lg font-semibold text-white mb-4">Confirmar Exclusão</h3>
      <p class="text-gray-300 mb-6">
        Tem certeza que deseja excluir a tarefa "{{ todo.descricao }}"?
      </p>
      <div class="flex gap-3 justify-end">
        <button
          @click="emit('cancel')"
          class="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors"
        >
          Cancelar
        </button>
        <button
          @click="emit('confirm')"
          class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors"
        >
          Excluir
        </button>
      </div>
    </div>
  </div>
</template>
```

**`src/components/ItemTodo.vue` (componente mais complexo):**

```vue
<script setup lang="ts">
import { ref } from 'vue'
import type { itemTodo } from '@/api/itemType'
import { useTodoStore } from '@/stores'
import ModalDelete from './ModalDelete.vue'

interface Props {
  todo: itemTodo
}

const props = defineProps<Props>()
const todoStore = useTodoStore()

const isEditing = ref(false)
const editText = ref(props.todo.descricao)
const showDeleteModal = ref(false)

const toggleComplete = async () => {
  await todoStore.updateToggleTodo(props.todo.id, !props.todo.completo)
}

const startEdit = () => {
  isEditing.value = true
  editText.value = props.todo.descricao
}

const saveEdit = async () => {
  if (editText.value.trim() && editText.value !== props.todo.descricao) {
    await todoStore.updateTodo(props.todo.id, { descricao: editText.value.trim() })
  }
  isEditing.value = false
}

const cancelEdit = () => {
  isEditing.value = false
  editText.value = props.todo.descricao
}

const confirmDelete = async () => {
  await todoStore.deleteTodo(props.todo.id)
  showDeleteModal.value = false
}
</script>

<template>
  <div class="flex items-center gap-3 p-4 bg-gray-800 rounded-lg border border-gray-700">
    <!-- Checkbox -->
    <button
      @click="toggleComplete"
      class="flex-shrink-0 w-5 h-5 rounded border-2 border-gray-600 flex items-center justify-center transition-colors"
      :class="todo.completo ? 'bg-green-600 border-green-600' : 'hover:border-gray-500'"
    >
      <svg v-if="todo.completo" class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
      </svg>
    </button>

    <!-- Conteúdo -->
    <div class="flex-1 min-w-0">
      <input
        v-if="isEditing"
        v-model="editText"
        @keyup.enter="saveEdit"
        @keyup.escape="cancelEdit"
        @blur="saveEdit"
        class="w-full bg-gray-700 text-white px-2 py-1 rounded border border-gray-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
        autofocus
      />
      <p
        v-else
        @dblclick="startEdit"
        class="text-white cursor-pointer"
        :class="todo.completo ? 'line-through text-gray-400' : ''"
      >
        {{ todo.descricao }}
      </p>
    </div>

    <!-- Ações -->
    <div class="flex gap-2">
      <button
        v-if="!isEditing"
        @click="startEdit"
        class="p-2 text-gray-400 hover:text-blue-400 transition-colors"
        title="Editar"
      >
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
        </svg>
      </button>
      
      <button
        @click="showDeleteModal = true"
        class="p-2 text-gray-400 hover:text-red-400 transition-colors"
        title="Excluir"
      >
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z" clip-rule="evenodd" />
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
        </svg>
      </button>
    </div>

    <!-- Modal de Confirmação -->
    <ModalDelete
      v-if="showDeleteModal"
      :todo="todo"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>
```

### 🏠 Passo 9: Configurando os Arquivos Principais

**9.1. Substitua o conteúdo de `src/main.ts`:**

```typescript
import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

app.mount('#app')
```

**9.2. Substitua o conteúdo de `src/App.vue`:**

```vue
<script setup lang="ts">
import SpinnerTodo from './components/SpinnerTodo.vue'
import FormTodo from './components/FormTodo.vue'
import ListaTodo from './components/ListaTodo.vue'
import VazioTodo from './components/VazioTodo.vue'
import LogoLeds from './components/LogoLeds.vue'

import './assets/main.css'
import { onMounted, ref } from 'vue'
import { useTodoStore } from '@/stores'

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
```

### 🎯 Passo 10: Executando o Projeto

```bash
npm run dev
```

Abra o navegador em: `http://localhost:5173`

### 🚀 Funcionalidades Implementadas

- ✅ **Adicionar tarefas** - Digite e clique em "Adicionar"
- ✅ **Marcar como concluída** - Clique no checkbox
- ✅ **Editar tarefas** - Duplo clique na tarefa ou clique no ícone de edição
- ✅ **Excluir tarefas** - Clique no ícone de lixeira e confirme
- ✅ **Interface responsiva** - Funciona em desktop e mobile
- ✅ **Loading state** - Spinner durante carregamento
- ✅ **Estado vazio** - Mensagem quando não há tarefas
- ✅ **Gerenciamento de estado** - Pinia para controle global
- ✅ **Integração com API** - Axios para requisições HTTP

### 🔗 API Backend

O projeto está configurado para se conectar com uma API em `http://localhost:5132/api/Todo`.

**Endpoints esperados:**
- `GET /api/Todo` - Listar todos
- `POST /api/Todo` - Criar novo todo
- `PUT /api/Todo/{id}` - Atualizar todo
- `PATCH /api/Todo/{id}/toggle` - Alternar status
- `DELETE /api/Todo/{id}` - Excluir todo

### 📚 Recursos de Aprendizado

- [Vue 3 Documentation](https://vuejs.org/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Tailwind CSS Docs](https://tailwindcss.com/docs)
- [Pinia Documentation](https://pinia.vuejs.org/)
- [Vite Guide](https://vitejs.dev/guide/)

---

**🎉 Parabéns! Você criou uma aplicação Todo completa com Vue 3, TypeScript e Tailwind CSS!**

*Desenvolvido com ❤️ para aprendizado através de copiar e colar.*

