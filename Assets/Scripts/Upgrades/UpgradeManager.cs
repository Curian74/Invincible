using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private float upgradePanelDelayTime = 0.5f;
    private const int UPGRADE_COUNT = 3;
    public GameObject upgradePanel;
    public List<UpgradeOption> allUpgrades;
    public List<Button> upgradeButtons;
    public PlayerStats playerStats;
    private List<UpgradeOption> currentUpgrades;

    private void Awake()
    {
        upgradePanel.SetActive(false);
        blackPanel.SetActive(false);
    }

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);
        blackPanel.SetActive(true);
        Time.timeScale = 0;

        currentUpgrades = GetRandomUpgrades(UPGRADE_COUNT);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < currentUpgrades.Count)
            {
                UpgradeOption upgrade = currentUpgrades[i];
                Text[] texts = upgradeButtons[i].GetComponentsInChildren<Text>();
                texts[0].text = upgrade.upgradeName;
                texts[1].text = upgrade.description;
                upgradeButtons[i].GetComponent<Image>().sprite = upgrade.backgroundColor;
                Image upgradeImage = upgradeButtons[i].transform.Find("Icon").GetComponent<Image>();
                upgradeImage.sprite = upgrade.icon;

                // Disable button initially
                upgradeButtons[i].interactable = false;

                // Store index for the lambda function
                int index = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgrade));
            }
        }

        StartCoroutine(EnableUpgradeButtonsAfterDelay(upgradePanelDelayTime));
    }

    private IEnumerator EnableUpgradeButtonsAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        foreach (Button button in upgradeButtons)
        {
            button.interactable = true;
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
        ScoreManager.Instance.UpdateUpgradeLevelUI();
        blackPanel.SetActive(false);
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
