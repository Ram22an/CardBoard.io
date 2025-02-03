using UnityEngine;

public class TrailGameOver : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            Debug.Log("Check");
        }
        if (other.tag == "Ground")
        {
            //gameObject.SetActive(false);
        }
    }
}
