using Godot;
using System;

public class player : KinematicBody2D
{
    // Member variables 
    const float GRAVITY = 500.0f;
    // Angle in degrees towards either side that the player can consider "floor"
    const float FLOOR_ANGLE_TOLERANCE = 40.0f;
    
    const float WALK_FORCE = 600.0f;
    const float WALK_MIN_SPEED = 10.0f;
    const float WALK_MAX_SPEED = 200.0f;
    const float STOP_FORCE = 1300.0f;
    const float JUMP_SPEED = 200.0f;
    const float JUMP_MAX_AIRBORNE_TIME = 0.2f;

    const float SLIDE_STOP_VELOCITY = 1.0f; // one pixel/second
    const float SLIDE_STOP_MIN_TRAVEL = 1.0f; // one pixel

    Vector2 velocity;
    float onAirTime = 100.0f;
    bool jumping = false;

    bool prevJumpPressed = false;
    public override void _Ready()
    {
        velocity = new Vector2(0, 0); 
        
    }
    public override void _PhysicsProcess(float delta)
    {
        Vector2 force = new Vector2(0, GRAVITY);
        bool walkLeft = Input.IsActionPressed("move_left");
        bool walkRight = Input.IsActionPressed("move_right");
        bool jump = Input.IsActionPressed("jump");
        bool stop = true;

        if (walkLeft)
        {
            if (velocity.x <= WALK_MIN_SPEED && velocity.x > -WALK_MAX_SPEED)
            {
                force.x -= WALK_FORCE;
                stop = false;
            }
        }
        else if (walkRight)
        {
            if (velocity.x >= -WALK_MIN_SPEED && velocity.x < WALK_MAX_SPEED)
            {
                force.x += WALK_FORCE;
                stop = false;
            }
        }

        if (stop)
        {
            int vSign = Math.Sign(velocity.x);
            float vLen = Math.Abs(velocity.x);

            vLen -= STOP_FORCE * delta;
            if (vLen < 0)
                vLen = 0;

            velocity.x = vLen * vSign;
        }
        //Integrate forces to velocity
        velocity += force * delta;
        //Integrate velocity into motion and move
        velocity = MoveAndSlide(velocity, new Vector2(0, -1));

        if (IsOnFloor())
            onAirTime = 0;

        if (jumping && velocity.y > 0) //falling
            jumping = false;

        if (onAirTime < JUMP_MAX_AIRBORNE_TIME && jump && !prevJumpPressed && !jumping)
        {
            // This allows the player to jump if they've barely left the floor
            // Makes controls more snappy.
            velocity.y = -JUMP_SPEED;
            jumping = true;
        }

        onAirTime += delta;
        prevJumpPressed = jump;
        
    }
}
