// @ts-nocheck
import { getSheetsService } from '../service.js';
import { VITE_GOOGLE_DRIVE_SHEET_TIERS_ID } from '$env/static/private';
import { json } from '@sveltejs/kit';

/** @type {import('./$types').RequestHandler} */
export async function POST({ request }) {
	let { address } = await request.json();

	const nfd = await fetch(`https://api.nf.domains/nfd/${address.toLowerCase()}`);
	const nfdData = await nfd.json();
	if (nfdData.depositAccount) address = nfdData.depositAccount;

	// googleapis
	const sheets = getSheetsService();
	const spreadsheetId = VITE_GOOGLE_DRIVE_SHEET_TIERS_ID;
	const sheetsRequest = {
		spreadsheetId,
		range: 'A2:G'
	};
	try {
		let result = (await sheets.spreadsheets.values.get(sheetsRequest)).data.values;
		let responses = result.slice(0, 5);
		let text = result.slice(6, 7);
		let addresses = result.slice(7);
		let response = { message: responses[0], status: 'invalid' };
		addresses.forEach((item, i) => {
			let addr = item[0] ? item[0].toString().toUpperCase() : '';
			if (address.toString().trim().toUpperCase() == addr) {
				let messageMain;
				if (item[1] == 1) messageMain = responses[1][0];
				else if (item[1] == 2) messageMain = responses[2][0];
				else if (item[1] == 3) messageMain = responses[3][0];
				else if (item[1] == 4) messageMain = responses[4][0];

				let wallet = Math.floor(item[3] / 1000000) > 0 ? `${Math.floor(item[3] / 1000000)} ${text[0][3]}` : '';
				let staked = Math.floor(item[4] / 1000000) > 0 ? `, ${Math.floor(item[4] / 1000000)} ${text[0][4]}` : '';
				let lp = Math.floor(item[5] / 1000000) > 0 ? `, ${Math.floor(item[5] / 1000000)} ${text[0][5]}` : '';
				let message = `${messageMain} <p class="text-xs mt-4">Your calculated COSG holdings based on our snapshots for current period: ${Math.floor(
					item[6] / 1000000
				)} COSG (${wallet}${staked}${lp}).</p>`;

				if (message) response = { message, status: 'valid' };
			}
		});
		return json(response);
	} catch (err) {
		throw err;
	}
}
