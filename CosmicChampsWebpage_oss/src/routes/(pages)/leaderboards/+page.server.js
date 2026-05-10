// @ts-nocheck
import { fetchDataFromDynamoDB } from '$lib/server/dynamodb';
import tournaments from './data/tournaments.json';
import { get } from 'svelte/store';
import { tournament } from '$lib/stores/tournament.js';

const $tournament = get(tournament);

/** @type {import('./$types').LayoutServerLoad} */
export async function load({ fetch }) {
	const today = new Date();
	const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
	const year = thirtyDaysAgo.getFullYear();
	const month = ('0' + (thirtyDaysAgo.getMonth() + 1)).slice(-2);
	const day = ('0' + thirtyDaysAgo.getDate()).slice(-2);
	const date = year + '-' + month + '-' + day;

	let tournamentData = [];
	if ($tournament.started) {
		const dynamodb = await fetchDataFromDynamoDB();
		tournamentData = JSON.stringify(dynamodb);
	}

	let tournamentsData =  tournaments.map(async (tournament) => {
		const { data } = tournament;
		const filePath = `data/${data}.json`;
    const response = await fetch(filePath);
		let jsonData	= [];
      if (response.ok) {
      	jsonData = await response.json();	
      }
		return {...tournament, data: jsonData}; 
	});
	tournamentsData = await Promise.all(tournamentsData);

	return { date, tournament:tournamentData, tournaments:tournamentsData };
}
