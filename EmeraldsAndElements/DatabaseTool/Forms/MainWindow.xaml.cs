using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BixBite;
using BixBite.Characters;
using BixBite.Combat;
using BixBite.Crafting;
using BixBite.Items;
using SQLite;

//Change Log////////////////////////////////////////////////////////////////////////////////////////
// VERSION 1.0.0.1: Fixed Skills Not showing correctly. And Erroring out if No modifier is linked.
// Added the Skill Function Pointer in the Edit Section of the skills tab. -AM
// VERSION 1.0.0.2: Fixed the skills and the Items sections. Added boxes and logic for the new 
// Area of Effect variable, as well as the new targeting boolean.
// VERSION 1.0.0.3: Added the function pointer string name variable to the GUI and queries.
// Fixed the name change text boxes for items and Skills to show the new variables for 1.0.0.2v & 1.0.0.3v
// VERSION 1.0.0.4: Added the Accessories, and Clothes tabs, and database support

// VERSION 1.0.0.5 Organization, and added the recipe tab, and database support
//Change Log////////////////////////////////////////////////////////////////////////////////////////



namespace Forms.DatabaseTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window , INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		#region Delegates

		#endregion

		#region Fields

		public SQLiteConnection _sqlite_conn;
		public static String SQLDatabasePath { get; set; }



		#endregion

		#region Properties


		public ObservableCollection<FollowUpAttack> CurrentFollowUpAttacksInDatabase { get; set; }

		public ObservableCollection<Point_Coeffiencts> CurrentPointCoeffiencts { get; set; }

		public ObservableCollection<Inventory> CurrentInventoryItems { get; set; }

		#endregion


		public MainWindow()
		{
			InitializeComponent();

			//INIT collections
	
			MainWindow_Jobs();
			MainWindow_Skills();
			MainWindow_Enemies();
			MainWindow_PartyMembers();
			MainWindow_GameplayModifiers();
			MainWindow_Items();
			MainWindow_Weapons();
			MainWindow_Accessories();
			MainWindow_Clothes();
			MainWindow_Recipes();

			CurrentInventoryItems = new ObservableCollection<Inventory>();
			//CurrentPointCoeffiencts = new ObservableCollection<Point_Coeffiencts>();
			//LoadPointCoefficentsFromDatabase();

			DataContext = this;

		}

	

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			LoadAddGameplayModifiersGrids();
			LoadMagicTypesItemControls();
			LoadWeaponTypesItemsControl();
			LoadCreationTypeItemsControl();
			LoadItemGrid();
		}

		private void LoadInventoryItemsFromDatabase()
		{
			String Createsql = String.Empty;
			Createsql = ("SELECT * FROM `inventory`;");

			IEnumerable<Inventory> varlist = _sqlite_conn.Query<Inventory>(Createsql);
			foreach (Inventory inventoryItem in varlist.ToList())
			{
				CurrentInventoryItems.Add(inventoryItem);
			}
		}

		private void LoadItemGrid()
		{
			foreach (EItemType val in Enum.GetValues(typeof(EItemType)))
			{
				if(val == 0) continue;
				ItemTypesEquip_Add_IC.Items.Add(val);
				ItemTypesEquip_Edit_IC.Items.Add(val);
			}
		}

		private void LoadPointCoefficentsFromDatabase()
		{
			String Createsql = String.Empty;
			Createsql = ("SELECT * FROM `point_coeffiencts`;");

			IEnumerable<Point_Coeffiencts> varlist = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);
			foreach (Point_Coeffiencts pointCoeffiencts in varlist.ToList())
			{
				CurrentPointCoeffiencts.Add(pointCoeffiencts);
			}
		}

		#region Load/Add Weapon Types

		private void LoadWeaponTypesItemsControl()
		{
			foreach (EWeaponType val in Enum.GetValues(typeof(EWeaponType)))
			{
				if (val == 0) continue;
				PartyMembersWeaponWeakness_Add_IC.Items.Add(val);
				PartyMembersWeaponStrength_Add_IC.Items.Add(val);
				PartyMembersWeaponWeakness_Edit_IC.Items.Add(val);
				PartyMembersWeaponStrength_Edit_IC.Items.Add(val);

				EnemyWeaponWeakness_Add_IC.Items.Add(val);
				EnemyWeaponStrength_Add_IC.Items.Add(val);
				EnemyWeaponWeakness_Edit_IC.Items.Add(val);
				EnemyWeaponStrength_Edit_IC.Items.Add(val);

				ItemsWeaponWeakness_Add_IC.Items.Add(val);
				ItemsWeaponStrength_Add_IC.Items.Add(val);
				ItemsWeaponWeakness_Edit_IC.Items.Add(val);
				ItemsWeaponStrength_Edit_IC.Items.Add(val);

				AccessoriesWeakness_Add_IC.Items.Add(val);
				AccessoriesWeaponStrength_Add_IC.Items.Add(val);
				AccessoriesWeakness_Edit_IC.Items.Add(val);
				AccessoriesWeaponStrength_Edit_IC.Items.Add(val);

				ClothesWeakness_Add_IC.Items.Add(val);
				ClothesWeaponStrength_Add_IC.Items.Add(val);
				ClothesWeakness_Edit_IC.Items.Add(val);
				ClothesWeaponStrength_Edit_IC.Items.Add(val);

				WeaponsWeakness_Edit_IC.Items.Add(val);
				WeaponsWeaponStrength_Edit_IC.Items.Add(val);
				WeaponsWeakness_Add_IC.Items.Add(val);
				WeaponsWeaponStrength_Add_IC.Items.Add(val);

				RecipesWeakness_Add_IC.Items.Add(val);
				RecipesStrength_Add_IC.Items.Add(val);
				RecipesWeakness_Edit_IC.Items.Add(val);
				RecipesStrength_Edit_IC.Items.Add(val);


			}
		}

		#endregion

		#region Load/Add Magic Types
		private void LoadMagicTypesItemControls()
		{
			foreach (EMagicType val in Enum.GetValues(typeof(EMagicType)))
			{
				if (val == 0) continue;
				WeaponMagicTypesEquip_Add_IC.Items.Add(val);
				WaponsMagicTypesEquip_Edit_IC.Items.Add(val);

				ItemMagicTypesEquip_Add_IC.Items.Add(val);
				ItemMagicTypesEquip_Edit_IC.Items.Add(val);

				SkillMagicTypesEquip_Add_IC.Items.Add(val);
				SkillMagicTypesEquip_Edit_IC.Items.Add(val);

				PartyMembersMagicWeakness_Add_IC.Items.Add(val);
				PartyMembersMagicStrength_Add_IC.Items.Add(val);
				PartyMembersMagicWeakness_Edit_IC.Items.Add(val);
				PartyMembersMagicStrength_Edit_IC.Items.Add(val);

				EnemyMagicWeakness_Add_IC.Items.Add(val);
				EnemyMagicStrength_Add_IC.Items.Add(val);
				EnemyMagicWeakness_Edit_IC.Items.Add(val);
				EnemyMagicStrength_Edit_IC.Items.Add(val);


				ItemsMagicWeakness_Add_IC.Items.Add(val);
				ItemsMagicStrength_Add_IC.Items.Add(val);
				ItemsMagicWeakness_Edit_IC.Items.Add(val);
				ItemsMagicStrength_Edit_IC.Items.Add(val);

				AccessoryMagicTypesEquip_Add_IC.Items.Add(val);
				AccessoryMagicTypesEquip_Edit_IC.Items.Add(val);

				AccessoriesMagicWeakness_Add_IC.Items.Add(val);
				AccessoriesMagicStrength_Add_IC.Items.Add(val);
				AccessoriesMagicWeakness_Edit_IC.Items.Add(val);
				AccessoriesMagicStrength_Edit_IC.Items.Add(val);

				ClothesMagicTypesEquip_Add_IC.Items.Add(val);
				ClothesMagicTypesEquip_Edit_IC.Items.Add(val);

				ClothesMagicWeakness_Add_IC.Items.Add(val);
				ClothesMagicStrength_Add_IC.Items.Add(val);
				ClothesMagicWeakness_Edit_IC.Items.Add(val);
				ClothesMagicStrength_Edit_IC.Items.Add(val);

				WeaponsMagicWeakness_Edit_IC.Items.Add(val);
				WeaponsMagicStrength_Edit_IC.Items.Add(val);
				WeaponsMagicWeakness_Add_IC.Items.Add(val);
				WeaponsMagicStrength_Add_IC.Items.Add(val);

				RecipesMagicWeakness_Add_IC.Items.Add(val);
				RecipesMagicStrength_Add_IC.Items.Add(val);
				RecipesMagicWeakness_Edit_IC.Items.Add(val);
				RecipesMagicStrength_Edit_IC.Items.Add(val);

			}
		}
		#endregion

		#region Creation Type

		private void LoadCreationTypeItemsControl()
		{
			foreach (ECreationTypes val in Enum.GetValues(typeof(ECreationTypes)))
			{
				if (val == 0) continue;
				RecipesCreationType_Add_IC.Items.Add(val);
				RecipesCreationType_Edit_IC.Items.Add(val);
			}
		}

		private void AddRecipeCreationType_CB_OnClick(object sender, RoutedEventArgs e)
		{

		}

		#endregion





		private void ChangeGridToAdd(object sender, RoutedEventArgs e)
		{
			TabItem CurTabItem = ((sender as Button).Parent as Grid).Parent as TabItem;
			if (CurTabItem != null)
			{
				switch (CurTabItem.Header)
				{
					case ("FollowUpAttacks"):
						AddGrid_FollowupAttacks.Visibility = Visibility.Visible;
						EditGrid_FollowupAttacks.Visibility = Visibility.Hidden;
						break;
					case ("Jobs"):
						AddGrid_Jobs.Visibility = Visibility.Visible;
						EditGrid_Jobs.Visibility = Visibility.Hidden;
						break;
					case ("Base Stats"):
						throw new NotImplementedException();
						break;
					case ("Weaknesses And Strengths"):
						throw new NotImplementedException();
						break;
					case ("Skills"):
						Skills_Add_Grid.Visibility = Visibility.Visible;
						Skills_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Enemy"):
						Enemy_Add_Grid.Visibility = Visibility.Visible;
						Enemy_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Party Member"):
						PartyMember_Add_Grid.Visibility = Visibility.Visible;
						PartyMember_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("GamePlay Modifiers"):
						GameplayModifiers_Add_Grid.Visibility = Visibility.Visible;
						GameplayModifiers_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Items"):
						Item_Edit_Grid.Visibility = Visibility.Hidden;
						Item_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Accessories"):
						Accessory_Edit_Grid.Visibility = Visibility.Hidden;
						Accessory_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Clothes"):
						Clothes_Edit_Grid.Visibility = Visibility.Hidden;
						Clothes_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Weapons"):
						Weapons_Add_Grid.Visibility = Visibility.Visible;
						Weapons_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Recipes"):
						Add_Recipe_Grid.Visibility = Visibility.Visible;
						Edit_Recipe_Grid.Visibility = Visibility.Hidden;
						break;
					default:
						throw new NoNullAllowedException();
						break;
				}
			}
		}

		private void ChangeGridToEdit(object sender, RoutedEventArgs e)
		{
			TabItem CurTabItem = ((sender as Button).Parent as Grid).Parent as TabItem;
			if (CurTabItem != null)
			{
				switch (CurTabItem.Header)
				{
					case ("FollowUpAttacks"):
						AddGrid_FollowupAttacks.Visibility = Visibility.Hidden;
						EditGrid_FollowupAttacks.Visibility = Visibility.Visible;
						break;
					case ("Jobs"):
						AddGrid_Jobs.Visibility = Visibility.Hidden;
						EditGrid_Jobs.Visibility = Visibility.Visible;
						break;
					case ("Base Stats"):
						throw new NotImplementedException();
						break;
					case ("Weaknesses And Strengths"):
						throw new NotImplementedException();
						break;
					case ("Skills"):
						Skills_Add_Grid.Visibility = Visibility.Hidden;
						Skills_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Enemy"):
						Enemy_Add_Grid.Visibility = Visibility.Hidden;
						Enemy_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Party Member"):
						PartyMember_Add_Grid.Visibility = Visibility.Hidden;
						PartyMember_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("GamePlay Modifiers"):
						GameplayModifiers_Add_Grid.Visibility = Visibility.Hidden;
						GameplayModifiers_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Items"):
						Item_Edit_Grid.Visibility = Visibility.Visible;
						Item_Add_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Accessories"):
						Accessory_Edit_Grid.Visibility = Visibility.Visible;
						Accessory_Add_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Clothes"):
						Clothes_Edit_Grid.Visibility = Visibility.Visible;
						Clothes_Add_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Weapons"):
						Weapons_Add_Grid.Visibility = Visibility.Hidden;
						Weapons_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Recipes"):
						Add_Recipe_Grid.Visibility = Visibility.Hidden;
						Edit_Recipe_Grid.Visibility = Visibility.Visible;
						break;
					default:
						//throw new NoNullAllowedException();
						break;
				}
			}
		}

		private void TestBTN_Click(object sender, RoutedEventArgs e)
		{
			EditJobsDB_LB.ItemsSource = null;
			//CurrentJobsInDatabase.Add(new Job(EJob.LuckyStar)
			//{
			//	LeftStance = EStanceType.Bloody,
			//	RightStance = EStanceType.Feather
			//});
			EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
			UpdateLayout();
		}

		

		private void BrowseForDataBase_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Emeralds and Elements Database",
				FileName = "", //default file name
				Filter = "SQLite database (*.db)|*.db",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			Databases_TB.Text = dlg.FileName;
			SQLDatabasePath = dlg.FileName;
			

			//Load the data to the screen from the database
			LoadJobsFromDatabase();
			LoadGameplayModifiersFromDatabase();
			LoadWeaponsFromDatabase();
			LoadItemsFromDatabase();
			LoadSkillsFromDatabase();
			LoadPartyMembersFromDatabase();
			LoadEnemyFromDatabase();
			LoadAccessoriesFromDatabase();
			LoadClothesFromDatabase();
			LoadRecipeFromDatabase();
			LoadInventoryItemsFromDatabase();

			SetOutputLog(String.Format("Successfully opened {0} database", dlg.FileName));

		}

		/// <summary>
		/// Sets the output logs message. AUTO TIMESTAMP
		/// </summary>
		/// <param name="s"></param>
		private void SetOutputLog(String s)
		{
			GlobalStatusLog_TB.Text = ( String.Format("{0}: {1}", DateTime.Now.ToString(), s));
		}

		private void InsertRecordIntoModifierKeys(ItemsControl IC,  SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Modifier_Keys modkey = new Modifier_Keys()
					{
						Req_Table = fromTableName,
						Req_Name =  fromrecorodID,
						Modifier_ID = ename,
					};
					_sqLiteConnection.Insert(modkey);
				}
			}
		}

		private void InsertRecordIntoSkillKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Skill_Keys modkey = new Skill_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Skill_ID = ename,
					};
					_sqLiteConnection.Insert(modkey);
				}
			}
		}

		private void InsertRecordIntoItemKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID, bool bdrop = false)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Item_Keys itemKey = new Item_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Item_ID = ename,
						bDrop = bdrop
					};
					_sqLiteConnection.Insert(itemKey);
				}
			}
		}

		private void InsertRecordIntoWeaponKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Weapon_Keys itemKey = new Weapon_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Weapon_ID = ename,
					};
					_sqLiteConnection.Insert(itemKey);
				}
			}
		}


		


		private void SetItemsTypesData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}

				i++;
			}
		}

		private void SetMagicTypesData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}

				i++;
			}
		}

		private void SetItemControlCheckboxData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, String CBName , bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName(CBName, c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}
				i++;
			}
		}





		

		private void DisplayGameplayModifiersToIC(ItemsControl desItemsControl, ModifierData mdata, ObservableCollection<Tuple<String, String>> CollectionToBind ,bool bReset = false)
		{
			//clear ptr and data to avoid runtime errors
			desItemsControl.ItemsSource = null;
			CollectionToBind.Clear();

			AddModifierDataTextblockToCollection(mdata, CollectionToBind);
			
			desItemsControl.ItemsSource = CollectionToBind;
			desItemsControl.UpdateLayout();
		}

		private void AddModifierDataTextblockToCollection(ModifierData mdata ,ObservableCollection<Tuple<String, String>> CollectionToBind)
		{
			//set the check boxes
			int i = 0;
			Enum etype = EChanceEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Chance_Modifiers != null && (en & (int)mdata.Chance_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ETurnEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Turn_Modifiers != null && (en & (int)mdata.Turn_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EDamageModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Damage_Modifiers != null && (en & (int)mdata.Damage_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ESeverityEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Severity_Modifiers != null && (en & (int)mdata.Severity_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}


			i = 0;
			etype = EMagicDamageModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Magic_Damage_Modifiers != null && (en & (int)mdata.Magic_Damage_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EMagicDefenseModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Magic_Defense_Modifiers != null && (en & (int)mdata.Magic_Defense_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EStatEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Stat_Modifiers != null && (en & (int)mdata.Stat_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EStatusEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Status_Effect_Modifiers != null && (en & (int)mdata.Status_Effect_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ENullifyStatusEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Nullify_Status_Effect_Modifiers != null && (en & (int)mdata.Nullify_Status_Effect_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ENonBattleEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.NonBattle_Modifiers != null && (en & (int)mdata.NonBattle_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ESpecialEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Special_Modifiers != null && (en & (int)mdata.Special_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}


		}

		
		private int GetBitWiseEnumeratedValFromIC(ItemsControl desIC, Enum etype, String CBName)
		{
			int i = 0;
			int returnval = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)desIC.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName(CBName, c);

				if ((bool)(vv as CheckBox).IsChecked)
				{
					returnval += (int)Math.Pow(2, i);
				}

				i++;
			}
			return returnval;
		}



		private void BrowseForFollowUpCSV_2_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_2_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void BrowseForFollowUpCSV_3_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_3_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void BrowseForFollowUpCSV_4_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_4_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void ClearFollowUps_BTN_Click(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void RunForFollowUpCSV_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (SQLDatabasePath == null)
			{
				SetOutputLog( "Please connect to the database First!");
				return;
			}
			if (CurrentFollowUpAttacksInDatabase.Count == 0)
			{
				FillFollowAttacks(FollowUpAttackCSV_2_TB.Text);
				FillFollowAttacks(FollowUpAttackCSV_3_TB.Text);
				FillFollowAttacks(FollowUpAttackCSV_4_TB.Text);

				//Now that its on the screen we need to put it in the database.
				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				foreach (FollowUpAttack followUpatk in CurrentFollowUpAttacksInDatabase)
				{
					FollowUp_Attacks temp = new FollowUp_Attacks()
					{
						Job_1 = followUpatk.Job_1,
						Job_2 = followUpatk.Job_2,
						Job_3 = followUpatk.Job_3,
						Job_4 = followUpatk.Job_4,
					};
					_sqlite_conn.Insert(temp);
				}
			}
		}


		private void FillFollowAttacks(String filepath)
		{
			FollowUpAttacks_LB.ItemsSource = null;

			using (var reader = new StreamReader(filepath))
			{
				List<string> LineData = new List<string>();
				while (!reader.EndOfStream)
				{
					String line = reader.ReadLine();
					LineData = line.Split(',').ToList();

					CurrentFollowUpAttacksInDatabase.Add(new FollowUpAttack()
					{
						Job_1 = LineData[0],
						Job_2 = LineData[1],
						Job_3 = LineData[2],
						Job_4 = LineData[3],
						Name = "NONE",
						Function_PTR = "NONE",
					});
				}
			}
			FollowUpAttacks_LB.ItemsSource = CurrentFollowUpAttacksInDatabase;
		}


		private void EditFollowUpAttacksInDB_BTN_Click(object sender, RoutedEventArgs e)
		{
			SetOutputLog(  "");
			//Now that its on the screen we need to put it in the database.
			String masterfile = (SQLDatabasePath);
			_sqlite_conn = new SQLiteConnection(masterfile);
			String Createsql = "";

			try
			{

				foreach (FollowUpAttack fua in CurrentFollowUpAttacksInDatabase)
				{
					Createsql = "UPDATE `followup_attacks` " +
											"SET " +
											String.Format("{0} = '{1}',", "name", fua.Name) +
											String.Format("{0} = '{1}' ", "function_ptr", fua.Function_PTR) +

											String.Format("WHERE job_1='{0}' AND job_2='{1}' AND job_3='{2}' AND job_4='{3}';",
												fua.Job_1, fua.Job_2, fua.Job_3, fua.Job_4);
					_sqlite_conn.Query<FollowUp_Attacks>(Createsql);
				}
			}
			catch (Exception ex)
			{
				SetOutputLog("Error Occured when updating follow up attacks: " + ex.Message);
			}
		}


		private void AddInventoryItem_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			ComboBox cb = InventoryItemToAdd_CB;
			String Createsql = "";
			if (cb.SelectedItem == null) return;
			else
			{
				Item selectedItem = (Item) cb.SelectedItem;
				//We need to fill up the selected items external data objects.
				Createsql = String.Format("SELECT * FROM `point_coeffiencts` where id = {0};", selectedItem.Point_Coeffiencts_FK  );
				List<Point_Coeffiencts> queryResults = _sqlite_conn.Query<Point_Coeffiencts>(Createsql);
				selectedItem.PointCoeffiencts = (Point_Coeffiencts)queryResults.First();

				Createsql = String.Format("SELECT * FROM `base_stats` where id = {0};", selectedItem.Stats_FK);
				List<BaseStats> queryResults_base = _sqlite_conn.Query<BaseStats>(Createsql);
				selectedItem.Stats = (BaseStats)queryResults_base.First();

				Createsql = String.Format("SELECT * FROM `weaknesses_strengths` where id = {0};", selectedItem.Weakness_Strength_FK);
				List<weaknesses_strengths> queryResults_weak = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
				selectedItem.WeaknessAndStrengths = (weaknesses_strengths)queryResults_weak.First();


				//Let's create the item we are trying to spawn into the game
				Createsql = String.Format("SELECT * FROM `created_items`;");
				List<Created_Items> pcList = _sqlite_conn.Query<Created_Items>(Createsql);
				int maxID = (pcList.Count == 0 ? 0 : pcList.Max(x => x.ID));

				Created_Items newItem = new Created_Items(selectedItem, Player.Instance){ID = maxID  +1};

				//At this point the actual newItem is set up with all the correct data and is ready for game use.
				//Let's put it in the database!
				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				

				try
				{
					_sqlite_conn.Insert(newItem);
					SetOutputLog(String.Format("Successfully added created_item to DB: {0}", newItem.ID));

					//If we have created a new REAL item. so let's add it to the inventory!
					Createsql = String.Format("SELECT * FROM `inventory`;");
					List<Inventory> invList = _sqlite_conn.Query<Inventory>(Createsql);
					int invMax = (invList.Count == 0 ? 0 : invList.Max(x => x.ID));

					Inventory inventoryItem = new Inventory()
					{
						ID = invMax+1,
						Req_ID = newItem.ID,
						Req_Name =  newItem.Name,
						Req_Type = "created_items"
					};
					_sqlite_conn.Insert(inventoryItem);
					SetOutputLog(String.Format("Successfully added new created_item to DB/INVENTORY: {0}", inventoryItem.Req_Name));
				}
				catch (Exception ex)
				{
					SetOutputLog("Error when added item to [created_Item] Table: " + ex.Message);
				}


			}
		}
	}


	/// <summary>
	/// This is here because in the past i would keep track of Lists of {equipables} for this database tool
	/// But in the game, you can only go from one to the other in a row when changing the {equipable} so it turned 
	/// into a LinkedList.
	/// </summary>
	public static class ExtensionMethods
	{
		public static LinkedListNode<T> RemoveAt<T>(this LinkedList<T> list, int index)
		{
			LinkedListNode<T> currentNode = list.First;
			for (int i = 0; i <= index && currentNode != null; i++)
			{
				if (i != index)
				{
					currentNode = currentNode.Next;
					continue;
				}

				list.Remove(currentNode);
				return currentNode;
			}

			throw new IndexOutOfRangeException();
		}
	}

	#region Job Related Window Functions
	//public partial class MainWindow
	//{
	//	private void LoadJobsFromDatabase()
	//	{
	//		String masterfile = (MainWindow.SQLDatabasePath);
	//		first up we need to connect to our database
	//		_sqlite_conn = new SQLiteConnection(masterfile);
	//		int rowid = 0;
	//		try
	//		{
	//			EditJobsDB_LB.ItemsSource = null; //reset
	//			PartyMemberMainJob_Add_CB.ItemsSource = null;
	//			PartyMemberSubJob_Add_CB.ItemsSource = null;
	//			PartyMemberMainJob_Edit_CB.ItemsSource = null;
	//			PartyMemberSubJob_Edit_CB.ItemsSource = null;


	//			StringBuilder Createsql = new StringBuilder();
	//			Createsql.Append("SELECT * FROM `jobs`;");

	//			IEnumerable<Job> varlist = _sqlite_conn.Query<Job>(Createsql.ToString());
	//			int i = 0;
	//			i++;
	//			foreach (Job j in varlist.ToList())
	//			{
	//				CurrentJobsInDatabase.Add(j);
	//			}
	//		}
	//		catch (Exception ex)
	//		{
	//			Console.WriteLine("Job Read from database FAILURE {0}:", ex.Message);
	//			SetOutputLog( = String.Format("Loading/Reading Database failed: {0}", ex.Message);
	//		}
	//		finally
	//		{
	//			EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
	//			PartyMemberMainJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
	//			PartyMemberSubJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
	//			PartyMemberMainJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
	//			PartyMemberSubJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
	//		}
	//	}
	//}
	#endregion

}