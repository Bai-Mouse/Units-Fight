

using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveOnHoverByTag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;
    public Vector3 MoveOffset;
    Vector3 NewPosition,OriginalPosition;
    float counter;
    private void Start()
    {
        OriginalPosition=transform.position;
        NewPosition = transform.position + MoveOffset;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OriginalPosition = transform.position;
        NewPosition = transform.position + MoveOffset;
        isHovering = true;
        counter = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        counter = 0;
    }

    void Update()
    {
        if (isHovering)
        {
            if (counter <= 0.2f)
            {
                counter += Time.deltaTime;
                transform.position += (NewPosition - transform.position) / 2;
            }

        }
        else
        {
            if (counter <= 0.2f)
            {
                counter += Time.deltaTime;
                transform.position += (OriginalPosition - transform.position) / 2;
            }

        }
    }
}
