using BixBite.Combat;
using BixBite.Items;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Forms.DatabaseTool
{


	public partial class MainWindow
	{

		public ObservableCollection<Item> CurrentItemsInDatabase { get; set; }

		public void MainWindow_Items()
		{
			CurrentItemsInDatabase = new ObservableCollection<Item>();

		}

		private void LoadItemsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			ItemName_Edit_CB.ItemsSource = null;

			PartyMemberItems_Add_CB.ItemsSource = null;
			PartyMemberItems_Edit_CB.ItemsSource = null;
			CurrentItemsInDatabase.Clear();

			EnemyItems_Add_CB.ItemsSource = null;
			EnemyItemsDrops_Add_CB.ItemsSource = null;
			EnemyItems_Edit_CB.ItemsSource = null;
			EnemyItemsDrops_Edit_CB.ItemsSource = null;

			RecipeIngredientItem_Add_CB.ItemsSource = null;


			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `items`;");

				IEnumerable<Items> varlist = _sqlite_conn.Query<Items>(Createsql);
				foreach (Item item in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys
					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", item.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata = CurrentGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
							item.Effects.Add(moddata);
						else
							item.Traits.Add(moddata);
					}

					#endregion

					#region Skills
					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", item.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						item.ItemSkills.Add(skilldata);
					}
					#endregion
					#endregion
					CurrentItemsInDatabase.Add(item);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Weapons Read from database FAILURE {0}:", ex.Message);
				SetOutputLog(String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message));
			}
			finally
			{
				ItemName_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				PartyMemberItems_Add_CB.ItemsSource = CurrentItemsInDatabase;
				PartyMemberItems_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItems_Add_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItemsDrops_Add_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItems_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItemsDrops_Edit_CB.ItemsSource = CurrentItemsInDatabase;

				RecipeIngredientItem_Add_CB.ItemsSource = CurrentItemsInDatabase;
				RecipeIngredientItem_Edit_CB.ItemsSource = CurrentItemsInDatabase;
			}
		}


		private void itemEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemName_Edit_CB.SelectedIndex >= 0)
			{
				if (ItemEffectEquip_Edit_IC.Items.Count < 3)
				{
					string effectname = CurrentGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex].Id;
					if (!ItemEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						ItemEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Effects.Add(
							CurrentGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex]
						);
						ItemEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}


		private void itemEquipEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemEquipEffects_Add_CB.SelectedIndex >= 0)
			{
				if (ItemEffectEquip_Add_IC.Items.Count < 3)
				{
					string traitname = CurrentGameplayModifiersInDatabase_Effects[ItemEquipEffects_Add_CB.SelectedIndex].Id;
					if (!ItemEffectEquip_Add_IC.Items.Contains(traitname))
					{
						ItemEffectEquip_Add_IC.Items.Add(traitname);
					}
				}
			}
		}

		private void RemoveItemEffect_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemEffectEquip_Add_IC.Items.IndexOf(item);

			ItemEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveItemEffect_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemEffectEquip_Edit_IC.Items.IndexOf(item);

			ItemEffectEquip_Edit_IC.Items.RemoveAt(index);

			CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);

		}

		private void itemEquipTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemName_Edit_CB.SelectedIndex >= 0)
			{
				if (ItemTraitsEquip_Edit_IC.Items.Count < 3)
				{
					string traitname = CurrentGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!ItemTraitsEquip_Edit_IC.Items.Contains(traitname))
					{
						ItemTraitsEquip_Edit_IC.Items.Add(traitname);
						CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Traits.Add(
						CurrentGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex]
					);
						ItemTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void itemEquipTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemEquipTraits_Add_CB.SelectedIndex >= 0)
			{
				if (ItemTraitsEquip_Add_IC.Items.Count < 3)
				{
					string traitname = CurrentGameplayModifiersInDatabase_Traits[ItemEquipTraits_Add_CB.SelectedIndex].Id;
					if (!ItemTraitsEquip_Add_IC.Items.Contains(traitname))
					{
						ItemTraitsEquip_Add_IC.Items.Add(traitname);
					}
				}
			}
		}

		private void RemoveItemTrait_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemTraitsEquip_Add_IC.Items.IndexOf(item);

			ItemTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveItemTrait_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemTraitsEquip_Edit_IC.Items.IndexOf(item);

			ItemTraitsEquip_Edit_IC.Items.RemoveAt(index);

			CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);
		}


		//Updated this method to include the AoE and targeting variables -AM 8/29/2020 1.0.0.2v
		//Updated this method to include the function pointer name string variable -AM 9/4/2020 1.0.0.3v
		private void AddItemToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (ItemName_Add_TB.Text.Length > 0 &&
					ItemWeaponType_Add_CB.SelectedIndex >= 0 && ItemRarity_Add_CB.SelectedIndex >= 0 && int.TryParse(ItemWeight_Add_TB.Text, out int weight) &&
					int.TryParse(ItemAoEWidth_Add_TB.Text, out int AoE_W_Val) && int.TryParse(ItemAoEHeight_Add_TB.Text, out int AoE_H_Val) &&

					int.TryParse(ItemMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(ItemMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(ItemAtk_Add_TB.Text, out int atkResult) &&
					int.TryParse(ItemDef_Add_TB.Text, out int defResult) &&
					int.TryParse(ItemDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(ItemAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(ItemMor_Add_TB.Text, out int morResult) &&
					int.TryParse(ItemWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(ItemRes_Add_TB.Text, out int resResult) &&
					int.TryParse(ItemLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(ItemRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(ItemItl_Add_TB.Text, out int itlResult) &&

					int.TryParse(Item_pointCo_Min_Size_Add_TB.Text, out int minSize) &&
					int.TryParse(Item_pointCo_Max_Size_Add_TB.Text, out int maxSize) &&
					int.TryParse(Item_pointCo_Min_Qual_Add_TB.Text, out int minQual) &&
					int.TryParse(Item_pointCo_Max_Qual_Add_TB.Text, out int maxQual) &&
					int.TryParse(Item_pointCo_Min_Rare_Add_TB.Text, out int minRare) &&
					int.TryParse(Item_pointCo_Max_Rare_Add_TB.Text, out int maxRare) &&
					int.TryParse(Item_pointCo_Min_Points_Add_TB.Text, out int minPoints) &&
					int.TryParse(Item_pointCo_Max_Points_Add_TB.Text, out int maxPoints)

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
						ContentPresenter c = ((ContentPresenter)ItemsMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ItemsWeaponWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemweaponWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ItemsMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicStrength_CB", c);

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
						ContentPresenter c = ((ContentPresenter)ItemsWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemWeaknessStrength_CB", c);

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

					//Set up the Point coefficents object.
					Createsql = "SELECT * FROM `point_coeffiencts`;";
					List<Point_Coeffiencts> pcList = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);
					int newID_stat_pc = (pcList.Count == 0 ? 0 : pcList.Max(x => x.ID));

					//Set up the point coefficent object!
					int pc_piece_types = 0;
					foreach (UIElement element in Item_Piece_Types_Add_Grid.Children)
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

					//Set up the item object!
					Items item = new Items()
					{
						ID = ItemName_Add_TB.Text,
						bDamage = (bool)ItemIsDamage_Add_CB.IsChecked,
						Weapon_Type = (int)((EWeaponType)ItemWeaponType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)ItemRarity_Add_CB.SelectedValue),
						bAllies = (bool)ItemAllies_Add_CB.IsChecked, //1.0.0.2v
						AoE_W = AoE_W_Val, //1.0.0.2v
						AoE_H = AoE_H_Val, //1.0.0.2v
						Weight = weight,
						Point_Coeffiencts_FK = pointCoeffiencts.ID,
						Weakness_Strength_FK = weakstrToAdd.ID,
						Stats_FK = basestat.ID,
						Function_PTR = ItemFuncPTR_Add_TB.Text //1.0.0.3v
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					item.Elemental = magictypesval;
					#endregion

					#region Item Types
					i = 0;
					int itemstypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EItemType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							itemstypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					item.Item_Type = itemstypesval;
					#endregion

					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "items", item.ID);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "items", item.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "items", item.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(item);
					Console.WriteLine("RowID Val: {0}", retval);

					SetOutputLog(String.Format("Successfully added item to DB: {0}", retval));
				}
				catch (Exception ex)
				{
					Console.WriteLine("Items write from database [items] FAILURE | {0}", ex.Message);
					SetOutputLog(String.Format("Loading/Writing Database [items] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		//Updated this method to include the function pointer, AoE, and allies -AM 9/4/2020 1.0.0.3v
		private void ItemName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			//Clear the equips
			ItemEffectEquip_Edit_IC.Items.Clear();
			ItemTraitsEquip_Edit_IC.Items.Clear();
			itemSkillsEquip_Edit_IC.Items.Clear();

			//We need to populate the data to the GUI.
			Item currentItem = CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex];
			ItemIsDamage_Edit_CB.IsChecked = currentItem.bDamage;
			//ItemInflictingVal_Edit_TB.Text = currentItem.Inflicting_Value.ToString(); // This doesn't exist anymore. was phased out by Str&weak stats
			ItemWeaponType_Edit_CB.SelectedItem = (EWeaponType)currentItem.Weapon_Type;
			ItemRarity_Edit_CB.SelectedItem = (ERarityType)currentItem.Rarity;
			ItemAoEWidth_Edit_TB.Text = currentItem.AoE_W.ToString();   //1.0.0.3v
			ItemAoEHeight_Edit_TB.Text = currentItem.AoE_H.ToString();  //1.0.0.3v
			ItemFuncPTR_Edit_TB.Text = currentItem.Function_PTR;        //1.0.0.3v
			ItemAllies_Edit_CB.IsChecked = currentItem.bAllies;         //1.0.0.3v
			ItemWeight_Edit_TB.Text = currentItem.Weight.ToString();

			#region Base Stats
			String Createsql = "SELECT * FROM `base_stats`;";
			List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			Base_Stats baseStats = bsList.Single(x => x.ID == currentItem.Stats_FK);
			currentItem.Stats = baseStats;
			#endregion
			#region Weaknesses and strengths
			Createsql = "SELECT * FROM `weaknesses_strengths`;";
			List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

			weaknesses_strengths wsData = wsList.Single(x => x.ID == currentItem.Weakness_Strength_FK);
			currentItem.WeaknessAndStrengths = wsData;
			#endregion

			#region Stats
			ItemMaxHP_Edit_TB.Text = currentItem.Stats.Max_Health.ToString();
			ItemMaxMP_Edit_TB.Text = currentItem.Stats.Max_Mana.ToString();


			ItemAtk_Edit_TB.Text = currentItem.Stats.Attack.ToString();
			ItemDef_Edit_TB.Text = currentItem.Stats.Defense.ToString();
			ItemDex_Edit_TB.Text = currentItem.Stats.Dexterity.ToString();
			ItemAgl_Edit_TB.Text = currentItem.Stats.Agility.ToString();
			ItemMor_Edit_TB.Text = currentItem.Stats.Morality.ToString();

			ItemWis_Edit_TB.Text = currentItem.Stats.Wisdom.ToString();
			ItemRes_Edit_TB.Text = currentItem.Stats.Resistance.ToString();
			ItemLuc_Edit_TB.Text = currentItem.Stats.Luck.ToString();
			ItemRsk_Edit_TB.Text = currentItem.Stats.Risk.ToString();
			ItemItl_Edit_TB.Text = currentItem.Stats.Intelligence.ToString();
			#endregion

			//Check boxes time!
			#region Weaknesses and strengths

			#region Elemental "Binding"
			//reset
			SetItemControlCheckboxData(ItemsMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddItemMagWeak_CB", true);
			SetItemControlCheckboxData(ItemsMagicWeakness_Edit_IC, currentItem.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddItemMagWeak_CB", false);

			SetItemControlCheckboxData(ItemsMagicStrength_Edit_IC, null, EMagicType.NONE, "AddItemMagicStrength_CB", true);
			SetItemControlCheckboxData(ItemsMagicStrength_Edit_IC, currentItem.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddItemMagicStrength_CB", false);
			ItemsMagicWeakness_Edit_IC.UpdateLayout();
			ItemsMagicStrength_Edit_IC.UpdateLayout();
			#endregion

			#region Weakness & Strengths "Binding"
			//reset
			SetItemControlCheckboxData(ItemsWeaponWeakness_Edit_IC, null, EMagicType.NONE, "AddItemweaponWeak_CB", true);
			SetItemControlCheckboxData(ItemsWeaponWeakness_Edit_IC, currentItem.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddItemweaponWeak_CB", false);

			SetItemControlCheckboxData(ItemsWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddItemWeaknessStrength_CB", true);
			SetItemControlCheckboxData(ItemsWeaponStrength_Edit_IC, currentItem.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddItemWeaknessStrength_CB", false);
			ItemsWeaponStrength_Edit_IC.UpdateLayout();
			ItemsWeaponStrength_Edit_IC.UpdateLayout();
			#endregion

			#endregion

			#region point coefficents
			Createsql = "SELECT * FROM `point_coeffiencts`;";
			List<Point_Coeffiencts> pcList = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);

			Point_Coeffiencts pointCoeffiencts = pcList.Single(x => x.ID == currentItem.Point_Coeffiencts_FK);
			currentItem.PointCoeffiencts = pointCoeffiencts;
			Item_pointCo_Min_Size_Edit_TB.Text = pointCoeffiencts.Min_Size.ToString();
			Item_pointCo_Max_Size_Edit_TB.Text = pointCoeffiencts.Max_Size.ToString();
			Item_pointCo_Min_Qual_Edit_TB.Text = pointCoeffiencts.Min_Quality.ToString();
			Item_pointCo_Max_Qual_Edit_TB.Text = pointCoeffiencts.Max_Quality.ToString();
			Item_pointCo_Min_Rare_Edit_TB.Text = pointCoeffiencts.Min_Rarity.ToString();
			Item_pointCo_Max_Rare_Edit_TB.Text = pointCoeffiencts.Max_Rarity.ToString();
			Item_pointCo_Min_Points_Edit_TB.Text = pointCoeffiencts.Min_Points.ToString();
			Item_pointCo_Max_Points_Edit_TB.Text = pointCoeffiencts.Max_Points.ToString();

			//Set up the point coefficent checkboxes
			foreach (UIElement element in Item_Piece_Types_Edit_Grid.Children)
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

			#region Item Types
			SetItemsTypesData(ItemTypesEquip_Edit_IC, null, EItemType.NONE, true);
			SetItemsTypesData(ItemTypesEquip_Edit_IC, currentItem.Item_Type, EItemType.NONE, false);
			#endregion

			#region Elemental "Binding"
			//reset
			SetMagicTypesData(ItemMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
			SetMagicTypesData(ItemMagicTypesEquip_Edit_IC, currentItem.Elemental, EMagicType.NONE, false);
			WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
			#endregion

			#region Effect/Traits Binding

			foreach (ModifierData modd in currentItem.Effects)
			{
				ItemEffectEquip_Edit_IC.Items.Add(modd.Id);
			}
			foreach (ModifierData modd in currentItem.Traits)
			{
				ItemTraitsEquip_Edit_IC.Items.Add(modd.Id);
			}
			#endregion

			#region Skills Binding

			foreach (Skill skill in currentItem.ItemSkills)
			{
				itemSkillsEquip_Edit_IC.Items.Add(skill);
			}

			#endregion

		}

		/// Updated to include AoE, and target side variables -AM 8/29/2020 1.0.0.2v
		/// Updated this method to include the function pointer name string variable -AM 9/4/2020 1.0.0.3v
		private void UpdateItemInDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{

			if (
					int.TryParse(ItemAoEWidth_Edit_TB.Text, out int AoE_W_Val) && //1.0.0.2v
					int.TryParse(ItemAoEHeight_Edit_TB.Text, out int AoE_H_Val) &&
					int.TryParse(ItemMaxHP_Edit_TB.Text, out int maxHpResult) &&
					int.TryParse(ItemMaxMP_Edit_TB.Text, out int maxMPResult) &&

					int.TryParse(ItemAtk_Edit_TB.Text, out int atkResult) &&
					int.TryParse(ItemDef_Edit_TB.Text, out int defResult) &&
					int.TryParse(ItemDex_Edit_TB.Text, out int dexResult) &&
					int.TryParse(ItemAgl_Edit_TB.Text, out int aglResult) &&
					int.TryParse(ItemMor_Edit_TB.Text, out int morResult) &&
					int.TryParse(ItemWis_Edit_TB.Text, out int wisResult) &&
					int.TryParse(ItemRes_Edit_TB.Text, out int resResult) &&
					int.TryParse(ItemLuc_Edit_TB.Text, out int LucResult) &&
					int.TryParse(ItemRsk_Edit_TB.Text, out int RskResult) &&
					int.TryParse(ItemItl_Edit_TB.Text, out int itlResult) &&

					int.TryParse(Item_pointCo_Min_Size_Edit_TB.Text, out int minSize) &&
					int.TryParse(Item_pointCo_Max_Size_Edit_TB.Text, out int maxSize) &&
					int.TryParse(Item_pointCo_Min_Qual_Edit_TB.Text, out int minQual) &&
					int.TryParse(Item_pointCo_Max_Qual_Edit_TB.Text, out int maxQual) &&
					int.TryParse(Item_pointCo_Min_Rare_Edit_TB.Text, out int minRare) &&
					int.TryParse(Item_pointCo_Max_Rare_Edit_TB.Text, out int maxRare) &&
					int.TryParse(Item_pointCo_Min_Points_Edit_TB.Text, out int minPoints) &&
					int.TryParse(Item_pointCo_Max_Points_Edit_TB.Text, out int maxPoints)


					) //1.0.0.2v
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = ItemName_Edit_CB.SelectedIndex;
				Item itemdata = CurrentItemsInDatabase[absindex];

				itemdata.bDamage = (bool)ItemIsDamage_Edit_CB.IsChecked;
				// itemdata.Inflicting_Value = inflictval; // This doesn't exist anymore. was phased out by Str&weak stats
				itemdata.Weapon_Type = ItemWeaponType_Edit_CB.SelectedIndex;
				itemdata.Rarity = ItemRarity_Edit_CB.SelectedIndex;
				itemdata.AoE_H = AoE_H_Val; //1.0.0.2v
				itemdata.AoE_W = AoE_W_Val; //1.0.0.2v
				itemdata.Function_PTR = ItemFuncPTR_Edit_TB.Text; // 1.0.0.3v
				itemdata.bAllies = (bool)ItemAllies_Edit_CB.IsChecked; //1.0.0.2v
																															 //TODO: Add the wepdata weakness after that table is done in this tool.
																															 //itemdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ItemMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				itemdata.Elemental = magictypesval;
				#endregion

				#region Item Types
				i = 0;
				int itemstypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EItemType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						itemstypesval += (int)Math.Pow(2, i);
					}
					i++;
				}
				itemdata.Item_Type = itemstypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `items` " +
											"SET " +
											String.Format("{0} = {1},", "bdamage", itemdata.bDamage) +
											//String.Format("{0} = {1},", "inflicting_value", itemdata.Inflicting_Value) + // This doesn't exist anymore. was phased out by Str&weak stats
											String.Format("{0} = {1},", "elemental", itemdata.Elemental) +
											String.Format("{0} = {1},", "weapon_type", itemdata.Weapon_Type) +
											String.Format("{0} = {1},", "item_type", itemdata.Item_Type) +
											String.Format("{0} = {1},", "aoe_w", itemdata.AoE_W) + //1.0.0.2v
											String.Format("{0} = {1},", "aoe_h", itemdata.AoE_H) + //1.0.0.2v
											String.Format("{0} = {1},", "ballies", itemdata.bAllies) + //1.0.0.2v
											String.Format("{0} = '{1}',", "function_ptr", itemdata.Function_PTR) + //1.0.0.3v

											String.Format("{0} = {1}, ", "rarity", itemdata.Rarity) +
											String.Format("{0} = {1}, ", "weakness_strength_fk", itemdata.Weakness_Strength_FK) +
											String.Format("{0} = {1}, ", "stats_fk", itemdata.Stats_FK) +
											String.Format("{0} = {1} ", "point_coeffiencts_fk", itemdata.Point_Coeffiencts_FK) +
											String.Format("WHERE id='{0}'", itemdata.ID);
					_sqlite_conn.Query<Item>(Createsql);

					Base_Stats stats = (Base_Stats)CurrentItemsInDatabase[absindex].Stats;
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
											String.Format("{0} = {1},", "current_health", base_stats.Current_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +
											String.Format("{0} = {1},", "current_mana", base_stats.Current_Mana) +

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


					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentItemsInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = itemdata.Weakness_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(ItemsMagicWeakness_Edit_IC, EMagicType.NONE, "AddItemMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(ItemsMagicStrength_Edit_IC, EMagicType.NONE, "AddItemMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(ItemsWeaponWeakness_Edit_IC, EWeaponType.NONE, "AddItemweaponWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(ItemsWeaponStrength_Edit_IC, EWeaponType.NONE, "AddItemWeaknessStrength_CB");
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
					foreach (UIElement element in Item_Piece_Types_Edit_Grid.Children)
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
						ID = itemdata.Point_Coeffiencts_FK,
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

					Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", itemdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in itemdata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in itemdata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", itemdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in itemdata.ItemSkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

					SetOutputLog(String.Format("Successfully updated Item Database Record: {0}", "1"));

				}
				catch (Exception ex)
				{
					Console.WriteLine("Item Update from database FAILURE {0}:", ex.Message);
					SetOutputLog(String.Format("Update Database [Item ] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


	}
}
