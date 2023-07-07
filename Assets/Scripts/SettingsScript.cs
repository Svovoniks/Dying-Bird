using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.Mathematics;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle deathToggle;
    [SerializeField] private Slider volumeSlider;


    private Resolution[] resolutions;
    private List<string> DISPLAY_TYPES = new(){"FullScreen", "Windowed"};

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        displayDropdown.ClearOptions();
        qualityDropdown.ClearOptions();

        List<string> resolutionList = new();
        int curResIdx = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionList.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height)
            {
                curResIdx = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = curResIdx;
        resolutionDropdown.RefreshShownValue();

        displayDropdown.AddOptions(DISPLAY_TYPES);
        displayDropdown.value = Screen.fullScreen ? 0 : 1;
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
        Debug.Log(volume);
        audioMixer.SetFloat("Volume", Utils.VolumeToDecibel(volume));
        PlayerPrefs.SetFloat(Utils.VOLUME_KEY, volume);
    }
    public void SetQuality(int idx)
    {
        QualitySettings.SetQualityLevel(idx);
    }

    public void SetDisplay(int idx)
    {
        Screen.fullScreen = idx == 0 ? true : false;
    }

    public void SetResolution(int idx)
    {
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, Screen.fullScreen);
    }
}
