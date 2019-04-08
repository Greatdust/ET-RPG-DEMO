using BulletSharp;
using System.ComponentModel;
using UnityEngine;

public class TerrainConfig : ISupportInitialize
{
    public int width;
    public int length;
    public double maxHeight;

    public int upIndex;
    public PhyScalarType scalarType;

    public double scale_x;
    public double scale_y;
    public double scale_z;

    public void BeginInit()
    {
   
    }

    public void EndInit()
    {
    
    }
}


public class BoxShapeConfig : ISupportInitialize
{
    public double postion_x;
    public double postion_y;
    public double postion_z;
    public double extents_x;
    public double extents_y;
    public double extents_z;

    public void BeginInit()
    {
    
    }

    public void EndInit()
    {
       
    }
}

