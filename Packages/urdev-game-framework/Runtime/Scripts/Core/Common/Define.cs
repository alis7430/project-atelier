namespace URDev.GameFramework
{
    public static class Define
    {
        public enum Sound
        {
            BGM,
            SFX,
            MaxCount,
        }

        public enum UIEvent
        {
            Click,
            Drag,
        }

        public enum MouseEvent
        {
            Press,
            Click
        }

        public enum NodeType
        {
            None = -1,
            Start,
            Battle,
            Elite,
            Shop,
            Rest,
            Event,
            End
        }
    }
}
