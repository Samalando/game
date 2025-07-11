using MalbersAnimations.Utilities;
using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

//FROM MOTH

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/ledgegrab")]
    public class LedgeGrab : State
    {
        public override string StateName => "Ledge Grab";

        /// <summary>Air Resistance while falling</summary>
        [Header("Ledge Parameters"), Space]
        [Tooltip("Layer to identify climbable surfaces")]
        public LayerReference LedgeLayer = new LayerReference(1);

        [Tooltip("Climb the Ledge automatically when is near a climbable surface")]
        public BoolReference automatic = new BoolReference();

        [Tooltip("Set the Animal Rigidbody to Kinematic while is on this state. This avoid the colliders to interfiere with ledge.")]
        public BoolReference Kinematic = new BoolReference(true);

        [Tooltip("Correct Distance from the wall to the character")]
        [Min(0)] public float wallDistance = 0.5f;

        //[Tooltip("Correct Vertical Distance from the wall to the character")]
        // public float VerticalOffset = -1;

        [Tooltip("Min Angle needed to enable the Ledge Grab State")]
        public float MinTerrainAngle = 45f;

        [Tooltip("Distance required to check a wall in front of the character")]
        [Min(0)] public float ForwardLength = 1f;


        //[Tooltip("Length of the Ledge Ray when pointing Down")]
        //[Min(0)] public float DownLength = 1f;

        [Tooltip("Correct Distance from the wall to the character")]
        [Min(0)] public float WallChecker = 0.1f;


        [Tooltip("Smoothness value to align the animal to the wall")]
        [Min(0)] public float AlignSmoothness = 10f;
        //[Tooltip("Time to align the animal to the wall")]
        //public float AlignTime = 0.2f;

        public List<LedgeProfiles> profiles = new List<LedgeProfiles>();

        /// <summary>Aligmnet offset found from the character to the ledge</summary>
        private Vector3 AlignmentOffset;
        private float  AngleDifference;

        private Vector3 StartPosition;
        private Quaternion StartRotation;
        private Vector3 TargetPosition;
        private Vector3 WallNormal;


        /// <summary> Store the Current Ledge Profile </summary>
        private LedgeProfiles LedgeProfile;
        private RaycastHit FoundLedgeHit;
        private RaycastHit FoundWallHit;

       

        public override bool TryActivate()
        {
            if (automatic || InputValue) return FindLedge();
            return false;
        }


        private bool OrientToWall = false;
        public bool FindLedge()
        {
            foreach (var p in profiles)
            {
                //Check if we are in Vertical Speed Range
                if (p.MaxVSpeed == 0 || p.MaxVSpeed <= animal.VerticalSmooth)
                {
                    var LedgeForwardPoint1 = transform.TransformPoint(new Vector3(0, p.Height, 0));
                    var WallPoint1 = animal.transform.TransformPoint(new Vector3(0, p.Height - p.LedgeExitDistance - WallChecker, 0));

                    var ForwardDistance = ForwardLength * ScaleFactor * p.ForwardMultiplier;
                    var LedgeExitDistance = p.LedgeExitDistance * ScaleFactor;
                    var LedgeDownPoint1 = LedgeForwardPoint1 + (Forward * ForwardDistance);


                    if (animal.debugGizmos)
                    {
                        Debug.DrawRay(LedgeForwardPoint1, (Forward * ForwardDistance), Color.green);
                        Debug.DrawRay(WallPoint1, (Forward * ForwardDistance), Color.yellow);
                        Debug.DrawRay(LedgeDownPoint1, -Up * LedgeExitDistance, Color.red);
                    }

                    var seg = 3f;

                    //Cast the first Ray--- to see if there nothing in front of the character
                    //No walls poiting forward 
                    if (Physics.Raycast(LedgeForwardPoint1, Forward, out _, ForwardDistance, LedgeLayer.Value, IgnoreTrigger) == false) 
                    {
                        //Check Ledge Pointing Down the Second First Ray
                        if (Physics.Raycast(LedgeDownPoint1, -Up, out FoundLedgeHit, LedgeExitDistance, LedgeLayer.Value, IgnoreTrigger))
                        {
                            Debug.DrawRay(FoundLedgeHit.point, FoundLedgeHit.normal, Color.cyan, seg);

                            var LedgeAngle = Vector3.Angle(FoundLedgeHit.normal, Up);

                            //Do not Grab ledge on a Slope Angle
                            if (LedgeAngle < animal.maxAngleSlope)
                            //We need to not find wall 
                            {
                                if (Physics.Raycast(WallPoint1, Forward, out FoundWallHit, ForwardDistance, LedgeLayer.Value, IgnoreTrigger))
                                {
                                    //Debug.DrawRay(FoundWallHit.point, FoundWallHit.normal * 2, Color.white, 2);

                                    var WallAngle = Vector3.Angle(FoundWallHit.normal, Up);

                                    if (Mathf.Abs(WallAngle - LedgeAngle) < MinTerrainAngle) return false; //Hack to avoid grabbing Weird Ledge Angles

                                  //  Debug.Log("ForwardAngle = " + Vector3.Angle(FoundWallHit.normal, Up));

                                    var CrossLedgeHit = Vector3.Cross(-FoundLedgeHit.normal, FoundWallHit.normal);
                                    WallNormal = Vector3.Cross(FoundLedgeHit.normal, CrossLedgeHit).normalized;

                                    //Find the Correct Orientation
                                    var Y_Point = MTools.ClosestPointOnPlane(transform.position, WallNormal, FoundLedgeHit.point);

                                    MTools.DrawWireSphere(Y_Point,Color.green,0.02f,seg);

                                    Y_Point = MTools.ClosestPointOnLine(transform.position + (UpVector * 5), transform.position - (UpVector * 5), Y_Point);


                                    MTools.DrawWireSphere(Y_Point,Color.yellow,0.02f,seg);

                                    var CloseEdgePoint = FoundWallHit.collider.ClosestPoint(Y_Point);

                                    var H_Point = MTools.ClosestPointOnPlane(FoundWallHit.point, WallNormal, transform.position);

                                    animal.SetPlatform(FoundLedgeHit.transform);
                                    LedgeProfile = p; //Store the current Ledge Profile
                                   

                                    var YAxis = Vector3.Distance(Y_Point, transform.position) + (LedgeProfile.AlingOffset.y * ScaleFactor);

                                    var ZAxis = (wallDistance * ScaleFactor) - Vector3.Distance(H_Point, transform.position)
                                        + (LedgeProfile.AlingOffset.x * ScaleFactor);


                                    var UPDifference = YAxis * FoundLedgeHit.normal;
                                    var HorizontalDifference = ZAxis * WallNormal;

                                    AlignmentOffset = UPDifference + (HorizontalDifference);
                                    AngleDifference = Vector3.SignedAngle(Forward, -WallNormal, Up); //?????

                                    animal.InertiaPositionSpeed = Vector3.zero; //Remove internia
                                    animal.AdditivePosition = Vector3.zero; //Remove additive
                                    CheckKinematic();

                                    StartPosition = animal.transform.position;
                                    StartRotation = animal.transform.rotation;

                                    TargetPosition = animal.transform.position + (AlignmentOffset);

                                    #region Debug
                                   
                                    Debug.DrawRay(FoundLedgeHit.point, CrossLedgeHit, Color.white, seg);
                                    Debug.DrawRay(FoundLedgeHit.point, WallNormal*5, Color.green, seg);

                                    MTools.DrawWireSphere(CloseEdgePoint, Color.blue, 0.1f, seg);

                                    MTools.DrawWireSphere(Y_Point, Color.red, 0.1f, seg);
                                    MTools.DrawWireSphere(FoundLedgeHit.point, Color.yellow, 0.1f, seg);


                                    MTools.DrawWireSphere(H_Point, Color.red, 0.1f, seg);
                                    MTools.DrawWireSphere(transform.position, Color.yellow, 0.1f, seg);

                                    Debug.DrawLine(Y_Point, FoundLedgeHit.point, Color.yellow, seg);
                                    Debug.DrawLine(H_Point, transform.position, Color.red, seg);
                                    MTools.DrawWireSphere(StartPosition, Color.white, 0.02f, seg);

                                    MTools.DrawWireSphere(TargetPosition, Color.green, 0.02f, seg);
                                    #endregion

                                  //  WallNormal = FoundWallHit.normal;
                                    OrientToWall = p.Orient;

                                    Debugging($"Try [Ledge-Grab] Wall and Ledge found. <B>[{p.name}]</B>. Wall-Hit Difference: [{HorizontalDifference}]");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
           return false;
        }

        public override Vector3 Speed_Direction() => Vector3.zero; //This State does not require a speed

        public override void Activate()
        {
            base.Activate();
            CheckKinematic();
            SetEnterStatus(LedgeProfile.EnterStatus);
            animal.StopMoving_Zero(); //Remove all Input stuff
            animal.Force_Remove(); //Remove all forces when grabbing a ledge
        }

        private void CheckKinematic()
        {
            animal.InertiaPositionSpeed = Vector3.zero;         //Remove internia
            animal.DeltaPos = Vector3.zero;                     //Remove Delta position
            animal.DeltaRootMotion = Vector3.zero;              //Remove Delta position

            if (Kinematic.Value)
            {
                animal.RB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; 
                animal.RB.isKinematic = true;
            }
        }

        private bool InTransition;
        private bool ExitTransition;

        public override void OnStateMove(float deltatime)
        {
            if (InCoreAnimation)
            {
                InTransition = false;

                if (Anim.IsInTransition(0))
                {
                    var TransTime = Anim.GetAnimatorTransitionInfo(0).normalizedTime;
                    animal.AdditivePosition = Vector3.zero;
                    animal.AdditiveRotation = Quaternion.identity;

                    Quaternion AlignRot = Quaternion.FromToRotation(Forward, -WallNormal) * transform.rotation;  //Calculate the orientation to Terrain 

                    transform.position = Vector3.Lerp(StartPosition, TargetPosition, TransTime);

                    //Orient to wall 
                    if (OrientToWall) 
                        transform.rotation = Quaternion.Lerp(StartRotation, AlignRot, TransTime);
                    InTransition = true;
                }


                if (!InTransition && !ExitTransition && IsActiveState)
                {
                    animal.transform.position = TargetPosition;
                    ExitTransition = true;
                    //Debug.Log("ExitTransition");
                }


                animal.InertiaPositionSpeed = Vector3.zero; //Remove internia
                animal.PlatformMovement();

                if (LedgeProfile != null)
                {
                    if (LedgeProfile.Orient)
                    {
                        float DeltaAngle = Mathf.Lerp(0, AngleDifference, deltatime * AlignSmoothness * 2f);
                        AngleDifference -= DeltaAngle;
                        //animal.AdditiveRotation *= Quaternion.Euler(0, DeltaAngle, 0); //NOT WORKING DON't KNWO WHY
                        animal.transform.rotation *= Quaternion.Euler(0, DeltaAngle, 0);
                    }

                    if (LedgeProfile.AdditivePosition)
                    {
                        var time = animal.AnimState.normalizedTime;
                        // Debug.Log($"animal { time:F3}");

                        animal.AdditivePosition += Up * LedgeProfile.HeightCurve.Evaluate(time) * LedgeProfile.HeightSpeed * deltatime;
                        animal.AdditivePosition += Forward * LedgeProfile.ForwardCurve.Evaluate(time) * LedgeProfile.ForwardSpeed * deltatime;
                    }
                }
            }
        }

        public override void TryExitState(float DeltaTime)
        {
            //Debug.Log("animal.AnimState.normalizedTime = " + animal.AnimState.normalizedTime);

            if (animal.AnimState.normalizedTime > LedgeProfile.ExitTime) //Exit after the Current Ledge Profile time
            {
                AllowExit();
                animal.Grounded = true;
                //animal.CheckIfGrounded();
                Debugging($"Allow Exit - {LedgeProfile.name} After Exit Time {animal.AnimState.normalizedTime:F3} > {LedgeProfile.ExitTime}");
            }
        }

        public override void ResetStateValues()
        {
            LedgeProfile = null;
            InTransition = false;
            ExitTransition = false;
            OrientToWall = false;
            AngleDifference = 0;

            StartPosition = 
            TargetPosition =  
            WallNormal =
            AlignmentOffset = Vector3.zero;

            StartRotation =Quaternion.identity; 

            FoundLedgeHit = new RaycastHit();
            FoundWallHit = new RaycastHit();

            if (Kinematic.Value && animal)
                animal.RB.isKinematic = false;
        }


#if UNITY_EDITOR

        public override void SetSpeedSets(MAnimal animal)
        {
            //Do nothing... the Ledge Grab does not require a Speed Set
        }


        public override void StateGizmos(MAnimal animal)
        {
            if (Application.isPlaying) return;

             
            foreach (var p in profiles)
            {
                var point1 = animal.transform.TransformPoint(new Vector3(0, p.Height, 0));
                var pointWall1 = animal.transform.TransformPoint(new Vector3(0, p.Height - p.LedgeExitDistance - WallChecker, 0));


                var scale = animal.ScaleFactor;

                var dir = animal.Forward * ForwardLength * scale * p.ForwardMultiplier;
                var dirWall = animal.Forward * wallDistance * scale;
                var point2 = point1 + dir;
                var downExit = -animal.Up * p.LedgeExitDistance * scale;


                Gizmos.color = Color.green;

                Gizmos.DrawRay(point1, dir);
               // Gizmos.DrawRay(point2, downDir);


                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(pointWall1, dir);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(pointWall1, dirWall);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(point2, downExit);
            }

        }
        void Reset()
        {
            //Surface = MTools.GetResource<PhysicMaterial>("Climbable");
            ID = MTools.GetInstance<StateID>("LedgeGrab");

            automatic.Value = true;

            General = new AnimalModifier()
            {
                modify = (modifier)(-1),
                RootMotion = true,
                AdditivePosition = true,
                AdditiveRotation = false,
                Grounded = false,
                Sprint = true,
                OrientToGround = false,
                Gravity = false,
                CustomRotation = false,
                FreeMovement = false,
                IgnoreLowerStates = true,
            };

            profiles = new List<LedgeProfiles>();

            var prof = new LedgeProfiles();

            profiles.Add(prof);

            Input = "Jump";

            Editor_Tabs1 = 3;
        }
#endif
    }
    [System.Serializable]
    public class LedgeProfiles
    {
        public string name = "Ledge Grab";

        [Tooltip("State Enter Status to Activate while")]
        public int EnterStatus = 0;

        [Tooltip("Max Vertical Speed Needed to Check this Profile")]
        public float MaxVSpeed = 0;


        [Tooltip("Forward Length Multiplier applied to the Global Length")]
        public float ForwardMultiplier = 1;

        [Tooltip("Height Offset to cast the Ray for checking a ledge")]
        [Min(0)] public float Height = 1.5f;  

        [Tooltip("Ray to check if we have found a ledge")]
        [Min(0)] public float LedgeExitDistance = 0.25f;

        [Tooltip("If the Animation Normalized Time of this state (Ledge Grab) is greater Exit Animation time,\n" +
            " the State will Allow Exit()... so other states can try activate themselves.")]
        [Range(0,1)] public float ExitTime = 0.9f;

        [Tooltip("Horizontal(X) and Vertical(Y) values needed to apply offset movement to have better alignment with the Ledge")]
        public Vector2 AlingOffset;


        [Tooltip("Align the Animal to the Wall's normal direction")]
        public bool Orient = true;

        public bool AdditivePosition = false;

        [Hide("AdditivePosition",false)]
        [Min(0)] public float HeightSpeed = 0.5f;
        [Hide("AdditivePosition",false)]
        [Min(0)] public float ForwardSpeed = 0.5f;

        [Hide("AdditivePosition",false)]
        public AnimationCurve HeightCurve = new AnimationCurve(
               new Keyframe(0, 1), new Keyframe(0.45f, 1), new Keyframe(0.55f, 0f), new Keyframe(1, 0f)
            );

        [Hide("AdditivePosition",false)]
        public AnimationCurve ForwardCurve = new AnimationCurve(
              new Keyframe(0, 0), new Keyframe(0.45f, 0), new Keyframe(0.55f, 1f), new Keyframe(1, 1f)
           );
    }
}