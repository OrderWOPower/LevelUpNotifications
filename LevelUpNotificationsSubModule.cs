using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace LevelUpNotifications
{
    // This mod displays notifications when either the player or player's troops level up.
    public class LevelUpNotificationsSubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new LevelUpNotificationsBehavior());
                ScreenManager.OnPushScreen += OnPushScreen;
            }
        }

        public void OnPushScreen(ScreenBase pushedScreen)
        {
            if (pushedScreen is MapScreen mapScreen)
            {
                mapScreen.AddMapView<LevelUpNotificationsView>(Array.Empty<object>());
            }
        }
    }
}
