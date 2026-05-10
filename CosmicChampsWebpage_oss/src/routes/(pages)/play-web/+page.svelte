<script>
	import { browser } from '$app/environment';
	import { game } from '$lib/stores/game.js';
	const title = 'Cosmic Champs beta';
	const src = `https://s3.eu-central-1.amazonaws.com/beta.cosmicchamps/WebGL/${$game.test}/index.html`;

	$: if (browser) document.body.classList.add('overflow-hidden');
</script>

<svelte:head>
	<meta http-equiv="Content-Security-Policy" content="upgrade-insecure-requests" />
</svelte:head>
<iframe id="game" {title} {src} frameborder="0" allowfullscreen allow="insecure" width="100" height="100" />

<style lang="scss">
	iframe {
		@apply relative h-full w-full max-w-full -translate-y-16 transform overflow-clip;
	}
</style>
