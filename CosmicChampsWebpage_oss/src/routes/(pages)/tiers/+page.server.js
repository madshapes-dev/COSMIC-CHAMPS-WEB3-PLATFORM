/** @type {import('./$types').PageServerLoad} */
export async function load({ fetch }) {
	const response = await fetch('../../api/tiers/read-more');
	const read_more = await response.json();
	return {
		read_more: read_more
	};
}
