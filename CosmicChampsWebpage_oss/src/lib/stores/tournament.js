import { readable } from 'svelte/store';

const dateToCheck = new Date();
// const dateToCheck = new Date('Oct 23 2023 11:55:00 UTC');
const start = 'Oct 23 2023 09:00:00 UTC';
const end = 'Oct 30 2023 09:00:00 UTC';
const startDate = new Date(Date.UTC(dateToCheck.getUTCFullYear(), 9, 23, 9, 0, 0));
const endDate = new Date(Date.UTC(dateToCheck.getUTCFullYear(), 9, 30, 9, 0, 0));

function isTournamentLive(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());
	return utcDate >= startDate && utcDate < endDate;
}

function hasTournamentStarted(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());
	return utcDate > startDate;
}
function hasTop8Started(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());
	// Define the target date (July 2nd, 00:01 UTC)
	const targetDate = new Date(Date.UTC(utcDate.getUTCFullYear(), 6, 9, 0, 1, 0));
	// Check if the date is greater than the target date
	return utcDate > targetDate;
}

const tournmentData = {
	show: false,
	start,
	end,
	title: 'Join the Tournament!',
	date: 'October 23 - 30',
	live: isTournamentLive(dateToCheck),
	started: hasTournamentStarted(dateToCheck),
	top8Started: hasTop8Started(dateToCheck)
};

export const tournament = readable(tournmentData);
