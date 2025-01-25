using UnityEngine;

namespace Haptikos.UI
{

    public class LeverRod : MonoBehaviour
    {
        public GameObject handle;

        private void Update()
        {
            Vector3 dir = handle.transform.position - transform.position;

            Vector3 pos = dir.normalized * dir.magnitude * 40;

            GetComponent<Rigidbody>().velocity = pos;
        }
    }
}