using Godot;
using System;
using System.Collections.Generic;

public class navmesh : Navigation
{
    const float SPEED = 4.0f;
    float camRot = 0.0f;
    Vector3 begin;
    Vector3 end;
    SpatialMaterial m;
    List<Vector3> path;
    bool drawPath = true;

    //Copied directly from input_event.h in Godot source Code
    //Added here because I cannot find the variables in GodotSharp...?
    [Flags]
    enum ButtonList : short
    {
        BUTTON_LEFT = 1,
        BUTTON_RIGHT = 2,
        BUTTON_MIDDLE = 3,
        BUTTON_WHEEL_UP = 4,
        BUTTON_WHEEL_DOWN = 5,
        BUTTON_WHEEL_LEFT = 6,
        BUTTON_WHEEL_RIGHT = 7,
        BUTTON_MASK_LEFT = (1 << (BUTTON_LEFT - 1)),
        BUTTON_MASK_RIGHT = (1 << (BUTTON_RIGHT - 1)),
        BUTTON_MASK_MIDDLE = (1 << (BUTTON_MIDDLE - 1))
    };

    public override void _Ready()
    {
        SetProcessInput(true);
        m = new SpatialMaterial();
        path = new List<Vector3>();
        m.FlagsUnshaded = true;
        m.FlagsUsePointSize = true;
        m.AlbedoColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public override void _Process(float delta)
    {

        if (path.Count > 1)
        {
            float toWalk = delta * SPEED;
            Vector3 toWatch = new Vector3(0.0f, 1.0f, 0.0f);
            while (toWalk > 0.0f && path.Count >= 2)
            {
                Vector3 pFrom = path[path.Count - 1];
                Vector3 pTo = path[path.Count - 2];
                toWatch = (pTo - pFrom).Normalized();
                float d = pFrom.DistanceTo(pTo);
                if (d <= toWalk)
                {
                    path.RemoveAt(path.Count - 1);
                    toWalk -= d;
                }
                else
                {
                    path[path.Count - 1] = pFrom.LinearInterpolate(pTo, toWalk / d);
                    toWalk = 0.0f;
                }
            }
            Vector3 atPos = path[path.Count - 1];
            Vector3 atDir = toWatch;
            atDir.y = 0;
            Transform tr = new Transform();
            tr.origin = atPos;
            tr = tr.LookingAt(atPos + atDir, new Vector3(0, 1.0f, 0));
            Position3D robot = GetNode("robot_base") as Position3D; //Downcast to the correct type
            robot.SetTransform(tr);
            if (path.Count < 2)
            {
                path.Clear();
                SetProcess(false);
            }
        }
        else SetProcess(false);
    }

    private void UpdatePath()
    {
        path = new List<Vector3>(GetSimplePath(begin, end, true)); //Convert the array to a list for easier access
        path.Reverse();
        SetProcess(true);
        if (drawPath)
        {
            ImmediateGeometry im = GetNode("draw") as ImmediateGeometry;
            im.SetMaterialOverride(m);
            im.Clear();
            im.Begin(Mesh.PRIMITIVE_POINTS, null);
            im.AddVertex(begin);
            im.AddVertex(end);
            im.End();
            im.Begin(Mesh.PRIMITIVE_LINE_STRIP, null);
            for(int i = 0; i < path.Count; i++)
            {
                im.AddVertex(path[i]);
            }
            im.End();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e.GetType() == typeof(InputEventMouseButton))
        {
            InputEventMouseButton mbEvent = e as InputEventMouseButton;
            if(mbEvent.ButtonIndex == (int)ButtonList.BUTTON_LEFT  && mbEvent.Pressed) 
            {
                Camera cam = GetNode("cambase/Camera") as Camera;
                Position3D robot = GetNode("robot_base") as Position3D;
                Vector3 rayFrom = cam.ProjectRayOrigin(mbEvent.Position);
                Vector3 rayTo = rayFrom + cam.ProjectRayNormal(mbEvent.Position) * 100;
                Vector3 p = GetClosestPointToSegment(rayFrom, rayTo);
                begin = GetClosestPoint(robot.Translation);
                end = p;
                UpdatePath();
            }
        }
        
        else if (e.GetType() == typeof(InputEventMouseMotion))
        {
            InputEventMouseMotion mmEvent = e as InputEventMouseMotion;
            if (mmEvent.ButtonMask == (int)ButtonList.BUTTON_MASK_RIGHT ||
                mmEvent.ButtonMask == (int)ButtonList.BUTTON_MASK_MIDDLE)
            {
                Spatial camBase = GetNode("cambase") as Spatial;
                camRot -= mmEvent.Relative.x * 0.005f;
                camBase.SetRotation(new Vector3(0, camRot, 0));
            }
        }
        base._Input(e);
    }
}
/*
func _input(event):
#	if (event extends InputEventMouseButton and event.button_index == BUTTON_LEFT and event.pressed):
	if (event.is_class("InputEventMouseButton") and event.button_index == BUTTON_LEFT and event.pressed):
		var from = get_node("cambase/Camera").project_ray_origin(event.position)
		var to = from + get_node("cambase/Camera").project_ray_normal(event.position)*100
		var p = get_closest_point_to_segment(from, to)
		
		begin = get_closest_point(get_node("robot_base").get_translation())
		end = p

		_update_path()
	
	if (event.is_class("InputEventMouseMotion")):
		if (event.button_mask&(BUTTON_MASK_MIDDLE+BUTTON_MASK_RIGHT)):
			camrot += event.relative.x*0.005
			get_node("cambase").set_rotation(Vector3(0, camrot, 0))
			print("camrot ", camrot)


func _ready():
	set_process_input(true)

	m.flags_unshaded = true
	m.flags_use_point_size = true
	m.albedo_color = Color(1.0, 1.0, 1.0, 1.0)
*/