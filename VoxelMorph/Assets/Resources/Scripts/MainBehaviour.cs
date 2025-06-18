using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autodesk.Fbx;
using System.IO;
//using UnityEditor;
using UnityEngine.Formats.Fbx.Exporter;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
#endif




public class MainBehaviour : MonoBehaviour
{
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;



    public Vector2 turn;
    public Vector2 move;
    public float sz = 1;
    public static int RM = 2;//Rotation Multiplier
    public static float PM = 0.05f;//Position Multiplier
    public Transform CameraCont;
    public RaycastHit hit;

    public Transform currentTransform;
    public Transform oldTransform;
    public Material oldMaterial;
    public Material selectionMaterial;

    public static GameObject SelectedObject;
    public static bool clicked = false;
    public static int currentFunction = 0;

    public static int selectedColor32 = 0;
    public static int prevSelectedColor32 = 0;
    public static int prevPrevSelectedColor32 = 0;

    public static int[][] selection = new int[2][];
    public static int xStart;
    public static int yStart;
    public static int zStart;





    void Start()
    {
        GameObject GO = GameObject.Find("PlayGround");//PlayGround
        GameObject[] GOs = {GO};

        CameraCont = GameObject.Find("CameraCont").GetComponent<Transform>();
        selectionMaterial = Resources.Load("Materials/selection", typeof(Material)) as Material;
        currentTransform = GameObject.Find("selection").GetComponent<Transform>();

        selection[0] = new int[3];
        selection[1] = new int[3];
    }





    void Update()
    {
        if (Input.GetMouseButton(1)){
            turn.x += Input.GetAxis("Mouse X");
            turn.y += Input.GetAxis("Mouse Y");
            CameraCont.localRotation = Quaternion.Euler(-turn.y*RM, turn.x*RM, 0);
        }

        if (Input.GetMouseButton(2)){
            move.x = Input.GetAxis("Mouse X");
            move.y = Input.GetAxis("Mouse Y");
            sz = CameraCont.localScale.x;
            CameraCont.Translate(-move.x*PM*sz, -move.y*PM*sz, 0);
        }
        
        if(CameraCont.localScale.x <= 1 && Input.mouseScrollDelta.y > 0){
        }else{
            CameraCont.localScale += new Vector3(-Input.mouseScrollDelta.y,-Input.mouseScrollDelta.y,-Input.mouseScrollDelta.y);
        }

        if(currentFunction == 0){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.DeleteCubeStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.DeleteCubeEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 1){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.AddCubeStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.AddCubeEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 2){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.PaintStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.PaintEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 3){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.LinearGradientStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.LinearGradientEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 4){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.SelectionStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.SelectionEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 5){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.RadialGradientStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.RadialGradientEnd(SelectedObject);
                }
            }
        }else if(currentFunction == 9){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.MoveStart();
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.MoveEnd();
                }
            }
        }else if(currentFunction == 7){
                if (Input.GetMouseButtonDown(0)){
                    if(!clicked){
                        clicked = true;
                    }
                }
                if (Input.GetMouseButtonUp(0)){
                    if(clicked){
                        clicked = false;
                        GameObject.Find("Main Camera").GetComponent<OBJConverter>().ClickSaveState();
                        Application.Quit();
                    }
                }
        }else if(currentFunction == 8){
            if (Input.GetMouseButtonDown(0)){
                if(!clicked){
                    clicked = true;
                    Functions.InvertCubeStart(SelectedObject);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                if(clicked){
                    clicked = false;
                    Functions.InvertCubeEnd(SelectedObject);
                }
            }
        }



        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            Functions.MoveBack();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            Functions.MoveForward();
        }
    }



    public static void ClickBtn(int a){
        currentFunction = a;
    }



}


