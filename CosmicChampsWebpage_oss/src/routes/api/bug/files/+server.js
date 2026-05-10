// Read base64 files, write them locally and upload them to Google Drive folder
import { Readable } from 'stream';
import service from '../../service.js';
import fs from 'fs';

export async function POST({ request }) {
	const data = await request.json();

	// googleapis
	const drive = service();
	const folderId = data['folderId'] ? data['folderId'] : import.meta.env.VITE_GOOGLE_DRIVE_FOLDER_ID;
	let fileMetadata = {
		name: `${data['id']}.${data['extension']}`,
		parents: [folderId]
	};
	let media = {
		mimeType: `${data['type']}`,
		body: Readable.from(Buffer.from(data['file'], 'base64')) // Convert base64 to buffer
	};

	try {
		const file = await drive.files.create({
			requestBody: fileMetadata,
			media: media,
			fields: 'id'
		});

		return new Response(String(file.data.id));
	} catch (err) {
		throw err;
	}
}
