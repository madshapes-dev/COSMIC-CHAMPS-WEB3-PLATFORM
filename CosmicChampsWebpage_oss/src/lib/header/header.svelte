<script>
	// import { MetaMaskSDK } from '@metamask/sdk';
	import { PeraWalletConnect } from '@perawallet/connect';
	import { DeflyWalletConnect } from '@blockshake/defly-connect';
	import { walletAccounts } from '$lib/stores/walletAccounts.js';
	import { wallet } from '$lib/stores/wallet.js';
	import { tournament } from '$lib/stores/tournament.js';
	import { Icon } from '@steeze-ui/svelte-icon';
	import { Home } from '@steeze-ui/heroicons';
	import { browser } from '$app/environment';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { clickOutside } from '$lib/helpers.js';
	import DropdownMobile from '$lib/header/-dropdown.svelte';
	import MenuWallet from '$lib/header/menuWallet.svelte';
	import Modal from '$lib/header/-modal.svelte';
	import Alert from '$lib/-alert.svelte';

	import logo from '$assets/logo.png';
	import logoChamps from '$assets/logo-192.png';
	import leaderboards from '$lib/header/assets/menu/leaderboards.webp';
	import tournamentImg from '$lib/header/assets/menu/tournament.webp';
	import bugs from '$lib/header/assets/menu/bugs.webp';
	import feature from '$lib/header/assets/menu/feature.webp';
	import nfts from '$lib/header/assets/menu/nfts.svg';
	import cards from '$lib/header/assets/menu/cards.svg';
	import about from '$lib/header/assets/menu/about.webp';

	export let pageClass;

	$: hiddenModal = true;
	$: hiddenMenuDropdown = true;

	async function getNFDomainsData(address) {
		const url = `https://api.nf.domains/nfd/v2/address?address=${address}&limit=20&view=brief`;
		const response = await fetch(url);
		let data = '';
		if (response.ok) {
			data = await response.json();
			if (data[`${address}`]) data = await data[`${address}`][0].name;
		}
		return data;
	}

	// Create Pera wallet connector (browser-only — ESM version fails during SSR)
	let peraWallet = browser ? new PeraWalletConnect() : null;
	async function connectPeraWallet() {
		peraWallet
			.connect()
			.then(async (peraAccounts) => {
				const accounts = await Promise.all(
					peraAccounts.map(async (item) => {
						const nfdomain = await getNFDomainsData(item);
						return { address: item, nfdomain };
					})
				);
				if (accounts.length > 0) {
					accounts.forEach(async (item) => {
						await getAlgoConnectData(item.address);
					});
					$walletAccounts = accounts;
					if (browser) window.localStorage.setItem('accounts', JSON.stringify(accounts));
				}
			})
			.catch((error) => {
				if (error.data && error.data.type !== 'CONNECT_MODAL_CLOSED') {
					console.log(error);
				}
			});
	}

	// Defly wallet connector (browser-only — ESM version fails during SSR)
	let deflyWallet = browser ? new DeflyWalletConnect() : null;
	async function connectDeflyWallet() {
		deflyWallet
			.connect()
			.then(async (deflyAccounts) => {
				const accounts = await Promise.all(
					deflyAccounts.map(async (item) => {
						const nfdomain = await getNFDomainsData(item);
						return { address: item, nfdomain };
					})
				);
				if (accounts.length > 0) {
					accounts.forEach(async (item) => {
						await getAlgoConnectData(item.address);
					});
					$walletAccounts = accounts;
					if (browser) window.localStorage.setItem('accounts', JSON.stringify(accounts));
				}
			})
			.catch((error) => {
				if (error?.data?.type !== 'CONNECT_MODAL_CLOSED') {
					console.log(error);
				}
			});
	}

	// Lute wallet connector
	async function connectLuteWallet() {
		try {
			const { default: LuteConnect } = await import('lute-connect');
			const luteWallet = new LuteConnect('Cosmic Champs');
			const luteAccounts = await luteWallet.connect('mainnet-v1.0');
			const accounts = await Promise.all(
				luteAccounts.map(async (item) => {
					const nfdomain = await getNFDomainsData(item);
					return { address: item, nfdomain };
				})
			);
			if (accounts.length > 0) {
				accounts.forEach(async (item) => {
					await getAlgoConnectData(item.address);
				});
				$walletAccounts = accounts;
				if (browser) window.localStorage.setItem('accounts', JSON.stringify(accounts));
			}
		} catch (error) {
			console.log(error);
		}
	}

	// MetaMask wallet connector
	// const MMSDK = new MetaMaskSDK({
	// 	dappMetadata: { name: 'Cosmic Champs', url: 'https://beta.cosmicchamps.com' },
	// 	checkInstallationOnAllCalls: true
	// });
	// async function connectMetaMaskWallet() {
	// 	const ethereum = MMSDK.getProvider();
	// 	const metaMask = ethereum
	// 		.request({ method: 'eth_requestAccounts', params: [] })
	// 		.then((accounts) => {
	// 			console.log('eth accounts ', accounts);
	// 			if (accounts.length > 0) {
	// 				$wallet = { address: accounts[0], network: 'Ethereum', networkId: 'ETH' };
	// 				if (browser) window.localStorage.setItem('wallet', JSON.stringify($wallet));
	// 				$walletAccounts = [{ address: accounts[0] }];
	// 				if (browser) window.localStorage.setItem('accounts', JSON.stringify($walletAccounts));
	// 			}
	// 		})
	// 		.catch((error) => {
	// 			if (error) console.log(error);
	// 		});
	// }

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

	$: urlSplit = $page.url.pathname.trim($page.url.pathname.lastIndexOf('/'));
	$: pageClass = urlSplit.replace('/', '').replaceAll('/', '-');
