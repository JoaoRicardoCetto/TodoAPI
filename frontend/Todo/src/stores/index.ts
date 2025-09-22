import { defineStore } from 'pinia'
import axios from 'axios'
import type { itemTodo } from '@/api/itemType'

//const entityTodoApi = 'https://localhost:7062/api/Todo'
const entityTodoMock = 'http://localhost:3000/todos'

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
        const response = await axios.get<itemTodo[]>(`${entityTodoMock}`)
        this.todos = response.data
      } catch (error) {
        console.error('There was an error!', error)
      }
    },

    setTodos(todos: itemTodo[]) {
      this.todos = todos
    },

    async addTodo(data: { descricao: string }) {
      try {
        const response = await axios.post<itemTodo>(`${entityTodoMock}`, {
          ...data,
          completo: false,
        })

        this.todos.push(response.data)

        console.log('Todo adicionado:', response.data)
      } catch (error) {
        console.error('Erro ao adicionar todo:', error)
      }
    },

    async updateTodo(id: string, data: Partial<itemTodo>) {
      try {
        await axios.patch(`${entityTodoMock}/${id}`, data)

        const todo = this.todos.find((todo) => todo.id === id)
        if (todo) Object.assign(todo, data)
      } catch (error) {
        console.error('Erro ao atualizar todo:', error)
      }
    },
  },
})
