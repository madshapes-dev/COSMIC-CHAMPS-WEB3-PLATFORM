<script>
	import { signOut } from '@auth/sveltekit/client';
	import { page } from '$app/stores';
	import Header from '$lib/header/header.svelte';
	import Alert from '$lib/-alert.svelte';
	import logo from '$assets/logo.png';
	import '../app.scss';

	import bgTournament from '$assets/banner.webp';

	$: urlSplit = $page.url.pathname.trim($page.url.pathname.lastIndexOf('/'));
	$: pageClass = urlSplit.replace('/', '').replaceAll('/', '-');
	$: pageTitle = urlSplit.replace('/', '').replaceAll('/', ' | ');

	const metaDescription =
		'Cosmic Champs beta game. Cosmic Champs is a free-to-play, mobile first, tower rush style strategy game built on Algorand blockchain. Cosmic Champs is real time, 3D, and set in a retro space future.';
	const metaImage = `${$page.url.origin}/cosmic-champs.jpg`;
</script>

<svelte:head>
	<title>Cosmic Champs beta{pageTitle ? ` | ${pageTitle}` : ''}</title>
	<meta name="description" content={metaDescription} />
	<meta
		name="keywords"
		content="Cosmic Champs, Cosmic Champs game, Cosmic Champs beta, Cosmic Champs nft game, nft game, COG, game, champs, defi, algorand game, gameFi"
	/>
	<meta property="og:title" content="Cosmic Champs Beta release{pageTitle ? ` | ${pageTitle}` : ''}" />
	<meta property="og:type" content="website" />
	<meta property="og:url" content={$page.url} />
	<meta property="og:description" content={metaDescription} />
	<meta property="og:image" content={metaImage} />

	<meta name="twitter:card" content="summary_large_image" />
	<meta name="twitter:site" content="@site" />
	<meta property="twitter:title" content="Cosmic Champs Beta release{pageTitle ? ` | ${pageTitle}` : ''}" />
	<meta property="twitter:url" content={$page.url} />
	<meta property="twitter:description" content={metaDescription} />
	<meta property="twitter:image" content={metaImage} />
	<meta property="twitter:image:alt" content="Cosmic Champs beta" />
</svelte:head>

{#if pageClass == 'play-web'}
	<header data-sveltekit-prefetch class="relative top-0 left-0 right-0 z-40 box-border {pageClass}">
		<Header {pageClass} />
	</header>
{:else}
	<header
		data-sveltekit-prefetch
		class="{pageClass == '' ? 'fixed wide:relative wide:mb-12' : 'sticky'} top-0 left-0 right-0 z-40 box-border {pageClass}"
	>
		<Header {pageClass} />
	</header>
{/if}

{#if pageClass == '' || pageClass == 'play-web'}
	<slot />
{:else}
	<main class="{pageClass} {pageClass == '' ? 'wide:mb-12' : ''} relative" style="--bg: url({bgTournament})">
		<div class="wrapper py-8 sm:py-12">
			<div class="container-wrapper container mx-auto rounded-lg p-1 shadow-2xl {pageClass == 'leaderboards' ? '' : 'max-w-2xl'}">
				<div class="rounded-md bg-slate-50 p-3 sm:p-6">
					<slot />
				</div>
			</div>
		</div>
	</main>
{/if}

<footer>
	<div class="container mx-auto text-center text-cyan-300">
		<a href="https://cosmicchamps.com/terms.html" target="_blank" class="underline">Privacy Policy</a> |
		<a href="https://cosmicchamps.com/terms.html" target="_blank" class="underline">Terms and Conditions</a> |
		<a href="https://www.t.me/cosmicchamps" target="_blank" class="underline">Telegram</a> |
		<a href="https://discord.com/invite/cosmicchamps" target="_blank" class="underline">Discord</a>
	</div>
</footer>

<style lang="scss">
	.ram {
		@apply w-[40%] lg:absolute lg:right-16 lg:w-[300px] xl:bottom-[6rem] xl:right-8;
		animation: floating 2s ease infinite;
	}
	.fairy {
		@apply right-8 top-0;
	}
	.ship {
		@apply absolute right-8 top-0  w-[20%];
		transform: rotateZ(60deg);

		transform: perspective(800px) rotateZ(60deg);
	}
	.orb {
		@apply absolute top-0 w-[20%];
		animation: rotate 2s ease infinite;
	}
	.container-wrapper {
		background: var(--silver-gradient);
	}
	.wrapper {
		background: var(--secondary-gradient);
		max-width: none;
		min-height: 100vh;
	}
	@screen md {
		.tournament .wrapper {
			background: var(--bg), var(--secondary-gradient);
			background-repeat: no-repeat;
			background-size: contain;
			background-position: bottom;
		}
	}
	footer {
		@apply relative z-10 bg-cyan-900 py-2 text-xs;
	}
</style>
