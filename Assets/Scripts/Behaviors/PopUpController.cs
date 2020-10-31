using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetActivePopUp(bool value)
    {
      GameObject popUp = GameObject.Find("PopUp").GetComponent<GameObject>();
      popUp.SetActive(value);
    }

}
