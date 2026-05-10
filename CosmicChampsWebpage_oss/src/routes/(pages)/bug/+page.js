// Used as endpoint to fetch Google Sheets data
// data is exported in /bug/+page.svelte

// export async function load({ fetch }) {
//   const response = await fetch(`/api/bug/sheets`, {
//     method: 'GET',
//     headers: {
//       'Content-Type': 'application/json',
//     }
//   });
//   let items = await response.json();
//   items.values = items.values.filter((item)=>{if(item[13]=="" || item[13]==undefined) return item; })
//   // console.log(items)
//   return items;
// }
