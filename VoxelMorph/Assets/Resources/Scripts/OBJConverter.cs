using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SFB;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Diagnostics;
using System.IO;
using System.Globalization;

public class OBJConverter : MonoBehaviour 
{
    public Material materialP1;
    public GameObject PlayGround;
    public static Material StandardMaterial;
    public GameObject text;
    public UnityEngine.Object OBJScript;
    public GameObject CamCont;
    public static GameObject LastBtn;
    public static GameObject LastMenuBtn;

    public string objContent = "";
    public string mtlContent = "";
    public string voxContent = "";

    float[] verts = new float[100];
    float[] faces = new float[100];

    Vector3 camPos;

    public static int n = 9; //весь размер модели
    public static int chunkN = 3; // количество чанков
    public static int normalChunk = 3; //количество блоков в обычном чанке
    public static int lastChunk = 3; //количество блоков в последнем чанке

    public static Color32[][][] model;
    public static bool[][][] selectionModel; //are cubes selected?

    public static List<Color32[][][]> modelHistory = new List<Color32[][][]>();
    public static List<Vector3Int> modelHistorySelectionStart = new List<Vector3Int>();
    public static List<Vector3Int> modelHistorySelectionEnd = new List<Vector3Int>();
    public static int historyPosition = 1;

    public static Color32 SelectedBack = new Color32(148,148,148,255);
    public static Color32 HoverBack = new Color32(115,115,115,255);
    public static Color32 ActiveBack = new Color32(80,80,80,255);
    public static Color32 PassiveBack = new Color32(39,39,39,255);
    

    public static Color32 SelectedHighlight = new Color32(245,245,245,255);
    public static Color32 HoverHighlight = new Color32(230,230,230,255);
    public static Color32 ActiveHighlight = new Color32(160,160,160,255);
    public static Color32 PassiveHighlight = new Color32(100,100,100,255);

    public int test;
    public static List<GameObject> Parts = new List<GameObject>();

    public static bool update1 = true;



    public void DeselectAllButtons(){
        GameObject.Find("Import").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("Export").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("Voxelize").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("Add").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("Remove").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("Invert").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("Paint").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("LinearGradient").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("RadialGradient").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("Select").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("DropSelection").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("InvertSelection").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("ClearModel").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("Exit").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("MainMenuBtn").GetComponent<ButtonIconColorsMenus>().Deselect();
        GameObject.Find("RenderMenuBtn").GetComponent<ButtonIconColorsMenus>().Deselect();
        GameObject.Find("MoveMenuBtn").GetComponent<ButtonIconColorsMenus>().Deselect();
        GameObject.Find("HideUIBtn").GetComponent<ButtonIconColorsMenus>().Deselect();

        GameObject.Find("FlipX").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("FlipY").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("FlipZ").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("RotateX").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("RotateY").GetComponent<ButtonIconColors>().Deselect();
        GameObject.Find("RotateZ").GetComponent<ButtonIconColors>().Deselect();

        GameObject.Find("Move").GetComponent<ButtonIconColors>().Deselect();
    }





    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; ///// FRAME RATE CAP /////


        DeselectAllButtons();
        AdjustUI();
        LastBtn = GameObject.Find("Remove");
        LastBtn.GetComponent<ButtonIconColors>().Select();

        LastMenuBtn = GameObject.Find("MainMenuBtn");
        LastMenuBtn.GetComponent<ButtonIconColorsMenus>().Select();

        GameObject.Find("Main Camera").GetComponent<Functions>().ShowMainMenu();

        initializeOBJConverter();

