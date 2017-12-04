using Godot;
using System;


namespace dragdrop
{
    public class drag_drop_script : ColorPickerButton
    {
        //I think it may be an error in the API that GetDragData returns a Godot.Object,
        //but CanDropData and DropData take a System.Object as a parameter.
        //I have tried to fix this with a hack (creating a ColorObject that inherits from Godot.Object)
        ColorObject c;
        public override void _Ready()
        {
            c = new ColorObject(new Color(1.0f, 1.0f, 1.0f));
        }

        public override Godot.Object GetDragData(Vector2 position)
        {
            ColorPickerButton cpb = new ColorPickerButton();
            cpb.Color = c.color;
            cpb.RectSize = new Vector2(50.0f, 50.0f);
            SetDragPreview(cpb);
            return c;
        }

        public override bool CanDropData(Vector2 position, object data)
        {
            return data.GetType() == typeof(ColorObject);
            //The hack isn't working however, because even if I just hardcode a return value of true,
            //The UI will still not allow drops.

        }

        public override void DropData(Vector2 position, object data)
        {
            c = (ColorObject)data;
        }

    }
}