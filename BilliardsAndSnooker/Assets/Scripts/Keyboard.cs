using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public List<GameObject> AlphaKeys = new List<GameObject>();
    public GameObject SelectedInputField;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressShift(Toggle toggle)
    {
        foreach (GameObject key in AlphaKeys)
        {
            string text = key.GetComponentInChildren<Text>().text;
            if (toggle.isOn)
            {
                key.GetComponentInChildren<Text>().text = text.ToUpper();
            }
            else
            {
                key.GetComponentInChildren<Text>().text = text.ToLower();
            }
        }
    }

    public void PressBackSpace()
    {
        if (SelectedInputField)
        {
            string text = SelectedInputField.GetComponent<InputField>().text.ToString();

            if (text.Length > 0)
            {
                text = text.Remove(text.Length - 1);
                SelectedInputField.GetComponent<InputField>().SetTextWithoutNotify(text);
            }
        }
    }

    public void PressKey(GameObject key)
    {
        if (SelectedInputField)
        {
            string text = SelectedInputField.GetComponent<InputField>().text.ToString();

            string keyString = key.GetComponentInChildren<Text>().text;
            SelectedInputField.GetComponent<InputField>().SetTextWithoutNotify(text + keyString);
            gameObject.transform.Find("ShiftKey").GetComponent<Toggle>().isOn = false;
        }
    }
}
