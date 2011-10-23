using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace WinBMA
{
    public partial class ManageForm : Form
    {
        private FriendlyAuth selectedAuth = null;
        private bool generated = true;
        private MainForm frmParent = null;

        public ManageForm(bool modeNew, MainForm parent)
        {
            InitializeComponent();
            frmParent = parent;

            RefreshAuthList();

            if (modeNew)
            {
                DoNewAuth();
            }
        }

        private void lstAuths_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsOnSelection();
        }

        private void RefreshAuthList()
        {
            FriendlyAuth oldSelAuth = selectedAuth;

            lstAuths.Items.Clear();
            UpdateAuthCount();

            foreach (FriendlyAuth auth in Settings.Authenticators)
            {
                lstAuths.Items.Add(auth);
            }

            if (oldSelAuth != null && lstAuths.Items.Contains(oldSelAuth))
            {
                lstAuths.SelectedItem = oldSelAuth;
            }
            else if(lstAuths.Items.Count > 0)
            {
                lstAuths.SelectedIndex = 0;
            }
            UpdateControlsOnSelection();
        }

        private void UpdateAuthCount()
        {
            lblCount.Text = Settings.Authenticators.Count + " Authenticator" + ((Settings.Authenticators.Count != 1) ? "s" : "");
        }

        private void UpdateControlsOnSelection()
        {
            selectedAuth = (FriendlyAuth)lstAuths.SelectedItem;

            if (selectedAuth == null)
            {
                mnuDelete.Enabled = false;
                mnuExport.Enabled = false;
                lblAuthKey.Enabled = false;
                txtName.ReadOnly = true;
                lblAuthKey.Enabled = false;
                txtName.Text = txtSerial.Text = txtToken.Text = txtRegion.Text = lblAuthKey.Text = null;
                barTime.Value = 0;
            }
            else
            {
                grpNew.Visible = false;
                mnuDelete.Enabled = true;
                mnuExport.Enabled = true;


                txtName.ReadOnly = false;
                txtName.Text = selectedAuth.FriendlyName;
                txtSerial.Text = selectedAuth.Authenticator.Serial;
                txtToken.Text = selectedAuth.Authenticator.Token;
                txtRegion.Text = (selectedAuth.Authenticator.Region.RegionNumber == 0) ? "North America" : "Europe";

                barTime.ToolTipText = "Clock skew: " + (selectedAuth.Authenticator.Region.TimeDrift / 1000F).ToString("0.000") + "s";
                barTime.Value = (int)selectedAuth.Authenticator.TimeSinceLastKeyChange;

                lblAuthKey.Enabled = true;
                lblAuthKey.Text = selectedAuth.Authenticator.GetKey();
                generated = true;
            }
        }

        private void DoNewAuth()
        {
            lstAuths.SelectedItem = null;
            UpdateControlsOnSelection();

            grpNew.Visible = true;
        }

        private void timGenerate_Tick(object sender, EventArgs e)
        {
            if (selectedAuth == null)
            {
                generated = false;
                barTime.Value = 0;
                return;
            }

            long lastChange = selectedAuth.Authenticator.TimeSinceLastKeyChange;

            if (generated)
            {
                if (lastChange > 1000)
                {
                    generated = false;
                }
            }
            else
            {
                if (lastChange <= 1000)
                {
                    lblAuthKey.Text = selectedAuth.Authenticator.GetKey();
                    generated = true;
                }
            }

            barTime.Value = (int)lastChange;
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            DoNewAuth();
        }

        private void btnRegionUS_Click(object sender, EventArgs e)
        {
            grpNew.Visible = false;
            CreateNewAuth(BlizzAuth.Region.RegionType.US);
        }

        private void CreateNewAuth(BlizzAuth.Region.RegionType type)
        {
            BlizzAuth.Authenticator newAuth = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    newAuth = BlizzAuth.Authenticator.Create(type);
                    break;
                }
                catch (Exception)
                {

                }
                Thread.Sleep(2000);
            }

            if (newAuth == null)
            {
                MessageBox.Show("An unknown error has occured while trying to generate an authenticator. Please try again later.", "WinBMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FriendlyAuth fAuth = new FriendlyAuth(newAuth);

            Settings.Authenticators.Add(fAuth);
            frmParent.AuthenticatorsChanged();

            lstAuths.Items.Add(fAuth);
            lstAuths.SelectedItem = fAuth;
            UpdateAuthCount();
            UpdateControlsOnSelection();
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            if (selectedAuth != null)
            {
                selectedAuth.FriendlyName = txtName.Text;
                frmParent.AuthenticatorsChanged();
                RefreshAuthList();
            }
        }

        private void btnRegionEU_Click(object sender, EventArgs e)
        {
            grpNew.Visible = false;
            CreateNewAuth(BlizzAuth.Region.RegionType.EU);
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                txtName_Validated(txtName, EventArgs.Empty);
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            Settings.Authenticators.Remove(selectedAuth);
            frmParent.AuthenticatorsChanged();

            lstAuths.Items.Remove(selectedAuth);
            UpdateAuthCount();
            UpdateControlsOnSelection();
        }

        private void mnuExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfDialog = new SaveFileDialog();
            sfDialog.DefaultExt = "bma";
            sfDialog.AddExtension = true;
            sfDialog.Filter = "WinBMA files (*.bma)|*.bma";
            sfDialog.FileName = selectedAuth.Authenticator.Serial + ".bma";

            if (sfDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = sfDialog.FileName;

                try
                {
                    using (BinaryWriter binWriter = new BinaryWriter(File.Create(filePath)))
                    {
                        binWriter.Write("WINBMAEXP".ToCharArray());
                        binWriter.Write(1);

                        binWriter.Write(selectedAuth.FriendlyName);
                        binWriter.Write(selectedAuth.Authenticator.Serial);
                        binWriter.Write(BlizzAuth.Helper.ConvertHexStringToBytes(selectedAuth.Authenticator.Token));
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mnuImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();
            ofDialog.DefaultExt = "bma";
            ofDialog.AddExtension = true;
            ofDialog.Filter = "WinBMA files (*.bma)|*.bma";

            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofDialog.FileName;

                try
                {
                    using (BinaryReader binReader = new BinaryReader(File.OpenRead(filePath)))
                    {
                        string fileHeader = "";

                        for (int i = 0; i < 9; i++)
                        {
                            fileHeader += binReader.ReadChar();
                        }

                        if (fileHeader != "WINBMAEXP")
                        {
                            return;
                        }

                        int fileVersion = binReader.ReadInt32();

                        string name = binReader.ReadString();
                        string serial = binReader.ReadString();
                        string token = BlizzAuth.Helper.ConvertBytesToHexString(binReader.ReadBytes(20));

                        FriendlyAuth fAuth = new FriendlyAuth(new BlizzAuth.Authenticator(serial, token), name);

                        Settings.Authenticators.Add(fAuth);
                        frmParent.AuthenticatorsChanged();

                        lstAuths.Items.Add(fAuth);
                        lstAuths.SelectedItem = fAuth;
                        UpdateAuthCount();
                        UpdateControlsOnSelection();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblAuthKey_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblAuthKey.Text.Replace(" ", ""));
        }

        private void lstAuths_MouseDown(object sender, MouseEventArgs e)
        {
            int mouseDownIndex = lstAuths.IndexFromPoint(new Point(e.X, e.Y));

            if (mouseDownIndex > -1)
            {
                lstAuths.DoDragDrop(new DragDropIndex(mouseDownIndex), DragDropEffects.All);
            }


            UpdateControlsOnSelection();
        }

        private void lstAuths_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropIndex)))
            {
                int mouseUpIndex = lstAuths.IndexFromPoint(lstAuths.PointToClient(new Point(e.X, e.Y)));

                if (mouseUpIndex != -1)
                {
                    int mouseDownIndex = ((DragDropIndex)e.Data.GetData(typeof(DragDropIndex))).Index;
                    if (mouseUpIndex != mouseDownIndex)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void lstAuths_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropIndex)))
            {
                int mouseUpIndex = lstAuths.IndexFromPoint(lstAuths.PointToClient(new Point(e.X, e.Y)));
                if (mouseUpIndex != -1)
                {
                    int mouseDownIndex = ((DragDropIndex)e.Data.GetData(typeof(DragDropIndex))).Index;
                    if (mouseUpIndex != mouseDownIndex)
                    {
                        int delIndex = mouseDownIndex;
                        if (mouseUpIndex < mouseDownIndex)
                        {
                            delIndex += 1;
                        }

                        Settings.Authenticators.Insert(mouseUpIndex, Settings.Authenticators[mouseDownIndex]);
                        Settings.Authenticators.RemoveAt(delIndex);

                        frmParent.AuthenticatorsChanged();
                        RefreshAuthList();
                    }
                }
            }
        }

        private class DragDropIndex
        {
            public DragDropIndex(int index)
            {
                Index = index;
            }

            public int Index { get; set; }
        }
    }
}
