<script>
	import { onMount } from 'svelte';
	import { game } from '$lib/stores/game.js';
	import { tournament } from '$lib/stores/tournament.js';
	import video from '$assets/cosmic_champs_beta_video.mp4';
	import placeholder from '$assets/cosmic-champs.jpg';
	import sprites from '$assets/sprites3.png';
	import spritesSm from '$assets/sprites3-300.png';

	import image from '$assets/bg/tires.webp';
	import imageTournament from '$assets/emoji/tournament.webp';

	const logoChamps = 'logo-192.png';
	const tournamentLive = $tournament.live;
	let buttonVisible = true;

	onMount(() => {
		const element = document.getElementById('typewriter');
		const typewriter = document.querySelector('.typewriter');

		function startAnimation() {
			element.classList.add('animate');
		}

		function stopAnimation() {
			element.classList.remove('animate');
		}

		// Define the class names to toggle
		const text1 = 'text1';
		const text2 = 'text2';
		const text3 = 'text3';

		function toggleClasses() {
			startAnimation();
			if (element.classList.contains(text1)) {
				element.classList.remove(text1);
				element.classList.add(text2);
			} else if (element.classList.contains(text2)) {
				element.classList.remove(text2);
				element.classList.add(text3);
			} else {
				element.classList.remove(text3);
				element.classList.add(text1);
			}
		}
		setInterval(toggleClasses, 8000);
		setInterval(stopAnimation, 5000);
	});
</script>

<div class="video-foreground">
	<video width="100%" height="100%" loop muted autoplay playsinline class="video-background">
		<source src={video} type="video/mp4" />
	</video>
	<div class="video-overlay" />
</div>

