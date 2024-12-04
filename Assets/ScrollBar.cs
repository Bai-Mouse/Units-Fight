using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandler : MonoBehaviour
{
    public Scrollbar scrollbar; 
    public GameManager gameManager; 

    private void Start()
    {
        if (scrollbar != null)
        {

            scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
        }
    }

    private void OnScrollbarValueChanged(float value)
    {
        if (gameManager != null)
        {

            gameManager.sensitivity = Mathf.Lerp(-0.5f, -2f, value);

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
