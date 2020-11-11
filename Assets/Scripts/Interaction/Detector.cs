using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    Gyroscope m_gyro;
    Queue<Quaternion> attitudeQueue = new Queue<Quaternion>();
    int maxQueueSize = 30;
    float lastSpeakedTime;
    float minSpeakSpan = 3.0f;
    // for datas preserved
    List<FileStructure> wavAndAttitudeFileList;

    // Start is called before the first frame update
    void Start()
    {
      m_gyro = Input.gyro;
      attitudeQueue.Enqueue(m_gyro.attitude);
      wavAndAttitudeFileList = getWavAndAttitudeFileList();
      lastSpeakedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
      m_gyro = Input.gyro;
      // TODO: implement time.sleep(while saying word);
      if(m_gyro != null)
      {
        Quaternion processed = Preprocess(m_gyro.attitude);
        attitudeQueue.Enqueue(processed);
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
      if(Time.time - lastSpeakedTime < minSpeakSpan)
      {
        return;
      }
      foreach(FileStructure file in wavAndAttitudeFileList)
      {
        double similarlity = getSimilarlity(sensorAttitudeList, file.attitudeList);
        if(similarlity > 0.5 && !alreadySayFlag)
        {
          Debug.Log("Similar:" + file.attitudeFilePath);
          // TODO: implement
          // say(wavFilePath);
          alreadySayFlag = true;
          lastSpeakedTime = Time.time;
          Debug.Log("LastTime:" + lastSpeakedTime);
        }
      }
    }
    List<FileStructure> getWavAndAttitudeFileList()
    {
      string path = "Assets/Datas/Behaviors/";
      string[] wavFilesPath = Directory.GetFiles(path, "*.wav");
      string[] attitudeFilesPath = Directory.GetFiles(path, "*.txt");
      Debug.Log(attitudeFilesPath.Length);
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
      // TODO: implement
      float diffSum = 0.0f;
      int minListLength = Math.Min(storedList.Count, sensorList.Count);
      for(int i=0; i < minListLength; i++)
      {
        diffSum += (float)Math.Sqrt(Math.Abs(getGyroDiff(sensorList[i], storedList[i])));
      }
      float diffAverage = diffSum / minListLength;

      return 1 - diffAverage;
    }
    float getGyroDiff(Quaternion q1, Quaternion q2)
    {
      float diffSum = 0.0f;
      diffSum += Mathf.Pow(q1.x - q2.x, 2);
      diffSum += Mathf.Pow(q1.y - q2.y, 2);
      diffSum += Mathf.Pow(q1.z - q2.z, 2);
      diffSum += Mathf.Pow(q1.w - q2.w, 2);
      diffSum = (float)Math.Sqrt(diffSum);
      return diffSum;
    }
    List<Quaternion> readFile(string path)
    {
      // TODO: implement
      return new List<Quaternion>();
    }
    Quaternion Preprocess(Quaternion attitude)
    {
      float x = -attitude.x;
      float y = -attitude.y;
      float z = attitude.z;
      float w = attitude.w;
      return new Quaternion(x, y, z, w);
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