<div class="container relative z-10 m-auto flex h-full items-center">
	<div class="mt-28 mb-6 flex w-full flex-col items-end justify-between text-center md:mt-auto md:items-center lg:mt-0 lg:flex-row lg:text-left">
		<div class="px-4 lg:w-2/4">
			{#if $tournament.show}
				<h2 class="title--main title">{$tournament.title}</h2>
				<div class="title title--light flex justify-center lg:justify-start">
					{$tournament.date}
					{#if tournamentLive}<span class="ml-4 mr-0.5 font-black uppercase text-cyan-200">live now</span>
						<div class="h-3 w-3 animate-pulse rounded-full bg-yellow-700" />{/if}
				</div>
			{:else}
				<h2 class="title--main title">{$game.title}</h2>
				<!--<div class="title title--light">{$game.releaseDate} - {$game.subtitleText}</div>-->
				<div class="title title--light">{$game.releaseDate} <br /> {$game.subtitleText}</div>
			{/if}

			<div class=" flex flex-wrap justify-center items-center lg:justify-start">
				<a
					href="https://medium.com/cosmic-champs"
					class="button button--primary button-changelog"
					class:tournament={$tournament.show}
					target="_blank"
					rel="noreferrer"
				>
					<span class="button__inner py-0.5">Blog</span>
				</a>
				<a href="/cosg-v2" class="button button--primary"><span class="button__inner">COSG V2 info</span></a>
				<a href="/tiers" class="button button--primary"><span class="button__inner">Holder tiers</span></a>
				{#if $tournament.show}
					<a href="/tournament" class="button button--secondary button-image button-tournament order-3 lg:order-1"
						><span class="button__inner flex justify-center">Tournament info<img src={imageTournament} alt="Tournament" /></span></a
					>
				{/if}
				<!-- <a href="/tiers" class="button button--secondary button-image"
					><span class="button__inner flex justify-center">Holder tiers <img src={image} alt="PP Ram" /></span></a
				> -->
			</div>
		</div>
		<div class="relative mx-auto mt-6 flex flex-col items-center justify-center md:mb-8 md:mt-12 lg:w-2/4 lg:flex-row">
			<div
				class="ram flex flex-col items-center justify-center"
				class:animation={!sprites && !spritesSm}
				style=" --sprites: url({sprites}); --spritesSm:url({spritesSm});"
			>
				<div class="speech-bubble">
					<div class="typewriter text1 flex" id="typewriter">
						<div class="text text1 flex">Let's play <img class="ml-1 mr-0.5 w-6" src={logoChamps} alt="Cosmic Champs beta" />!</div>
						<div class="text text2 flex">Battle awaits!</div>
						<div class="text text3 flex">Can you win?</div>
					</div>
				</div>

				<div class="sprites" />
			</div>
			<div class="flex-col items-center justify-center md:flex">
				<a href="/play" class="button button--secondary button--play relative">
					<div class="button__inner text-center">
						Play
						<div class="text-xs md:mt-2 md:-mb-3">{$game.version}</div>
					</div>
				</a>
				<a
					href="https://medium.com/cosmic-champs/getting-started-with-cosmic-champs-eb58857c9e4a"
					class="relative mt-1 block font-medium text-cyan-400 underline underline-offset-2"
					target="_blank">How to?</a
				>
			</div>
		</div>
	</div>
</div>

<style lang="scss">
	.button-image {
		@apply relative;
		.button__inner {
			@apply flex items-center justify-between;
			img {
				@apply absolute -right-3 -top-3 z-10 h-8 md:-right-2 md:h-8 lg:-right-3 lg:h-14;
				animation: floatingRotate 2s ease infinite;
			}
		}
		&:hover img {
			animation-play-state: paused;
		}
	}
	.button-changelog.tournament {
		@apply hidden sm:block;
	}
	.button-tournament {
		.button__inner {
			@apply py-3 text-lg lg:py-5;
		}
	}
	@screen lg {
		.button-tournament {
			.button__inner {
				@apply text-lg lg:px-10 #{!important};
			}
		}
	}

	.ram {
		@apply select-none lg:absolute lg:-right-20 lg:-top-16 lg:w-[60%] xl:-right-10 xl:-top-[6rem];
		animation: floating 3s ease infinite;

		&.animation {
			@apply blur-3xl;
			animation:
				blur 0.5s ease 0.5s forwards,
				floating 3s ease infinite;
		}
	}

	.speech-bubble {
		position: relative;
		width: 70%;
		text-align: center;
		line-height: 1.4em;
		@apply max-w-[150px] rounded-md  bg-white px-3 py-2 text-cyan-50 shadow-sm sm:p-3 md:mx-auto md:my-9;
		animation: floatingBig 5s ease infinite;
		background: var(--highlight-gradient-shade);
	}
	.speech-bubble:before,
	.speech-bubble:after {
		content: '';
		position: absolute;
		width: 0;
		height: 0;
	}
	.speech-bubble:before {
		right: 60px;
		bottom: -40px;
		@apply -bottom-6 right-24 h-4 w-4 rounded-full bg-white p-1 shadow-sm md:-bottom-8 md:right-16 lg:-bottom-8 lg:h-5 lg:w-5 xl:-bottom-12 xl:right-14 xl:h-8 xl:w-8;
		animation: floating 1.8s ease infinite alternate;
		background: var(--highlight-gradient-shade);
	}
	.speech-bubble:after {
		width: 10px !important;
		height: 10px;
		right: 45px;
		bottom: -55px;
		@apply rounded-full  bg-white p-1 shadow-sm md:-bottom-10 md:right-9 xl:-bottom-16 xl:right-8;
		animation: floatingBig 2s ease infinite;
		background: var(--highlight-gradient-shade);
	}

	.sprites {
		transform-origin: top center;

		height: 500px;
		width: 500px;
		// background: url('/sprites3.png');
		background: var(--sprites);
		animation: sprite 2.9s steps(42) infinite;
		transform: scale(0.8);

		@media screen and (max-width: 1120px) {
			height: 270px;
			width: 250px;
			// background: url('/sprites3-300.png');
			background: var(--spritesSm);
			animation: spriteSm 2s steps(42) infinite;
			transform: scale(0.65);
			margin-bottom: -120px;
		}
	}
	@keyframes sprite {
		from {
			background-position: 0px;
		}
		to {
			background-position: -21000px;
		}
	}
	@keyframes spriteSm {
		from {
			background-position: 0px;
		}
		to {
			background-position: -12600px;
		}
	}
	@keyframes blur {
		from {
			filter: blur(50px);
		}
		to {
			filter: blur(0);
		}
	}

	.video-background {
		position: fixed;
		left: 50%;
		top: 50%;
		min-width: 100%;
		min-height: 100%;
		width: auto;
		height: auto;
		transform: translate(-50%, -50%);
		z-index: -100;
		background: black;
		max-width: none;
	}
	.video-overlay {
		position: fixed;
		width: 100%;
		height: 100%;
		z-index: 90;
		@apply bottom-0 left-0 right-0 top-0 z-10 bg-black opacity-30;
	}
	.container {
		@apply lg:min-h-[630px];
	}
	.button {
		@apply mx-1 mt-2.5 h-full;
		.button__inner {
			@apply px-2.5 text-xs sm:px-4 sm:text-base;
		}
	}
	.title--main {
		@apply mb-1 text-4xl lg:text-6xl xl:text-7xl xs:text-5xl;
		line-height: 1.3;
	}
	.title--light {
		@apply mb-6 text-lg sm:text-2xl;

		.animate-pulse {
			@apply bg-cyan-900 text-cyan-900;
		}
	}

	.button--play {
		@apply w-full p-1 md:w-auto md:rounded-md md:rounded-bl-3xl  md:rounded-tr-3xl;
		box-shadow: 0 0 10px rgb(14 116 144 / 90);
		.button__inner {
			@apply w-full overflow-hidden px-16 text-xl text-cyan-800 shadow-md transition-all md:rounded-lg md:rounded-bl-3xl md:rounded-tr-3xl md:py-6 md:text-4xl lg:px-12;
		}
	}
</style>
