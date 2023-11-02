using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class SchetsEditor : Form
{
    private MenuStrip menuStrip;
    SchetsControl schetscontrol;

    public SchetsEditor()
    {   
        this.ClientSize = new Size(800, 600);
        menuStrip = new MenuStrip();
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakHelpMenu();
        this.Text = "Schets editor";
        this.IsMdiContainer = true;
        this.MainMenuStrip = menuStrip;
    }
    private void maakFileMenu()
    {   
        ToolStripDropDownItem menu = new ToolStripMenuItem("File");
        menu.DropDownItems.Add("Nieuw", null, this.nieuw);
        menu.DropDownItems.Add("Exit", null, this.afsluiten);
        menu.DropDownItems.Add("Open", null, this.Open);
        menuStrip.Items.Add(menu);
    }
    private void maakHelpMenu()
    {   
        ToolStripDropDownItem menu = new ToolStripMenuItem("Help");
        menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
        menuStrip.Items.Add(menu);
    }
    private void about(object o, EventArgs ea)
    {   
        MessageBox.Show ( "Schets versie 2.0\n(c) UU Informatica 2022"
                        , "Over \"Schets\""
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Information
                        );
    }

    private void nieuw(object sender, EventArgs e)
    {   
        SchetsWin s = new SchetsWin();
        s.MdiParent = this;
        s.Show();
    }
    private void afsluiten(object sender, EventArgs e)
    {   
        this.Close();
    }
    private void Open(object sender, EventArgs e)
    {
        Console.WriteLine("Open");
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Open an existing text file";
        openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = openFileDialog.FileName;

            this.nieuw(sender, e);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                    {
                        string line;
                        
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] sub_line = line.Split(',');
                            line = string.Join(" ", sub_line);

                            string[] charsToRemove = new string[] { "{X=", "Y=", "}", "Color [", "]" };

                            foreach (string s in charsToRemove)
                            {
                                line = line.Replace(s, "");
                            }

                            
                            string[] parts = line.Split(" ");
                            Console.WriteLine(parts);
                            string type = parts[0];
                           
                            int x1 = int.Parse(parts[2]);
                            int y1 = int.Parse(parts[3]);
                            int x2 = int.Parse(parts[5]);   
                            int y2 = int.Parse(parts[6]); 
                            

                            Color color = Color.FromName(parts[8]);
                            /*dit doen we omdat er spaties in de file staan, en we die willen we niet gebuiken*/
                            Point p = new Point(x1, y1);
                            Point q = new Point(x2, y2);
                            
                            Schets.getekendelijst.Add(new GetekendObject(type, p, q, color));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}