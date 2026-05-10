<script>
	import { invalidateAll } from '$app/navigation';
	import { applyAction } from '$app/forms';

	export let form;
	let success = false;
	let fileInput;
	let files;
	let id;
	let filesSizeLimit = false;
	const todayDate = new Date();
	const timestamp = Date.parse(todayDate);

	// set form ID from current date timestamp
	function padTo2Digits(num) {
		return num.toString().padStart(2, '0');
	}
	function formatDate(date) {
		return [padTo2Digits(date.getDate()), padTo2Digits(date.getMonth() + 1), date.getFullYear()].join('/');
	}

	function fileSizeLimit(number) {
		// 10 MB = 10485760 bytes
		// 15 MB = 15728640 bytes
		// 20 MB
		if (number <= 20971520) return false;
		else return true;
	}

	async function getBase64(files, id) {
		let fileSize = 0;
		for (let file of files) {
			fileSize += file.size;
		}
		let i = 0;
		filesSizeLimit = fileSizeLimit(fileSize);
		if (!filesSizeLimit) {
			const folderId = await createGoogleDriveFolder(id.value);
			for (let file of files) {
				const reader = new FileReader();
				reader.readAsDataURL(file);
				reader.onload = (e) => {
					uploadFiles(e.target.result, file.type, `${id.value}_${i++}`, folderId);
				};
			}
		} else {
			fileInput.value = '';
		}
	}
	async function createGoogleDriveFolder(id) {
		const data = {};
		data['id'] = id;
		const response = await fetch(`/api/bug/folder`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(data)
		});
		const folder = await response.json();
		return folder.id;
	}
	async function uploadFiles(base64, type, id, folderId) {
		const data = {};
		const file = base64.split(',');
		data['file'] = file[1];
		data['id'] = id;
		data['type'] = type;
		data['folderId'] = folderId;
		data['extension'] = type.split('/')[1];
		await fetch(`/api/bug/files`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				Accept: 'application/json'
			},
			body: JSON.stringify(data)
		});
	}

	let error;
	async function handleSubmit(event) {
		const data = new FormData(this);
		// if files submitted, upload them
		const filesInput = data.get('files');
		if (filesInput.size) getBase64(files, id);
		// trigger +page.server.js default action
		const response = await fetch(this.action, {
			method: 'POST',
			body: data
		});
		const result = await response.json();

		if (result.type === 'success') {
			// re-run all `load` functions, following the successful update
			success = true;
			await invalidateAll();
		}
	}
</script>

<h1 class="title mt-2">Submit a bug</h1>

