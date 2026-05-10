import { json } from '@sveltejs/kit';

export async function GET() {
	const today = new Date();
	const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
	const year = thirtyDaysAgo.getFullYear();
	const month = ('0' + (thirtyDaysAgo.getMonth() + 1)).slice(-2);
	const day = ('0' + thirtyDaysAgo.getDate()).slice(-2);
	const date = year + '-' + month + '-' + day;

	const urlCurrent = `http://APISERVICE/getMatchdata/${date}`;
	const resCurrent = await fetch(urlCurrent);
	const dataCurrent = await resCurrent.json();

	// return dataCurrent, date;
	return json(dataCurrent, date);
}