        if(Directory.Exists("VoxelMorph_Data/SavedState")){
            ClickImport(true);
        }

    }





    public void initializeMask(){
        GameObject.Find("EditorBorder").GetComponent<Transform>().localScale = new Vector3(n, n, n);
        GameObject.Find("EditorBorder").GetComponent<Transform>().position = new Vector3((128.5f+n/2), (128.5f+n/2), (128.5f+n/2));
        if(n % 2 == 0){GameObject.Find("EditorBorder").GetComponent<Transform>().position -= new Vector3(0.5f, 0.5f, 0.5f);}

        GameObject.Find("Mask").GetComponent<Transform>().position = new Vector3((128.0f + n/2.0f), (128.0f + n/2.0f), (128.0f + n/2.0f));
        GameObject.Find("Mask").GetComponent<Transform>().localScale = new Vector3((n+0.01f), (n+0.01f), (n+0.01f));

        GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetVector("_MaskMin", new Vector4((127.99f), (127.99f), (127.99f), 0));
        GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1.SetVector("_MaskMax", new Vector4((128.01f + n), (128.01f + n), (128.01f + n), 0));

    }





    public void initializeOBJConverter(){
        PlayGround = GameObject.Find("PlayGround");
        StandardMaterial = Resources.Load("Materials/standard", typeof(Material)) as Material;
        OBJScript = Resources.Load("Scripts/object");
        initializeMask();

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);



        model = new Color32[n][][];
        selectionModel = new bool[n][][];
        for (int i = 0; i < model.Length; i++)
        {
            model[i] = new Color32[n][];
            selectionModel[i] = new bool[n][];
            for (int j = 0; j < model[i].Length; j++)
            {
                model[i][j] = new Color32[n];
                selectionModel[i][j] = new bool[n];
                for (int k = 0; k < model[i][j].Length; k++) {
                    model[i][j][k] = new Color32(102,102,102,255);
                    selectionModel[i][j][k] = true;
                }
            }
        }



        modelHistory.Add(Functions.CloneJaggedArray(model));
        modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().PreInitializeRaycast();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().PreInitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
        GameObject.Find("InputField").GetComponent<TMP_InputField>().text = n.ToString();
    }





public void ClickExport(){
    string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "model", "obj");
        if (!string.IsNullOrEmpty(path))
        {
            string newPath = path.Split(".")[0];
            OBJToString();
            FileWriter.WriteFile(newPath, objContent, mtlContent, voxContent);//"model1"
        }
    
}

public void ClickImport(bool IsStarting){

    if(!IsStarting){
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "vox", false);
        if(paths.Length > 0 ){
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri));
        }
    }else{
        string path = Path.Combine(Application.dataPath, "SavedState\\SavedState.vox");
        if (File.Exists(path))
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(path).AbsoluteUri));
        }
        else
        {
            UnityEngine.Debug.LogError("Файл не найден: " + path);
        }
    }
}

private IEnumerator OutputRoutineOpen(string url){
    UnityWebRequest www = UnityWebRequest.Get(url);
    yield return www.SendWebRequest();

    if(www.result != UnityWebRequest.Result.Success){
        UnityEngine.Debug.Log("error");
    }else{
        string[] importedContent = www.downloadHandler.text.Split("\n");
        NumberInput.calcNumbers(int.Parse(importedContent[0]));

        Transform PG = GameObject.Find("PlayGround").GetComponent<Transform>();
        while (PG.childCount > 0) {
            DestroyImmediate(PG.GetChild(0).gameObject);
        }



        model = new Color32[n][][];
        selectionModel = new bool[n][][];
        for (int i = 0; i < model.Length; i++)
        {
            model[i] = new Color32[n][];
            selectionModel[i] = new bool[n][];
            for (int j = 0; j < model[i].Length; j++)
            {
                model[i][j] = new Color32[n];
                selectionModel[i][j] = new bool[n];
                for (int k = 0; k < model[i][j].Length; k++) {
                    selectionModel[i][j][k] = true;

                    string[] a = importedContent[(i*model.Length*model[i].Length+j*model[i].Length+k) +1].Split(" ");
                    if(a[0] != ""){
                        model[i][j][k] = new Color32((byte)(float.Parse(a[0])*255),(byte)(float.Parse(a[1])*255),(byte)(float.Parse(a[2])*255),255);//"(" + a[0] + ":" + a[1] + ":"  + a[2] + ")";
                    }else{
                        model[i][j][k].a = 0;
                    }

                }
            }
        }



        VoxelMeshGenerator.changedSize = true;
        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        modelHistory.Add(Functions.CloneJaggedArray(model));
        modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        GameObject.Find("MoveForward").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveForwardIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;

        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().PreInitializeRaycast();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().PreInitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();
        GameObject.Find("InputField").GetComponent<TMP_InputField>().text = n.ToString();

        initializeMask();
    }
}





