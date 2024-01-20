using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEditor.PlayerSettings;

public partial class GenerateGridSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<GridGeneratorConfig>();
    }
    protected override void OnUpdate()
    {
        this.Enabled = false;

        GridGeneratorConfig gridGeneratorConfig = SystemAPI.GetSingleton<GridGeneratorConfig>();

        GenerateGrid(gridGeneratorConfig);

    }

    public void GenerateGrid(GridGeneratorConfig config)
    {
        float3 pos = float3.zero;
        Entity tile;

        switch (config.hexOrientation)
        {
            case HexOrientation.Flat:
                for (int q = 0; q < config.gridSizeX; q++)
                {
                    int qOff = q >> 1;
                    for (int r = -qOff; r < config.gridSizeZ - qOff; r++)
                    {
                        pos.x = config.hexRadius * 3.0f / 2.0f * (q - config.gridSizeX/2f);
                        pos.z = config.hexRadius * Mathf.Sqrt(3.0f) * ((r + q / 2.0f) - config.gridSizeZ/2f);

                        tile = CreateCellEntity(config, pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                        //tile.index = new CubeIndex(q, r, -q - r);
                        //grid.Add(tile.index.ToString(), tile);
                    }
                }
                break;

            case HexOrientation.Pointy:
                for (int r = 0; r < config.gridSizeZ; r++)
                {
                    int rOff = r >> 1;
                    for (int q = -rOff; q < config.gridSizeX - rOff; q++)
                    {
                        pos.x = config.hexRadius * Mathf.Sqrt(3.0f) * ((q + r / 2.0f) - config.gridSizeX / 2f);
                        pos.z = config.hexRadius * 3.0f / 2.0f * (r - config.gridSizeZ / 2f);

                        tile = CreateCellEntity(config, pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                        //tile.index = new CubeIndex(q, r, -q - r);
                        //grid.Add(tile.index.ToString(), tile);
                    }
                }
                break;
        }
    }

    public Entity CreateCellEntity(GridGeneratorConfig config, float3 position, string newName)
    {
        Entity entity = EntityManager.Instantiate(config.cellPrefabEntity);
        //here Bake runs only once for the prefab when it itself is instantiated, but not every time when that prefab's new instance is spawned
        Quaternion rot = Quaternion.identity;
        if(config.hexOrientation == HexOrientation.Flat)
        {
            rot = Quaternion.Euler(new Vector3(0f, 30f, 0f));
        }
        EntityManager.SetComponentData(entity, new LocalTransform
        {
            Position = position,
            Rotation = rot,
            Scale = 1f
        });
        EntityManager.SetName(entity, newName);
        return entity;
    }

    [System.Serializable]
    public struct CubeIndex
    {
        public int x;
        public int y;
        public int z;

        public CubeIndex(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public CubeIndex(int x, int z)
        {
            this.x = x; this.z = z; this.y = -x - z;
        }

        public static CubeIndex operator +(CubeIndex one, CubeIndex two)
        {
            return new CubeIndex(one.x + two.x, one.y + two.y, one.z + two.z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            CubeIndex o = (CubeIndex)obj;
            if ((System.Object)o == null)
                return false;
            return ((x == o.x) && (y == o.y) && (z == o.z));
        }

        public override int GetHashCode()
        {
            return (x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
        }

        public override string ToString()
        {
            return string.Format("[" + x + "," + y + "," + z + "]");
        }
    }
}
