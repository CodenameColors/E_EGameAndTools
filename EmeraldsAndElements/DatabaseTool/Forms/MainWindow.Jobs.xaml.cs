using BixBite.Combat;
using SQLite;
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

		public ObservableCollection<Job> CurrentJobsInDatabase { get; set; }


		public void MainWindow_Jobs()
		{
			CurrentJobsInDatabase = new ObservableCollection<Job>();
		}

		public void LoadJobsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				EditJobsDB_LB.ItemsSource = null; //reset
				PartyMemberMainJob_Add_CB.ItemsSource = null;
				PartyMemberSubJob_Add_CB.ItemsSource = null;
				PartyMemberMainJob_Edit_CB.ItemsSource = null;
				PartyMemberSubJob_Edit_CB.ItemsSource = null;


				StringBuilder Createsql = new StringBuilder();
				Createsql.Append("SELECT * FROM `jobs`;");

				IEnumerable<Job> varlist = _sqlite_conn.Query<Job>(Createsql.ToString());
				int i = 0;
				i++;
				foreach (Job j in varlist.ToList())
				{
					CurrentJobsInDatabase.Add(j);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Job Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database failed: {0}", ex.Message);
			}
			finally
			{
				EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberMainJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberSubJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberMainJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberSubJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
			}
		}


		private void AddJobToDB_BTN_OnClick(object sender, RoutedEventArgs e)
		{

		}
	}

}

