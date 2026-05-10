import { CognitoIdentityProviderClient, InitiateAuthCommand, AdminGetUserCommand } from '@aws-sdk/client-cognito-identity-provider';
import { fromCognitoIdentityPool } from '@aws-sdk/credential-provider-cognito-identity';
import { COGNITO_USER_POOL_ID, COGNITO_CLIENT_ID, VITE_AWS_ACCESS, VITE_AWS_SECRET } from '$env/static/private';

// Configure AWS SDK
const REGION = 'eu-central-1';
const USER_POOL_ID = COGNITO_USER_POOL_ID;
const USER_POOL_CLIENT_ID = COGNITO_CLIENT_ID;
const accessKeyId = VITE_AWS_ACCESS;
const secretAccessKey = VITE_AWS_SECRET;

const cognitoClient = new CognitoIdentityProviderClient({
	region: REGION,
	credentials: {
		accessKeyId,
		secretAccessKey
	}
});

export async function authenticateUser(email, password) {
	const params = {
		AuthFlow: 'USER_PASSWORD_AUTH',
		ClientId: USER_POOL_CLIENT_ID,
		UserPoolId: USER_POOL_ID,
		AuthParameters: {
			USERNAME: email,
			PASSWORD: password
		}
	};

	try {
		const command = new InitiateAuthCommand(params);
		const response = await cognitoClient.send(command);
		// console.log('cognito response', response);
		// Check if the authentication is successful
		if (response.AuthenticationResult && response.AuthenticationResult.AccessToken) {
			const accessToken = response.AuthenticationResult.AccessToken;
			// Retrieve user data after successful authentication
			const userDataParams = {
				UserPoolId: USER_POOL_ID,
				Username: email
			};

			const getUserCommand = new AdminGetUserCommand(userDataParams);
			const userResponse = await cognitoClient.send(getUserCommand);
			const userData = userResponse.UserAttributes;
			return userData;
		} else {
			console.log('Authentication failed.');
			return null;
		}
	} catch (error) {
		console.error('Login failed:', error);
	}
}
