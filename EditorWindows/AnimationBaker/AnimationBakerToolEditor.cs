using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;


public class AnimationBakerToolEditor : EditorWindow
{
#region Variables
    public GameObject objectToRecord;
    public bool recordChildrens = true;
    GameObjectRecorder m_Recorder;

    public string animationName;
    public string animationPath;
    public AnimationClip clip;
    bool isRecording = false;
    bool create = false;
    bool overrideAnim = true;

    int toolbarIndex;
    string[] toolbarContent = {"Override", "Create"};


    SerializedProperty objectToRecordProperty;
    SerializedProperty clipProperty;
    SerializedObject so;


    bool simulatePhysicsInEditMode;
    Dictionary<Rigidbody, Pose> rigidbodiesPoses = new Dictionary<Rigidbody, Pose>();

    public UnityEvent OnStartRecord;
    public UnityEvent OnStopRecord;

    SerializedProperty OnStartRecordProperty;
    SerializedProperty OnStopRecordProperty;



#endregion Variables

#region UnityCallBacks

    //Just put your utility path here
    [MenuItem("Tools/Louis/Animation Baker Tool")]
    static void ShowWindow()
    {
        AnimationBakerToolEditor window = (AnimationBakerToolEditor)EditorWindow.GetWindow(typeof(AnimationBakerToolEditor));
        window.Show();
    }

    void OnEnable()
    {
        //Create a scriptable from the editor window to cast our properties on (like a custom editor)
        ScriptableObject target = this;
        so = new SerializedObject(target);

        //Retrieve properties to cast them
        objectToRecordProperty = so.FindProperty("objectToRecord");
        clipProperty = so.FindProperty("clip");

        OnStartRecordProperty = so.FindProperty("OnStartRecord");
        OnStopRecordProperty = so.FindProperty("OnStopRecord");
    }
    
    private void OnGUI()
    {
        so.Update();

        GUILayout.Label("ANIMATION RECORDER", "ToolbarButton");
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("You can record in PLAY but also in EDITOR with the Simulate Physics in Edit Mode.", MessageType.Info);

        EditorGUILayout.Space(5);

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarContent);

        overrideAnim = toolbarIndex == 0;
        create =  toolbarIndex == 1;


        EditorGUILayout.Space(5);
        
        EditorGUILayout.PropertyField(objectToRecordProperty, true);
        recordChildrens = EditorGUILayout.Toggle("Record Children", recordChildrens);

        EditorGUILayout.Space(5);


