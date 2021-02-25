using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChipNDale
{
    [CreateAssetMenu]
    public class PlayerConfiguration : ScriptableObject
    {
        public List<PlayerInitData> PlayersData;
        public float ImmortalTime;
    }


    public enum PlayerControlType
    {
        None = 0,
        Keyboard1 = 1,
        Keyboard2 = 2,
        Joystick1 = 101,
        Network1 = 201
    }


    [Serializable]
    public struct PlayerInitData
    {
        public Rigidbody Rigidbody;

        public PlayerControlType ControlsType;
        public string PlayerName;
        public float HorizontalAcceleration;
        public float VerticalAcceleration;
        public int MaxHealthPoint;
        public Vector2 ItemForcePush;
        
        public PlayerView PlayerViewPrefab;
    }


}
