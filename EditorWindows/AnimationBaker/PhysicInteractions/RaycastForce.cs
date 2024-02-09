using UnityEngine;

public class RaycastForce : MonoBehaviour
{
    [SerializeField] float strengh;
    [SerializeField] ForceMode forceMode;

    [ContextMenu("Raycast Physic Shot")]
    public void Raycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.rigidbody)
            {
                hit.rigidbody.AddForce(-hit.normal * strengh, forceMode);
            }
        }
    }
}
