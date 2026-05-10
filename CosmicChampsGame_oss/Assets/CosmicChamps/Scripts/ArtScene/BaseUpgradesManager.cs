using UnityEngine;

public class BaseUpgradesManager : MonoBehaviour
{


    [SerializeField]
    GameObject[] upgradeParts;

    // Start is called before the first frame update
    void Start()
    {
        SetUpgradeActive(false);
    }

    public void ShowUpgrades() 
    {
        SetUpgradeActive(true);
    }

    public void HideUpgrades() 
    {
        SetUpgradeActive(false);
    }

    // Update is called once per frame
    void SetUpgradeActive(bool state)
    {
        upgradeParts[0].SetActive(state);        
        upgradeParts[1].SetActive(state);        

    }
}
