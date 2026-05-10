<script>
	import logo from './assets/logo.png';
	import '../app.scss';
	import { page } from '$app/stores';
</script>

<main class="wrapper">
	<div class="container mx-auto my-auto px-3 text-center">
		<h1 class="title">Oh no, lost in Cosmos... <br /> <span class="text-4xl">{$page.status}: {$page.error.message}</span></h1>
		<a href="/" class="button button--primary"><span class="button__inner text-xl">Go back home</span></a>
	</div>
</main>

<style>
	.wrapper {
		width: auto;
		height: 100%;
		min-height: 600px;
		position: relative;
		background-color: black;
		background: url(../routes/assets/bg/bg-1.webp) no-repeat;
		background-size: cover;
		@apply flex justify-center rounded-lg;
	}
</style>
