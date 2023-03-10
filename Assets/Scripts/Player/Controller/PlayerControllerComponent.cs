using GLTFImporter.Data;

namespace GLTFImporter.Player
{
    public abstract class PlayerControllerComponent
    { 
        protected PlayerController Controller { get; private set; }
        protected PlayerData Data => Controller.Data;

        public virtual void Init(PlayerController controller)
        {
            Controller = controller;
        }
    }

}