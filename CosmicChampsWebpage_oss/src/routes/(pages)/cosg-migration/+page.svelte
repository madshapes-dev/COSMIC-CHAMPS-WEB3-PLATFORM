<script>
	import { fade } from 'svelte/transition';
	import { wallet } from '$lib/stores/wallet.js';

	let message = '';
	let status = '';
	let spinner = false;
	let hidden = true;
	let hideSubmit = false;

	async function validate() {
		spinner = true;
		let data;
		const response = await fetch('/api/migration/cosg', {
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
	// let address = 'AAAHGK7IPSIY7633IJ7PZADSNXU6K4FJFIHB7SEO2YQJYWLWOFGHT5MTAL';

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
	<h1 class="title">
		COSG v2 <br /><span class="font-sm title--size-4">COSG Migration</span>
	</h1>

	<div class="mb-3">Enter wallet address or NFDomain and see if you qualify for COSG v2 reimbursement:</div>
	<form on:submit|preventDefault class="flex flex-col items-center">
		<input bind:value={address} type="text" placeholder="Your wallet address..." class="mb-6" required />
		{#if !hideSubmit}
			<button
				on:click={() => {
					if (address) validate();
				}}
				class="button button--primary"
				type="submit"
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
		{:else if hidden}
			<svg
				in:fade|global
				out:fade|global={{ duration: 50 }}
				class="mx-auto ml-auto h-10 w-10 animate-spin text-white"
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
	<div class="mt-6 flex w-full justify-center">
		<a href="/lp-migration" class="button button--primary"><span class="button__inner">Check my LP compensation</span></a>
	</div>
</div>

<style lang="scss">
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
		@apply flex items-start rounded-md border-2 border-2 border-cyan-500 bg-cyan-900 p-6 text-left font-medium text-cyan-200 md:items-center md:p-10 md:text-base;
		:global(ul) {
			@apply mt-3 ml-1 list-inside list-disc;
			:global(li) {
				@apply my-0.5 pl-2;
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
</style>
