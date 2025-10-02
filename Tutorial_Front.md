# Tutorial Completo: Desenvolvendo um Sistema de Tarefas do Zero

Este tutorial vai te ensinar a criar um sistema completo de gerenciamento de tarefas usando Vue.js 3, TypeScript, Tailwind CSS e Pinia. 

## FASE 1: Criando o Projeto Base

### Passo 1.1: Criar o projeto Vue.js

```bash
# Criar projeto com Vite (ferramenta moderna)
npm create vue@latest oficina

# Durante a criação, escolha:
# ✅ TypeScript
# ✅ Router (se perguntado)
# ✅ Pinia
# ✅ ESLint
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

## FASE 4: Criando Componentes

### Passo 4.1: Componente para adicionar tarefas

**Arquivo: `src/components/FormTodo.vue`**
```vue
<script setup lang="ts">
import { ref } from 'vue'
import { useTodoStore } from '@/stores/useTodoStore'

// Variável reativa para o texto do input
const descricao = ref('')
const todoStore = useTodoStore()

async function addNovaTarefa() {
  try {
    if (descricao.value.trim()) {
      await todoStore.addTodo({ descricao: descricao.value })
      descricao.value = '' // Limpar o campo
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

## FASE 5: Configurando o Backend (API Falsa)

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

## FASE 6: Executando o Projeto

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

**Recursos para continuar:**
- [Vue.js Docs](https://vuejs.org/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Pinia Docs](https://pinia.vuejs.org/)
- [Tailwind CSS](https://tailwindcss.com/)



