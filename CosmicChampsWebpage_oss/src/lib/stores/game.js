import { readable } from 'svelte/store';

const gameData = {
	title: 'Play, Conquer and EARN!',
	releaseDate: '17 Champs, 200+ unique NFTs & Unlimited Fun!',
	subtitleText: 'Available for iOS, Android & Web',
	version: 'v0.19 (b190000)',
	test: 'TestB',
	urlAPK: 'https://s3.eu-central-1.amazonaws.com/beta.cosmicchamps/CosmicChamps-beta_0.19.0-b190000.apk',
	urlTestFlight: 'https://apps.apple.com/us/app/testflight/id899247664',
	urlIOS: 'https://testflight.apple.com/join/6ma9EwJB',
	urlInfo: 'https://medium.com/cosmic-champs/getting-started-with-cosmic-champs-eb58857c9e4a',
	urlWebGL: 'https://s3.eu-central-1.amazonaws.com/beta.cosmicchamps/WebGL/TestB/index.html'
};

export const game = readable(gameData);

/*
const gameData = {
	title: 'Introducing new Champ - STRIKER!',
	releaseDate: 'Latest update: March 15th',
	version: 'v0.14.0 (b140)',
	test: 'TestA',
	urlAPK: 'https://s3.eu-central-1.amazonaws.com/beta.cosmicchamps/CosmicChamps-beta_b140.apk',
	urlTestFlight: 'https://apps.apple.com/us/app/testflight/id899247664',
	urlIOS: 'https://testflight.apple.com/join/9qke1ptW',
	urlInfo: 'https://medium.com/cosmic-champs/getting-started-with-cosmic-champs-eb58857c9e4a'
};
*/