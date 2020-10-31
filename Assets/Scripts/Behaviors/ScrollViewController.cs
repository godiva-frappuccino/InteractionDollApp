using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject popUp;
    public Button removeFileButton;
    // Start is called before the first frame update
    void Start()
    {
      UpdateList();
      SetActivePopUp(false);
    }

    // Update is called once per frame
    void Update()
    {
      // if only button pressed update
    }

    void UpdateList()
    {
      string path = "Assets/Datas/Behaviors/";
      string[] files = Directory.GetFiles(path, "*.txt");
      GameObject contentField = GameObject.Find("Content");
      foreach(Transform buttonTransform in contentField.gameObject.transform)
      {
        GameObject.Destroy(buttonTransform.gameObject);
      }
      int i = 0;
      foreach(string file in files)
      {
        Button behaviorButton = Instantiate(buttonPrefab, new Vector3(0,100 - i*30, 0), Quaternion.identity).GetComponent<Button>();
        behaviorButton.gameObject.transform.SetParent(contentField.gameObject.transform, false);
        string removePath = file;
        string behaviorName = Path.GetFileName(file);
        behaviorButton.GetComponentInChildren<Text>().text = behaviorName;
        behaviorButton.onClick.AddListener(() => {
            this.ViewPopUp(behaviorName, removePath);
        });
        i += 1; // index start from 1.
      }
    }
    public void ViewPopUp(string name, string path)
    {
      Debug.Log("popup: " + name + ":" + path);
      SetActivePopUp(true);
      popUp.GetComponentInChildren<Text>().text = name;
      string removePath = path + "";
      removeFileButton.onClick.RemoveAllListeners();
      removeFileButton.onClick.AddListener(() => {
        RemoveBehaviorFile(removePath);
      });
      /*
      popUp.GetComponentInChildren<Button>().onClick.AddListener(() => {
        RemoveBehaviorFile(path);
      });*/
    }
    public void RemoveBehaviorFile(string removePath)
    {
      Debug.Log("Remove file: " + removePath);
      File.Delete(removePath);
      File.Delete(removePath + ".meta");
      UpdateList();
    }
    public void SetActivePopUp(bool value)
    {
      popUp.SetActive(value);
    }
}
