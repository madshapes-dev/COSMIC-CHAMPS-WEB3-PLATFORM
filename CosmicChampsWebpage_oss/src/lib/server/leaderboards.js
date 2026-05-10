export async function getLeaderboards() {
	const url = `http://APISERVICE:3000/getMatchdata/all`;
	const res = await fetch(url);
	const data = await res.json();

	return data;
}
export async function getLeaderboardsCurrent() {
	let date = new Date();
	let day = date.getDay();
	let diff = date.getDate() - day + (day == 0 ? -6 : 1);
	let monday = new Date(date.setDate(diff));
	let year = monday.getFullYear();
	let month = ('0' + (monday.getMonth() + 1)).slice(-2);
	day = ('0' + monday.getDate()).slice(-2);
	date = year + '-' + month + '-' + day;

	const urlCurrent = `http://APISERVICE:3000/getMatchdata/${date}`;
	const resCurrent = await fetch(urlCurrent);
	const dataCurrent = await resCurrent.json();

	return dataCurrent, date;
}
