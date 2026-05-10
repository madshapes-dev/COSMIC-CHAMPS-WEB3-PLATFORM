<script>
	import { browser } from '$app/environment';
	import { Icon } from '@steeze-ui/svelte-icon';
	import { EllipsisHorizontalCircle, DocumentDuplicate, XMark } from '@steeze-ui/heroicons';
	import { millisecondsToReadableTime, formatDate, copyToClipboard, deepMerge, isTournamentLive, hasTop8Started, roundNumber } from './helpers';
	import { wallet } from '$lib/stores/wallet.js';
	import { tournament } from '$lib/stores/tournament.js';
	import TH from './_table_th.svelte';
	import Bracket from './_bracket.svelte';
	export let data;
	// console.log(data, $tournament);

	$: ({ tournaments } = data);

	const startDate = new Date($tournament.start);
	const endDate = new Date($tournament.end);
	// set Date formats for tabs
	let currentDate = formatDate(data.date);
	// set data view table/bracket
	let presentation = 'table';
	let tournamentTable = false;
	let showPreviosTournament = false;

	// set current data
	let selectedData;
	function setCurrentData() {
		if ($tournament.started && $tournament.live) selectedData = 'tournamentNew';
		else selectedData = 'dataCurrent';

		if (browser && selectedData === 'dataCurrent') getDataCurrent();
	}
	setCurrentData();

	// get all data for trournament
	let dataAll, tempAll;
	async function getData() {
		const response = await fetch('/api/leaderboards', {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json'
			}
		});
		const data = await response.json();

		tempAll = data.map((item) => {
			return {
				...item,
				WalletId: item.id,
				duration: millisecondsToReadableTime(item.totalduration),
				wins: Math.round((item.gameswon / item.gamesplayed) * 100)
			};
		});
		dataAll = tempAll.sort((firstItem, secondItem) => (firstItem.gamesplayed > secondItem.gamesplayed ? -1 : 1)).slice(0, 15);
	}
	// get data for latest top 15
	let dataCurrent, tempCurrent;
	async function getDataCurrent() {
		const response = await fetch('/api/leaderboards/current', {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json'
			}
		});
		const data = await response.json();

		tempCurrent = data.map((item) => {
			return {
				...item,
				WalletId: item.id,
				duration: millisecondsToReadableTime(item.totalduration),
				wins: Math.round((item.gameswon / item.gamesplayed) * 100)
			};
		});
		dataCurrent = tempCurrent.sort((firstItem, secondItem) => (firstItem.gamesplayed > secondItem.gamesplayed ? -1 : 1)).slice(0, 15);
	}

	let dataTournament;
	function getTournamentData() {
		let dataTournament = [];
		let uniqueWalletId = {};
		let tournament = JSON.parse(data.tournament)
			.sort((firstItem, secondItem) => (firstItem.Rating > secondItem.Rating ? -1 : 1))
			.sort((firstItem, secondItem) => {
				const dateA = new Date(firstItem.LastPlayedGameDate);
				const dateB = new Date(secondItem.LastPlayedGameDate);
				return dateB - dateA;
			})
			.filter((item) => {
				const date = new Date(item.LastPlayedGameDate);
				if (date > startDate && date <= endDate) return item;
			})
			.forEach((item, i) => {
				if (!uniqueWalletId[item.WalletId]) {
					uniqueWalletId[item.WalletId] = true;
					dataTournament.push(item);
				}
			});
		dataTournament.sort((firstItem, secondItem) => (firstItem?.Rating > secondItem?.Rating ? -1 : 1));

		if (dataTournament.length > 1) {
			const correction = 1200;
			const sumCoins = 2000;
			const sumCoinsTop8 = 3000;
			const prizesTop = [800, 500, 400, 300, 250, 200, 175, 150, 125, 100];
			let sumRating = 0;
			let sumRatingTop8 = 0;
			for (let i = 10; i < dataTournament.length; i++) {
				if (i < 41) sumRating = sumRating + (dataTournament[i].Rating - correction);
			}
			let x = sumCoins / sumRating;

			dataTournament = dataTournament.map((item, i) => {
				let Final = false;
				let Prize = '0';

				if (i < 10) {
					Final = true;
					let prize = prizesTop[i];
					Prize = `${prize}`;
				} else if (item.Rating > correction) {
					let num = x * (item.Rating - correction);
					let prize = Math.round(num) > 100 || Math.sign(num) === -1 ? '100' : Math.round(num);
					Prize = `${prize}`;
				}

				return {
					...item,
					Ranking: i + 1,
					Final,
					Prize: Prize
				};
			});
		}

		return dataTournament;
	}
	if ($tournament.started) dataTournament = getTournamentData();

	// filter data based on address and tab selection
	let items = [];
	let tournamentData = [];
	$: addressSelected = $wallet.address ? $wallet.address : '';
	$: address = addressSelected ? addressSelected : '';
	$: {
		function filterByAddress(data, address) {
			if (address) {
				let item = data.filter((el) => el.WalletId.toLowerCase().includes(address?.toLowerCase()));
				let itemsAll = data.filter((el) => el.WalletId.toLowerCase() != address?.toLowerCase());
				const newitem = { ...item[0], selected: true };
				if (item[0]) return [newitem, ...itemsAll];
				else return data;
			} else {
				return data;
			}
		}
		if (selectedData === 'tournamentNew') {
			// console.log(dataTournament);
			// const print = dataTournament.map((item) => ({
			// 	Rating: item.Rating,
			// 	WalletId: item.WalletId,
			// 	Prize: parseInt(item.Prize)
			// }));
			// console.log(print);
			items = filterByAddress(dataTournament, address);
			tournamentTable = true;
		} else if (selectedData === 'dataAll') {
			if (dataAll) items = filterByAddress(dataAll, address);
			tournamentTable = false;
		} else if (selectedData === 'dataCurrent') {
			if (dataCurrent) items = filterByAddress(dataCurrent, address);
			tournamentTable = false;
		} else {
			const dataTournament = tournaments.find((el) => el.id == selectedData);
			if (dataTournament?.presentation === 'bracket') presentation = 'bracket';
			else presentation = 'table';

			if (dataTournament) items = filterByAddress(dataTournament.data, address);
			else items = [];
			tournamentTable = true;
			selectedData = dataTournament?.id;
		}
	}

	// sort data based on column
	let ascending = false;
	let sortBy = { col: '', ascending: ascending };
	$: sort = (column) => {
		if (sortBy.col == column) {
			sortBy.ascending = !sortBy.ascending;
		} else {
			sortBy.col = column;
			sortBy.ascending = !sortBy.ascending;
		}
		// Modifier to sorting function for ascending or descending
		let sortModifier = sortBy.ascending ? 1 : -1;
		let sort = (a, b) => (a[column] < b[column] ? -1 * sortModifier : a[column] > b[column] ? 1 * sortModifier : 0);

		items = items.sort(sort);
	};

	// toggle address in table
	let hiddenFullAddress = true;
	function onToggleAddress(event) {
		const targetElement = event.target;
		targetElement.classList.toggle('text-cyan-500');
	}
