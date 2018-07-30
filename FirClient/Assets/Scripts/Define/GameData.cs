using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
}

public class TankData
{
    public string name;
    public string atlas;
    public string[] bases;
    public Vector3 base_pos;
    public string armor;
    public Vector3 armor_pos;
    public string canon;
    public Vector3 canon_pos;
    public Vector3 scale;
}

public class AnimalData
{
    public string name;
    public string atlas;
    public string frame;
    public Vector3 frame_pos;
    public Vector3 frame_size;
    public string body;
    public Vector3 body_pos;
    public Vector3 scale;
}

public class BulletData
{
    public string name;
    public string resource;
    public string animName;
    public Vector3 scale;
    public string sound;
}

public class EffectData
{
    public string name;
    public string resource;
    public string animName;
    public Vector3 scale;
    public string sound;
}

public class NPCData
{
    public long npcId = 0;
}