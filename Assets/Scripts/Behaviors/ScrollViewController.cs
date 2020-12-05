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
    public GameObject audioScrollView;
    public Button removeFileButton;
    public Button setAudioButton;
    public static string selectedFilePath;

    // Start is called before the first frame update
    void Start()
    {
      selectedFilePath = null;
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
      // TODO: check
      //string path = "Assets/Resources/Behaviors/";
      string path = Application.persistentDataPath;
      //string path = RecordBehavior.GetSecureDataPath();
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
        behaviorButton.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(behaviorName);
        behaviorButton.onClick.AddListener(() => {
            this.ViewPopUp(behaviorName, removePath);
        });
        i += 1; // index start from 1.
      }
    }

    public void ViewPopUp(string behaviorName, string path)
    {
      selectedFilePath = path;
      Debug.Log("popup: " + behaviorName + ":" + path);
      SetActivePopUp(true);
      popUp.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(behaviorName);
      string removePath = path + "";
      removeFileButton.onClick.RemoveAllListeners();
      removeFileButton.onClick.AddListener(() => {
        RemoveBehaviorFile(removePath);
      });
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
