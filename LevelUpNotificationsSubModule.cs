using SandBox.View.Map;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace LevelUpNotifications
{
    // This mod displays notifications when the player's troops level up.
    public class LevelUpNotificationsSubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) => ScreenManager.OnPushScreen += OnScreenManagerPushScreen;

        public void OnScreenManagerPushScreen(ScreenBase pushedScreen)
        {
            if (pushedScreen is MapScreen mapScreen)
            {
                mapScreen.AddMapView<LevelUpNotificationsView>(Array.Empty<object>());
            }
        }
    }
}
