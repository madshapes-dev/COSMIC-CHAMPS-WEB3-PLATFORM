// Create Google Drive folder and return folder id

import service from '../../service.js';
import { json } from '@sveltejs/kit';
import { VITE_GOOGLE_DRIVE_FOLDER_ID } from '$env/static/private';

/** @type {import('./$types').RequestHandler} */
export async function POST({ request }) {
	const data = await request.json();
	const fileId = data['id'];

	// googleapis
	const drive = service();
	const folderId = VITE_GOOGLE_DRIVE_FOLDER_ID;
	const folderMetadata = {
		name: `ID_${fileId}`,
		parents: [folderId],
		mimeType: 'application/vnd.google-apps.folder'
	};
	try {
		const folder = await drive.files.create({
			requestBody: folderMetadata,
			fields: 'id'
		});
		return json({ id: folder.data.id });
	} catch (err) {
		throw err;
	}
}
