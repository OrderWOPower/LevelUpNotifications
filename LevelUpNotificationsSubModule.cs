﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

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
                _levelUpNotificationsTracker = new LevelUpNotificationsTracker();
            }
        }
        protected override void OnApplicationTick(float dt)
        {
            if (Campaign.Current != null)
            {
                _levelUpNotificationsTracker.SetPartyNotification();
            }
        }
        private LevelUpNotificationsTracker _levelUpNotificationsTracker;
    }
}
