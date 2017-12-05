using Godot;
using System;

public class troll : KinematicBody2D
{
    // This is a demo showing how KinematicBody2D MoveAndSlide works

    // Member variables
    const int MOTION_SPEED = 160; // pixels / second

    public override void _PhysicsProcess(float delta)
    {
        var motion = new Vector2();

        if (Input.IsActionPressed("move_up"))
            motion += new Vector2(0, -1);
        else if (Input.IsActionPressed("move_bottom"))
            motion += new Vector2(0, 1);
        if (Input.IsActionPressed("move_left"))
            motion += new Vector2(-1, 0);
        else if (Input.IsActionPressed("move_right"))
            motion += new Vector2(1, 0);

        motion = motion.Normalized() * MOTION_SPEED;

        MoveAndSlide(motion);

    }
}
