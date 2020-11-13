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
    int maxQueueSize = 50;
    float lastSpeakedTime;
    float minSpeakSpan = 3.0f;
    bool flag = true;
    // for datas preserved
    List<FileStructure> wavAndAttitudeFileList;

    // Start is called before the first frame update
    void Start()
    {
      kawauso = GameObject.Find("kawauso_for_app2");
      m_gyro = Input.gyro;
      attitudeQueue.Enqueue(m_gyro.attitude);
      wavAndAttitudeFileList = getWavAndAttitudeFileList();
      lastSpeakedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
      m_gyro = Input.gyro;
      if(m_gyro != null)
      {
        Quaternion position = kawauso.transform.rotation;
        attitudeQueue.Enqueue(position);
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
        RecordValuesInEuler(wavAndAttitudeFileList[0].attitudeList, "stored_rot.csv");
        RecordValuesInEuler(sensorAttitudeList, "sensor_rot.csv");
        flag = false;
      }
      if(Time.time - lastSpeakedTime < minSpeakSpan)
      {
        return;
      }
      foreach(FileStructure file in wavAndAttitudeFileList)
      {
        double similarlity = getSimilarlity(sensorAttitudeList, file.attitudeList);
        Debug.Log(file.attitudeFilePath + ":" + similarlity);
        if(similarlity > 0.5 && !alreadySayFlag)
        {
          Debug.Log("Similar!!!!:" + file.attitudeFilePath);
          // TODO: implement
          // say(wavFilePath);
          alreadySayFlag = true;
          lastSpeakedTime = Time.time;
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
      int minListLength = Math.Min(storedList.Count, sensorList.Count);
      for(int i=0; i < minListLength; i++)
      {
        diffSum += (double)Mathf.Pow(Math.Abs(getGyroDiff(sensorList[i], storedList[i])), 2);
      }
      double diffAverage = (double)diffSum / (double)minListLength;

      return 1 / diffAverage;
    }
    float getGyroDiff(Quaternion q1, Quaternion q2)
    {
      float diffSum = 0.0f;
      Vector3 r1 = q1.eulerAngles;
      Vector3 r2 = q2.eulerAngles;
      diffSum += Mathf.Cos(Mathf.Deg2Rad * (r1.x - r2.x));
      diffSum += Mathf.Cos(Mathf.Deg2Rad * (r1.y - r2.y));
      diffSum += Mathf.Cos(Mathf.Deg2Rad * (r1.z - r2.z));
      diffSum = (float)Math.Sqrt(diffSum);
      return diffSum;
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
