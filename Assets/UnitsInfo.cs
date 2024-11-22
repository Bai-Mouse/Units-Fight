using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitsInfo : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI Text,Health,Damage;
    public CharacterData temp;
    public float money;
    public void setInfo(CharacterData u,float m)
    {
        temp=u;

        Icon.sprite = u.icon;
        Text.text = "REQUIRED MONEY: " + u.cost.ToString();
        Health.text = "HEALTH: " + u.health.ToString()+ "\nUpgrade:" + (1 + u.HPUpgradeCount).ToString();
        Damage.text = "DAMAGE: " + Mathf.Abs(u.damage).ToString() + "\nUpgrade:" + (10 + u.DamUpgradeCount * 5).ToString();
        Health.color = m < 1 + u.HPUpgradeCount? Color.red : Color.green;
        Damage.color = m < 10 + u.DamUpgradeCount * 5? Color.red : Color.green;
    }
    public void setColor(Color color, float m)
    {
        Text.color = color;
        Health.color = m < 1 + temp.HPUpgradeCount ? Color.red : Color.green;
        Damage.color = m < 10 + temp.DamUpgradeCount * 5 ? Color.red : Color.green;
    }
    
}
