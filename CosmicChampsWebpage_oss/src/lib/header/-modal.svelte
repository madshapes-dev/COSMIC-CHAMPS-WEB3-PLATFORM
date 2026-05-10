<script>
	import { createEventDispatcher } from 'svelte';
	import { fly } from 'svelte/transition';
	import { clickOutside } from '$lib/helpers.js';

	export let id = 'modalWallet';
	export let hidden = true;
	export let connectPeraWallet = () => {};
	export let connectDeflyWallet = () => {};
	export let connectLuteWallet = () => {};
	// export let connectMetaMaskWallet = () => {};

	import myAlgo from './assets/wallet-MyAlgo.svg';
	import pera from './assets/wallet-Pera.svg';
	import walletDefly from './assets/wallet-Defly.svg';
	import walletMetaMask from './assets/wallet-MetaMask.svg';
	import walletLute from './assets/wallet-Lute.svg';

	// a way to send variable state to parent component
	const dispatch = createEventDispatcher();
	function hideModal() {
		dispatch('hideModal', {
			hiddenModal: !hidden
		});
	}
</script>

{#if !hidden}
	<div
		{id}
		tabindex="-1"
		aria-hidden={hidden}
		class="modal h-modal fixed top-0 left-0 right-0 z-[60] flex h-full w-full items-center justify-center overflow-y-auto overflow-x-hidden bg-black bg-opacity-80 md:inset-0 md:h-full"
	>
		<div
			class="relative flex h-full w-full w-full max-w-md items-center p-4 md:h-auto"
			in:fly|global={{ y: 50, duration: 300 }}
			out:fly|global={{ y: 50, duration: 100 }}
		>
			<div
				class="relative rounded-lg bg-gray-50 shadow dark:bg-cyan-900 w-full"
				use:clickOutside={(e) => {
					if (!hidden) hideModal();
				}}
			>
				<button
					type="button"
					class="absolute top-3 right-2.5 ml-auto inline-flex items-center rounded-lg bg-transparent p-1.5 text-sm text-gray-300 hover:bg-gray-200 hover:text-gray-900 dark:hover:bg-cyan-800 dark:hover:text-white"
					data-modal-toggle="crypto-modal"
					on:click={() => hideModal()}
				>
					<svg aria-hidden="true" class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"
						><path
							fill-rule="evenodd"
							d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
							clip-rule="evenodd"
						/></svg
					>
					<span class="sr-only">Close modal</span>
				</button>
				<!-- Modal header -->
				<div class=" rounded-t border-b px-6 py-4 dark:border-cyan-700">
					<h3 class="title title--size-4 mb-0 text-base font-semibold text-gray-900 dark:text-white lg:text-xl">Connect wallet</h3>
				</div>
				<!-- Modal body -->
				<div class="p-6">
					<p class="text-sm font-normal text-gray-500 dark:text-cyan-500">Supported Algorand wallets</p>
					<ul class="my-4 space-y-3">
						<li>
							<a
								href="/"
								class="group flex items-center rounded-lg bg-slate-200 p-3 text-base font-bold text-gray-900 hover:bg-slate-200 hover:shadow dark:bg-cyan-700 dark:text-cyan-200 dark:hover:bg-cyan-600"
								on:click|preventDefault={() => {
									connectPeraWallet();
									hideModal();
								}}
							>
								<img src={pera} class="h-8" alt="Pera Wallet logo" />
								<span class="ml-3 flex-1 whitespace-nowrap">Pera wallet</span>
							</a>
						</li>
						<li>
							<a
								href="/"
								class="group flex items-center rounded-lg bg-slate-200 p-3 text-base font-bold text-gray-900 hover:bg-slate-200 hover:shadow dark:bg-cyan-700 dark:text-cyan-200 dark:hover:bg-cyan-600"
								on:click|preventDefault={() => {
									connectDeflyWallet();
									hideModal();
								}}
							>
								<img src={walletDefly} class="h-8 w-8 rounded-full" alt="Defly Wallet logo" />
								<span class="ml-3 flex-1 whitespace-nowrap">Defly wallet</span>
							</a>
						</li>
						<li>
							<a
								href="/"
								class="group flex items-center rounded-lg bg-slate-200 p-3 text-base font-bold text-gray-900 hover:bg-slate-200 hover:shadow dark:bg-cyan-700 dark:text-cyan-200 dark:hover:bg-cyan-600"
								on:click|preventDefault={() => {
									connectLuteWallet();
									hideModal();
								}}
							>
								<img src={walletLute} class="h-8 w-8 rounded-full" alt="Lute Wallet logo" />
								<span class="ml-3 flex-1 whitespace-nowrap">Lute wallet</span>
							</a>
						</li>
					</ul>
					<!-- <p class="text-sm font-normal text-gray-500 dark:text-cyan-500 mt-6">Supported Etherium wallets</p>
					<ul class="my-4 space-y-3">
						<li>
							<a
								href="/"
								class="group flex items-center rounded-lg bg-slate-200 p-3 text-base font-bold text-gray-900 hover:bg-slate-200 hover:shadow dark:bg-cyan-700 dark:text-cyan-200 dark:hover:bg-cyan-600"
								on:click|preventDefault={() => {
									connectMetaMaskWallet();
									hideModal();
								}}
							>
								<img src={walletMetaMask} class="h-8 w-8 rounded-full" alt="Defly Wallet logo" />
								<span class="ml-3 flex-1 whitespace-nowrap">MetaMask wallet</span>
							</a>
						</li>
					</ul> -->
				</div>
			</div>
		</div>
	</div>
{/if}

<style lang="scss">
	.dark .modal .title {
		-webkit-text-fill-color: transparent;
		background: var(--highlight-gradient-secondary);
		-webkit-background-clip: text;
		background-clip: text;
	}
	h3 {
		@apply mb-0;
	}
</style>
