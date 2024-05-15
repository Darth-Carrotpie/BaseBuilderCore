using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEditor.PlayerSettings;
using Unity.Collections;
using static Unity.Physics.CompoundCollider;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public partial class GenerateGridSystem : SystemBase
{
    //GridGeneratorConfig config;
    //EntityManager entityManager;

    protected override void OnCreate()
    {
        RequireForUpdate<GridGeneratorConfig>();
    }
    protected override void OnUpdate()
    {
        //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();

        GenerateGrid(config);

        this.Enabled = false;
    }

    public void GenerateGrid(GridGeneratorConfig config)
    {
        float3 pos = float3.zero;

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

                        CreateCellEntity(config, pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                        //SetEntityParent(tile);
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

                        CreateCellEntity(config, pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                        //SetEntityParent(tile);

                    }
                }
                break;
        }
    }

    public void CreateCellEntity(GridGeneratorConfig config, float3 position, string newName)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity newEntity = entityManager.Instantiate(config.cellPrefabEntity); //here Bake runs only once for the prefab when it itself is instantiated, but not every time when that prefab's new instance is spawned

        //this does not work:
        //get the entity of config to parent the cells to
        //EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(GridGeneratorConfig));
        //Entity configEntity = entityQuery.ToEntityArray(Allocator.TempJob)[0];
        //Entity configEntity = entityQuery.GetSingletonEntity();
        //entityManager.AddComponent<Parent>(newEntity);
        //entityManager.SetComponentData(newEntity, new Parent { Value = configEntity });

        //string childName = entityManager.GetName(newEntity);
        //string parentName = entityManager.GetName(configEntity);
        //Debug.Log("child: " + childName + "     parentName: " + parentName);

        Quaternion rot = Quaternion.identity;
        if(config.hexOrientation == HexOrientation.Flat)
        {
            rot = Quaternion.Euler(new Vector3(0f, 30f, 0f));
        }

        entityManager.SetComponentData(newEntity, new LocalTransform
        {
            Position = position,
            Rotation = rot,
            Scale = 1f
        });

        entityManager.SetName(newEntity, newName);
    }
    /*public void SetEntityParent(Entity child)
    {
        //get the entity of config to parent the cells to
        var entityQuery = entityManager.CreateEntityQuery(typeof(GridGeneratorConfig));
        //EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GridGeneratorConfig>());
        Entity configEntity = entityQuery.GetSingletonEntity();
        //set it up for the newly created Entity
        //entityManager.AddComponent<Parent>(entity);
        entityManager.AddComponentData(child, new Parent { Value = configEntity });
        string childName = entityManager.GetName(child);
        string parentName = entityManager.GetName(configEntity);
        Debug.Log("child: " + childName + "     parentName: " + parentName);
    }*/
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
