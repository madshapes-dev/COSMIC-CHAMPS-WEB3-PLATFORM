// @ts-nocheck

export function millisecondsToReadableTime(duration) {
	let seconds = Math.floor((duration / 1000) % 60);
	let minutes = Math.floor((duration / (1000 * 60)) % 60);
	let hours = Math.floor(duration / (1000 * 60 * 60));

	// Convert duration to hours if it is greater than or equal to 24 hours
	if (hours >= 24) {
		hours = Math.floor(duration / (1000 * 60 * 60 * 24)) * 24 + (hours % 24);
	}

	let durationStr = '';

	if (hours > 0) {
		durationStr += hours + 'h ';
	}

	if (minutes > 0) {
		durationStr += minutes + 'm ';
	}

	if (seconds > 0) {
		durationStr += seconds + 's ';
	}

	return durationStr.trim();
}
// Example usage:
// var milliseconds = 1234567890;
// var readableTime = millisecondsToReadableTime(milliseconds);
// Output: "03:25:45"

// Format date
export function formatDate(dateInput) {
	let date = new Date(dateInput);

	let options = {
		month: 'short'
	};
	let day = date.getDate();
	let ordinal = '';
	if (day % 10 === 1) {
		ordinal = 'st';
	} else if (day % 10 === 2) {
		ordinal = 'nd';
	} else if (day % 10 === 3) {
		ordinal = 'rd';
	} else {
		ordinal = 'th';
	}
	let result = date.toLocaleString('en-US', options) + ' ' + day;
	return result;
}
// Example usage:
// var dateInput = '2022-12-5';
// var readableTime = formatDate(dateInput);
// Output: "Dec 5th"

export function copyToClipboard(copyTextId) {
	// Get text or input field
	var copyText = document.getElementById(copyTextId);
	// Select the input field
	copyText.select();
	copyText.setSelectionRange(0, 99999); // For mobile devices
	// Copy the text inside the input field
	navigator.clipboard.writeText(copyText.value);
}

export function deepMerge(obj1, obj2) {
	// Create a new object that combines the properties of both input objects
	const merged = {
		...obj1,
		...obj2
	};

	// Loop through the properties of the merged object
	for (const key of Object.keys(merged)) {
		// Check if the property is an object
		if (typeof merged[key] === 'object' && merged[key] !== null) {
			// If the property is an object, recursively merge the objects
			merged[key] = deepMerge(obj1[key], obj2[key]);
		}
	}

	return merged;
}

export function isTournamentLive(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());

	// Define the start and end dates of the range
	const startDate = new Date(Date.UTC(utcDate.getUTCFullYear(), 10, 23, 9, 0, 0));
	const endDate = new Date(Date.UTC(utcDate.getUTCFullYear(), 10, 30, 8, 0, 0));

	// Check if the date is within the range
	return utcDate >= startDate && utcDate < endDate;
}

export function hasTop8Started(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());

	// Define the target date (July 2nd, 00:01 UTC)
	const targetDate = new Date(Date.UTC(utcDate.getUTCFullYear(), 6, 9, 0, 1, 0));

	// Check if the date is greater than the target date
	return utcDate > targetDate;
}
export function hasTournamentStarted(date) {
	// Convert the date to UTC to ensure consistent comparison
	const utcDate = new Date(date.toISOString());

	// Define the target date (July 2nd, 00:01 UTC)
	const targetDate = new Date(Date.UTC(utcDate.getUTCFullYear(), 10, 23, 9, 0, 0));

	// Check if the date is greater than the target date
	return utcDate > targetDate;
}

export function roundNumber(number) {
	const rounded = parseFloat(number.toFixed(1));
	return rounded % 1 === 0 ? rounded.toFixed(0) : rounded.toFixed(1);
}
