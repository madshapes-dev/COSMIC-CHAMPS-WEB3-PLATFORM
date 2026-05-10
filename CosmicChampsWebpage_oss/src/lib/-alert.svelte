<script>
	import { fade } from 'svelte/transition';
	import { wallet } from '$lib/stores/wallet.js';

	let hide = true;
	$: hide = $wallet.assets?.find((item) => item['asset-id'] === 1065092715);
</script>

{#if !hide}
	<div class="flex justify-center bg-cyan-800" role="alert" transition:fade|global>
		<div class="flex inline-flex flex-nowrap items-center rounded-md bg-cyan-800 p-1.5 text-xs text-cyan-100 md:p-2 lg:p-3 lg:text-base" role="alert">
			<span
				class="mr-2 self-center rounded bg-cyan-500 px-2 py-0.5 text-center text-[9px] font-bold uppercase leading-none lg:mr-4 lg:py-1 lg:text-xs"
			>
				$COSG token
			</span>
			<span class="mr-2 flex-auto text-left font-semibold">Please opt-in to an asset: <b>1065092715</b></span>
			<a href="https://medium.com/@CosmicChamps/1c49662b870a" target="_blank">
				<svg class="h-4 w-4 fill-current opacity-75" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
					><path d="M12.95 10.707l.707-.707L8 4.343 6.586 5.757 10.828 10l-4.242 4.243L8 15.657l4.95-4.95z" /></svg
				>
			</a>
		</div>
	</div>
{/if}
