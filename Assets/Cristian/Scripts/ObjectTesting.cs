using UnityEngine;

public class ObjectTesting : MonoBehaviour
{
    private Material m;
    private void Start()
    {
        m = gameObject.GetComponent<Renderer>().material;
    }
    private void OnTriggerEnter(Collider other)
    {
        m.color = Color.purple;
    }
}
