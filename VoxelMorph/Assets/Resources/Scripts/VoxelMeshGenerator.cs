using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class VoxelMeshGenerator : MonoBehaviour
{
        private MeshFilter meshFilter;
        public static Color32 lastColor32 = new Color32(255,255,255,255);
        public static Vector3 lastVox = new Vector3(0,0,0);
        public static Vector3 lastPre = new Vector3(0,0,0);

        private static MeshFilter[][][] meshFilters;

        public static Vector3Int selectionStart = new Vector3Int(0,0,0);
        public static Vector3Int selectionEnd = new Vector3Int(63,63,63);

        public static bool changedSize = false;
    
    private void Start()
    {

    }


    public void InitializeMesh()
    {
        GameObject PlayGround = GameObject.Find("PlayGround");
        for (int i = PlayGround.GetComponent<Transform>().childCount - 1; i >= 0; i--)
        {
            Destroy(PlayGround.GetComponent<Transform>().GetChild(i).gameObject);
        }



        if(meshFilters != null && meshFilters[0][0][0] != null){
            for (int x = meshFilters.Length-1; x >= 0; x--)
            {
                for (int y = meshFilters.Length-1; y >= 0; y--)
                {
                    for (int z = meshFilters.Length-1; z >= 0; z--)
                    {
                        Destroy(meshFilters[x][y][z].mesh);
                    }
                }
            }
        }

        

        int n = OBJConverter.chunkN;
        Material a1 = new Material (Resources.Load("Materials/p11", typeof(Material)) as Material);
        a1 = GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1;

        meshFilters = new MeshFilter[n][][];
        for (int i = 0; i < n; i++)
        {
            meshFilters[i] = new MeshFilter[n][];
            for (int j = 0; j < n; j++)
            {
                meshFilters[i][j] = new MeshFilter[n];
                for (int k = 0; k < n; k++) {
                    GameObject chunk = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
                    chunk.GetComponent<Transform>().localPosition = new Vector3(128,128,128);
                    chunk.name = i + "," + j + "," + k + ",";
                    
                    meshFilters[i][j][k] = chunk.AddComponent<MeshFilter>();

                    chunk.AddComponent<MeshRenderer>();
                    chunk.AddComponent<PortalWrapRenderer>();
                    chunk.GetComponent<Transform>().GetComponent<MeshRenderer>().material = a1;
                    chunk.GetComponent<Transform>().SetParent(PlayGround.GetComponent<Transform>());
                }
            }
        }



        size = OBJConverter.n;
        voxels = new bool[size, size, size];
        GenerateMesh();
    }



private void AddCubeHardEdges(List<Vector3> vertices, List<int> triangles, Vector3 position, bool[,,] voxels, int x, int y, int z, int size)
{
    int startIndex = vertices.Count;

    Vector3[] cubeVertices = {
        new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0),// Передняя
        new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0),// Правая

        new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1),// Задняя
        new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1),// Левая

        new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1),// Верхняя
        new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 0),// Нижняя
    };

    int[] cubeTriangles = {
        0, 2, 1,    0, 3, 2, // Передняя
        0+4, 2+4, 1+4,    0+4, 3+4, 2+4, // Правая

        0+8, 2+8, 1+8,    0+8, 3+8, 2+8, // Задняя
        0+12, 2+12, 1+12,    0+12, 3+12, 2+12, // Левая

        0+16, 2+16, 1+16,    0+16, 3+16, 2+16, // Верхняя
        0+20, 2+20, 1+20,    0+20, 3+20, 2+20  // Нижняя
    };

    foreach (var v in cubeVertices)
        vertices.Add(v + position);

    // Проверка соседей
    Vector3Int[] neighbors = {
        new Vector3Int(0, 0, -1), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
    };

    for (int i1 = 0; i1 < 6; i1++) //для каждого соседа его x y z
    {

        int nx = x + neighbors[i1].x;
        int ny = y + neighbors[i1].y;
        int nz = z + neighbors[i1].z;

        if (nx < 0 || ny < 0 || nz < 0 || nx >= size || ny >= size || nz >= size || !voxels[nx, ny, nz])
        {
            for(int j1 = 0; j1 < 6; j1++){
                triangles.Add((startIndex + cubeTriangles[j1 + i1*6]));//v
            }
        }
    }
}



