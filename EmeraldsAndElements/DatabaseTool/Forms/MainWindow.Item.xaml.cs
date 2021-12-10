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
						ModifierData moddata = CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
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
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message);
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
			}
		}


		private void itemEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemName_Edit_CB.SelectedIndex >= 0)
			{
				if (ItemEffectEquip_Edit_IC.Items.Count < 3)
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex].Id;
					if (!ItemEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						ItemEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Effects.Add(
							CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex]
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
					string traitname = CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_Add_CB.SelectedIndex].Id;
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
					string traitname = CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!ItemTraitsEquip_Edit_IC.Items.Contains(traitname))
					{
						ItemTraitsEquip_Edit_IC.Items.Add(traitname);
						CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Traits.Add(
						CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex]
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
					string traitname = CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Add_CB.SelectedIndex].Id;
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
			if (ItemName_Add_TB.Text.Length > 0 && int.TryParse(ItemInflictingVal_Add_TB.Text, out int inflictval) &&
					ItemWeaponType_Add_CB.SelectedIndex >= 0 && ItemRarity_Add_CB.SelectedIndex >= 0 &&
					int.TryParse(ItemAoEWidth_Add_TB.Text, out int AoE_W_Val) && int.TryParse(ItemAoEHeight_Add_TB.Text, out int AoE_H_Val)) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					//Set up the weapon object!
					Items item = new Items()
					{
						ID = ItemName_Add_TB.Text,
						bDamage = (bool)ItemIsDamage_Add_CB.IsChecked,
						//Inflicting_Value = inflictval, // This doesn't exist anymore. was phased out by Str&weak stats
						Weapon_Type = (int)((EWeaponType)ItemWeaponType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)ItemRarity_Add_CB.SelectedValue),
						bAllies = (bool)ItemAllies_Add_CB.IsChecked, //1.0.0.2v
						AoE_W = AoE_W_Val, //1.0.0.2v
						AoE_H = AoE_H_Val, //1.0.0.2v
						Function_PTR = ItemFuncPTR_Add_TB.Text //1.0.0.3v
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					int i = 0;
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
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [items] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [items] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		//Updated this method to include the function pointer, AoE, and allies -AM 9/4/2020 1.0.0.3v
		private void ItemName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
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
			if (int.TryParse(ItemInflictingVal_Edit_TB.Text, out int inflictval) &&
					int.TryParse(ItemAoEWidth_Edit_TB.Text, out int AoE_W_Val) && //1.0.0.2v
					int.TryParse(ItemAoEHeight_Edit_TB.Text, out int AoE_H_Val)) //1.0.0.2v
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

											String.Format("{0} = {1},", "rarity", itemdata.Rarity) +
											String.Format("{0} = {1} ", "weak_strength_fk", itemdata.Weakness_Strength_FK) +
											String.Format("WHERE id='{0}'", itemdata.ID);
					_sqlite_conn.Query<Item>(Createsql);

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

				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

	}
}
