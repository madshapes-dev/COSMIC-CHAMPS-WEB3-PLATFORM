// @ts-nocheck
import { getSheetsService } from '../../service.js';
import { VITE_GOOGLE_DRIVE_SHEET_TIERS_ID } from '$env/static/private';
import { json } from '@sveltejs/kit';

/** @type {import('./$types').RequestHandler} */
export async function GET() {
	// googleapis
	const sheets = getSheetsService();
	const spreadsheetId = VITE_GOOGLE_DRIVE_SHEET_TIERS_ID;

	const sheetsRequest = {
		spreadsheetId,
		range: 'A1'
	};

	try {
		const result = (await sheets.spreadsheets.values.get(sheetsRequest)).data.values[0][0];
		return json(result);
	} catch (err) {
		throw err;
	}
}
