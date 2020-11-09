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
    float limitTime = 3.0f;
    void Start()
    {
      isRecord = false;
      inputField = GameObject.Find("InputField").GetComponent<InputField>();

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
        timerTime += Time.deltaTime;
        if(timerTime > limitTime)
        {
          FinishRecord();
        }
        timerText.text = timerTime.ToString("F3");
        string filename = "Assets/Datas/Behaviors/" + behaviorName + ".txt";
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
            Debug.Log("Gyro: " + m_gyro.attitude);
            // preprocess only recording
            float x = -m_gyro.attitude.x;
            float y = -m_gyro.attitude.y;
            float z = m_gyro.attitude.z;
            float w = m_gyro.attitude.w;
            transform.rotation = Quaternion.Euler(90, 0, 0) * (new Quaternion(x, y, z, w));
            string toWrite = x.ToString("F3") + "," + y.ToString("F3") + "," + z.ToString("F3") + "," + w.ToString("F3");
            Debug.Log("ToWrite: " + toWrite);
            writer.WriteLine(toWrite);
            //writer.WriteLine(m_gyro.attitude);
            }
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
