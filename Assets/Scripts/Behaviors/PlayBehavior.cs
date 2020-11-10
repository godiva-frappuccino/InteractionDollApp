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
    List<Quaternion> attitudeList;
    Text displayText;
    Text filenameText;
    void Start()
    {
      isPlay = false;
      isFileSet = false;

      displayText = GameObject.Find("DisplayText").GetComponent<Text>();
      filenameText = GameObject.Find("FilenameText").GetComponent<Text>();
      displayText.enabled = false;
      filenameText.text = Path.GetFileName(ScrollViewController.selectedFilePath);

      var pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      Debug.Log("Playing" + isPlay);
      if(isPlay){
        string filename = ScrollViewController.selectedFilePath;
        if(!File.Exists(filename))
        {
          FinishPlayer();
          return;
        }
        if(!isFileSet)
        {
          Debug.Log("setting file…");
          attitudeList = getAttitudeList(filename);
          isFileSet = true;
          nowLine = 0;
          return;
        }
        if(nowLine >= attitudeList.Count())
        {
          Debug.Log("Read file finished…");
          FinishPlayer();
          return;
        }
        // in playing, only read data.(preprocess has already done.)
        kawauso.transform.rotation = Quaternion.Euler(90, 0, 0) * (attitudeList[nowLine]);
        nowLine++;
      }
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
        lineCounter++;
      }
      file.Close();
      return attitudeList;
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
        attitudeList.Clear();
      }
      isFileSet = false;

    }

    public void SetActiveText(bool value)
    {
      bool isActive = value;
      displayText.enabled = value;
    }

}
