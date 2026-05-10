import service from './service.js';
import { json } from '@sveltejs/kit';
// import { VITE_GOOGLE_DRIVE_SHEET_TOURNAMENT_FORM } from '$env/static/private';

export async function tournamentSheets() {
	// googleapis
	const sheets = service();
	const spreadsheetId = import.meta.env.VITE_GOOGLE_DRIVE_SHEET_TOURNAMENT_FORM;

	const request = {
		spreadsheetId,
		range: 'B2:B'
	};

	const data = (await sheets.spreadsheets.values.get(request)).data;
	const result = data.values;

	return result;
}
