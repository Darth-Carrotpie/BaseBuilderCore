using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
namespace BaseBuilderCore {
    public struct HexLink : IComponentData {
        public int firstNodeID;
        public int secondNodeID;
        public float width;
        public Color color;
        public Entity lineGraphics;
    }

    public struct HexNode : IComponentData {
        public int id;
        public FixedString64Bytes name;
        public Color color;
    }

    public struct LinkOrder : IComponentData { public bool startLinking; }

    public struct MarkedForLinkStart : IComponentData { }

    public struct MarkedNodeForLinkStart : IComponentData { }
}