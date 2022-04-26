using BixBite.Characters;
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

		public ObservableCollection<party_member> CurrentPartyMembersInDatabase { get; set; }

		public void MainWindow_PartyMembers()
		{
			CurrentPartyMembersInDatabase = new ObservableCollection<party_member>();
		}


		private void LoadPartyMembersFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			//SkillsName_Edit_CB.ItemsSource = null;
			PartyMemberName_Edit_CB.ItemsSource = null;
			CurrentPartyMembersInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `party_member`;");

				IEnumerable<party_member> varlist = _sqlite_conn.Query<party_member>(Createsql);
				foreach (party_member partyMember in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentPartyMembersInDatabase.Add(partyMember);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Skills Read from database FAILURE {0}:", ex.Message);
				SetOutputLog(String.Format("Loading/Reading [FROM Party Member] Database failed: {0}", ex.Message));
			}
			finally
			{
				//SkillsName_Edit_CB.ItemsSource = CurrentPartyMembersInDatabase;
				PartyMemberName_Edit_CB.ItemsSource = CurrentPartyMembersInDatabase;
			}
		}

		

		private void PartymemberSkillsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberSkills_Add_IC.Items.Count <= 6)
			{
				PartyMemberSkills_Add_IC.Items.Add(((Skill)PartymemberSkills_Add_CB.SelectedValue).Name);
			}
		}

		private void PartymemberSkillsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberSkills_Edit_IC.Items.Count <= 6)
			{
				PartyMemberSkills_Edit_IC.Items.Add(((Skill)PartymemberSkills_Edit_CB.SelectedValue).Name);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Skills.Add((Skill)PartymemberSkills_Edit_CB.SelectedValue);
			}
		}

		private void RemovePartMemberSkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberSkills_Add_IC.Items.IndexOf(item);

			PartyMemberSkills_Add_IC.Items.RemoveAt(index);
		}

		private void RemovePartMemberSkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberSkills_Edit_IC.Items.IndexOf(item);

			PartyMemberSkills_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Skills.RemoveAt(index);
		}

		private void PartyMemberItem_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberItems_Add_IC.Items.Count <= 6)
			{
				PartyMemberItems_Add_IC.Items.Add(((Item)PartyMemberItems_Add_CB.SelectedValue).ID);
			}

		}

		private void PartyMemberItem_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberItems_Edit_IC.Items.Count <= 6)
			{
				PartyMemberItems_Edit_IC.Items.Add(((Item)PartyMemberItems_Edit_CB.SelectedValue).ID);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Items.Add((Item)PartyMemberItems_Edit_CB.SelectedValue);
			}

		}

		private void RemovePartyMemberItem_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberItems_Add_IC.Items.IndexOf(item);

			PartyMemberItems_Add_IC.Items.RemoveAt(index);
		}

		private void RemovePartyMemberItem_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberItems_Edit_IC.Items.IndexOf(item);

			PartyMemberItems_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Items.RemoveAt(index);
		}


		private void PartyMemberWeapon_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberWeapon_Add_IC.Items.Count <= 3)
			{
				PartyMemberWeapon_Add_IC.Items.Add(((Weapon)PartyMemberWeapons_Add_CB.SelectedValue).ID);
			}
		}

		private void PartyMemberWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberWeapon_Edit_IC.Items.Count <= 3)
			{
				PartyMemberWeapon_Edit_IC.Items.Add(((Weapon)PartyMemberWeapons_Edit_CB.SelectedValue).ID);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Weapons.AddFirst((Weapon)PartyMemberWeapons_Edit_CB.SelectedValue);
			}
		}


		private void RemovePartyMemberWeapon_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberWeapon_Add_IC.Items.IndexOf(item);

			PartyMemberWeapon_Add_IC.Items.RemoveAt(index);
		}


		private void RemovePartyMemberWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberWeapon_Edit_IC.Items.IndexOf(item);

			PartyMemberWeapon_Edit_IC.Items.RemoveAt(index);

			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Weapons.RemoveAt(index);
		}


		private void PartyMemberAddToDatabase_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (
				PartyMemberFirstName_Add_TB.Text.Length > 0 &&
				PartyMemberLastName_Add_TB.Text.Length > 0 &&
				int.TryParse(PartyMemberFriendPoints_Add_TB.Text, out int fpResult) &&
				int.TryParse(PartyMemberCombatLevel_Add_TB.Text, out int combatlevelResult) &&

				int.TryParse(PartyMemberMaxHP_Add_TB.Text, out int maxHpResult) &&
				int.TryParse(PartyMemberCurHP_Add_TB.Text, out int curHpResult) &&
				int.TryParse(PartyMemberMaxMP_Add_TB.Text, out int maxMPResult) &&
				int.TryParse(PartyMemberCurMP_Add_TB.Text, out int curMPResult) &&

				int.TryParse(PartyMemberAtk_Add_TB.Text, out int atkResult) &&
				int.TryParse(PartyMemberDef_Add_TB.Text, out int defResult) &&
				int.TryParse(PartyMemberDex_Add_TB.Text, out int dexResult) &&
				int.TryParse(PartyMemberAgl_Add_TB.Text, out int aglResult) &&
				int.TryParse(PartyMemberMor_Add_TB.Text, out int morResult) &&
				int.TryParse(PartyMemberWis_Add_TB.Text, out int wisResult) &&
				int.TryParse(PartyMemberRes_Add_TB.Text, out int resResult) &&
				int.TryParse(PartyMemberLuc_Add_TB.Text, out int LucResult) &&
				int.TryParse(PartyMemberRsk_Add_TB.Text, out int RskResult) &&
				int.TryParse(PartyMemberItl_Add_TB.Text, out int itlResult)
			)
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					int newID_stat = 0;
					int newID_weakstr = 0;

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Current_Health = curHpResult,
						Max_Mana = maxMPResult,
						Current_Mana = curMPResult,
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
					newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)PartyMembersMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberMagWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)PartyMembersWeaponWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberweaponWeak_CB", c);

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
						ContentPresenter c = ((ContentPresenter)PartyMembersMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberMagicStrength_CB", c);

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
						ContentPresenter c = ((ContentPresenter)PartyMembersWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberWeaknessStrength_CB", c);

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

					#region Party Member

					party_member partyMember = new party_member()
					{
						First_Name = PartyMemberFirstName_Add_TB.Text,
						Last_Name = PartyMemberLastName_Add_TB.Text,
						Friendship_Points = fpResult,
						Level = combatlevelResult,

						Stats_FK = basestat.ID,
						Weak_Strength_FK = weakstrToAdd.ID,
					};

					if (PartyMemberMainJob_Add_CB.SelectedIndex != -1)
					{
						partyMember.Main_Job_FK = ((Job)PartyMemberMainJob_Add_CB.SelectedValue).Id;
					}
					if (PartyMemberSubJob_Add_CB.SelectedIndex != -1)
					{
						partyMember.Sub_Job_FK = ((Job)PartyMemberSubJob_Add_CB.SelectedValue).Id;
					}
					#endregion
					_sqlite_conn.Insert(partyMember);

					#region Create Keys Entries
					#region Skills
					InsertRecordIntoSkillKeys(PartyMemberSkills_Add_IC, _sqlite_conn, "party_member",
						String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#region Weapons
					InsertRecordIntoWeaponKeys(PartyMemberWeapon_Add_IC, _sqlite_conn, "party_member",
					String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#region Items
					InsertRecordIntoItemKeys(PartyMemberItems_Add_IC, _sqlite_conn, "party_member",
						String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#endregion
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [items] FAILURE | {0}", ex.Message);
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

		private void PartyMemberName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex >= 0)
			{
				//reset the IC so we don't have false dups
				PartyMemberSkills_Edit_IC.Items.Clear();
				PartyMemberItems_Edit_IC.Items.Clear();
				PartyMemberWeapon_Edit_IC.Items.Clear();

				party_member partyMember = (party_member)CB.SelectedValue;
				Console.WriteLine(String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
				//create a partymemeber and fill out the objects internal object references.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base Stats
					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

					Base_Stats baseStats = bsList.Single(x => x.ID == partyMember.Stats_FK);
					partyMember.Stats = baseStats;
					#endregion
					#region Weaknesses and strengths
					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

					weaknesses_strengths wsData = wsList.Single(x => x.ID == partyMember.Weak_Strength_FK);
					partyMember.WeaknessAndStrengths = wsData;
					#endregion


					#region Skills
					Createsql = "SELECT * FROM `skill_keys`;";
					List<Skill_Keys> skillList_keys = _sqlite_conn.Query<Skill_Keys>(Createsql);
					skillList_keys = skillList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Skill> result_skills = CurrentSkillsInDatabase.Where(p => skillList_keys.Any(p2 => p2.Skill_ID == p.Name)).ToList() as List<Skill>;
					partyMember.Skills = result_skills;
					result_skills.ForEach(x => PartyMemberSkills_Edit_IC.Items.Add(x.Name));
					#endregion

					#region Items
					Createsql = "SELECT * FROM `item_keys`;";
					List<Item_Keys> ItemList_keys = _sqlite_conn.Query<Item_Keys>(Createsql);
					ItemList_keys = ItemList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Item> result_items = CurrentItemsInDatabase.Where(p => ItemList_keys.Any(p2 => p2.Item_ID == p.ID)).ToList() as List<Item>;
					partyMember.Items = result_items;
					result_items.ForEach(x => PartyMemberItems_Edit_IC.Items.Add((x.ID)));
					#endregion

					#region Weapons
					Createsql = "SELECT * FROM `weapon_keys`;";
					List<Weapon_Keys> WeaponList_keys = _sqlite_conn.Query<Weapon_Keys>(Createsql);
					WeaponList_keys = WeaponList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Weapon> result_weapons = CurrentWeaponsInDatabase.Where(p => WeaponList_keys.Any(p2 => p2.Weapon_ID == p.ID)).ToList() as List<Weapon>;
					partyMember.Weapons = new LinkedList<Weapon>(result_weapons);
					result_weapons.ForEach(x => PartyMemberWeapon_Edit_IC.Items.Add((x.ID)));
					#endregion

					//Next up fill out ALL the text boxes with the correct info

					#region Party Member Data
					PartyMemberFriendPoints_Edit_TB.Text = partyMember.Friendship_Points.ToString();
					PartyMemberCombatLevel_Edit_TB.Text = partyMember.Level.ToString();
					#endregion

					#region Stats
					PartyMemberMaxHP_Edit_TB.Text = partyMember.Stats.Max_Health.ToString();
					PartyMemberCurHP_Edit_TB.Text = partyMember.Stats.Current_Health.ToString();
					PartyMemberMaxMP_Edit_TB.Text = partyMember.Stats.Max_Mana.ToString();
					PartyMemberCurMP_Edit_TB.Text = partyMember.Stats.Current_Mana.ToString();


					PartyMemberAtk_Edit_TB.Text = partyMember.Stats.Attack.ToString();
					PartyMemberDef_Edit_TB.Text = partyMember.Stats.Defense.ToString();
					PartyMemberDex_Edit_TB.Text = partyMember.Stats.Dexterity.ToString();
					PartyMemberAgl_Edit_TB.Text = partyMember.Stats.Agility.ToString();
					PartyMemberMor_Edit_TB.Text = partyMember.Stats.Morality.ToString();

					PartyMemberWis_Edit_TB.Text = partyMember.Stats.Wisdom.ToString();
					PartyMemberRes_Edit_TB.Text = partyMember.Stats.Resistance.ToString();
					PartyMemberLuc_Edit_TB.Text = partyMember.Stats.Luck.ToString();
					PartyMemberRsk_Edit_TB.Text = partyMember.Stats.Risk.ToString();
					PartyMemberItl_Edit_TB.Text = partyMember.Stats.Intelligence.ToString();
					#endregion

					//Check boxes time!
					#region Weaknesses and strengths

					#region Elemental "Binding"
					//reset
					SetItemControlCheckboxData(PartyMembersMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddPartyMemberMagWeak_CB", true);
					SetItemControlCheckboxData(PartyMembersMagicWeakness_Edit_IC, partyMember.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddPartyMemberMagWeak_CB", false);

					SetItemControlCheckboxData(PartyMembersMagicStrength_Edit_IC, null, EMagicType.NONE, "AddPartyMemberMagicStrength_CB", true);
					SetItemControlCheckboxData(PartyMembersMagicStrength_Edit_IC, partyMember.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddPartyMemberMagicStrength_CB", false);
					PartyMembersMagicWeakness_Edit_IC.UpdateLayout();
					PartyMembersMagicStrength_Edit_IC.UpdateLayout();
					#endregion

					#region Weakness & Strengths "Binding"
					//reset
					SetItemControlCheckboxData(PartyMembersWeaponWeakness_Edit_IC, null, EMagicType.NONE, "AddPartyMemberweaponWeak_CB", true);
					SetItemControlCheckboxData(PartyMembersWeaponWeakness_Edit_IC, partyMember.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddPartyMemberweaponWeak_CB", false);

					SetItemControlCheckboxData(PartyMembersWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB", true);
					SetItemControlCheckboxData(PartyMembersWeaponStrength_Edit_IC, partyMember.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB", false);
					PartyMembersWeaponStrength_Edit_IC.UpdateLayout();
					PartyMembersWeaponStrength_Edit_IC.UpdateLayout();
					#endregion

					#endregion

					#region Jobs
					PartyMemberMainJob_Edit_CB.SelectedValue = CurrentJobsInDatabase.Single(x => x.Id == partyMember.Main_Job_FK);
					PartyMemberSubJob_Edit_CB.SelectedValue = CurrentJobsInDatabase.Single(x => x.Id == partyMember.Sub_Job_FK);
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Filling Party member Data  FAILURE | {0}", ex.Message);
					SetOutputLog(String.Format("Loading/Writing Database [Filling Party member Data ] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}

			}
		}

		private void PartyMemberUpdateToDatabase_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{


			if
			(
				int.TryParse(PartyMemberFriendPoints_Edit_TB.Text, out int fpResult) &&
				int.TryParse(PartyMemberCombatLevel_Edit_TB.Text, out int combatlevelResult) &&

				int.TryParse(PartyMemberMaxHP_Edit_TB.Text, out int maxHpResult) &&
				int.TryParse(PartyMemberCurHP_Edit_TB.Text, out int curHpResult) &&
				int.TryParse(PartyMemberMaxMP_Edit_TB.Text, out int maxMPResult) &&
				int.TryParse(PartyMemberCurMP_Edit_TB.Text, out int curMPResult) &&

				int.TryParse(PartyMemberAtk_Edit_TB.Text, out int atkResult) &&
				int.TryParse(PartyMemberDef_Edit_TB.Text, out int defResult) &&
				int.TryParse(PartyMemberDex_Edit_TB.Text, out int dexResult) &&
				int.TryParse(PartyMemberAgl_Edit_TB.Text, out int aglResult) &&
				int.TryParse(PartyMemberMor_Edit_TB.Text, out int morResult) &&
				int.TryParse(PartyMemberWis_Edit_TB.Text, out int wisResult) &&
				int.TryParse(PartyMemberRes_Edit_TB.Text, out int resResult) &&
				int.TryParse(PartyMemberLuc_Edit_TB.Text, out int LucResult) &&
				int.TryParse(PartyMemberRsk_Edit_TB.Text, out int RskResult) &&
				int.TryParse(PartyMemberItl_Edit_TB.Text, out int itlResult)
			)
			{
				//before we send the SQL update query we need to update the info in memory.
				int absindex = PartyMemberName_Edit_CB.SelectedIndex;
				party_member partyMember = (party_member)CurrentPartyMembersInDatabase[absindex];
				partyMember.Friendship_Points = fpResult;
				partyMember.Level = combatlevelResult;
				if (PartyMemberMainJob_Edit_CB.SelectedValue != null)
					partyMember.Main_Job_FK = ((Job)PartyMemberMainJob_Edit_CB.SelectedValue).Id;
				if (PartyMemberSubJob_Edit_CB.SelectedValue != null)
					partyMember.Sub_Job_FK = ((Job)PartyMemberSubJob_Edit_CB.SelectedValue).Id;

				#region Equipment

				if (PartyMemberClothes_Head_Edit_CB.SelectedIndex >= 0)
					partyMember.HeadGear_FK = CurrentClothesInDatabase_Head[PartyMemberClothes_Head_Edit_CB.SelectedIndex].ID;
				else partyMember.HeadGear_FK = "";

				if (PartyMemberClothes_Body_Edit_CB.SelectedIndex >= 0)
					partyMember.BodyGear_FK = CurrentClothesInDatabase_Body[PartyMemberClothes_Body_Edit_CB.SelectedIndex].ID;
				else partyMember.BodyGear_FK = "";

				if (PartyMemberClothes_Legs_Edit_CB.SelectedIndex >= 0)
					partyMember.LegGear_FK = CurrentClothesInDatabase_Legs[PartyMemberClothes_Legs_Edit_CB.SelectedIndex].ID;
				else partyMember.LegGear_FK = "";

				if (PartyMemberClothes_Acc1_Edit_CB.SelectedIndex >= 0)
					partyMember.Accessory1_FK = CurrentAccessoriesInDatabase[PartyMemberClothes_Acc1_Edit_CB.SelectedIndex].ID;
				else partyMember.Accessory1_FK = "";

				if (PartyMemberClothes_Acc2_Edit_CB.SelectedIndex >= 0)
					partyMember.Accessory2_FK = CurrentAccessoriesInDatabase[PartyMemberClothes_Acc2_Edit_CB.SelectedIndex].ID;
				else partyMember.Accessory2_FK = "";
				#endregion


				Base_Stats stats = (Base_Stats)CurrentPartyMembersInDatabase[absindex].Stats;
				Base_Stats base_stats = new Base_Stats()
				{
					ID = stats.ID,
					Max_Health = maxHpResult,
					Current_Health = curHpResult,
					Max_Mana = maxMPResult,
					Current_Mana = curMPResult,
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
				weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentPartyMembersInDatabase[absindex].WeaknessAndStrengths;
				weaknessStrengths.ID = partyMember.Weak_Strength_FK;
				weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(PartyMembersMagicWeakness_Edit_IC, EMagicType.NONE, "AddPartyMemberMagWeak_CB");
				weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(PartyMembersMagicStrength_Edit_IC, EMagicType.NONE, "AddPartyMemberMagicStrength_CB");
				weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(PartyMembersWeaponWeakness_Edit_IC, EWeaponType.NONE, "AddPartyMemberweaponWeak_CB");
				weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(PartyMembersWeaponStrength_Edit_IC, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB");

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
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

					Createsql = "UPDATE `weaknesses_strengths` " +
											"SET " +
											String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
											String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
											String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
											String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

											String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);

					Createsql = "UPDATE `party_member` " +
											"SET " +
											String.Format("{0} = {1},", "friendship_points", partyMember.Friendship_Points) +
											String.Format("{0} = {1},", "level", partyMember.Level) +

											(partyMember.HeadGear_FK != String.Empty ?
												String.Format("{0} = '{1}',", "headgear_fk", partyMember.HeadGear_FK) :
												String.Format("{0} = '{1}',", "headgear_fk", "")) +
												(partyMember.BodyGear_FK != String.Empty ?
												String.Format("{0} = '{1}',", "bodygear_fk", partyMember.BodyGear_FK) :
												String.Format("{0} = '{1}',", "bodygear_fk", "")) +
											(partyMember.LegGear_FK != String.Empty ?
												String.Format("{0} = '{1}',", "leggear_fk", partyMember.LegGear_FK) :
												String.Format("{0} = '{1}',", "leggear_fk", "")) +
											(partyMember.Accessory1_FK != String.Empty ?
												String.Format("{0} = '{1}',", "accessory1_fk", partyMember.Accessory1_FK) :
												String.Format("{0} = '{1}',", "accessory1_fk", "")) +
											(partyMember.Accessory2_FK != String.Empty ?
												String.Format("{0} = '{1}',", "accessory2_fk", partyMember.Accessory2_FK) :
												String.Format("{0} = '{1}',", "accessory2_fk", "")) +

											String.Format("{0} = {1},", "main_job_fk", partyMember.Main_Job_FK) +
											String.Format("{0} = {1} ", "sub_job_fk", partyMember.Sub_Job_FK) +

											String.Format("WHERE first_name='{0}'", partyMember.First_Name) +
											String.Format("AND last_name='{0}'", partyMember.Last_Name);
					_sqlite_conn.Query<party_member>(Createsql);

					#region Key Deletion And reinsertion
					#region Skill

					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Skill>(Createsql); //delete

					foreach (Skill skill in partyMember.Skills)
					{
						Skill_Keys skillKey = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(skillKey);
					}
					#endregion

					#region Item
					Createsql = String.Format("DELETE FROM `item_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Item>(Createsql); //delete

					foreach (Item item in partyMember.Items)
					{
						Item_Keys itemkey = new Item_Keys()
						{
							Item_ID = item.ID,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(itemkey);
					}
					#endregion

					#region Weapon
					Createsql = String.Format("DELETE FROM `weapon_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Weapon>(Createsql); //delete

					foreach (Weapon item in partyMember.Weapons)
					{
						Weapon_Keys itemkey = new Weapon_Keys()
						{
							Weapon_ID = item.ID,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(itemkey);
					}
					#endregion
					#endregion
				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					SetOutputLog(String.Format("Loading/Reading Database [Party Member] failed: {0}", ex.Message));
				}
				finally

				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

	}
}
