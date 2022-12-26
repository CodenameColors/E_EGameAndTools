using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using SQLite;

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{
		
		public ObservableCollection<Clothes> CurrentClothesInDatabase { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Head { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Body { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Legs { get; set; }

		public void MainWindow_Clothes()
		{
			CurrentClothesInDatabase = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Head = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Body = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Legs = new ObservableCollection<Clothes>();
		}

		private void LoadClothesFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			ClothesName_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Head_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Body_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Legs_Edit_CB.ItemsSource = null;

			enemyClothes_Head_Edit_CB.ItemsSource = null;
			enemyClothes_Body_Edit_CB.ItemsSource = null;
			enemyClothes_Legs_Edit_CB.ItemsSource = null;

			(RecipeIngredientEquipable_Add_CB.ItemsSource as CompositeCollection).Clear();

			CurrentClothesInDatabase.Clear();
			CurrentClothesInDatabase_Head.Clear();
			CurrentClothesInDatabase_Body.Clear();
			CurrentClothesInDatabase_Legs.Clear();

			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `clothes`;");



				IEnumerable<Clothes> varlist = _sqlite_conn.Query<Clothes>(Createsql);
				foreach (Clothes clothes in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys

					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", clothes.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata = CurrentGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
							clothes.Effects.Add(moddata);
						else
							clothes.Traits.Add(moddata);
					}

					#endregion

					#region Skills

					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", clothes.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						clothes.ClothesSkills.Add(skilldata);
					}

					#endregion

					#endregion

					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentClothesInDatabase.Add(clothes);
					if (clothes.Clothes_Type == (int)EClothesType.Head)
					{
						CurrentClothesInDatabase_Head.Add(clothes);
					}
					else if (clothes.Clothes_Type == (int)EClothesType.Body)
					{
						CurrentClothesInDatabase_Body.Add(clothes);
					}
					else if (clothes.Clothes_Type == (int)EClothesType.Legs)
					{
						CurrentClothesInDatabase_Legs.Add(clothes);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("clothes Read from database FAILURE {0}:", ex.Message);
				SetOutputLog(String.Format("Loading/Reading [FROM enemy] Database failed: {0}", ex.Message));
			}
			finally
			{
				ClothesName_Edit_CB.ItemsSource = CurrentClothesInDatabase;
				PartyMemberClothes_Head_Edit_CB.ItemsSource = CurrentClothesInDatabase_Head;
				PartyMemberClothes_Body_Edit_CB.ItemsSource = CurrentClothesInDatabase_Body;
				PartyMemberClothes_Legs_Edit_CB.ItemsSource = CurrentClothesInDatabase_Legs;

				enemyClothes_Head_Edit_CB.ItemsSource = CurrentClothesInDatabase_Head;
				enemyClothes_Body_Edit_CB.ItemsSource = CurrentClothesInDatabase_Body;
				enemyClothes_Legs_Edit_CB.ItemsSource = CurrentClothesInDatabase_Legs;

				(RecipeIngredientEquipable_Add_CB.ItemsSource as CompositeCollection).Add(new CollectionContainer(){Collection = CurrentClothesInDatabase });
				(RecipeIngredientEquipable_Edit_CB.ItemsSource as CompositeCollection).Add(new CollectionContainer(){Collection = CurrentClothesInDatabase });

			}
		}


		private void ClothesEquipEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipEffects_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesEffectEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Add_CB.SelectedIndex].Id;
					if (!ClothesEffectEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesEffectEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesEffectEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void ClothesEquipTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipTraits_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesTraitsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Add_CB.SelectedIndex].Id;
					if (!ClothesTraitsEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesTraitsEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesTraitsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void ClothesEquipSkill_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipSkills_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesSkillsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[ClothesEquipSkills_Add_CB.SelectedIndex].Name;
					if (!ClothesSkillsEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesSkillsEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesSkillsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}


		private void AddClothesToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (ClothesName_Add_TB.Text.Length > 0 &&
					int.TryParse(ClothesWeight_Add_TB.Text, out int weightval) &&
					ClothesType_Add_CB.SelectedIndex >= 0 && ClothesRarity_Add_CB.SelectedIndex >= 0 &&

					int.TryParse(ClothesMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(ClothesMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(ClothesAtk_Add_TB.Text, out int atkResult) &&
					int.TryParse(ClothesDef_Add_TB.Text, out int defResult) &&
					int.TryParse(ClothesDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(ClothesAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(ClothesMor_Add_TB.Text, out int morResult) &&
					int.TryParse(ClothesWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(ClothesRes_Add_TB.Text, out int resResult) &&
					int.TryParse(ClothesLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(ClothesRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(ClothesItl_Add_TB.Text, out int itlResult) &&

					int.TryParse(Clothes_pointCo_Min_Size_Add_TB.Text, out int minSize) &&
					int.TryParse(Clothes_pointCo_Max_Size_Add_TB.Text, out int maxSize) &&
					int.TryParse(Clothes_pointCo_Min_Qual_Add_TB.Text, out int minQual) &&
					int.TryParse(Clothes_pointCo_Max_Qual_Add_TB.Text, out int maxQual) &&
					int.TryParse(Clothes_pointCo_Min_Rare_Add_TB.Text, out int minRare) &&
					int.TryParse(Clothes_pointCo_Max_Rare_Add_TB.Text, out int maxRare) &&
					int.TryParse(Clothes_pointCo_Min_Points_Add_TB.Text, out int minPoints) &&
					int.TryParse(Clothes_pointCo_Max_Points_Add_TB.Text, out int maxPoints)

					) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					int newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

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
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesMagWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ClothesWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ClothesMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesMagicStrength_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ClothesWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesWeaknessStrength_CB", c);

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

					#region Point Coefficents

					//Set up the Point coefficents object.
					Createsql = "SELECT * FROM `point_coeffiencts`;";
					List<Point_Coeffiencts> pcList = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);
					int newID_stat_pc = (pcList.Count == 0 ? 0 : pcList.Max(x => x.ID));

					//Set up the point coefficent object!
					int pc_piece_types = 0;
					foreach (UIElement element in Clothes_Piece_Types_Add_Grid.Children)
					{
						if (element is CheckBox cb)
						{
							if ((bool)(cb as CheckBox).IsChecked)
							{
								pc_piece_types += (int)Math.Pow(2, Grid.GetRow(cb));
							}
						}
					}

					Point_Coeffiencts pointCoeffiencts = new Point_Coeffiencts()
					{
						ID = newID_stat_pc + 1,
						Max_Size = maxSize,
						Min_Size = minSize,
						Min_Quality = minQual,
						Max_Quality = maxQual,
						Min_Rarity = minRare,
						Max_Rarity = maxRare,
						Min_Points = minPoints,
						Max_Points = maxPoints,
						possible_piece_types = pc_piece_types
					};
					_sqlite_conn.Insert(pointCoeffiencts);

					#endregion

					//Set up the weapon object!
					Clothes Clothes = new Clothes()
					{
						ID = ClothesName_Add_TB.Text,
						Clothes_Type = (int)((EWeaponType)ClothesType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)ClothesRarity_Add_CB.SelectedValue),
						Weight = weightval,
						Function_PTR = ClothesFuncPTR_Add_TB.Text, //1.0.0.3v

						Stats_FK = basestat.ID,
						Point_Coeffiencts_FK = pointCoeffiencts.ID,
						Weakness_Strength_FK = weakstrToAdd.ID
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					Clothes.Elemental = magictypesval;
					#endregion

					//DNE
					#region Item Types
					//i = 0;
					//int itemstypesval = 0;
					//foreach (int en in Enum.GetValues(typeof(EItemType)))
					//{
					//	if (en == 0) continue;
					//	ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
					//	var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

					//	if ((bool)(vv as CheckBox).IsChecked)
					//	{
					//		itemstypesval += (int)Math.Pow(2, i);
					//	}
					//	i++;
					//}
					//item.Item_Type = itemstypesval;
					#endregion

					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(Clothes);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [Clothes] FAILURE | {0}", ex.Message);
					SetOutputLog(String.Format("Loading/Writing Database [Clothes] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


		private void ClothesEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipEffects_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesEffectEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Edit_CB.SelectedIndex].Id;
					if (!ClothesEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Effects.Add(
							CurrentGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Edit_CB.SelectedIndex]);

						ClothesEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveClothesEffect_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesEffectEquip_Add_IC.Items.IndexOf(item);

			ClothesEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesEffect_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesEffectEquip_Edit_IC.Items.IndexOf(item);

			ClothesEffectEquip_Edit_IC.Items.RemoveAt(index);

			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);

		}

		private void ClothesEquipTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipTraits_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesTraitsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!ClothesTraitsEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesTraitsEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Traits.Add(
							CurrentGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Edit_CB.SelectedIndex]);
						ClothesTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveClothesTrait_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesTraitsEquip_Add_IC.Items.IndexOf(item);

			ClothesTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesTrait_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesTraitsEquip_Edit_IC.Items.IndexOf(item);

			ClothesTraitsEquip_Edit_IC.Items.RemoveAt(index);

			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);
		}


		private void ClothesEquipSkill_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipSkills_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesSkillsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[ClothesEquipSkills_Edit_CB.SelectedIndex].Name;
					if (!ClothesSkillsEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesSkillsEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].ClothesSkills.Add(
							CurrentSkillsInDatabase[ClothesEquipSkills_Edit_CB.SelectedIndex]);
						ClothesSkillsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}


		private void RemoveClothesSkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesSkillsEquip_Add_IC.Items.IndexOf(item);

			ClothesSkillsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesSkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesSkillsEquip_Edit_IC.Items.IndexOf(item);

			ClothesSkillsEquip_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].ClothesSkills.RemoveAt(index);
		}

		private void UpdateClothesInDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (
					int.TryParse(ClothesWeight_Edit_TB.Text, out int weightval) &&
					int.TryParse(ClothesMaxHP_Edit_TB.Text, out int maxHpResult) &&
					int.TryParse(ClothesMaxMP_Edit_TB.Text, out int maxMPResult) &&

					int.TryParse(ClothesAtk_Edit_TB.Text, out int atkResult) &&
					int.TryParse(ClothesDef_Edit_TB.Text, out int defResult) &&
					int.TryParse(ClothesDex_Edit_TB.Text, out int dexResult) &&
					int.TryParse(ClothesAgl_Edit_TB.Text, out int aglResult) &&
					int.TryParse(ClothesMor_Edit_TB.Text, out int morResult) &&
					int.TryParse(ClothesWis_Edit_TB.Text, out int wisResult) &&
					int.TryParse(ClothesRes_Edit_TB.Text, out int resResult) &&
					int.TryParse(ClothesLuc_Edit_TB.Text, out int LucResult) &&
					int.TryParse(ClothesRsk_Edit_TB.Text, out int RskResult) &&
					int.TryParse(ClothesItl_Edit_TB.Text, out int itlResult) &&

					int.TryParse(Clothes_pointCo_Min_Size_Edit_TB.Text, out int minSize) &&
					int.TryParse(Clothes_pointCo_Max_Size_Edit_TB.Text, out int maxSize) &&
					int.TryParse(Clothes_pointCo_Min_Qual_Edit_TB.Text, out int minQual) &&
					int.TryParse(Clothes_pointCo_Max_Qual_Edit_TB.Text, out int maxQual) &&
					int.TryParse(Clothes_pointCo_Min_Rare_Edit_TB.Text, out int minRare) &&
					int.TryParse(Clothes_pointCo_Max_Rare_Edit_TB.Text, out int maxRare) &&
					int.TryParse(Clothes_pointCo_Min_Points_Edit_TB.Text, out int minPoints) &&
					int.TryParse(Clothes_pointCo_Max_Points_Edit_TB.Text, out int maxPoints)

			) //1.0.0.2v
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = ClothesName_Edit_CB.SelectedIndex;
				Clothes Clothesdata = CurrentClothesInDatabase[absindex];

				Clothesdata.Clothes_Type = ClothesType_Edit_CB.SelectedIndex;
				Clothesdata.Rarity = ClothesRarity_Edit_CB.SelectedIndex;
				Clothesdata.Weight = weightval;
				Clothesdata.Function_PTR = ClothesFuncPTR_Edit_TB.Text; // 1.0.0.3v
																																//TODO: Add the wepdata weakness after that table is done in this tool.
																																//itemdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ClothesMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				Clothesdata.Elemental = magictypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `clothes` " +
											"SET " +
											String.Format("{0} = {1},", "elemental", Clothesdata.Elemental) +
											String.Format("{0} = {1},", "weight", Clothesdata.Weight) +
											String.Format("{0} = {1},", "clothes_type", Clothesdata.Clothes_Type) +
											String.Format("{0} = '{1}',", "function_ptr", Clothesdata.Function_PTR) + //1.0.0.3v

											String.Format("{0} = {1},", "rarity", Clothesdata.Rarity) +
											String.Format("{0} = {1},", "stats_fk", Clothesdata.Stats_FK) +
											String.Format("{0} = {1}, ", "point_coeffiencts_fk", Clothesdata.Point_Coeffiencts_FK) +
											String.Format("{0} = {1} ", "weakness_strength_fk", Clothesdata.Weakness_Strength_FK) +
											String.Format("WHERE id='{0}'", Clothesdata.ID);
					_sqlite_conn.Query<Clothes>(Createsql);


					Base_Stats stats = (Base_Stats)CurrentClothesInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
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
					Createsql = "";
					Createsql = "UPDATE `base_stats` " +
											"SET " +
											String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +

											String.Format("{0} = {1},", "attack", base_stats.Attack) +
											String.Format("{0} = {1},", "defense", base_stats.Defense) +
											String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
											String.Format("{0} = {1},", "agility", base_stats.Agility) +
											String.Format("{0} = {1},", "morality", base_stats.Morality) +

											String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
											String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
											String.Format("{0} = {1},", "luck", base_stats.Luck) +
											String.Format("{0} = {1},", "risk", base_stats.Risk) +
											String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

											String.Format("WHERE id='{0}'", base_stats.ID);
					_sqlite_conn.Query<Base_Stats>(Createsql);


					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentClothesInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = Clothesdata.Weakness_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(ClothesMagicWeakness_Edit_IC, EMagicType.NONE, "AddClothesMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(ClothesMagicStrength_Edit_IC, EMagicType.NONE, "AddClothesMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(ClothesWeakness_Edit_IC, EWeaponType.NONE, "AddClothesWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(ClothesWeaponStrength_Edit_IC, EWeaponType.NONE, "AddClothesWeaknessStrength_CB");
					Createsql = "UPDATE `weaknesses_strengths` " +
											"SET " +
											String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
											String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
											String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
											String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

											String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);

					//Set up the point coefficent object!
					int pc_piece_types = 0;
					foreach (UIElement element in Clothes_Piece_Types_Edit_Grid.Children)
					{
						if (element is CheckBox cb)
						{
							if ((bool)(cb as CheckBox).IsChecked)
							{
								pc_piece_types += (int)Math.Pow(2, Grid.GetRow(cb));
							}
						}
					}

					Point_Coeffiencts pointCoeffiencts = new Point_Coeffiencts()
					{
						ID = Clothesdata.Point_Coeffiencts_FK,
						Max_Size = maxSize,
						Min_Size = minSize,
						Min_Quality = minQual,
						Max_Quality = maxQual,
						Min_Rarity = minRare,
						Max_Rarity = maxRare,
						Min_Points = minPoints,
						Max_Points = maxPoints,
						possible_piece_types = pc_piece_types
					};
					Createsql = "UPDATE `point_coeffiencts` " +
					            "SET " +
					            String.Format("{0} = {1},", "min_size", pointCoeffiencts.Min_Size) +
					            String.Format("{0} = {1},", "max_size", pointCoeffiencts.Max_Size) +
					            String.Format("{0} = {1},", "min_quality", pointCoeffiencts.Min_Quality) +
					            String.Format("{0} = {1}, ", "max_quality", pointCoeffiencts.Max_Quality) +
					            String.Format("{0} = {1}, ", "min_rarity", pointCoeffiencts.Min_Rarity) +
					            String.Format("{0} = {1}, ", "max_rarity", pointCoeffiencts.Max_Rarity) +
					            String.Format("{0} = {1}, ", "min_points", pointCoeffiencts.Min_Points) +
					            String.Format("{0} = {1}, ", "max_points", pointCoeffiencts.Max_Points) +
					            String.Format("{0} = {1} ", "possible_piece_types", pointCoeffiencts.possible_piece_types) +

					            String.Format("WHERE id='{0}'", pointCoeffiencts.ID);
					_sqlite_conn.Query<Point_Coeffiencts>(Createsql);


					//Delete all the associated keys
					#region Key Deletion And reinsertion
					#region Effect/Trait

					Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", Clothesdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in Clothesdata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in Clothesdata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", Clothesdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in Clothesdata.ClothesSkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("clothes Read from database FAILURE {0}:", ex.Message);
					SetOutputLog(String.Format("Loading/Reading Database [clothes] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		private void ClothesName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//We need to populate the data to the GUI.
			Clothes currentClothes = CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex];
			ClothesType_Edit_CB.SelectedItem = (EClothesType)currentClothes.Clothes_Type;
			ClothesRarity_Edit_CB.SelectedItem = (ERarityType)currentClothes.Rarity;
			ClothesFuncPTR_Edit_TB.Text = currentClothes.Function_PTR;        //1.0.0.3v

			#region Elemental "Binding"
			//reset
			SetMagicTypesData(ClothesMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
			SetMagicTypesData(ClothesMagicTypesEquip_Edit_IC, currentClothes.Elemental, EMagicType.NONE, false);
			WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
			#endregion

			#region Base Stats
			String Createsql = "SELECT * FROM `base_stats`;";
			List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			Base_Stats baseStats = bsList.Single(x => x.ID == currentClothes.Stats_FK);
			currentClothes.Stats = baseStats;
			#endregion
			#region Weaknesses and strengths
			Createsql = "SELECT * FROM `weaknesses_strengths`;";
			List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

			weaknesses_strengths wsData = wsList.Single(x => x.ID == currentClothes.Weakness_Strength_FK);
			currentClothes.WeaknessAndStrengths = wsData;
			#endregion

			#region Stats
			ClothesMaxHP_Edit_TB.Text = currentClothes.Stats.Max_Health.ToString();
			ClothesMaxMP_Edit_TB.Text = currentClothes.Stats.Max_Mana.ToString();


			ClothesAtk_Edit_TB.Text = currentClothes.Stats.Attack.ToString();
			ClothesDef_Edit_TB.Text = currentClothes.Stats.Defense.ToString();
			ClothesDex_Edit_TB.Text = currentClothes.Stats.Dexterity.ToString();
			ClothesAgl_Edit_TB.Text = currentClothes.Stats.Agility.ToString();
			ClothesMor_Edit_TB.Text = currentClothes.Stats.Morality.ToString();

			ClothesWis_Edit_TB.Text = currentClothes.Stats.Wisdom.ToString();
			ClothesRes_Edit_TB.Text = currentClothes.Stats.Resistance.ToString();
			ClothesLuc_Edit_TB.Text = currentClothes.Stats.Luck.ToString();
			ClothesRsk_Edit_TB.Text = currentClothes.Stats.Risk.ToString();
			ClothesItl_Edit_TB.Text = currentClothes.Stats.Intelligence.ToString();
			#endregion

			//Check boxes time!
			#region Weaknesses and strengths

			#region Elemental "Binding"
			//reset
			SetItemControlCheckboxData(ClothesMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddClothesMagWeak_CB", true);
			SetItemControlCheckboxData(ClothesMagicWeakness_Edit_IC, currentClothes.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddClothesMagWeak_CB", false);

			SetItemControlCheckboxData(ClothesMagicStrength_Edit_IC, null, EMagicType.NONE, "AddClothesMagicStrength_CB", true);
			SetItemControlCheckboxData(ClothesMagicStrength_Edit_IC, currentClothes.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddClothesMagicStrength_CB", false);
			ClothesMagicWeakness_Edit_IC.UpdateLayout();
			ClothesMagicStrength_Edit_IC.UpdateLayout();
			#endregion

			#region Weakness & Strengths "Binding"
			//reset
			SetItemControlCheckboxData(ClothesWeakness_Edit_IC, null, EMagicType.NONE, "AddClothesWeak_CB", true);
			SetItemControlCheckboxData(ClothesWeakness_Edit_IC, currentClothes.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddClothesWeak_CB", false);

			SetItemControlCheckboxData(ClothesWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddClothesWeaknessStrength_CB", true);
			SetItemControlCheckboxData(ClothesWeaponStrength_Edit_IC, currentClothes.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddClothesWeaknessStrength_CB", false);
			ClothesWeaponStrength_Edit_IC.UpdateLayout();
			ClothesWeaponStrength_Edit_IC.UpdateLayout();
			#endregion
			#endregion


			#region point coefficents
			Createsql = "SELECT * FROM `point_coeffiencts`;";
			List<Point_Coeffiencts> pcList = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);

			Point_Coeffiencts pointCoeffiencts = pcList.Single(x => x.ID == currentClothes.Point_Coeffiencts_FK);
			currentClothes.PointCoeffiencts = pointCoeffiencts;
			Clothes_pointCo_Min_Size_Edit_TB.Text = pointCoeffiencts.Min_Size.ToString();
			Clothes_pointCo_Max_Size_Edit_TB.Text = pointCoeffiencts.Max_Size.ToString();
			Clothes_pointCo_Min_Qual_Edit_TB.Text = pointCoeffiencts.Min_Quality.ToString();
			Clothes_pointCo_Max_Qual_Edit_TB.Text = pointCoeffiencts.Max_Quality.ToString();
			Clothes_pointCo_Min_Rare_Edit_TB.Text = pointCoeffiencts.Min_Rarity.ToString();
			Clothes_pointCo_Max_Rare_Edit_TB.Text = pointCoeffiencts.Max_Rarity.ToString();
			Clothes_pointCo_Min_Points_Edit_TB.Text = pointCoeffiencts.Min_Points.ToString();
			Clothes_pointCo_Max_Points_Edit_TB.Text = pointCoeffiencts.Max_Points.ToString();

			//Set up the point coefficent checkboxes
			foreach (UIElement element in Clothes_Piece_Types_Edit_Grid.Children)
			{
				if (element is CheckBox cb)
				{
					int grid = Grid.GetRow(cb);
					if ((pointCoeffiencts.possible_piece_types & (0x1 << grid)) > 0)
					{
						cb.IsChecked = true;
					}
					else cb.IsChecked = false;

				}
			}
			#endregion


			#region Effect/Traits Binding

			foreach (ModifierData modd in currentClothes.Effects)
			{
				ClothesEffectEquip_Edit_IC.Items.Add(modd.Id);
			}
			foreach (ModifierData modd in currentClothes.Traits)
			{
				ClothesTraitsEquip_Edit_IC.Items.Add(modd.Id);
			}
			#endregion

			#region Skills Binding

			foreach (Skill skill in currentClothes.ClothesSkills)
			{
				ClothesSkillsEquip_Edit_IC.Items.Add(skill);
			}

			#endregion

		}

	}
}
