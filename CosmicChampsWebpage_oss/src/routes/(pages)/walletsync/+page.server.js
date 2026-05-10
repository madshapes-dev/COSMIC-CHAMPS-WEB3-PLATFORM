import { authenticateUser } from '$lib/server/cognito';
import { updateItemInDynamoDB } from '$lib/server/dynamodb';
import { get } from 'svelte/store';
import { game } from '$lib/stores/game.js';

const gameData = get(game);
function getValueFromName(userData, name) {
	for (const item of userData) {
		if (item.Name === name) {
			return item.Value;
		}
	}
	return null;
}

/** @type {import('./$types').Actions} */
export const actions = {
	default: async ({ cookies, request }) => {
		const data = await request.formData();
		const wallet = data.get('wallet');
		const email = data.get('email');
		const password = data.get('password');
		const userData = await authenticateUser(email, password);

		if (userData) {
			const id = getValueFromName(userData, 'sub');
			// Insert data into DynamoDB
			const inserted = await updateItemInDynamoDB(`${gameData.test}-Player`, id, wallet);
			if (inserted) {
				return { success: true, userData };
			} else {
				return { success: false };
			}
		} else {
			return { success: false };
		}
	}
};
