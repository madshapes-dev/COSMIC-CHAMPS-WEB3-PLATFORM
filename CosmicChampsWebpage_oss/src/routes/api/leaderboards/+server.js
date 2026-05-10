import { json } from '@sveltejs/kit';

export async function GET() {
	const url = `http://APISERVICE:3000/getMatchdata/all`;
	const res = await fetch(url);
	const data = await res.json();

	return json(data);
}
