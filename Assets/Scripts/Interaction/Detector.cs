using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    Gyroscope m_gyro;
    // TODO: how to convert queue to list
    Queue<Quaternion> attitudeQueue = new Queue<Quaternion>();
    int maxQueueSize = 30;

    // for datas preserved
    List<FileStructure> wavAndAttitudeFileList;

    // Start is called before the first frame update
    void Start()
    {
      m_gyro = Input.gyro;
      attitudeQueue.Enqueue(m_gyro.attitude);
      wavAndAttitudeFileList = getWavAndAttitudeFileList();
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
      foreach(FileStructure file in wavAndAttitudeFileList)
      {
        // TODO: fix params
        double similarlity = getSimilarlity(file.attitudeList, file.attitudeList);
        if(similarlity > 0.5 && !alreadySayFlag)
        {
          // TODO: implement
          // say(wavFilePath);
          alreadySayFlag = true;
        }
      }
    }

    List<FileStructure> getWavAndAttitudeFileList()
    {
      string path = "Assets/Datas/Behaviors/";
      string[] wavFilesPath = Directory.GetFiles(path, "*.wav");
      string[] attitudeFilesPath = Directory.GetFiles(path, "*.txt");
      List<FileStructure> fileList = new List<FileStructure>();
      for(int i = 0; i < wavFilesPath.Length; i++)
      {
        FileStructure file;
        file.attitudeList = getAttitudeList(attitudeFilesPath[i]);
        file.wavFilePath = wavFilesPath[i];
        fileList.Add(file);
      }
      return fileList;
    }
    List<Quaternion> getAttitudeList(string path)
    {
      List<Quaternion> attitudeList = new List<Quaternion>();
      return attitudeList;
    }
    double getSimilarlity(List<Quaternion> a1, List<Quaternion> a2)
    {
      // TODO: implement
      return 0.0f;
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
    void Voice()
    {
      // TODO: implement
    }

}

public struct FileStructure
{
  public string wavFilePath;
  public List<Quaternion> attitudeList;
}
