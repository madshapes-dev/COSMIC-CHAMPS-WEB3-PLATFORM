// @ts-nocheck

export function clickOutside(element, callbackFunction) {
	function onClick(event) {
		if (!element.contains(event.target)) {
			callbackFunction(event);
		}
	}

	document.body.addEventListener('click', onClick);

	return {
		update(newCallbackFunction) {
			callbackFunction = newCallbackFunction;
		},
		destroy() {
			document.body.removeEventListener('click', onClick);
		}
	};
}

// Convert string to kebab case
export function toKebabCase(string) {
	return string.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
}

export function copyToClipboard(copyTextId) {
	// Get text or input field
	var copyText = document.getElementById(copyTextId);
	// Select the input field
	copyText.select();
	copyText.setSelectionRange(0, 99999); // For mobile devices
	// Copy the text inside the input field
	navigator.clipboard.writeText(copyText.value);
}

export function shortenString(str) {
	let stringLength = str.length;
	if (stringLength > 22) {
		let start = str.slice(0, 13);
		let end = str.slice(-5);
		return start + '... ' + end;
	}
	return str;
}
