import adapter from '@sveltejs/adapter-vercel';
import preprocess from 'svelte-preprocess';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({ runtime: 'nodejs22.x' }),
		alias: {
			$assets: './src/routes/assets',
			$api: '.src/routes/api'
		}
	},
	preprocess: [
		preprocess({
			postcss: true
		})
	]
};

export default config;
