using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using Zenject;

public enum GameState
{
    Idle,
    Swapping,
    ReverseSwapping,
    PreMatching,
    Matching,
    Falling,
    Filling,
    CheckingCompletion
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState currentState = GameState.Idle;
    [SerializeField] bool debug = true;
    [SerializeField] Ease ease = Ease.InQuad;
    [SerializeField] GameObject explosion;

    private Vector2Int selectedGem = Vector2Int.one * -1;

    private ScoreController scoreController;
    private AudioController audioController;
    private LevelController levelController;
    private InputReader inputReader;
    private GridController gridController;
    private ConfigurationController configController;

    [Inject]
    public void Construct(GridController gridController, ConfigurationController configController,
        ScoreController scoreController, AudioController audioController, LevelController levelController,
        InputReader inputReader, GenericPool<GemView> gemPool, GenericPool<TileView> tilePool)
    {
        this.configController = configController;
        this.gridController = gridController;
        this.scoreController = scoreController;
        this.audioController = audioController;
        this.levelController = levelController;
        this.inputReader = inputReader;
    }

    void Awake()
    {
        inputReader.OnFire += OnSelectGem;
    }

    private void Start()
    {
        levelController.LoadLevel(levelController.currentLevelIndex);
    }

    void OnDestroy()
    {
        inputReader.OnFire -= OnSelectGem;
    }

