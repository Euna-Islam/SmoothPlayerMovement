using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * class PlayerSmoothMovement
 * Detects keyboard/touch input
 * Moves player left/right smoothly
 */
public class PlayerSmoothMovement: MonoBehaviour {
  Vector3 PlayerTargetPosition = Vector3.zero; //next target position for player

  public float PlayerMovementRange; //maximum range for player movement, set from Unity

  Vector3 RefVelocityP = Vector3.zero; //reference velocity for smooth transition

  public float PlayerMovementSmoothness; //smoothness factors, set from Unity

  public float PlayerMovementSpeed; //movement speed, set from Unity

  //variables for measuring swipe distance
  private Vector2 FingerDownPos;
  private Vector2 FingerUpPos;

  public float SwipeThreshold = 50 f; //swipe distance has to be greater than this
  public float SpeedFactor = 35 f;

  /*
   * Update is called once per frame
   */
  void Update() {
    MovePlayerInLane();
  }

  /*
   * checks if the swipe distance is greater than swipe threshold
   * if yes, determines the direction of movement 
   */
  private void CheckSwipe() {
    float deltaX = this.FingerDownPos.x - this.FingerUpPos.x; //find swipe distance
    PlayerMovementSpeed = Mathf.Abs(deltaX) / SpeedFactor; //player movement speed is directly proportional to swipe speed

    //Player only moves if swipe distance is greater than swipe threshold
    if (Mathf.Abs(deltaX) > this.SwipeThreshold) {
      if (deltaX > 0) {
        GoRight();
      } else if (deltaX < 0) {
        GoLeft();
      }
    }

    this.FingerUpPos = this.FingerDownPos; //reset variables for measuring swipe distance
  }

  /*
   * detects touch/keyboard input
   * moves player smoothly to target position
   */
  void MovePlayerInLane() {
    if (SystemInfo.deviceType == DeviceType.Handheld) //detect device type
    {
      if (Input.touchCount > 0) {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase) {
        case TouchPhase.Began:
          this.FingerDownPos = touch.position;
          this.FingerUpPos = touch.position;
          break;
        case TouchPhase.Moved:
          this.FingerDownPos = touch.position;
          this.FingerUpPosTime = DateTime.Now;
          this.CheckSwipe();
          break;
        case TouchPhase.Stationary:
          this.FingerDownPos = touch.position;
          this.FingerUpPos = touch.position;
          break;
        case TouchPhase.Ended:
          this.FingerDownPos = touch.position;
          this.FingerUpPosTime = DateTime.Now;
          this.CheckSwipe();
          break;
        default:
          break;
        }
      }
    } else if (SystemInfo.deviceType == DeviceType.Desktop) {
      if (Input.GetKey(KeyCode.LeftArrow)) {
        GoLeft();
      }
      if (Input.GetKey(KeyCode.RightArrow)) {
        GoRight();
      }

    }
    //move player smoothly from current position to target position
    PlayerTargetPosition.z = transform.position.z;
    PlayerTargetPosition.y = transform.position.y;
    gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, PlayerTargetPosition, ref RefVelocityP, Time.deltaTime * PlayerMovementSmoothness);
  }
  /*
   * Finds next player position on left
   */
  void GoLeft() {
    if (transform.position.x <= -PlayerMovementRange)
      PlayerTargetPosition.x = transform.position.x;
    else PlayerTargetPosition.x -= PlayerMovementSpeed;
  }

  /*
   * Finds next player position on right
   */
  void GoRight() {
    if (transform.position.x >= PlayerMovementRange)
      PlayerTargetPosition.x = transform.position.x;
    else PlayerTargetPosition.x += PlayerMovementSpeed;
  }
}