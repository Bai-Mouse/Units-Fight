using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandler : MonoBehaviour
{
    public Scrollbar scrollbar; 
    public GameManager gameManager;
    public GameObject manu;
    float xposition;
    public enum type{
        sensitive,
        manu,
    }
    public type mytype;
    private void Start()
    {
        if (scrollbar != null)
        {

            scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
        }
        if(manu)
        xposition = manu.transform.position.x;
    }

    private void OnScrollbarValueChanged(float value)
    {
        if(mytype==type.sensitive)
        if (gameManager != null)
        {

            gameManager.sensitivity = Mathf.Lerp(-0.5f, -2f, value);

        }
        if (mytype == type.manu)
        {
            manu.transform.position = new Vector3(Mathf.Lerp(xposition, 269, value), manu.transform.position.y, manu.transform.position.z);
        }

    }

    private void OnDestroy()
    {
        if (scrollbar != null)
        {
            scrollbar.onValueChanged.RemoveListener(OnScrollbarValueChanged);
        }
    }
}
