//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPart : MonoBehaviour
{
    //General
    public enum Type {firstNode, arrow, straight, curve}
    public Type type;
    public List<Mesh> meshes;   
    public Tile AssignedTile { get; private set; }
    public float Rotation { get; private set; }

    public void Init(Type type, Tile tile)
    {
        this.AssignedTile = tile;
        SetGfx(type);
        SetPosition(tile.Position);
    }

    public void SetGfx(Type type)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        this.type = type;
        switch (type)
        {
            case Type.firstNode: meshFilter.mesh = null; break;
            case Type.arrow: meshFilter.mesh = meshes[0]; break;
            case Type.straight: meshFilter.mesh = meshes[1];break;                
            case Type.curve:meshFilter.mesh = meshes[2];break;               
            default: throw new System.Exception("Arrow graphic type not found!");                
        }        
    }

    public void SetPosition(Vector2Int pos)
    {
        this.transform.position = new Vector3(pos.x, Core.Model.Database.arrowPathHeight, pos.y);
    }

    public void SetRotation(float angle)
    {
        Rotation = angle;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }    
}
