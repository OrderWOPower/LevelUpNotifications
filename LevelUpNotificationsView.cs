using SandBox.View.Map;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace LevelUpNotifications
{
	public class LevelUpNotificationsView : MapView
	{
		protected override void OnMapScreenUpdate(float dt)
		{
			PartyBase mainParty = PartyBase.MainParty;
			Campaign campaign = Campaign.Current;
			IViewDataTracker viewDataTracker = campaign.GetCampaignBehavior<IViewDataTracker>();

			foreach (TroopRosterElement troopRosterElement in mainParty.MemberRoster.GetTroopRoster())
			{
				CharacterObject currentCharacter = troopRosterElement.Character;
				bool isTroopUpgradable = false;

				for (int i = 0; i < currentCharacter.UpgradeTargets?.Length; i++)
				{
					if (!isTroopUpgradable)
					{
						CharacterObject targetCharacter = currentCharacter.UpgradeTargets[i];
						ItemCategory upgradeRequiresItemFromCategory = targetCharacter?.UpgradeRequiresItemFromCategory;
						int numOfTroops = troopRosterElement.Number, troopXp = troopRosterElement.Xp, upgradeGoldCost = currentCharacter.GetUpgradeGoldCost(mainParty, i), upgradeXpCost = currentCharacter.GetUpgradeXpCost(mainParty, i);
						int numOfTroopsWithGoldRequirementsMet = upgradeGoldCost > 0 ? (int)MathF.Clamp(Hero.MainHero.Gold / upgradeGoldCost, 0f, numOfTroops) : numOfTroops;
						int numOfTroopsWithItemRequirementsMet = upgradeRequiresItemFromCategory != null ? mainParty.ItemRoster.Where(itemRosterElement => itemRosterElement.EquipmentElement.Item.ItemCategory == upgradeRequiresItemFromCategory).Sum(itemRosterElement => itemRosterElement.Amount) : numOfTroops;
						int numOfTroopsWithXpRequirementsMet = targetCharacter?.Level >= currentCharacter.Level && troopXp >= upgradeXpCost ? (int)MathF.Clamp(upgradeXpCost > 0 ? troopXp / upgradeXpCost : numOfTroops, 0f, numOfTroops) : 0;
						int numOfTroopsWithPerkRequirementsMet = targetCharacter != null && campaign.Models.PartyTroopUpgradeModel.DoesPartyHaveRequiredPerksForUpgrade(mainParty, currentCharacter, targetCharacter, out _) ? numOfTroops : 0;

						isTroopUpgradable = MathF.Min(MathF.Min(numOfTroopsWithGoldRequirementsMet, numOfTroopsWithItemRequirementsMet), MathF.Min(numOfTroopsWithXpRequirementsMet, numOfTroopsWithPerkRequirementsMet)) > 0;
					}
				}

				if (currentCharacter.IsHero && (currentCharacter.HeroObject.HeroDeveloper.UnspentAttributePoints > 0 || currentCharacter.HeroObject.HeroDeveloper.UnspentFocusPoints > 0) && !viewDataTracker.IsCharacterNotificationActive)
				{
					// Display the character notification when the player's heroes level up.
					typeof(ViewDataTrackerCampaignBehavior).GetField("_isCharacterNotificationActive", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(viewDataTracker, true);
				}

				if (isTroopUpgradable && !viewDataTracker.IsPartyNotificationActive)
				{
					// Display the party notification when the player's troops level up.
					typeof(ViewDataTrackerCampaignBehavior).GetProperty("IsPartyNotificationActive").SetValue(viewDataTracker, true);
				}
			}
		}
	}
}
