using Fusion;
using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : NetworkCharacterController
{
    public event Action<float> OnMovement;

    public override void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        if (Grounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += gravity * deltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);

            //Multiplicar por 180 si el direction.x es menor a 0
            //Multiplicar por 0 si el direction.x es mayor a 0

            //If ternario
            transform.eulerAngles = Vector3.up * (direction.x < 0 ? 180 : 0);
        }

        moveVelocity.x = horizontalVel.x;

        _controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Grounded = _controller.isGrounded;

        OnMovement?.Invoke(Velocity.x);
    }
}
