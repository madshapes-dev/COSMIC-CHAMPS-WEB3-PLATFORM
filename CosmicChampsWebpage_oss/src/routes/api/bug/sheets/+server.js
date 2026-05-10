// Read base64 files, write them locally and upload them to Google Drive folder
import service from '../../service.js';
import { json } from '@sveltejs/kit';

export async function GET() {
	// googleapis
	const sheets = service();
	const spreadsheetId = import.meta.env.VITE_GOOGLE_DRIVE_SHEET_ID;

	const request = {
		spreadsheetId,
		range: 'C2:R'
	};

	try {
		const result = (await sheets.spreadsheets.values.get(request)).data;
		// console.log(result);
		return json(result);
	} catch (err) {
		throw err;
	}
}
