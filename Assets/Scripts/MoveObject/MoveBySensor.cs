using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBySensor : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 initialRotationInEuler;
    void Start()
    {
      var pos = transform.position;
      Vector3 m_accel = Input.acceleration;
      Input.gyro.enabled = true;
      Gyroscope m_gyro = Input.gyro;
      float x = -m_gyro.attitude.x;
      float y = -m_gyro.attitude.y;
      float z = m_gyro.attitude.z;
      float w = m_gyro.attitude.w;
      transform.rotation = Quaternion.Euler(90, 0, 0) * (new Quaternion(x, y, z, w));
      initialRotationInEuler = transform.localEulerAngles;

    }

    // Update is called once per frame
    void Update()
    {
      var pos = transform.position;
      //Vector3 m_accel = Input.acceleration;
      Gyroscope m_gyro = Input.gyro;
      /*
      if(false)
      {
        Debug.Log("Accel: " + m_accel);
        var moveX = m_accel.x * Time.deltaTime * Time.deltaTime;
        var moveY = m_accel.y * Time.deltaTime * Time.deltaTime;
        var moveZ = m_accel.z * Time.deltaTime * Time.deltaTime;
        pos.x += moveX;
        pos.y += moveY;
        pos.z += moveZ;
        transform.position = pos;
      }
      */
      if(m_gyro != null)
      {
        float x = -m_gyro.attitude.x;
        float y = -m_gyro.attitude.y;
        float z = m_gyro.attitude.z;
        float w = m_gyro.attitude.w;
        float toRotY = -initialRotationInEuler.y;
        transform.rotation = Quaternion.Euler(90, toRotY, 0) * (new Quaternion(x, y, z, w));
      }
    }
}
