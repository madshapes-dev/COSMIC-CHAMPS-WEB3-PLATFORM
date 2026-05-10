<script>
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { PeraWalletConnect } from '@perawallet/connect';
	import { DeflyWalletConnect } from '@blockshake/defly-connect';
	import { clickOutside, shortenString } from '$lib/helpers.js';
	import { walletAccounts } from '$lib/stores/walletAccounts.js';
	import { wallet } from '$lib/stores/wallet.js';
	import { browser } from '$app/environment';
	import Dropdown from '$lib/header/-dropdown.svelte';

	import assetCC from '$lib/header/assets/asset-logo-cosmic-champs.svg';
	import assetAlgorand from '$lib/header/assets/asset-logo-algorand.svg';

	let hiddenAmount = true;
	$: hiddenWalletDropdown = true;

	let timer;
	let hiddenReload = false;
	$: timer = 10;
	async function getAlgoConnectData(address, delayReload) {
		const url = `https://mainnet-api.algonode.cloud/v2/accounts/${address}`;
		const response = await fetch(url);
		const data = await response.json();
		const account = $walletAccounts?.find((item) => item.address == address);
		const nfdomain = account?.nfdomain ? account.nfdomain : '';
		const network = 'Algorand';
		const networkId = 'ALGO';
		if (response.ok && data) {
			const name = account?.name ? account.name : '';
			$wallet = { ...data, name, nfdomain, network, networkId };
			if (browser) window.localStorage.setItem('wallet', JSON.stringify($wallet));
		} else if (account) {
			$wallet = { ...account, nfdomain, network, networkId };
			if (browser) window.localStorage.setItem('wallet', JSON.stringify(account));
		} else {
			$wallet = { address: 'undefined' };
			if (browser) window.localStorage.setItem('wallet', JSON.stringify({ address: 'undefined' }));
		}
		if (delayReload) {
			hiddenReload = true;
			let countdown = setInterval(() => {
				if (timer > 0 && hiddenReload) timer--;
				else {
					timer = 10;
					hiddenReload = false;
					clearInterval(countdown);
				}
			}, 1000);
		}
	}

	let peraWallet = new PeraWalletConnect();
	let deflyWallet = new DeflyWalletConnect();
	function disconnectWallet() {
		if (browser) {
			window.localStorage.setItem('wallet', []);
			window.localStorage.setItem('accounts', []);
		}
		$wallet = [];
		$walletAccounts = [];
		hiddenWalletDropdown = true;
		peraWallet.disconnect();
		deflyWallet.disconnect();
	}

	let algo = 0;
	let cosg = 0;
	$: {
		algo = $wallet?.amount ? parseInt($wallet.amount) : 0;
		algo = (algo / 1000000)
			.toFixed(2)
			.toString()
			.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

		cosg =
			$wallet?.assets && parseInt($wallet.assets.find((item) => item['asset-id'] == 1065092715)?.amount)
				? parseInt($wallet.assets.find((item) => item['asset-id'] == 1065092715)?.amount)
				: 0;
		cosg = (cosg / 1000000)
			.toFixed(2)
			.toString()
			.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
	}
</script>

<div
	class="menu-wallet-wrapper relative flex flex-col justify-center text-xs"
	use:clickOutside={(e) => {
		if (!e.target.classList.contains('dropdown-toggle')) hiddenWalletDropdown = true;
	}}
