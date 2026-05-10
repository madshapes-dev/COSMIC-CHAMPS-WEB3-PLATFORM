<script>
	import Bug from './-bug.svelte';
	import data from '../../api/bug/sheets/data.json';
	// Used as endpoint to fetch Google Sheets data
	// export let data;
	// console.log(data);

	const grouped = groupBy(data.values, (group) => group[7]);
	function groupBy(list, keyGetter) {
		const map = new Map();
		list.forEach((item) => {
			const key = keyGetter(item);
			const collection = map.get(key);
			if (!collection) {
				map.set(key, [item]);
			} else {
				collection.push(item);
			}
		});
		return map;
	}
</script>

<div class="content">
	<div class="mb-1 text-sm text-cyan-900">Last updated on: 5. Dec. 2022</div>
	<h1 class="title title--gradient">Known bugs</h1>

	{#each [...grouped] as [key, items]}
		<div class="title--gradient mb-2 mt-4 font-bold sm:mt-6">{key}</div>

		{#each items as item, i}
			{#if !item[13]}
				<Bug {item} />
			{/if}
		{/each}
	{/each}
	<a href="/bug/submit" class="button button--primary mt-6 w-full text-center"><span class="button__inner">Submit a bug</span></a>
</div>

<style lang="scss">
	.title.title--gradient {
		@apply mb-4 text-4xl sm:mb-6 sm:text-5xl;
	}
</style>
