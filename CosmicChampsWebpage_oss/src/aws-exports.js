import { COGNITO_USER_POOL_ID, COGNITO_CLIENT_ID } from '$env/static/private';

const REGION = "eu-central-1";
const USER_POOL_ID = COGNITO_USER_POOL_ID;
const APP_CLIENT_ID = COGNITO_CLIENT_ID;

export default {
  Auth: {
    region: 'REGION',
    userPoolId: 'COGNITO_USER_POOL_ID',
    userPoolWebClientId: 'COGNITO_CLIENT_ID',
  },
};