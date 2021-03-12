using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetDropdownValues : MonoBehaviour
{

    [SerializeField] TMP_Dropdown senarioDropDown;
    [SerializeField] TMP_Dropdown levelDropDown;
    [SerializeField] TMP_Dropdown subLevelDropDown;

    [SerializeField] private TMP_Text displaySelectedValue;
    [SerializeField] private TMP_Text noValue;
    public WindowGraph windowGraph;
    public LevelLoader levelLoader;
    void OnEnable()
    {
        displaySelectedValue.text = SetDisplayText(senarioDropDown.value, levelDropDown.value, subLevelDropDown.value);
        //Invoke("ShowValues",.2f);
        Invoke("ShowFullLevelPercent", .2f);
    }

    private void ShowFullLevelPercent()
    {
        var allLevels = levelLoader.currentLevelStats;
        var complatedAllLevelPercent = allLevels.GetComplatedAllLevelPercent();
        var complatedLevelPercentBySenario = allLevels.GetComplatedLevelPercentBySenario();

        List<float> data = new List<float>(){complatedAllLevelPercent};
        windowGraph.StartShowGraph(data);
        windowGraph.ShowPieGraph(complatedLevelPercentBySenario);
    }
    private void ShowValues()
    {
        var data = SaveLoadUserData.instance.GetSpecificUserData(senarioDropDown.value + 1,
            levelDropDown.value + 1, subLevelDropDown.value + 1);
        if (data.Count == 0)
        {
            noValue.gameObject.SetActive(true);
        }
        else
        {
            noValue.gameObject.SetActive(false);
        }
        windowGraph.StartShowGraph(data);
    }

    public void SenarioDropDownChange(TMP_Dropdown change)
    {
        displaySelectedValue.text = change.value.ToString();
        displaySelectedValue.text = SetDisplayText(senarioDropDown.value, levelDropDown.value, subLevelDropDown.value);
        ShowValues();
    }

    public void LevelDropDownChange(TMP_Dropdown change)
    {
        displaySelectedValue.text = change.value.ToString();
        displaySelectedValue.text = SetDisplayText(senarioDropDown.value, levelDropDown.value, subLevelDropDown.value);
        ShowValues();
    }

    public void SubLevelDropDownChange(TMP_Dropdown change)
    {
        displaySelectedValue.text = change.value.ToString();
        displaySelectedValue.text = SetDisplayText(senarioDropDown.value, levelDropDown.value, subLevelDropDown.value);
        ShowValues();
    }

    string SetDisplayText(int senario, int level, int subLevel)
    {
        return (senario + 1) + ". Senaryo " + (level + 1) + ". bölümün " + (subLevel + 1) +
               ". alt bölümü verileri.";
    }
}
