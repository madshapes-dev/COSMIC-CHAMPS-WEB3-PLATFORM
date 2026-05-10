<script>
	import { game } from '$lib/stores/game.js';
	import DeviceDetector from 'svelte-device-detector';

	const urlInfo = $game.urlInfo;
	const urlAPK = $game.urlAPK;
	const urlTestFlight = $game.urlTestFlight;
	const urlIOS = $game.urlIOS;
	const urlWebGL = $game.urlWebGL;
</script>

<div class="mx-auto max-w-3xl">
	<div class="content">
		<h1 class="title">Play game</h1>
		<DeviceDetector showInOs="android" showInDevice={['mobile', 'tablet']}>
			<div class="mt-4 text-sm sm:text-lg md:mt-6">
				You can download the latest version of the game to your Android device using the "Download APK" button below. <br /><br />For more information
				about how to play the game please
				<a class="underline" href={urlInfo} target="_blank">read here</a>.
			</div>
			<div class="mt-8 flex justify-center">
				<a href="/" class="button button--primary mx-3 mb-5"><span class="button__inner">Back</span></a>
				<a href={urlAPK} class="button button--golden mx-3 mb-5"><span class="button__inner">Download APK</span></a>
			</div>
			<br />
			<p>Alternatively you can try our experimental web version. 8GB of ram required!</p>
			<div class="flex justify-center">
				<a class="underline" href={urlWebGL}>play in browser (experimental)</a>.
			</div>
		</DeviceDetector>
		<DeviceDetector showInOs={['ios']} showInDevice={['mobile', 'tablet']}>
			<div class="mt-4 text-sm sm:text-lg md:mt-6">
				<div class="mx-auto flex justify-center">
					<ul class="mb-3 list-inside list-decimal sm:mb-6">
						<li>
							Install TestFlight on your iOS device.
							<span class="inline-flex">
								<a href={urlTestFlight} target="_blank">Click here</a>
								<a class=" no-underline" href={urlTestFlight} target="_blank">🔗</a></span
							>
						</li>
						<li>
							Once TestFlight is installed get Cosmic Champs beta. <span class="inline-flex"
								><a href={urlIOS} target="_blank"> Click here</a>
								<a class="no-underline" href={urlIOS} target="_blank"> 🔗</a></span
							>
						</li>
						<li>Tap “Install” or “Update” when prompted.</li>
					</ul>
				</div>
				<p>
					For more information about how to play the game please <a href={urlInfo} target="_blank">read here</a>.
				</p>

				<div class="flex justify-center">
					<a href="/" class="button button--primary mx-3 mb-5"><span class="button__inner">Back</span></a>
					<a href={urlIOS} target="_blank" class="button button--golden mx-3 mb-5"><span class="button__inner">Play game</span></a>					
				</div>
				<br />
				<p>Alternatively you can try our experimental web version. 8GB of ram required!</p>
				<div class="flex justify-center">
					<a class="underline" href={urlWebGL}>play in browser (experimental)</a>.
				</div>

			</div>
		</DeviceDetector>
		<DeviceDetector showInDevice="desktop">
			<div class="my-8 text-sm sm:text-lg">
				<p>
					This is an experimental browser version of our game <br />(please visit this page on mobile device to download mobile version).
				</p>
				<!--<p>
					To play you require Algorand compatible wallet such as: <a
						class="text-cyan-300 underline"
						href="https://perawallet.app/"
						target="_blank"
						rel="noreferrer">Pera</a
					>
					or <a class="text-cyan-300 underline" href="https://defly.app/" target="_blank" rel="noreferrer">Defly</a> installed on your device.
				</p>-->

				For more information about how to play the game please
				<a class="text-cyan-300 underline" href={urlInfo} target="_blank">read here</a>.
			</div>
			<div class="flex justify-center">
				<a href="/" class="button button--primary mx-3 mb-5"><span class="button__inner">Back</span></a>
				<!--<a href="/play-web" class="button button--golden mx-3 mb-5"><span class="button__inner">Play game</span></a>-->
				<a href={urlWebGL} class="button button--golden mx-3 mb-5"><span class="button__inner">Play game</span></a>
			</div>
		</DeviceDetector>
	</div>
</div>

<style lang="scss">
	h1 {
		@apply mb-4 text-3xl sm:text-4xl md:text-5xl;
	}
	p {
		@apply mb-6;
	}

	a:not(.button):not(.no-underline) {
		@apply font-medium text-cyan-600 underline;
	}

	ul li {
		@apply my-3.5 leading-6 sm:leading-normal;
		a {
			@apply inline-flex justify-between;
			&.no-underline {
				@apply ml-1.5;
			}
		}
	}
</style>
