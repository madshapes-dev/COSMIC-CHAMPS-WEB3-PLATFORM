/** @type {import('./$types').Actions} */
export const actions = {
	default: async ({ request }) => {
		// submit form to Google Forms
		const data = await request.formData();
		const id = data.get('id');
		const email = data.get('email_agreement') ? data.get('email') : '';
		const deviceOS = data.get('device_os');
		const deviceOSversion = data.get('os_version');
		const deviceModel = data.get('device_model');
		const deviceBattery = data.get('device_battery');
		const deviceConnection = data.get('device_connection');
		const speedtestDown = data.get('speedtest_download');
		const speedtestUp = data.get('speedtest_upload');
		const bugImpact = data.get('bug_impact');
		const bugCategory = data.get('bug_category');
		const bugDescription = data.get('bug_description');
		const bugSteps = data.get('bug_steps');
		const msg = data.get('general_msg');

		const formUrl = 'https://docs.google.com/forms/d/e/1FAIpQLSeNIISV2whYA_ACjp5q3qfhg65QLz4fZw8pUFs6V57zBrEoSw/formResponse?usp=pp_url';
		const url = `${formUrl}&entry.315180490=${email}&entry.135456494=${deviceOS}&entry.1617940255=${deviceModel}&entry.2138369666=${deviceOSversion}&entry.478571918=${deviceBattery}&entry.919835730=${deviceConnection}&entry.427053253=Download:+${speedtestDown},+Upload:+${speedtestUp}&entry.98808643=${bugImpact}&entry.966112849=${bugCategory}&entry.1164673991=${bugDescription}&entry.2108884243=${bugSteps}&entry.1044245360=${msg}&entry.175813408=${id}&submit=Submit`;
		const encodedURL = encodeURI(url);
		const response = await fetch(encodedURL);

		if (response.ok) {
			return {
				success: true
			};
		}
	}
};
