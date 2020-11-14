using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    Gyroscope m_gyro;
    GameObject kawauso;
    Queue<Quaternion> attitudeQueue = new Queue<Quaternion>();
    Quaternion initialRotation;
    int maxQueueSize = 50;
    float lastSpeakedTime;
    float minSpeakSpan = 3.0f;
    bool flag = false;
    float similarityThresh = 0.80f;
    // for datas preserved
    List<FileStructure> wavAndAttitudeFileList;

    // Start is called before the first frame update
    void Start()
    {
      kawauso = GameObject.Find("kawauso_for_app2");
      initialRotation = Input.gyro.attitude;
      wavAndAttitudeFileList = getWavAndAttitudeFileList();
      lastSpeakedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
      m_gyro = Input.gyro;
      if(m_gyro != null)
      {
        Quaternion rotation = kawauso.transform.rotation;
        attitudeQueue.Enqueue(rotation * Quaternion.Euler(0, initialRotation.y, 0));
      }
      if(attitudeQueue.Count > maxQueueSize)
      {
        while(attitudeQueue.Count > maxQueueSize)
        {
          Quaternion notUsed = attitudeQueue.Dequeue();
        }
      }
      bool alreadySayFlag = false;

      List<Quaternion> sensorAttitudeList = new List<Quaternion>();
      sensorAttitudeList.AddRange(attitudeQueue.ToArray());
      if(sensorAttitudeList.Count < maxQueueSize)
      {
        return;
      }
      if(flag)
      {
        for(int i = 0; i < wavAndAttitudeFileList.Count; i++)
        {
          double similarlity = getSimilarlity(wavAndAttitudeFileList[i].attitudeList, sensorAttitudeList);
          RecordValuesInEuler(wavAndAttitudeFileList[i].attitudeList, "data" + i + ".csv");
        }
        RecordValuesInEuler(sensorAttitudeList, "sensor.csv");
        flag = false;
      }
      if(Time.time - lastSpeakedTime < minSpeakSpan)
      {
        return;
      }
      foreach(FileStructure file in wavAndAttitudeFileList)
      {
        double similarlity = getSimilarlity(sensorAttitudeList, file.attitudeList);
        //Debug.Log(file.attitudeFilePath + ":" + similarlity);
        if(similarlity > similarityThresh && !alreadySayFlag)
        {
          Debug.Log("Similar!!!!:" + file.attitudeFilePath);
          // TODO: implement
          // say(wavFilePath);
          alreadySayFlag = true;
          //lastSpeakedTime = Time.time;
        }
      }
    }
    List<FileStructure> getWavAndAttitudeFileList()
    {
      string path = "Assets/Datas/Behaviors/";
      string[] wavFilesPath = Directory.GetFiles(path, "*.wav");
      string[] attitudeFilesPath = Directory.GetFiles(path, "*.txt");

      List<FileStructure> fileList = new List<FileStructure>();
      for(int i = 0; i < attitudeFilesPath.Length; i++)
      {
        FileStructure file;
        file.attitudeList = getAttitudeList(attitudeFilesPath[i]);
        // TODO: fix type
        file.wavFilePath = "None";
        file.attitudeFilePath = attitudeFilesPath[i];
        fileList.Add(file);
      }
      Debug.Log("Reading file done…");
      return fileList;
    }
    List<Quaternion> getAttitudeList(string filepath)
    {
      List<Quaternion> attitudeList = new List<Quaternion>();
      System.IO.StreamReader file = new System.IO.StreamReader(filepath);
      string line;
      while((line = file.ReadLine()) != null)
      {
        float x = float.Parse(line.Split(',')[0].Replace("(",""));
        float y = float.Parse(line.Split(',')[1]);
        float z = float.Parse(line.Split(',')[2]);
        float w = float.Parse(line.Split(',')[3].Replace(")",""));
        attitudeList.Add(new Quaternion(x,y,z,w));
      }
      file.Close();
      return attitudeList;
    }
    double getSimilarlity(List<Quaternion> sensorList, List<Quaternion> storedList)
    {
      double diffSum = 0.0;
      double thresh = (double)Mathf.Cos(30.0f /180.0f*3.14f);
      int minListLength = Math.Min(storedList.Count, sensorList.Count);

      for(int i=0; i < minListLength; i++)
      {
        Vector3 r1 = sensorList[i].eulerAngles;
        Vector3 r2 = storedList[i].eulerAngles;
        if(Mathf.Cos(Mathf.Deg2Rad * (r1.x - r2.x)) < thresh) diffSum += 1.0;
        else if(Mathf.Cos(Mathf.Deg2Rad * (r1.y - r2.y)) < thresh) diffSum += 1.0;
        else if(Mathf.Cos(Mathf.Deg2Rad * (r1.z - r2.z)) < thresh) diffSum += 1.0;
        //diffSum += (double)Mathf.Pow(Math.Abs(getGyroDiff(sensorList[i], storedList[i])), 2);
      }
      double diffAverage = diffSum / (double)minListLength;
      return 1 - diffAverage;
    }
    void RecordValuesInEuler(List<Quaternion> quaternionList, string filePath)
    {
      using(StreamWriter writer = new StreamWriter(filePath, true))
      {
        foreach(Quaternion quat in quaternionList)
        {
          Vector3 rot = quat.eulerAngles;
          string toWrite = rot.x.ToString("F") + "," + rot.y.ToString("F") + "," + rot.z.ToString("F");
          writer.WriteLine(toWrite);
          }
        writer.Close();
      }
    }
    void Voice(string name)
    {
      // TODO: implement
    }
}

public struct FileStructure
{
  public string attitudeFilePath;
  public string wavFilePath;
  public List<Quaternion> attitudeList;
}
