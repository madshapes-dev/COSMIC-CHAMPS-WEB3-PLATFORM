using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    public class CustomScrollRect : ScrollRect
    {
        public void AdjustContentPosition (Vector2 delta)
        {
            content.anchoredPosition += delta;
            m_ContentStartPosition += delta;
        }
    }
}