    public void SetState(GameState newState, Vector2Int selectedPos = default, List<MatchModel> preMatchingList = null)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.Swapping:
                Debug.Log("start Swapping");
                SwapGems(selectedGem, selectedPos, false);
                Debug.Log("done Swapping");
                break;
            case GameState.ReverseSwapping:
                Debug.Log("start ReverseSwapping");
                SwapGems(selectedPos, selectedGem, true);
                Debug.Log("done ReverseSwapping");
                break;
            case GameState.PreMatching:
                Debug.Log("start PreMatching");
                StartPreMatching(selectedPos);
                Debug.Log("done PreMatching");
                break;
            case GameState.Matching:
                Debug.Log("start Matching");
                StartCoroutine(StartMatching(preMatchingList));
                Debug.Log("done Matching");
                break;
            case GameState.Falling:
                Debug.Log("start Falling");
                StartCoroutine(MakeGemsFall());
                Debug.Log("done Falling");
                break;
            case GameState.Filling:
                Debug.Log("start Filling");
                StartCoroutine(FillEmptySpots());
                Debug.Log("done Filling");
                break;
            case GameState.CheckingCompletion:
                Debug.Log("start CheckingCompletion");
                CheckForPossibleCompletion();
                Debug.Log("done CheckingCompletion");
                break;
            case GameState.Idle:
                Debug.Log("start Idle");
                DeselectGem();
                Debug.Log("done Idle");
                break;
        }
    }

    private void OnSelectGem()
    {
        if (currentState != GameState.Idle) return;

        var gridPos = gridController.GetXY(Camera.main.ScreenToWorldPoint(inputReader.SelectedPosition));
        if (!IsValidPosition(gridPos) || IsEmptyTilePosition(gridPos)) return;

        if (selectedGem == gridPos)
        {
            DeselectGem();
            audioController.PlayDeselect();
        }
        else if (selectedGem == Vector2Int.one * -1)
        {
            SelectGem(gridPos);
            audioController.PlayClick();
        }
        else if (!IsAdjacentPosition(selectedGem, gridPos))
        {
            DeselectGem();
            Debug.Log("You must select two adjacent gems to swap");
        }
        else
        {
            SetState(GameState.Swapping, gridPos);
        }
    }

    private void StartPreMatching(Vector2Int selectedGem2)
    {
        var preProcessMatchList = new List<MatchModel>();
        FindMatchesForOneGem(selectedGem2, preProcessMatchList);
        FindMatchesForOneGem(selectedGem, preProcessMatchList);
        if (preProcessMatchList.Count == 0)
        {
            SetState(GameState.ReverseSwapping, selectedPos: selectedGem2);
        }
        else
        {
            SetState(GameState.Matching, preMatchingList: preProcessMatchList);
        }
    }

    private IEnumerator StartMatching(List<MatchModel> preProcessMatchList)
    {
        var matches = preProcessMatchList ?? FindMatches();
        if (matches.Count > 0)
        {
            scoreController.UpdateScore(matches);
            yield return StartCoroutine(ExplodeGems(matches, scoreController.UpdateScoreUI));
            SetState(GameState.Falling);
        }
        else
        {
            SetState(GameState.Idle);
        }
    }

    private IEnumerator ExplodeGems(List<MatchModel> matches, Action callback)
    {
        foreach (var match in matches)
        {
            foreach (var pos in match.Positions)
            {
                var tileController = gridController.GetTile(pos.x, pos.y);
                if (tileController == null) continue;
                var gemController = tileController.GetGemController();
                if (gemController == null) continue;
                var gemView = gemController.GemView;
                if(gemView == null) continue;
                ExplodeVFX(pos.x, pos.y);
                gemView.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f)
                    .OnComplete(() => audioController.PlayPop());
                yield return new WaitForSeconds(0.2f);
                gemController.DisposeGemView();
            }
        }

        callback?.Invoke();
    }

    private void ExplodeVFX(int x, int y)
    {
        //todo: build a pool for vfx objects
        var fx = Instantiate(explosion, transform);
        fx.transform.position = gridController.GetWorldPositionCenter(x, y);
        Destroy(fx, 5f);
    }

    private IEnumerator FillEmptySpots()
    {
        var config = configController.CurrentLevelConfig;
        for (var x = 0; x < config.BoardWidth; x++)
        {
            for (var y = 0; y < config.BoardWidth; y++)
            {
                var gem = gridController.GetGem(x, y);
                if (gem.GemView != null) continue;

                // view must generate before model to update view successfully.
                gem.ReGenerateGemView(gridController.GetWorldPositionCenter(x, y));
                gem.ReGenerateGemModel(config, gem.GemModel);

                yield return new WaitForSeconds(0.1f);
            }
        }

        SetState(GameState.CheckingCompletion);
    }

    private IEnumerator MakeGemsFall()
    {
        var config = configController.CurrentLevelConfig;
        for (var x = 0; x < config.BoardWidth; x++)
        {
            for (var y = 0; y < config.BoardHeight; y++)
            {
                var gemController = gridController.GetGem(x, y);
                if (gemController.GemView != null) continue; // current active gem views don't need refill
                
                for (var i = y + 1; i < config.BoardHeight; i++)
                {
                    var gemController2 = gridController.GetGem(x, i);
                    if (gemController2.GemView == null) continue; // looking for active gem views to fall, skip empty views
                    
                    gridController.SetGem(x, y, gemController2);
                    gridController.SetGem(x, i, gemController);
                    gemController2.GemView.transform.DOLocalMove(gridController.GetWorldPositionCenter(x, y), 0.5f)
                        .SetEase(ease).OnComplete(() => audioController.PlayWoosh());
                    yield return new WaitForSeconds(0.1f);
                    break;
                }
            }
        }
        SetState(GameState.Filling);
    }

    /// <summary>
    /// Find Matches for one single Gem
    /// </summary>
    /// <param name="gem1"></param>
    /// <param name="matches"></param>
    /// <returns>the position where this matching ends which is the position next match can starts</returns>
    private void FindMatchesForOneGem(Vector2Int gem1, List<MatchModel> matches)
    {
        var config = configController.CurrentLevelConfig;
        
        // Horizontal 
        var x = gem1.x;
        while (x - 1 >= 0)
        {
            var gemThis = gridController.GetTile(x, gem1.y).GetGemController();
            var gemLeft = gridController.GetTile(x - 1, gem1.y).GetGemController();
            if (gemThis.GemType == gemLeft.GemType)
            {
                x--;
            }
            else
            {
                break;
            }
        }

        if (x < config.BoardWidth - 2)
        {
            var gemA = gridController.GetTile(x, gem1.y).GetGemController();
            var gemB = gridController.GetTile(x + 1, gem1.y).GetGemController();
            var gemC = gridController.GetTile(x + 2, gem1.y).GetGemController();

            if (gemA == null || gemB == null || gemC == null)
            {
                return;
            }

            if (gemA.GemModel.Type == gemB.GemModel.Type && gemB.GemModel.Type == gemC.GemModel.Type)
            {
                var positions = new List<Vector2Int>
                {
                    new(x, gem1.y),
                    new(x + 1, gem1.y),
                    new(x + 2, gem1.y)
                };
                int xCor = x + 3;
                while (xCor < config.BoardWidth &&
                       gridController.GetTile(xCor, gem1.y)?.GetGemController().GemModel.Type ==
                       gemA.GemModel.Type)
                {
                    positions.Add(new Vector2Int(xCor, gem1.y));
                    xCor++;
                }

                var specialGems = FindSpecialGems(positions);
                matches.Add(new MatchModel(positions, specialGems, gemA.GemModel.BasePoint));
            }
        }
        
        

        // Vertical
        var y = gem1.y;

        while (y - 1 >= 0)
        {
            var gemThis = gridController.GetTile(gem1.x, y).GetGemController();
            var gemBot = gridController.GetTile(gem1.x, y - 1).GetGemController();
            if (gemThis.GemType == gemBot.GemType)
            {
                y--;
            }
            else
            {
                break;
            }
        }

        if (y < config.BoardHeight - 2)
        {
            var gemD = gridController.GetTile(gem1.x, y).GetGemController();
            var gemE = gridController.GetTile(gem1.x, y + 1).GetGemController();
            var gemF = gridController.GetTile(gem1.x, y + 2).GetGemController();

            if (gemD == null || gemE == null || gemF == null)
            {
                return;
            }

            if (gemD.GemModel.Type == gemE.GemModel.Type && gemE.GemModel.Type == gemF.GemModel.Type)
            {
                var positions = new List<Vector2Int>
                {
                    new(gem1.x, y),
                    new(gem1.x, y + 1),
                    new(gem1.x, y + 2)
                };

                var yCor = y + 3;
                while (yCor < config.BoardHeight &&
                       gridController.GetTile(gem1.x, yCor)?.GetGemController().GemModel.Type ==
                       gemD.GemModel.Type)
                {
                    positions.Add(new Vector2Int(gem1.x, yCor));
                    yCor++;
                }

                var specialGems = FindSpecialGems(positions);
                matches.Add(new MatchModel(positions, specialGems, gemD.GemModel.BasePoint));
            }
        }
        

        if (matches.Count == 0)
        {
            audioController.PlayNoMatch();
        }
        else
        {
            audioController.PlayMatch();
        }

    }

    private Vector2Int FindMatchesForVerticalDirection(int x, int y, List<MatchModel> matches)
    {
        var config = configController.CurrentLevelConfig;
        var newY = y;
        
        var gemA = gridController.GetTile(x, newY).GetGemController();
        var gemB = gridController.GetTile(x, newY + 1).GetGemController();
        var gemC = gridController.GetTile(x, newY + 2).GetGemController();

        if (gemA == null || gemB == null || gemC == null)
        {
            newY++;
            return new Vector2Int(x, newY);
        }

        if (gemA.GemModel.Type == gemB.GemModel.Type && gemB.GemModel.Type == gemC.GemModel.Type)
        {
            var positions = new List<Vector2Int>
            {
                new(x, newY),
                new(x, newY + 1),
                new(x, newY + 2)
            };
            int yCor = newY + 3;
            while (yCor < config.BoardWidth &&
                   gridController.GetTile(x, yCor)?.GetGemController().GemModel.Type ==
                   gemA.GemModel.Type)
            {
                positions.Add(new Vector2Int(x, yCor));
                yCor++;
            }

            var specialGems = FindSpecialGems(positions);
            matches.Add(new MatchModel(positions, specialGems, gemA.GemModel.BasePoint));
            newY = yCor;
        }
        else
        {
            newY++;
        }

        return new Vector2Int(x, newY);
    }

    private Vector2Int FindMatchesForHorizontalDirection(int x, int y, List<MatchModel> matches)
    {
        var config = configController.CurrentLevelConfig;
        var newX = x;
        
        var gemD = gridController.GetTile(newX, y).GetGemController();
        var gemE = gridController.GetTile(newX + 1, y).GetGemController();
        var gemF = gridController.GetTile(newX + 2, y).GetGemController();

        if (gemD == null || gemE == null || gemF == null)
        {
            newX++;
            return new Vector2Int(newX, y);
        }

        if (gemD.GemModel.Type == gemE.GemModel.Type && gemE.GemModel.Type == gemF.GemModel.Type)
        {
            var positions = new List<Vector2Int>
            {
                new(newX, y),
                new(newX + 1, y),
                new(newX + 2, y)
            };

            var xCor = newX + 3;
            while (xCor < config.BoardHeight &&
                   gridController.GetTile(xCor, y)?.GetGemController().GemModel.Type ==
                   gemD.GemModel.Type)
            {
                positions.Add(new Vector2Int(xCor, y));
                xCor++;
            }

            var specialGems = FindSpecialGems(positions);
            matches.Add(new MatchModel(positions, specialGems, gemD.GemModel.BasePoint));
            newX = xCor;
        }
        else
        {
            newX++;
        }

        return new Vector2Int(newX, y);
    }

    private List<MatchModel> FindMatches()
    {
        List<MatchModel> matches = new();
        var config = configController.CurrentLevelConfig;
        // for (var y = 0; y < config.BoardWidth - 2; y++)
        // {
        //     for (var x = 0; x < config.BoardHeight - 2; x++)
        //     {
        //         var NextGemPositionToMatch = FindMatchesForOneGem(new Vector2Int(x, y), matches);
        //         x = NextGemPositionToMatch.x; // x coordinate validation will be check in for loop condition
        //         y = NextGemPositionToMatch.y < config.BoardWidth - 2 ? NextGemPositionToMatch.y : config.BoardWidth - 2; // in case y coordinate got out of bound
        //     }
        // }

        // horizontal
        for (int y = 0; y < config.BoardHeight; y++)
        {
            var x = 0;
            while (x < config.BoardWidth - 2)
            {
                var NextGemPositionToMatchHorizontally = FindMatchesForHorizontalDirection(x, y, matches);
                x = NextGemPositionToMatchHorizontally.x;
            }
        }
        // vertical
        for (int x = 0; x < config.BoardWidth; x++)
        {
            var y = 0;
            while (y < config.BoardHeight - 2)
            {
                var NextGemPositionToMatchVertically = FindMatchesForVerticalDirection(x, y, matches);
                y = NextGemPositionToMatchVertically.y;
            }
        }
        
        //print the grid elements
        StringBuilder gridElements = new StringBuilder();
        for (int y = config.BoardWidth - 1; y >= 0; y--)
        {
            for (int x = 0; x < config.BoardHeight; x++)
            {
                var gem = gridController.GetGem(x, y);
                gridElements.Append(gem.GemType.ColorType);
                gridElements.Append(gem.GemView.transform.position);
                gridElements.Append(" | ");
            }

            gridElements.Append("\n");
        }
        Debug.Log(gridElements);

        StringBuilder matchPos = new();
        foreach (var match in matches)
        {
            foreach (var pos in match.Positions)
            {
                matchPos.Append(pos);
                matchPos.Append(" | ");
            }
            matchPos.Append("\n");
        }
        Debug.Log(matchPos);
        
        return matches;
    }

    private Dictionary<SpecialGemType, int> FindSpecialGems(List<Vector2Int> positions)
    {
        var specialGemDictionary = new Dictionary<SpecialGemType, int>(positions.Count);
        foreach (var pos in positions)
        {
            var gem = gridController.GetTile(pos.x, pos.y).GetGemController();
            if (gem == null) continue;

            var specialGemType = gem.GemModel.SpecialType;
            if (specialGemType == SpecialGemType.NotSpecial) continue;

            if (specialGemDictionary.ContainsKey(specialGemType))
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

    private void SwapGems(Vector2Int gridPosA, Vector2Int gridPosB, bool isReverseSwap = false)
    {
        var gridObjectA = gridController.GetGem(gridPosA.x, gridPosA.y);
        var gridObjectB = gridController.GetGem(gridPosB.x, gridPosB.y);

        gridObjectA.GemView.transform
            .DOLocalMove(gridController.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f)
            .SetEase(ease);
        gridObjectB.GemView.transform
            .DOLocalMove(gridController.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f)
            .SetEase(ease).OnComplete(() =>
            {
                gridController.SetGem(gridPosA.x, gridPosA.y, gridObjectB);
                gridController.SetGem(gridPosB.x, gridPosB.y, gridObjectA);

                if (isReverseSwap)
                {
                    SetState(GameState.Idle);
                }
                else
                {
                    SetState(GameState.PreMatching, gridPosB);
                }
            });
    }

    public void SetupLevel()
    {
        var config = configController.CurrentLevelConfig;
        if (config == null)
        {
            Debug.LogError("No level configuration available.");
            return;
        }

        scoreController.ResetScore();
        gridController.ClearGrid();
        gridController.Initialize(config);
        SetState(GameState.Matching);
    }


    private void DeselectGem() => selectedGem = new Vector2Int(-1, -1);
    private void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    private bool IsEmptyTilePosition(Vector2Int gridPosition) =>
        gridController.GetTile(gridPosition.x, gridPosition.y) == null;

    private bool IsValidPosition(Vector2 gridPosition)
    {
        var config = configController.CurrentLevelConfig;
        if (config == null)
        {
            Debug.LogError("Configuration is unavailable for position validation.");
            return false;
        }

        return gridPosition.x >= 0 && gridPosition.x < config.BoardWidth && gridPosition.y >= 0 &&
               gridPosition.y < config.BoardWidth;
    }

    private bool IsAdjacentPosition(Vector2Int pos1, Vector2Int pos2)
    {
        return (pos1.x == pos2.x && Mathf.Abs(pos1.y - pos2.y) == 1) ||
               (pos1.y == pos2.y && Mathf.Abs(pos1.x - pos2.x) == 1);
    }

    private void CheckForPossibleCompletion()
    {
        if (levelController != null)
        {
            var isCompleted = levelController.CheckCompletion();
            if (!isCompleted)
            {
                SetState(GameState.Matching);
            }
        }
        else
        {
            Debug.LogError("Level manager is not found");
        }
    }
    
    
}