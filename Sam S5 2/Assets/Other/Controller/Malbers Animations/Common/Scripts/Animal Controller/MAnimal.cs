﻿using UnityEngine;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;

namespace MalbersAnimations.Controller
{
    /// <summary>  This will controll all Animals Motion
    /// See changelog here https://malbersanimations.gitbook.io/animal-controller/annex/changelog
    /// </summary>

    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller")]
    [DefaultExecutionOrder(-10)]
    [SelectionBase]
    [AddComponentMenu("Malbers/Animal Controller/Animal")]
    public partial class MAnimal : MonoBehaviour,   
        IAnimatorListener, ICharacterMove, IGravity, IObjectCore, 
        IRandomizer, IMAnimator, ISleepController, IMDamagerSet, 
        IAnimatorStateCycle, ICharacterAction 
    {
        //Animal Variables: All variables
        //Animal Movement:  All Locomotion Logic
        //Animal CallBacks: All public methods and behaviors that it can be called outside the script

        #region Editor Show 
        [HideInInspector] public int PivotPosDir;
       // [HideInInspector] public bool ModeShowEvents;
        [HideInInspector] public int SelectedMode;
        [HideInInspector] public int SelectedState;
        [HideInInspector] public int SelectedStance;
        //[HideInInspector] public bool ShowAnimParametersOptional = false;
        //[HideInInspector] public bool ShowAnimParameters = false;
        //[HideInInspector] public bool ShowLockInputs = false;
        //[HideInInspector] public bool ShowMisc = false;
        
        
        [HideInInspector] public bool ShowStateInInspector = false;

        [HideInInspector] public int Editor_Tabs1;
        [HideInInspector] public int Mode_Tabs1;
        [HideInInspector] public int Editor_Tabs2;
        [HideInInspector] public int Ability_Tabs;
        [HideInInspector] public int Editor_EventTabs;
      
        [HideInInspector] public bool showPivots = true;
        [HideInInspector] public bool debugStates;
        [HideInInspector] public bool debugStances;
        [HideInInspector] public bool debugModes;
        [HideInInspector] public bool debugGizmos = true;
        #endregion

#if UNITY_EDITOR

        private void OnValidate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (Anim == null) Anim = GetComponentInParent<Animator>();   //Cache the Animator
            if (RB == null) RB = GetComponentInParent<Rigidbody>();      //Cache the Rigid Body  
            if (Aimer == null) Aimer = gameObject.FindComponent<Aim>();  //Cache the Aim Component 
        }