        if(create)
        {
            animationName = EditorGUILayout.TextField("Animation Name", animationName);
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Save Location", "MiniButtonLeft"))
            {
                animationPath = EditorUtility.OpenFolderPanel("Animation Path", "Select your path", Application.dataPath); 
            }

            GUI.enabled = false;
            GUILayout.Label(animationPath);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        else if(overrideAnim)
        {
            EditorGUILayout.PropertyField(clipProperty, true);
        }
        

        EditorGUILayout.Space(10);

        float originalValue = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 200;   

        simulatePhysicsInEditMode = EditorGUILayout.Toggle("Simulate Physics in Edit Mode", simulatePhysicsInEditMode);

        EditorGUIUtility.labelWidth = originalValue;

        EditorGUILayout.Space(5);
        

        EditorGUILayout.BeginHorizontal();

        if(!isRecording)
        {
            if(GUILayout.Button("Start Recording"))
            {
                StartRecording();
            }
        }

        EditorGUILayout.Space(5);

        if(isRecording)
        {
            if(GUILayout.Button("Stop Recording"))
            {
                StopRecording();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(OnStartRecordProperty, true);
        EditorGUILayout.PropertyField(OnStartRecordProperty, true);

        so.ApplyModifiedProperties();
    }
    
    private void Update()
    {
        //Debug.Log("Recording = " + isRecording);

        if (isRecording)
        {

            m_Recorder.TakeSnapshot(Time.fixedDeltaTime);
            Repaint();
            //Debug.Log("Recording...");


            //SIMULATE PHYSICS IF IN EDITOR
            HandlePhysicSimulation();

        }

    }

#endregion Unity Callbacks

    private void HandlePhysicSimulation()
    {
        if (!EditorApplication.isPlaying)
        {
#if UNITY_2022_1_OR_NEWER
            if (simulatePhysicsInEditMode)
            {
                if (Physics.simulationMode == SimulationMode.FixedUpdate)
                {
                    Physics.simulationMode = SimulationMode.Script;
                }

                Physics.Simulate(Time.fixedDeltaTime);
            }

            else if (Physics.simulationMode == SimulationMode.Script)
            {
                Physics.simulationMode = SimulationMode.FixedUpdate;
            }
#endif

#if !UNITY_2022_1_OR_NEWER

                if(simulatePhysicsInEditMode)
                {
                    if(Physics.autoSimulation == false)
                    {
                        Physics.autoSimulation = true;
                    }
                    
                    Physics.Simulate(Time.fixedDeltaTime);
                }

                else if(Physics.autoSimulation == true)
                {
                    Physics.autoSimulation = false;
                }

#endif
        }
    }

    void CreateNewAnimationClip()
    {
        AnimationClip newClip = new AnimationClip
        {
            name = animationName
        };

        clip = newClip;
        AssetDatabase.CreateAsset(clip, GetCorrectPath(animationPath) + "/" + animationName + ".anim");
        AssetDatabase.SaveAssets();
    }
    void StartRecording()
    { 
        //Instantiate a new recorder and set the binding type
        m_Recorder = new GameObjectRecorder(objectToRecord);
        m_Recorder.BindComponentsOfType<Transform>(objectToRecord, recordChildrens);

        if(create)
        {
            CreateNewAnimationClip();
        }


        //If we are simulating the physic in edit mode, we record rigidbodies poses to reset them later
        if(simulatePhysicsInEditMode)
        SaveRigibodiesPoses();
        

        isRecording = true;
        OnStartRecord?.Invoke();

        Debug.Log("Start Recording Animation..");
        
    }
    void StopRecording()
    {
        isRecording = false;
        OnStopRecord?.Invoke();

        m_Recorder.SaveToClip(clip);
        AssetDatabase.SaveAssets();
        DestroyImmediate(m_Recorder);

        Debug.Log("Stop Recording Animation..");

        if(simulatePhysicsInEditMode)
        RestaureRigidbodiesPoses();
    }
    string GetCorrectPath(string path)
    {
        string savePath;
        string[] paths = path.Split("/Assets");
      
        Debug.Log("Assets" + paths[1]);
        savePath = "Assets" + paths[1];

        return savePath;
    }
    void SaveRigibodiesPoses()
    {
        rigidbodiesPoses.Clear();

        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();

        for (int i = 0; i < bodies.Length; i++)
        {
            Pose bodyPose = new Pose
            {
                position = bodies[i].transform.position,
                rotation = bodies[i].transform.rotation
            };

            rigidbodiesPoses.Add(bodies[i], bodyPose);
        }
        
        Debug.Log($"Saving poses of {bodies.Length} rigidbodies.");
    }
    void RestaureRigidbodiesPoses()
    {
        foreach(Rigidbody rb in rigidbodiesPoses.Keys)
        {
            rb.transform.SetPositionAndRotation(rigidbodiesPoses[rb].position, rigidbodiesPoses[rb].rotation);
        }
    }

    void OnDestroy()
    {
        //Ensure that the physic simulation mode is set back to fixed update ('cause it can do a lot of damages)
        
        #if UNITY_2022_1_OR_NEWER
        Physics.simulationMode = SimulationMode.FixedUpdate;
        #endif

        #if !UNITY_2022_1_OR_NEWER
        Physics.autoSimulation = true;
        #endif
    }
}
