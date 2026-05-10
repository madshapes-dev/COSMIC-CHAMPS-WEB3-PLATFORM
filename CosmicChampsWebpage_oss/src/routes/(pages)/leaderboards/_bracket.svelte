<script>
	import tournament from '$lib/header/assets/menu/tournament.webp';
	export let items;
	// top-8-1, top-8-2, top-8-3, top-8-4
	// top-4-1,  top-4-2
	// top-2-1
	// ->winner top-1-1
	let top81 = items.filter((item) => item.TournamentId === 'top-8-1');
	let top82 = items.filter((item) => item.TournamentId === 'top-8-2');
	let top83 = items.filter((item) => item.TournamentId === 'top-8-3');
	let top84 = items.filter((item) => item.TournamentId === 'top-8-4');
	let top41 = items.filter((item) => item.TournamentId === 'top-4-1');
	let top42 = items.filter((item) => item.TournamentId === 'top-4-2');
	let top21 = items.filter((item) => item.TournamentId === 'top-2-1');
	let top11 = items.filter((item) => item.TournamentId === 'top-1-1');
</script>

<div class="flex overflow-hidden overflow-x-auto border border-gray-200 bg-white sm:rounded-lg">
	<div class="bracket">
		<section class="round quarterfinals">
			<div class="winners">
				<div class="matchups">
					<div class="matchup">
						<div class="participants">
							{#if top81.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top81 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
					<div class="matchup">
						<div class="participants">
							{#if top82.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top82 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
				</div>
				<div class="connector">
					<div class="merger" />
					<div class="line" />
				</div>
			</div>
			<div class="winners">
				<div class="matchups">
					<div class="matchup">
						<div class="participants">
							{#if top83.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top83 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
					<div class="matchup">
						<div class="participants">
							{#if top84.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top84 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
				</div>
				<div class="connector">
					<div class="merger" />
					<div class="line" />
				</div>
			</div>
		</section>
		<section class="round semifinals">
			<div class="winners">
				<div class="matchups">
					<div class="matchup">
						<div class="participants">
							{#if top41.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top41 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
					<div class="matchup">
						<div class="participants">
							{#if top42.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top42 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
				</div>
				<div class="connector">
					<div class="merger" />
					<div class="line" />
				</div>
			</div>
		</section>
		<section class="round finals">
			<div class="winners">
				<div class="matchups">
					<div class="matchup">
						<div class="participants">
							{#if top21.length === 1}
								<div class="participant winner"><span /></div>
							{/if}
							{#each top21 as item}
								<div class="participant"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
				</div>
				<div class="connector">
					<div class="merger" />
					<div class="line" />
				</div>
			</div>
		</section>
		<section class="round finals">
			<div class="winners">
				<div class="matchups">
					<div class="matchup relative">
						<img class=" absolute -top-16 left-0 right-0 mx-auto w-16" src={tournament} alt="Trophy emoji" />
						<div class="participants">
							{#each top11 as item}
								<div class="participant winner"><span>{item.WalletId?.substring(0, 6)} . . . {item.WalletId?.slice(-6)}</span></div>
							{/each}
						</div>
					</div>
				</div>
			</div>
		</section>
	</div>
</div>

<style lang="scss">
	.bracket {
		@apply mx-auto my-10 inline-block whitespace-nowrap px-6;
		// font-size: 0;
	}
	.bracket .round {
		@apply inline-block align-middle;
	}
	.bracket .round .winners > div {
		display: inline-block;
		vertical-align: middle;
	}
	.bracket .round .winners > div.matchups .matchup:last-child {
		margin-bottom: 0 !important;
	}
	.bracket .round .winners > div.matchups .matchup .participants {
		border-radius: 0.25rem;
		overflow: hidden;
		@apply border border-gray-200;
	}
	.bracket .round .winners > div.matchups .matchup .participants .participant {
		box-sizing: border-box;
		color: #858585;
		border-left: 0.25rem solid #858585;
		background: white;
		width: 12rem;
		height: 3rem;
		box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.12);
		@apply border-gray-400 bg-gray-50 text-gray-400;
	}
	.bracket .round .winners > div.matchups .matchup .participants .participant.winner {
		@apply border-cyan-600 bg-gray-100 text-gray-500;
	}
	.bracket .round .winners > div.matchups .matchup .participants .participant:not(:last-child) {
		@apply border-b border-b-gray-300;
	}
	.bracket .round .winners > div.matchups .matchup .participants .participant span {
		margin: 0 1.25rem;
		line-height: 3;
		font-size: 1rem;
	}
	.bracket .round .winners > div.connector.filled .line,
	.bracket .round .winners > div.connector.filled.bottom .merger:after,
	.bracket .round .winners > div.connector.filled.top .merger:before {
		@apply border-cyan-600;
	}
	.bracket .round .winners > div.connector .line,
	.bracket .round .winners > div.connector .merger {
		box-sizing: border-box;
		width: 2rem;
		display: inline-block;
		vertical-align: top;
	}
	.bracket .round .winners > div.connector .line {
		border-bottom: thin solid #c0c0c8;
		height: 4rem;
	}
	.bracket .round .winners > div.connector .merger {
		position: relative;
		height: 8rem;
	}
	.bracket .round .winners > div.connector .merger:before,
	.bracket .round .winners > div.connector .merger:after {
		content: '';
		display: block;
		box-sizing: border-box;
		width: 100%;
		height: 50%;
		border: 0 solid;
		border-color: #c0c0c8;
	}
	.bracket .round .winners > div.connector .merger:before {
		border-right-width: thin;
		border-top-width: thin;
	}
	.bracket .round .winners > div.connector .merger:after {
		border-right-width: thin;
		border-bottom-width: thin;
	}
	.bracket .round.quarterfinals .winners:not(:last-child) {
		margin-bottom: 2rem;
	}
	.bracket .round.quarterfinals .winners .matchups .matchup:not(:last-child) {
		margin-bottom: 2rem;
	}
	.bracket .round.semifinals .winners .matchups .matchup:not(:last-child) {
		margin-bottom: 10rem;
	}
	.bracket .round.semifinals .winners .connector .merger {
		height: 16rem;
	}
	.bracket .round.semifinals .winners .connector .line {
		height: 8rem;
	}
	.bracket .round.finals .winners .connector .merger {
		height: 3rem;
	}
	.bracket .round.finals .winners .connector .line {
		height: 1.5rem;
	}
</style>
