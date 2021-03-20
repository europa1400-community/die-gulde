using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde.Cam
{
    public class CameraComponent : MonoBehaviour
    {
        [SerializeField] float _scrollRadius;
        [SerializeField] float _scrollSpeed;
        [SerializeField] float _cameraZ;
        Transform Transform { get; set; }
        Camera Camera { get; set; }

        Vector2 MousePosition => Mouse.current.position.ReadValue();
        Vector2 ScreenTopRight => new Vector2(Screen.width, Screen.height);
        Vector2 ScreenBottomLeft => new Vector2(0, 0);

        void Start()
        {
            Transform = GetComponent<Transform>();
            Camera = GetComponent<Camera>();
        }

        void Update()
        {
            var mousePos = MousePosition;

            var scrollDirection = Vector3.zero;

            if (mousePos.y <= ScreenBottomLeft.y + _scrollRadius)
            {
                scrollDirection += Vector3.down;
            }

            if (mousePos.y > ScreenTopRight.y - _scrollRadius)
            {
                scrollDirection += Vector3.up;
            }

            if (mousePos.x <= ScreenBottomLeft.x + _scrollRadius)
            {
                scrollDirection += Vector3.left;
            }

            if (mousePos.x > ScreenTopRight.x - _scrollRadius)
            {
                scrollDirection += Vector3.right;
            }

            scrollDirection = scrollDirection.normalized;

            var position = Transform.position;

            position += scrollDirection * (_scrollSpeed * Time.deltaTime);
            position = new Vector3(position.x, position.y, _cameraZ);

            Transform.position = position;
        }
    }

}