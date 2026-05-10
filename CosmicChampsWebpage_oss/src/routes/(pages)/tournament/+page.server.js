/** @type {import('./$types').Actions} */
export const actions = {
	default: async ({ request }) => {
		// submit form to Google Forms
		const data = await request.formData();
		const address = data.get('address');
		const agreement = data.get('email_agreement') ? 'Yes' : 'No';
		const email = data.get('email') ? data.get('email') : '';
		const algorand = data.get('algorand');
		const other_blockchains = data.get('other_blockchains');
		const blockchains = data.get('blockchains') ? data.get('blockchains') : '';
		const referral = data.get('referral') ? data.get('referral') : '';
		const token = data.get('token') ? 'Yes' : 'No';

		// viewform?usp=pp_url&entry.115079860=address&entry.885271560=mail&entry.27457922=yes&entry.103401561=Yes&entry.661642338=No&entry.719248846=etherium,+polygon&entry.1531360088=tanja.algo

		const formUrl = 'https://docs.google.com/forms/d/e/1FAIpQLSf87XIyCpHBPwFrMwOwxVj1jeLcE-p-Rcaq_NlDzxwKCtipqw/formResponse?usp=pp_url';
		const url = `${formUrl}&entry.115079860=${address}&entry.885271560=${email}&entry.1867215194=${agreement}&entry.103401561=${algorand}&entry.661642338=${other_blockchains}&entry.719248846=${blockchains}&entry.1531360088=${referral}&entry.684400045=${token}&submit=Submit`;
		const encodedURL = encodeURI(url);
		const response = await fetch(encodedURL);

		if (response.ok) {
			return {
				success: true
			};
		}
	}
};
