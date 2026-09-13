namespace URDev.GameFramework
{
    public class UI_Popup : UI_Base
    {
        public override void Init()
        {
            if (IsInitialized)
                return;

            base.Init();
            Managers.UI.SetCanvas(gameObject, true);
        }

        public virtual void Close()
        {
            Managers.UI.ClosePopupUI(this);
        }
    }
}
