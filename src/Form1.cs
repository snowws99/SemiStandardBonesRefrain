using PEPlugin;
using PEPlugin.Pmd;
using PEPlugin.Pmx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace SemiStandardBonesRefrain
{
	public class Form1 : Form
	{
		private IContainer components;

		private Button btnOK;

		private Button btnCancel;

		public CheckBox checkArmTwist;

		public CheckBox checkHandTwist;

		public CheckBox checkUpper2Bones;

		public CheckBox checkGrooveBone;

		public CheckBox checkWaist;

		public CheckBox checkSingleAxis;

		public CheckBox checkAutoBoneList;

		public CheckBox checkAllParent;

		public CheckBox checkLegIK;

		private Button btnSelectAll;

		public CheckBox checkOperationCenter;

		public CheckBox checkToeIK;

		private Button btnMaterial;

		public CheckedListBox listMaterials;

		public CheckBox checkDummyHandHeld;

		public CheckBox checkShoulderCancel;

		public CheckBox checkDummy;

		public CheckBox checkElbowPosOffset;

		private int basewidth;

		public bool mat_mode;

		private List<CheckBox> checkBoxList = new List<CheckBox>();
        public CheckBox checkThumbLocalAxis;
        public CheckBox checkToeEX;
        public CheckBox checkLegDControll;
        private GroupBox groupBox1;
        private GroupBox groupIPEXorPMX;
        public RadioButton radioPMX;
        public RadioButton radioIPEX;
        public string SettingPath = "";

        public IPEConnector Connector;


		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkArmTwist = new System.Windows.Forms.CheckBox();
            this.checkHandTwist = new System.Windows.Forms.CheckBox();
            this.checkUpper2Bones = new System.Windows.Forms.CheckBox();
            this.checkGrooveBone = new System.Windows.Forms.CheckBox();
            this.checkWaist = new System.Windows.Forms.CheckBox();
            this.checkSingleAxis = new System.Windows.Forms.CheckBox();
            this.checkAutoBoneList = new System.Windows.Forms.CheckBox();
            this.checkAllParent = new System.Windows.Forms.CheckBox();
            this.checkLegIK = new System.Windows.Forms.CheckBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.checkOperationCenter = new System.Windows.Forms.CheckBox();
            this.checkToeIK = new System.Windows.Forms.CheckBox();
            this.btnMaterial = new System.Windows.Forms.Button();
            this.listMaterials = new System.Windows.Forms.CheckedListBox();
            this.checkDummyHandHeld = new System.Windows.Forms.CheckBox();
            this.checkShoulderCancel = new System.Windows.Forms.CheckBox();
            this.checkDummy = new System.Windows.Forms.CheckBox();
            this.checkElbowPosOffset = new System.Windows.Forms.CheckBox();
            this.checkThumbLocalAxis = new System.Windows.Forms.CheckBox();
            this.checkToeEX = new System.Windows.Forms.CheckBox();
            this.checkLegDControll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupIPEXorPMX = new System.Windows.Forms.GroupBox();
            this.radioPMX = new System.Windows.Forms.RadioButton();
            this.radioIPEX = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupIPEXorPMX.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(10, 490);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 29);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(90, 490);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // checkArmTwist
            // 
            this.checkArmTwist.AutoSize = true;
            this.checkArmTwist.Checked = true;
            this.checkArmTwist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkArmTwist.Location = new System.Drawing.Point(10, 87);
            this.checkArmTwist.Name = "checkArmTwist";
            this.checkArmTwist.Size = new System.Drawing.Size(102, 17);
            this.checkArmTwist.TabIndex = 2;
            this.checkArmTwist.Text = "Arm twist bone *";
            this.checkArmTwist.UseVisualStyleBackColor = true;
            this.checkArmTwist.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkHandTwist
            // 
            this.checkHandTwist.AutoSize = true;
            this.checkHandTwist.Checked = true;
            this.checkHandTwist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkHandTwist.Location = new System.Drawing.Point(10, 133);
            this.checkHandTwist.Name = "checkHandTwist";
            this.checkHandTwist.Size = new System.Drawing.Size(122, 17);
            this.checkHandTwist.TabIndex = 3;
            this.checkHandTwist.Text = "Hand-twisted bone *";
            this.checkHandTwist.UseVisualStyleBackColor = true;
            // 
            // checkUpper2Bones
            // 
            this.checkUpper2Bones.AutoSize = true;
            this.checkUpper2Bones.Checked = true;
            this.checkUpper2Bones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkUpper2Bones.Location = new System.Drawing.Point(10, 156);
            this.checkUpper2Bones.Name = "checkUpper2Bones";
            this.checkUpper2Bones.Size = new System.Drawing.Size(129, 17);
            this.checkUpper2Bones.TabIndex = 4;
            this.checkUpper2Bones.Text = "Upper body 2 bones *";
            this.checkUpper2Bones.UseVisualStyleBackColor = true;
            // 
            // checkGrooveBone
            // 
            this.checkGrooveBone.AutoSize = true;
            this.checkGrooveBone.Checked = true;
            this.checkGrooveBone.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkGrooveBone.Location = new System.Drawing.Point(10, 179);
            this.checkGrooveBone.Name = "checkGrooveBone";
            this.checkGrooveBone.Size = new System.Drawing.Size(89, 17);
            this.checkGrooveBone.TabIndex = 5;
            this.checkGrooveBone.Text = "Groove Bone";
            this.checkGrooveBone.UseVisualStyleBackColor = true;
            // 
            // checkWaist
            // 
            this.checkWaist.AutoSize = true;
            this.checkWaist.Location = new System.Drawing.Point(10, 202);
            this.checkWaist.Name = "checkWaist";
            this.checkWaist.Size = new System.Drawing.Size(80, 17);
            this.checkWaist.TabIndex = 6;
            this.checkWaist.Text = "Waist bone";
            this.checkWaist.UseVisualStyleBackColor = true;
            // 
            // checkSingleAxis
            // 
            this.checkSingleAxis.AutoSize = true;
            this.checkSingleAxis.Location = new System.Drawing.Point(90, 64);
            this.checkSingleAxis.Name = "checkSingleAxis";
            this.checkSingleAxis.Size = new System.Drawing.Size(146, 17);
            this.checkSingleAxis.TabIndex = 7;
            this.checkSingleAxis.Text = "Single axis of elbow bone";
            this.checkSingleAxis.UseVisualStyleBackColor = true;
            this.checkSingleAxis.Visible = false;
            // 
            // checkAutoBoneList
            // 
            this.checkAutoBoneList.AutoSize = true;
            this.checkAutoBoneList.Checked = true;
            this.checkAutoBoneList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoBoneList.Location = new System.Drawing.Point(10, 431);
            this.checkAutoBoneList.Name = "checkAutoBoneList";
            this.checkAutoBoneList.Size = new System.Drawing.Size(229, 17);
            this.checkAutoBoneList.TabIndex = 8;
            this.checkAutoBoneList.Text = "Automatic registration in bone display frame";
            this.checkAutoBoneList.UseVisualStyleBackColor = true;
            // 
            // checkAllParent
            // 
            this.checkAllParent.AutoSize = true;
            this.checkAllParent.Checked = true;
            this.checkAllParent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAllParent.Location = new System.Drawing.Point(10, 64);
            this.checkAllParent.Name = "checkAllParent";
            this.checkAllParent.Size = new System.Drawing.Size(75, 17);
            this.checkAllParent.TabIndex = 9;
            this.checkAllParent.Text = "All parents";
            this.checkAllParent.UseVisualStyleBackColor = true;
            // 
            // checkLegIK
            // 
            this.checkLegIK.AutoSize = true;
            this.checkLegIK.Location = new System.Drawing.Point(10, 225);
            this.checkLegIK.Name = "checkLegIK";
            this.checkLegIK.Size = new System.Drawing.Size(90, 17);
            this.checkLegIK.TabIndex = 10;
            this.checkLegIK.Text = "Leg IK parent";
            this.checkLegIK.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(10, 454);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(72, 30);
            this.btnSelectAll.TabIndex = 11;
            this.btnSelectAll.Text = "Select all";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkOperationCenter
            // 
            this.checkOperationCenter.AutoSize = true;
            this.checkOperationCenter.Location = new System.Drawing.Point(10, 248);
            this.checkOperationCenter.Name = "checkOperationCenter";
            this.checkOperationCenter.Size = new System.Drawing.Size(105, 17);
            this.checkOperationCenter.TabIndex = 13;
            this.checkOperationCenter.Text = "Operation center";
            this.checkOperationCenter.UseVisualStyleBackColor = true;
            // 
            // checkToeIK
            // 
            this.checkToeIK.AutoSize = true;
            this.checkToeIK.Location = new System.Drawing.Point(10, 271);
            this.checkToeIK.Name = "checkToeIK";
            this.checkToeIK.Size = new System.Drawing.Size(65, 17);
            this.checkToeIK.TabIndex = 14;
            this.checkToeIK.Text = "Toe IK *";
            this.checkToeIK.UseVisualStyleBackColor = true;
            // 
            // btnMaterial
            // 
            this.btnMaterial.Location = new System.Drawing.Point(121, 455);
            this.btnMaterial.Name = "btnMaterial";
            this.btnMaterial.Size = new System.Drawing.Size(118, 29);
            this.btnMaterial.TabIndex = 15;
            this.btnMaterial.Text = "Material Selection >>";
            this.btnMaterial.UseVisualStyleBackColor = true;
            this.btnMaterial.Click += new System.EventHandler(this.btnMaterial_Click);
            // 
            // listMaterials
            // 
            this.listMaterials.CheckOnClick = true;
            this.listMaterials.FormattingEnabled = true;
            this.listMaterials.Location = new System.Drawing.Point(10, 18);
            this.listMaterials.Name = "listMaterials";
            this.listMaterials.Size = new System.Drawing.Size(202, 469);
            this.listMaterials.TabIndex = 16;
            this.listMaterials.Visible = false;
            // 
            // checkDummyHandHeld
            // 
            this.checkDummyHandHeld.AutoSize = true;
            this.checkDummyHandHeld.Location = new System.Drawing.Point(10, 340);
            this.checkDummyHandHeld.Name = "checkDummyHandHeld";
            this.checkDummyHandHeld.Size = new System.Drawing.Size(185, 17);
            this.checkDummyHandHeld.TabIndex = 18;
            this.checkDummyHandHeld.Text = "Dummy for hand-held accessories";
            this.checkDummyHandHeld.UseVisualStyleBackColor = true;
            // 
            // checkShoulderCancel
            // 
            this.checkShoulderCancel.AutoSize = true;
            this.checkShoulderCancel.Location = new System.Drawing.Point(10, 363);
            this.checkShoulderCancel.Name = "checkShoulderCancel";
            this.checkShoulderCancel.Size = new System.Drawing.Size(130, 17);
            this.checkShoulderCancel.TabIndex = 19;
            this.checkShoulderCancel.Text = "Shoulder cancel bone";
            this.checkShoulderCancel.UseVisualStyleBackColor = true;
            // 
            // checkDummy
            // 
            this.checkDummy.AutoSize = true;
            this.checkDummy.Location = new System.Drawing.Point(10, 386);
            this.checkDummy.Name = "checkDummy";
            this.checkDummy.Size = new System.Drawing.Size(95, 17);
            this.checkDummy.TabIndex = 20;
            this.checkDummy.Text = "Dummy bone *";
            this.checkDummy.UseVisualStyleBackColor = true;
            // 
            // checkElbowPosOffset
            // 
            this.checkElbowPosOffset.AutoSize = true;
            this.checkElbowPosOffset.Checked = true;
            this.checkElbowPosOffset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkElbowPosOffset.Location = new System.Drawing.Point(28, 110);
            this.checkElbowPosOffset.Name = "checkElbowPosOffset";
            this.checkElbowPosOffset.Size = new System.Drawing.Size(182, 17);
            this.checkElbowPosOffset.TabIndex = 23;
            this.checkElbowPosOffset.Text = "Automatic rotation axis correction";
            this.checkElbowPosOffset.UseVisualStyleBackColor = true;
            // 
            // checkThumbLocalAxis
            // 
            this.checkThumbLocalAxis.AutoSize = true;
            this.checkThumbLocalAxis.Checked = true;
            this.checkThumbLocalAxis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkThumbLocalAxis.Location = new System.Drawing.Point(28, 408);
            this.checkThumbLocalAxis.Name = "checkThumbLocalAxis";
            this.checkThumbLocalAxis.Size = new System.Drawing.Size(144, 17);
            this.checkThumbLocalAxis.TabIndex = 24;
            this.checkThumbLocalAxis.Text = "Thumb local axis settings";
            this.checkThumbLocalAxis.UseVisualStyleBackColor = true;
            // 
            // checkToeEX
            // 
            this.checkToeEX.AutoSize = true;
            this.checkToeEX.Location = new System.Drawing.Point(10, 294);
            this.checkToeEX.Name = "checkToeEX";
            this.checkToeEX.Size = new System.Drawing.Size(69, 17);
            this.checkToeEX.TabIndex = 25;
            this.checkToeEX.Text = "Toe EX *";
            this.checkToeEX.UseVisualStyleBackColor = true;
            // 
            // checkLegDControll
            // 
            this.checkLegDControll.AutoSize = true;
            this.checkLegDControll.Location = new System.Drawing.Point(28, 317);
            this.checkLegDControll.Name = "checkLegDControll";
            this.checkLegDControll.Size = new System.Drawing.Size(167, 17);
            this.checkLegDControll.TabIndex = 26;
            this.checkLegDControll.Text = "Leg D-bone can be controlled";
            this.checkLegDControll.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listMaterials);
            this.groupBox1.Location = new System.Drawing.Point(259, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 502);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Materials to be automatically set weights:";
            // 
            // groupIPEXorPMX
            // 
            this.groupIPEXorPMX.Controls.Add(this.radioPMX);
            this.groupIPEXorPMX.Controls.Add(this.radioIPEX);
            this.groupIPEXorPMX.Location = new System.Drawing.Point(10, 17);
            this.groupIPEXorPMX.Name = "groupIPEXorPMX";
            this.groupIPEXorPMX.Size = new System.Drawing.Size(243, 41);
            this.groupIPEXorPMX.TabIndex = 28;
            this.groupIPEXorPMX.TabStop = false;
            this.groupIPEXorPMX.Text = "Type";
            // 
            // radioPMX
            // 
            this.radioPMX.AutoSize = true;
            this.radioPMX.Location = new System.Drawing.Point(114, 18);
            this.radioPMX.Name = "radioPMX";
            this.radioPMX.Size = new System.Drawing.Size(48, 17);
            this.radioPMX.TabIndex = 1;
            this.radioPMX.TabStop = true;
            this.radioPMX.Text = "PMX";
            this.radioPMX.UseVisualStyleBackColor = true;
            this.radioPMX.CheckedChanged += new System.EventHandler(this.checkedRadioType);
            // 
            // radioIPEX
            // 
            this.radioIPEX.AutoSize = true;
            this.radioIPEX.Location = new System.Drawing.Point(7, 20);
            this.radioIPEX.Name = "radioIPEX";
            this.radioIPEX.Size = new System.Drawing.Size(49, 17);
            this.radioIPEX.TabIndex = 0;
            this.radioIPEX.TabStop = true;
            this.radioIPEX.Text = "IPEX";
            this.radioIPEX.UseVisualStyleBackColor = true;
            this.radioIPEX.CheckedChanged += new System.EventHandler(this.checkedRadioType);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 528);
            this.Controls.Add(this.groupIPEXorPMX);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkLegDControll);
            this.Controls.Add(this.checkToeEX);
            this.Controls.Add(this.checkThumbLocalAxis);
            this.Controls.Add(this.checkElbowPosOffset);
            this.Controls.Add(this.checkDummy);
            this.Controls.Add(this.checkShoulderCancel);
            this.Controls.Add(this.checkDummyHandHeld);
            this.Controls.Add(this.btnMaterial);
            this.Controls.Add(this.checkToeIK);
            this.Controls.Add(this.checkOperationCenter);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.checkLegIK);
            this.Controls.Add(this.checkAllParent);
            this.Controls.Add(this.checkAutoBoneList);
            this.Controls.Add(this.checkSingleAxis);
            this.Controls.Add(this.checkWaist);
            this.Controls.Add(this.checkGrooveBone);
            this.Controls.Add(this.checkUpper2Bones);
            this.Controls.Add(this.checkHandTwist);
            this.Controls.Add(this.checkArmTwist);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Add semi-standard bone (Refrain)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupIPEXorPMX.ResumeLayout(false);
            this.groupIPEXorPMX.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		public Form1()
		{
			InitializeComponent();
			basewidth = base.Width;
			checkBoxList.Add(checkAllParent);
			checkBoxList.Add(checkArmTwist);
			checkBoxList.Add(checkHandTwist);
			checkBoxList.Add(checkUpper2Bones);
			checkBoxList.Add(checkGrooveBone);
			checkBoxList.Add(checkWaist);
			checkBoxList.Add(checkSingleAxis);
			checkBoxList.Add(checkLegIK);
			checkBoxList.Add(checkOperationCenter);
			checkBoxList.Add(checkToeIK);
			checkBoxList.Add(checkDummyHandHeld);
			checkBoxList.Add(checkShoulderCancel);
			checkBoxList.Add(checkDummy);
            checkBoxList.Add(checkToeEX);
            checkBoxList.Add(checkLegDControll);
            checkBoxList.Add(checkThumbLocalAxis);
        }

		private void button3_Click(object sender, EventArgs e)
		{
			bool flag = true;
			foreach (CheckBox checkBox in checkBoxList)
			{
				flag &= checkBox.Checked;
			}
			foreach (CheckBox checkBox2 in checkBoxList)
			{
				checkBox2.Checked = !flag;
			}
		}

		private void btnMaterial_Click(object sender, EventArgs e)
		{
			if (mat_mode)
			{
				btnMaterial.Text = btnMaterial.Text.Replace("<<", ">>");
				base.Width = basewidth;
				listMaterials.Visible = false;
			}
			else
			{
				btnMaterial.Text = btnMaterial.Text.Replace(">>", "<<");
				base.Width = basewidth + 220;
				listMaterials.Visible = true;
			}
			mat_mode = !mat_mode;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			checkElbowPosOffset.Enabled = checkArmTwist.Checked;
		}

		private void SaveCheckedItem()
		{
			string settingPath = SettingPath;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Control control in base.Controls)
			{
				if (control.Name.StartsWith("check") && ((CheckBox)control).Checked)
				{
					stringBuilder.AppendLine(control.Name);
				}
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(settingPath);
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
			catch
			{
			}
		}

		private void LoadCheckedItem()
		{
			string settingPath = SettingPath;
			List<string> list = new List<string>();
			if (!File.Exists(settingPath))
			{
				return;
			}
			try
			{
				StreamReader streamReader = new StreamReader(settingPath);
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine();
					if (text != "")
					{
						list.Add(text);
					}
				}
				streamReader.Close();
			}
			catch
			{
			}
			foreach (Control control in base.Controls)
			{
				if (!control.Name.StartsWith("check"))
				{
					continue;
				}
				foreach (string item in list)
				{
					if (control.Name == item)
					{
						((CheckBox)control).Checked = true;
					}
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadCheckedItem();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SaveCheckedItem();
		}

        private void checkedRadioType(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            bool ischeck = radioButton.Checked;
            listMaterials.Items.Clear();
            btnOK.Enabled = true;
            if (radioButton.Name == "radioIPEX" && ischeck)
            {
                IPEXPmd currentState = Connector.Pmd.GetCurrentStateEx();
                int num = 0;
                foreach (IPEXMaterial item in currentState.Material)
                {
                    listMaterials.Items.Add(num + ": " + item.Name);
                    listMaterials.SetItemChecked(num, true);
                    num++;
                }

                checkThumbLocalAxis.Enabled = false;
                checkToeEX.Enabled = false;
                checkLegDControll.Enabled = false;
                checkToeIK.Enabled = true;
            }
            else if (radioButton.Name == "radioPMX" && ischeck)
            {
                IPXPmx currentState = Connector.Pmx.GetCurrentState();
                int num = 0;
                foreach (IPXMaterial item in currentState.Material)
                {
                    listMaterials.Items.Add(num + ": " + item.Name);
                    listMaterials.SetItemChecked(num, true);
                    num++;
                }
                checkThumbLocalAxis.Enabled = true;
                checkToeEX.Enabled = true;
                checkLegDControll.Enabled = true;
                checkToeIK.Enabled = false;
            }
        }
    }
}
