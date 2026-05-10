import { browser } from '$app/environment';
import { writable } from 'svelte/store';

let initValue = [];
if (browser) {
	const localStorage = window.localStorage.getItem('wallet');
	if (localStorage) {
		initValue = JSON.parse(localStorage);
	}
}
export const wallet = writable(initValue);
