using Godot;
using System;

public class beach_cave : Node2D
{
    const int BUTTON_MASK_LEFT = 1;
    const int CAVE_LIMIT = 1000;
    public override void _Input(InputEvent e)
    {
        if (e.GetType() == typeof(InputEventMouseMotion))
        {
            InputEventMouseMotion mmEvent = e as InputEventMouseMotion;
            if (mmEvent.ButtonMask == BUTTON_MASK_LEFT)
            {
                Sprite cave = GetNode("cave") as Sprite;
                float relX = mmEvent.Relative.x;
                Vector2 cavePos = cave.Position;
                cavePos.x += relX;
                if (cavePos.x < -CAVE_LIMIT)
                {
                    cavePos.x = -CAVE_LIMIT;
                }
                else if (cavePos.x > 0)
                {
                    cavePos.x = 0;
                }
                cave.Position = cavePos;
            }
        }
    }
}
