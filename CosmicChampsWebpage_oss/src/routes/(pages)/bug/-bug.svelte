<script>
	import { slide } from 'svelte/transition';
	export let item;
	let isOpen = false;
	const toggle = () => (isOpen = !isOpen);
</script>

<div class="accordion">
	<div class="accordion-item">
		<h2 class="accordion-header">
			<button
				class="
        accordion-button
        relative
        flex
        w-full
        flex-wrap
        items-center
        justify-between
        rounded-none
        border-0
        py-2.5
        px-3
        text-left
        transition
        focus:outline-none
      "
				type="button"
				aria-expanded={isOpen ? 'false' : 'true'}
				on:click={toggle}
				on:keypress={toggle}
			>
				<span class="mr-2">{item[11]}</span>
				<span class="my-0.5 ml-auto mr-1 rounded-sm bg-slate-200 py-0.5 px-1.5 text-[11px] font-normal">{item[7]}</span>
				{#if item[14]}
					<span
						class="my-0.5 rounded-sm bg-slate-200 py-0.5 px-1.5 text-[11px] font-normal {item[14] == 'Pending'
							? 'bg-orange-200 text-orange-800'
							: ''}{item[14] == 'Resolved' ? 'bg-green-200 text-green-800' : ''}">{item[14]}</span
					>
				{/if}
			</button>
		</h2>
		{#if isOpen}
			<div class="accordion-collapse" class:collapse={!isOpen} transition:slide|global={{ duration: 300 }}>
				<div class="accordion-body px-3 text-sm sm:text-base">
					{#if item[8]}
						<div class="py-3">
							<h4 class="font-medium text-cyan-800">Bug description</h4>
							<div>{item[8]}</div>
						</div>
					{/if}
					{#if item[15]}
						<div class="py-3">
							<h4 class="font-medium text-cyan-800">Temporary solution</h4>
							<div>{item[15]}</div>
						</div>
					{/if}
					<div class="py-3">
						<h4 class="font-medium text-cyan-700">Device info</h4>
						<div class="capitalize">
							<span class="mr-4"><span class="mr-1 text-xs text-slate-600">OS:</span>{item[0]}</span>
							{#if item[2]}
								<span class="mr-4"
									><span class="mr-1 text-xs text-slate-600">OS version:</span>
									{item[2]}</span
								>
							{/if}
							{#if item[1]}
								<span class="mr-4"><span class="mr-1 text-xs text-slate-600">Device:</span>{item[1]}</span>
							{/if}
						</div>
					</div>
				</div>
			</div>
		{/if}
	</div>
</div>

<style lang="scss">
	.accordion-header {
		@apply font-medium text-cyan-800;
	}
	.accordion-item {
		@apply mb-3 rounded-md border border-cyan-600;
	}
</style>
