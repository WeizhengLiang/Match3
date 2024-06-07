using System.Collections.Generic;
using UnityEngine;

public class MatchService : IMatchService
{
    private readonly GridService<GemModel> gridService;

    public MatchService(GridService<GemModel> gridService)
    {
        this.gridService = gridService;
    }
}