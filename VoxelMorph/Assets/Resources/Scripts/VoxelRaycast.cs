using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoxelRaycast : MonoBehaviour
{
    public int[,,] voxelGrid; // Трёхмерный массив вокселей (1 - есть куб, 0 - пусто)
    public Vector3 gridSize = new Vector3(40, 40, 40); // Размер воксельного мира
    public float voxelSize = 1.0f; // Размер одного вокселя
    public int offset = 128;
    public static Vector3 vox1 = new Vector3(-12,-12,-12);
    public static Vector3 pre1 = new Vector3(-12,-12,-13);

    public static Vector3 preRayVox = new Vector3(-1,0,0);
    public static bool deSelect = false;

    public static Vector3 vox2 = new Vector3(0,0,0);
    public static Vector3 pre2 = new Vector3(-1,0,0);

    void Update()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 hitVoxel;
            if (RaycastVoxel(ray.origin, ray.direction, out hitVoxel))
            {
                    if(MainBehaviour.currentFunction == 9){
                        VoxelMover.HandleInput(hitVoxel, preRayVox, true);
                    }

                if (!(Input.GetMouseButton(0) && MainBehaviour.currentFunction == 9)){
                    vox2 = (hitVoxel-new Vector3(offset,offset,offset));
                    pre2 = (preRayVox-new Vector3(offset,offset,offset));

                    if(vox1 != vox2 || pre1 != pre2){
                        
                        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().SelectVoxel(vox2, pre2, new Color32(50,255,100,255),      VoxelMeshGenerator.lastVox, VoxelMeshGenerator.lastPre, VoxelMeshGenerator.lastColor32);//,   convX,convY,convZ
                        vox1 = vox2;
                        pre1 = pre2;
                        deSelect = true;
                    }
                }

            }
            else if(deSelect){
                if (!(Input.GetMouseButton(0) && MainBehaviour.currentFunction == 9)){
                    GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().SelectVoxel(new Vector3(-123,-123,-123), VoxelMeshGenerator.lastPre, VoxelMeshGenerator.lastColor32,      VoxelMeshGenerator.lastVox, VoxelMeshGenerator.lastPre, VoxelMeshGenerator.lastColor32); //,   convXO,convYO,convZO
                    deSelect = false;
                    vox1 = new Vector3(-12,-12,-12);
                    pre1 = new Vector3(-12,-12,-13);
                }
                
                if(MainBehaviour.currentFunction == 9){
                    VoxelMover.HandleInput(new Vector3(127, 127, 127), new Vector3(127, 127, 127), false);
                }
            }else{
                if(MainBehaviour.currentFunction == 9){
                    VoxelMover.HandleInput(new Vector3(127, 127, 127), new Vector3(127, 127, 127), false);
                }
            }
    }

    public int size;
    public void PreInitializeRaycast(){
        size = OBJConverter.model.Length+(offset*2);
        gridSize = new Vector3(size, size, size);
        voxelGrid = new int[size, size, size];

        for(int x = 0; x < size; x++){
            for(int y = 0; y < size; y++){
                for(int z = 0; z < size; z++){

                    if(x >= offset && y >= offset && z >= offset && x < size-offset && y < size-offset && z < size-offset){
                        if(OBJConverter.model[x-offset][y-offset][z-offset].a != 0 && OBJConverter.selectionModel[x-offset][y-offset][z-offset]){
                            voxelGrid[x, y, z] = 1; // Весь объём заполнен
                        }else{
                            voxelGrid[x, y, z] = 0;
                        }
                    }else{
                        voxelGrid[x, y, z] = 0;
                    }

                }
            }
        }

    }

    public void InitializeRaycast(){
        int chkN = OBJConverter.chunkN;
        int nrmChk = OBJConverter.normalChunk;

        int n = OBJConverter.n;

        Vector3Int selectionStart1 = VoxelMeshGenerator.selectionStart;
        Vector3Int selectionEnd1 = VoxelMeshGenerator.selectionEnd;

        int ss1X = selectionStart1[0];
        int ss1Y = selectionStart1[1];
        int ss1Z = selectionStart1[2];

        int se1X = selectionEnd1[0];
        int se1Y = selectionEnd1[1];
        int se1Z = selectionEnd1[2];

        if(ss1X > 0 || true){ ss1X -= 1; }
        if(ss1Y > 0 || true){ ss1Y -= 1; }
        if(ss1Z > 0 || true){ ss1Z -= 1; }

        if(se1X < n-1 || true){ se1X += 1; }
        if(se1Y < n-1 || true){ se1Y += 1; }
        if(se1Z < n-1 || true){ se1Z += 1; }

        // Определяем чанки начала и конца
        int startChunkX = ss1X / nrmChk;
        int startChunkY = ss1Y / nrmChk;
        int startChunkZ = ss1Z / nrmChk;

        int endChunkX = se1X / nrmChk;
        int endChunkY = se1Y / nrmChk;
        int endChunkZ = se1Z / nrmChk;

        // Первый куб в начальном чанке (его начало)
        int firstCubeX = startChunkX * nrmChk;
        int firstCubeY = startChunkY * nrmChk;
        int firstCubeZ = startChunkZ * nrmChk;

        // Последний куб в конечном чанке (его конец)
        int lastCubeX = (endChunkX + 1) * nrmChk - 1;
        int lastCubeY = (endChunkY + 1) * nrmChk - 1;
        int lastCubeZ = (endChunkZ + 1) * nrmChk - 1;

        for (int x = firstCubeX+offset; x <= lastCubeX+offset; x++)
        {
            for (int y = firstCubeY+offset; y <= lastCubeY+offset; y++)
            {
                for (int z = firstCubeZ+offset; z <= lastCubeZ+offset; z++)
                {

                    if(x >= offset && y >= offset && z >= offset && x < size-offset && y < size-offset && z < size-offset){
                        if(OBJConverter.model[x-offset][y-offset][z-offset].a != 0 && OBJConverter.selectionModel[x-offset][y-offset][z-offset]){
                            voxelGrid[x, y, z] = 1; // 
                        }else{
                            voxelGrid[x, y, z] = 0;
                        }
                    }else{
                        voxelGrid[x, y, z] = 0;
                    }

                }
            }
        }
        deSelect = false;
        vox1 = new Vector3(-12,-12,-12);
        pre1 = new Vector3(-12,-12,-13);
    }






    bool RaycastVoxel(Vector3 origin, Vector3 direction, out Vector3 hitVoxel)
    {
        hitVoxel = Vector3.zero;

        // Преобразуем мировые координаты в индексы воксельной сетки
        Vector3 voxelPos = origin / voxelSize;

        int x = Mathf.FloorToInt(voxelPos.x);
        int y = Mathf.FloorToInt(voxelPos.y);
        int z = Mathf.FloorToInt(voxelPos.z);

        // Определяем шаги и дельты для DDA
        int stepX = (direction.x > 0) ? 1 : -1;
        int stepY = (direction.y > 0) ? 1 : -1;
        int stepZ = (direction.z > 0) ? 1 : -1;

        float tMaxX = ((x + (stepX > 0 ? 1 : 0)) - voxelPos.x) / direction.x;
        float tMaxY = ((y + (stepY > 0 ? 1 : 0)) - voxelPos.y) / direction.y;
        float tMaxZ = ((z + (stepZ > 0 ? 1 : 0)) - voxelPos.z) / direction.z;

        float tDeltaX = Mathf.Abs(1 / direction.x);
        float tDeltaY = Mathf.Abs(1 / direction.y);
        float tDeltaZ = Mathf.Abs(1 / direction.z);

        // Ограничение по дальности трассировки
        int maxSteps = offset*2;

        for (int i = 0; i < maxSteps; i++)
        {
            // Если попали в воксель (непустой), возвращаем его координаты
            if (IsVoxelFilled(x, y, z))
            {
                hitVoxel = new Vector3(x, y, z);
                return true;
            }
            preRayVox = new Vector3(x, y, z);


            // Выбираем, в каком направлении двигаться
            if (tMaxX < tMaxY && tMaxX < tMaxZ)
            {
                tMaxX += tDeltaX;
                x += stepX;
            }
            else if (tMaxY < tMaxZ)
            {
                tMaxY += tDeltaY;
                y += stepY;
            }
            else
            {
                tMaxZ += tDeltaZ;
                z += stepZ;
            }

            // Если вышли за границы мира – прекращаем поиск
            if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y || z < 0 || z >= gridSize.z)
                return false;
        }

        return false;
    }

    // Проверка, есть ли воксель в этой позиции
    public bool IsVoxelFilled(int x, int y, int z)
    {
        return voxelGrid[x, y, z] == 1;
    }
}
