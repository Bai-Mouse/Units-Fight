
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveOnHoverByTag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;
    public Vector3 MoveOffset;
    Vector3 NewPosition,OriginalPosition;
    private void Start()
    {
        OriginalPosition=transform.position;
        NewPosition = transform.position + MoveOffset;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void Update()
    {
        if (isHovering)
        {
            transform.position +=( NewPosition - transform.position )/5 ;
        }
        else
        {
            transform.position += (OriginalPosition - transform.position) / 5;
        }
    }
}
