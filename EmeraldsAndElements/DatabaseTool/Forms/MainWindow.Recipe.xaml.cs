using BixBite.Combat;
using BixBite.Crafting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{

		public ObservableCollection<RewardsForIC> FireRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> IceRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> EarthRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> WaterRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> LightningRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> ExplosiveRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> ShadowRewards_Add_List { get; set; }
		public ObservableCollection<RewardsForIC> LuminiousRewards_Add_List { get; set; }

		public Recipe CurrentRecipeToAdd { get; set; }
		public CraftingHelpers.MagicTypeValues CurrentRecipeMaxPoints {get; set;}

		public void MainWindow_Recipes()
		{
			FireRewards_Add_List = new ObservableCollection<RewardsForIC>();
			IceRewards_Add_List = new ObservableCollection<RewardsForIC>();
			EarthRewards_Add_List = new ObservableCollection<RewardsForIC>();
			WaterRewards_Add_List = new ObservableCollection<RewardsForIC>();
			LightningRewards_Add_List = new ObservableCollection<RewardsForIC>();
			ExplosiveRewards_Add_List = new ObservableCollection<RewardsForIC>();
			ShadowRewards_Add_List = new ObservableCollection<RewardsForIC>();
			LuminiousRewards_Add_List = new ObservableCollection<RewardsForIC>();

			CurrentRecipeToAdd = new Recipe();
			CurrentRecipeMaxPoints = new CraftingHelpers.MagicTypeValues();
		}
		

		public void AddFireReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeFireReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeFireReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if(int.TryParse(FireRewardThreshold_Add_TB.Text, out int val))
			{
				if(val > 0)
				{
					FireRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}

		public void AddIceReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeIceReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeIceReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(IceRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					IceRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}

		public void AddEarthReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeEarthReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeEarthReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(EarthRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					EarthRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}


		public void AddWaterReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeWaterReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeWaterReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(WaterRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					WaterRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}
		}


		public void AddLightningReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeLightningReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeLightningReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(LightningRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					LightningRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}

		public void AddExplosiveReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeExplosiveReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeExplosiveReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(ExplosiveRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					ExplosiveRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}

		public void AddShadowReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeShadowReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeShadowReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(ShadowRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					ShadowRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}

		public void AddLuminiousReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeLuminiousReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeLuminiousReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(LuminiousRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					LuminiousRewards_Add_List.Add(new RewardsForIC(modifier, val));
				}
			}

		}




	}

	public class RewardsForIC
	{
		public ModifierData modifier { get; set; }
		public int Threshold { get; set; }

		public RewardsForIC(ModifierData gameplayModifier, int threshold)
		{
			this.modifier = gameplayModifier;
			this.Threshold = threshold;
		}
	}

}
