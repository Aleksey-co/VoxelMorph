using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class Functions : MonoBehaviour
{
    void Start()
    {
        
    }





    public static void DeleteCubeStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void DeleteCubeEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            int xMax = MainBehaviour.selection[1][0];
            int yMax = MainBehaviour.selection[1][1];
            int zMax = MainBehaviour.selection[1][2];
            int xMin = MainBehaviour.selection[0][0];
            int yMin = MainBehaviour.selection[0][1];
            int zMin = MainBehaviour.selection[0][2];
            int buff = 0;
            if(xMax < xMin){buff = xMax; xMax = xMin; xMin = buff;}
            if(yMax < yMin){buff = yMax; yMax = yMin; yMin = buff;}
            if(zMax < zMin){buff = zMax; zMax = zMin; zMin = buff;}

            VoxelMeshGenerator.selectionStart = new Vector3Int(xMin,yMin,zMin);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(xMax,yMax,zMax);

            for (int i = 0; i < OBJConverter.model.Length; i++)//destroying everything
            {
                for (int j = 0; j < OBJConverter.model[i].Length; j++)
                {
                    for (int k = 0; k < OBJConverter.model[i][j].Length; k++) {
                        if(xMin <= i && i <= xMax && yMin <= j && j <= yMax && zMin <= k && k <= zMax && OBJConverter.selectionModel[i][j][k]){
                            OBJConverter.model[i][j][k].a = 0;
                        }
                    }
                }
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }

    }





    public static void AddCubeStart(GameObject obj){
            if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
                int xo = (int)VoxelRaycast.pre2[0] - (int)VoxelRaycast.vox2[0];
                int yo = (int)VoxelRaycast.pre2[1] - (int)VoxelRaycast.vox2[1];
                int zo = (int)VoxelRaycast.pre2[2] - (int)VoxelRaycast.vox2[2];

                MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0]+xo;
                MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1]+yo;
                MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2]+zo;
            }else{
                MainBehaviour.selection[0][0] = -1;
            }
    }





    public static void AddCubeEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            int xo = (int)VoxelRaycast.pre2[0] - (int)VoxelRaycast.vox2[0];
            int yo = (int)VoxelRaycast.pre2[1] - (int)VoxelRaycast.vox2[1];
            int zo = (int)VoxelRaycast.pre2[2] - (int)VoxelRaycast.vox2[2];

            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0]+xo;
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1]+yo;
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2]+zo;

                int xMax = MainBehaviour.selection[1][0];
                int yMax = MainBehaviour.selection[1][1];
                int zMax = MainBehaviour.selection[1][2];
                int xMin = MainBehaviour.selection[0][0];
                int yMin = MainBehaviour.selection[0][1];
                int zMin = MainBehaviour.selection[0][2];
                int buff = 0;
                if(xMax < xMin){buff = xMax; xMax = xMin; xMin = buff;}
                if(yMax < yMin){buff = yMax; yMax = yMin; yMin = buff;}
                if(zMax < zMin){buff = zMax; zMax = zMin; zMin = buff;}

                if(xMax >= OBJConverter.n){xMax = OBJConverter.n-1;}
                if(yMax >= OBJConverter.n){yMax = OBJConverter.n-1;}
                if(zMax >= OBJConverter.n){zMax = OBJConverter.n-1;}
                if(xMin < 0){xMin = 0;}
                if(yMin < 0){yMin = 0;}
                if(zMin < 0){zMin = 0;}

                VoxelMeshGenerator.selectionStart = new Vector3Int(xMin,yMin,zMin);
                VoxelMeshGenerator.selectionEnd = new Vector3Int(xMax,yMax,zMax);

                int ln = OBJConverter.model.Length;
                if (xMin < 0 || yMin < 0 || zMin < 0 ||        xMax > ln || yMax > ln || zMax > ln){
                }else{
                    for (int i = 0; i < OBJConverter.model.Length; i++)//adding cubes
                    {
                        for (int j = 0; j < OBJConverter.model[i].Length; j++)
                        {
                            for (int k = 0; k < OBJConverter.model[i][j].Length; k++) {
                                if(xMin <= i && i <= xMax && yMin <= j && j <= yMax && zMin <= k && k <= zMax){
                                    if(OBJConverter.model[i][j][k].a == 0 && OBJConverter.selectionModel[i][j][k]){
                                        OBJConverter.model[i][j][k] = new Color32(102,102,102,255);
                                    }
                                }
                            }
                        }
                    }

                GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
                GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
                GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
            }

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }
    }





    public static void InvertCubeStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void InvertCubeEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            int xMax = MainBehaviour.selection[1][0];
            int yMax = MainBehaviour.selection[1][1];
            int zMax = MainBehaviour.selection[1][2];
            int xMin = MainBehaviour.selection[0][0];
            int yMin = MainBehaviour.selection[0][1];
            int zMin = MainBehaviour.selection[0][2];
            int buff = 0;
            if(xMax < xMin){buff = xMax; xMax = xMin; xMin = buff;}
            if(yMax < yMin){buff = yMax; yMax = yMin; yMin = buff;}
            if(zMax < zMin){buff = zMax; zMax = zMin; zMin = buff;}

            VoxelMeshGenerator.selectionStart = new Vector3Int(xMin,yMin,zMin);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(xMax,yMax,zMax);
            
            for (int i = 0; i < OBJConverter.model.Length; i++)//destroying everything
            {
                for (int j = 0; j < OBJConverter.model[i].Length; j++)
                {
                    for (int k = 0; k < OBJConverter.model[i][j].Length; k++) {
                        if(xMin <= i && i <= xMax && yMin <= j && j <= yMax && zMin <= k && k <= zMax){
                            if(OBJConverter.model[i][j][k].a == 0 && OBJConverter.selectionModel[i][j][k]){ OBJConverter.model[i][j][k] = new Color32(102,102,102,255);
                            }else if(OBJConverter.selectionModel[i][j][k]){
                                OBJConverter.model[i][j][k].a = 0;
                            }
                        }
                    }
                }
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }
    }





    public static void PaintStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void PaintEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            int xMax = MainBehaviour.selection[1][0];
            int yMax = MainBehaviour.selection[1][1];
            int zMax = MainBehaviour.selection[1][2];
            int xMin = MainBehaviour.selection[0][0];
            int yMin = MainBehaviour.selection[0][1];
            int zMin = MainBehaviour.selection[0][2];
            int buff = 0;
            if(xMax < xMin){buff = xMax; xMax = xMin; xMin = buff;}
            if(yMax < yMin){buff = yMax; yMax = yMin; yMin = buff;}
            if(zMax < zMin){buff = zMax; zMax = zMin; zMin = buff;}

            VoxelMeshGenerator.selectionStart = new Vector3Int(xMin,yMin,zMin);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(xMax,yMax,zMax);

            for (int i = 0; i < OBJConverter.model.Length; i++)//destroying everything
            {
                for (int j = 0; j < OBJConverter.model[i].Length; j++)
                {
                    for (int k = 0; k < OBJConverter.model[i][j].Length; k++) {
                        if(xMin <= i && i <= xMax && yMin <= j && j <= yMax && zMin <= k && k <= zMax){
                            if(OBJConverter.model[i][j][k].a != 0 && OBJConverter.selectionModel[i][j][k]){
                                Color32 pickedColor32 = GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
                                OBJConverter.model[i][j][k] = pickedColor32;//"" + pickedColor32[0] + ";" + pickedColor32[1] + ";" + pickedColor32[2] + ";" + pickedColor32[3] + ";";
                            }
                        }
                    }
                }
            }
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        }
    }





    public static void SelectionStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void SelectionEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            int xMax = MainBehaviour.selection[1][0];
            int yMax = MainBehaviour.selection[1][1];
            int zMax = MainBehaviour.selection[1][2];
            int xMin = MainBehaviour.selection[0][0];
            int yMin = MainBehaviour.selection[0][1];
            int zMin = MainBehaviour.selection[0][2];
            int buff = 0;
            if(xMax < xMin){buff = xMax; xMax = xMin; xMin = buff;}
            if(yMax < yMin){buff = yMax; yMax = yMin; yMin = buff;}
            if(zMax < zMin){buff = zMax; zMax = zMin; zMin = buff;}

            VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

            for (int i = 0; i < OBJConverter.model.Length; i++){
                for (int j = 0; j < OBJConverter.model[i].Length; j++){
                    for (int k = 0; k < OBJConverter.model[i][j].Length; k++){
                        if(i >= xMin && i <= xMax && j >= yMin && j <= yMax && k >= zMin && k <= zMax){
                            OBJConverter.selectionModel[i][j][k] = true;
                        }else{
                            OBJConverter.selectionModel[i][j][k] = false;
                        }
                    }
                }
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
        }
    }





    public static void dropSelection(){
        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

        for (int i = 0; i < OBJConverter.model.Length; i++){
            for (int j = 0; j < OBJConverter.model[i].Length; j++){
                for (int k = 0; k < OBJConverter.model[i][j].Length; k++){
                    OBJConverter.selectionModel[i][j][k] = true;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
    }





    public static void LinearGradientStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void LinearGradientEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

            //gradient vector
            int x = (MainBehaviour.selection[1][0] - MainBehaviour.selection[0][0]);
            int y = (MainBehaviour.selection[1][1] - MainBehaviour.selection[0][1]);
            int z = (MainBehaviour.selection[1][2] - MainBehaviour.selection[0][2]);

            Color a = GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.prevSelectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
            Color b = GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;

            float[] initCol = new float[3];
            float[] colMarg = new float[3];

            float interp = 0;//interpolation coefficient

            for(int i = 0; i < 3; i++){
                initCol[i] = a[i];
                colMarg[i] = b[i] - a[i];
            }

            int SMN = OBJConverter.n;

            for (int i = 0; i < SMN; i++)
            {
                for (int j = 0; j < SMN; j++)
                {
                    for (int k = 0; k < SMN; k++) {

                        Vector3 point = new Vector3(i+MainBehaviour.xStart-MainBehaviour.selection[0][0],j+MainBehaviour.yStart-MainBehaviour.selection[0][1],k+MainBehaviour.zStart-MainBehaviour.selection[0][2]);
                        Vector3 lineDirection = new Vector3(x,y,z);

                        Vector3 projection = ProjectPointOntoLine(point, lineDirection);
                        interp = Mathf.Clamp(projection.magnitude / lineDirection.magnitude, 0f, 1f);

                        if((lineDirection.x>0 && projection.x<0) || (lineDirection.y>0 && projection.y<0) || (lineDirection.z>0 && projection.z<0) || (lineDirection.x<0 && projection.x>0) || (lineDirection.y<0 && projection.y>0) || (lineDirection.z<0 && projection.z>0)){
                            interp = 0;
                        }

                        Color32 newCol = new Color32((byte)((initCol[0] + colMarg[0] * interp)*255), (byte)((initCol[1] + colMarg[1] * interp)*255), (byte)((initCol[2] + colMarg[2] * interp)*255), 255);

                        if(OBJConverter.model[(i+MainBehaviour.xStart)][(j+MainBehaviour.yStart)][(k+MainBehaviour.zStart)].a != 0 && OBJConverter.selectionModel[(i+MainBehaviour.xStart)][(j+MainBehaviour.yStart)][(k+MainBehaviour.zStart)]){
                            Color32 pickedColor32 = newCol;
                            OBJConverter.model[i][j][k] = pickedColor32;
                        }
                    }
                }
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }
    }





    public static Vector3 ProjectPointOntoLine(Vector3 point, Vector3 lineDirection){
        float distance = Vector3.Dot(point, lineDirection) / (lineDirection.magnitude*lineDirection.magnitude);//.LengthSquared();
        return distance * lineDirection;
    }





    public static void RadialGradientStart(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12)){
            MainBehaviour.selection[0][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[0][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[0][2] = (int)VoxelRaycast.vox2[2];
        }else{
            MainBehaviour.selection[0][0] = -1;
        }
    }





    public static void RadialGradientEnd(GameObject obj){
        if(VoxelRaycast.vox1 != new Vector3(-12,-12,-12) && MainBehaviour.selection[0][0] != -1){
            MainBehaviour.selection[1][0] = (int)VoxelRaycast.vox2[0];
            MainBehaviour.selection[1][1] = (int)VoxelRaycast.vox2[1];
            MainBehaviour.selection[1][2] = (int)VoxelRaycast.vox2[2];

            VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

            Color a = GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.prevSelectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
            Color b = GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;

            float[] initCol = new float[3];
            float[] colMarg = new float[3];

            float interp = 0;//interpolation coefficient

            for(int i = 0; i < 3; i++){
                initCol[i] = a[i];
                colMarg[i] = b[i] - a[i];
            }

            int SMN = OBJConverter.n;

            for (int i = 0; i < SMN; i++)
            {
                for (int j = 0; j < SMN; j++)
                {
                    for (int k = 0; k < SMN; k++) {
                        Vector3 CP = new Vector3(i+MainBehaviour.xStart,j+MainBehaviour.yStart,k+MainBehaviour.zStart);
                        Vector3 SP = new Vector3(MainBehaviour.selection[0][0],MainBehaviour.selection[0][1],MainBehaviour.selection[0][2]);
                        Vector3 EP = new Vector3(MainBehaviour.selection[1][0],MainBehaviour.selection[1][1],MainBehaviour.selection[1][2]);

                        float initialMagnitude = Mathf.Sqrt((EP.x-SP.x)*(EP.x-SP.x) + (EP.y-SP.y)*(EP.y-SP.y) + (EP.z-SP.z)*(EP.z-SP.z));
                        float currentMagnitude = Mathf.Sqrt((CP.x-SP.x)*(CP.x-SP.x) + (CP.y-SP.y)*(CP.y-SP.y) + (CP.z-SP.z)*(CP.z-SP.z));

                        interp = currentMagnitude / initialMagnitude;
                        if(interp > 1){interp = 1;}

                        Color32 newCol = new Color32((byte)((initCol[0] + colMarg[0] * interp)*255), (byte)((initCol[1] + colMarg[1] * interp)*255), (byte)((initCol[2] + colMarg[2] * interp)*255), 255);

                        if(OBJConverter.model[(i+MainBehaviour.xStart)][(j+MainBehaviour.yStart)][(k+MainBehaviour.zStart)].a != 0 && OBJConverter.selectionModel[(i+MainBehaviour.xStart)][(j+MainBehaviour.yStart)][(k+MainBehaviour.zStart)]){
                            Color32 pickedColor32 = newCol;
                            OBJConverter.model[i][j][k] = pickedColor32;
                        }
                        
                    }
                }
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }
    }





    public static void MoveForward(){
        if(OBJConverter.historyPosition <= 2){
            GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        	GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

            GameObject.Find("MoveForward").GetComponent<Button>().interactable = false;
        }
        
        if(OBJConverter.historyPosition >= 2){
            OBJConverter.historyPosition -= 1;
            OBJConverter.model = CloneJaggedArray(OBJConverter.modelHistory[OBJConverter.modelHistory.Count-OBJConverter.historyPosition]);
            
            VoxelMeshGenerator.selectionStart = OBJConverter.modelHistorySelectionStart[OBJConverter.modelHistory.Count-OBJConverter.historyPosition];
            VoxelMeshGenerator.selectionEnd = OBJConverter.modelHistorySelectionEnd[OBJConverter.modelHistory.Count-OBJConverter.historyPosition];

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        	GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.ActiveBack;
        	GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.ActiveHighlight;

            GameObject.Find("MoveBack").GetComponent<Button>().interactable = true;

        }
    }





    public static void RemoveRestIndecies(){
        if(OBJConverter.historyPosition > 1){
            while(OBJConverter.historyPosition > 2){
                OBJConverter.historyPosition -= 1;
                OBJConverter.modelHistory.RemoveAt(OBJConverter.modelHistory.Count - 1);
                OBJConverter.modelHistorySelectionStart.RemoveAt(OBJConverter.modelHistory.Count - 1);
                OBJConverter.modelHistorySelectionEnd.RemoveAt(OBJConverter.modelHistory.Count - 1);
            }

            Vector3Int a = OBJConverter.modelHistorySelectionStart[OBJConverter.modelHistorySelectionStart.Count-2];
            Vector3Int b = OBJConverter.modelHistorySelectionEnd[OBJConverter.modelHistorySelectionEnd.Count-2];

            OBJConverter.historyPosition -= 1;
            OBJConverter.modelHistory.RemoveAt(OBJConverter.modelHistory.Count - 1);
            OBJConverter.modelHistorySelectionStart.RemoveAt(OBJConverter.modelHistory.Count - 1);
            OBJConverter.modelHistorySelectionEnd.RemoveAt(OBJConverter.modelHistory.Count - 1);

            OBJConverter.modelHistorySelectionStart[OBJConverter.modelHistorySelectionStart.Count-1] = a;
            OBJConverter.modelHistorySelectionEnd[OBJConverter.modelHistorySelectionEnd.Count-1] = b;
        }

        GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.ActiveBack;
        GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.ActiveHighlight;

        GameObject.Find("MoveForward").GetComponent<Button>().interactable = false;
        GameObject.Find("MoveBack").GetComponent<Button>().interactable = true;
    }





    public static void MoveBack(){ //Ctrl+Z
        if(OBJConverter.modelHistory.Count-OBJConverter.historyPosition <= 1){
        	GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        	GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

            GameObject.Find("MoveBack").GetComponent<Button>().interactable = false;
        }

        if(OBJConverter.modelHistory.Count-OBJConverter.historyPosition >= 1){
            OBJConverter.historyPosition += 1;
            OBJConverter.model = CloneJaggedArray(OBJConverter.modelHistory[OBJConverter.modelHistory.Count-OBJConverter.historyPosition]);

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.ActiveBack;
        	GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.ActiveHighlight;
            GameObject.Find("MoveForward").GetComponent<Button>().interactable = true;

            VoxelMeshGenerator.selectionStart = OBJConverter.modelHistorySelectionStart[OBJConverter.modelHistory.Count-OBJConverter.historyPosition];
            VoxelMeshGenerator.selectionEnd = OBJConverter.modelHistorySelectionEnd[OBJConverter.modelHistory.Count-OBJConverter.historyPosition];
        }
    }





    public static Color32[][][] CloneJaggedArray(Color32[][][] original)
    {
        Color32[][][] copy = new Color32[original.Length][][];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = new Color32[original[i].Length][];
            for (int j = 0; j < original[i].Length; j++)
            {
                copy[i][j] = (Color32[])original[i][j].Clone();  // Копируем каждый вложенный массив
            }
        }
        return copy;
    }





    public void ClearModel(){
        int n = OBJConverter.n;
        OBJConverter.model = new Color32[n][][];
        OBJConverter.selectionModel = new bool[n][][];
        for (int i = 0; i < n; i++)
        {
            OBJConverter.model[i] = new Color32[n][];
            OBJConverter.selectionModel[i] = new bool[n][];
            for (int j = 0; j < n; j++)
            {
                OBJConverter.model[i][j] = new Color32[n];
                OBJConverter.selectionModel[i][j] = new bool[n];
                for (int k = 0; k < n; k++) {
                    OBJConverter.model[i][j][k] = new Color32(102,102,102,255);//"0,4;0,4;0,4;1;";//StandardMaterial.name
                    OBJConverter.selectionModel[i][j][k] = true;
                }
            }
        }

        OBJConverter.historyPosition = 1;
        OBJConverter.modelHistory.Clear();
        OBJConverter.modelHistorySelectionStart.Clear();
        OBJConverter.modelHistorySelectionEnd.Clear();

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

        GameObject.Find("MoveForward").GetComponent<Button>().interactable = false;
        GameObject.Find("MoveBack").GetComponent<Button>().interactable = false;

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
    }





    public static void DeleteModel(){
        int n = OBJConverter.n;
        OBJConverter.model = new Color32[n][][];
        OBJConverter.selectionModel = new bool[n][][];
        for (int i = 0; i < n; i++)
        {
            OBJConverter.model[i] = new Color32[n][];
            OBJConverter.selectionModel[i] = new bool[n][];
            for (int j = 0; j < n; j++)
            {
                OBJConverter.model[i][j] = new Color32[n];
                OBJConverter.selectionModel[i][j] = new bool[n];
                for (int k = 0; k < n; k++) {
                    OBJConverter.model[i][j][k] = new Color32(102,102,102,0);//"0,4;0,4;0,4;1;";//StandardMaterial.name
                    OBJConverter.selectionModel[i][j][k] = true;
                }
            }
        }

        OBJConverter.historyPosition = 1;
        OBJConverter.modelHistory.Clear();
        OBJConverter.modelHistorySelectionStart.Clear();
        OBJConverter.modelHistorySelectionEnd.Clear();

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

        GameObject.Find("MoveForward").GetComponent<Button>().interactable = false;
        GameObject.Find("MoveBack").GetComponent<Button>().interactable = false;

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
    }





    public void ShowMainMenu(){
        MenuId = 2;

        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);

        GameObject.Find("MainMenuBtn").GetComponent<Button>().interactable = false;
        GameObject.Find("RenderMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("MoveMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("HideUIBtn").GetComponent<Button>().interactable = true;
    }
    public void ShowRenderMenu(){
        MenuId = 1;

        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);

        GameObject.Find("MainMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("RenderMenuBtn").GetComponent<Button>().interactable = false;
        GameObject.Find("MoveMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("HideUIBtn").GetComponent<Button>().interactable = true;
    }
    public void ShowMoveMenu(){
        MenuId = 3;

        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("MainMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
        GameObject.Find("RenderMenu").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        GameObject.Find("MoveMenu").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        GameObject.Find("MainMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("RenderMenuBtn").GetComponent<Button>().interactable = true;
        GameObject.Find("MoveMenuBtn").GetComponent<Button>().interactable = false;
        GameObject.Find("HideUIBtn").GetComponent<Button>().interactable = true;
    }

    public int MenuId = 1;
    public bool IsHidden = false;

    public void HideUI(){
        if(!IsHidden){
            IsHidden = !IsHidden;
            GameObject.Find("NavBar").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
            GameObject.Find("NavBar").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
            GameObject.Find("Menus").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
            GameObject.Find("Menus").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
            GameObject.Find("HiddenUI").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("HiddenUI").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.Find("EditorBorder").GetComponent<Renderer>().material.color = new Color(0,0,0,0);
        }else{
            IsHidden = !IsHidden;
            GameObject.Find("NavBar").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("NavBar").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.Find("Menus").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("Menus").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.Find("HiddenUI").GetComponent<RectTransform>().offsetMin = new Vector2(-10000, 0);
            GameObject.Find("HiddenUI").GetComponent<RectTransform>().offsetMax = new Vector2(-10000, 0);
            GameObject.Find("EditorBorder").GetComponent<Renderer>().material.color = new Color(0.6f,0.6f,0.6f,1);
        }
    }





    public bool metallic1 = true;
    public bool smooth1 = true;
    public bool light1 = true;

    public void SliderMetallic(){
        if(metallic1){
            float value1 = Mathf.Round(GameObject.Find("SliderMetallic").GetComponent<Slider>().value);
            value1 = ValidateSlider(value1);
            GameObject.Find("InputMetallic").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderMetallic").GetComponent<Slider>().value = value1;
            float value3 = value1/255;
            GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetFloat("_Metallic", value3);
        }
    }

    public void InputMetallic(){
        float value1 = float.Parse(GameObject.Find("InputMetallic").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSlider(value1);
        if(value1 != value2){
            GameObject.Find("InputMetallic").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        metallic1 = false;
        GameObject.Find("SliderMetallic").GetComponent<Slider>().value = value2;
        metallic1 = true;
        float value3 = value2/255;
        GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetFloat("_Metallic", value3);
    }

    public void SliderSmooth(){
        if(smooth1){
            float value1 = Mathf.Round(GameObject.Find("SliderSmooth").GetComponent<Slider>().value);
            value1 = ValidateSlider(value1);
            GameObject.Find("InputSmooth").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderSmooth").GetComponent<Slider>().value = value1;

            float value3 = value1/255;
            GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetFloat("_Glossiness", value3);
        }
    }

    public void InputSmooth(){
        float value1 = float.Parse(GameObject.Find("InputSmooth").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSlider(value1);
        if(value1 != value2){
            GameObject.Find("InputSmooth").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        smooth1 = false;
        GameObject.Find("SliderSmooth").GetComponent<Slider>().value = value2;
        smooth1 = true;

        float value3 = value2/255;
        GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetFloat("_Glossiness", value3);
    }

    public void SliderLightIntensity(){
        if(light1){
            float value1 = Mathf.Round(GameObject.Find("SliderLightIntensity").GetComponent<Slider>().value);
            value1 = ValidateSlider(value1);
            GameObject.Find("InputLightIntensity").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderLightIntensity").GetComponent<Slider>().value = value1;

            float value3 = value1/255;
            GameObject.Find("DirectionalLight1").GetComponent<Light>().intensity = value3*2*1.5f;
            GameObject.Find("DirectionalLight2").GetComponent<Light>().intensity = value3*2;
        }
    }

    public void InputLightIntensity(){
        float value1 = float.Parse(GameObject.Find("InputLightIntensity").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSlider(value1);
        if(value1 != value2){
            GameObject.Find("InputLightIntensity").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        light1 = false;
        GameObject.Find("SliderLightIntensity").GetComponent<Slider>().value = value2;
        light1 = true;

        float value3 = value2/255;
        GameObject.Find("DirectionalLight1").GetComponent<Light>().intensity = value3*2*1.5f;
        GameObject.Find("DirectionalLight2").GetComponent<Light>().intensity = value3*2;
    }

    public float ValidateSlider(float value1){
        if(value1 > 255){ value1 = 255; }
        else if(value1 < 0){ value1 = 0; }
        return value1;
    }





    public bool ClH = true;
    public bool ClS = true;
    public bool ClV = true;

    public static float HVal = 0.0f;
    public static float SVal = 1.0f;
    public static float VVal = 1.0f;

    public void SliderH(){
        if(ClH){
            float value1 = Mathf.Round(GameObject.Find("SliderH").GetComponent<Slider>().value);
            value1 = ValidateSliderH(value1);
            GameObject.Find("InputH").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderH").GetComponent<Slider>().value = value1;

            HVal = value1/360;
            GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
            GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
        }
    }

    public void InputH(){
        float value1 = float.Parse(GameObject.Find("InputH").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderH(value1);
        if(value1 != value2){
            GameObject.Find("InputH").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClH = false;
        GameObject.Find("SliderH").GetComponent<Slider>().value = value2;
        ClH = true;

        HVal = value2/360;
        GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
        GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
    }

    public void SliderS(){
        if(ClS){
            float value1 = Mathf.Round(GameObject.Find("SliderS").GetComponent<Slider>().value);
            value1 = ValidateSliderSV(value1);
            GameObject.Find("InputS").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderS").GetComponent<Slider>().value = value1;

            SVal = value1/255;
            GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
            GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
        }
    }

    public void InputS(){
        float value1 = float.Parse(GameObject.Find("InputS").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderSV(value1);
        if(value1 != value2){
            GameObject.Find("InputS").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClS = false;
        GameObject.Find("SliderS").GetComponent<Slider>().value = value2;
        ClS = true;

        SVal = value2/255;
        GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
        GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
    }

    public void SliderV(){
        if(ClV){
            float value1 = Mathf.Round(GameObject.Find("SliderV").GetComponent<Slider>().value);
            value1 = ValidateSliderSV(value1);
            GameObject.Find("InputV").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderV").GetComponent<Slider>().value = value1;

            VVal = value1/255;
            GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
            GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
            GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
        }
    }

    public void InputV(){
        float value1 = float.Parse(GameObject.Find("InputV").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderSV(value1);
        if(value1 != value2){
            GameObject.Find("InputV").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClV = false;
        GameObject.Find("SliderV").GetComponent<Slider>().value = value2;
        ClV = true;

        VVal = value2/255;
        GameObject.Find("colBtnM").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, VVal);
        GameObject.Find("SliderSBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, 1, 1);
        GameObject.Find("SliderVBG").GetComponent<RawImage>().color = Color.HSVToRGB(HVal, SVal, 1);
    }





    public bool ClH2 = true;
    public bool ClS2 = true;
    public bool ClV2 = true;

    public static float HVal2 = 0.0f;
    public static float SVal2 = 0.0f;
    public static float VVal2 = 1.0f;

    public void SliderH2(){
        if(ClH2){
            float value1 = Mathf.Round(GameObject.Find("SliderH2").GetComponent<Slider>().value);
            value1 = ValidateSliderH(value1);
            GameObject.Find("InputH2").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderH2").GetComponent<Slider>().value = value1;

            HVal2 = value1/360;
            GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
            GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
            GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

            GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
            GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        }
    }

    public void InputH2(){
        float value1 = float.Parse(GameObject.Find("InputH2").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderH(value1);
        if(value1 != value2){
            GameObject.Find("InputH2").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClH2 = false;
        GameObject.Find("SliderH2").GetComponent<Slider>().value = value2;
        ClH2 = true;

        HVal2 = value2/360;
        GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
        GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
        GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

        GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
    }

    public void SliderS2(){
        if(ClS2){
            float value1 = Mathf.Round(GameObject.Find("SliderS2").GetComponent<Slider>().value);
            value1 = ValidateSliderSV(value1);
            GameObject.Find("InputS2").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderS2").GetComponent<Slider>().value = value1;

            SVal2 = value1/255;
            GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
            GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
            GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

            GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
            GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        }
    }

    public void InputS2(){
        float value1 = float.Parse(GameObject.Find("InputS2").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderSV(value1);
        if(value1 != value2){
            GameObject.Find("InputS2").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClS2 = false;
        GameObject.Find("SliderS2").GetComponent<Slider>().value = value2;
        ClS2 = true;

        SVal2 = value2/255;
        GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
        GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
        GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

        GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
    }

    public void SliderV2(){
        if(ClV2){
            float value1 = Mathf.Round(GameObject.Find("SliderV2").GetComponent<Slider>().value);
            value1 = ValidateSliderSV(value1);
            GameObject.Find("InputV2").GetComponent<TMP_InputField>().text = value1.ToString();
            GameObject.Find("SliderV2").GetComponent<Slider>().value = value1;

            VVal2 = value1/255;
            GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
            GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
            GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

            GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
            GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        }
    }

    public void InputV2(){
        float value1 = float.Parse(GameObject.Find("InputV2").GetComponent<TMP_InputField>().text);
        float value2 = ValidateSliderSV(value1);
        if(value1 != value2){
            GameObject.Find("InputV2").GetComponent<TMP_InputField>().text = value2.ToString();
        }
        ClV2 = false;
        GameObject.Find("SliderV2").GetComponent<Slider>().value = value2;
        ClV2 = true;

        VVal2 = value2/255;
        GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, VVal2);
        GameObject.Find("SliderSBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, 1, 1);
        GameObject.Find("SliderVBG2").GetComponent<RawImage>().color = Color.HSVToRGB(HVal2, SVal2, 1);

        GameObject.Find("DirectionalLight1").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
        GameObject.Find("DirectionalLight2").GetComponent<Light>().color = GameObject.Find("colBtnR").GetComponent<Transform>().GetChild(0).GetComponent<RawImage>().color;
    }





    public float ValidateSliderH(float value1){
        if(value1 > 360){ value1 = 360; }
        else if(value1 < 0){ value1 = 0; }
        return value1;
    }
    public float ValidateSliderSV(float value1){
        if(value1 > 255){ value1 = 255; }
        else if(value1 < 0){ value1 = 0; }
        return value1;
    }





    public void InvertSelection(){
        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(OBJConverter.n-1,OBJConverter.n-1,OBJConverter.n-1);

        for (int i = 0; i < OBJConverter.model.Length; i++){
            for (int j = 0; j < OBJConverter.model[i].Length; j++){
                for (int k = 0; k < OBJConverter.model[i][j].Length; k++){
                    OBJConverter.selectionModel[i][j][k] = !OBJConverter.selectionModel[i][j][k];
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
    }





    ///// // Movement // /////

    //Flips
    public void FlipX(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int i = 0; i < OBJConverter.model.Length/2; i++){
            for (int j = 0; j < OBJConverter.model[i].Length; j++){
                for (int k = 0; k < OBJConverter.model[i][j].Length; k++){
                    Buff = OBJConverter.model[i][j][k];
                    OBJConverter.model[i][j][k] = OBJConverter.model[n - 1 - i][j][k];
                    OBJConverter.model[n - 1 - i][j][k] = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }

    public void FlipY(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int i = 0; i < OBJConverter.model.Length; i++){
            for (int j = 0; j < OBJConverter.model[i].Length/2; j++){
                for (int k = 0; k < OBJConverter.model[i][j].Length; k++){
                    Buff = OBJConverter.model[i][j][k];
                    OBJConverter.model[i][j][k] = OBJConverter.model[i][n - 1 - j][k];
                    OBJConverter.model[i][n - 1 - j][k] = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }

    public void FlipZ(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int i = 0; i < OBJConverter.model.Length; i++){
            for (int j = 0; j < OBJConverter.model[i].Length; j++){
                for (int k = 0; k < OBJConverter.model[i][j].Length/2; k++){
                    Buff = OBJConverter.model[i][j][k];
                    OBJConverter.model[i][j][k] = OBJConverter.model[i][j][n - 1 - k];
                    OBJConverter.model[i][j][n - 1 - k] = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }





    //Rotations
    public void RotateX(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int x = 0; x < OBJConverter.model.Length; x++){
            for (int y = 0; y < OBJConverter.model[x].Length/2; y++){
                for (int z = y; z < OBJConverter.model[x][y].Length - 1 - y; z++){

                    // Циклическая перестановка 4 элементов
                    Buff = OBJConverter.model[x][y][z];
                    OBJConverter.model[x][y][z]                 = OBJConverter.model[x][n - 1 - z][y];
                    OBJConverter.model[x][n - 1 - z][y]         = OBJConverter.model[x][n - 1 - y][n - 1 - z];
                    OBJConverter.model[x][n - 1 - y][n - 1 - z] = OBJConverter.model[x][z][n - 1 - y];
                    OBJConverter.model[x][z][n - 1 - y]         = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }

    public void RotateY(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int y = 0; y < n; y++){
            for (int x = 0; x < n / 2; x++){
                for (int z = x; z < n - 1 - x; z++){
                    Buff = OBJConverter.model[x][y][z];
                    OBJConverter.model[x][y][z]                 = OBJConverter.model[z][y][n - 1 - x];
                    OBJConverter.model[z][y][n - 1 - x]         = OBJConverter.model[n - 1 - x][y][n - 1 - z];
                    OBJConverter.model[n - 1 - x][y][n - 1 - z] = OBJConverter.model[n - 1 - z][y][x];
                    OBJConverter.model[n - 1 - z][y][x]         = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }

    public void RotateZ(){
        int n = OBJConverter.model.Length;

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        Color32 Buff;
        for (int z = 0; z < n; z++){
            for (int x = 0; x < n / 2; x++){
                for (int y = x; y < n - 1 - x; y++){
                    Buff = OBJConverter.model[x][y][z];
                    OBJConverter.model[x][y][z]                 = OBJConverter.model[y][n - 1 - x][z];
                    OBJConverter.model[y][n - 1 - x][z]         = OBJConverter.model[n - 1 - x][n - 1 - y][z];
                    OBJConverter.model[n - 1 - x][n - 1 - y][z] = OBJConverter.model[n - 1 - y][x][z];
                    OBJConverter.model[n - 1 - y][x][z]         = Buff;
                }
            }
        }

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        RemoveRestIndecies();
        OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
        OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
    }





    public static void MoveStart(){
    }

    public static void MoveEnd(){
        Vector3 targetObjectOffset = GameObject.Find("TargetObject").transform.position - new Vector3(128,128,128);
        GameObject.Find("TargetObject").transform.position = new Vector3(128,128,128);

        if(targetObjectOffset != Vector3.zero){
            int n = OBJConverter.model.Length;
            VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
            VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

            if(targetObjectOffset.x != 0){
                ShiftArrayXInPlace(OBJConverter.model, (int)(targetObjectOffset.x));
            }else if(targetObjectOffset.y != 0){
                ShiftArrayYInPlace(OBJConverter.model, (int)(targetObjectOffset.y));
            }else if(targetObjectOffset.z != 0){
                ShiftArrayZInPlace(OBJConverter.model, (int)(targetObjectOffset.z));
            }

            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
            GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

            RemoveRestIndecies();
            OBJConverter.modelHistory.Add(CloneJaggedArray(OBJConverter.model));
            OBJConverter.modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
            OBJConverter.modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);
        }
        
    }





    public static void ShiftArrayXInPlace(Color32[][][] array, int shift)
    {
        int sizeX = array.Length;
        if (sizeX == 0) return;

        // Приводим shift к диапазону [0, sizeX)
        shift = ((shift % sizeX) + sizeX) % sizeX;
        if (shift == 0) return;

        int countMoved = 0;
        bool[] visited = new bool[sizeX];

        for (int start = 0; countMoved < sizeX; start++)
        {
            if (visited[start]) continue;

            int current = start;
            Color32[][] prev = array[start];

            do
            {
                int next = (current + shift) % sizeX;

                // Swap
                Color32[][] temp = array[next];
                array[next] = prev;
                prev = temp;

                visited[current] = true;
                current = next;
                countMoved++;

            } while (current != start);
        }
    }

    public static void ShiftArrayYInPlace(Color32[][][] array, int shift)
    {
        int sizeX = array.Length;
        for (int x = 0; x < sizeX; x++)
        {
            int sizeY = array[x]?.Length ?? 0;
            if (sizeY == 0) continue;

            int realShift = ((shift % sizeY) + sizeY) % sizeY;
            if (realShift == 0) continue;

            int countMoved = 0;
            bool[] visited = new bool[sizeY];

            for (int start = 0; countMoved < sizeY; start++)
            {
                if (visited[start]) continue;

                int current = start;
                Color32[] prev = array[x][start];

                do
                {
                    int next = (current + realShift) % sizeY;

                    Color32[] temp = array[x][next];
                    array[x][next] = prev;
                    prev = temp;

                    visited[current] = true;
                    current = next;
                    countMoved++;
                }
                while (current != start);
            }
        }
    }

    public static void ShiftArrayZInPlace(Color32[][][] array, int shift)
    {
        int sizeX = array.Length;
        for (int x = 0; x < sizeX; x++)
        {
            int sizeY = array[x]?.Length ?? 0;
            for (int y = 0; y < sizeY; y++)
            {
                int sizeZ = array[x][y]?.Length ?? 0;
                if (sizeZ == 0) continue;

                int realShift = ((shift % sizeZ) + sizeZ) % sizeZ;
                if (realShift == 0) continue;

                int countMoved = 0;
                bool[] visited = new bool[sizeZ];

                for (int start = 0; countMoved < sizeZ; start++)
                {
                    if (visited[start]) continue;

                    int current = start;
                    Color32 prev = array[x][y][start];

                    do
                    {
                        int next = (current + realShift) % sizeZ;

                        Color32 temp = array[x][y][next];
                        array[x][y][next] = prev;
                        prev = temp;

                        visited[current] = true;
                        current = next;
                        countMoved++;
                    }
                    while (current != start);
                }
            }
        }
    }





    public void SelectColor32(int colNum){
        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.prevPrevSelectedColor32).GetComponent<Transform>().GetChild(1).GetComponent<RawImage>().color = new Color(0.35f,0.35f,0.35f,1);
        MainBehaviour.prevPrevSelectedColor32 = MainBehaviour.prevSelectedColor32;

        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.prevSelectedColor32).GetComponent<Transform>().GetChild(1).GetComponent<RawImage>().color = new Color(0.35f,0.35f,0.35f,1);
        MainBehaviour.prevSelectedColor32 = MainBehaviour.selectedColor32;

        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(1).GetComponent<RawImage>().color = new Color(0.34f,0.75f,0.80f,1);
        MainBehaviour.selectedColor32 = colNum;
        GameObject.Find("ContentCols").GetComponent<Transform>().GetChild(MainBehaviour.selectedColor32).GetComponent<Transform>().GetChild(1).GetComponent<RawImage>().color = new Color(0.42f,0.92f,0.50f,1);
    }





}


