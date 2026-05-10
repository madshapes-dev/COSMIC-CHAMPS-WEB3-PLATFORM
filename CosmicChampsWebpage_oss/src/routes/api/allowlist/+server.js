// @ts-nocheck
import service from '../service.js';
import { json } from '@sveltejs/kit';

export async function POST({ request }) {
	let { address } = await request.json();
	let response = { message: 'Sorry, your wallet is not on allow list.', status: 'invalid' };

	const nfd = await fetch(`https://api.nf.domains/nfd/${address}`);
	const nfdData = await nfd.json();
	if (nfdData.depositAccount) address = nfdData.depositAccount;

	// googleapis
	const sheets = service();
	const spreadsheetId = import.meta.env.VITE_GOOGLE_DRIVE_SHEET_WHITELIST_ID;
	const sheetsRequest = {
		spreadsheetId,
		range: 'A2:C'
	};
	try {
		const result = (await sheets.spreadsheets.values.get(sheetsRequest)).data;
		result.values.forEach((item, i) => {
			let addr = item[0] ? item[0].toString().toUpperCase() : '';
			if (address.toString().trim().toUpperCase() == addr) {
				let purchases = item[2] ? item[2] + ' purchases' : 1 + ' purchase';
				response = {
					message: `You are entitled to ${item[1]} private shuffle spots in total.<br> You are guaranteed at least ${purchases} in the  first 24 hours.`,
					status: 'valid'
				};
			}
		});
		// return json(result);
	} catch (err) {
		throw err;
	}

	return json(response);
}