{#if form?.success || success}
	<p class="font-medium text-cyan-900">
		The form was successfuly submitted 👍. <br /> Thank you for helping us make our game better.
	</p>
{:else}
	<p class="mb-12 -mt-6 text-sm font-medium text-cyan-900 sm:text-base">
		Heya Champ 👋 thanks for taking your time to help improve our game! Please try to provide as much information as possible.<br /> Please fill out all
		the fields.
	</p>

	<div class="content">
		<div class="-mt-4">
			<form method="POST" on:submit|preventDefault={handleSubmit} class="grid grid-cols-1 gap-6">
				<fieldset>
					<legend class="title--gradient">Bug info</legend>
					<div>
						<input class="timestamp" type="number" name="id" bind:this={id} value={timestamp} />
					</div>
					<div class="bug-impact block">
						<div class="label-title">
							How much did the bug impact your gameplay?<sup>*</sup>
						</div>
						<div class="mt-4 flex flex-wrap items-center justify-between">
							<span class="mr-auto text-xs">little</span>
							<div class="flex">
								<div class="mx-2 flex flex-col items-center">
									<label for="bug_impact_1" class="mb-1 cursor-pointer text-xs">1</label>
									<input type="radio" id="bug_impact_1" name="bug_impact" value="1" required />
								</div>
								<div class="mx-2 flex flex-col items-center">
									<label for="bug_impact_2" class="mb-1 cursor-pointer text-xs">2</label>
									<input type="radio" id="bug_impact_2" name="bug_impact" value="2" />
								</div>
								<div class="mx-2 flex flex-col items-center">
									<label for="bug_impact_3" class="mb-1 cursor-pointer text-xs">3</label>
									<input type="radio" id="bug_impact_3" name="bug_impact" value="3" />
								</div>
								<div class="mx-2 flex flex-col items-center">
									<label for="bug_impact_4" class="mb-1 cursor-pointer text-xs">4</label>
									<input type="radio" id="bug_impact_4" name="bug_impact" value="4" />
								</div>
								<div class="mx-2 flex flex-col items-center">
									<label for="bug_impact_5" class="mb-1 cursor-pointer text-xs">5</label>
									<input type="radio" id="bug_impact_5" name="bug_impact" value="5" />
								</div>
							</div>
							<span class="ml-auto text-xs">unplayable</span>
						</div>
					</div>
					<label class="block">
						<div class="label-title">Bug category<sup>*</sup></div>
						<div class="label-subtitle">Please select the category that best describes your bug</div>
						<select name="bug_category" required>
							<option>General</option>
							<option>Matchmaking</option>
							<option>Connectivity</option>
							<option>Wallet integration</option>
							<option>Game mechanics</option>
							<option>UI / Visual</option>
							<option>Sound</option>
						</select>
					</label>
					<label class="block">
						<div class="label-title">Bug description<sup>*</sup></div>
						<div class="label-subtitle">Please describe your bug in as much details as possible</div>
						<textarea name="bug_description" rows="3" required />
					</label>
					<label class="block">
						<div class="label-title">
							Steps to reproduce the bug<sup>*</sup>
						</div>
						<div class="label-subtitle">Please provide steps to reproduce the bug</div>
						<textarea name="bug_steps" rows="3" required />
					</label>
					<label class="block">
						<div class="label-title">Please provide any images or videos to help identify the bug</div>
						<div class="label-subtitle">Upload files size limit is 20 MB</div>
						<input
							type="file"
							bind:files
							bind:this={fileInput}
							name="files"
							multiple
							accept="video/*,image/*"
							class="block w-full rounded border border-solid border-gray-300 bg-white bg-clip-padding px-3 py-1.5 text-gray-700 focus:outline-none"
						/>
						{#if filesSizeLimit}
							<p class="mt-0.5 text-red-700">Upload limit is 20 MB. Please select smaller files.</p>
						{/if}
					</label>
				</fieldset>
				<fieldset>
					<legend class="title--gradient">Device info</legend>
					<div class="block">
						<div class="label-title">Operating system<sup>*</sup></div>
						<input type="radio" id="android" name="device_os" value="android" required />
						<label for="android" class="label-radio">Andriod</label>
						<input type="radio" id="ios" name="device_os" value="ios" />
						<label for="ios" class="label-radio">iOS</label>
					</div>
					<label class="block">
						<div class="label-title">Operating system version</div>
						<div class="label-subtitle">
							Please write OS version of your device (e.g. Android 10,11 or iOS 13,14,...).<br />
							Unsure where to look, check here:
							<a class="text-cyan-700 underline" href="https://support.google.com/android/answer/7680439?hl=en" target="_blank" rel="noreferrer"
								>Android</a
							>,
							<a class="ml-1 text-cyan-700 underline" href="https://support.apple.com/en-us/HT201685" target="_blank" rel="noreferrer">iOS</a>
						</div>
						<input name="os_version" type="text" />
					</label>
					<label class="block">
						<div class="label-title">Device model</div>
						<div class="label-subtitle">E.g. iPhone 12 mini, POCO f4, Samsung Galaxy Z Fold 4, ...</div>
						<input name="device_model" type="text" />
					</label>
					<label class="block">
						<div class="label-title">Battery charge</div>
						<div class="label-subtitle">What was the battery charge on your phone at the time of the bug?</div>
						<select name="device_battery" class="w-full">
							<option>Not sure</option>
							<option>&gt; 80%</option>
							<option>&gt; 60%</option>
							<option>&gt; 40%</option>
							<option>&lt; 40%</option>
							<option>Plugged in</option>
						</select>
					</label>
					<div class="block">
						<div class="label-title">Internet connection<sup>*</sup></div>
						<div class="label-subtitle">How was your device connected to the internet?</div>
						<input name="device_connection" type="radio" id="wifi" value="wifi" required />
						<label for="wifi" class="label-radio">WiFi</label>

						<input name="device_connection" type="radio" id="mobile_data" value="mobile data" />
						<label for="mobile_data" class="label-radio">Mobile data</label>
					</div>
					<div class="block">
						<div class="label-title inline-flex items-center">Speedtest</div>
						<div class="label-subtitle">
							Please visit a <a class="text-cyan-700 underline" href="https://www.speedtest.net/" target="_blank" rel="noreferrer">speedtest.net</a> and
							write results of your connection speed
						</div>
						<div class="flex justify-between">
							<label class="mr-1">
								<span class="text-xs text-slate-600">Download</span>
								<div class="relative">
									<input name="speedtest_download" type="number" class="pr-10" />
									<span class="absolute right-2 top-3 text-[11px] text-slate-600">Mbps</span>
								</div>
							</label>
							<label class="ml-1">
								<span class="text-xs text-slate-600">Upload</span>
								<div class="relative">
									<input name="speedtest_upload" type="number" class="pr-10" />
									<span class="absolute right-2 top-3 text-[11px] text-slate-600">Mbps</span>
								</div>
							</label>
						</div>
					</div>
				</fieldset>
				<fieldset>
					<legend class="title--gradient">Keep in touch</legend>
					<label class="block">
						<div class="label-title">Is there anything else you want to share with the team?</div>
						<textarea name="general_msg" rows="2" />
					</label>
					<label class="block">
						<div class="label-title">Email address</div>
						<input name="email" type="email" class="w-full" />
					</label>
					<div class="block">
						<div class="flex items-center">
							<input type="checkbox" id="email_agreement" name="email_agreement" class="checkbox my-0 rounded-sm" />
							<label for="email_agreement" class="pl-2 text-xs leading-tight"
								>I agree to recieve general and product information emails from Cosmic Champs</label
							>
						</div>
					</div>
				</fieldset>
				<button class="button button--primary" type="submit"><span class="button__inner">Submit</span></button>
			</form>
		</div>
	</div>
{/if}

<style lang="scss" global>
	.title {
		@apply text-4xl sm:text-5xl;
	}
	fieldset {
		@apply rounded-md border-2 border-cyan-500 px-4 pb-2 pt-3 sm:px-8;
	}
	legend {
		@apply text-lg font-bold;
	}
	.label-title {
		@apply mb-2 text-sm font-medium leading-tight text-cyan-800 sm:text-base;
	}
	.label-subtitle {
		@apply -mt-1.5 mb-3 text-xs leading-tight text-slate-600;
	}
	.label-radio {
		@apply mr-6 inline-flex cursor-pointer items-center text-sm text-slate-700;
	}
	.block {
		@apply mb-5;
	}
	input,
	select,
	textarea {
		@apply rounded-md border border-slate-200 p-2 shadow-sm  focus:border-cyan-400 focus:ring focus:ring-cyan-400 focus:ring-opacity-30;
	}
	textarea,
	select {
		@apply w-full;
	}
	input[type='number'] {
		@apply w-full;
		&.timestamp {
			@apply hidden;
		}
	}
	input[type='text'] {
		@apply block w-full;
	}
	input[type='radio'] {
		@apply mr-2 rounded-full p-2.5 text-cyan-600;
	}
	input[type='checkbox'] {
		@apply mr-2 cursor-pointer rounded-sm p-2.5 text-cyan-600 focus:ring-offset-0;
	}
	.bug-impact {
		input[type='radio'] {
			@apply mr-0;
		}
	}
	sup {
		@apply text-[10px] text-red-600;
	}
</style>
