// @ts-nocheck
import { getSheetsService } from '../../service.js';
import { VITE_GOOGLE_DRIVE_SHEET_LP_MIGRATION_ID } from '$env/static/private';
import { json } from '@sveltejs/kit';

/** @type {import('./$types').RequestHandler} */
export async function POST({ request }) {
	let { address } = await request.json();

	const nfd = await fetch(`https://api.nf.domains/nfd/${address}`);
	const nfdData = await nfd.json();
	if (nfdData.depositAccount) address = nfdData.depositAccount;

	// googleapis
	const sheets = getSheetsService();
	const spreadsheetId = VITE_GOOGLE_DRIVE_SHEET_LP_MIGRATION_ID;
	const sheetsRequest = {
		spreadsheetId,
		range: 'B2:H'
	};
	try {
		let result = (await sheets.spreadsheets.values.get(sheetsRequest)).data.values;
		let responses = result.slice(0, 2);
		let text = result.slice(5, 6);
		let addresses = result.slice(6);
		let response = { message: responses[0], status: 'invalid' };
		addresses.forEach((item, i) => {
			let addr = item[0] ? item[0].toString().toUpperCase() : '';
			if (address.toString().trim().toUpperCase() == addr) {
				let tinyman = Math.floor(item[1] / 1000000) > 0 ? `<li>${Math.floor(item[1] / 1000000)} COSG (${text[0][1]})</li>` : '';
				let tinymanAlgo = Math.floor(item[2] / 1000000) > 0 ? `<li>${Math.floor(item[2] / 1000000)} ALGO (${text[0][2]})</li>` : '';
				let humble = Math.floor(item[3] / 1000000) > 0 ? `<li>${Math.floor(item[3] / 1000000)} COSG (${text[0][3]})</li>` : '';
				let humbleAlgo = Math.floor(item[4] / 1000000) > 0 ? `<li>${Math.floor(item[4] / 1000000)} ALGO (${text[0][4]})</li>` : '';
				let pact = Math.floor(item[5] / 1000000) > 0 ? `<li>${Math.floor(item[5] / 1000000)} COSG (${text[0][5]})</li>` : '';
				let pactAlgo = Math.floor(item[6] / 1000000) > 0 ? `<li>${Math.floor(item[6] / 1000000)} ALGO (${text[0][6]})</li>` : '';
				let t = Math.floor(item[1] / 1000000) > 0 ? Math.floor(item[1] / 1000000) : 0;
				let tAlgo = Math.floor(item[2] / 1000000) > 0 ? Math.floor(item[2] / 1000000) : 0;
				let h = Math.floor(item[3] / 1000000) > 0 ? Math.floor(item[3] / 1000000) : 0;
				let hAlgo = Math.floor(item[4] / 1000000) > 0 ? Math.floor(item[4] / 1000000) : 0;
				let p = Math.floor(item[5] / 1000000) > 0 ? Math.floor(item[5] / 1000000) : 0;
				let pAlgo = Math.floor(item[6] / 1000000) > 0 ? Math.floor(item[6] / 1000000) : 0;

				let message = `${responses[1]} <ul> ${tinyman} ${tinymanAlgo} ${humble} ${humbleAlgo} ${pact} ${pactAlgo}</ul> <br> In total: ${
					t + h + p
				} COSG and ${tAlgo + hAlgo + pAlgo} ALGO `;

				if (message) response = { message, status: 'valid' };
			}
		});
		// return json(responses);
		return json(response);
	} catch (err) {
		throw err;
	}
}