public void ClickSaveState(){
    voxContent = n.ToString() + "\n";

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            for (int k = 0; k < n; k++)
            {
                if (model[i][j][k].a != 0) {
                    string[] cols = {((float)model[i][j][k].r/255).ToString(), ((float)model[i][j][k].g/255).ToString(), ((float)model[i][j][k].b/255).ToString()};//model[i][j][k].Split(";"); 
                    voxContent += cols[0] + " " + cols[1] + " " + cols[2] + "\n";
                }else{
                    voxContent += "\n";
                }
            }
        }
    }
    FileWriter.SaveState(voxContent); 
}





public void AdjustUI(){
    RectTransform[] uiElements = GameObject.FindObjectsOfType<RectTransform>();

    foreach (RectTransform rect in uiElements)
    {
        if (rect.gameObject.name == "MenuScaleBox")
        {
            rect.localScale *= 1.5f;
        }
    }
}





public void ClickVoxelize(){
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if(paths.Length > 0 ){
            StartCoroutine(OutputRoutineOpen2(new System.Uri(paths[0]).AbsoluteUri));
        }
}

private IEnumerator OutputRoutineOpen2(string url){
    UnityWebRequest www = UnityWebRequest.Get(url);
    yield return www.SendWebRequest();

    if(www.result != UnityWebRequest.Result.Success){
        UnityEngine.Debug.Log("error");
    }else{
        string importedContent = www.downloadHandler.text;
        List<Vector3> vertices = LoadVertices(importedContent);
        Vector3[] bounds = GetModelBounds(vertices);
        HashSet<Vector3Int> calculatedVoxels = ConvertOBJToVoxels(importedContent, n);
        calculatedVoxels = ErodeVoxels(calculatedVoxels, 4);
        Vector3Int minBounds = new Vector3Int(0, 0, 0); // Минимальная граница
        Vector3Int maxBounds = new Vector3Int(n - 1, n - 1, n - 1); // Максимальная граница (n - размер сетки)
        calculatedVoxels = DilateVoxels(calculatedVoxels, minBounds, maxBounds);
        calculatedVoxels = BlurVoxels(calculatedVoxels);
        Functions.DeleteModel();
        ImportCalculatedVoxels(calculatedVoxels);

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);

        modelHistory.Clear();
        modelHistorySelectionStart.Clear();
        modelHistorySelectionEnd.Clear();
        historyPosition = 1;
        Functions.RemoveRestIndecies();

        modelHistory.Add(Functions.CloneJaggedArray(model));
        modelHistorySelectionStart.Add(VoxelMeshGenerator.selectionStart);
        modelHistorySelectionEnd.Add(VoxelMeshGenerator.selectionEnd);

        GameObject.Find("MoveBack").GetComponent<Button>().GetComponent<RawImage>().color = OBJConverter.PassiveBack;
        GameObject.Find("MoveBackIcon").GetComponent<RawImage>().color = OBJConverter.PassiveHighlight;
        GameObject.Find("MoveBack").GetComponent<Button>().interactable = false;

        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().GenerateMesh();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelMeshGenerator>().InitializeVoxelSelection();
        GameObject.Find("VoxelMesh1").GetComponent<VoxelRaycast>().InitializeRaycast();

        initializeMask();

        VoxelMeshGenerator.selectionStart = new Vector3Int(0,0,0);
        VoxelMeshGenerator.selectionEnd = new Vector3Int(n-1,n-1,n-1);
        MainBehaviour.clicked = false;
    }
}





public static List<Vector3> LoadVertices(string content)
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (string line in content.Split("\n"))
        {
            if (line.StartsWith("v ")) // Только вершины
            {
                string[] parts = line.Split(' ');
                float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                vertices.Add(new Vector3(x, y, z));
            }
        }
        return vertices;
    }

