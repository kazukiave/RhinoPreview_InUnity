using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhino;
using Rhino.Geometry;
using System.Linq;
using RhinoInside.Unity;

public static class RhinoPreview 
{
    public static GameObject PolyLineShow(Rhino.Geometry.Polyline polyLine, Color color, float width, string name = "PolyLine")
    {
        var polyLineObj = new GameObject(name);
        var lineRender = polyLineObj.AddComponent<LineRenderer>();
        var vtxs = polyLine.ToList();
        lineRender.positionCount = vtxs.Count;

        for (int i = 0; i < vtxs.Count; i++)
        {
            lineRender.SetPosition(i, new Vector3((float)vtxs[i].X, (float)vtxs[i].Y, (float)vtxs[i].Z));
        }

        lineRender.startColor = color;
        lineRender.endColor = color;
        lineRender.startWidth = width;
        lineRender.endWidth = width;
        lineRender.receiveShadows = false;
        lineRender.material = new Material(Shader.Find("UI/Default"));

        return polyLineObj;
    }

    public static GameObject GridShow(List<Rhino.Geometry.Point3d> grid, int gridSize, Color color, string name = "Grid")
    {
        var gridObj = new GameObject("Grid");

        var breps = new List<Brep>();
        foreach (var pt in grid)
        {
            
            var plane = new Rhino.Geometry.Plane(pt, Vector3d.ZAxis);
            var interval = new Interval(-gridSize / 2, gridSize / 2);
            
            var srf = new Rhino.Geometry.PlaneSurface(plane, interval, interval);
            var brep = srf.ToBrep();
            breps.Add(brep);
        }
        var joinedBrep = Rhino.Geometry.Brep.CreateBooleanUnion(breps, 0.1);

        var meshParam = MeshingParameters.FastRenderMesh;
        var meshs = Rhino.Geometry.Mesh.CreateFromBrep(joinedBrep[0], meshParam);

        var joinedMesh = new Rhino.Geometry.Mesh();
        foreach (var m in meshs)
        {
            joinedMesh.Append(m);
        }
        joinedMesh.Weld(180);


        //attatch Mesh
        var UnityMesh = joinedMesh.ToHost();

        var meshRender = gridObj.AddComponent<MeshRenderer>();
        meshRender.material.color = color;
        var meshFilter = gridObj.AddComponent<MeshFilter>();
        meshFilter.mesh = UnityMesh;

        return gridObj;
    }

}
