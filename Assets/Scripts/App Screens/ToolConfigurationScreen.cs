using GreenNacho.UI;

namespace MetaMonster
{
    public abstract class ToolConfigurationScreen : AppScreen
    {
        public static int ToolPositionIndex { get; set; } = -1;

        protected abstract void OnAddToolConfiguration();

        public void AddToolConfiguration()
        {
            OnAddToolConfiguration();
            AppNavigator.Instance.ReturnToPreviousScreen();
        }
    }
}