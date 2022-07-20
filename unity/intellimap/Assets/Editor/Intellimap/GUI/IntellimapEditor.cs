using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/*
 * This is the custom editor window which contains all UI elements and triggers the WFC.
 */
public class IntellimapEditor : EditorWindow {
    public static bool repaint;
    public static int startingSpace;

    private Vector2 scrollPos;

    private string baseDataPath;
    private TilemapStats tilemapStats;

    private Matrix matrix;
    private Histogram histogram;

    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;

    private bool useSeed;
    private int seed;

    #nullable enable
    WFCRun? wfc;
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    public void OnEnable() {
        // This is needed to be able to register the MouseMove event
        wantsMouseMove = true;

        // can be used by components to repaint the window
        repaint = false;

        // space used by every component
        startingSpace = 15;

        scrollPos = new Vector2(0, 0);

        targetWidth = 0;
        targetHeight = 0;

        baseDataPath = "";

        useSeed = false;
        seed = 0;

        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        matrix = new Matrix(10, foregroundColor, backgroundColor, Color.grey, Color.white, 0.9f, 25, this);
        
        histogram = new Histogram(10);
    }

    // Window GUI code
    private void OnGUI() {
        Vector2 newScrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (newScrollPos.x != scrollPos.x || newScrollPos.y != scrollPos.y) {
            if (!IntellimapInput.CtrlHeld()) {
                scrollPos = newScrollPos;
            }
        }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(startingSpace);

                baseDataPath = EditorGUILayout.TextField("Base data:", baseDataPath);
                bool openBaseDataFolder = GUILayout.Button("...", GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();

            if (openBaseDataFolder) {
                baseDataPath = EditorUtility.OpenFolderPanel("Open base data folder", Application.dataPath, "");
            }

            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(startingSpace);
                EditorGUILayout.LabelField("Current base data path:", baseDataPath);
            EditorGUILayout.EndHorizontal();

            GUIUtil.HorizontalLine(Color.grey);

            if (GUIUtil.CenteredButton("Analyze Base Data", 170, 25)) {
                AnalyzeBaseData();
            }

            matrix.Show();

            GUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

            histogram.Show();

            GUIUtil.HorizontalLine(Color.grey);

            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(startingSpace);

                EditorGUILayout.BeginVertical();
                    targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap:", targetTilemap, typeof(Tilemap), true);
                    targetWidth = EditorGUILayout.IntField("Width:", targetWidth);
                    targetHeight = EditorGUILayout.IntField("Height:", targetHeight);
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUIUtil.HorizontalLine(Color.grey);    

            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(startingSpace);

                EditorGUILayout.BeginVertical();
                    useSeed = EditorGUILayout.Toggle("Use own seed", useSeed);

                    EditorGUILayout.BeginHorizontal();

                    if (!useSeed)
                        GUI.enabled = false;

                    seed = EditorGUILayout.IntField("Seed:", seed);
                    
                    if (!useSeed)
                        GUI.enabled = true;

                    if (GUILayout.Button("Copy", GUILayout.Width(45))) {
                        EditorGUIUtility.systemCopyBuffer = seed.ToString();
                        ShowNotification(new GUIContent("Copied to clipboard"), 0.5f);
                    }

                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Generate Entire Map", GUILayout.Width(140), GUILayout.Height(25))) {
                        GenerateEntireMapButtonPressed();
                    }

                    if (GUILayout.Button("Toggle Animated Generation", GUILayout.Width(185), GUILayout.Height(25))) {
                        ToggleAnimatedGenerationButtonPressed();
                    }
                    if (GUILayout.RepeatButton("Collapse Multiple cell", GUILayout.Width(150), GUILayout.Height(25))){
                        CollapseMultipleCell();
                    }

        if (GUILayout.Button("Clear", GUILayout.Width(80), GUILayout.Height(25))) {
                        if (targetTilemap != null)
                            targetTilemap.ClearAllTiles();
                    }
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        if (repaint) {
            Repaint();
        }
    }

    private void AnalyzeBaseData() {
        if (baseDataPath == "") {
            ShowNotification(new GUIContent("Select a folder with tilemap prefabs"));
        }
        else {
            tilemapStats = DataUtil.LoadTilemapStats(baseDataPath);

            if (tilemapStats.tileCount == 0)
                ShowNotification(new GUIContent("The folder must contain prefabs of tilemaps"));
            else
                FillUIWithData();
        }
    }

    private void FillUIWithData() {
        // Re-initialize matrix
        Tile[] axisTiles = new Tile[tilemapStats.tileCount];
        for (int i = 0; i < tilemapStats.tileCount; i++) {
            axisTiles[i] = tilemapStats.idToTile[i];
        }

        matrix.Init(tilemapStats.tileCount);
        matrix.SetAxisTiles(axisTiles);

        // Fill matrix with normalized adjacency values
        float[] weights = new float[4];

        for (int y = 0; y < tilemapStats.tileCount; y++) {
            for (int x = 0; x < tilemapStats.tileCount; x++) {
                if (x >= y) {
                    for (int d = 0; d < 4; d++) {
                        weights[d] = tilemapStats.normalizedAdjacency[y, x, d];
                    }

                    matrix.SetBoxWeights(x, y, weights);
                }
            }
        }

        // Re-initialize histogram
        histogram.Init(tilemapStats.tileCount);
        histogram.SetBottomTiles(axisTiles);
        histogram.SetSliderValues(tilemapStats.tileFrequencies);
    }

    private void GenerateEntireMapButtonPressed() {
        createNewWFCRun();

        if (wfc != null) {
            wfc.GenerateEntireMap();
            wfc = null;
        }
    }

    private void ToggleAnimatedGenerationButtonPressed() {
        if (wfc == null) {
            createNewWFCRun();
        }

        if (wfc != null) {
            wfc.ToggleRunning();
        }
    }
    private void CollapseMultipleCell()
    {
        TryInitiliazeWFCInstance();

        if (currentWFCInstance != null)
        {
            (Vector2Int tilePosition, int tileId)? result = currentWFCInstance.RunSingleCellCollapse();

            if (result == null)
            {
                // Reset the WFC instance if we receive null as a result of single cell collapse
                currentWFCInstance = null;

            }
            else
            {
                RenderSingleCell(result.Value.tilePosition, result.Value.tileId);
            }
        }

    }
    


    private void createNewWFCRun() {
        if (Errors()) {
            return;
        }

        ClearTargetTilemap();

        float[,,] directionalWeights = matrix.GetAllBoxWeights();
        float[] tileFrequencies = histogram.GetNormalizedSliderValues();            
            
        if (!useSeed) {
            seed = GUIUtil.GetTimestamp();
        }

        wfc = new WFCRun(targetTilemap, targetWidth, targetHeight, directionalWeights, tileFrequencies, seed, tilemapStats);
    }
    
    private bool Errors() {
        if (targetTilemap == null) {
            ShowNotification(new GUIContent("Set a target tilemap"));
            return true;
        }

        if (tilemapStats == null) {
            ShowNotification(new GUIContent("Select a folder with tilemap prefabs and analyze the data"));
            return true;
        }

        if (targetWidth == 0 || targetHeight == 0) {
            ShowNotification(new GUIContent("Set target width and height"));
            return true;
        }

        return false;
    }

    private void ClearTargetTilemap() {
        if (targetTilemap != null) {
            targetTilemap.ClearAllTiles();
            wfc = null;
        }
    }

    private void Update() {
        if (wfc != null) {
            bool done = wfc.Update();

            if (done) {
                wfc = null;
            }
        }
    }
}
