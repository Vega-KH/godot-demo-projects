using Godot;
using System;

public class colworld : Node2D
{
    private void _on_princess_body_entered(Godot.Object body)
    {
        if (body.Equals(GetNode("player")))
            {
                var winLabel = GetNode("youwin") as Label; 
                winLabel.Show();
            }
    }
}


