using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomInputField : MonoBehaviour
{
    public GameObject keyboard;
    Toggle ShiftKey;
    // Start is called before the first frame update
    void Start()
    {
        ShiftKey = keyboard.transform.Find("ShiftKey").GetComponent<Toggle>();
        Debug.Log(ShiftKey);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<InputField>().isFocused)
        {
            keyboard.GetComponent<Keyboard>().SelectedInputField = gameObject;
            gameObject.GetComponent<InputField>().text = "";
            ShiftKey.isOn = true;
        }              
    }
}
