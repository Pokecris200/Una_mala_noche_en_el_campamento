using System.Collections;
using UnityEngine;

public class ObjectTesting : MonoBehaviour
{
    private Material m;
    private Color colorOrigin;
    private void Start()
    {
        m = gameObject.GetComponent<Renderer>().material;
        colorOrigin = m.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        m.color = Color.purple;
        StartCoroutine(RecuperarColor());
    }

    private IEnumerator RecuperarColor()
    {
        yield return new WaitForSeconds(5);
        m.color = colorOrigin;
    }
}