public int size;
public bool[,,] voxels; //model
public Mesh[][][] mesh;

public void GenerateMesh()
{
        int chkN = OBJConverter.chunkN;
        int nrmChk = OBJConverter.normalChunk;


        Vector3Int selectionStart1 = selectionStart;
        Vector3Int selectionEnd1 = selectionEnd;

        int n = OBJConverter.n;


        int ss1X = selectionStart1[0];
        int ss1Y = selectionStart1[1];
        int ss1Z = selectionStart1[2];

        int se1X = selectionEnd1[0];
        int se1Y = selectionEnd1[1];
        int se1Z = selectionEnd1[2];



        int ss2X = selectionStart1[0];
        int ss2Y = selectionStart1[1];
        int ss2Z = selectionStart1[2];

        int se2X = selectionEnd1[0];
        int se2Y = selectionEnd1[1];
        int se2Z = selectionEnd1[2];




        if(ss1X > 0){ ss1X -= 1; }
        if(ss1Y > 0){ ss1Y -= 1; }
        if(ss1Z > 0){ ss1Z -= 1; }

        if(se1X < n-1){ se1X += 1; }
        if(se1Y < n-1){ se1Y += 1; }
        if(se1Z < n-1){ se1Z += 1; }


        // Определяем чанки начала и конца
        int startChunkX = ss1X / nrmChk;
        int startChunkY = ss1Y / nrmChk;
        int startChunkZ = ss1Z / nrmChk;

        int endChunkX = se1X / nrmChk;
        int endChunkY = se1Y / nrmChk;
        int endChunkZ = se1Z / nrmChk;



        // Определяем чанки начала и конца без отступов
        int startChunkX2 = ss2X / nrmChk;
        int startChunkY2 = ss2Y / nrmChk;
        int startChunkZ2 = ss2Z / nrmChk;

        int endChunkX2 = se2X / nrmChk;
        int endChunkY2 = se2Y / nrmChk;
        int endChunkZ2 = se2Z / nrmChk;



        // Первый куб в начальном чанке (его начало)
        int firstCubeX = startChunkX * nrmChk;
        int firstCubeY = startChunkY * nrmChk;
        int firstCubeZ = startChunkZ * nrmChk;

        // Последний куб в конечном чанке (его конец)
        int lastCubeX = (endChunkX + 1) * nrmChk - 1;
        int lastCubeY = (endChunkY + 1) * nrmChk - 1;
        int lastCubeZ = (endChunkZ + 1) * nrmChk - 1;

        if(lastCubeX > n-1){lastCubeX = n-1;}
        if(lastCubeY > n-1){lastCubeY = n-1;}
        if(lastCubeZ > n-1){lastCubeZ = n-1;}



            if(mesh != null && !changedSize){
                for (int x = endChunkX; x >= startChunkX; x--)
                {
                    for (int y = endChunkY; y >= startChunkY; y--)
                    {
                        for (int z = endChunkZ; z >= startChunkZ; z--)
                        {
                            Destroy(mesh[x][y][z]);
                            Destroy(meshFilters[x][y][z].mesh);
                            
                        }
                    }
                }
            }


            if(changedSize){changedSize = false;}





    mesh = new Mesh[chkN][][];
    //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Включаем 32-битные индексы
    List<Vector3>[][][] vertices = new List<Vector3>[chkN][][];
    List<int>[][][] triangles = new List<int>[chkN][][];
    for (int x = 0; x < chkN; x++)
    {
        mesh[x] = new Mesh[chkN][];
        //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Включаем 32-битные индексы
        vertices[x] = new List<Vector3>[chkN][];
        triangles[x] = new List<int>[chkN][];
        for (int y = 0; y < chkN; y++)
        {
            mesh[x][y] = new Mesh[chkN];
            //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Включаем 32-битные индексы
            vertices[x][y] = new List<Vector3>[chkN];
            triangles[x][y] = new List<int>[chkN];
            for (int z = 0; z < chkN; z++)
            {
                mesh[x][y][z] = new Mesh();

                //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Включаем 32-битные индексы
                vertices[x][y][z] = new List<Vector3>();
                triangles[x][y][z] = new List<int>();
            }
        }
    }





    for (int x = selectionStart1[0]; x <= selectionEnd1[0]; x++)
    {
        for (int y = selectionStart1[1]; y <= selectionEnd1[1]; y++)
        {
            for (int z = selectionStart1[2]; z <= selectionEnd1[2]; z++)
            {
                if(OBJConverter.selectionModel[x][y][z] && OBJConverter.model[x][y][z].a != 0){// != ""
                    voxels[x, y, z] = true; // Весь объём заполнен
                }else{
                    voxels[x, y, z] = false;
                }
            }
        }
    }



    for (int x = firstCubeX; x <= lastCubeX; x++)
    {
        for (int y = firstCubeY; y <= lastCubeY; y++)
        {
            for (int z = firstCubeZ; z <= lastCubeZ; z++)
            {
                if (!voxels[x, y, z]) continue; // Если куба нет, пропускаем

                int convX = (int)Math.Ceiling( (float)((((float)x+1f)/nrmChk)-1f) );
                int convY = (int)Math.Ceiling( (float)((((float)y+1f)/nrmChk)-1f) );
                int convZ = (int)Math.Ceiling( (float)((((float)z+1f)/nrmChk)-1f) );

                AddCubeHardEdges(vertices[convX][convY][convZ], triangles[convX][convY][convZ], new Vector3(x, y, z), voxels, x, y, z, size);//, new Vector3(x, y, z) //////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                
            }
        }
    }




    Color32[][][][] colors321 = new Color32[chkN][][][];
    for (int x = 0; x < chkN; x++)
    {
        colors321[x] = new Color32[chkN][][];
        for (int y = 0; y < chkN; y++)
        {
            colors321[x][y] = new Color32[chkN][];
            for (int z = 0; z < chkN; z++)
            {
                colors321[x][y][z] = new Color32[vertices[x][y][z].Count];
            }
        }
    }



                for (int x = startChunkX; x <= endChunkX; x++)
                {
                    for (int y = startChunkY; y <= endChunkY; y++)
                    {
                        for (int z = startChunkZ; z <= endChunkZ; z++)
                        {



                            for(int v = 0; v < vertices[x][y][z].Count; v+=24){
                                Color32 b = OBJConverter.model[(int)vertices[x][y][z][v + 0][0]] [(int)vertices[x][y][z][v + 0][1]] [(int)vertices[x][y][z][v + 0][2]];
                                for(int i = 0; i < 24; i++){
                                    colors321[x][y][z][v + i] = b;
                                }
                            }



                        }
                    }
                }



            for (int x = startChunkX; x <= endChunkX; x++)
                {
                    for (int y = startChunkY; y <= endChunkY; y++)
                    {
                        for (int z = startChunkZ; z <= endChunkZ; z++)
                        {

                        mesh[x][y][z].vertices = vertices[x][y][z].ToArray();
                        mesh[x][y][z].triangles = triangles[x][y][z].ToArray();
                        mesh[x][y][z].colors32 = colors321[x][y][z]; // Применяем цвета
                        mesh[x][y][z].normals = CalculateHardNormals(vertices[x][y][z], triangles[x][y][z]);
                        meshFilters[x][y][z].mesh = mesh[x][y][z];

                    }
                }
            }



            if(mesh != null){
                for (int x = mesh.Length-1; x >= 0; x--)
                {
                    for (int y = mesh.Length-1; y >= 0; y--)
                    {
                        for (int z = mesh.Length-1; z >= 0; z--)
                        {
                            Destroy(mesh[x][y][z]);
                        }
                    }
                }
            }
            


}



