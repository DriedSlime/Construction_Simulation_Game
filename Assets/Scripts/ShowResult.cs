using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowResult : MonoBehaviour
{
    public TextMeshProUGUI demolitionText; // 철거 점수 텍스트
    public TextMeshProUGUI transportText;  // 운반 점수 텍스트
    public TextMeshProUGUI assemblyText;   // 조립 점수 텍스트

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 144;

        ShowResultPannel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowResultPannel()
    {
        string excavatorRank = PlayerPrefs.GetString("ExcavatorRank", "-");
        string transportRank = PlayerPrefs.GetString("TransportRank", "-");
        string craneRank = PlayerPrefs.GetString("CraneRank", "-");

        demolitionText.text = $"철거 :  {excavatorRank}";
        transportText.text = $"운반 :  {transportRank}";
        assemblyText.text = $"조립 :  {craneRank}";
    }

    public void ReturnFirstScene()
    {
        PlayerPrefs.DeleteKey("ExcavatorRank");
        PlayerPrefs.DeleteKey("TransportRank");
        PlayerPrefs.DeleteKey("CraneRank");
        SceneManager.LoadScene("StartScene");
    }
}
