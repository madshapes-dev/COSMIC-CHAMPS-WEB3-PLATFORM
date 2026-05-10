<script>
	import { fade } from 'svelte/transition';
	import { wallet } from '$lib/stores/wallet.js';
	import EyeOn from '$assets/svg/eyeOn.svelte';
	import EyeOff from '$assets/svg/eyeOff.svelte';
	export let form;
	$: form = form;

	let hiddenPassword = true;
	let email = '';
	let password = '';

	function showPassword() {
		var input = document.getElementById('password');
		if (input.type === 'password') input.type = 'text';
		else input.type = 'password';
	}
	function connectWallet() {
		const element = document.getElementById('button-modal');
		if (element) element.click();
	}
</script>

<div class="content">
	<div class="flex justify-between">
		<h1 class="title">Wallet Sync</h1>
	</div>
	<fieldset class=" mb-3">
		<legend class="title--gradient">1. Connect wallet</legend>
		{#if !$wallet.address}
			<p class="mb-4">Please connect an Algorand compatible wallet you use in-game.</p>
			<a href="/" class="button button--primary mb-3 flex w-full lg:w-3/4 text-center mx-auto" on:click|preventDefault|stopPropagation={connectWallet}
				><span class="button__inner">Connect wallet</span></a
			>
		{:else}
			<div class="block">
				<div class="label-title">Wallet address</div>
				{`${$wallet.address.substring(0, 8)}...${$wallet.address.slice(-8)}`}
			</div>
		{/if}
	</fieldset>
	<fieldset class="pb-4">
		<legend class="title--gradient">2. Sign In</legend>
		{#if form?.success === true}
			<div class="alert flex-col" in:fade|global out:fade|global={{ duration: 100 }}>
				<p class="md:text-center font-medium">
					Wallet successfully synced 👍. <br />Please restart or refresh your game and you are good to go!
				</p>
			</div>
		{:else if !$wallet.address}
			<p class="mb-3">Please complete first step (Connect wallet).</p>
		{:else}
			<p class="mb-4">Please Sign In with your in-game credentials.</p>
			<form method="POST" class="flex flex-col items-center">
				<input type="hidden" name="wallet" value={$wallet.address} />
				<label class="block w-full lg:w-3/4">
					<div class="label-title">Email address</div>
					<input name="email" type="email" class="w-full" bind:value={email} required />
				</label>
				<label class="block w-full lg:w-3/4 relative">
					<div class="label-title">Password</div>
					<input id="password" name="password" type="password" class="w-full" bind:value={password} required />
					<a
						href="/"
						on:click|preventDefault|stopPropagation={() => {
							showPassword();
							hiddenPassword = !hiddenPassword;
						}}
						class="icon absolute right-2 top-9 md:top-11"
					>
						{#if hiddenPassword}
							<EyeOff classList="h-5 w-5 text-gray-400" />
						{:else}
							<EyeOn classList="h-5 w-5 text-gray-400" />
						{/if}
					</a>
				</label>
				{#if form?.success === false}
					<div class="text-red-500 -mt-3 mb-4 mr-auto block lg:mx-auto lg:w-3/4">⚠️ Invalid email or password.</div>
					<div class="-mt-3 mb-4 mr-auto block lg:mx-auto lg:w-3/4">
						If you haven't registered yet, please create a new game account:
						<a href="https://beta.cosmicchamps.com/play" class="underline">beta.cosmicchamps.com/play</a>
					</div>
				{/if}
				<button class="button button--primary" type="submit"><span class="button__inner"><span class="mx-auto">Submit</span></span></button>
			</form>
		{/if}
	</fieldset>
</div>

<style lang="scss">
	a.button {
		@apply block;
	}
	button {
		@apply w-full sm:w-56;
		.button__inner {
			@apply flex items-center justify-around;
		}
	}
	input {
		@apply mb-0;
	}
	h1 {
		@apply text-3xl sm:text-4xl md:text-5xl;
	}
</style>
