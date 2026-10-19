
using UnityEngine;

public class HeatingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null)
        {
            player.isHeating = true;

            Debug.Log("Le joueur entre dans la zone réchauffante !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null)
        {
            player.isHeating = false;

            Debug.Log("Le joueur sort de la zone réchauffante !");
        }
    }
}
