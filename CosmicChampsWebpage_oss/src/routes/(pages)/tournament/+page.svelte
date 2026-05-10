<script>
	import { fade } from 'svelte/transition';
	import { invalidateAll } from '$app/navigation';
	import { applyAction } from '$app/forms';
	import { wallet } from '$lib/stores/wallet.js';
	import { isTournamentLive } from '../leaderboards/helpers';
	import bgTournament from '$assets/bg/tournament-sm.webp';

	let success = false;
	let blockchains = 'No';

	const dateToCheck = new Date();
	const tournamentLive = isTournamentLive(dateToCheck);
</script>

<div class="content">
	{#if tournamentLive}<span class="animate-pulse rounded-sm px-0.5 font-black uppercase text-cyan-700">live now</span>{/if}
	<div class="flex flex-wrap">
		<h1 class="title title--gradient mr-2">Tournament</h1>
		<span class="title title--size-4 title--golden">Oct 23 - 30</span>
	</div>
	<ul class="mb-6">
		<li><strong>start:</strong> 23rd October 09:00AM UTC</li>
		<li><strong>end:</strong> 30th October 09:00AM UTC</li>
		<li><strong>prize pool:</strong> 5000 ALGOs</li>
		<li><strong>ranking criteria:</strong> <a href="/leaderboards">leaderboard score</a></li>
	</ul>
	<h3 class="title title--size-4 font-bold">How will the tournament work?</h3>
	<p class="-mt-8">
		The tournament is open to anyone with an Algorand wallet and device that can run our beta game. The tournament will run over 7 days from 9 AM on
		the 23rd of October until 9 AM on the 30th of October. The tournament is unstructured so battle as much as you like to earn points on the
		leaderboard. For every win you will earn points and for every loss you will lose points. The number of points won or lost will depend on the rank
		of your opponent. For example, if you beat someone with a higher rank then the points won will be greater than if you beat someone with a lower
		rank.
	</p>
	<p>
		You can access the beta game here:
		<a href="https://beta.cosmicchamps.com">https://beta.cosmicchamps.com</a>. All you need is an internet connection and smart iOS or Android device
		or a PC.
	</p>
	<p>
		If you need help constructing or fine-tuning your deck then check out our new card comparison tool:
		<a href="https://cards.cosmicchamps.com/compare" target="_blank">https://cards.cosmicchamps.com/compare</a>.
	</p>

	<h3 class="title title--size-4 font-bold">What are the prizes?</h3>
	<p class="-mt-8">
		When the tournament ends, the top 10 on the leaderboard will share in 3000 ALGOs. The next 30 places on the leaderboard will share in the
		remaining 2000 ALGOs. The higher you climb on the leaderboard the greater your chances of earning a bag of loot. During the tournament, you can
		view the live leaderboard here: <a href="https://beta.cosmicchamps.com/leaderboards">https://beta.cosmicchamps.com/leaderboards</a>. The
		leaderboard will show the prize amount for each place.
	</p>
	<p>
		We will also have two bonus prizes. The first player to reach 1300 points will receive a 50 ALGO bonus and the first player to reach 1400 points
		will receive a 100 ALGO bonus. To claim this prize you’ll need to post a screenshot of the leaderboard with the score and your wallet to our
		Discord server: <a href="https://discord.gg/cosmicchamps" target="_blank">https://discord.gg/cosmicchamps </a>.
	</p>
	<p>
		All prizes are paid half in ALGO and half in <a href="https://app.tinyman.org/#/swap?asset_in=0&asset_out=1065092715" target="_blank">COSG</a>. If
		you aren't opted in to the COSG at the time of distribution, you forfeit all rewards.
	</p>
	<p class="mt-6">
		<a href="/leaderboards" class="button button--primary flex w-full text-center md:w-auto mb-1.5 mb-2 md:mt-1.5"
			><span class="button__inner">Check leaderboards</span></a
		>
		<a href="/play" target="_blank" class="button button--primary flex w-full text-center md:w-auto mb-1.5 md:ml-2 mt-2 md:mt-0"
			><span class="button__inner">Play</span></a
		>
	</p>
	<div class="-mx-3 -mb-3 sm:-mx-6 sm:-mb-6 md:hidden">
		<img class="rounded-md" src={bgTournament} alt="Cosmic Champs tournament" />
	</div>
</div>

<style lang="scss">
	.title.title--gradient {
		@apply mb-4 text-4xl sm:mb-6 sm:text-5xl;
	}
	.title--size-4 {
		@apply text-sm md:text-lg;
	}
	p {
		@apply mb-4;
	}
	b {
		@apply font-normal text-cyan-700;
	}
	a:not(.button) {
		@apply text-cyan-600 underline;
	}
</style>
