import { defineStore } from 'pinia'
import axios from 'axios'
import type { itemTodo } from '@/api/itemType'

const entityTodoApi = 'http://localhost:5132/api/Todo'
//const entityTodoMock = 'http://localhost:3000/todos'

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
        console.error('Erro ao carregar lista de tarefas!', error)
      }
    },

    setTodos(todos: itemTodo[]) {
      this.todos = todos
    },

    async addTodo(data: { descricao: string }) {
      try {
        const response = await axios.post<itemTodo>(`${entityTodoApi}`, {
          ...data,
          completo: false,
        })

        this.todos.push(response.data)

        console.log('Tarefa adicionada:', response.data)
      } catch (error) {
        console.error('Erro ao adicionar tarefa:', error)
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
        console.error('Erro ao atualizar tarefa:', error)
      }
    },

    async updateToggleTodo(id: string, completo: boolean) {
      try {
        await axios.patch(`${entityTodoApi}/${id}/toggle`, { completo })

        const todo = this.todos.find((todo) => todo.id === id)

        if (todo) todo.completo = completo
      } catch (error) {
        console.error('Erro ao atualizar check da tarefa:', error)
      }
    },

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
