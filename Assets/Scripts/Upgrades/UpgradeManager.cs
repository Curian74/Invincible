using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
     public GameObject upgradePanel;
    public List<UpgradeOption> allUpgrades;
    public List<Button> upgradeButtons;
    public Text upgradeDescriptionText; // UI Text hiển thị mô tả
    public PlayerStats playerStats;
    private List<UpgradeOption> currentUpgrades;


    private void Start()
    {
        upgradePanel.SetActive(false);
        upgradeDescriptionText.text = "";
    }

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0;

        currentUpgrades = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < currentUpgrades.Count)
            {
                UpgradeOption upgrade = currentUpgrades[i];
                upgradeButtons[i].GetComponentInChildren<Text>().text = upgrade.upgradeName;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgrade));

                // Hiển thị mô tả khi di chuột vào
                int index = i;
                upgradeButtons[i].onClick.AddListener(() => ShowUpgradeDescription(currentUpgrades[index]));
            }
        }
    }

    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> randomUpgrades = new List<UpgradeOption>();
        List<UpgradeOption> tempList = new List<UpgradeOption>(allUpgrades);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, tempList.Count);
            randomUpgrades.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        return randomUpgrades;
    }

    private void ApplyUpgrade(UpgradeOption upgrade)
    {
        Debug.Log("Upgrade picked: " + upgrade.upgradeName);
        upgrade.Apply(playerStats);
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void ShowUpgradeDescription(UpgradeOption upgrade)
    {
        upgradeDescriptionText.text = upgrade.description;
    }

}
