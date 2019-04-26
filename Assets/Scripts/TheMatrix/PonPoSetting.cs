using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace PresentSetting
    {
        //用于存放各种参数
        [CreateAssetMenu(fileName = "PonPoSetting", menuName = "系统配置文件/PonPoSetting")]
        public class PonPoSetting : ScriptableObject
        {
            [Header("主角移动参数")]
            public float groundForce = 20;  //constant force when moving on ground
            public float airForce = 2;      //constant force when moving on air
            public float groundDrag = 2; 
            public float jumpPower = 6;

            [Header("枪参数")]
            public float gunForceHorizontal = 6;
            public float gunForceVertical = 6;
            public float reloadTime = 2.0f;
            public float ammoTimeDelay = 0.2f;
            public float ammoTimeFactor = 0.3f;
            public float ammoTimeSeconds = 3f;
            public float cannonAngle = 30;
            public float cannonDistance = 3f;

            [Header("主角交互参数")]
            public float reactPower = 1.5f; //slightly jump up when hurt/shoot
            public float damageForceHorizontal = 6;
            public float damageForceVertical = 6;
            public float dieDelayTime = 0.5f;

            [Header("摄像机参数")]
            public float movingRate = 0.9f;
            public float reactDistance = 0.6f;

            [Header("敌人通用参数")]
            public float enemyReactPower = 1.5f; //slightly jump up when hurt
            public float enemyHitPowerAlive = 3f;
            public float enemyHitPowerDead = 8f;

        }
    }
}