// Метод для вычисления жестких нормалей для каждой вершины
private Vector3[] CalculateHardNormals(List<Vector3> vertices, List<int> triangles)
{
    Vector3[] normals = new Vector3[vertices.Count];



    for (int i = 0; i < triangles.Count; i += 3)
    {
        // Индексы вершин треугольника
        int i0 = triangles[i];
        int i1 = triangles[i + 1];
        int i2 = triangles[i + 2];

        // Нормаль для текущего треугольника
        Vector3 v1 = vertices[i1] - vertices[i0];
        Vector3 v2 = vertices[i2] - vertices[i0];
        Vector3 normal = Vector3.Cross(v1, v2).normalized;

        // Применяем нормаль ко всем вершинам этого треугольника
        normals[i0] = normal;
        normals[i1] = normal;
        normals[i2] = normal;
    }

    return normals;
}





public static Color32[][][][] colors322;
public static Vector3[][][][] vertices2;

public static Vector3 front = new Vector3(0,0,-1); ///preVoxel sides
public static Vector3 right = new Vector3(1,0,0);
public static Vector3 back = new Vector3(0,0,1);
public static Vector3 left = new Vector3(-1,0,0);
public static Vector3 top = new Vector3(0,1,0);
public static Vector3 bottom = new Vector3(0,-1,0);
public static int off = 0;

