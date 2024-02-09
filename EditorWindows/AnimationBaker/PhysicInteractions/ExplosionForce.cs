using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    [SerializeField, Tooltip("Radius of the explosion, represented by a red wired sphere")] float explosionRadius = 2f;
    [SerializeField, Tooltip("Strengh of the explosion")] float explosionForce = 300f;

    [Tooltip("Will disable kinematic property on affected rigidbodies")]
    [SerializeField] bool forceDisableKinematic = false;
    
    [Tooltip("To have more control, we specify the list of affected rigidbodies. If not, we use every rigidbodies in the explosion radius.")]
    [SerializeField] bool useList = false;
    [SerializeField] List<Rigidbody> rigidbodies = new List<Rigidbody>();

    [ContextMenu("Simulate Explosion")]
    public void Explode()
    {
        Rigidbody[] rbs;

        if(useList)
        {
            rbs = rigidbodies.ToArray();
        }

        else
        {
            rbs = Physics.OverlapSphere(transform.position, explosionRadius).Select(x => x.GetComponent<Rigidbody>()).ToArray();
        }
        

        for (int i = 0; i < rbs.Length; i++)
        {
            if(forceDisableKinematic)
            {
                rbs[i].isKinematic = false;
            }

            rbs[i].AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
