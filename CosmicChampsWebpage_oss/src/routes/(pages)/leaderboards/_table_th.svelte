<script>
	import Icon from './_icon.svelte';
	export let title = '';
	export let icon = true;
	// export const sort =  () => {};
	let hidden = true;
	let activeClass = 'text-cyan-500';
	function show(event) {
		// console.log('show', event);
		hidden = !hidden;
	}
</script>

<div class="flex cursor-pointer select-none flex-nowrap items-center whitespace-nowrap">
	{title}
	{#if icon}
		<Icon on:togle={(e) => show(e)} classes={!hidden ? activeClass : ''} />
	{/if}
</div>
