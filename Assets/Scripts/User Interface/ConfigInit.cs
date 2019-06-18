using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigInit : MonoBehaviour
{
    [SerializeField] Dropdown qualityDropDown;
    [SerializeField] Dropdown resolutionDropDown;
    [SerializeField] Toggle fullscreenToggle;
    void Start(){
        resolutionDropDown.ClearOptions();
        qualityDropDown.ClearOptions();
        fullscreenToggle.isOn = Screen.fullScreen;
        getResolution();
    }

    void getResolution(){
        GameManager.Instance.updateResolutions();
        resolutionDropDown.AddOptions(GameManager.Instance.resolutions);
        resolutionDropDown.value = GameManager.Instance.currentResolution;
    }
}