public static Vector3[] GetModelBounds(List<Vector3> vertices)
{
    Vector3 min = vertices[0], max = vertices[0];
    foreach (var v in vertices)
    {
        min = Vector3.Min(min, v);
        max = Vector3.Max(max, v);
    }
    Vector3[] a = {min, max};
    return a;
}

HashSet<Vector3Int> Voxelize(List<Vector3> vertices, Vector3 voxelOffset, Vector3 voxelCoeff, int n)
{
    HashSet<Vector3Int> voxels = new HashSet<Vector3Int>();

    float coeff = Mathf.Max(Mathf.Max(voxelCoeff.x, voxelCoeff.y), voxelCoeff.z);

    UnityEngine.Debug.Log("off: "+voxelOffset);
    UnityEngine.Debug.Log("coeff: "+coeff);
    
    foreach (var v in vertices)
    {
        Vector3Int voxelCoord = new Vector3Int(
            Mathf.RoundToInt((v.x - voxelOffset.x) / coeff * (n-1)),
            Mathf.RoundToInt((v.y - voxelOffset.y) / coeff * (n-1)),
            Mathf.RoundToInt((v.z - voxelOffset.z) / coeff * (n-1))
        );
        voxels.Add(voxelCoord);
    }
    return voxels;
}

public void ImportCalculatedVoxels(HashSet<Vector3Int> calculatedVoxels){
    foreach (var v in calculatedVoxels)
    {
        model[v.x][v.y][v.z] = new Color32(102,102,102,255);
    }
}





public static HashSet<Vector3Int> ConvertOBJToVoxels(string content, int n)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int[]> triangles = new List<int[]>();

        // 1️⃣ Парсим вершины и треугольники
        string[] lines = content.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Trim().Split(' ');
            if (parts.Length < 4) continue;

            if (line.StartsWith("v ")) // Вершины
            {
                float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                vertices.Add(new Vector3(x, y, z));
            }
            else if (line.StartsWith("f ")) // Треугольники
            {
                int v1 = int.Parse(parts[1].Split('/')[0]) - 1;
                int v2 = int.Parse(parts[2].Split('/')[0]) - 1;
                int v3 = int.Parse(parts[3].Split('/')[0]) - 1;
                triangles.Add(new int[] { v1, v2, v3 });
            }
        }

        if (vertices.Count == 0 || triangles.Count == 0) return new HashSet<Vector3Int>();

        // 2️⃣ Определяем границы модели
        Vector3 min = vertices[0], max = vertices[0];
        foreach (var v in vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        Vector3 size = max - min;
        float maxSize = Mathf.Max(size.x, size.y, size.z);
        float voxelSize = maxSize / (n - 1);

        // Смещаем модель так, чтобы min была в (0,0,0)
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = (vertices[i] - min) / voxelSize;
        }

        HashSet<Vector3Int> voxels = new HashSet<Vector3Int>();

        // 3️⃣ Вокселизация треугольников
        foreach (var tri in triangles)
        {
            Vector3 v0 = vertices[tri[0]];
            Vector3 v1 = vertices[tri[1]];
            Vector3 v2 = vertices[tri[2]];
            RasterizeTriangle(v0, v1, v2, voxels);
        }

        return voxels;
    }

    // 4️⃣ Заполнение вокселей внутри треугольника
    private static void RasterizeTriangle(Vector3 v0, Vector3 v1, Vector3 v2, HashSet<Vector3Int> voxels)
    {
        Bounds bounds = new Bounds(v0, Vector3.zero);
        bounds.Encapsulate(v1);
        bounds.Encapsulate(v2);

        Vector3Int min = new Vector3Int(
            Mathf.FloorToInt(bounds.min.x),
            Mathf.FloorToInt(bounds.min.y),
            Mathf.FloorToInt(bounds.min.z)
        );

        Vector3Int max = new Vector3Int(
            Mathf.CeilToInt(bounds.max.x),
            Mathf.CeilToInt(bounds.max.y),
            Mathf.CeilToInt(bounds.max.z)
        );

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    Vector3 voxelCenter = new Vector3(x, y, z);
                    if (IsPointInTriangle(voxelCenter, v0, v1, v2))
                    {
                        voxels.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }

    // 5️⃣ Проверка попадания точки в треугольник
    private static bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

public static HashSet<Vector3Int> SmoothVoxels(HashSet<Vector3Int> voxels, int minNeighbors = 3)
{
    HashSet<Vector3Int> smoothed = new HashSet<Vector3Int>();

    foreach (Vector3Int voxel in voxels)
    {
        int neighbors = 0;
        foreach (Vector3Int offset in GetNeighborOffsets()) // См. метод ниже
        {
            if (voxels.Contains(voxel + offset)) neighbors++;
        }

        if (neighbors >= minNeighbors)
        {
            smoothed.Add(voxel); // Оставляем только те, у которых есть достаточно соседей
        }
    }
    return smoothed;
}

private static List<Vector3Int> GetNeighborOffsets()
{
    return new List<Vector3Int>
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
    };
}

