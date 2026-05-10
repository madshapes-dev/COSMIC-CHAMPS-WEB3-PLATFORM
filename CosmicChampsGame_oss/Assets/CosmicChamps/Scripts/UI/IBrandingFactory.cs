using System.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.UI
{
    public interface IBrandingFactory
    {
        Task<GameObject> GetCountdown (string opponentId, Transform parent = null);
        Task<GameObject> GetWin (string opponentId, Transform parent = null);
        Task<GameObject> GetLoss (string opponentId, Transform parent = null);
    }
}