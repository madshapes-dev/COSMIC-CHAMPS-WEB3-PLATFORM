<script>
	import { clickOutside, toKebabCase } from '$lib/helpers.js';
	import { page } from '$app/stores';

	export let id;
	export let hidden = true;

	const className = toKebabCase(id);
</script>

<slot name="dropdown-toggle" />
{#if !hidden}
	<div
		{id}
		class="{className} dropdown absolute inset-x-0 top-0 z-[150] hidden origin-top-right scale-75 transform p-2 transition"
		class:opened={!hidden}
	>
		<div class="divide-y-2 divide-gray-50 rounded-lg bg-white shadow-lg ring-1 ring-black ring-opacity-5 dark:divide-cyan-700 dark:bg-cyan-900">
			<slot name="dropdown" />
		</div>
	</div>
{/if}

<style lang="scss" global>
	.dropdown {
		@apply invisible;

		&.menu-wallet {
			@apply visible top-full left-auto w-[230px] scale-100 p-0 opacity-100 duration-100 ease-in;
			.amount-number {
				@apply ml-2 h-4 px-1 text-right;
				&__hidden {
					@apply rounded-sm bg-gray-200 text-transparent dark:bg-cyan-700;
				}
			}
			.icon {
				@apply ml-2 flex h-8  items-center justify-center p-1 text-cyan-600 hover:text-gray-400 dark:text-cyan-300 dark:hover:text-cyan-500;
			}
		}
		&.menu-mobile {
			@apply top-10;
		}
		&.opened {
			@apply visible block scale-100 opacity-100 duration-100 ease-in;
		}
		button,
		a {
			@apply pointer-events-auto;
		}
		.active {
			@apply bg-gray-100 dark:bg-cyan-600;
		}
	}

	// wallet dropdown
	.dropdown-toggle.active {
		.button__inner {
			@apply text-cyan-900 opacity-80;
			background: var(--highlight-gradient-secondary);
		}
	}
	.dropdown-toggle {
		@apply mt-0;
	}
</style>