public static HashSet<Vector3Int> ErodeVoxels(HashSet<Vector3Int> voxels, int minNeighbors = 4)
{
    HashSet<Vector3Int> eroded = new HashSet<Vector3Int>();

    foreach (Vector3Int voxel in voxels)
    {
        int neighbors = 0;
        foreach (Vector3Int offset in GetNeighborOffsets())
        {
            if (voxels.Contains(voxel + offset)) neighbors++;
        }

        if (neighbors >= minNeighbors)
        {
            eroded.Add(voxel);
        }
    }
    return eroded;
}

public static HashSet<Vector3Int> DilateVoxels(HashSet<Vector3Int> voxels, Vector3Int boundsMin, Vector3Int boundsMax)
{
    HashSet<Vector3Int> dilated = new HashSet<Vector3Int>(voxels);

    foreach (Vector3Int voxel in voxels)
    {
        foreach (Vector3Int offset in GetNeighborOffsets())
        {
            Vector3Int newVoxel = voxel + offset;
            
            // Проверяем, что новый воксель в пределах границ
            if (!voxels.Contains(newVoxel) &&
                newVoxel.x >= boundsMin.x && newVoxel.x <= boundsMax.x &&
                newVoxel.y >= boundsMin.y && newVoxel.y <= boundsMax.y &&
                newVoxel.z >= boundsMin.z && newVoxel.z <= boundsMax.z)
            {
                dilated.Add(newVoxel);
            }
        }
    }
    return dilated;
}

public static HashSet<Vector3Int> BlurVoxels(HashSet<Vector3Int> voxels)
{
    Dictionary<Vector3Int, int> neighborCounts = new Dictionary<Vector3Int, int>();

    foreach (Vector3Int voxel in voxels)
    {
        foreach (Vector3Int offset in GetNeighborOffsets())
        {
            Vector3Int neighbor = voxel + offset;
            if (!voxels.Contains(neighbor))
            {
                if (!neighborCounts.ContainsKey(neighbor))
                    neighborCounts[neighbor] = 0;
                neighborCounts[neighbor]++;
            }
        }
    }

    HashSet<Vector3Int> blurred = new HashSet<Vector3Int>(voxels);
    foreach (var kvp in neighborCounts)
    {
        if (kvp.Value >= 3) // Если соседей достаточно, добавляем воксель
            blurred.Add(kvp.Key);
    }
    
    return blurred;
}










