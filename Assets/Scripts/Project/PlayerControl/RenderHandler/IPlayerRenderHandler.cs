namespace Project.PlayerControl
{
    /// <summary>
    /// render based on player input
    /// </summary>
    public interface IPlayerRenderHandler{
        void OnDirectionChanged(UnityEngine.Vector3 direction);
    }
}