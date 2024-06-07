using UnityEngine;
using Zenject;

public class GemControllerFactory : IFactory<GemController>
{
    private readonly DiContainer _container;

    public GemControllerFactory(DiContainer container)
    {
        _container = container;
    }

    public GemController Create()
    {
        // Create a new GameObject for the controller if necessary
        var gameObject = new GameObject("GemController");
        var gemController = _container.InstantiateComponent<GemController>(gameObject);
        return gemController;
    }
}