namespace URDev.GameFramework
{
    public class UI_Scene : UI_Base
    {
        public override void Init()
        {
            if (IsInitialized)
                return;

            base.Init();
            Managers.UI.SetCanvas(gameObject, false);
        }
    }
}
