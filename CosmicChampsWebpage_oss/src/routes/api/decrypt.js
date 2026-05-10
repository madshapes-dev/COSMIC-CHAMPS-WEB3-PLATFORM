import crypto from 'crypto';
import { VITE_SERVICE_ENCRYPTION_KEY, VITE_SERVICE_ENCRYPTION_IV } from '$env/static/private';

export const decrypt = (data) => {
	const algorithm = 'aes-128-cbc';
	const decipher = crypto.createDecipheriv(algorithm, VITE_SERVICE_ENCRYPTION_KEY, VITE_SERVICE_ENCRYPTION_IV);
	let decrypted = decipher.update(data, 'base64', 'utf8');
	decrypted += decipher.final('utf8');
	return JSON.parse(decrypted);
};
