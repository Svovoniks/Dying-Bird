using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.Mathematics;
using JetBrains.Annotations;
using System.IO;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle deathToggle;
    [SerializeField] private Slider volumeSlider;


    private List<string> resolutionList = new();
    private readonly List<string> DISPLAY_TYPES = new(){"Windowed", "FullScreen"};

    // Start is called before the first frame update
    void Start()
    {
        Resolution[] resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        displayDropdown.ClearOptions();
        qualityDropdown.ClearOptions();

        
        int curResIdx = 0;

        foreach (Resolution r in Screen.resolutions)
        {
            if ((float)r.width / r.height < 16f / 10)
            {
                continue;
            }

            string res = r.width + " x " + r.height;
            if (resolutionList.Contains(res)) 
            {
                continue;
            }

            resolutionList.Add(res);

            if (r.width == Screen.width
                && r.height == Screen.height)
            {
                curResIdx = resolutionList.Count-1;
            }
        }

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = curResIdx;
        resolutionDropdown.RefreshShownValue();

        displayDropdown.AddOptions(DISPLAY_TYPES);
        displayDropdown.value = Screen.fullScreen ? 1 : 0;
        displayDropdown.RefreshShownValue();

        qualityDropdown.AddOptions(QualitySettings.names.ToList());
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        deathToggle.isOn = Utils.GetBool(Utils.DEATH_KEY);
        
        audioMixer.GetFloat("Volume", out float curDec);

        volumeSlider.minValue = Utils.MIN_VOLUME;
        volumeSlider.maxValue = Utils.MAX_VOLUME;

        volumeSlider.value = Utils.DecibelToVolume(curDec);
    }
    
    public void SetDeathMode(bool mode)
    {
        PlayerPrefs.SetInt(Utils.DEATH_KEY, mode ? 1 : 0);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Utils.VolumeToDecibel(volume));
        PlayerPrefs.SetFloat(Utils.VOLUME_KEY, volume);
    }
    public void SetQuality(int idx)
    {
        QualitySettings.SetQualityLevel(idx);
    }

    public void SetDisplay(int idx)
    {
        Screen.fullScreen = idx == 1;
        string[] arr = resolutionList[resolutionDropdown.value].Split("x");
        Screen.SetResolution(int.Parse(arr[0]), int.Parse(arr[1]), idx == 1);
    }

    public void SetResolution(int idx)
    {
        string[] arr = resolutionList[idx].Split("x");
        Screen.SetResolution(int.Parse(arr[0]), int.Parse(arr[1]), Screen.fullScreen);
    }
}
