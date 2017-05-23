using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubemapPreviewer : EditorWindow
{
    //Editor Window
    private static CubemapPreviewer window;

    //Cubemap Settings
    private Cubemap previewCubemap;
    private GameObject cubemapObj;

    //Camera Settings
    private GameObject camGameObject;
    private Camera cam;
    private float zoomAmount = 60f;

    //Mouse Movement
    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;
    private Vector2 clampInDegrees = new Vector2(360, 180);
    private Vector2 sensitivity = new Vector2(30f, -30f);
    private Vector2 smoothing = new Vector2(0.2f, 0.2f);
    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;
    private float smoothVaule = 4f;
    Quaternion currentRot = Quaternion.identity;
    Quaternion updatedRot = Quaternion.identity;
    private Vector2 editorMouseDelta;
    private Vector2 editorMousePosition;

    private bool hasSelectedCubemapView;

    [MenuItem("FSVR Tools/Cubemap Preview")]
    static void DisplayWindow()
    {
        window = (CubemapPreviewer)EditorWindow.GetWindow(typeof(CubemapPreviewer));
    }

    #region Camera Methods

    private void CreateViewingCamera()
    {

        camGameObject = new GameObject("Collision Painter Camera");
        camGameObject.transform.position = Vector3.zero;
        camGameObject.AddComponent<Camera>();
        camGameObject.hideFlags = HideFlags.HideInHierarchy;

        cam = camGameObject.GetComponent<Camera>();//.camera;
        cam.backgroundColor = Color.black;
        cam.enabled = false;
        cam.orthographic = false;
        cam.clearFlags = CameraClearFlags.Color;
        cam.nearClipPlane = 0.01f;

        if (cam.targetTexture == null)
        {
            cam.targetTexture = new RenderTexture((int)window.position.width, (int)window.position.height, 24,RenderTextureFormat.ARGB32);
        }
    }

    private void RemoveCamera()
    {
        DestroyImmediate(camGameObject);
    }

    #endregion

    #region Cubemap Methods

    private void CreateCubemap()
    {
        cubemapObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubemapObj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        cubemapObj.layer = 31; //Bit of a hack - If someone has something on layer 31 they will see this cube appear in the scene

        cubemapObj.transform.position = Vector3.zero;
        cubemapObj.transform.rotation = Quaternion.identity;

        Renderer cubemapRenderer = cubemapObj.GetComponent<Renderer>();

        cubemapRenderer.sharedMaterial = new Material(Shader.Find("Skybox/Cubemap"));
        cubemapRenderer.sharedMaterial.SetTexture("_Tex", previewCubemap);
    }

    private void RemoveCubemap()
    {
        DestroyImmediate(cubemapObj);
    }

    private void UpdateCubemapTexture()
    {
        if (cubemapObj != null)
        {
            Renderer cubemapRenderer = cubemapObj.GetComponent<Renderer>();
            cubemapRenderer.sharedMaterial.SetTexture("_Tex", previewCubemap);
        }
    }

    #endregion

    #region Mono Methods

    private void OnDestroy()
    {
        //Destroy the Camera if it exists
        if (cam != null)
        {
            RemoveCamera();
        }

        //Destroy the cubemap if it exists
        if (cubemapObj != null)
        {
            RemoveCubemap();
        }
    }

    private void Update()
    {
        if(window == null)
            return;
   
        if (cam != null)
        {
            if (hasSelectedCubemapView)
            {
                MouseMovement(cam.transform);
            }
        }
    }

    private void OnSelectionChange()
    {
        //Cubemap Rect
        Rect cubemapRect = new Rect(0, position.height * 0.25f, position.width, position.height * 0.75f);

        if (!cubemapRect.Contains(editorMousePosition))
        {
            hasSelectedCubemapView = false;
        }
    }

    void OnGUI()
    {
        //Get window if null
        if (window == null)
        {
            window = (CubemapPreviewer)EditorWindow.GetWindow(typeof(CubemapPreviewer));
        }

        //Create or remove if we have a cubemap texture
        if (previewCubemap != null && cubemapObj == null)
        {
            CreateCubemap();
        }
        else if (previewCubemap == null)
        {
            RemoveCubemap();
        }

        float fieldHeight = position.height * 0.02f;

        //Check for cubemap change
        EditorGUI.BeginChangeCheck();
        previewCubemap = (Cubemap)EditorGUILayout.ObjectField( previewCubemap, typeof(Cubemap), false);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateCubemapTexture();
        }

        //Mouse sensitivity
        EditorGUI.BeginChangeCheck();
        sensitivity = EditorGUILayout.Vector2Field("Sensitivity", sensitivity);
        zoomAmount = EditorGUILayout.Slider("Zoom", zoomAmount, 60, 120);
        smoothVaule = EditorGUILayout.FloatField("Smooth", smoothVaule);
        if (EditorGUI.EndChangeCheck())
        {
            hasSelectedCubemapView = false;
        }

        //Breathing space
        GUILayout.Space(10);

        //Create Camera if we dont have one yet
        if (cam == null)
        {
            CreateViewingCamera();
        }
        else
        {
            //Update FOV
            cam.fieldOfView = zoomAmount;

            //Render our Rendertexture
            cam.Render();

            //Draw it
            GUI.DrawTexture(new Rect(0, position.height * 0.25f, position.width, position.height * 0.75f), cam.targetTexture);
           
            //Mouse Events for controlling Look Controller for Cubemap View
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                //Clicked inside cubemap view
                Rect cubemapRect = new Rect(0, position.height * 0.25f, position.width, position.height * 0.75f);

                if (cubemapRect.Contains(Event.current.mousePosition))
                {
                    hasSelectedCubemapView = true;
                }

                //Clicked outside cubemap view
                Rect propertyRect = new Rect(0, 0, position.width, position.height * 0.25f);

                if (propertyRect.Contains(Event.current.mousePosition))
                {
                    hasSelectedCubemapView = false;
                }
            }

            //Update Mouse Delta for Rotation Movement
            UpdateEditorMouse();

            //What it says on the tin (WISOTT)
            Repaint();
        }

    }

    #endregion

    #region Input Methods

    void UpdateEditorMouse()
    {
        editorMouseDelta = Event.current.delta.normalized;
        editorMousePosition = Event.current.mousePosition;
    }

    void MouseMovement(Transform target)
    {
 
        currentRot = target.localRotation;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = editorMouseDelta;

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta,
            new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local X value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
        {
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
        }

        var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
        target.localRotation = xRotation;

        // Then clamp and apply the global Y value.
        if (clampInDegrees.y < 360)
        {
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
        }

        target.localRotation *= targetOrientation;
        var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, target.InverseTransformDirection(Vector3.up));

        updatedRot = target.localRotation * yRotation;
      
        target.localRotation = Quaternion.Slerp(currentRot, updatedRot, Time.deltaTime * smoothVaule);

    }

    #endregion

}
