using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class PlayBehavior : MonoBehaviour
{
    bool isPlay;
    bool isFileSet;
    int lineCounter;
    int nowLine;
    public GameObject kawauso;
    List<string> behaviorTextData;
    InputField inputField;
    Text displayText;
    string behaviorName = "";
    void Start()
    {
      isPlay = false;
      isFileSet = false;
      behaviorTextData = new List<string>(){};
      inputField = GameObject.Find("InputField").GetComponent<InputField>();

      displayText = GameObject.Find("DisplayText").GetComponent<Text>();
      displayText.enabled = false;


      var pos = transform.position;
      Vector3 m_accel = Input.acceleration;
      Input.gyro.enabled = true;
      Gyroscope m_gyro = Input.gyro;
    }

    // Update is called once per frame
    void Update()
    {
      Debug.Log("Playing" + isPlay);
      if(isPlay){
        string filename = "Assets/Datas/Behaviors/" + behaviorName + ".txt";
        if(!File.Exists(filename))
        {
          FinishPlayer();
          return;
        }
        if(!isFileSet)
        {
          Debug.Log("setting file…");
          SetFile(filename);
          isFileSet = true;
          nowLine = 0;
          return;
        }
        if(nowLine >= lineCounter)
        {
          Debug.Log("Read file finished…");
          FinishPlayer();
          return;
        }
        string m_gyro_txt = ReadNextLine(nowLine);
        float x = float.Parse(m_gyro_txt.Split(',')[0].Replace("(",""));
        float y = float.Parse(m_gyro_txt.Split(',')[1]);
        float z = float.Parse(m_gyro_txt.Split(',')[2]);
        float w = float.Parse(m_gyro_txt.Split(',')[3].Replace(")",""));

        if(m_gyro_txt != null)
        {
          Debug.Log("Running transform");
          kawauso.transform.rotation = Quaternion.Euler(90, 0, 0) * (new Quaternion(-x, -y, z, w));
        }
        nowLine++;
      }
    }
    public void SetFile(string path)
    {
      string line;
      lineCounter = 0;
      System.IO.StreamReader file = new System.IO.StreamReader(path);
      while((line = file.ReadLine()) != null)
      {
        behaviorTextData.Add(line);
        lineCounter++;
      }
      Debug.Log("Set data: " + behaviorTextData);

      file.Close();
    }
    public string ReadNextLine(int line)
    {
      return behaviorTextData[line];
    }
    public void StartPlayer()
    {
      isPlay = true;
    }
    public void FinishPlayer()
    {
      isPlay = false;
      if(isFileSet)
      {
        behaviorTextData.Clear();
      }
      isFileSet = false;

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

}
