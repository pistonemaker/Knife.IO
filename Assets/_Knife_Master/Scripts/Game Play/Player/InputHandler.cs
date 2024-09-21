using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public Transform centerMove;
    private Player player;
    private float boundTop = 41f;
    private float boundRight = 41f;
    private Vector3 previousMousePosition;
    public GameObject check;

    public CinemachineVirtualCamera virtualCam;
    private float initialOrthoSize;
    private Tween zoomTween;

    private void Start()
    {
        initialOrthoSize = virtualCam.m_Lens.OrthographicSize;
        player = transform.parent.GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUIElement())
        {
            SetPreviousMousePosition();
            MovePlayerOnlyClick();

            CheckPositionInBound();
            player.RotateKnifesMoving();

            StartZoomTween(GameManager.Instance.cameraSizeMoving);
        }

        if (Input.GetMouseButton(0) && !IsMouseOverUIElement())
        {
            MovePlayer();

            CheckPositionInBound();
            player.RotateKnifesMoving();

            StartZoomTween(GameManager.Instance.cameraSizeMoving);
        }
        else
        {
            player.RotateKnifesStanding();
            centerMove.gameObject.SetActive(true);
            Tip.Instance.ShowMoveTip();

            EndZoomTween();
        }
    }

    private void MovePlayerOnlyClick()
    {
        Vector3 moveDirection;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;

        moveDirection = (targetPosition - transform.position).normalized;

        previousMousePosition = targetPosition;

        centerMove.gameObject.SetActive(false);
        Tip.Instance.HideMoveTip();
        transform.parent.position += moveDirection * (player.curSpeed * Time.deltaTime);
    }

    private void MovePlayer()
    {
        bool canMove = false;
        Vector3 moveDirection;
        
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;

        moveDirection = (targetPosition - check.transform.position).normalized;
            
        if (moveDirection.magnitude >= 0.5f)
        {
            canMove = true;
        }

        previousMousePosition = targetPosition;
        
        if (canMove)
        {
            centerMove.gameObject.SetActive(false);
            Tip.Instance.HideMoveTip();
            transform.parent.position += moveDirection * (player.curSpeed * Time.deltaTime);
        }
    }

    private void CheckPositionInBound()
    {
        var position = transform.parent.position;
        if (transform.parent.position.x >= boundRight)
        {
            position = new Vector3(boundRight, position.y, position.z);
            transform.parent.position = position;
        }

        if (transform.parent.position.x <= -boundRight)
        {
            position = new Vector3(-boundRight, position.y, position.z);
            transform.parent.position = position;
        }

        if (transform.parent.position.y >= boundTop)
        {
            position = new Vector3(position.x, boundTop, position.z);
            transform.parent.position = position;
        }

        if (transform.parent.position.y <= -boundTop)
        {
            position = new Vector3(position.x, -boundTop, position.z);
            transform.parent.position = position;
        }
    }

    private void SetPreviousMousePosition()
    {
        previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        previousMousePosition.z = 0f;
        check.transform.position = previousMousePosition;
    }

    private bool IsMouseOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void StartZoomTween(float targetSize)
    {
        if (zoomTween != null && zoomTween.IsActive())
        {
            zoomTween.Kill();
        }

        zoomTween = DOTween.To(() => virtualCam.m_Lens.OrthographicSize,
            x => virtualCam.m_Lens.OrthographicSize = x, targetSize, 1f);
    }

    private void EndZoomTween()
    {
        if (zoomTween != null && zoomTween.IsActive())
        {
            zoomTween.Kill();
        }

        zoomTween = DOTween.To(() => virtualCam.m_Lens.OrthographicSize,
            x => virtualCam.m_Lens.OrthographicSize = x, initialOrthoSize, 1f);
    }
}
