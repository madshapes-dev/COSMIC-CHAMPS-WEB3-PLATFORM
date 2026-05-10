export function formBody(body) {
	return [...body.entries()].reduce((data, [k, v]) => {
		let value = v;
		if (v === 'true') value = true;
		if (v === 'false') value = false;
		if (!isNaN(Number(v))) value = Number(v);

		// For grouped fields like multi-selects and checkboxes, we need to
		// store the values in an array.
		if (k in data) {
			const val = data[k];
			value = Array.isArray(val) ? [...val, value] : [val, value];
		}

		data[k] = value;

		return data;
	}, {});
}