>
	<Dropdown id="menuWallet" hidden={hiddenWalletDropdown}>
		<button
			slot="dropdown-toggle"
			on:click={() => (hiddenWalletDropdown = !hiddenWalletDropdown)}
			class:active={!hiddenWalletDropdown}
			class="button button--primary dropdown-toggle w-32 md:ml-4"
			><span class="button__inner">
				{#await $wallet}
					<div class="icon">
						<svg
							fill="none"
							viewBox="0 0 24 24"
							stroke="currentColor"
							aria-hidden="true"
							xmlns="http://www.w3.org/2000/svg"
							width="100%"
							height="100%"
							class="mx-auto h-4 w-4 animate-spin md:h-5 md:w-5 lg:h-6 lg:w-6"
							outline="true"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
							/></svg
						>
					</div>
				{:then $wallet}
					{#if $wallet?.address == 'undefined'}
						Invalid address
					{:else if $wallet?.address}
						{#if $wallet.nfdomain}
							<div class="-mt-[3px] text-center text-[9px] font-medium leading-none lg:text-[10px]">{shortenString($wallet.nfdomain)}</div>
						{/if}
						{`${$wallet.address.substring(0, 5)}...${$wallet.address.slice(-4)}`}
					{:else}
						<div class="icon">
							<svg
								fill="none"
								viewBox="0 0 24 24"
								stroke="currentColor"
								aria-hidden="true"
								xmlns="http://www.w3.org/2000/svg"
								width="100%"
								height="100%"
								class="mx-auto h-4 w-4 animate-spin md:h-5 md:w-5 lg:h-6 lg:w-6"
								outline="true"
								><path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
								/></svg
							>
						</div>
					{/if}
				{/await}
			</span>
		</button>
		<div slot="dropdown" class="menu-wallet-inner z-[52] px-3 pt-2 pb-5">
			<div class="flex items-center justify-between pb-4">
				{#if $wallet.nfdomain}
					<div class="pt-1 font-medium text-gray-500 dark:text-cyan-500">
						<span class="text-sm text-cyan-700 dark:text-cyan-300">{$wallet.nfdomain}</span>
						{#if $wallet.name}
							<br />{$wallet.name}{/if}
					</div>
				{:else if $wallet.name}
					<div class="font-medium text-gray-500 dark:text-cyan-300">
						{$wallet.name}
					</div>
				{/if}

				<div class="-mr-1 ml-auto">
					<button
						type="button"
						class="inline-flex items-center justify-center rounded-md bg-white p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-cyan-500 dark:bg-cyan-200 dark:text-cyan-700"
						on:click={() => (hiddenWalletDropdown = !hiddenWalletDropdown)}
					>
						<span class="sr-only">Close menu</span>
						<svg
							fill="none"
							viewBox="0 0 24 24"
							stroke="currentColor"
							aria-hidden="true"
							xmlns="http://www.w3.org/2000/svg"
							width="100%"
							height="100%"
							class="h-6 w-6"
							outline="true"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg
						>
					</button>
				</div>
			</div>
			<nav class="grid gap-y-3">
				{#if $wallet.networkId === 'ALGO'}
					<div class="flex w-full justify-between border-b pb-4 dark:border-cyan-700">
						<div class="">
							<div class="mb-1 flex flex-wrap items-center justify-between">
								<a
									href="https://app.tinyman.org/#/swap?asset_in=1065092715&asset_out=0"
									class="mr-auto flex w-16 items-center justify-between rounded-sm bg-slate-100 p-2 hover:bg-slate-200 dark:bg-cyan-700 dark:hover:bg-cyan-600"
									target="_blank"
									rel="noreferrer"
								>
									<img src={assetCC} alt="" class="w-4" />
									<span class="ml-1 font-medium dark:text-cyan-300">COSG</span>
								</a>
								<span class="amount-number" class:amount-number__hidden={hiddenAmount}>{cosg}</span>
							</div>
							<div class="flex flex-wrap items-center justify-between">
								<a
									href="https://app.tinyman.org/#/swap?asset_in=0&asset_out=1065092715"
									class="mr-auto flex w-16 items-center justify-between rounded-sm bg-slate-100 p-2 hover:bg-slate-200 dark:bg-cyan-700 dark:hover:bg-cyan-600"
									target="_blank"
									rel="noreferrer"
								>
									<img src={assetAlgorand} alt="" class="w-4 dark:invert" /><span class="ml-1 font-medium dark:text-cyan-300">ALGO</span>
								</a>
								<span class="amount-number" class:amount-number__hidden={hiddenAmount}>{algo}</span>
							</div>
						</div>
						<div class="flex flex-col justify-between">
							<a href="/" on:click|preventDefault|stopPropagation={() => (hiddenAmount = !hiddenAmount)} class="icon">
								{#if hiddenAmount}
									<svg
										fill="none"
										viewBox="0 0 24 24"
										stroke="currentColor"
										aria-hidden="true"
										xmlns="http://www.w3.org/2000/svg"
										width="100%"
										height="100%"
										class="h-4 w-4"
										outline="true"
										><path
											stroke-linecap="round"
											stroke-linejoin="round"
											stroke-width="2"
											d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21"
										/></svg
									>
								{:else}
									<svg
										fill="none"
										viewBox="0 0 24 24"
										stroke="currentColor"
										aria-hidden="true"
										xmlns="http://www.w3.org/2000/svg"
										width="100%"
										height="100%"
										class="h-4 w-4"
										outline="true"
										><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path
											stroke-linecap="round"
											stroke-linejoin="round"
											stroke-width="2"
											d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
										/></svg
									>
								{/if}
							</a>
							{#if !hiddenReload}
								<a href="/" on:click|preventDefault={() => getAlgoConnectData($wallet.address, true)} class="icon">
									<svg
										fill="none"
										viewBox="0 0 24 24"
										stroke="currentColor"
										aria-hidden="true"
										xmlns="http://www.w3.org/2000/svg"
										width="100%"
										height="100%"
										class="h-4 w-4"
										outline="true"
										><path
											stroke-linecap="round"
											stroke-linejoin="round"
											stroke-width="2"
											d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
										/></svg
									>
								</a>
							{:else}
								<div class="icon">
									<svg
										fill="none"
										viewBox="0 0 24 24"
										stroke="currentColor"
										aria-hidden="true"
										xmlns="http://www.w3.org/2000/svg"
										width="100%"
										height="100%"
										class="h-4 w-4 animate-spin"
										outline="true"
										><path
											stroke-linecap="round"
											stroke-linejoin="round"
											stroke-width="2"
											d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
										/></svg
									>
								</div>
							{/if}
						</div>
					</div>
					{#if $walletAccounts.length > 1 && $wallet.address != 'undefined'}
						<div class="-mt-2 border-b p-2 pb-3 dark:border-cyan-700">
							Switch address to: <br />
							<div class="mt-1 flex flex-wrap">
								{#each $walletAccounts as item}
									{#if item.address != $wallet.address}
										<a
											on:click|preventDefault={() => getAlgoConnectData(item.address)}
											href="/"
											class="my-0.5 mr-2 flex items-center rounded-sm bg-gray-100 px-1 dark:bg-cyan-700"
										>
											{#if item.nfdomain}
												{`${item.nfdomain}`}<br />
												{`${item.address.substring(0, 4)}...${item.address.slice(-4)}`}
											{:else}{`${item.address.substring(0, 5)}...${item.address.slice(-4)}`}{/if}
										</a>
									{/if}
								{/each}
							</div>
						</div>
					{/if}
				{/if}
				<!-- <a
					href="https://nft.cosmicchamps.com"
					class="flex items-center rounded-md p-2 hover:bg-slate-200 dark:hover:bg-cyan-600"
					target="_blank"
					on:click={() => (hiddenWalletDropdown = !hiddenWalletDropdown)}
				>
					<svg
						fill="none"
						viewBox="0 0 24 24"
						stroke="currentColor"
						aria-hidden="true"
						xmlns="http://www.w3.org/2000/svg"
						width="100%"
						height="100%"
						class="h-6 w-6 flex-shrink-0 text-cyan-600 dark:text-cyan-400"
						outline="true"
						><path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
						/></svg
					>
					<span class="ml-3 text-sm font-medium text-gray-900 dark:text-cyan-200"> Cosmic Champs NFTs </span>
				</a> -->

				<a
					href="/"
					class="-my-2 flex items-center rounded-md p-2 hover:bg-slate-200 dark:hover:bg-cyan-600"
					on:click|preventDefault={() => {
						disconnectWallet();
					}}
				>
					<svg
						fill="none"
						viewBox="0 0 24 24"
						stroke="currentColor"
						aria-hidden="true"
						xmlns="http://www.w3.org/2000/svg"
						width="100%"
						height="100%"
						class="h-6 w-6 flex-shrink-0 text-cyan-600 dark:text-cyan-400"
						outline="true"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg
					>
					<span class="ml-3 text-sm font-medium text-gray-900 dark:text-cyan-200"> Disconnect wallet </span>
				</a>
			</nav>
		</div>
	</Dropdown>
</div>

<style global>
	#menuWallet {
		background: var(--silver-gradient);
		@apply rounded-lg p-1;
	}
	.button {
		@apply mt-0;
	}
</style>