</script>

<div class="content">
	<div class="mb-3 flex flex-wrap md:mb-6">
		<h1 class="title title--gradient mr-2">Leaderboards</h1>
		<span class="title title--size-4 title--golden"
			>{#if selectedData === 'tournamentNew'}Tournament{:else if showPreviosTournament}Tournaments{:else}Top 15{/if}</span
		>
		<div class="w-full text-xs ld:text-base">40k+ algo in tournament prizes distributed to date*</div>
	</div>

	<div class="mb-1 text-sm font-bold uppercase text-gray-700">My ranking</div>
	<div class="mb-5 md:mb-8 flex items-end">
		<input bind:value={address} type="text" placeholder="Your wallet address..." required />
		<button class="button button--primary ml-2" on:click={() => (address = '')}
			><span class="button__inner"><Icon src={XMark} theme="solid" class="h-5 w-5 lg:h-6 lg:w-6" /></span></button
		>
	</div>
	<div class="overflow-hidden overflow-x-auto">
		{#if showPreviosTournament}
			<ul class="tabs mt-1 flex min-w-min justify-end pt-1 text-center text-sm font-bold uppercase text-gray-400 sm:mt-4 xl:mt-0">
				<li class="mr-2 w-[150px] sm:w-auto text-center">
					<button
						class="label uppercase bg-white items-center justify-center"
						on:click={() => {
							showPreviosTournament = false;
							presentation = 'table';
							setCurrentData();
						}}
					>
						Leaderboards
						<div class="text-xs lowercase">show all</div>
					</button>
				</li>
				{#each data.tournaments as item}
					<li class="relative mr-2 w-[150px] sm:w-auto">
						<label class="" class:active={selectedData == item.id}
							><input type="radio" value={item.id} bind:group={selectedData} class="flex h-0 w-0 opacity-0" checked />
							<div class="flex items-end justify-center">
								<span>{item.title}</span>
							</div>
							<div class="text-xs capitalize">{item.description}</div></label
						>
					</li>
				{/each}
			</ul>
		{:else}
			<ul class="tabs mt-1 flex min-w-min justify-end pt-1 text-center text-sm font-bold uppercase text-gray-400 sm:mt-4 xl:mt-0">
				{#if $tournament.started && $tournament.live}
					<li class="relative mr-2 w-[150px] sm:w-auto">
						<label class="" class:active={selectedData == 'tournamentNew'}
							><input type="radio" value="tournamentNew" bind:group={selectedData} class="flex h-0 w-0 opacity-0" checked />
							<div class="flex items-end justify-center">
								<span>Tournament</span>
								{#if $tournament.live}<div class="absolute -top-1 -right-1 overflow-hidden rounded-sm border bg-gray-100">
										<span class="flex h-full animate-pulse px-1 text-xs tracking-wide text-cyan-700">live</span>
									</div>{/if}
							</div>
							<div class="text-xs capitalize">{$tournament.date}</div></label
						>
					</li>
				{/if}
				<li class="mr-2 w-[150px] sm:w-auto">
					<label class="" class:active={selectedData === 'dataCurrent'}
						><input type="radio" value="dataCurrent" bind:group={selectedData} on:click={getDataCurrent} class="h-0 w-0 opacity-0" checked />Monthly
						<br />
						<div class="text-xs lowercase">since <span class="capitalize">{currentDate}</span></div></label
					>
				</li>
				<li class="mr-2 w-[150px] sm:w-auto">
					<label class="" class:active={selectedData == 'dataAll'}
						><input type="radio" id="dataAll" value="dataAll" bind:group={selectedData} on:click={getData} class="h-0 w-0 opacity-0" />All-time
						<br />
						<div class="text-xs lowercase">since <span class="capitalize">All-time</span></div></label
					>
				</li>
				<li class="mr-2 w-[150px] sm:w-auto">
					<button
						class="label uppercase items-center justify-center"
						on:click={() => {
							showPreviosTournament = true;
							selectedData = `${tournaments[0].id}`;
						}}
					>
						Tournaments
						<div class="text-xs lowercase">past tournaments</div>
					</button>
				</li>
			</ul>
		{/if}
	</div>
	{#if presentation === 'bracket'}
		<Bracket {items} />
	{:else}
		<div class="overflow-hidden overflow-x-auto border border-gray-200 sm:rounded-lg">
			{#if tournamentTable || selectedData === 'tournamentNew'}
				<table class="w-full animate-pulse text-left text-xs text-gray-500 sm:text-sm">
					<thead class="border-b border-b-gray-200 bg-gray-100 text-xs uppercase text-gray-700">
						<tr>
							<th scope="col" class="py-3 px-1 md:px-6"><TH title="Ranking" icon={0} /> </th>
							<th scope="col" class="py-3 px-3 md:px-6"><TH title="Rating" icon={0} /> </th>
							<th scope="col" class="py-3 px-3 md:px-6"><TH title="Wallet" icon={0} /> </th>
							<th scope="col" class="py-3 px-3 md:px-6"><TH title="Prize" icon={0} /> </th>
						</tr>
					</thead>
					<tbody>
						{#each items as row, i}
							<tr
								class="border-b {row.selected ? ' border-cyan-800 bg-gray-100 bg-gradient-to-t from-slate-300 shadow-lg' : 'bg-white'}"
								class:selected={row.selected}
								class:final={row.Final}
								class:top50={row.Top50}
							>
								<td class="py-2 px-3 md:py-4 md:px-6">{row.Ranking}.</td>
								<td class="py-2 px-3 md:py-4 md:px-6">{row.Rating}</td>
								<td class="whitespace-nowrap py-2 px-3 md:py-4 md:px-6">
									<div class="flex">
										{#if row.WalletId != 'PZSE6JLUWFWDIK4EQCHVQUZU4DWSMTRVYFSOTOD4MDF72J4ELRU6D2JPY4' && row.WalletId != 'CCDEV23KIZIJKRSUMYXSIC3KTLCHXYJAOSRJTDCNYJCE7SQFU4X3T5XX7I'}
											{row.WalletId?.substring(0, 6)} . . . {row.WalletId?.slice(-6)}
											<input id="copyAddress-{i}-id" value={row.WalletId} class="hidden" />
											<a href="/" class="text-gray-400" on:click|preventDefault={() => copyToClipboard(`copyAddress-${i}-id`)}>
												<Icon src={DocumentDuplicate} theme="solid" class="pointer-events-none ml-1.5 h-4 w-4 md:ml-4" />
											</a>
										{:else if row.WalletId === 'PZSE6JLUWFWDIK4EQCHVQUZU4DWSMTRVYFSOTOD4MDF72J4ELRU6D2JPY4'}
											{row.WalletId?.substring(0, 6)} ... MATTY <input id="copyAddress-{i}-id" value={row.WalletId} class="hidden" />
											<a href="/" class="text-gray-400" on:click|preventDefault={() => copyToClipboard(`copyAddress-${i}-id`)}>
												<Icon src={DocumentDuplicate} theme="solid" class="pointer-events-none ml-1.5 h-4 w-4 md:ml-4" />
											</a>
										{:else if row.WalletId === 'CCDEV23KIZIJKRSUMYXSIC3KTLCHXYJAOSRJTDCNYJCE7SQFU4X3T5XX7I'}
											{row.WalletId?.substring(0, 6)} ... SIMON
										{/if}
									</div>
								</td>
								<td class="whitespace-nowrap py-2 px-3 md:py-4 md:px-6">
									{row.Prize}
									<sup class="text-cyan-800">*</sup>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
				<p class="p-3 text-xs md:text-sm">
					<sup class="text-cyan-800">*</sup> Prizes are paid half-and-half in COSG and ALGO.<br /> If you aren't opted in to the COSG at the time of distribution,
					you forfeit all rewards.
				</p>
			{:else}
				<table class="w-full text-left text-xs text-gray-500 sm:text-sm" class:loading={!dataAll && !dataCurrent}>
					<thead class="border-b border-b-gray-200 bg-gray-100 text-xs uppercase text-gray-700">
						<tr>
							<th on:click={sort('gamesplayed')} scope="col" class="py-3 px-3 md:px-6"><TH title="Games played" /> </th>
							<th on:click={sort('gameswon')} scope="col" class="py-3 px-3 md:px-6"><TH title="Games won" /> </th>
							<th on:click={sort('wins')} scope="col" class="py-3 px-3 md:px-6"><TH title="Win %" /></th>
							<th on:click={sort('drawn')} scope="col" class="py-3 px-3 md:px-6"><TH title="Drawn" /> </th>
							<th on:click={sort('totalduration')} scope="col" class="py-3 px-3 md:px-6"><TH title="Total duration" /> </th>
							<th on:click={sort('Id')} scope="col" class="py-3 px-3 text-right md:px-6"><TH title="Wallet" icon={0} /> </th>
						</tr>
					</thead>
					<tbody>
						{#if items.length > 1}
							{#each items as row, i}
								<tr
									class="border-b {row.selected ? ' border-cyan-800 bg-gray-100 bg-gradient-to-t from-slate-300 shadow-lg' : 'bg-white'}"
									class:selected={row.selected}
								>
									<td class="py-2 px-3 md:py-4 md:px-6">{row.gamesplayed}</td>
									<td class="py-2 px-3 md:py-4 md:px-6">{row.gameswon}</td>
									<td class="py-2 px-3 md:py-4 md:px-6">{row.wins} %</td>
									<td class="py-2 px-3 md:py-4 md:px-6">{row.drawn}</td>
									<td class="py-2 px-3 md:py-4 md:px-6">{row.duration}</td>
									<td class="relative z-10 flex justify-end whitespace-nowrap py-2 px-3 md:py-4 md:px-6 uppercase">
										{row.id?.substring(0, 6)} . . . {row.id?.slice(-6)}
										<input id="copyAddress-{i}" value={row.id} class="hidden" />
										<a href="/" class="text-gray-400" on:click|preventDefault={() => copyToClipboard(`copyAddress-${i}`)}>
											<Icon src={DocumentDuplicate} theme="solid" class="pointer-events-none ml-4 h-4 w-4" />
										</a>
									</td>
								</tr>
							{/each}
						{:else if !dataAll && !dataCurrent}
							{#each { length: 15 } as row}
								<tr class="h-[3.25rem] border-b bg-white">
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
									<td><span class="my-auto mx-3 flex h-4 w-8/12 bg-gray-300 md:mx-6" /></td>
								</tr>
							{/each}
						{/if}
					</tbody>
				</table>
			{/if}
		</div>
	{/if}
</div>

<style lang="scss">
	.title.title--gradient {
		@apply mb-0 text-3xl sm:text-5xl;
	}
	.title.title--golden {
		@apply mb-0 text-xl md:text-2xl;
	}
	input[type='text'] {
		@apply w-full rounded border-2 border-cyan-500  p-2 text-left text-sm text-cyan-900 placeholder:text-cyan-900 lg:w-[610px] lg:text-base;
	}
	.tabs label,
	.tabs .label {
		@apply flex h-full w-full cursor-pointer flex-col rounded-t-lg border border-b-0  p-1.5 text-center text-xs transition duration-200 ease-in-out hover:bg-gray-100 sm:w-auto sm:p-4 md:p-3 md:text-sm lg:px-16;
		&.active {
			@apply bg-gray-100 text-gray-700 shadow-sm;
		}
	}
	input {
		@apply mb-0 min-h-0 p-0;
	}
	:global(table.loading) {
		@apply bg-gray-100 blur-sm;
		:global(tr) {
			@apply animate-pulse;
		}
	}
	:global(th.text-right > *) {
		@apply justify-center;
	}

	:global(tr.final) {
		@apply bg-gray-100;
	}
</style>
