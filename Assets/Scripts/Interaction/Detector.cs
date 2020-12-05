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


    int maxQueueSize = 75;
    float lastSpeakedTime;
    float minSpeakSpan = 3.0f;
    float minDisplayTime = 1.5f;
    bool flag = false;
    float similarityThresh = 0.90f;
    // for datas preserved
    List<FileStructure> wavAndAttitudeFileList;
    //TextToSpeech tts;
    [SerializeField]
    GameObject voicePrefab;

    public GameObject voiceText;
    public GameObject voiceContentPanel;
    public Text voiceContentText;
    public Text debugText;

    // Start is called before the first frame update
    void Start()
    {
      voiceContentPanel.SetActive(false);
      voiceText.SetActive(false);
      kawauso = GameObject.Find("kawauso_for_app2");
      initialRotation = Input.gyro.attitude;
      wavAndAttitudeFileList = getWavAndAttitudeFileList();
      lastSpeakedTime = Time.time;
      /*
      // testing
      GameObject obj = Instantiate(voicePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
      AudioSource source = obj.GetComponent<AudioSource>();
      //source.clip = Resources.Load<AudioClip>("Voices/cat2");
      source.Play();
      */
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
      /*
      // For analyzing
      if(flag)
      {
        for(int i = 0; i < wavAndAttitudeFileList.Count; i++)
        {
          double similarlity = getSimilarlity(wavAndAttitudeFileList[i].attitudeList, sensorAttitudeList);
          //RecordValuesInEuler(wavAndAttitudeFileList[i].attitudeList, "data" + i + ".csv");
        }
        //RecordValuesInEuler(sensorAttitudeList, "sensor.csv");
        flag = false;
      }
      */
      if(Time.time - lastSpeakedTime < minSpeakSpan)
      {
        if(Time.time - lastSpeakedTime > minDisplayTime)
        {
          voiceText.SetActive(false);
          //voiceContentPanel.SetActive(false);
          voiceContentText.text = "";
        }
        return;
      }else{
      }
      debugText.text = "";
      foreach(FileStructure file in wavAndAttitudeFileList)
      {
        double similarlity = getSimilarlity(sensorAttitudeList, file.attitudeList);
        debugText.text += file.voiceContentText + ":" + similarlity + "  ";

        if(similarlity > similarityThresh && !alreadySayFlag)
        {
          // TODO: implement
          /*
          GameObject obj = Instantiate(voicePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
          AudioSource source = obj.GetComponent<AudioSource>();
          source.clip = Resources.Load<AudioClip>(file.wavFilePath);
          source.Play();
          Debug.Log("playing:" + file.wavFilePath);
          */
          voiceText.SetActive(true);
          voiceContentPanel.SetActive(true);
          voiceContentText.text = file.voiceContentText;


          alreadySayFlag = true;
          lastSpeakedTime = Time.time;
        }
      }
    }
    List<FileStructure> getWavAndAttitudeFileList()
    {
      //TODO: check
      //string attitudePath = "Assets/Resources/Behaviors/";
      string attitudePath = Application.persistentDataPath;
      //string attitudePath = RecordBehavior.GetSecureDataPath();
      //string wavPath = "Assets/Resources/Voices/";

      //string[] wavFilesPath = Directory.GetFiles(wavPath, "*.mp3");
      string[] attitudeFilesPath = Directory.GetFiles(attitudePath, "*.txt");

      List<FileStructure> fileList = new List<FileStructure>();
      for(int i = 0; i < attitudeFilesPath.Length; i++)
      {
        FileStructure file;
        file.attitudeList = getAttitudeList(attitudeFilesPath[i]);
        // TODO; check
        file.wavFilePath = "tmp";
        //file.wavFilePath = "Voices/" + System.IO.Path.GetFileNameWithoutExtension(wavFilesPath[UnityEngine.Random.Range(0, wavFilesPath.Length)]);
        //file.wavFilePath = Application.persistentDataPath + "/" + System.IO.Path.GetFileNameWithoutExtension(wavFilesPath[UnityEngine.Random.Range(0, wavFilesPath.Length)]);
        //file.wavFilePath = wavFilesPath + "/" + System.IO.Path.GetFileNameWithoutExtension(wavFilesPath[UnityEngine.Random.Range(0, wavFilesPath.Length)]);
        file.attitudeFilePath = attitudeFilesPath[i];
        file.voiceContentText = System.IO.Path.GetFileNameWithoutExtension(attitudeFilesPath[i]);
        fileList.Add(file);
      }
      // sorting
      fileList.Sort((a, b) => b.attitudeList.Count - a.attitudeList.Count);
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
      double thresh = (double)Mathf.Cos(10.0f /180.0f*3.14f);
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
      debugText.text += (minListLength - diffSum) + "/" + minListLength;
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
  public string voiceContentText;
}
