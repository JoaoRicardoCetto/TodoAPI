# Tutorial Completo: Desenvolvendo um Sistema de Tarefas do Zero

Este tutorial vai te ensinar a criar um sistema completo de gerenciamento de tarefas usando Vue.js 3, TypeScript, Tailwind CSS e Pinia. 

## FASE 1: Criando o Projeto Base

### Passo 1.1: Criar o projeto Vue.js

```bash
# Criar projeto com Vite (ferramenta moderna)
npm create vue@latest oficina

# Durante a criação, escolha:
# ✅ TypeScript
# ✅ Pinia
# ✅ ESLint
# ❌ Router (se perguntado)
# ❌ Vitest (pode pular)
# ❌ End-to-End Testing (pode pular)

# Entre na pasta do projeto criado
cd oficina

# Instale as depêndencias do projeto
npm install
```

### Passo 1.2: Instalar Tailwind CSS

```bash
# Instalar Tailwind
npm install tailwindcss @tailwindcss/postcss postcss
npm install -D autoprefixer postcss
```

**Configure o `postcss.config.js`:**
```javascript
/**export default{
  plugins: {
    '@tailwindcss/postcss': {},
    autoprefixer: {},
  },
}
```

**Configure o `src/assets/main.css`:**
```css
@import 'tailwindcss';
```

### Passo 1.3: Instalar dependências extras

```bash
# Para fazer requisições HTTP
npm install axios

# Para servidor de desenvolvimento da API
npm install -g json-server
```

## FASE 2: Definindo a Estrutura de Dados

### Passo 2.1: Criar tipos TypeScript

**Arquivo: `src/api/itemType.ts`**
```typescript
// Interface que define como é uma tarefa
export interface itemTodo {
  id: string         
  descricao: string  
  completo: boolean  
}
```

**Por que TypeScript?**
- Evita bugs
- Facilita desenvolvimento
- Torna o projeto mais escalável

## FASE 3: Gerenciamento de Estado (Pinia)

### Passo 3.1: Criar o store (gerenciador de estado)

**Arquivo: `src/stores/useTodoStore.ts`**
```typescript
import { defineStore } from 'pinia'
import axios from 'axios'
import type { itemTodo } from '@/api/itemType'

// URL da nossa API falsa
const entityTodoApi = 'http://localhost:5132/api/Todo'

export const useTodoStore = defineStore('todo', {
  // Estado da aplicação (dados)
  state: () => ({
    todos: [] as itemTodo[],
  }),

  // Getters (dados calculados)
  getters: {
    getTodos: (state) => state.todos,
  },

  // Actions (funções que modificam o estado)
  actions: {
    // Buscar todas as tarefas
    async fetchTodos() {
      try {
        const response = await axios.get<itemTodo[]>(`${entityTodoApi}`)
        this.todos = response.data
      } catch (error) {
        console.error('Erro ao carregar lista de tarefas!', error)
      }
    },

    // Adicionar nova tarefa
    async addTodo(data: { descricao: string }) {
      try {
        const response = await axios.post<itemTodo>(`${entityTodoApi}`, {
          ...data,
          completo: false,
        })
        this.todos.push(response.data)
      } catch (error) {
        console.error('Erro ao adicionar tarefa:', error)
      }
    },

    // Atualizar tarefa existente
    async updateTodo(id: string, data: Partial<itemTodo>) {
      try {
        await axios.put<itemTodo>(`${entityTodoApi}/${id}`, data)
        const todo = this.todos.find((todo) => todo.id === id)
        if (todo) Object.assign(todo, data)
      } catch (error) {
        console.error('Erro ao atualizar tarefa:', error)
      }
    },

    // Marcar/desmarcar como concluída
    async updateToggleTodo(id: string, completo: boolean) {
      try {
        await axios.patch(`${entityTodoApi}/${id}/toggle`, { completo })
        const todo = this.todos.find((todo) => todo.id === id)
        if (todo) todo.completo = completo
      } catch (error) {
        console.error('Erro ao atualizar check da tarefa:', error)
      }
    },

    // Deletar tarefa
    async deleteTodo(id: string) {
      try {
        await axios.delete(`${entityTodoApi}/${id}`)
        this.todos = this.todos.filter((todo) => todo.id !== id)
      } catch (error) {
        console.error('Erro ao deletar tarefa:', error)
      }
    },
  },
})
```

**Conceitos importantes:**
- **State**: Onde ficam os dados
- **Getters**: Dados calculados/filtrados
- **Actions**: Funções que modificam o estado
- **Async/Await**: Para operações assíncronas (API)

## 💚 FASE 4: App.vue
```vue

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

```

## FASE 5: Arquivo main.ts
```
import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
//import router from './router'
import './assets/main.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
//app.use(router)

app.mount('#app')
```

## 🧩 FASE 6: Criando Componentes

### Passo 6.1: Componente para adicionar tarefas

**Arquivo: `src/components/FormTodo.vue`**
```vue
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
```

**Conceitos Vue:**
- **ref()**: Cria variável reativa
- **v-model**: Liga input ao JavaScript
- **@submit.prevent**: Previne reload da página
- **Composition API**: Forma moderna de escrever Vue

### Passo 4.2: Lista de tarefas

**Arquivo: `src/components/ListaTodo.vue`**
```vue
<script setup lang="ts">
import ItemToDo from './ItemTodo.vue'
import { useTodoStore } from '@/stores/useTodoStore'

const todoStore = useTodoStore()
</script>

<template>
  <div class="space-y-2">
    <ItemToDo v-for="todo in todoStore.getTodos" :key="todo.id" :todo="todo" />
  </div>
</template>
```

