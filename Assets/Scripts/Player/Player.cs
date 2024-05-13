using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed;
    PhotonView view;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    void Update()
    {
        if (view.IsMine)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            playerRb.AddForce(Vector3.right * horizontalInput * speed * Time.deltaTime);
        }
    }
}
