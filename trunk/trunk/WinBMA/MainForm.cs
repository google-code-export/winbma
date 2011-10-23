using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinBMA.BlizzAuth;

namespace WinBMA
{
    public partial class MainForm : Form
    {
        public delegate void AuthsChanged();
        private AuthsChanged authsDelegate;

        private FriendlyAuth selectedAuth = null;
        private bool firstListPop = true;

        public MainForm()
        {
            InitializeComponent();
            authsDelegate = new AuthsChanged(AuthenticatorsChanged);

            this.Text = String.Format(this.Text, this.GetType().Assembly.GetName().Version.ToString());

            AuthenticatorsChanged();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void DoSelectionChanged()
        {
            selectedAuth = (FriendlyAuth)cmbAuths.SelectedItem;

            if (selectedAuth == null)
            {
                lblAuthKey.Text = lblSerial.Text = string.Empty;
                lblAuthKey.Enabled = false;
                barTime.ToolTipText = string.Empty;
                Settings.LastSelectedIndex = -1;
            }
            else
            {
                barTime.Value = (int)selectedAuth.Authenticator.TimeSinceLastKeyChange;
                barTime.ToolTipText = "Clock skew: " + (selectedAuth.Authenticator.Region.TimeDrift / 1000F).ToString("0.000") + "s";

                lblAuthKey.Enabled = true;
                lblAuthKey.Text = selectedAuth.Authenticator.GetKey();

                lblSerial.Text = selectedAuth.Authenticator.Serial;
                Settings.LastSelectedIndex = cmbAuths.SelectedIndex;

                TimeSpan offset = (DateTime.Now - Settings.TimeSinceLastSync[selectedAuth.Authenticator.Region.RegionNumber]);

                if (offset > new TimeSpan(14, 0, 0, 0))
                {
                    try
                    {
                        selectedAuth.Authenticator.Region.ResyncServerTime();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void AuthenticatorsChanged()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(authsDelegate);
            }
            else
            {
                cmbAuths.Items.Clear();

                if (Settings.Authenticators.Count == 0)
                {
                    cmbAuths.Enabled = false;
                    DoSelectionChanged();
                }
                else
                {
                    FriendlyAuth oldSelAuth = selectedAuth;

                    cmbAuths.Enabled = true;
                    foreach (FriendlyAuth fAuth in Settings.Authenticators)
                    {
                        cmbAuths.Items.Add(fAuth);
                    }

                    if (oldSelAuth != null && cmbAuths.Items.Contains(oldSelAuth))
                    {
                        cmbAuths.SelectedItem = oldSelAuth;
                    }
                    else
                    {
                        if (firstListPop)
                        {
                            firstListPop = false;
                            if (Settings.LastSelectedIndex < cmbAuths.Items.Count && Settings.LastSelectedIndex >= 0)
                            {
                                cmbAuths.SelectedIndex = Settings.LastSelectedIndex;
                            }
                            else
                            {
                                cmbAuths.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            cmbAuths.SelectedIndex = 0;
                        }
                    }

                    DoSelectionChanged();
                }
            }
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            ManageForm frmManage = new ManageForm(true, this);
            frmManage.ShowDialog();
        }

        private void mnuView_Click(object sender, EventArgs e)
        {
            ManageForm frmManage = new ManageForm(false, this);
            frmManage.ShowDialog();
        }

        private bool generated = false;
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

        private void cmbAuths_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoSelectionChanged();
        }

        private void mnuResync_Click(object sender, EventArgs e)
        {
            bool resyncUS = false;
            bool resyncEU = false;

            foreach (FriendlyAuth auth in Settings.Authenticators)
            {
                if (auth.Authenticator.Region.RegionNumber == 0 && resyncUS == false)
                {
                    resyncUS = true;

                    try
                    {
                        auth.Authenticator.Region.ResyncServerTime();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error has occured while syncing time for US region.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (auth.Authenticator.Region.RegionNumber == 1 && resyncEU == false)
                {
                    resyncEU = true;

                    try
                    {
                        auth.Authenticator.Region.ResyncServerTime();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error has occured while syncing time for EU region.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (resyncUS && resyncEU)
                    break;
            }

            DoSelectionChanged();
        }

        private void lblSerial_Click(object sender, EventArgs e)
        {
            if (lblSerial.Text != string.Empty)
            {
                Clipboard.SetText(lblSerial.Text.Replace("-", ""));
            }
        }

        private void lblAuthKey_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblAuthKey.Text.Replace(" ", ""));
        }

    }
}
