using System;


namespace GameNet.GameService.World
{
    public interface IWorld
    {

        Vector2[] Position {get;set;}
        Vector2 this[int index] { get; set; }
    }
}
