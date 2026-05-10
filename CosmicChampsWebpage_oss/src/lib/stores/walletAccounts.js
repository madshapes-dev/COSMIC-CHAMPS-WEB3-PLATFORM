import { browser } from '$app/environment';
import { writable } from 'svelte/store';

let initValue = [];
if (browser) {
	const localStorage = window.localStorage.getItem('accounts');
	if (localStorage) {
		initValue = JSON.parse(localStorage);
	}
}
export const walletAccounts = writable(initValue);
