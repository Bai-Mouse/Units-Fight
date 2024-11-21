using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitsInfo : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI Text;
    public void setInfo(Sprite icon, string text)
    {
        Icon.sprite = icon;
        Text.text = text;

    }
    public void setColor(Color color)
    {
        Text.color = color;
    }
}
