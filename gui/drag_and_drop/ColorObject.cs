using System;
using Godot;

namespace dragdrop
{
    class ColorObject : Godot.Object
    {
        public Color color { get; set; }

        public ColorObject(Color c)
        {
            color = c;
        }

    }
}