</script>

<div
	class="header-main-menu shadow-xl {pageClass}"
	use:clickOutside={(e) => {
		if (!e.target.classList.contains('dropdown-toggle')) hiddenMenuDropdown = true;
	}}
>
	<div class="-mt-1">
		{#if pageClass != 'play-web' && pageClass != 'play'}
			<Alert />
		{/if}
		<nav class="rounded-b-lg border-gray-200 bg-white">
			<div class="mx-auto flex max-w-screen-2xl flex-wrap items-center justify-between px-2 py-2 xl:p-4 xs:px-4">
				<a href="/" class="mr-auto flex flex-col items-center xl:flex-row">
					<img src={logo} class="h-8 xl:h-10 xl:mr-2" alt="Cosmic Champs beta" />

					<span class="title--cyan sm:text-md hidden w-full self-center whitespace-nowrap rounded py-0.5 text-center text-xs font-bold sm:px-2">BETA</span>
				</a>

				<div class="ml-auto flex items-center lg:order-2">
					{#if pageClass != 'play-web' && pageClass != 'play'}
						<div class="relative">
							{#if !hiddenModal}
								<Modal
									on:hideModal={(event) => (hiddenModal = event.detail.hiddenModal)}
									hidden={hiddenModal}
									{connectPeraWallet}
									{connectDeflyWallet}
									{connectLuteWallet}
								/>
							{/if}
							<div class="container mx-auto">
								{#if !$walletAccounts.length}
									<button
										on:click|stopPropagation={() => (hiddenModal = false)}
										id="button-modal"
										class="button-modal button button--primary pointer-events-auto w-32"
										><span class="button__inner pointer-events-none flex items-center"
											><div class="py-1 text-xs leading-none lg:text-sm lg:leading-none">Connect wallet</div></span
										></button
									>
								{:else}
									<MenuWallet />
								{/if}
							</div>
						</div>
					{:else}
						<a href="/" class="invisible flex flex-col items-center xl:flex-row">
							<img src={logo} class="h-8 xl:mr-2" alt="Cosmic Champs beta" />
						</a>
					{/if}
					<button
						type="button"
						class="button button--secondary ml-3 inline-flex items-center rounded-lg p-2 text-sm text-gray-500 focus:outline-none lg:hidden"
						aria-controls="navbar-cta"
						aria-expanded={!hiddenMenuDropdown}
						class:active={!hiddenMenuDropdown}
						on:click={() => (hiddenMenuDropdown = !hiddenMenuDropdown)}
					>
						<span class="sr-only">Open main menu</span>
						<span class="button__inner">
							<svg class="h-6 w-6" aria-hidden="true" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"
								><path
									fill-rule="evenodd"
									d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
									clip-rule="evenodd"
								/></svg
							>
						</span>
					</button>
				</div>

				<div
					class="mx-auto w-full items-center justify-between md:order-1 md:w-auto md:pt-6 lg:flex lg:px-4 lg:pt-0"
					class:hidden={hiddenMenuDropdown}
				>
					<ul
						class="mt-4 flex flex-col space-y-1 py-4 font-medium sm:text-sm md:mt-0 md:flex-row md:items-center md:space-y-0 md:space-x-4 md:border-0 md:bg-white md:p-0 lg:text-base xl:space-x-8"
					>
						<li>
							<a
								href="/play"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								on:click={() => (hiddenMenuDropdown = true)}
								class:active={$page.url.pathname === '/play' || $page.url.pathname === '/play-web'}
								><img class="mr-1 h-6 w-6" src={logoChamps} alt="Cosmic Champs logo" /> <span>Play</span></a
							>
						</li>
						{#if $tournament.show}
							<li>
								<a
									href="/tournament"
									class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
									on:click={() => (hiddenMenuDropdown = true)}
									class:active={$page.url.pathname === '/tournament'}
									><span class="mr-1 w-6"><img class=" w-5" src={tournamentImg} alt="Trophy emoji" /></span>Tournament</a
								>
							</li>
						{/if}
						<li>
							<a
								href="/leaderboards"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								on:click={() => (hiddenMenuDropdown = true)}
								class:active={$page.url.pathname === '/leaderboards'}
								><span class="mr-1 w-6"><img class=" w-5" src={leaderboards} alt="Medal emoji" /></span>Leaderboards</a
							>
						</li>
						{#if false}
						<li>
							<a
								href="/bug"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								on:click={() => (hiddenMenuDropdown = true)}
								class:active={$page.url.pathname === '/bug' || $page.url.pathname === '/bug/submit'}
								><span class="mr-1 w-6"><img class=" w-5" src={bugs} alt="Bug emoji" /></span>Bugs</a
							>
						</li>
						<li>
							<a
								href="https://cosmic-champs.changelogfy.com/feature-requests"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								target="_blank"
								on:click={() => (hiddenMenuDropdown = true)}
								><span class="mr-1 w-6"><img class=" w-5" src={feature} alt="Star emoji" /></span>Features
							</a>
						</li>
						{/if}
						<li>
							<a
								href="https://nft.cosmicchamps.com"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								target="_blank"
								on:click={() => (hiddenMenuDropdown = true)}
								><span class="mr-1 w-6"><img class=" w-5" src={nfts} alt="Cosmic Champs nfts icon" /></span>NFTs</a
							>
						</li>
						<li>
							<a
								href="https://cards.cosmicchamps.com"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								target="_blank"
								on:click={() => (hiddenMenuDropdown = true)}
								><span class="mr-1 w-6"><img class="h-6" src={cards} alt="Cosmic Champs cards icon" /></span>Cards</a
							>
						</li>
						<li>
							<a
								href="https://cosmicchamps.com"
								class="flex w-full items-center rounded bg-gray-50 py-2 pl-3 pr-4 text-gray-900 md:bg-transparent md:p-0 md:hover:bg-transparent md:hover:text-cyan-700"
								target="_blank"
								on:click={() => (hiddenMenuDropdown = true)}><span class="mr-1 w-6"><img class=" w-5" src={about} alt="Rocket emoji" /></span>About</a
							>
						</li>
					</ul>
				</div>
			</div>
		</nav>
	</div>
</div>

<style lang="scss">
	.header-main-menu {
		background: var(--silver-gradient);
		@apply box-border rounded-bl-lg rounded-br-lg p-1;
	}
	li .active {
		@apply text-cyan-700;
	}
	.button--secondary {
		.button__inner {
			@apply flex h-full items-center justify-center px-2 py-1.5;
		}
	}
</style>
