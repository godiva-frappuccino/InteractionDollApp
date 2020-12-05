using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class RecordBehavior : MonoBehaviour
{
    bool isRecord;
    InputField inputField;
    Text displayText;
    Text timerText;
    float timerTime;
    string behaviorName = "";
    string filename;
    float limitTime = 5.0f;
    int frameCounter = 0;
    GameObject kawauso;
    void Start()
    {
      isRecord = false;
      inputField = GameObject.Find("InputField").GetComponent<InputField>();
      kawauso = GameObject.Find("kawauso_for_app2");

      displayText = GameObject.Find("DisplayText").GetComponent<Text>();
      displayText.enabled = false;
      timerText = GameObject.Find("TimerText").GetComponent<Text>();
      timerText.text = "0";
      timerText.enabled = false;


      var pos = transform.position;
      Vector3 m_accel = Input.acceleration;
      Input.gyro.enabled = true;
      Gyroscope m_gyro = Input.gyro;
    }

    // Update is called once per frame
    void Update()
    {

      if(isRecord){
        frameCounter += 1;
        timerTime += Time.deltaTime;
        if(timerTime > limitTime)
        {
          FinishRecord();
        }
        timerText.text = timerTime.ToString("F1");
        //TODO:check
        //filename = "Assets/Resources/Behaviors/" + behaviorName + ".txt";
        filename = Application.persistentDataPath + "/" + behaviorName+".txt";
        //filename = GetSecureDataPath() + "/" + behaviorName + ".txt";
        Debug.Log(filename);
        if(!File.Exists(filename))
        {
          File.CreateText(filename).Dispose();
        }
        using(StreamWriter writer = new StreamWriter(filename, true))
        {
          var pos = transform.position;
          Vector3 m_accel = Input.acceleration;
          Gyroscope m_gyro = Input.gyro;
          if(m_gyro != null)
          {
            Quaternion trans = kawauso.transform.rotation;
            string toWrite = trans.x.ToString("F3") + "," + trans.y.ToString("F3") + "," + trans.z.ToString("F3") + "," + trans.w.ToString("F3");
            writer.WriteLine(toWrite);
            //writer.WriteLine(m_gyro.attitude);
            }
          writer.Flush();
          writer.Close();
        }
      }

    }// Start is called before the first frame update
    public void StartRecord()
    {
      isRecord = true;
      timerTime = 0.0f;
      displayText.enabled = true;
      timerText.enabled = true;
    }
    public void FinishRecord()
    {
      isRecord = false;
      displayText.enabled = false;
      timerText.enabled = false;
      MakeWavFile(filename);
      Debug.Log("Finish: " + frameCounter + ":" + (frameCounter/5));
    }
    void MakeWavFile(string filename)
    {
      // TODO: implement
    }
    public void getInputValue()
    {
      Debug.Log("GET CALLED");
      inputField = GameObject.Find("InputField").GetComponent<InputField>();
      behaviorName = inputField.text;
    }
    public void SetActiveText(bool value)
    {
      bool isActive = value;
      displayText.enabled = value;
    }
    /*
    public static string GetSecureDataPath()
    {
    #if !UNITY_EDITOR && UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var getFilesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
        {
            string secureDataPathForAndroid = getFilesDir.Call<string>("getCanonicalPath");
            return secureDataPathForAndroid;
        }
    #else
        // TODO: 本来は各プラットフォームに対応した処理が必要
        return Application.persistentDataPath;
    #endif
    }
    */
}
