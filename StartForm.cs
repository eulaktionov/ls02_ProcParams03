using System.Configuration;
using System.Data;
using System.Data.Common;

using Microsoft.Data.SqlClient;

namespace ls02_CommmandParams02
{
    public partial class StartForm : Form
    {
        TextBox queryText;
        SqlConnection connection;
        SqlDataAdapter dataAdapter;
        DataSet dataSet;
        DataGridView dataGrid;

        public StartForm()
        {
            InitializeComponent();
            CreateControls();

            try
            {
                var connectionString =
                    ConfigurationManager.AppSettings.Get("connectionString");
                connection = new SqlConnection(connectionString);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dataSet = new DataSet();

            Text = "Data Table Query";
        }

        void CreateControls()
        {
            dataGrid = new DataGridView();
            dataGrid.Dock = DockStyle.Fill;
            Controls.Add(dataGrid);

            Size buttonSize = new Size(80, 40);
            Button buttonFill = CreateButton("Fill", buttonSize,
                (s, e) => FillData());
            buttonFill.Dock = DockStyle.Left;

            Button buttonUpdate = CreateButton("Update", buttonSize,
                (s, e) => SaveData());
            buttonUpdate.Dock = DockStyle.Right;

            queryText = new TextBox();
            queryText.TextAlign = HorizontalAlignment.Center;
            queryText.Dock = DockStyle.Fill;
            queryText.Text = "select * from Books";
            queryText.ReadOnly = true;

            Panel panel = new Panel();
            panel.Height = buttonSize.Height;

            panel.Controls.Add(buttonFill);
            panel.Controls.Add(buttonUpdate);
            panel.Controls.Add(queryText);

            panel.Dock = DockStyle.Top;
            Controls.Add(panel);
        }

        Button CreateButton(string text, Size size, EventHandler handler)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = size;
            button.Click += handler;
            return button;
        }

        void FillData()
        {
            try
            {
                if (dataSet.Tables.Count > 0)
                {
                    dataSet.Tables.Remove(dataSet.Tables[0]);
                }
                dataAdapter = CreateAdapter();

                var commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.Fill(dataSet);
                dataGrid.DataSource = dataSet.Tables[0];
                dataGrid.Update();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        SqlDataAdapter CreateAdapter()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(queryText.Text, connection);
            dataAdapter.UpdateCommand = new SqlCommand
                ("UpdateBooks", connection);
            dataAdapter.UpdateCommand.CommandType = CommandType.StoredProcedure;

            dataAdapter.UpdateCommand.Parameters
                .Add(new SqlParameter("@Id", SqlDbType.Int, 4, "Id"));
            dataAdapter.UpdateCommand.Parameters["@Id"]
                .SourceVersion = DataRowVersion.Original;

            dataAdapter.UpdateCommand.Parameters
                .Add(new SqlParameter("@Title", SqlDbType.VarChar, 100, "Title"));
            dataAdapter.UpdateCommand.Parameters
                .Add(new SqlParameter("@Price", SqlDbType.Int, 4, "Price"));
            dataAdapter.UpdateCommand.Parameters
                .Add(new SqlParameter("@Pages", SqlDbType.Int, 4, "Pages"));

            return dataAdapter;
        }

        void SaveData()
        {
            dataAdapter.Update(dataSet.Tables[0]);
        }
    }
}