void OBJToString() {

    objContent = "mtllib model1.mtl\n\n";
    mtlContent = "";
    voxContent = n.ToString() + "\n";
    int nn = 0;

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            for (int k = 0; k < n; k++)
            {
                if (model[i][j][k].a != 0) {
                    string[] cols = {((float)model[i][j][k].r/255).ToString(), ((float)model[i][j][k].g/255).ToString(), ((float)model[i][j][k].b/255).ToString()};//model[i][j][k].Split(";"); 
                    string finCol = "("+cols[0]+":"+cols[1]+":"+cols[2]+")";

                    objContent += "" +
                            "v " + (0.0 + i) + " " + (0.0 + j) + " " + (0.0 + k) + "\n" +
                            "v " + (1.0 + i) + " " + (0.0 + j) + " " + (0.0 + k) + "\n" +
                            "v " + (0.0 + i) + " " + (1.0 + j) + " " + (0.0 + k) + "\n" +
                            "v " + (1.0 + i) + " " + (1.0 + j) + " " + (0.0 + k) + "\n" +

                            "v " + (0.0 + i) + " " + (0.0 + j) + " " + (1.0 + k) + "\n" +
                            "v " + (1.0 + i) + " " + (0.0 + j) + " " + (1.0 + k) + "\n" +
                            "v " + (0.0 + i) + " " + (1.0 + j) + " " + (1.0 + k) + "\n" +
                            "v " + (1.0 + i) + " " + (1.0 + j) + " " + (1.0 + k) + "\n" +

                            "usemtl " + finCol + "\n";


                    if (!((k - 1) >= 0 && model[i][j][k - 1].a != 0)) //k-1 = face
                    {
                        objContent += "" +
                        "f " + (1 + 8 * nn) + " " + (2 + 8 * nn) + " " + (3 + 8 * nn) + "\n" +//face
                        "f " + (2 + 8 * nn) + " " + (3 + 8 * nn) + " " + (4 + 8 * nn) + "\n";
                    }
                    if (!((j + 1) < model.Length && model[i][j + 1][k].a != 0)) //j+1 = top
                    {
                        objContent += "" +
                            "f " + (3 + 8 * nn) + " " + (4 + 8 * nn) + " " + (7 + 8 * nn) + "\n" +//top
                            "f " + (4 + 8 * nn) + " " + (7 + 8 * nn) + " " + (8 + 8 * nn) + "\n";
                    }
                    if (!((k + 1) < model.Length && model[i][j][k + 1].a != 0)) //k+1 = back
                    {
                        objContent += "" +
                        "f " + (7 + 8 * nn) + " " + (8 + 8 * nn) + " " + (5 + 8 * nn) + "\n" +//back
                        "f " + (8 + 8 * nn) + " " + (5 + 8 * nn) + " " + (6 + 8 * nn) + "\n";
                    }
                    if (!((j - 1) >= 0 && model[i][j - 1][k].a != 0)) //j-1 = bottom
                    {
                        objContent += "" +
                            "f " + (5 + 8 * nn) + " " + (6 + 8 * nn) + " " + (1 + 8 * nn) + "\n" +//bottom
                            "f " + (6 + 8 * nn) + " " + (1 + 8 * nn) + " " + (2 + 8 * nn) + "\n";
                    }
                    if (!((i - 1) >= 0 && model[i - 1][j][k].a != 0)) //i-1 = right
                    {
                        objContent += "" +
                            "f " + (1 + 8 * nn) + " " + (3 + 8 * nn) + " " + (5 + 8 * nn) + "\n" +//right
                            "f " + (3 + 8 * nn) + " " + (5 + 8 * nn) + " " + (7 + 8 * nn) + "\n";
                    }
                    if (!((i + 1) < model.Length && model[i + 1][j][k].a != 0)) //i+1 = left
                    {
                        objContent += "" +
                            "f " + (2 + 8 * nn) + " " + (4 + 8 * nn) + " " + (6 + 8 * nn) + "\n" +//left
                            "f " + (4 + 8 * nn) + " " + (6 + 8 * nn) + " " + (8 + 8 * nn) + "\n";
                    }
                    objContent += "\n";

                    //MTL
                    if (!mtlContent.Contains(finCol)) { 
                        
                    string Color32 = "Kd " + cols[0] + " " + cols[1] + " " + cols[2] + "\n\n";
                    Color32 = Color32.Replace(',','.');

                    mtlContent += "" +
                        "newmtl " + finCol + "\n" + //material name
                        Color32; //material Color32
                    }
                    nn += 1;

                    voxContent += cols[0] + " " + cols[1] + " " + cols[2] + "\n";
                }else{
                    voxContent += "\n";
                }

                
            }
        }
    }
}
}


