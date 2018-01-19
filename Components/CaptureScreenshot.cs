using UnityEngine;
using System.Collections;

public class CaptureScreenshot : MonoBehaviour {

    public KeyCode keyToPress = KeyCode.Space;
    public int resolutionModifier = 1;
    public string prefix = "ss";
    int id;

    void Start () {
        if (!System.IO.Directory.Exists(Application.dataPath + "/../Screenshots")) {
            System.IO.Directory.CreateDirectory (Application.dataPath + "/../Screenshots");
        }
    }

    void Update () {
        if (Input.GetKeyUp(keyToPress)) {
            string dateTime = System.DateTime.Now.Month.ToString()+ "-" + 
                System.DateTime.Now.Day.ToString() + "_" + 
                    System.DateTime.Now.Hour.ToString() + "-" + 
                    System.DateTime.Now.Minute.ToString() + "-" + 
                    System.DateTime.Now.Second.ToString();
            string filename = prefix + id.ToString() + "_" + dateTime + ".png";
            Debug.Log(filename);
            ScreenCapture.CaptureScreenshot((Application.dataPath + "/../Screenshots/" + filename), resolutionModifier);
            id++;
        }
    }
}
