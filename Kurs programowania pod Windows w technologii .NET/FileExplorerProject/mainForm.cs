using FileExplorerProject;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FileExplorer
{
    
    public partial class MainForm : Form
    {
        string substringDirectory;
        private string substringFile;

        public MainForm()
        {
            InitializeComponent();
            string startDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists(startDirectory))
            {
                directoryTreeView.Nodes.Add(startDirectory);

                PopulateTreeView(startDirectory, directoryTreeView.Nodes[0]);
            }
            ColumnHeader columnHeader1 = new ColumnHeader { Text = "Nazwa" };
            ColumnHeader columnHeader2 = new ColumnHeader { Text = "Rozszerzenie" };
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            autoScaleColumns();
            listView1.Anchor = (AnchorStyles)0xF;
            listView1.SizeChanged += (sender, e) => autoScaleColumns();
        }

        private void autoScaleColumns()
        {
            var proportional = listView1.ClientRectangle.Width / listView1.Columns.Count;
            foreach (ColumnHeader column in listView1.Columns)
            {
                column.Width = proportional;
            }
        }

        public const string DUMMYNODE = "DUMMYNODE";

        public void PopulateTreeView(string directoryValue, TreeNode parentNode)
        {
            string[] directoryArray = Directory.GetDirectories(directoryValue);

            try
            {
                string[] fileArray = Directory.GetFiles(directoryValue);
                if (fileArray.Length != 0)
                {
                    foreach (string file in fileArray)
                    {
                        substringFile = Path.GetFileName(file);
                        TreeNode myNode2 = new TreeNode(substringFile);
                        parentNode.Nodes.Add(myNode2);
                    }
                }
                if (directoryArray.Length != 0)
                {
                    foreach (string directory in directoryArray)
                    {
                        substringDirectory = directory.Replace(Directory.GetParent(directory).FullName, "");
                        if (substringDirectory[0] == '/' || substringDirectory[0] == '\\')
                            substringDirectory = substringDirectory.Substring(1);
                        TreeNode myNode = new TreeNode(substringDirectory)
                        {
                            Tag = directory
                        };

                        parentNode.Nodes.Add(myNode);

                        var dummyNode = new TreeNode(DUMMYNODE)
                        {
                            Tag = DUMMYNODE
                        };

                        myNode.Nodes.Add(dummyNode);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                parentNode.Nodes.Add("Access denied");
            }
        }

        private void PopulateListView(string directory)
        {
            
            listView1.Items.Clear();
            try
            {
                string[] directories = Directory.GetDirectories(directory);
                string dirExt = "dir";
                foreach (string dir in directories)
                {
                    string dirName = new DirectoryInfo(dir).Name;
                    ListViewItem item = new ListViewItem(new[] { dirName, dirExt })
                    {
                        Tag = dir
                    };
                    listView1.Items.Add(item);

                }

                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string fileExt = Path.GetExtension(file).Substring(1);

                    ListViewItem item = new ListViewItem(new[] { fileName, fileExt })
                    {
                        Tag = file
                    };
                    listView1.Items.Add(item);

                }
                listView1.View = View.Details;
            }
            
            catch (UnauthorizedAccessException)
            {
                listView1.Items.Add("Access denied");
            }
            catch (IOException) { }
        }

        private void directoryTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && (string)e.Node.Nodes[0].Tag == DUMMYNODE)
            {
                e.Node.Nodes.Clear();


                if (e.Node.Tag is string nodeDirectory)
                {
                    PopulateTreeView(nodeDirectory, e.Node);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            directoryTreeView.Nodes.Clear();
            string startDirectory = (string)comboBox1.SelectedItem;

            directoryTreeView.Nodes.Add(startDirectory);

            PopulateTreeView(startDirectory, directoryTreeView.Nodes[0]);
        }

        private void directoryTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string directory = e.Node.FullPath;
            
            PopulateListView(directory);
        }

        private void nowyElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditFile eF = new EditFile();
            eF.Show();
        }


        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                try
                {
                    string file = item.Tag as string;
                    if (Directory.Exists(file))
                    {
                        throw new FileExplorerException("DIR");
                    }
                    
                    string ext = file.Substring(file.LastIndexOf('.') + 1);
                    if (ext == "exe" || ext == "ini" || ext == "exe" || ext == "bat" || ext == "bin" || ext == "com" || ext == "cmd")
                    {
                        throw new FileExplorerException("");
                    }
                    else if (ext == "txt")
                    {
                        if (File.Exists(file))
                        {
                            EditFile eF = new EditFile(file);
                            eF.Show();
                        }
                        else
                        {
                            EditFile eF = new EditFile();
                            eF.Show();
                        }
                    }
                    else
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = (string)item.Tag,
                            UseShellExecute = true
                        };
                        var process = Process.Start(startInfo);
                    }
                }
                catch (FileExplorerException exc)
                {
                    if (exc.Message == "DIR")
                    {
                        string dir = item.Tag as string;
                        PopulateListView(dir);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            else
            {
                listView1.SelectedItems.Clear();
            }
            
            // https://softwareparticles.com/handling-external-processes-in-csharp/
        }
    }
    public class FileExplorerException : Exception
    {
        public FileExplorerException() { }
        public FileExplorerException(string message) : base(message) { }
        public FileExplorerException(string message, Exception inner) : base(message, inner) { }
    }
}
