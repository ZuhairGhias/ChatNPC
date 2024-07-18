using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Interactor")]
    [SerializeField]
    [Range(0.5f, 2f)]
    public float InteractionDistance = 1.0f;

    [SerializeField]
    public LayerMask interactableLayer;
    // Start is called before the first frame update
    void Start()
    {
        DebugUtils.HandleEmptyLayerMask(interactableLayer, this);
    }

    public void Preview(bool facingRight)
    {
        RaycastHit2D hit = GetHit(facingRight);
    }

    public IInteractable GetInteractable(bool facingRight)
    {
        RaycastHit2D hit = GetHit(facingRight);
        return hit.collider.GetComponent<IInteractable>();
    }

    private RaycastHit2D GetHit(bool facingRight)
    {
        Vector2 pos2d = transform.position.ToVector2();
        Vector2 dir2d = facingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(pos2d, dir2d, InteractionDistance, interactableLayer);

        Debug.DrawLine(pos2d, pos2d + dir2d, hit.collider == null ? Color.red : Color.green);

        return hit;
    }
}