public static Vector3 v0 = new Vector3(0,0,0); //stock vertex positions
public static Vector3 v1 = new Vector3(1,0,0);
public static Vector3 v2 = new Vector3(1,1,0);
public static Vector3 v3 = new Vector3(0,1,0);
public static Vector3 v4 = new Vector3(1,0,0);



public void PreInitializeVoxelSelection(){
    int chkN = OBJConverter.chunkN;

    colors322 = new Color32[chkN][][][];
    vertices2 = new Vector3[chkN][][][];
    for (int x = 0; x < chkN; x++)
    {
        colors322[x] = new Color32[chkN][][];
        vertices2[x] = new Vector3[chkN][][];
        for (int y = 0; y < chkN; y++)
        {
            colors322[x][y] = new Color32[chkN][];
            vertices2[x][y] = new Vector3[chkN][];
            for (int z = 0; z < chkN; z++)
            {
                colors322[x][y][z] = meshFilters[x][y][z].mesh.colors32;
                vertices2[x][y][z] = meshFilters[x][y][z].mesh.vertices;
            }
        }
    }
}



public void InitializeVoxelSelection(){
        int chkN = OBJConverter.chunkN;
        int nrmChk = OBJConverter.normalChunk;

        int n = OBJConverter.n;

        Vector3Int selectionStart1 = selectionStart;
        Vector3Int selectionEnd1 = selectionEnd;

        int ss1X = selectionStart1[0];
        int ss1Y = selectionStart1[1];
        int ss1Z = selectionStart1[2];

        int se1X = selectionEnd1[0];
        int se1Y = selectionEnd1[1];
        int se1Z = selectionEnd1[2];

        if(ss1X > 0){ ss1X -= 1; }
        if(ss1Y > 0){ ss1Y -= 1; }
        if(ss1Z > 0){ ss1Z -= 1; }

        if(se1X < n-1){ se1X += 1; }
        if(se1Y < n-1){ se1Y += 1; }
        if(se1Z < n-1){ se1Z += 1; }

        // Определяем чанки начала и конца
        int startChunkX = ss1X / nrmChk;
        int startChunkY = ss1Y / nrmChk;
        int startChunkZ = ss1Z / nrmChk;

        int endChunkX = se1X / nrmChk;
        int endChunkY = se1Y / nrmChk;
        int endChunkZ = se1Z / nrmChk;


    for (int x = startChunkX; x <= endChunkX; x++)
    {
        for (int y = startChunkY; y <= endChunkY; y++)
        {
            for (int z = startChunkZ; z <= endChunkZ; z++)
            {
                colors322[x][y][z] = meshFilters[x][y][z].mesh.colors32;
                vertices2[x][y][z] = meshFilters[x][y][z].mesh.vertices;
            }
        }
    }

    lastVox = new Vector3(-12,-12,-12);
}



