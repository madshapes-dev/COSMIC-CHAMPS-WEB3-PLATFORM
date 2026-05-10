import { DynamoDBClient, ScanCommand, PutItemCommand, UpdateItemCommand } from '@aws-sdk/client-dynamodb';
import { unmarshall } from '@aws-sdk/util-dynamodb';
import { VITE_AWS_ACCESS, VITE_AWS_SECRET } from '$env/static/private';
import { get } from 'svelte/store';
import { game } from '$lib/stores/game.js';

const gameData = get(game);

const accessKeyId = VITE_AWS_ACCESS;
const secretAccessKey = VITE_AWS_SECRET;

const dynamoDBClient = new DynamoDBClient({
	region: 'eu-central-1',
	credentials: {
		accessKeyId,
		secretAccessKey
	}
});


//updated functions to bind to a different field (used to bind to WalletId now we bind to LinkedWalletId)

// Function to fetch data from DynamoDB
// FilterExpression: 'attribute_exists(WalletId) AND attribute_exists(LastPlayedGameDate)'
export async function fetchDataFromDynamoDB() {
	const top8Date = new Date(Date.UTC(2023, 6, 9, 0, 30, 0));
	const currentDate = new Date();
	let tableName = `${gameData.test}-Player`;
	// if (top8Date < currentDate) tableName = 'TournamentResult';

	const params = {
		TableName: tableName,
		FilterExpression: 'attribute_exists(WalletId) AND attribute_exists(LastPlayedGameDate)'
	};

	try {
		const response = await dynamoDBClient.send(new ScanCommand(params));
		const unmarshalledItems = response.Items.map((item) => unmarshall(item));
		return unmarshalledItems;
	} catch (error) {
		console.error('Error fetching data from DynamoDB:', error);
		throw error;
	}
}


//used when linking linkedwalletid (custom wallet) to a player entry in db
//each player gets autoassigned walletid now as part of guest account or email account creation process
export async function updateItemInDynamoDB(tableName, id, wallet) {
	//replacing WalletId with LinkedWalletId in set sentence
	const params = {
		TableName: tableName,
		Key: {
			Id: { S: id } // Assuming Id is a string and is the partition key of the table
		},
		UpdateExpression: 'SET LinkedWalletId = :walletVal',
		ConditionExpression: 'attribute_exists(Id)',
		ExpressionAttributeValues: {
			':walletVal': { S: wallet } // Assuming LinkedWalletId is a string
		}
	};

	try {
		const updateCommand = new UpdateItemCommand(params);
		const response = await dynamoDBClient.send(updateCommand);
		return response;
	} catch (err) {
		console.error('Error:', err);
		throw err;
	}
}


//unsure if thsi is used anywhere leaving here for now
export async function insertDataIntoDynamoDB(tableName, id, nickname, wallet) {
	const params = {
		TableName: tableName,
		Item: {
			Id: { S: id }, // Assuming Id is a string
			Nickname: { S: nickname }, // Assuming Name is a string
			WalletId: { S: wallet } // Assuming Name is a string
		}
	};

	try {
		const putCommand = new PutItemCommand(params);
		const response = await dynamoDBClient.send(putCommand);
	} catch (err) {
		console.error('Error inserting data into DynamoDB:', err);
		throw err;
	}
}
