<script>
	import { fade } from 'svelte/transition';
	import { wallet } from '$lib/stores/wallet.js';
	import image from '../../assets/bg/tires.webp';

	export let data;

	let message = '';
	let status = '';
	let spinner = false;
	let hidden = true;
	let hideSubmit = false;

	async function validate() {
		spinner = true;
		let data;
		const response = await fetch('/api/tiers', {
			method: 'POST',
			body: JSON.stringify({ address }),
			headers: {
				'content-type': 'application/json'
			}
		});
		data = await response.json();
		message = data.message;
		status = data.status;
		hidden = false;
	}

	$: addressSelected = $wallet.address ? $wallet.address : '';
	$: address = addressSelected ? addressSelected : '';
	$: {
		if ($wallet.address === address) {
			validate();
			hideSubmit = true;
		} else if (address) {
			hidden = true;
			spinner = false;
			hideSubmit = false;
		}
	}
</script>

<div class="content">
	<div class="flex justify-between">
		<h1 class="title">
			COSG Holder Tier <br /><span class="font-sm title--size-4">Current month eligibility</span>
		</h1>
		<a href="https://nft.cosmicchamps.com/nft/1108380528" target="_blank">
			<img src={image} alt="PP Ram" class="h-auto max-w-[65px] sm:max-w-[80px]" />
		</a>
	</div>
	{#if data.read_more}
		<div class="-mt-7 mb-6"><a href={data.read_more} target="_blank">Read more</a> about COSG holder tiers.</div>
	{/if}
	<div class="mb-3">Enter your wallet address or NFDomain and check if you are eligible:</div>

	<form
		on:submit|preventDefault={() => {
			if (address) validate();
		}}
		class="flex flex-col items-center"
	>
		<input
			bind:value={address}
			type="text"
			placeholder="Your wallet address..."
			class="mb-6"
			pattern="[a-zA-Z0-9]+|[a-zA-Z0-9]+\.algo|[a-zA-Z0-9]+\.ALGO"
			minlength="6"
			maxlength="58"
			required
		/>
		{#if !hideSubmit}
			<button class="button button--primary" type="submit"
				><span class="button__inner"
					><span class="mx-auto">Submit</span>
					{#if spinner && hidden}
						<svg
							in:fade|global
							out:fade|global={{ duration: 50 }}
							class="ml-auto h-5 w-5 animate-spin text-white"
							xmlns="http://www.w3.org/2000/svg"
							fill="none"
							viewBox="0 0 24 24"
						>
							<circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
							<path
								class="opacity-75"
								fill="currentColor"
								d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
							/>
						</svg>
					{/if}
				</span></button
			>
		{/if}
	</form>
	{#if address && !hidden}
		<div class="alert" in:fade|global out:fade|global={{ duration: 100 }}>
			<span class="icon button button--primary" class:icon--error={status == 'invalid'}>
				{#if status == 'valid'}
					<svg
						xmlns="http://www.w3.org/2000/svg"
						fill="none"
						viewBox="0 0 24 24"
						stroke-width="1.5"
						stroke="currentColor"
						class="button__inner h-7 w-7"
					>
						<path stroke-linecap="round" stroke-linejoin="round" d="M4.5 12.75l6 6 9-13.5" />
					</svg>
				{:else if status == 'invalid'}
					<svg
						xmlns="http://www.w3.org/2000/svg"
						fill="none"
						viewBox="0 0 24 24"
						stroke-width="1.5"
						stroke="currentColor"
						class="button__inner h-7 w-7"
					>
						<path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
					</svg>
				{/if}
			</span>
			<span>{@html message}</span>
		</div>
	{/if}
</div>

<style lang="scss">
	img {
		animation: floating 3s ease infinite;
		&:hover {
			animation-play-state: paused;
		}
	}
	.icon {
		@apply mr-3 cursor-none rounded-full md:mr-6;
		svg {
			@apply rounded-full p-1;
		}
		&--error svg {
			background: red !important;
		}
	}
	.alert {
		@apply flex items-start justify-center rounded-md border-2 border-2 border-cyan-500 bg-cyan-900 p-6 text-left font-medium text-cyan-200 md:items-center md:p-10 md:text-base;
		:global(a) {
			@apply underline;
		}
		:global(ul) {
			@apply mt-3 list-inside list-disc text-sm;
			:global(li) {
				@apply my-0.5;
			}
		}
	}
	input {
		@apply w-full rounded border-2 border-cyan-500 bg-black p-2 text-left text-sm text-cyan-900 placeholder:text-cyan-900 lg:w-[610px] lg:text-base;
		background: var(--highlight-gradient-secondary);
	}
	button {
		@apply mb-6 w-full sm:w-56;
		.button__inner {
			@apply flex items-center justify-around;
		}
	}
	h1 {
		@apply text-3xl sm:text-4xl md:text-5xl;
	}
	a {
		@apply text-cyan-600 underline;
	}
</style>
