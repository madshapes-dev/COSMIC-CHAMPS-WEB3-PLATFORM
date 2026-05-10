import { google } from 'googleapis';
import { encrypted } from './service-account.enc';
import { decrypt } from './decrypt';

const getDriveService = () => {
	const SCOPES = ['https://www.googleapis.com/auth/drive'];
	const credentials = decrypt(encrypted);

	const auth = new google.auth.GoogleAuth({
		credentials,
		scopes: SCOPES
	});
	const driveService = google.sheets({ version: 'v4', auth });
	return driveService;
};

export default getDriveService;
