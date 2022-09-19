using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Vector2 maxFollowOffset = new Vector2(-1f, 6f);
    [SerializeField]
    private Vector2 cameraVelocity = new Vector2(4f, 0.25f);
    [SerializeField]
    private Transform playerTransform = null;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera = null;

    private CinemachineTransposer transposer;

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        virtualCamera.gameObject.SetActive(true);

        enabled = true;
    }
}