**Arquivo: `src/components/ItemTodo.vue`**
```vue
<script setup lang="ts">
import { defineProps, ref, nextTick } from 'vue'
import ModalDelete from './ModalDelete.vue'
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
        <button
          class="hover:cursor-pointer"
          :class="{ 'text-gray-600': !isCompleted, 'text-green-400': isCompleted }"
          @click="onCheckClick"
        >
          <svg
            class="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M5 13l4 4L19 7"
            ></path>
          </svg>
        </button>
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
        <button class="focus:outline-none hover:cursor-pointer group" @click="onUpdateClick">
          <svg
            class="ml-3 h-4 w-4 text-gray-500 group-hover:text-blue-500 transition-colors duration-200"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M12 20h9" />
            <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19.5 3 21l1.5-4L16.5 3.5z" />
          </svg>
        </button>
      </div>

      <div class="ml-auto flex items-center justify-center">
        <button class="focus:outline-none hover:cursor-pointer group" @click="onDeleteClick">
          <svg
            class="ml-3 h-4 w-4 text-gray-500 group-hover:text-[#de0025] transition-colors duration-200"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19 7L18.1327 19.1425C18.0579 20.1891 17.187 21 16.1378 21H7.86224C6.81296 21 5.94208 20.1891 5.86732 19.1425L5 7M10 11V17M14 11V17M15 7V4C15 3.44772 14.5523 3 14 3H10C9.44772 3 9 3.44772 9 4V7M4 7H20"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </button>
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
```

**Arquivo: `src/components/ModalDelete.vue`**
```vue
<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

const props = defineProps<{
	mensagem: string
	id: string
}>()

const emit = defineEmits(['excluir', 'cancelar'])

function onExcluir() {
	emit('excluir', props.id)
}
function onCancelar() {
	emit('cancelar')
}
</script>

<template>
	<div class="fixed inset-0 flex items-center justify-center z-50 backdrop-blur-sm bg-black/10">
		<div class="bg-white rounded-lg shadow-lg p-6 w-full max-w-sm flex flex-col items-center">
			<p class="text-gray-800 text-center mb-6">{{ mensagem }}</p>
			<div class="flex gap-4 w-full justify-center">
				<button
					class="bg-[#de0025] text-white px-4 py-2 rounded-md font-semibold hover:bg-red-700 transition-colors w-fit"
					@click="onExcluir"
				>
					Excluir
				</button>
				<button
					class="bg-gray-300 text-gray-800 px-4 py-2 rounded-md font-semibold hover:bg-gray-400 transition-colors w-fit"
					@click="onCancelar"
				>
					Cancelar
				</button>
			</div>
		</div>
	</div>
</template>
```

## 🔧 FASE 5: Configurando o Backend (API Falsa)

### Passo 5.1: Criar banco de dados falso

**Arquivo: `db.json` (na raiz do projeto)**
```json
{
  "Todo": [
    {
      "id": "1",
      "descricao": "Aprender Vue.js",
      "completo": false
    },
    {
      "id": "2", 
      "descricao": "Criar minha primeira aplicação",
      "completo": true
    },
    {
      "id": "3",
      "descricao": "Estudar TypeScript",
      "completo": false
    }
  ]
}
```

## 🚀 FASE 6: Executando o Projeto

### Passo 6.1: Iniciar o backend
```bash
# Terminal 1 - API
npm run json-server
```

### Passo 6.2: Iniciar o frontend
```bash
# Terminal 2 - Interface
npm run dev
```

### Passo 6.3: Acessar a aplicação
Abra: `http://localhost:5173`

## 🎓 CONCEITOS IMPORTANTES QUE VOCÊ APRENDEU

### 1. **Arquitetura de Componentes**
- Cada parte da interface é um componente reutilizável
- Comunicação entre componentes via props e eventos

### 2. **Gerenciamento de Estado**
- Estado centralizado com Pinia
- Separação entre dados, getters e actions

### 3. **TypeScript**
- Tipagem forte previne bugs
- Interfaces definem contratos de dados

### 4. **Reatividade Vue**
- `ref()` para dados simples
- `computed()` para dados calculados
- `watch()` para observar mudanças

## 🛠️ PRÓXIMOS PASSOS

### Melhorias que você pode implementar:

1. **Filtros**: Mostrar só concluídas/pendentes
2. **Busca**: Procurar tarefas por texto
3. **Categorias**: Organizar por tipo
4. **Drag & Drop**: Reordenar tarefas
5. **Dark Mode**: Tema escuro/claro
6. **Animações**: Transições suaves
7. **PWA**: Funcionar offline
8. **Backend Real**: PostgreSQL, MongoDB

### Tecnologias para estudar depois:

- **Nuxt.js**: Framework full-stack Vue
- **Vuetify/Quasar**: Bibliotecas de componentes
- **Vue Router**: Navegação entre páginas
- **Vitest**: Testes automatizados
- **Docker**: Containerização
- **Deploy**: Vercel, Netlify, AWS

## 🎉 PARABÉNS!

Você criou uma aplicação web moderna e profissional! Este projeto demonstra conhecimentos em:

✅ Frontend moderno (Vue.js + TypeScript)
✅ Gerenciamento de estado (Pinia)
✅ Estilização (Tailwind CSS)
✅ Integração com APIs
✅ Componentização
✅ Boas práticas de código

Continue praticando e evoluindo este projeto. O desenvolvimento web é uma jornada de aprendizado constante!

---

**📚 Recursos para continuar:**
- [Vue.js Docs](https://vuejs.org/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Pinia Docs](https://pinia.vuejs.org/)
- [Tailwind CSS](https://tailwindcss.com/)
