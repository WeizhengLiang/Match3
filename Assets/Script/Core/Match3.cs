using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.Core;
using Script.Data;
using UnityEngine;
using Match = Script.Core.Match;
using Random = UnityEngine.Random;

public enum GameState
{
    Idle,
    Swapping,
    Matching,
    Falling,
    Filling,
    CheckingCompletion
}

public class Match3 : MonoBehaviour
{
    [SerializeField] private GameState currentState = GameState.Idle;
    [SerializeField] float cellSize = 1f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] bool debug = true;

    [SerializeField] Gem gemPrefab;
    [SerializeField] Ease ease = Ease.InQuad;
    [SerializeField] GameObject explosion;
    
    GridSystem2D<GridObject<Gem>> grid;
    
    Vector2Int selectedGem = Vector2Int.one * -1;
    
    InputReader inputReader;
    AudioManager audioManager;
    ScoreManager scoreManager;
    LevelManager levelManager;
    
    void Awake() {
        inputReader = GetComponent<InputReader>();
        audioManager = GetComponent<AudioManager>();
        scoreManager = GetComponent<ScoreManager>();
        levelManager = GetComponent<LevelManager>();
        inputReader.Fire += OnSelectGem;
    }
    void OnDestroy() {
        inputReader.Fire -= OnSelectGem;
    }

    private void CheckForPossibleCompletion()
    {
        if (levelManager != null)
        {
            var isCompleted = levelManager.CheckCompletion();
            if (!isCompleted)
            {
                SetState(GameState.Idle);
            }
        }
        else
        {
            Debug.LogError("level manager is not found");
        }
    }
    
    void OnSelectGem() {
        // Debug.Log($"Grid status before selecting gem: {grid != null}");
        if(currentState != GameState.Idle) return;
        
        var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));
            
        if (!IsValidPosition(gridPos) || IsEmptyPosition(gridPos)) return;
            
        if (selectedGem == gridPos) {
            DeselectGem();
            audioManager.PlayDeselect();
        } else if (selectedGem == Vector2Int.one * -1) {
            SelectGem(gridPos);
            audioManager.PlayClick();
        } else {
            //StartCoroutine(RunGameLoop(selectedGem, gridPos));
            SetState(GameState.Swapping, gridPos);
        }
    }
    
    public void SetState(GameState newState, Vector2Int selectedPos = default)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.Swapping:
                // Debug.Log("enter swapping state");
                SwapGems(selectedGem, selectedPos);
                break;
            case GameState.Matching:
                // Debug.Log("enter matching state");
                StartCoroutine(StartMatching());
                break;
            case GameState.Falling:
                // Debug.Log("enter falling state");
                StartCoroutine(MakeGemsFall());
                break;
            case GameState.Filling:
                // Debug.Log("enter filling state");
                StartCoroutine(FillEmptySpots());
                break;
            case GameState.CheckingCompletion:
                // Debug.Log("enter checking state");
                CheckForPossibleCompletion();
                break;
            case GameState.Idle:
                DeselectGem();
                // Debug.Log("enter idle state");
                break;
        }
    }
    
    // IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
    // {
    //     IsGameLoopRunning = true;
    //     //yield return StartCoroutine(SwapGems(gridPosA, gridPosB));
    //     
    //     // Matches?
    //     var matches = FindMatches();
    //
    //     if (matches.Count == 0)
    //     {
    //         CheckForPossibleCompletion(); 
    //     }
    //     else
    //     {
    //         scoreManager.UpdateScoreValue(matches.Count);
    //         void UpdateScoreUIAction()
    //         {
    //             scoreManager.UpdateScoreUI();
    //             scoreManager.AnimateScoreChange();
    //         }
    //
    //         // Make Gems explode
    //         yield return StartCoroutine(ExplodeGems(matches, UpdateScoreUIAction));
    //         // Make gems fall
    //         yield return StartCoroutine(MakeGemsFall());
    //         // Fill empty spots
    //         yield return StartCoroutine(FillEmptySpots());
    //          
    //         CheckForPossibleCompletion();  
    //     }
    //
    //     DeselectGem();
    //     IsGameLoopRunning = false;
    //     yield return null;
    // }

    IEnumerator StartMatching()
    {
        var matches = FindMatches();

        if (matches.Count > 0)
        {
            scoreManager.UpdateScoreValue(matches);
            void UpdateScoreUIAction()
            {
                scoreManager.UpdateScoreUI();
                scoreManager.AnimateScoreChange();
            }

            yield return StartCoroutine(ExplodeGems(matches, UpdateScoreUIAction));
            
            SetState(GameState.Falling);
        }
        else
        {
            SetState(GameState.CheckingCompletion);
        }
    }
    
    IEnumerator FillEmptySpots() {
        var config = ConfigurationManager.Instance.CurrentLevelConfig;
        for (var x = 0; x < config.boardWidth; x++) {
            for (var y = 0; y < config.boardWidth; y++) {
                if (grid.GetValue(x, y) == null) {
                    CreateGem(x, y, config.availableGemTypes, config.availableSpecialGemTypes);
                    //audioManager.PlayPop();
                    yield return new WaitForSeconds(0.1f);;
                }
            }
        }
        SetState(GameState.CheckingCompletion);
    }

    
    IEnumerator MakeGemsFall() {
        var config = ConfigurationManager.Instance.CurrentLevelConfig;
        // TODO: Make this more efficient
        for (var x = 0; x < config.boardWidth; x++) {
            for (var y = 0; y < config.boardHeight; y++) {
                if (grid.GetValue(x, y) == null) {
                    Debug.Log($"({x}, {y}) is empty ---------------");
                    for (var i = y + 1; i < config.boardHeight; i++) {
                        if (grid.GetValue(x, i) != null) {
                            Debug.Log($"checking ({x}, {i})");
                            var gem = grid.GetValue(x, i).GetValue();
                            grid.SetValue(x, y, grid.GetValue(x, i));
                            grid.SetValue(x, i, null);
                            gem.transform
                                .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f)
                                .SetEase(ease).onComplete = audioManager.PlayWoosh;
                            //audioManager.PlayWoosh();
                            Debug.Log($"({x}, {y}) is filled by ({x}, {i})-------");
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                    }
                }
            }
        }
        SetState(GameState.Filling);
    }
    
    IEnumerator ExplodeGems(List<Match> matches, Action callback) {

        foreach (var match in matches) {
            foreach (var pos in match.Positions)
            {
                var gridObject = grid.GetValue(pos.x, pos.y);
                if(gridObject == null) continue; // skip if this gem/gridObject was destroyed during the iteration of a previous match
                var gem = gridObject.GetValue();
                grid.SetValue(pos.x, pos.y, null);

                ExplodeVFX(pos.x, pos.y);
                Debug.Log($"vfx locates at ({pos.x}, {pos.y}) is exploded");
                
                gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f).onComplete = audioManager.PlayPop;

                yield return new WaitForSeconds(0.1f);
            
                Destroy(gem.gameObject, 0.1f);
                Debug.Log($"Gem locates at ({pos.x}, {pos.y}) is destroyed");
            }
        }
        callback?.Invoke();
    }
    
    void ExplodeVFX(int x, int y) {
        // TODO: Pool
        var fx = Instantiate(explosion, transform);
        fx.transform.position = grid.GetWorldPositionCenter(x, y);
        Destroy(fx, 5f);
    }

    List<Match> FindMatches()
    {
        List<Match> matches = new();
        var config = ConfigurationManager.Instance.CurrentLevelConfig;
        // Horizontal
        for (var y = 0; y < config.boardWidth; y++) {
            int x = 0;
            while (x < config.boardWidth - 2)
            {
                var gemA = grid.GetValue(x, y);
                var gemB = grid.GetValue(x + 1, y);
                var gemC = grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null) {
                    x++;
                    continue;
                }

                if (gemA.GetValue().GemType == gemB.GetValue().GemType && 
                    gemB.GetValue().GemType == gemC.GetValue().GemType)
                {
                    var positions = new List<Vector2Int>
                    {
                        new (x, y), 
                        new (x + 1, y), 
                        new (x + 2, y)
                    };
                    int xCor = x + 3;
                    while (xCor < config.boardWidth && grid.GetValue(xCor, y)?.GetValue().GemType == gemA.GetValue().GemType)
                    {
                        positions.Add(new Vector2Int(xCor, y));
                        xCor++;
                    }
                    // find special gem
                    var specialGems = FindSpecialGems(positions);
                    matches.Add(new Match(positions, specialGems));
                    x = xCor; // Jump the counter to the end of the current match
                } else {
                    x++;
                }
            }
        }
        
        // Vertical
        for (var x = 0; x < config.boardWidth; x++)
        {
            int y = 0;
            while (y < config.boardHeight - 2) {
                var gemA = grid.GetValue(x, y);
                var gemB = grid.GetValue(x, y + 1);
                var gemC = grid.GetValue(x, y + 2);

                if (gemA == null || gemB == null || gemC == null)
                {
                    y++;
                    continue;
                }

                if (gemA.GetValue().GemType == gemB.GetValue().GemType 
                    && gemB.GetValue().GemType == gemC.GetValue().GemType) {
                    var positions = new List<Vector2Int>
                    {
                        new (x,y),
                        new (x,y+1),
                        new (x,y+2)
                    };
        
                    var yCor = y + 3;
                    while (yCor < config.boardHeight && grid.GetValue(x, yCor)?.GetValue().GemType == gemA.GetValue().GemType)
                    {
                        positions.Add(new Vector2Int(x, yCor));
                        yCor++;
                    }
                    
                    // find special gem
                    // todo: optimization, if no special gem, then no need to new a dictionary for special gems
                    var specialGems = FindSpecialGems(positions);
                    matches.Add(new Match(positions, specialGems));
                    y = yCor;
                }
                else
                {
                    y++;
                }
            }
        }

        if (matches.Count == 0) {
            audioManager.PlayNoMatch();
        } else {
            audioManager.PlayMatch();
        }
            
        return matches;
    }

    Dictionary<SpecialGemType, int> FindSpecialGems(List<Vector2Int> positions)
    {
        var specialGemDictionary = new Dictionary<SpecialGemType, int>(positions.Count);
        foreach (var pos in positions)
        {
            var gem = grid.GetValue(pos.x, pos.y).GetValue();
            if(gem == null) continue;
                        
            var specialGemType = gem.SpecialGemType;

            if (specialGemType == SpecialGemType.NotSpecial) continue;
            
            if(specialGemDictionary.ContainsKey(specialGemType))
            {
                specialGemDictionary[specialGemType] += 1;
            }
            else
            {
                specialGemDictionary.Add(specialGemType, 1);
            }
        }

        return specialGemDictionary;
    }
    
    void SwapGems(Vector2Int gridPosA, Vector2Int gridPosB) {
        var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);

        // See README for a link to the DOTween asset
        gridObjectA.GetValue().transform
            .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f)
            .SetEase(ease);
        gridObjectB.GetValue().transform
            .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f)
            .SetEase(ease).OnComplete(() =>
            {
                grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
                grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

                SetState(GameState.Matching);
            });
        
    }
    
    void InitializeGrid(LevelConfig config) {
        for (var x = 0; x < config.boardWidth; x++) {
            for (var y = 0; y < config.boardHeight; y++) {
                CreateGem(x, y, config.availableGemTypes, config.availableSpecialGemTypes);
            }
        }
        
        if (grid == null) {
            Debug.LogError("Failed to initialize grid");
        } else {
            Debug.Log("Grid initialized successfully");
        }
    }
    
    void ClearGrid() {
        if (grid != null) {
            for (int x = 0; x < grid.width; x++) {
                for (int y = 0; y < grid.height; y++) {
                    GridObject<Gem> gridObject = grid.GetValue(x, y);
                    if (gridObject != null && gridObject.GetValue() != null) {
                        Destroy(gridObject.GetValue().gameObject); // Destroy the gem GameObject
                        grid.SetValue(x, y, null); // Optionally clear the reference
                    }
                }
            }
            grid = null;
        }
    }

    
    public void SetupLevel()
    {
        var config = ConfigurationManager.Instance.CurrentLevelConfig;
        if (config == null) {
            Debug.LogError("No level configuration available.");
            return;
        }
        scoreManager.ResetScore();
        scoreManager.UpdateScoreUI();
        ClearGrid();
        grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(config.boardWidth, config.boardHeight, cellSize, originPosition, debug);
        InitializeGrid(config);
        SetState(GameState.Idle);
    }

    void CreateGem(int x, int y, GemType[] thisLevelGemTypes, SpecialGemType[] thisLevelSpecialGemTypes) {
        var gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        gem.SetType(thisLevelGemTypes[Random.Range(0, thisLevelGemTypes.Length)], thisLevelSpecialGemTypes[Random.Range(0, thisLevelSpecialGemTypes.Length)]);
        var gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem);
        grid.SetValue(x, y, gridObject);
    }

    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);
    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;
    
    bool IsEmptyPosition(Vector2Int gridPosition) => grid.GetValue(gridPosition.x, gridPosition.y) == null;

    bool IsValidPosition(Vector2 gridPosition) {
        var config = ConfigurationManager.Instance.CurrentLevelConfig;
        if (config == null) {
            Debug.LogError("Configuration is unavailable for position validation.");
            return false;
        }
        return gridPosition.x >= 0 && gridPosition.x < config.boardWidth && gridPosition.y >= 0 && gridPosition.y < config.boardWidth;
    }
}
