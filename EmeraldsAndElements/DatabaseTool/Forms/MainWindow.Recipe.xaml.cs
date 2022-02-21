using BixBite.Combat;
using BixBite.Crafting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BixBite.Combat.Equipables;
using BixBite.Items;
using SQLite;

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{

		public ObservableCollection<RecipeReward> FireRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> IceRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> EarthRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> WaterRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> LightningRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> ExplosiveRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> ShadowRewards_Add_List { get; set; }
		public ObservableCollection<RecipeReward> LuminiousRewards_Add_List { get; set; }

		public ObservableCollection<RecipeReward> FireRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> IceRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> EarthRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> WaterRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> LightningRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> ExplosiveRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> ShadowRewards_Edit_List { get; set; }
		public ObservableCollection<RecipeReward> LuminiousRewards_Edit_List { get; set; }


		public List<String> RecipeIngredientIDs = new List<string>();

		public Recipe CurrentRecipeToAdd { get; set; }

		private Recipe _currentRecipeEditing = new Recipe();
		public Recipe CurrentRecipeEditing
		{
			get => _currentRecipeEditing;
			set
			{
				_currentRecipeEditing = value;
				OnPropertyChanged("CurrentRecipeEditing");
			}
		}

		public ObservableCollection<Recipe> CurrentRecipesInDatabase = new ObservableCollection<Recipe>();
		public CraftingHelpers.Max_Elemental_Points CurrentRecipeMaxPoints {get; set;}

		public void MainWindow_Recipes()
		{
			FireRewards_Add_List = new ObservableCollection<RecipeReward>();
			IceRewards_Add_List = new ObservableCollection<RecipeReward>();
			EarthRewards_Add_List = new ObservableCollection<RecipeReward>();
			WaterRewards_Add_List = new ObservableCollection<RecipeReward>();
			LightningRewards_Add_List = new ObservableCollection<RecipeReward>();
			ExplosiveRewards_Add_List = new ObservableCollection<RecipeReward>();
			ShadowRewards_Add_List = new ObservableCollection<RecipeReward>();
			LuminiousRewards_Add_List = new ObservableCollection<RecipeReward>();

			CurrentRecipeToAdd = new Recipe();
			CurrentRecipeEditing = new Recipe();
			CurrentRecipeMaxPoints = new CraftingHelpers.Max_Elemental_Points();
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
					if (val <= CurrentRecipeMaxPoints.Fire)
					{
						if(FireRewards_Add_List.Count == 0 || val > FireRewards_Add_List.Last().Point_Threshold)
							FireRewards_Add_List.Add(new RecipeReward() {Modifier_ID = modifier.Id, Point_Threshold = val});
						else SetOutputLog("The Fire Threshold given ISN'T Bigger t");
					} else SetOutputLog("The Fire Threshold given ISN'T Smaller than the Max");
				} else SetOutputLog("The Fire Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Ice)
					{
						if (IceRewards_Add_List.Count == 0 || val > IceRewards_Add_List.Last().Point_Threshold)
							IceRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Ice Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Ice Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Ice Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Earth)
					{
						if (EarthRewards_Add_List.Count == 0 || val > EarthRewards_Add_List.Last().Point_Threshold)
							EarthRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Earth Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Earth Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Earth Threshold given ISN'T Over 0");
			}

		}


		public void AddWaterReward_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeWaterReward_Add_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeWaterReward_Add_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(FireRewardThreshold_Add_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Fire)
					{
						if (FireRewards_Add_List.Count == 0 || val > FireRewards_Add_List.Last().Point_Threshold)
							FireRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Fire Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Fire Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Fire Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Lightning)
					{
						if (LightningRewards_Add_List.Count == 0 || val > LightningRewards_Add_List.Last().Point_Threshold)
							LightningRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Lightning Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Lightning Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Lightning Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Explosive)
					{
						if (ExplosiveRewards_Add_List.Count == 0 || val > ExplosiveRewards_Add_List.Last().Point_Threshold)
							ExplosiveRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Explosive Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Explosive Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Explosive Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Shadow)
					{
						if (ShadowRewards_Add_List.Count == 0 || val > ShadowRewards_Add_List.Last().Point_Threshold)
							ShadowRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Shadow Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Shadow Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Shadow Threshold given ISN'T Over 0");
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
					if (val <= CurrentRecipeMaxPoints.Luminous)
					{
						if (LuminiousRewards_Add_List.Count == 0 || val > LuminiousRewards_Add_List.Last().Point_Threshold)
							LuminiousRewards_Add_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Luminous Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Luminous Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Luminous Threshold given ISN'T Over 0");
			}

		}

		public void AddFireReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeFireReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeFireReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(FireRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Fire)
					{
						if (FireRewards_Edit_List.Count == 0 || val > FireRewards_Edit_List.Last().Point_Threshold)
							FireRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Fire Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Fire Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Fire Threshold given ISN'T Over 0");
			}

		}

		public void AddIceReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeIceReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeIceReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(IceRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Ice)
					{
						if (IceRewards_Edit_List.Count == 0 || val > IceRewards_Edit_List.Last().Point_Threshold)
							IceRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Ice Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Ice Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Ice Threshold given ISN'T Over 0");
			}

		}

		public void AddEarthReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeEarthReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeEarthReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(EarthRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Earth)
					{
						if (EarthRewards_Edit_List.Count == 0 || val > EarthRewards_Edit_List.Last().Point_Threshold)
							EarthRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Earth Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Earth Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Earth Threshold given ISN'T Over 0");
			}

		}


		public void AddWaterReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeWaterReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeWaterReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(FireRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Fire)
					{
						if (FireRewards_Edit_List.Count == 0 || val > FireRewards_Edit_List.Last().Point_Threshold)
							FireRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Fire Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Fire Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Fire Threshold given ISN'T Over 0");
			}
		}


		public void AddLightningReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeLightningReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeLightningReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(LightningRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Lightning)
					{
						if (LightningRewards_Edit_List.Count == 0 || val > LightningRewards_Edit_List.Last().Point_Threshold)
							LightningRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Lightning Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Lightning Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Lightning Threshold given ISN'T Over 0");
			}

		}

		public void AddExplosiveReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeExplosiveReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeExplosiveReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(ExplosiveRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Explosive)
					{
						if (ExplosiveRewards_Edit_List.Count == 0 || val > ExplosiveRewards_Edit_List.Last().Point_Threshold)
							ExplosiveRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Explosive Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Explosive Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Explosive Threshold given ISN'T Over 0");
			}

		}

		public void AddShadowReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeShadowReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeShadowReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(ShadowRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Shadow)
					{
						if (ShadowRewards_Edit_List.Count == 0 || val > ShadowRewards_Edit_List.Last().Point_Threshold)
							ShadowRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Shadow Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Shadow Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Shadow Threshold given ISN'T Over 0");
			}

		}

		public void AddLuminiousReward_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (RecipeLuminiousReward_Edit_CB.SelectedIndex < 0) return;

			ModifierData modifier = RecipeLuminiousReward_Edit_CB.SelectedItem as ModifierData;
			if (modifier == null) return;

			if (int.TryParse(LuminiousRewardThreshold_Edit_TB.Text, out int val))
			{
				if (val > 0)
				{
					if (val <= CurrentRecipeMaxPoints.Luminous)
					{
						if (LuminiousRewards_Edit_List.Count == 0 || val > LuminiousRewards_Edit_List.Last().Point_Threshold)
							LuminiousRewards_Edit_List.Add(new RecipeReward() { Modifier_ID = modifier.Id, Point_Threshold = val });
						else SetOutputLog("The Luminous Threshold given ISN'T Bigger t");
					}
					else SetOutputLog("The Luminous Threshold given ISN'T Smaller than the Max");
				}
				else SetOutputLog("The Luminous Threshold given ISN'T Over 0");
			}

		}




		private void RecipeIngredient_Add_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			ComboBox cb = sender as ComboBox;

			//We are going to "clear" the other ingredient options
			if (cb == RecipeIngredientItem_Add_CB)
			{
				//RecipeIngredientItem_Add_CB.SelectedIndex = -1;
				RecipeIngredientEquipable_Add_CB.SelectedIndex = -1;
				RecipeIngredientsType_Add_CB.SelectedIndex = -1;
			}
			else if (cb == RecipeIngredientEquipable_Add_CB)
			{
				RecipeIngredientItem_Add_CB.SelectedIndex = -1;
				//RecipeIngredientEquipable_Add_CB.SelectedIndex = -1;
				RecipeIngredientsType_Add_CB.SelectedIndex = -1;
			}
			else if (cb == RecipeIngredientsType_Add_CB)
			{
				RecipeIngredientItem_Add_CB.SelectedIndex = -1;
				RecipeIngredientEquipable_Add_CB.SelectedIndex = -1;
				//RecipeIngredientsType_Add_CB.SelectedIndex = -1;
			}
		}

		private void RecipeAddIngredient_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//RecipesIngredients_Add_IC
			//search the boxes for the ONE selected ingredient type
			ComboBox cb = null;
			String s = "";

			//We are going to "clear" the other ingredient options
			if (RecipeIngredientItem_Add_CB.SelectedIndex != -1)
			{
				//cb = RecipeIngredientItem_Add_CB;
				s = "Item: \t" + (RecipeIngredientItem_Add_CB.SelectedItem as Item).ID;
			}
			else if (RecipeIngredientEquipable_Add_CB.SelectedIndex != -1)
			{
				s = "Equip: \t" + (RecipeIngredientEquipable_Add_CB.SelectedItem as Equipable).ID;
			}
			else if (RecipeIngredientsType_Add_CB.SelectedIndex > 0)
			{
				cb = RecipeIngredientsType_Add_CB;
				s = "Type: \t" + (RecipeIngredientsType_Add_CB.SelectedItem is ECreationTypes ? (ECreationTypes) RecipeIngredientsType_Add_CB.SelectedItem : ECreationTypes.NONE).ToString();
			}

			//We will be adding the string/ID values to the IC/List for later use.
			if (s != String.Empty)
			{
				RecipeIngredientIDs.Add(s);
				RecipesIngredients_Add_IC.Items.Add(s);
			}
		}

		private void RemoveIngredient_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = RecipesIngredients_Add_IC.Items.IndexOf(item);

			RecipesIngredients_Add_IC.Items.RemoveAt(index);
			RecipeIngredientIDs.RemoveAt(index);
		}

		private void RecipeIngredient_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			ComboBox cb = sender as ComboBox;

			//We are going to "clear" the other ingredient options
			if (cb == RecipeIngredientItem_Edit_CB)
			{
				//RecipeIngredientItem_Edit_CB.SelectedIndex = -1;
				RecipeIngredientEquipable_Edit_CB.SelectedIndex = -1;
				RecipeIngredientsType_Edit_CB.SelectedIndex = -1;
			}
			else if (cb == RecipeIngredientEquipable_Edit_CB)
			{
				RecipeIngredientItem_Edit_CB.SelectedIndex = -1;
				//RecipeIngredientEquipable_Edit_CB.SelectedIndex = -1;
				RecipeIngredientsType_Edit_CB.SelectedIndex = -1;
			}
			else if (cb == RecipeIngredientsType_Edit_CB)
			{
				RecipeIngredientItem_Edit_CB.SelectedIndex = -1;
				RecipeIngredientEquipable_Edit_CB.SelectedIndex = -1;
				//RecipeIngredientsType_Edit_CB.SelectedIndex = -1;
			}
		}

		private void RecipeAddIngredient_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//RecipesIngredients_Edit_IC
			//search the boxes for the ONE selected ingredient type
			ComboBox cb = null;
			String s = "";

			//We are going to "clear" the other ingredient options
			if (RecipeIngredientItem_Edit_CB.SelectedIndex != -1)
			{
				//cb = RecipeIngredientItem_Edit_CB;
				s = "Item: \t" + (RecipeIngredientItem_Edit_CB.SelectedItem as Item).ID;
			}
			else if (RecipeIngredientEquipable_Edit_CB.SelectedIndex != -1)
			{
				s = "Equip: \t" + (RecipeIngredientEquipable_Edit_CB.SelectedItem as Equipable).ID;
			}
			else if (RecipeIngredientsType_Edit_CB.SelectedIndex > 0)
			{
				cb = RecipeIngredientsType_Edit_CB;
				s = "Type: \t" + (RecipeIngredientsType_Edit_CB.SelectedItem is ECreationTypes ? (ECreationTypes)RecipeIngredientsType_Edit_CB.SelectedItem : ECreationTypes.NONE).ToString();
			}

			//We will be adding the string/ID values to the IC/List for later use.
			if (s != String.Empty)
			{
				RecipeIngredientIDs.Add(s);
				RecipesIngredients_Edit_IC.Items.Add(s);
			}
		}

		private void RemoveIngredient_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = RecipesIngredients_Edit_IC.Items.IndexOf(item);

			RecipesIngredients_Edit_IC.Items.RemoveAt(index);
			RecipeIngredientIDs.RemoveAt(index);
		}



		private void AddRecipeToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			int i = 0;

			//first check for all the required boxes
			if (CurrentRecipeToAdd.Name != "" &&
			    CurrentRecipeToAdd.Required_Quality >= 0 &&
			    CurrentRecipeToAdd.Required_Level >= 0 &&
			    int.TryParse(RecipeBaseOutputQuality_Add_TB.Text, out int baseQuality) &&
			    int.TryParse(RecipeBaseOutputRating_Add_TB.Text, out int baseRating) &&
			    int.TryParse(RecipeBaseOutputSize_Add_TB.Text, out int baseSize) &&
			    int.TryParse(RecipeBaseOutputUseCount_Add_TB.Text, out int baseUseCount) &&

					int.TryParse(RecipesMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(RecipesMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(RecipesAtk_Add_TB.Text, out int atkResult) &&
			    int.TryParse(RecipesDef_Add_TB.Text, out int defResult) &&
					int.TryParse(RecipesDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(RecipesAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(RecipesMor_Add_TB.Text, out int morResult) &&
					int.TryParse(RecipesWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(RecipesRes_Add_TB.Text, out int resResult) &&
					int.TryParse(RecipesLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(RecipesRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(RecipesItl_Add_TB.Text, out int itlResult) &&


					RecipeIngredientIDs.Count >= 1)
			{

				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Creation Type
					int CreationTypeEnumBits = 0;
					foreach (int en in Enum.GetValues(typeof(ECreationTypes)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)RecipesCreationType_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddRecipesWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							CreationTypeEnumBits += (int)Math.Pow(2, i);
						}
						i++;
					}

					#endregion

					#region Max Elemental Points

					Createsql = "SELECT * FROM `max_elemental_points`;";
					List<CraftingHelpers.Max_Elemental_Points> mpList = _sqlite_conn.Query<CraftingHelpers.Max_Elemental_Points>(Createsql);
					int newID_stat = (mpList.Count == 0 ? 0 : mpList.Max(x => x.ID));



					//Set up the weapon object!
					CraftingHelpers.Max_Elemental_Points maxPoints = new CraftingHelpers.Max_Elemental_Points()
					{
						ID = newID_stat + 1,
						Fire = CurrentRecipeMaxPoints.Fire,
						Ice = CurrentRecipeMaxPoints.Ice,
						Earth = CurrentRecipeMaxPoints.Earth,
						Water = CurrentRecipeMaxPoints.Water,
						Lightning = CurrentRecipeMaxPoints.Lightning,
						Explosive = CurrentRecipeMaxPoints.Explosive,
						Shadow = CurrentRecipeMaxPoints.Shadow,
						Luminous = CurrentRecipeMaxPoints.Luminous,
					};
					#endregion

					_sqlite_conn.Insert(maxPoints);

					#region Rewards

					Recipe_Rewards reward = new Recipe_Rewards();
					
					// Go through EVERY singe reward and add them to the DB
					foreach (RecipeReward r in FireRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Fire,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in IceRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Ice,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in EarthRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Earth,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in WaterRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Water,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in LightningRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Lightning,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in ExplosiveRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Explosive,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in ShadowRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Shadow,
						};
						_sqlite_conn.Insert(reward);
					}

					foreach (RecipeReward r in LuminiousRewards_Add_List)
					{
						reward = new Recipe_Rewards()
						{
							Modifier_ID = r.Modifier_ID,
							Point_Threshold = r.Point_Threshold,
							Req_Recipe = CurrentRecipeToAdd.Name,
							Magic_Type = (int)EMagicType.Luminous,
						};
						_sqlite_conn.Insert(reward);
					}

					#endregion

					#region Ingredients

					foreach (String ingredientID in RecipeIngredientIDs)
					{
						String[] ingredientInfo = ingredientID.Split(':');
						Recipe_Ingredients ingredient = new Recipe_Ingredients()
						{
							Req_Recipe = CurrentRecipeToAdd.Name,
							Type = ingredientInfo[0].Trim(),
							Value = ingredientInfo[1].Trim(),
						};
						_sqlite_conn.Insert(ingredient);
					}

					#endregion

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);


					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					int newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)RecipesMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddRecipesMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)RecipesWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddRecipesWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)RecipesMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddRecipesMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)RecipesStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddRecipesWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);


					//Set up the actual recipe!
					Recipes Recipes = new Recipes()
					{
						Name = CurrentRecipeToAdd.Name,
						Creation_Type = CreationTypeEnumBits,
						Required_Quality = CurrentRecipeToAdd.Required_Quality,
						Required_Level = CurrentRecipeToAdd.Required_Level,
						Hours_To_Make = CurrentRecipeToAdd.Hours_To_Make,
						Use_Count = baseUseCount,
						Rating =  baseRating,
						Quality = baseQuality,
						Size = baseSize,
						

						Stats_FK = basestat.ID,
						Max_Elemental_FK = maxPoints.ID,
						Weakness_Strength_FK = weakstrToAdd.ID
					};

					//GET THE MAGIC TYPE ENUMERATED BITS (CURRENT DISABLED COMMENTED OUT)
					#region Magic types
					//i = 0;
					//int magictypesval = 0;
					//foreach (int en in Enum.GetValues(typeof(EMagicType)))
					//{
					//	if (en == 0) continue;
					//	ContentPresenter c = ((ContentPresenter)RecipesMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
					//	var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

					//	if ((bool)(vv as CheckBox).IsChecked)
					//	{
					//		magictypesval += (int)Math.Pow(2, i);
					//	}
					//	i++;
					//}
					//Recipes.Elemental = magictypesval;
					#endregion


					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "Recipes", Recipes.Name);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "Recipes", Recipes.Name);
					#endregion
					#region Skills
					//InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "Recipes", Recipes.Name);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(Recipes);
					Console.WriteLine("RowID Val: {0}", retval);
					SetOutputLog(String.Format("Successfully Added Recipe to DB: {0}", retval));
				}
				catch (Exception ex)
				{
					Console.WriteLine("Recipe write from database [Recipes] FAILURE | {0}", ex.Message);
					SetOutputLog(String.Format("Loading/Writing Database [Recipes] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}

			}
		}

		private void LoadRecipeFromDatabase()
		{

			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			RecipeName_Edit_CB.ItemsSource = null;

			CurrentRecipesInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;

				Createsql = ("SELECT * FROM `recipe_rewards`;");
				IEnumerable<Recipe_Rewards> rewardlist = _sqlite_conn.Query<Recipe_Rewards>(Createsql);

				Createsql = ("SELECT * FROM `recipes`;");
					IEnumerable<Recipes> varlist = _sqlite_conn.Query<Recipes>(Createsql);
					foreach (Recipes rep in varlist.ToList())
					{
					String CreatesqlINNER = (String.Format("SELECT * FROM `max_elemental_points` WHERE id = {0};", rep.Max_Elemental_FK.ToString()));//Recipe Max points
					IEnumerable<CraftingHelpers.Max_Elemental_Points> varlist_mod = _sqlite_conn.Query<CraftingHelpers.Max_Elemental_Points>(CreatesqlINNER);
					rep.MaxPoints = varlist_mod.First();

					CreatesqlINNER = (String.Format("SELECT * FROM `base_stats` WHERE id = {0};", rep.Stats_FK.ToString()));//Recipe Max points
					IEnumerable<Base_Stats> varlist_stats = _sqlite_conn.Query<Base_Stats>(CreatesqlINNER);
					rep.stats = varlist_stats.First();

					CreatesqlINNER = (String.Format("SELECT * FROM `recipe_ingredients`;", rep.Name));
					IEnumerable<Recipe_Ingredient> varlist_ind = _sqlite_conn.Query<Recipe_Ingredient>(CreatesqlINNER);
					varlist_ind = Array.FindAll(varlist_ind.ToArray(), x => x.Req_Recipe == rep.Name);

					CreatesqlINNER = (String.Format("SELECT * FROM `weaknesses_strengths` WHERE id = {0} ;", rep.Weakness_Strength_FK));
					IEnumerable<weaknesses_strengths> varlist_weak = _sqlite_conn.Query<weaknesses_strengths>(CreatesqlINNER);
					rep.WeaknessesStrengths = varlist_weak.First();

					var q = varlist_ind.GroupBy(x => x.Value)
						.Select((g,x) => new { Key=g.Key, IndgredientType = (g as IGrouping<String, Recipe_Ingredient>).ToList()[0].Type, Count = g.Count()})
						.OrderByDescending(x => x.Count);


					foreach ( var v in q)
					{
						if(v.IndgredientType.ToLower().Contains("item"))
							rep.RequiredIngredients.Add(new CraftingHelpers.RecipeIngredient(CurrentItemsInDatabase.First(x=>x.ID==v.Key), v.Count));
						//else if (v.IndgredientType.ToLower().Contains("equipable"))
						//	rep.RequiredIngredients.Add(new CraftingHelpers.RecipeIngredient(Current.First(x => x.ID == v.Key), v.Count));
						else if (v.IndgredientType.ToLower().Contains("type"))
							rep.RequiredIngredients.Add(new CraftingHelpers.RecipeIngredient(Enum.Parse(typeof(ECreationTypes), v.Key) , v.Count));
					}

					//Each weapon a list of keys to it. And we must populate that data correctly
					Recipe_Rewards[] recipeRewards = Array.FindAll(rewardlist.ToArray(), x => x.Req_Recipe == rep.Name);
						foreach (Recipe_Rewards rewa in recipeRewards)
						{
							rep.PossibleRewards.Add(new CraftingHelpers.Reward_Requirement((EMagicType) rewa.Magic_Type,
								rewa.Point_Threshold,
								new GameplayModifier()
									{ModifierData = CurrentGameplayModifiersInDatabase.FirstOrDefault(y => y.Id == rewa.Modifier_ID)}));
						}
						CurrentRecipesInDatabase.Add(rep);
					}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Weapons Read from database FAILURE {0}:", ex.Message);
				SetOutputLog(String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message));
			}
			finally
			{
				RecipeName_Edit_CB.ItemsSource = CurrentRecipesInDatabase;
			}

			//Load Recipe Object 

			//Load Max Elemental Points

			// Load Load the Rewards

			//Load The ingredients

			//Load Stats

			//Load the Weakness and strengths
		}

		private void RecipeName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb != null)
			{
				Recipe recipe = cb.SelectedItem as Recipe;
				CurrentRecipeEditing = recipe;



				//Recipe Properties
				RecipeQualityThreshold_Edit_TB.Text = recipe.Required_Quality.ToString();
				RecipeRequiredLevel_Edit_TB.Text = recipe.Required_Level.ToString();
				RecipeTimeToMake_Edit_Tb.Text = recipe.Hours_To_Make.ToString();

				RecipeBaseOutputQuality_Edit_TB.Text = recipe.Quality.ToString();
				RecipeBaseOutputRating_Edit_TB.Text = recipe.Rating.ToString();
				RecipeBaseOutputSize_Edit_TB.Text = recipe.Size.ToString();
				RecipeBaseOutputUseCount_Edit_TB.Text = recipe.Use_Count.ToString();

				//Max Points
				RecipeMaxFire_Edit_TB.Text = recipe.MaxPoints.Fire.ToString();
				RecipeMaxIce_Edit_TB.Text = recipe.MaxPoints.Ice.ToString();
				RecipeMaxEarth_Edit_TB.Text = recipe.MaxPoints.Earth.ToString();
				RecipeMaxWater_Edit_TB.Text = recipe.MaxPoints.Water.ToString();
				RecipeMaxLightning_Edit_TB.Text = recipe.MaxPoints.Lightning.ToString();
				RecipeMaxExplosive_Edit_TB.Text = recipe.MaxPoints.Explosive.ToString();
				RecipeMaxShadow_Edit_TB.Text = recipe.MaxPoints.Shadow.ToString();
				RecipeMaxLightning_Edit_TB.Text = recipe.MaxPoints.Luminous.ToString();

				//Stats
				RecipesMaxHP_Add_TB.Text = recipe.stats.Max_Health.ToString();
				RecipesMaxMP_Add_TB.Text = recipe.stats.Max_Mana.ToString();

				RecipesAtk_Edit_TB.Text = recipe.stats.Max_Health.ToString();
				RecipesDef_Edit_TB.Text = recipe.stats.Max_Mana.ToString();
				RecipesDex_Edit_TB.Text = recipe.stats.Attack.ToString();
				RecipesAgl_Edit_TB.Text = recipe.stats.Agility.ToString();
				RecipesMor_Edit_TB.Text = recipe.stats.Morality.ToString();
				RecipesWis_Edit_TB.Text = recipe.stats.Wisdom.ToString();
				RecipesRes_Edit_TB.Text = recipe.stats.Resistance.ToString();
				RecipesLuc_Edit_TB.Text = recipe.stats.Luck.ToString();
				RecipesRsk_Edit_TB.Text = recipe.stats.Risk.ToString();
				RecipesItl_Edit_TB.Text = recipe.stats.Intelligence.ToString();


				//Weakness ANd strength
				#region Mag Weakness and strength
				//reset
				SetItemControlCheckboxData(RecipesMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddMagWeak_CB", true);
				SetItemControlCheckboxData(RecipesMagicWeakness_Edit_IC, recipe.WeaknessesStrengths.magic_weaknesses, EMagicType.NONE, "AddMagWeak_CB", false);

				SetItemControlCheckboxData(RecipesMagicStrength_Edit_IC, null, EMagicType.NONE, "AddMagicStrength_CB", true);
				SetItemControlCheckboxData(RecipesMagicStrength_Edit_IC, recipe.WeaknessesStrengths.magic_strengths, EMagicType.NONE, "AddMagicStrength_CB", false);
				RecipesMagicWeakness_Edit_IC.UpdateLayout();
				RecipesMagicStrength_Edit_IC.UpdateLayout();
				#endregion
				#region Weapon Weakness & Strengths
				//reset
				SetItemControlCheckboxData(RecipesWeakness_Edit_IC, null, EMagicType.NONE, "AddWeak_CB", true);
				SetItemControlCheckboxData(RecipesWeakness_Edit_IC, recipe.WeaknessesStrengths.physical_weaknesses, EWeaponType.NONE, "AddWeak_CB", false);

				SetItemControlCheckboxData(RecipesStrength_Edit_IC, null, EWeaponType.NONE, "AddStrength_CB", true);
				SetItemControlCheckboxData(RecipesStrength_Edit_IC, recipe.WeaknessesStrengths.physical_strengths, EWeaponType.NONE, "AddStrength_CB", false);
				RecipesStrength_Edit_IC.UpdateLayout();
				RecipesWeakness_Edit_IC.UpdateLayout();
				#endregion

				//Creation Type
				SetItemControlCheckboxData(RecipesCreationType_Edit_IC, null, ECreationTypes.NONE, "EditCheckbox", true);
				SetItemControlCheckboxData(RecipesCreationType_Edit_IC, recipe.Creation_Type, ECreationTypes.NONE, "EditCheckbox", false);

				//Ingredients
				foreach (CraftingHelpers.RecipeIngredient ingredient in recipe.RequiredIngredients)
				{
					String s = "";
					//We are going to "clear" the other ingredient options
					if (ingredient.Ingredient.GetType().Name.Contains("Item"))
					{
						s = "Item: \t" + (ingredient.Ingredient as Item).ID;
					}
					else if (ingredient.Ingredient.GetType().Name.Contains("Equipable"))
					{
						s = "Equip: \t" + (ingredient.Ingredient as Equipable).ID;
					}
					else if (ingredient.Ingredient.GetType().Name.Contains("ECreationTypes"))
					{
						//cb = RecipeIngredientsType_Edit_CB;
						s = "Type: \t" + ((ECreationTypes)((int)(ingredient?.Ingredient))).ToString();
					}
					for (int i = 0; i < ingredient.NumberOfIngredient; i++)
						RecipesIngredients_Edit_IC.Items.Add(s);
					
				}


				//Rewards.
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Fire))
					RecipeRewardFire_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Ice))
					RecipeRewardIce_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Earth))
					RecipeRewardEarth_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Water))
					RecipeRewardWater_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Lightning))
					RecipeRewardLightning_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Explosive))
					RecipeRewardExplosive_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Shadow))
					RecipeRewardShadow_Edit_IC.Items.Add(reward);
				foreach (CraftingHelpers.Reward_Requirement reward in recipe.PossibleRewards.GetListOfMagicTypeRewards(EMagicType.Luminous))
					RecipeRewardLuminous_Edit_IC.Items.Add(reward);



			}
		}


	}

	//public class RecipeReward
	//{
	//	public ModifierData modifier { get; set; }
	//	public int Threshold { get; set; }

	//	public RecipeReward(ModifierData gameplayModifier, int threshold)
	//	{
	//		this.modifier = gameplayModifier;
	//		this.Threshold = threshold;
	//	}
	//}

}
