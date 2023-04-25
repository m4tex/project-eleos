using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Player;
using _Scripts.UI;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minimumLookDirection;

    private MeshCollider[] _obstacles;

    private void Start()
    {
        _obstacles = GetComponentsInChildren<MeshCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 cameraDir = PlayerCamera.Main.transform.forward;
        cameraDir.y = 0; //Looking up or down shouldn't affect the similarity
        
        float lookDirectionSimilarity = Vector3.Dot(cameraDir, transform.forward);
        
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space) && lookDirectionSimilarity > 0 && lookDirectionSimilarity >= minimumLookDirection)
        {
            Debug.Log(Vector3.Dot(PlayerCamera.Main.transform.forward,
                transform.forward));
            Vault();
        }
    }

    private void Vault()
    {
        // UIManager.ControllsLock = true;

        StartCoroutine(vault());
    }

    private IEnumerator vault()
    {
        foreach (MeshCollider obstacle in _obstacles)
            obstacle.enabled = false;

        PlayerMovement.Main.Vault();

        yield return new WaitForSeconds(PlayerMovement.Main.vaultDuration);
        
        foreach (MeshCollider obstacle in _obstacles)
            obstacle.enabled = true;
    }
}