public void SelectVoxel(Vector3 vox, Vector3 pre, Color32 col, Vector3 voxO, Vector3 preO, Color32 colO){
    Color32 oldCol = new Color32(255,50,50,255);
    Color32 oldColO = new Color32(255,50,50,255);
    Color32 buff = new Color32(255,50,50,255);
    int buff2 = 0;
    bool foundFirst = false;
    bool foundSecond = false;
    int nrmChk = OBJConverter.normalChunk;



    int convX = (int)Math.Ceiling( (float)((((float)vox[0]+1f)/nrmChk)-1f) );
    int convY = (int)Math.Ceiling( (float)((((float)vox[1]+1f)/nrmChk)-1f) );
    int convZ = (int)Math.Ceiling( (float)((((float)vox[2]+1f)/nrmChk)-1f) );

    int convXO = (int)Math.Ceiling( (float)((((float)voxO[0]+1f)/nrmChk)-1f) );
    int convYO = (int)Math.Ceiling( (float)((((float)voxO[1]+1f)/nrmChk)-1f) );
    int convZO = (int)Math.Ceiling( (float)((((float)voxO[2]+1f)/nrmChk)-1f) );





    if((convX!=convXO || convY!=convYO || convZ!=convZO) && convXO>=0 && convYO>=0 && convZO>=0){
        for(int v = 0; v < vertices2[convXO][convYO][convZO].Length; v++){
                    if(foundFirst && foundSecond){
                        break;
                    }else if(!foundSecond && vertices2[convXO][convYO][convZO][v] == voxO){//закрашиваем предыдущий воксель с другого мэша
                        if(vertices2[convXO][convYO][convZO][v + 0] == (voxO + v0) && 
                            vertices2[convXO][convYO][convZO][v + 1] == (voxO + v1) && 
                            vertices2[convXO][convYO][convZO][v + 2] == (voxO + v2) && 
                            vertices2[convXO][convYO][convZO][v + 3] == (voxO + v3) && 
                            vertices2[convXO][convYO][convZO][v + 4] == (voxO + v4))
                        {
                            
                            Vector3 offsetO = preO - voxO;

                            if(offsetO == front){
                                off = 0;
                            }else if(offsetO == right){
                                off = 4;
                            }else if(offsetO == back){
                                off = 8;

                            }else if(offsetO == left){
                                off = 12;
                            }else if(offsetO == top){
                                off = 16;
                            }else if(offsetO == bottom){
                                off = 20;
                            }

                            for(int i = (0 + off); i < (4 + off); i++){
                                buff = colors322[convXO][convYO][convZO][v + i];
                                buff2 = v + i;
                                colors322[convXO][convYO][convZO][v + i] = colO;
                            }
                            foundSecond = true;
                        }
                    }
        }
        meshFilters[convXO][convYO][convZO].mesh.colors32 = colors322[convXO][convYO][convZO];

    }



        if(vox != new Vector3(-123,-123,-123)){
            if(vox != voxO){
                for(int v = 0; v < vertices2[convX][convY][convZ].Length; v++){
                    if(foundFirst && foundSecond){
                        break;
                    }else if(!foundSecond && vertices2[convX][convY][convZ][v] == voxO){//закрашиваем предыдущий воксель
                        if(vertices2[convX][convY][convZ][v + 0] == (voxO + v0) && 
                            vertices2[convX][convY][convZ][v + 1] == (voxO + v1) && 
                            vertices2[convX][convY][convZ][v + 2] == (voxO + v2) && 
                            vertices2[convX][convY][convZ][v + 3] == (voxO + v3) && 
                            vertices2[convX][convY][convZ][v + 4] == (voxO + v4))
                        {
                            
                            Vector3 offsetO = preO - voxO;

                            if(offsetO == front){
                                off = 0;
                            }else if(offsetO == right){
                                off = 4;
                            }else if(offsetO == back){
                                off = 8;

                            }else if(offsetO == left){
                                off = 12;
                            }else if(offsetO == top){
                                off = 16;
                            }else if(offsetO == bottom){
                                off = 20;
                            }

                            for(int i = (0 + off); i < (4 + off); i++){colors322[convX][convY][convZ][v + i] = colO;}
                            foundSecond = true;
                        }
                        
                    }else if(!foundFirst && vertices2[convX][convY][convZ][v] == vox){
                        if(vertices2[convX][convY][convZ][v + 0] == (vox + v0) && 
                            vertices2[convX][convY][convZ][v + 1] == (vox + v1) && 
                            vertices2[convX][convY][convZ][v + 2] == (vox + v2) && 
                            vertices2[convX][convY][convZ][v + 3] == (vox + v3) && 
                            vertices2[convX][convY][convZ][v + 4] == (vox + v4))
                        {
                            
                            Vector3 offset = pre - vox;

                            if(offset == front){
                                off = 0;
                            }else if(offset == right){
                                off = 4;
                            }else if(offset == back){
                                off = 8;

                            }else if(offset == left){
                                off = 12;
                            }else if(offset == top){
                                off = 16;
                            }else if(offset == bottom){
                                off = 20;
                            }

                            oldCol = colors322[convX][convY][convZ][v];

                            for(int i = (0 + off); i < (4 + off); i++){colors322[convX][convY][convZ][v + i] = col;}
                            foundFirst = true;
                        }
                        
                    }
                }
            }else{
                for(int v = 0; v < vertices2[convX][convY][convZ].Length; v++){
                    if(vertices2[convX][convY][convZ][v] == vox){
                        if(vertices2[convX][convY][convZ][v + 0] == (vox + v0) && 
                            vertices2[convX][convY][convZ][v + 1] == (vox + v1) && 
                            vertices2[convX][convY][convZ][v + 2] == (vox + v2) && 
                            vertices2[convX][convY][convZ][v + 3] == (vox + v3) && 
                            vertices2[convX][convY][convZ][v + 4] == (vox + v4))
                        {
                            
                            Vector3 offsetO = preO - voxO;
                            Vector3 offset = pre - vox;

                            if(offsetO == front){
                                off = 0;
                            }else if(offsetO == right){
                                off = 4;
                            }else if(offsetO == back){
                                off = 8;

                            }else if(offsetO == left){
                                off = 12;
                            }else if(offsetO == top){
                                off = 16;
                            }else if(offsetO == bottom){
                                off = 20;
                            }
                            for(int i = (0 + off); i < (4 + off); i++){colors322[convX][convY][convZ][v + i] = colO;}


                            if(offset == front){
                                off = 0;
                            }else if(offset == right){
                                off = 4;
                            }else if(offset == back){
                                off = 8;

                            }else if(offset == left){
                                off = 12;
                            }else if(offset == top){
                                off = 16;
                            }else if(offset == bottom){
                                off = 20;
                            }

                            oldCol = colors322[convX][convY][convZ][v];

                            for(int i = (0 + off); i < (4 + off); i++){colors322[convX][convY][convZ][v + i] = col;}


                            break;
                        }
                        
                    }
                }
            }
        meshFilters[convX][convY][convZ].mesh.colors32 = colors322[convX][convY][convZ];
        }else{//если переходишь за края
            for(int v = 0; v < vertices2[convXO][convYO][convZO].Length; v++){
                    if(vertices2[convXO][convYO][convZO][v] == voxO){
                        if(vertices2[convXO][convYO][convZO][v + 0] == (voxO + v0) && 
                            vertices2[convXO][convYO][convZO][v + 1] == (voxO + v1) && 
                            vertices2[convXO][convYO][convZO][v + 2] == (voxO + v2) && 
                            vertices2[convXO][convYO][convZO][v + 3] == (voxO + v3) && 
                            vertices2[convXO][convYO][convZO][v + 4] == (voxO + v4))
                        {
                            
                            Vector3 offsetO = preO - voxO;

                            if(offsetO == front){
                                off = 0;
                            }else if(offsetO == right){
                                off = 4;
                            }else if(offsetO == back){
                                off = 8;

                            }else if(offsetO == left){
                                off = 12;
                            }else if(offsetO == top){
                                off = 16;
                            }else if(offsetO == bottom){
                                off = 20;
                            }

                            oldCol = colO;

                            for(int i = (0 + off); i < (4 + off); i++){colors322[convXO][convYO][convZO][v + i] = colO;}
                            vox = (voxO + new Vector3(-12,-12,-12)*0);
                            pre = (preO + new Vector3(-12,-12,-12)*0);


                            break;
                        }
                        
                    }
                }
                meshFilters[convXO][convYO][convZO].mesh.colors32 = colors322[convXO][convYO][convZO];
        }



    lastColor32 = oldCol;
    lastVox = vox;
    lastPre = pre;
    }
}


