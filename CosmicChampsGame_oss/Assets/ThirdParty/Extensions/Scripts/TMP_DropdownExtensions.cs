using TMPro;

namespace ThirdParty.Extensions
{
    public static class TMP_DropdownExtensions
    {
        public static int GetValueAsInt (this TMP_Dropdown tmpDropdown)
        {
            var option = tmpDropdown.options[tmpDropdown.value];
            var chunks = option.text.Split (' ');
            foreach (var chunk in chunks)
            {
                if (int.TryParse (chunk, out var result))
                    return result;
            }

            return 0;
        }
    }
}