using UnityEngine;

public class RaycastForce : MonoBehaviour
{
    [SerializeField] float strengh;
    [SerializeField] ForceMode forceMode;

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
