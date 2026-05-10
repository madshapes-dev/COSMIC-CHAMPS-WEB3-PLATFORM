// Read base64 files, write them locally and upload them to Google Drive folder
import service from '../../service.js';
import { json } from '@sveltejs/kit';
import { VITE_GOOGLE_DRIVE_SHEET_TOURNAMENT_FORM } from '$env/static/private';

export async function GET({ request }) {
	// googleapis
	const sheets = service();
	const spreadsheetId = VITE_GOOGLE_DRIVE_SHEET_TOURNAMENT_FORM;

	const settings = {
		spreadsheetId,
		range: 'B2:B'
	};

	try {
		const result = (await sheets.spreadsheets.values.get(settings)).data;
		return json(result.values);
	} catch (err) {
		throw err;
	}
}