        void Reset()
        {

            MTools.SetLayer(base.transform, 20);     //Set all the Childrens to Animal Layer   .
            gameObject.tag = "Animal";                      //Set the Animal to Tag Animal
            AnimatorSpeed = 1;

            Anim = GetComponentInParent<Animator>();            //Cache the Animator
            RB = GetComponentInParent<Rigidbody>();             //Catche the Rigid Body  
            
            if (RB == null)
            {
                RB = gameObject.AddComponent<Rigidbody>();
                RB.useGravity = false;
                RB.constraints = RigidbodyConstraints.FreezeRotation;
                RB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            speedSets = new List<MSpeedSet>(1)
            {
                new MSpeedSet()
            {
                name = "Ground",
                    StartVerticalIndex = new IntReference(1),
                    TopIndex = new IntReference(3),
                    states =  new  List<StateID>(2) { MTools.GetInstance<StateID>("Idle") , MTools.GetInstance<StateID>("Locomotion")},
                    Speeds =  new  List<MSpeed>(3) { new MSpeed("Walk",1,4,4) , new MSpeed("Trot", 2, 4, 4), new MSpeed("Run", 3, 4, 4) }
            }
            };

            BoolVar useCameraInp = MTools.GetInstance<BoolVar>("Global Camera Input");
            BoolVar globalSmooth = MTools.GetInstance<BoolVar>("Global Smooth Vertical");
            FloatVar globalTurn = MTools.GetInstance<FloatVar>("Global Turn Multiplier");

            if (useCameraInp != null) useCameraInput.Variable = useCameraInp;
            if (globalSmooth != null) SmoothVertical.Variable = globalSmooth;
            if (globalTurn != null) TurnMultiplier.Variable = globalTurn;


            var DefStance = MTools.GetInstance<StanceID>("Default");

            if (defaultStance == null) defaultStance = DefStance;
            if (currentStance == null) currentStance = DefStance;

            pivots = new List<MPivots>
            { 
                new MPivots("Hip", new Vector3(0,0.7f,-0.7f), 1), 
                new MPivots("Chest", new Vector3(0,0.7f,0.7f), 1), 
                new MPivots("Water", new Vector3(0,1,0), 0.05f) 
            };


            //Pivot_Hip =  new  MPivots(pivots[0].name, pivots[0].position, pivots[0].multiplier);
            //Pivot_Chest = new MPivots(pivots[1].name, pivots[1].position, pivots[1].multiplier);
           
            //Has_Pivot_Hip = true;
            //Has_Pivot_Chest = true;
            //Starting_PivotChest = true;

            MTools.SetDirty(this);
        }

        [ContextMenu("Create Event Listeners")]
        void CreateListeners()
        {
            MEventListener listener = GetComponent<MEventListener>();

            if (listener == null) listener = gameObject.AddComponent<MEventListener>();
            if (listener.Events == null) listener.Events = new List<MEventItemListener>();

            MEvent MovementMobile = MTools.GetInstance<MEvent>("Set Movement Mobile");
            if (listener.Events.Find(item => item.Event == MovementMobile) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = MovementMobile,
                    useVoid = true,
                    useVector2 = true,
                };

                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseVector2, SetInputAxis);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.Response, UseCameraBasedInput);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseFloat, SetUpDownAxis);

                listener.Events.Add(item);

                Debug.Log("<B>Set Movement Mobile</B> Added to the Event Listeners");
            }

            //********************************//

            SetModesListeners(listener, "Set Attack1", "Attack1");
            SetModesListeners(listener, "Set Attack2", "Attack2");
            SetModesListeners(listener, "Set Action", "Action");

            /************************/

            MEvent actionstatus = MTools.GetInstance<MEvent>("Set Action Status");
            if (listener.Events.Find(item => item.Event == actionstatus) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = actionstatus,
                    useVoid = false,
                    useInt = true, useFloat = true
                };

                ModeID ac = MTools.GetInstance<ModeID>("Action");
                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener(item.ResponseInt, Mode_Pin, ac);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseInt, Mode_Pin_Status);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseFloat, Mode_Pin_Time);

                listener.Events.Add(item);

                Debug.Log("<B>Set Action Status</B> Added to the Event Listeners");
            }
            /************************/

            MEvent sprinting = MTools.GetInstance<MEvent>("Set Sprint");
            if (listener.Events.Find(item => item.Event == sprinting) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = sprinting,
                    useVoid = false,
                    useBool = true,
                };

                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool, SetSprint);

                listener.Events.Add(item);

                Debug.Log("<B>Sprint Listener</B> Added to the Event Listeners");
            }




            MEvent timeline = MTools.GetInstance<MEvent>("Timeline");
            if (listener.Events.Find(item => item.Event == timeline) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = timeline,
                    useVoid = false,
                    useBool = true,
                };

                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool,SetTimeline);

                listener.Events.Add(item);

                Debug.Log("<B>Timeline Listener</B> Added to the Event Listeners");
            }


            /************************/
            SetStateListeners(listener, "Set Jump", "Jump");
            SetStateListeners(listener, "Set Fly", "Fly");
            /************************/
        }

        
        void SetModesListeners(MEventListener listener ,string EventName, string ModeName)
        {
            MEvent e = MTools.GetInstance<MEvent>(EventName);
            if (listener.Events.Find(item => item.Event == e) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = e,
                    useVoid = true, useInt = true, useBool = true,
                };

                ModeID att2 = MTools.GetInstance<ModeID>(ModeName);

                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<ModeID>(item.ResponseBool, Mode_Pin, att2);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool, Mode_Pin_Input);
                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<ModeID>(item.ResponseInt, Mode_Pin, att2);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseInt, Mode_Pin_Ability);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.Response, Mode_Interrupt);

                listener.Events.Add(item);

                Debug.Log("<B>"+EventName+"</B> Added to the Event Listeners");
            }
        }

        void SetStateListeners(MEventListener listener, string EventName, string statename)
        {
            MEvent e = MTools.GetInstance<MEvent>(EventName);
            if (listener.Events.Find(item => item.Event == e) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = e,
                    useVoid = false,
                    useInt = true,
                    useBool = true,
                };

                StateID ss = MTools.GetInstance<StateID>(statename);

                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<StateID>(item.ResponseBool, State_Pin, ss);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool, State_Pin_ByInput);

                listener.Events.Add(item);

                Debug.Log("<B>" + EventName + "</B> Added to the Event Listeners");
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (!debugGizmos) return;
            float sc = transform.localScale.y;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Center, 0.02f * sc);
            Gizmos.DrawWireSphere(Center, 0.02f * sc);
        }

        void OnDrawGizmos()
        {
            float sc = transform.localScale.y;

            var pos = transform.position;

            if (showPivots)
            {
                foreach (var pivot in pivots)
                {
                    if (pivot != null)
                    {
                        if (pivot.PivotColor.a == 0)
                        {
                            pivot.PivotColor = Color.blue;
                        }

                        Gizmos.color = pivot.PivotColor;
                        Gizmos.DrawWireSphere(pivot.World(transform), sc * RayCastRadius);
                        Gizmos.DrawRay(pivot.World(transform), pivot.WorldDir(transform) * pivot.multiplier * sc);
                    }
                }
            }

            if (!debugGizmos) return;

          if (states.Count>1)
                states[SelectedState]?.StateGizmos(this);

            if (Application.isPlaying)
            {

                Gizmos.color = Color.green;
                MTools.Gizmo_Arrow(pos, TargetSpeed * 5 * sc);    //Draw the Target Direction 
               
                Gizmos.color = Color.white; 
                MTools.Gizmo_Arrow(pos, InertiaPositionSpeed * 2 * sc);  //Draw the Intertia Direction 


                Gizmos.color = Color.red;
              //  MTools.Gizmo_Arrow(pos, Move_Direction * sc*2); //MOVE DIRECTION RED

                Gizmos.color = Color.black;
                Gizmos.DrawSphere(pos + DeltaPos, 0.02f * sc);
              
                if (showPivots)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(Center, 0.02f * sc);
                    Gizmos.DrawSphere(Center, 0.02f * sc);
                }
                // return;



                if (CurrentExternalForce != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawRay(Center, CurrentExternalForce * sc /10);
                    Gizmos.DrawSphere(Center + (CurrentExternalForce  * sc/10), 0.05f * sc);
                }
            }
        }
#endif
    }

    [System.Serializable] public class AnimalEvent : UnityEvent<MAnimal> { }
}
