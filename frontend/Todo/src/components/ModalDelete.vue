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
