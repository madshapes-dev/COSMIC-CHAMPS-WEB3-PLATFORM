/** @type {import('tailwindcss').Config} */
module.exports = {
	content: ['./src/**/*.{html,js,svelte,ts}'],
	darkMode: 'class',
	theme: {
		extend: {
			colors: {
				transparent: 'transparent'
			},
			screens: {
				xs: '475px',
				'2xl': '1536px',
				tall: {
					raw: `only screen and (max-height: 960px) and (max-width: 480px)`
				},
				wide: {
					raw: `only screen and (max-height: 480px) and (max-width: 960px)`
				},
				portrait: {
					raw: '(orientation: portrait)'
				},
				landscape: {
					raw: '(orientation: landscape)'
				}
			}
		},
		mode: 'jit',
		tailwindConfig: './tailwind.config.cjs',
		plugins: [require('@tailwindcss/forms')]
	}
};
