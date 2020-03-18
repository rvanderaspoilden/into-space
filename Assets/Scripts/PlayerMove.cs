using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float jumpSpeed = 2f;
    [SerializeField] private float gravity = 20;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeedX, rotationSpeedY = 5f;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private new Camera camera;

    private float sprintValue = 0f;

    private void Start() {
        if(!this.photonView.IsMine) {
            this.camera.gameObject.SetActive(false);
        } else {
            this.camera.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update() {
        if(!this.photonView.IsMine) {
            return;
        }

        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        Vector3 moveDirection = new Vector3(horizontalAxis, 0, verticalAxis);
        moveDirection *= this.moveSpeed;

        if (this.characterController.isGrounded) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                this.sprintValue += this.sprintValue < 2 ? Time.deltaTime * 5 : 0f;
            } else if (this.sprintValue > 0) {
                this.sprintValue -= Time.deltaTime * 5;
            }

            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }
        } else {
            this.sprintValue = 0f;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        moveDirection = transform.TransformDirection(moveDirection);

        this.characterController.Move(moveDirection * Time.deltaTime);

        this.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * this.rotationSpeedX);

        this.camera.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * this.rotationSpeedY);

        if (this.characterController.isGrounded) {
            this.characterAnimator.SetFloat("Speed", verticalAxis + this.sprintValue);
        }
    }
}
