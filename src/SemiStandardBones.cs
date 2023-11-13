using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PEPlugin;
using PEPlugin.Pmd;
using PEPlugin.Pmx;
using PEPlugin.SDX;

namespace SemiStandardBonesRefrain
{
    public class SemiStandardBones : IPEPlugin, IDisposable
	{
		public string Name { get{ return "SemiStandardBones.Refrain"; } }

        public string Description { get { return ""; } }

		public string Version 
		{
			get
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				Version version = executingAssembly.GetName().Version;
				return version.ToString();
			}
		}

		public IPEPluginOption Option { get { return new Option(); } }

		public void Run(IPERunArgs args)
		{
            Form1 form = new Form1();
            form.SettingPath = Path.Combine(args.Host.Connector.System.DefaultPluginFolderPath, Name + ".dat");
            form.Connector = args.Host.Connector;
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            bool isPMX = form.radioPMX.Checked;
            int materials = form.listMaterials.Items.Count - 1;
            materials = materials > 0 ? materials - 1 : 0;
            if (isPMX)
            {
                IPXPmx currentState = args.Host.Connector.Pmx.GetCurrentState();

                executePMX(form, currentState, materials, args);
            }
            else
            {
                IPEXPmd currentState = args.Host.Connector.Pmd.GetCurrentStateEx();

                executePMD(form, currentState, materials, args);
            }


		}

        private void executePMD(Form1 form, IPEXPmd currentState, int num, IPERunArgs args)
        {
            List<string> newBoneList = new List<string>();
            IList<IPEXVertex> list2 = currentState.Vertex;
            if (form.mat_mode)
            {
                List<IPEXMaterial> list3 = new List<IPEXMaterial>();
                list3.AddRange(currentState.Material);
                foreach (int checkedIndex in form.listMaterials.CheckedIndices)
                {
                    list3[checkedIndex] = null;
                }
                for (num = list3.Count - 1; num >= 0; num--)
                {
                    if (list3[num] == null)
                    {
                        list3.RemoveAt(num);
                    }
                }
                list2 = new List<IPEXVertex>();
                foreach (IPEXVertex item2 in currentState.Vertex)
                {
                    bool flag = false;
                    foreach (IPEXMaterial item3 in list3)
                    {
                        foreach (IPEXFace face in item3.Faces)
                        {
                            if (face.Vertex1.Equals(item2) || face.Vertex2.Equals(item2) || face.Vertex3.Equals(item2))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list2.Add(item2);
                    }
                }
            }
            string[,] array = new string[4, 5]
            {
                { "右腕捩", "右腕捩", "右腕", "右ひじ", "arm twist_R" },
                { "左腕捩", "左腕捩", "左腕", "左ひじ", "arm twist_L" },
                { "右手捩", "右手捩", "右ひじ", "右手首", "wrist twist_R" },
                { "左手捩", "左手捩", "左ひじ", "左手首", "wrist twist_L" }
            };
            for (int i = 0; i < 4; i++)
            {
                if ((0 > i || i >= 2 || !form.checkArmTwist.Checked) && (2 > i || i >= 4 || !form.checkHandTwist.Checked))
                {
                    continue;
                }
                IPEXBone iPEXBone = null;
                IPEXBone iPEXBone2 = null;
                string text = array[i, 0];
                string text2 = array[i, 1];
                string text3 = array[i, 2];
                string name = array[i, 3];
                string nameE = array[i, 4];
                IPEXBone iPEXBone3 = SearchBone(currentState, text);
                if (iPEXBone3 != null)
                {
                    continue;
                }
                int num2 = 0;
                List<IPEXVertex> list4 = new List<IPEXVertex>();
                new List<IPEXVertex>();
                int num3 = 0;
                foreach (IPEXBone item4 in currentState.Bone)
                {
                    if (item4.Name == text3)
                    {
                        iPEXBone = item4;
                        num2 = num3;
                    }
                    num3++;
                }
                iPEXBone2 = SearchBone(currentState, name);
                if (iPEXBone == null || iPEXBone2 == null)
                {
                    continue;
                }
                float num4 = 0f;
                if (form.checkElbowPosOffset.Checked && 0 <= i && i < 2)
                {
                    float num5 = 0f;
                    float num6 = 0f;
                    foreach (IPEXVertex item5 in list2)
                    {
                        if (IsDominatedVertex(item5, iPEXBone, 0.6f))
                        {
                            num6 += item5.Position.Z;
                            num5 += 1f;
                        }
                    }
                    if (num5 > 0f)
                    {
                        num4 = num6 / num5 - iPEXBone2.Position.Z;
                        num4 *= 0.75f;
                    }
                    iPEXBone2.Position.Z += num4;
                }
                float3 @float = new float3(iPEXBone.Position);
                float3 float2 = new float3(iPEXBone2.Position);
                float3 float3 = (@float + float2) * 0.5f;
                float3 float4 = float3.normalize(float2 - @float);
                float num7 = float3.dot(@float - float3, float4) * 0.75f;
                float num8 = float3.dot(float2 - float3, float4) * 0.75f;
                iPEXBone3 = args.Host.Builder.CreateXBone();
                IPEXBone iPEXBone4 = args.Host.Builder.CreateXBone();
                foreach (IPEXVertex item6 in list2)
                {
                    float num9 = float3.dot(new float3(item6.Position) - float3, float4);
                    if (IsDominatedVertex(item6, iPEXBone))
                    {
                        list4.Add(item6);
                        if (num7 > num9)
                        {
                            num7 = num9;
                        }
                        if (num8 < num9)
                        {
                            num8 = num9;
                        }
                    }
                    else if (item6.Bone1.Equals(iPEXBone) && item6.Bone2.Equals(iPEXBone2))
                    {
                        if (num9 > 0f)
                        {
                            item6.Bone1 = iPEXBone3;
                        }
                    }
                    else if (item6.Bone2.Equals(iPEXBone) && item6.Bone1.Equals(iPEXBone2) && num9 > 0f)
                    {
                        item6.Bone2 = iPEXBone3;
                    }
                }
                iPEXBone3.Kind = BoneKind.Twist;
                iPEXBone3.Name = text;
                iPEXBone3.Position = float3.ToIPEVec();
                iPEXBone3.Parent = iPEXBone;
                iPEXBone3.To = iPEXBone4;
                iPEXBone3.NameE = nameE;
                iPEXBone4.Kind = BoneKind.Unvisible;
                iPEXBone4.Name = text + "軸";
                iPEXBone4.Position = (float3 + float4).ToIPEVec();
                iPEXBone4.Parent = iPEXBone;
                IPEXBone[] array2 = new IPEXBone[5];
                for (num3 = 0; num3 < array2.Length; num3++)
                {
                    array2[num3] = args.Host.Builder.CreateXBone();
                    array2[num3].Kind = BoneKind.RotateRatio;
                    array2[num3].Name = text2 + num3;
                    array2[num3].Position = (float3 + float4 * float3.lerp(num7, num8, (float)num3 / ((float)array2.Length - 1f))).ToIPEVec();
                    array2[num3].Parent = iPEXBone;
                    array2[num3].To = iPEXBone3;
                    array2[num3].RotationRatio = num3 * 100 / (array2.Length - 1);
                }
                array2[0] = iPEXBone;
                array2[4] = iPEXBone3;
                foreach (IPEXVertex item7 in list4)
                {
                    float num10 = float3.dot(new float3(item7.Position) - float3, float4);
                    item7.Bone1 = iPEXBone2;
                    item7.Bone2 = iPEXBone;
                    num10 = (num10 - num7) / (num8 - num7);
                    num10 *= (float)(array2.Length - 1);
                    int num11 = (int)num10;
                    if (0 <= num11 && num11 < array2.Length)
                    {
                        item7.Bone2 = array2[num11];
                        if (num11 + 1 < array2.Length)
                        {
                            item7.Bone1 = array2[num11 + 1];
                        }
                    }
                    item7.Weight = (int)(100f * num10) % 100;
                }
                foreach (IPEXVertex item8 in list2)
                {
                    if (item8.Bone1.Equals(iPEXBone) && item8.Bone2.Equals(iPEXBone2))
                    {
                        item8.Bone1 = iPEXBone3;
                    }
                    if (item8.Bone1.Equals(iPEXBone2) && item8.Bone2.Equals(iPEXBone))
                    {
                        item8.Bone2 = iPEXBone3;
                    }
                }
                currentState.Bone.Insert(num2 + 1, iPEXBone3);
                currentState.Bone.Insert(num2 + 2, iPEXBone4);
                for (num3 = 1; num3 < array2.Length - 1; num3++)
                {
                    currentState.Bone.Insert(num2 + 2 + num3, array2[num3]);
                }
                iPEXBone2.Parent = iPEXBone3;
                iPEXBone2.Position.Z -= num4;
                newBoneList.Add(text);
                if (!form.checkAutoBoneList.Checked)
                {
                    continue;
                }
                foreach (IPEXFrameBone item9 in currentState.FrameBone)
                {
                    for (num3 = 0; num3 < item9.Bones.Count; num3++)
                    {
                        if (item9.Bones[num3].Equals(iPEXBone))
                        {
                            item9.Bones.Insert(num3 + 1, iPEXBone3);
                            num3 = -1;
                            break;
                        }
                    }
                    if (num3 < 0)
                    {
                        break;
                    }
                }
            }
            if (form.checkUpper2Bones.Checked)
            {
                bool flag2 = false;
                string text = "上半身2"; //upperbody 2
                string text3 = "上半身"; //upper body
                string name = "首"; //head
                string nameE = "upper body2";
                foreach (IPEXBone item10 in currentState.Bone)
                {
                    if (item10.Name == text)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    IPEXBone iPEXBone5 = null;
                    IPEXBone iPEXBone6 = null;
                    int num12 = 0;
                    List<IPEXVertex> list5 = new List<IPEXVertex>();
                    int num13 = 0;
                    foreach (IPEXBone item11 in currentState.Bone)
                    {
                        if (item11.Name == text3)
                        {
                            iPEXBone5 = item11;
                            num12 = num13;
                        }
                        num13++;
                    }
                    foreach (IPEXBone item12 in currentState.Bone)
                    {
                        if (item12.Name == name)
                        {
                            iPEXBone6 = item12;
                        }
                    }
                    if (iPEXBone5 != null && iPEXBone6 != null)
                    {
                        float3 x = new float3(iPEXBone5.Position);
                        float3 y = new float3(iPEXBone6.Position);
                        float3 float5 = float3.lerp(x, y, 0.4f);
                        IPEXBone iPEXBone7 = args.Host.Builder.CreateXBone();
                        iPEXBone7.Kind = BoneKind.Rotate;
                        iPEXBone7.Name = text;
                        iPEXBone7.Position = float5.ToIPEVec();
                        iPEXBone7.Parent = iPEXBone5;
                        iPEXBone7.To = iPEXBone6;
                        iPEXBone7.NameE = nameE;
                        foreach (IPEXVertex item13 in list2)
                        {
                            if ((item13.Bone1.Equals(iPEXBone5) && item13.Weight >= 98) || (item13.Bone2.Equals(iPEXBone5) && item13.Weight <= 2))
                            {
                                list5.Add(item13);
                                continue;
                            }
                            if (item13.Bone1.Equals(iPEXBone5) && item13.Position.Y > iPEXBone7.Position.Y)
                            {
                                item13.Bone1 = iPEXBone7;
                            }
                            if (item13.Bone2.Equals(iPEXBone5) && item13.Position.Y > iPEXBone7.Position.Y)
                            {
                                item13.Bone2 = iPEXBone7;
                            }
                        }
                        foreach (IPEXVertex item14 in list5)
                        {
                            float num14 = item14.Position.Y - iPEXBone7.Position.Y;
                            float num15 = item14.Position.Z - iPEXBone7.Position.Z;
                            if (num15 > 0f)
                            {
                                num14 += num15 / 2f;
                            }
                            float num16 = num14 / (iPEXBone6.Position.Y - iPEXBone7.Position.Y);
                            if (num16 < -0.35f)
                            {
                                item14.Weight = 50;
                                item14.Bone1 = iPEXBone5;
                                item14.Bone2 = iPEXBone7;
                            }
                            else if (num16 > 0.35f)
                            {
                                item14.Weight = 50;
                                item14.Bone1 = iPEXBone7;
                                item14.Bone2 = iPEXBone6;
                            }
                            else
                            {
                                item14.Weight = 50;
                                item14.Bone1 = iPEXBone7;
                                item14.Bone2 = iPEXBone5;
                            }
                        }
                        foreach (IPEXBody item15 in currentState.Body)
                        {
                            if (item15.Bone != null && item15.Bone.Equals(iPEXBone5) && item15.Position.Y >= iPEXBone7.Position.Y)
                            {
                                item15.Bone = iPEXBone7;
                            }
                        }
                        foreach (IPEXBone item16 in currentState.Bone)
                        {
                            if (item16.Parent != null && item16.Parent.Equals(iPEXBone5))
                            {
                                item16.Parent = iPEXBone7;
                            }
                        }
                        currentState.Bone.Insert(num12 + 1, iPEXBone7);
                        iPEXBone5.To = iPEXBone7;
                        newBoneList.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            foreach (IPEXFrameBone item17 in currentState.FrameBone)
                            {
                                for (num13 = 0; num13 < item17.Bones.Count; num13++)
                                {
                                    if (item17.Bones[num13].Equals(iPEXBone5))
                                    {
                                        item17.Bones.Insert(num13 + 1, iPEXBone7);
                                        num13 = -1;
                                        break;
                                    }
                                }
                                if (num13 < 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (form.checkWaist.Checked)
            {
                bool flag2 = false;
                string text = "腰";
                string nameE = "waist";
                string name = "下半身";
                foreach (IPEXBone item18 in currentState.Bone)
                {
                    if (item18.Name == text)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    IPEXBone iPEXBone8 = null;
                    IPEXBone iPEXBone9 = null;
                    IPEXBone iPEXBone10 = null;
                    int num17 = 0;
                    int j = 0;
                    foreach (IPEXBone item19 in currentState.Bone)
                    {
                        if (item19.Name == name)
                        {
                            iPEXBone9 = item19;
                        }
                    }
                    iPEXBone8 = iPEXBone9.Parent;
                    foreach (IPEXBone item20 in currentState.Bone)
                    {
                        if (item20.Equals(iPEXBone8))
                        {
                            num17 = j;
                            break;
                        }
                        j++;
                    }
                    foreach (IPEXBone item21 in currentState.Bone)
                    {
                        if (item21.Name == "右足")
                        {
                            iPEXBone10 = item21;
                        }
                    }
                    if (iPEXBone8 != null && iPEXBone9 != null && iPEXBone10 != null)
                    {
                        float3 float6 = new float3(iPEXBone9.Position);
                        float3 float7 = new float3(iPEXBone10.Position);
                        float3 float8 = default(float3);
                        float8.Y = float3.lerp(float6.Y, float7.Y, 0.6f);
                        float8.Z = float6.Y * 0.02f;
                        IPEXBone iPEXBone11 = args.Host.Builder.CreateXBone();
                        iPEXBone11.Kind = BoneKind.Rotate;
                        iPEXBone11.Name = text;
                        iPEXBone11.Position = float8.ToIPEVec();
                        iPEXBone11.Parent = iPEXBone8;
                        iPEXBone11.To = iPEXBone9;
                        iPEXBone11.NameE = nameE;
                        foreach (IPEXBone item22 in currentState.Bone)
                        {
                            if (item22.Parent != null && item22.Parent.Equals(iPEXBone8) && item22.Name != "センター先")
                            {
                                item22.Parent = iPEXBone11;
                            }
                        }
                        currentState.Bone.Insert(num17 + 1, iPEXBone11);
                        newBoneList.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            foreach (IPEXFrameBone item23 in currentState.FrameBone)
                            {
                                for (j = 0; j < item23.Bones.Count; j++)
                                {
                                    if (item23.Bones[j].Equals(iPEXBone8))
                                    {
                                        item23.Bones.Insert(j + 1, iPEXBone11);
                                        j = -1;
                                        break;
                                    }
                                }
                                if (j < 0)
                                {
                                    break;
                                }
                            }
                            if (j >= 0)
                            {
                                IPEXFrameBone frameBone = GetFrameBone(currentState, "センター", true);
                                frameBone.Bones.Add(iPEXBone11);
                            }
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            string text3;
                            if (i == 0)
                            {
                                text3 = "右足";
                                name = "腰キャンセル右";
                            }
                            else
                            {
                                text3 = "左足";
                                name = "腰キャンセル左";
                            }
                            iPEXBone8 = SearchBone(currentState, text3);
                            if (text3 != null)
                            {
                                iPEXBone9 = args.Host.Builder.CreateXBone();
                                iPEXBone9.Kind = BoneKind.RotateRatio;
                                iPEXBone9.Name = name;
                                iPEXBone9.Position = (IPEVector3)iPEXBone8.Position.Clone();
                                iPEXBone9.Parent = iPEXBone8.Parent;
                                iPEXBone9.To = iPEXBone11;
                                iPEXBone9.RotationRatio = -100;
                                iPEXBone8.Parent = iPEXBone9;
                                EntryBoneFore(currentState, iPEXBone8, iPEXBone9);
                            }
                        }
                    }
                }
            }
            if (form.checkGrooveBone.Checked)
            {
                bool flag2 = false;
                string text = "グルーブ";
                string text3 = "センター";
                string nameE = "groove";
                foreach (IPEXBone item24 in currentState.Bone)
                {
                    if (item24.Name == text)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    IPEXBone iPEXBone12 = null;
                    int num18 = 0;
                    int k = 0;
                    foreach (IPEXBone item25 in currentState.Bone)
                    {
                        if (item25.Name == text3)
                        {
                            iPEXBone12 = item25;
                            num18 = k;
                        }
                        k++;
                    }
                    if (iPEXBone12 != null)
                    {
                        IPEXBone iPEXBone13 = args.Host.Builder.CreateXBone();
                        IPEXBone iPEXBone14 = args.Host.Builder.CreateXBone();
                        iPEXBone13.Kind = BoneKind.RotateMove;
                        iPEXBone13.Name = text;
                        iPEXBone13.Position = (IPEVector3)iPEXBone12.Position.Clone();
                        iPEXBone13.Position.Y += 0.2f;
                        iPEXBone13.Parent = iPEXBone12;
                        iPEXBone13.To = iPEXBone14;
                        iPEXBone13.NameE = nameE;
                        iPEXBone14.Kind = BoneKind.Unvisible;
                        iPEXBone14.Name = text + "先";
                        iPEXBone14.Position = (IPEVector3)iPEXBone12.Position.Clone();
                        iPEXBone14.Position.Y += 1.2f;
                        iPEXBone14.Parent = iPEXBone13;
                        foreach (IPEXBone item26 in currentState.Bone)
                        {
                            if (item26.Parent != null && item26.Parent.Equals(iPEXBone12) && item26.Name != "センター先")
                            {
                                item26.Parent = iPEXBone13;
                            }
                        }
                        currentState.Bone.Insert(num18 + 1, iPEXBone13);
                        currentState.Bone.Insert(num18 + 2, iPEXBone14);
                        newBoneList.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            foreach (IPEXFrameBone item27 in currentState.FrameBone)
                            {
                                for (k = 0; k < item27.Bones.Count; k++)
                                {
                                    if (item27.Bones[k].Equals(iPEXBone12))
                                    {
                                        item27.Bones.Insert(k + 1, iPEXBone13);
                                        k = -1;
                                        break;
                                    }
                                }
                                if (k < 0)
                                {
                                    break;
                                }
                            }
                            if (k >= 0)
                            {
                                IPEXFrameBone frameBone2 = GetFrameBone(currentState, "センター", true);
                                frameBone2.Bones.Add(iPEXBone13);
                            }
                        }
                    }
                }
            }
            if (form.checkLegIK.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    bool flag2 = false;
                    string text;
                    string text3;
                    string nameE;
                    if (i == 0)
                    {
                        text = "右足IK親";
                        text3 = "右足ＩＫ";
                        nameE = "leg IKP_R";
                    }
                    else
                    {
                        text = "左足IK親";
                        text3 = "左足ＩＫ";
                        nameE = "leg IKP_L";
                    }
                    foreach (IPEXBone item28 in currentState.Bone)
                    {
                        if (item28.Name == text)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        continue;
                    }
                    IPEXBone iPEXBone15 = null;
                    int index2 = 0;
                    int num19 = 0;
                    foreach (IPEXBone item29 in currentState.Bone)
                    {
                        if (item29.Name == text3)
                        {
                            iPEXBone15 = item29;
                            index2 = num19;
                        }
                        num19++;
                    }
                    if (iPEXBone15 == null)
                    {
                        continue;
                    }
                    IPEXBone iPEXBone16 = args.Host.Builder.CreateXBone();
                    iPEXBone16.Kind = BoneKind.RotateMove;
                    iPEXBone16.Name = text;
                    iPEXBone16.Position = (IPEVector3)iPEXBone15.Position.Clone();
                    iPEXBone16.Position.Y = 0f;
                    iPEXBone16.Parent = iPEXBone15.Parent;
                    iPEXBone16.To = iPEXBone15;
                    iPEXBone16.NameE = nameE;
                    iPEXBone15.Parent = iPEXBone16;
                    currentState.Bone.Insert(index2, iPEXBone16);
                    newBoneList.Add(text);
                    if (!form.checkAutoBoneList.Checked)
                    {
                        continue;
                    }
                    foreach (IPEXFrameBone item30 in currentState.FrameBone)
                    {
                        for (num19 = 0; num19 < item30.Bones.Count; num19++)
                        {
                            if (item30.Bones[num19].Equals(iPEXBone15))
                            {
                                item30.Bones.Insert(num19, iPEXBone16);
                                num19 = -1;
                                break;
                            }
                        }
                        if (num19 < 0)
                        {
                            break;
                        }
                    }
                }
            }
            if (form.checkToeIK.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text4 = ((i != 0) ? "左" : "右");
                    string text5 = ((i != 0) ? "_L" : "_R");
                    string text = text4 + "足先ＩＫ";
                    string text3 = text4 + "足先";
                    string name = text4 + "足先先";
                    IPEXBone iPEXBone17 = SearchBone(currentState, text);
                    IPEXBone iPEXBone18 = SearchBone(currentState, text3);
                    IPEXBone iPEXBone19 = SearchBone(currentState, name);
                    IPEXBone iPEXBone20 = SearchBone(currentState, text4 + "足ＩＫ");
                    IPEXBone iPEXBone21 = SearchBone(currentState, text4 + "つま先ＩＫ");
                    IPEXBone iPEXBone22 = SearchBone(currentState, text4 + "足首");
                    IPEXBone iPEXBone23 = SearchBone(currentState, text4 + "つま先");
                    if (iPEXBone17 != null || iPEXBone18 != null || iPEXBone19 != null || iPEXBone20 == null || iPEXBone21 == null || iPEXBone22 == null || iPEXBone23 == null)
                    {
                        continue;
                    }
                    iPEXBone17 = args.Host.Builder.CreateXBone();
                    iPEXBone18 = args.Host.Builder.CreateXBone();
                    iPEXBone19 = args.Host.Builder.CreateXBone();
                    float3 float9 = float3.lerp(new float3(iPEXBone22.Position), new float3(iPEXBone23.Position), 2f / 3f);
                    iPEXBone17.Name = text;
                    iPEXBone17.NameE = "toe2 IK" + text5;
                    iPEXBone17.Kind = BoneKind.IK;
                    iPEXBone17.Position = args.Host.Builder.CreateVector3();
                    iPEXBone17.Position.X = iPEXBone21.Position.X;
                    iPEXBone17.Position.Y = float9.Y;
                    iPEXBone17.Position.Z = iPEXBone21.Position.Z;
                    iPEXBone17.Parent = iPEXBone21;
                    iPEXBone18.Name = text3;
                    iPEXBone18.NameE = "toe2" + text5;
                    iPEXBone18.Kind = BoneKind.IKLink;
                    iPEXBone18.Position = float9.ToIPEVec();
                    iPEXBone18.Parent = iPEXBone22;
                    iPEXBone18.To = iPEXBone19;
                    iPEXBone18.IKParent = iPEXBone20;
                    iPEXBone19.Name = name;
                    iPEXBone19.Kind = BoneKind.IKTo;
                    iPEXBone19.Position = (IPEVector3)iPEXBone17.Position.Clone();
                    iPEXBone19.Parent = iPEXBone18;
                    EntryBoneNext(currentState, iPEXBone17.Parent, iPEXBone17);
                    EntryBoneNext(currentState, iPEXBone18.Parent, iPEXBone18);
                    EntryBoneNext(currentState, iPEXBone19.Parent, iPEXBone19);
                    foreach (IPEXVertex item31 in list2)
                    {
                        if ((item31.Bone1.Equals(iPEXBone22) && item31.Weight >= 98) || (item31.Bone2.Equals(iPEXBone22) && item31.Weight <= 2))
                        {
                            item31.Bone1 = iPEXBone18;
                            item31.Bone2 = iPEXBone22;
                            float num20 = iPEXBone18.Position.Z - iPEXBone22.Position.Z;
                            float num21 = item31.Position.Z - iPEXBone22.Position.Z;
                            float num22 = num21 / num20;
                            num22 = (num22 - 0.75f) / 0.5f * 100f;
                            item31.Weight = (int)num22;
                            if (item31.Weight < 0)
                            {
                                item31.Weight = 0;
                            }
                            if (item31.Weight > 100)
                            {
                                item31.Weight = 100;
                            }
                        }
                    }
                    IPEXIK iPEXIK = args.Host.Builder.CreateXIK();
                    iPEXIK.IK = iPEXBone17;
                    iPEXIK.LimitOnce = 1f;
                    iPEXIK.LoopCount = 3;
                    iPEXIK.Target = iPEXBone19;
                    iPEXIK.Links.Add(iPEXBone18);
                    currentState.IK.Add(iPEXIK);
                    newBoneList.Add(text);
                    newBoneList.Add(text3);
                    if (form.checkAutoBoneList.Checked)
                    {
                        EntryBoneFrameNext(currentState, iPEXBone17.Parent, iPEXBone17);
                        EntryBoneFrameNext(currentState, iPEXBone18.Parent, iPEXBone18);
                    }
                }
            }
            if (form.checkDummyHandHeld.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string text3;
                    string nameE;
                    string text2;
                    string name;
                    if (i == 0)
                    {
                        text = "右ダミー";
                        text3 = "右手首";
                        nameE = "dummy_R";
                        text2 = "右ダミー先";
                        name = "右中指１";
                    }
                    else
                    {
                        text = "左ダミー";
                        text3 = "左手首";
                        nameE = "dummy_L";
                        text2 = "左ダミー先";
                        name = "左中指１";
                    }
                    IPEXBone iPEXBone24 = SearchBone(currentState, text);
                    IPEXBone iPEXBone25 = SearchBone(currentState, text3);
                    IPEXBone iPEXBone26 = SearchBone(currentState, name);
                    if (iPEXBone24 == null && iPEXBone25 != null && iPEXBone26 != null)
                    {
                        iPEXBone24 = args.Host.Builder.CreateXBone();
                        IPEXBone iPEXBone27 = args.Host.Builder.CreateXBone();
                        float3 float10 = new float3(iPEXBone25.Position);
                        float3 float11 = new float3(iPEXBone26.Position);
                        float11.z = float10.z;
                        float3 float12 = float3.normalize(float11 - float10);
                        float3 float13 = default(float3);
                        if (i == 0)
                        {
                            float13.x = 0f - float12.y;
                            float13.y = float12.x;
                        }
                        else
                        {
                            float13.x = float12.y;
                            float13.y = 0f - float12.x;
                        }
                        float3 float14 = (float10 + float11) * 0.5f;
                        iPEXBone24.Name = text;
                        iPEXBone24.NameE = nameE;
                        iPEXBone24.Kind = BoneKind.RotateMove;
                        iPEXBone24.Parent = iPEXBone25;
                        iPEXBone24.To = iPEXBone27;
                        iPEXBone24.Position = (float14 + float13 * 0.25f).ToIPEVec();
                        iPEXBone27.Name = text2;
                        iPEXBone27.Kind = BoneKind.Unvisible;
                        iPEXBone27.Parent = iPEXBone24;
                        iPEXBone27.Position = (float14 + float13 * 1f).ToIPEVec();
                        EntryBoneNext(currentState, iPEXBone24.Parent, iPEXBone24);
                        EntryBoneNext(currentState, iPEXBone27.Parent, iPEXBone27);
                        newBoneList.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameNext(currentState, iPEXBone24.Parent, iPEXBone24);
                        }
                    }
                }
            }
            if (form.checkShoulderCancel.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string text2;
                    string nameE;
                    string text3;
                    string name;
                    if (i == 0)
                    {
                        text = "右肩P";
                        text2 = "右肩C";
                        nameE = "shoulderP_R";
                        text3 = "右肩";
                        name = "右腕";
                    }
                    else
                    {
                        text = "左肩P";
                        text2 = "左肩C";
                        nameE = "shoulderP_L";
                        text3 = "左肩";
                        name = "左腕";
                    }
                    IPEXBone iPEXBone28 = SearchBone(currentState, text);
                    IPEXBone iPEXBone29 = SearchBone(currentState, text3);
                    IPEXBone iPEXBone30 = SearchBone(currentState, name);
                    if (iPEXBone28 == null && iPEXBone29 != null && iPEXBone30 != null)
                    {
                        iPEXBone28 = args.Host.Builder.CreateXBone();
                        IPEXBone iPEXBone31 = args.Host.Builder.CreateXBone();
                        iPEXBone28.Name = text;
                        iPEXBone28.NameE = nameE;
                        iPEXBone28.Parent = iPEXBone29.Parent;
                        iPEXBone28.Kind = BoneKind.Rotate;
                        iPEXBone28.Position = (IPEVector3)iPEXBone29.Position.Clone();
                        iPEXBone31.Name = text2;
                        iPEXBone31.Parent = iPEXBone29;
                        iPEXBone31.Kind = BoneKind.RotateRatio;
                        iPEXBone31.Position = (IPEVector3)iPEXBone30.Position.Clone();
                        iPEXBone31.RotationRatio = -100;
                        iPEXBone31.To = iPEXBone28;
                        iPEXBone29.Parent = iPEXBone28;
                        iPEXBone30.Parent = iPEXBone31;
                        EntryBoneFore(currentState, iPEXBone29, iPEXBone28);
                        EntryBoneNext(currentState, iPEXBone29, iPEXBone31);
                        newBoneList.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameFore(currentState, iPEXBone29, iPEXBone28);
                        }
                    }
                }
            }
            if (form.checkDummy.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string nameE;
                    string text3;
                    string name;
                    if (i == 0)
                    {
                        text = "右親指０";
                        nameE = "thumb0_R";
                        text3 = "右手首";
                        name = "右親指１";
                    }
                    else
                    {
                        text = "左親指０";
                        nameE = "thumb0_L";
                        text3 = "左手首";
                        name = "左親指１";
                    }
                    IPEXBone iPEXBone32 = SearchBone(currentState, text);
                    IPEXBone iPEXBone33 = SearchBone(currentState, text3);
                    IPEXBone iPEXBone34 = SearchBone(currentState, name);
                    if (iPEXBone32 != null || iPEXBone33 == null || iPEXBone34 == null)
                    {
                        continue;
                    }
                    float3 float15 = new float3(iPEXBone33.Position);
                    float3 float16 = new float3(iPEXBone34.Position);
                    float3 float17 = float3.lerp(float15, float16, 2f / 3f);
                    float3.normalize(float16 - float15);
                    iPEXBone32 = args.Host.Builder.CreateXBone();
                    iPEXBone32.Name = text;
                    iPEXBone32.NameE = nameE;
                    iPEXBone32.Kind = BoneKind.Rotate;
                    iPEXBone32.Parent = iPEXBone33;
                    iPEXBone32.To = iPEXBone34;
                    iPEXBone32.Position = float17.ToIPEVec();
                    iPEXBone34.Parent = iPEXBone32;
                    EntryBoneFore(currentState, iPEXBone34, iPEXBone32);
                    float num23 = float3.length(float16 - float17);
                    float3 vec = ((i != 0) ? new float3(-1f, -1f, 0f) : new float3(1f, -1f, 0f));
                    vec = float3.normalize(vec);
                    foreach (IPEXVertex item32 in list2)
                    {
                        if (!VertexBoneHas(item32, iPEXBone33) && !VertexBoneHas(item32, iPEXBone34))
                        {
                            continue;
                        }
                        float3 float18 = new float3(item32.Position) - (float17 + float16) * 0.5f;
                        float num24 = float3.length(float18 - vec * float3.dot(float18, vec));
                        num24 /= num23 * 1.4f;
                        if (!(num24 < 1f))
                        {
                            continue;
                        }
                        float num25 = (1f - num24) * 1.3f;
                        if (num25 < 0f)
                        {
                            num25 = 0f;
                        }
                        if (num25 > 1f)
                        {
                            num25 = 1f;
                        }
                        int num26 = (int)(num25 * 100f);
                        if (IsDominatedVertex(item32, iPEXBone33))
                        {
                            item32.Bone1 = iPEXBone32;
                            item32.Bone2 = iPEXBone33;
                            item32.Weight = num26;
                        }
                        else if (IsDominatedVertex(item32, iPEXBone34))
                        {
                            item32.Bone1 = iPEXBone32;
                            item32.Bone2 = iPEXBone34;
                            item32.Weight = num26;
                        }
                        else if (item32.Bone1.Equals(iPEXBone33))
                        {
                            item32.Bone1 = iPEXBone32;
                            if (item32.Weight < num26)
                            {
                                item32.Weight = num26;
                            }
                        }
                        else if (item32.Bone2.Equals(iPEXBone33))
                        {
                            item32.Bone2 = iPEXBone32;
                            if (100 - item32.Weight < num26)
                            {
                                item32.Weight = 100 - num26;
                            }
                        }
                    }
                    newBoneList.Add(text);
                    if (form.checkAutoBoneList.Checked)
                    {
                        EntryBoneFrameFore(currentState, iPEXBone34, iPEXBone32);
                    }
                }
            }
            if (form.checkAllParent.Checked)
            {
                bool flag2 = false;
                string text = "全ての親";
                string nameE = "master";
                if (SearchBone(currentState, text) == null)
                {
                    IPEXBone iPEXBone35 = null;
                    iPEXBone35 = currentState.Bone[0];
                    IPEXBone iPEXBone36 = args.Host.Builder.CreateXBone();
                    iPEXBone36.Kind = BoneKind.RotateMove;
                    iPEXBone36.Name = text;
                    iPEXBone36.Position = args.Host.Builder.CreateVector3();
                    iPEXBone36.Parent = null;
                    iPEXBone36.To = iPEXBone35;
                    iPEXBone36.NameE = nameE;
                    foreach (IPEXBone item33 in currentState.Bone)
                    {
                        if (item33.Parent == null)
                        {
                            item33.Parent = iPEXBone36;
                        }
                        if (item33.To != null && item33.To.Equals(currentState.Bone[0]))
                        {
                            item33.To = iPEXBone36;
                        }
                        if (item33.IKParent != null && item33.IKParent.Equals(currentState.Bone[0]))
                        {
                            item33.IKParent = iPEXBone36;
                        }
                    }
                    currentState.Bone.Insert(0, iPEXBone36);
                    newBoneList.Add(text);
                    if (iPEXBone35 != null && form.checkAutoBoneList.Checked)
                    {
                        flag2 = false;
                        foreach (IPEXFrameBone item34 in currentState.FrameBone)
                        {
                            int l;
                            for (l = 0; l < item34.Bones.Count; l++)
                            {
                                if (item34.Bones[l].Equals(iPEXBone35))
                                {
                                    flag2 = true;
                                    l = -1;
                                    break;
                                }
                            }
                            if (l < 0)
                            {
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            IPEXFrameBone frameBone3 = GetFrameBone(currentState, "センター", true);
                            frameBone3.Bones.Insert(0, iPEXBone35);
                            frameBone3.NameE = "center";
                        }
                    }
                }
            }
            if (form.checkOperationCenter.Checked)
            {
                bool flag2 = false;
                string text = "操作中心";
                string nameE = "view cnt";
                if (SearchBone(currentState, text) == null)
                {
                    IPEXBone iPEXBone37 = currentState.Bone[0];
                    IPEXBone iPEXBone38 = args.Host.Builder.CreateXBone();
                    iPEXBone38.Kind = BoneKind.RotateMove;
                    iPEXBone38.Name = text;
                    iPEXBone38.Position = args.Host.Builder.CreateVector3();
                    iPEXBone38.Parent = null;
                    iPEXBone38.NameE = nameE;
                    foreach (IPEXBone item35 in currentState.Bone)
                    {
                        if (item35.To != null && item35.To.Equals(currentState.Bone[0]))
                        {
                            item35.To = iPEXBone38;
                        }
                        if (item35.IKParent != null && item35.IKParent.Equals(currentState.Bone[0]))
                        {
                            item35.IKParent = iPEXBone38;
                        }
                    }
                    currentState.Bone.Insert(0, iPEXBone38);
                    newBoneList.Add(text);
                    if (form.checkAutoBoneList.Checked)
                    {
                        flag2 = false;
                        foreach (IPEXFrameBone item36 in currentState.FrameBone)
                        {
                            int m;
                            for (m = 0; m < item36.Bones.Count; m++)
                            {
                                if (item36.Bones[m].Equals(iPEXBone37))
                                {
                                    flag2 = true;
                                    m = -1;
                                    break;
                                }
                            }
                            if (m < 0)
                            {
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            IPEXFrameBone frameBone4 = GetFrameBone(currentState, "センター", true);
                            frameBone4.Bones.Insert(0, iPEXBone37);
                            frameBone4.NameE = "center";
                        }
                    }
                }
            }
            string resultText = "Added the following bones: " + Environment.NewLine;
            foreach (string boneName in newBoneList)
            {
                resultText = resultText + boneName + Environment.NewLine;
            }
            if (newBoneList.Count <= 0)
            {
                resultText = "No bones were added.";
            }

            MessageBox.Show(resultText, Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            args.Host.Connector.Pmd.UpdateEx(currentState);
            args.Host.Connector.Form.UpdateList(UpdateObject.Bone);
            args.Host.Connector.Form.UpdateList(UpdateObject.FrameBone);
            args.Host.Connector.Form.UpdateList(UpdateObject.IK);
            args.Host.Connector.View.PMDView.UpdateModel();
            args.Host.Connector.View.PMDView.UpdateView();
            args.Host.Connector.View.TransformView.UpdateView();
        }

        private void executePMX(Form1 form, IPXPmx currentState, int num, IPERunArgs args)
        {
            List<string> list = new List<string>();
            IList<IPXVertex> list3;
            if (form.mat_mode)
            {
                List<IPXMaterial> list2 = new List<IPXMaterial>();
                list2.AddRange(currentState.Material);
                foreach (int checkedIndex in form.listMaterials.CheckedIndices)
                {
                    list2[checkedIndex] = null;
                }
                for (num = list2.Count - 1; num >= 0; num--)
                {
                    if (list2[num] == null)
                    {
                        list2.RemoveAt(num);
                    }
                }
                list3 = new List<IPXVertex>();
                foreach (IPXVertex item2 in currentState.Vertex)
                {
                    bool flag = false;
                    foreach (IPXMaterial item3 in list2)
                    {
                        foreach (IPXFace face in item3.Faces)
                        {
                            if (face.Vertex1.Equals(item2) || face.Vertex2.Equals(item2) || face.Vertex3.Equals(item2))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list3.Add(item2);
                    }
                }
            }
            else
            {
                list3 = currentState.Vertex;
            }
            string[,] array = new string[4, 5]
            {
                { "右腕捩", "右腕捩", "右腕", "右ひじ", "arm twist_R" },
                { "左腕捩", "左腕捩", "左腕", "左ひじ", "arm twist_L" },
                { "右手捩", "右手捩", "右ひじ", "右手首", "wrist twist_R" },
                { "左手捩", "左手捩", "左ひじ", "左手首", "wrist twist_L" }
            };
            for (int i = 0; i < 4; i++)
            {
                if ((0 > i || i >= 2 || !form.checkArmTwist.Checked) && (2 > i || i >= 4 || !form.checkHandTwist.Checked))
                {
                    continue;
                }
                string text = array[i, 0];
                string text2 = array[i, 1];
                string text3 = array[i, 2];
                string name = array[i, 3];
                string nameE = array[i, 4];
                IPXBone iPXBone = null;
                IPXBone iPXBone2 = null;
                IPXBone iPXBone3 = SearchBone(currentState, text);
                if (iPXBone3 != null)
                {
                    continue;
                }
                int num2 = 0;
                List<IPXVertex> list4 = new List<IPXVertex>();
                List<IPXVertex> list5 = new List<IPXVertex>();
                int num3 = 0;
                foreach (IPXBone item4 in currentState.Bone)
                {
                    if (item4.Name == text3)
                    {
                        iPXBone = item4;
                        num2 = num3;
                    }
                    num3++;
                }
                iPXBone2 = SearchBone(currentState, name);
                if (iPXBone == null || iPXBone2 == null)
                {
                    continue;
                }
                float num4 = 0f;
                if (form.checkElbowPosOffset.Checked && 0 <= i && i < 2)
                {
                    float num5 = 0f;
                    float num6 = 0f;
                    foreach (IPXVertex item5 in list3)
                    {
                        if (IsDominatedVertex(item5, iPXBone, 0.6f))
                        {
                            num6 += item5.Position.Z;
                            num5 += 1f;
                        }
                    }
                    if (num5 > 0f)
                    {
                        num4 = num6 / num5 - iPXBone2.Position.Z;
                        num4 *= 0.75f;
                    }
                    iPXBone2.Position.Z += num4;
                }
                float3 @float = new float3(iPXBone.Position);
                float3 float2 = new float3(iPXBone2.Position);
                float3 float3 = float3.lerp(@float, float2, 0.6f);
                float3 float4 = float3.normalize(float2 - @float);
                float num7 = float3.length(float2 - @float);
                float num8 = float3.dot(@float - float3, float4) * 0.8f;
                float num9 = float3.dot(float2 - float3, float4) * 0.8f;
                iPXBone3 = args.Host.Builder.Pmx.Bone();
                foreach (IPXVertex item6 in list3)
                {
                    float num10 = float3.dot(new float3(item6.Position) - float3, float4);
                    if (IsDominatedVertex(item6, iPXBone))
                    {
                        list4.Add(item6);
                        if (num8 > num10)
                        {
                            num8 = num10;
                        }
                        if (num9 < num10)
                        {
                            num9 = num10;
                        }
                    }
                    else
                    {
                        if (num10 > 0f && num10 < num7)
                        {
                            list5.Add(item6);
                        }
                        if (VertexBoneHas(item6, iPXBone2) && num10 > 0f)
                        {
                            VertexBoneReplace(item6, iPXBone, iPXBone3);
                        }
                    }
                }
                iPXBone3.Name = text;
                iPXBone3.NameE = nameE;
                iPXBone3.Position = float3.ToV3();
                iPXBone3.Parent = iPXBone;
                iPXBone3.FixAxis = float4.ToV3();
                iPXBone3.IsRotation = true;
                iPXBone3.IsFixAxis = true;
                IPXBone[] array2 = new IPXBone[5];
                for (num3 = 0; num3 < array2.Length; num3++)
                {
                    array2[num3] = args.Host.Builder.Pmx.Bone();
                    array2[num3].Name = text2 + num3;
                    array2[num3].Position = (float3 + float4 * float3.lerp(num8, num9, (float)num3 / ((float)array2.Length - 1f))).ToV3();
                    array2[num3].Parent = iPXBone;
                    array2[num3].AppendParent = iPXBone3;
                    array2[num3].AppendRatio = (float)num3 / (float)(array2.Length - 1);
                    array2[num3].IsAppendRotation = true;
                    array2[num3].Visible = false;
                }
                array2[0] = iPXBone;
                array2[4] = iPXBone3;
                foreach (IPXVertex item7 in list4)
                {
                    float num11 = float3.dot(new float3(item7.Position) - float3, float4);
                    item7.Bone1 = iPXBone2;
                    item7.Bone2 = iPXBone;
                    item7.Bone3 = null;
                    item7.Bone4 = null;
                    item7.SDEF = false;
                    item7.Weight3 = 0f;
                    item7.Weight4 = 0f;
                    num11 = (num11 - num8) / (num9 - num8);
                    num11 *= (float)(array2.Length - 1);
                    int num12 = (int)num11;
                    if (0 <= num12 && num12 < array2.Length)
                    {
                        item7.Bone2 = array2[num12];
                        if (num12 + 1 < array2.Length)
                        {
                            item7.Bone1 = array2[num12 + 1];
                        }
                    }
                    item7.Weight1 = (float)((int)(100f * num11) % 100) / 100f;
                    item7.Weight2 = 1f - item7.Weight1;
                }
                currentState.Bone.Insert(num2 + 1, iPXBone3);
                for (num3 = 1; num3 < array2.Length - 1; num3++)
                {
                    currentState.Bone.Insert(num2 + 1 + num3, array2[num3]);
                }
                iPXBone2.Parent = iPXBone3;
                iPXBone2.Position.Z -= num4;
                list.Add(text);
                if (form.checkAutoBoneList.Checked)
                {
                    EntryBoneFrameNext(currentState, iPXBone, iPXBone3);
                }
            }
            if (form.checkUpper2Bones.Checked)
            {
                string text = "上半身2";
                string text3 = "上半身";
                string name = "首";
                string nameE = "upper body2";
                IPXBone iPXBone4 = null;
                IPXBone iPXBone5 = null;
                IPXBone iPXBone6 = SearchBone(currentState, text);
                if (iPXBone6 == null)
                {
                    List<IPXVertex> list6 = new List<IPXVertex>();
                    iPXBone4 = SearchBone(currentState, text3);
                    iPXBone5 = SearchBone(currentState, name);
                    if (iPXBone4 != null && iPXBone5 != null)
                    {
                        float3 x = new float3(iPXBone4.Position);
                        float3 y = new float3(iPXBone5.Position);
                        float3 float5 = float3.lerp(x, y, 0.35f);
                        iPXBone6 = args.Host.Builder.Pmx.Bone();
                        iPXBone6.IsRotation = true;
                        iPXBone6.IsTranslation = false;
                        iPXBone6.Name = text;
                        iPXBone6.Position = float5.ToV3();
                        iPXBone6.Parent = iPXBone4;
                        iPXBone6.NameE = nameE;
                        iPXBone6.Level = iPXBone4.Level;
                        iPXBone6.ToOffset = ((new float3(iPXBone5.Position) - new float3(iPXBone6.Position)) * 0.8f).ToV3();
                        foreach (IPXVertex item8 in list3)
                        {
                            if (IsDominatedVertex(item8, iPXBone4))
                            {
                                list6.Add(item8);
                            }
                            else if (item8.Position.Y > iPXBone6.Position.Y)
                            {
                                VertexBoneReplace(item8, iPXBone4, iPXBone6);
                            }
                        }
                        foreach (IPXVertex item9 in list6)
                        {
                            float num13 = item9.Position.Y - iPXBone6.Position.Y;
                            float num14 = item9.Position.Z - iPXBone6.Position.Z;
                            if (num14 > 0f)
                            {
                                num13 += num14 / 2f;
                            }
                            float num15 = num13 / (iPXBone5.Position.Y - iPXBone6.Position.Y);
                            item9.Bone3 = null;
                            item9.Bone4 = null;
                            item9.SDEF = false;
                            item9.Weight3 = 0f;
                            item9.Weight4 = 0f;
                            if (num15 < -0.35f)
                            {
                                item9.Weight1 = 1f;
                                item9.Weight2 = 0f;
                                item9.Bone1 = iPXBone4;
                                item9.Bone2 = iPXBone6;
                            }
                            else if (num15 > 0.35f)
                            {
                                item9.Weight1 = 1f;
                                item9.Weight2 = 0f;
                                item9.Bone1 = iPXBone6;
                                item9.Bone2 = iPXBone5;
                            }
                            else
                            {
                                item9.Weight1 = (float)(int)(100f * (num15 + 0.35f) / 0.7f) / 100f;
                                item9.Weight2 = 1f - item9.Weight1;
                                item9.Bone1 = iPXBone6;
                                item9.Bone2 = iPXBone4;
                            }
                        }
                        foreach (IPXBody item10 in currentState.Body)
                        {
                            if (item10.Bone != null && item10.Bone.Equals(iPXBone4) && item10.Position.Y >= iPXBone6.Position.Y)
                            {
                                item10.Bone = iPXBone6;
                            }
                        }
                        foreach (IPXBone item11 in currentState.Bone)
                        {
                            if (item11.Parent != null && item11.Parent.Equals(iPXBone4))
                            {
                                item11.Parent = iPXBone6;
                            }
                        }
                        EntryBoneNext(currentState, iPXBone4, iPXBone6);
                        iPXBone4.ToBone = iPXBone6;
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameNext(currentState, iPXBone4, iPXBone6);
                        }
                    }
                }
            }
            if (form.checkWaist.Checked)
            {
                string text = "腰";
                string nameE = "waist";
                string name = "下半身";
                IPXBone iPXBone7 = null;
                IPXBone iPXBone8 = null;
                IPXBone iPXBone9 = null;
                IPXBone iPXBone10 = SearchBone(currentState, text);
                if (iPXBone10 == null)
                {
                    iPXBone8 = SearchBone(currentState, name);
                    iPXBone7 = iPXBone8.Parent;
                    iPXBone9 = SearchBone(currentState, "右足");
                    if (iPXBone7 != null && iPXBone8 != null && iPXBone9 != null)
                    {
                        float3 float6 = new float3(iPXBone8.Position);
                        float3 float7 = new float3(iPXBone9.Position);
                        float3 float8 = default(float3);
                        float8.Y = float3.lerp(float6.Y, float7.Y, 0.6f);
                        float8.Z = float6.Y * 0.02f;
                        iPXBone10 = args.Host.Builder.Pmx.Bone();
                        iPXBone10.IsRotation = true;
                        iPXBone10.IsTranslation = false;
                        iPXBone10.Name = text;
                        iPXBone10.Position = float8.ToV3();
                        iPXBone10.Parent = iPXBone7;
                        iPXBone10.NameE = nameE;
                        iPXBone10.Level = iPXBone7.Level;
                        iPXBone10.ToOffset = ((new float3(iPXBone8.Position) - new float3(iPXBone10.Position)) * 0.8f).ToV3();
                        foreach (IPXBone item12 in currentState.Bone)
                        {
                            if (item12.Parent != null && item12.Parent.Equals(iPXBone7) && item12.Name != "センター先")
                            {
                                item12.Parent = iPXBone10;
                            }
                        }
                        EntryBoneNext(currentState, iPXBone7, iPXBone10);
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked && !EntryBoneFrameNext(currentState, iPXBone7, iPXBone10))
                        {
                            IPXNode frameBone = GetFrameBone(currentState, "センター", true);
                            frameBone.Items.Add(args.Host.Builder.Pmx.BoneNodeItem(iPXBone10));
                            frameBone.NameE = "center";
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            string text3;
                            if (i == 0)
                            {
                                text3 = "右足";
                                name = "腰キャンセル右";
                            }
                            else
                            {
                                text3 = "左足";
                                name = "腰キャンセル左";
                            }
                            iPXBone7 = SearchBone(currentState, text3);
                            if (text3 != null)
                            {
                                iPXBone8 = args.Host.Builder.Pmx.Bone();
                                iPXBone8.Name = name;
                                iPXBone8.Position = iPXBone7.Position.Clone();
                                iPXBone8.Parent = iPXBone7.Parent;
                                iPXBone8.IsAppendRotation = true;
                                iPXBone8.AppendParent = iPXBone10;
                                iPXBone8.AppendRatio = -1f;
                                iPXBone8.Visible = false;
                                iPXBone7.Parent = iPXBone8;
                                EntryBoneFore(currentState, iPXBone7, iPXBone8);
                            }
                        }
                    }
                }
            }
            if (form.checkGrooveBone.Checked)
            {
                string text = "グルーブ";
                string text3 = "センター";
                string nameE = "groove";
                IPXBone iPXBone11 = null;
                IPXBone iPXBone12 = SearchBone(currentState, text);
                if (iPXBone12 == null)
                {
                    iPXBone11 = SearchBone(currentState, text3);
                    if (iPXBone11 != null)
                    {
                        iPXBone12 = args.Host.Builder.Pmx.Bone();
                        iPXBone12.IsRotation = true;
                        iPXBone12.IsTranslation = true;
                        iPXBone12.Name = text;
                        iPXBone12.Position = iPXBone11.Position.Clone();
                        iPXBone12.Position.Y += 0.2f;
                        iPXBone12.Parent = iPXBone11;
                        iPXBone12.NameE = nameE;
                        iPXBone12.ToOffset = new V3(0f, 1.4f, 0f);
                        foreach (IPXBone item13 in currentState.Bone)
                        {
                            if (item13.Parent != null && item13.Parent.Equals(iPXBone11) && item13.Name != "センター先")
                            {
                                item13.Parent = iPXBone12;
                            }
                        }
                        EntryBoneNext(currentState, iPXBone11, iPXBone12);
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked && !EntryBoneFrameNext(currentState, iPXBone11, iPXBone12))
                        {
                            IPXNode frameBone2 = GetFrameBone(currentState, "センター", true);
                            frameBone2.Items.Add(args.Host.Builder.Pmx.BoneNodeItem(iPXBone12));
                            frameBone2.NameE = "center";
                        }
                    }
                }
            }
            if (form.checkLegIK.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string text3;
                    string nameE;
                    if (i == 0)
                    {
                        text = "右足IK親";
                        text3 = "右足ＩＫ";
                        nameE = "leg IKP_R";
                    }
                    else
                    {
                        text = "左足IK親";
                        text3 = "左足ＩＫ";
                        nameE = "leg IKP_L";
                    }
                    IPXBone iPXBone13 = null;
                    IPXBone iPXBone14 = SearchBone(currentState, text);
                    if (iPXBone14 != null)
                    {
                        continue;
                    }
                    iPXBone13 = SearchBone(currentState, text3);
                    if (iPXBone13 != null)
                    {
                        iPXBone14 = args.Host.Builder.Pmx.Bone();
                        iPXBone14.IsRotation = true;
                        iPXBone14.IsTranslation = true;
                        iPXBone14.Name = text;
                        iPXBone14.Position = iPXBone13.Position.Clone();
                        iPXBone14.Position.Y = 0f;
                        iPXBone14.Parent = iPXBone13.Parent;
                        iPXBone14.ToBone = iPXBone13;
                        iPXBone14.NameE = nameE;
                        iPXBone13.Parent = iPXBone14;
                        EntryBoneFore(currentState, iPXBone13, iPXBone14);
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameFore(currentState, iPXBone13, iPXBone14);
                        }
                    }
                }
            }
            if (form.checkToeEX.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text4 = ((i != 0) ? "左" : "右");
                    string text5 = ((i != 0) ? "_L" : "_R");
                    string text = text4 + "足先EX";
                    IPXBone iPXBone15 = SearchBone(currentState, text);
                    IPXBone iPXBone16 = SearchBone(currentState, text4 + "足");
                    IPXBone iPXBone17 = SearchBone(currentState, text4 + "ひざ");
                    IPXBone iPXBone18 = SearchBone(currentState, text4 + "足首");
                    IPXBone iPXBone19 = SearchBone(currentState, text4 + "つま先ＩＫ");
                    if (iPXBone15 != null || iPXBone16 == null || iPXBone17 == null || iPXBone18 == null || iPXBone19 == null)
                    {
                        continue;
                    }
                    iPXBone15 = args.Host.Builder.Pmx.Bone();
                    IPXBone iPXBone20 = (IPXBone)iPXBone16.Clone();
                    IPXBone iPXBone21 = (IPXBone)iPXBone17.Clone();
                    IPXBone iPXBone22 = (IPXBone)iPXBone18.Clone();
                    iPXBone20.AppendParent = iPXBone16;
                    iPXBone20.AppendRatio = 1f;
                    iPXBone20.IsAppendRotation = true;
                    iPXBone20.Controllable = form.checkLegDControll.Checked;
                    iPXBone20.Visible = form.checkLegDControll.Checked;
                    iPXBone20.ToBone = null;
                    iPXBone20.ToOffset = new V3();
                    iPXBone20.Level++;
                    iPXBone20.Name += "D";
                    iPXBone20.NameE += "D";
                    iPXBone21.Parent = iPXBone20;
                    iPXBone21.AppendParent = iPXBone17;
                    iPXBone21.AppendRatio = 1f;
                    iPXBone21.IsAppendRotation = true;
                    iPXBone21.Controllable = form.checkLegDControll.Checked;
                    iPXBone21.Visible = form.checkLegDControll.Checked;
                    iPXBone21.ToBone = null;
                    iPXBone21.ToOffset = new V3();
                    iPXBone21.Level++;
                    iPXBone21.Name += "D";
                    iPXBone21.NameE += "D";
                    iPXBone22.Parent = iPXBone21;
                    iPXBone22.AppendParent = iPXBone18;
                    iPXBone22.AppendRatio = 1f;
                    iPXBone22.IsAppendRotation = true;
                    iPXBone22.Controllable = form.checkLegDControll.Checked;
                    iPXBone22.Visible = form.checkLegDControll.Checked;
                    iPXBone22.ToBone = null;
                    iPXBone22.ToOffset = new V3();
                    iPXBone22.Level++;
                    iPXBone22.Name += "D";
                    iPXBone22.NameE += "D";
                    iPXBone15.Name = text;
                    iPXBone15.NameE = "toe2" + text5;
                    iPXBone15.IsRotation = true;
                    iPXBone15.IsTranslation = false;
                    iPXBone15.Position = float3.lerp(new float3(iPXBone18.Position), new float3(iPXBone19.Position), 2f / 3f).ToV3();
                    iPXBone15.Parent = iPXBone22;
                    iPXBone15.ToOffset = new V3(0f, 0f, -1f);
                    iPXBone15.Level = iPXBone22.Level;
                    currentState.Bone.Add(iPXBone20);
                    currentState.Bone.Add(iPXBone21);
                    currentState.Bone.Add(iPXBone22);
                    currentState.Bone.Add(iPXBone15);
                    foreach (IPXVertex item14 in list3)
                    {
                        if (IsDominatedVertex(item14, iPXBone18))
                        {
                            item14.Bone1 = iPXBone15;
                            item14.Bone2 = iPXBone18;
                            float num16 = iPXBone15.Position.Z - iPXBone18.Position.Z;
                            float num17 = item14.Position.Z - iPXBone18.Position.Z;
                            float num18 = num17 / num16;
                            num18 = (num18 - 0.75f) / 0.5f;
                            item14.Weight1 = num18;
                            if (item14.Weight1 < 0f)
                            {
                                item14.Weight1 = 0f;
                            }
                            if (item14.Weight1 > 1f)
                            {
                                item14.Weight1 = 1f;
                            }
                            item14.Weight2 = 1f - item14.Weight1;
                        }
                        VertexBoneReplace(item14, iPXBone16, iPXBone20);
                        VertexBoneReplace(item14, iPXBone17, iPXBone21);
                        VertexBoneReplace(item14, iPXBone18, iPXBone22);
                    }
                    FullBodyBoneReplace(currentState.Body, iPXBone16, iPXBone20);
                    FullBodyBoneReplace(currentState.Body, iPXBone17, iPXBone21);
                    FullBodyBoneReplace(currentState.Body, iPXBone18, iPXBone22);
                    list.Add(text);
                    if (form.checkAutoBoneList.Checked)
                    {
                        EntryBoneFrameNext(currentState, iPXBone18, iPXBone15);
                        if (form.checkLegDControll.Checked)
                        {
                            EntryBoneFrameNext(currentState, iPXBone18, iPXBone22);
                            EntryBoneFrameNext(currentState, iPXBone18, iPXBone21);
                            EntryBoneFrameNext(currentState, iPXBone18, iPXBone20);
                        }
                    }
                }
            }
            if (form.checkDummyHandHeld.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string text3;
                    string nameE;
                    string name;
                    if (i == 0)
                    {
                        text = "右ダミー";
                        text3 = "右手首";
                        nameE = "dummy_R";
                        string text2 = "右ダミー先";
                        name = "右中指１";
                    }
                    else
                    {
                        text = "左ダミー";
                        text3 = "左手首";
                        nameE = "dummy_L";
                        string text2 = "左ダミー先";
                        name = "左中指１";
                    }
                    IPXBone iPXBone23 = SearchBone(currentState, text);
                    IPXBone iPXBone24 = SearchBone(currentState, text3);
                    IPXBone iPXBone25 = SearchBone(currentState, name);
                    if (iPXBone23 == null && iPXBone24 != null && iPXBone25 != null)
                    {
                        float3 float9 = new float3(iPXBone24.Position);
                        float3 float10 = new float3(iPXBone25.Position);
                        float10.z = float9.z;
                        float3 float11 = float3.normalize(float10 - float9);
                        float3 float12 = default(float3);
                        if (i == 0)
                        {
                            float12.x = 0f - float11.y;
                            float12.y = float11.x;
                        }
                        else
                        {
                            float12.x = float11.y;
                            float12.y = 0f - float11.x;
                        }
                        float3 float13 = (float9 + float10) * 0.5f;
                        iPXBone23 = args.Host.Builder.Pmx.Bone();
                        iPXBone23.IsRotation = true;
                        iPXBone23.IsTranslation = true;
                        iPXBone23.Name = text;
                        iPXBone23.NameE = nameE;
                        iPXBone23.Parent = iPXBone24;
                        iPXBone23.ToOffset = (float12 * 1.2f).ToV3();
                        iPXBone23.Position = (float13 + float12 * 0.25f).ToV3();
                        EntryBoneNext(currentState, iPXBone23.Parent, iPXBone23);
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameNext(currentState, iPXBone23.Parent, iPXBone23);
                        }
                    }
                }
            }
            if (form.checkShoulderCancel.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string text2;
                    string nameE;
                    string text3;
                    string name;
                    if (i == 0)
                    {
                        text = "右肩P";
                        text2 = "右肩C";
                        nameE = "shoulderP_R";
                        text3 = "右肩";
                        name = "右腕";
                    }
                    else
                    {
                        text = "左肩P";
                        text2 = "左肩C";
                        nameE = "shoulderP_L";
                        text3 = "左肩";
                        name = "左腕";
                    }
                    IPXBone iPXBone26 = SearchBone(currentState, text);
                    IPXBone iPXBone27 = SearchBone(currentState, text3);
                    IPXBone iPXBone28 = SearchBone(currentState, name);
                    if (iPXBone26 == null && iPXBone27 != null && iPXBone28 != null)
                    {
                        iPXBone26 = args.Host.Builder.Pmx.Bone();
                        IPXBone iPXBone29 = args.Host.Builder.Pmx.Bone();
                        iPXBone26.Name = text;
                        iPXBone26.NameE = nameE;
                        iPXBone26.Parent = iPXBone27.Parent;
                        iPXBone26.IsRotation = true;
                        iPXBone26.IsTranslation = false;
                        iPXBone26.Position = iPXBone27.Position.Clone();
                        iPXBone29.Name = text2;
                        iPXBone29.Parent = iPXBone27;
                        iPXBone29.IsAppendRotation = true;
                        iPXBone29.Position = iPXBone28.Position.Clone();
                        iPXBone29.AppendRatio = -1f;
                        iPXBone29.AppendParent = iPXBone26;
                        iPXBone29.Visible = false;
                        iPXBone27.Parent = iPXBone26;
                        iPXBone28.Parent = iPXBone29;
                        EntryBoneFore(currentState, iPXBone27, iPXBone26);
                        EntryBoneNext(currentState, iPXBone27, iPXBone29);
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameFore(currentState, iPXBone27, iPXBone26);
                        }
                    }
                }
            }
            if (form.checkDummy.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text;
                    string nameE;
                    string text3;
                    string name;
                    string name2;
                    string name3;
                    if (i == 0)
                    {
                        text = "右親指０";
                        nameE = "thumb0_R";
                        text3 = "右手首";
                        name = "右親指１";
                        name2 = "右親指２";
                        name3 = "右人指１";
                    }
                    else
                    {
                        text = "左親指０";
                        nameE = "thumb0_L";
                        text3 = "左手首";
                        name = "左親指１";
                        name2 = "左親指２";
                        name3 = "左人指１";
                    }
                    IPXBone iPXBone30 = SearchBone(currentState, text);
                    IPXBone iPXBone31 = SearchBone(currentState, text3);
                    IPXBone iPXBone32 = SearchBone(currentState, name);
                    IPXBone iPXBone33 = SearchBone(currentState, name2);
                    IPXBone iPXBone34 = SearchBone(currentState, name3);
                    if (iPXBone30 == null && iPXBone31 != null && iPXBone32 != null && iPXBone34 != null)
                    {
                        float3 float14 = new float3(iPXBone31.Position);
                        float3 float15 = new float3(iPXBone32.Position);
                        new float3(iPXBone34.Position);
                        float3 float16 = float3.lerp(float14, float15, 2f / 3f);
                        float3.normalize(float15 - float14);
                        iPXBone30 = args.Host.Builder.Pmx.Bone();
                        iPXBone30.Name = text;
                        iPXBone30.NameE = nameE;
                        iPXBone30.IsRotation = true;
                        iPXBone30.IsTranslation = false;
                        iPXBone30.Parent = iPXBone31;
                        iPXBone30.ToBone = iPXBone32;
                        iPXBone30.Position = float16.ToV3();
                        iPXBone32.Parent = iPXBone30;
                        EntryBoneFore(currentState, iPXBone32, iPXBone30);
                        float num19 = float3.length(float15 - float16);
                        float3 vec = ((i != 0) ? new float3(-1f, -1f, 0f) : new float3(1f, -1f, 0f));
                        vec = float3.normalize(vec);
                        foreach (IPXVertex item15 in list3)
                        {
                            if (!VertexBoneHas(item15, iPXBone31) && !VertexBoneHas(item15, iPXBone32))
                            {
                                continue;
                            }
                            float3 float17 = new float3(item15.Position) - (float16 + float15) * 0.5f;
                            float num20 = float3.length(float17 - vec * float3.dot(float17, vec));
                            num20 /= num19 * 1.4f;
                            if (!(num20 < 1f))
                            {
                                continue;
                            }
                            float num21 = (1f - num20) * 1.3f;
                            if (num21 < 0f)
                            {
                                num21 = 0f;
                            }
                            if (num21 > 1f)
                            {
                                num21 = 1f;
                            }
                            if (IsDominatedVertex(item15, iPXBone31))
                            {
                                item15.Bone1 = iPXBone30;
                                item15.Bone2 = iPXBone31;
                                item15.Bone3 = null;
                                item15.Bone4 = null;
                                item15.Weight1 = num21;
                                item15.Weight2 = 1f - num21;
                                item15.Weight3 = 0f;
                                item15.Weight4 = 0f;
                            }
                            else if (IsDominatedVertex(item15, iPXBone32))
                            {
                                item15.Bone1 = iPXBone30;
                                item15.Bone2 = iPXBone32;
                                item15.Bone3 = null;
                                item15.Bone4 = null;
                                item15.Weight1 = num21;
                                item15.Weight2 = 1f - num21;
                                item15.Weight3 = 0f;
                                item15.Weight4 = 0f;
                            }
                            else if (BoneMatch(item15.Bone1, iPXBone31))
                            {
                                item15.Bone1 = iPXBone30;
                                if (item15.Weight1 < num21)
                                {
                                    item15.Weight1 = num21;
                                    float num22 = item15.Weight2 + item15.Weight3 + item15.Weight4;
                                    if (num22 > 0f)
                                    {
                                        item15.Weight2 *= (1f - num21) / num22;
                                        item15.Weight3 *= (1f - num21) / num22;
                                        item15.Weight4 *= (1f - num21) / num22;
                                    }
                                }
                            }
                            else if (BoneMatch(item15.Bone2, iPXBone31))
                            {
                                item15.Bone2 = iPXBone30;
                                if (item15.Weight2 < num21)
                                {
                                    item15.Weight2 = num21;
                                    float num23 = item15.Weight1 + item15.Weight3 + item15.Weight4;
                                    if (num23 > 0f)
                                    {
                                        item15.Weight1 *= (1f - num21) / num23;
                                        item15.Weight3 *= (1f - num21) / num23;
                                        item15.Weight4 *= (1f - num21) / num23;
                                    }
                                }
                            }
                            else if (BoneMatch(item15.Bone3, iPXBone31))
                            {
                                item15.Bone3 = iPXBone30;
                                if (item15.Weight3 < num21)
                                {
                                    item15.Weight3 = num21;
                                    float num24 = item15.Weight1 + item15.Weight2 + item15.Weight4;
                                    if (num24 > 0f)
                                    {
                                        item15.Weight1 *= (1f - num21) / num24;
                                        item15.Weight2 *= (1f - num21) / num24;
                                        item15.Weight4 *= (1f - num21) / num24;
                                    }
                                }
                            }
                            else
                            {
                                if (!BoneMatch(item15.Bone4, iPXBone31))
                                {
                                    continue;
                                }
                                item15.Bone4 = iPXBone30;
                                if (item15.Weight4 < num21)
                                {
                                    item15.Weight4 = num21;
                                    float num25 = item15.Weight1 + item15.Weight2 + item15.Weight3;
                                    if (num25 > 0f)
                                    {
                                        item15.Weight1 *= (1f - num21) / num25;
                                        item15.Weight2 *= (1f - num21) / num25;
                                        item15.Weight3 *= (1f - num21) / num25;
                                    }
                                }
                            }
                        }
                        list.Add(text);
                        if (form.checkAutoBoneList.Checked)
                        {
                            EntryBoneFrameFore(currentState, iPXBone32, iPXBone30);
                        }
                    }
                    if (form.checkThumbLocalAxis.Checked && iPXBone30 != null && iPXBone31 != null && iPXBone32 != null && iPXBone33 != null && iPXBone34 != null)
                    {
                        float3 float18 = new float3(iPXBone30.Position);
                        new float3(iPXBone31.Position);
                        float3 float19 = new float3(iPXBone32.Position);
                        float3 float20 = new float3(iPXBone33.Position);
                        float3 float21 = new float3(iPXBone34.Position);
                        float3 float22 = float20 - float19;
                        float3 vec2 = default(float3);
                        if (i == 0)
                        {
                            vec2.x = 0f - float22.y;
                            vec2.y = float22.x;
                        }
                        else
                        {
                            vec2.x = float22.y;
                            vec2.y = 0f - float22.x;
                        }
                        vec2.z = (0f - float3.length(vec2)) * 0.2f;
                        float3 float23 = float3.normalize(float19 - float18);
                        float3 float24 = float3.normalize(float18 - float21);
                        iPXBone30.SetLocalAxis(float23.ToV3(), float24.ToV3());
                        iPXBone30.IsLocalFrame = true;
                        float23 = float3.normalize(float20 - float19);
                        float24 = float3.normalize(float19 - float21);
                        iPXBone32.SetLocalAxis(float23.ToV3(), vec2.ToV3());
                        iPXBone32.IsLocalFrame = true;
                        float3 float25 = ((iPXBone33.ToBone == null) ? new float3(iPXBone33.Position + iPXBone33.ToOffset) : new float3(iPXBone33.ToBone.Position));
                        float23 = float3.normalize(float25 - float20);
                        float24 = float3.normalize(float20 - float21);
                        iPXBone33.SetLocalAxis(float23.ToV3(), vec2.ToV3());
                        iPXBone33.IsLocalFrame = true;
                    }
                }
            }
            if (form.checkAllParent.Checked)
            {
                string text = "全ての親";
                string nameE = "master";
                if (SearchBone(currentState, text) == null)
                {
                    IPXBone iPXBone35 = currentState.Bone[0];
                    IPXBone iPXBone36 = args.Host.Builder.Pmx.Bone();
                    iPXBone36.IsRotation = true;
                    iPXBone36.IsTranslation = true;
                    iPXBone36.Name = text;
                    iPXBone36.Position = new V3();
                    iPXBone36.Parent = null;
                    iPXBone36.ToBone = iPXBone35;
                    iPXBone36.NameE = nameE;
                    foreach (IPXBone item16 in currentState.Bone)
                    {
                        if (item16.Parent == null)
                        {
                            item16.Parent = iPXBone36;
                        }
                        if (item16.ToBone != null && item16.ToBone.Equals(currentState.Bone[0]))
                        {
                            item16.ToBone = iPXBone36;
                        }
                    }
                    currentState.Bone.Insert(0, iPXBone36);
                    list.Add(text);
                    if (iPXBone35 != null)
                    {
                        if (form.checkAutoBoneList.Checked)
                        {
                            int j = 0;
                            foreach (IPXNode item17 in currentState.Node)
                            {
                                for (j = 0; j < item17.Items.Count; j++)
                                {
                                    if (item17.Items[j].Equals(iPXBone35))
                                    {
                                        j = -1;
                                        break;
                                    }
                                }
                                if (j < 0)
                                {
                                    break;
                                }
                            }
                            if (j >= 0)
                            {
                                IPXNode frameBone3 = GetFrameBone(currentState, "センター", true);
                                frameBone3.Items.Insert(0, args.Host.Builder.Pmx.BoneNodeItem(iPXBone35));
                                frameBone3.NameE = "center";
                            }
                        }
                        SetRootBone(currentState, iPXBone36);
                    }
                }
            }
            if (form.checkOperationCenter.Checked)
            {
                string text = "操作中心";
                string nameE = "view cnt";
                if (SearchBone(currentState, text) == null)
                {
                    IPXBone iPXBone37 = currentState.Bone[0];
                    IPXBone iPXBone38 = args.Host.Builder.Pmx.Bone();
                    iPXBone38.IsRotation = true;
                    iPXBone38.IsTranslation = true;
                    iPXBone38.Name = text;
                    iPXBone38.Position = new V3();
                    iPXBone38.Parent = null;
                    iPXBone38.NameE = nameE;
                    foreach (IPXBone item18 in currentState.Bone)
                    {
                        if (item18.ToBone != null && item18.ToBone.Equals(currentState.Bone[0]))
                        {
                            item18.ToBone = iPXBone38;
                        }
                    }
                    currentState.Bone.Insert(0, iPXBone38);
                    list.Add(text);
                    if (form.checkAutoBoneList.Checked)
                    {
                        int k = 0;
                        foreach (IPXNode item19 in currentState.Node)
                        {
                            for (k = 0; k < item19.Items.Count; k++)
                            {
                                if (item19.Items[k].Equals(iPXBone37))
                                {
                                    k = -1;
                                    break;
                                }
                            }
                            if (k < 0)
                            {
                                break;
                            }
                        }
                        if (k >= 0)
                        {
                            IPXNode frameBone4 = GetFrameBone(currentState, "センター", true);
                            frameBone4.Items.Insert(0, args.Host.Builder.Pmx.BoneNodeItem(iPXBone37));
                            frameBone4.NameE = "center";
                        }
                        SetRootBone(currentState, iPXBone38);
                    }
                }
            }
            string text6 = "以下のボーンを追加しました。" + Environment.NewLine;
            foreach (string item20 in list)
            {
                text6 = text6 + item20 + Environment.NewLine;
            }
            if (list.Count <= 0)
            {
                text6 = "追加したボーンはありませんでした。";
            }
            MessageBox.Show(text6, Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            args.Host.Connector.Pmx.Update(currentState);
            args.Host.Connector.Form.UpdateList(UpdateObject.All);
            args.Host.Connector.View.PMDView.UpdateModel();
            args.Host.Connector.View.PMDView.UpdateView();
            args.Host.Connector.View.TransformView.UpdateView();
        }



        private IPEXBone SearchBone(IPEXPmd pmd, string Name)
		{
			foreach (IPEXBone item in pmd.Bone)
			{
				if (item.Name == Name)
				{
					return item;
				}
			}
			return null;
		}

		private IPEXFrameBone GetFrameBone(IPEXPmd pmd, string BFName, bool InsertFirst)
		{
			foreach (IPEXFrameBone item in pmd.FrameBone)
			{
				if (item.Name == BFName)
				{
					return item;
				}
			}
			IPEXFrameBone iPEXFrameBone = PEStaticBuilder.Builder.CreateXFrameBone();
			iPEXFrameBone.Name = BFName;
			if (InsertFirst)
			{
				pmd.FrameBone.Insert(0, iPEXFrameBone);
			}
			else
			{
				pmd.FrameBone.Add(iPEXFrameBone);
			}
			return iPEXFrameBone;
		}

		private bool EntryBoneFrameNext(IPEXPmd pmd, IPEXBone Bone0, IPEXBone EntryBone)
		{
			foreach (IPEXFrameBone item in pmd.FrameBone)
			{
				for (int i = 0; i < item.Bones.Count; i++)
				{
					if (item.Bones[i].Equals(Bone0))
					{
						item.Bones.Insert(i + 1, EntryBone);
						return true;
					}
				}
			}
			return false;
		}

		private bool EntryBoneFrameFore(IPEXPmd pmd, IPEXBone Bone0, IPEXBone EntryBone)
		{
			foreach (IPEXFrameBone item in pmd.FrameBone)
			{
				for (int i = 0; i < item.Bones.Count; i++)
				{
					if (item.Bones[i].Equals(Bone0))
					{
						item.Bones.Insert(i, EntryBone);
						return true;
					}
				}
			}
			return false;
		}

		private bool EntryBoneNext(IPEXPmd pmd, IPEXBone Bone0, IPEXBone EntryBone)
		{
			for (int i = 0; i < pmd.Bone.Count; i++)
			{
				if (pmd.Bone[i].Equals(Bone0))
				{
					pmd.Bone.Insert(i + 1, EntryBone);
					return true;
				}
			}
			return false;
		}

		private bool EntryBoneFore(IPEXPmd pmd, IPEXBone Bone0, IPEXBone EntryBone)
		{
			for (int i = 0; i < pmd.Bone.Count; i++)
			{
				if (pmd.Bone[i].Equals(Bone0))
				{
					pmd.Bone.Insert(i, EntryBone);
					return true;
				}
			}
			return false;
		}

		private bool IsDominatedVertex(IPEXVertex vtx, IPEXBone bone)
		{
			return IsDominatedVertex(vtx, bone, 0.97f);
		}

		private bool IsDominatedVertex(IPEXVertex vtx, IPEXBone bone, float Threshold)
		{
			if ((vtx.Bone1.Equals(bone) && (float)vtx.Weight / 100f >= Threshold) || (vtx.Bone2.Equals(bone) && (float)(100 - vtx.Weight) / 100f >= Threshold))
			{
				return true;
			}
			return false;
		}

		private void VertexBoneReplace(IPEXVertex vtx, IPEXBone bone_old, IPEXBone bone_new)
		{
			if (vtx.Bone1.Equals(bone_old))
			{
				vtx.Bone1 = bone_new;
			}
			if (vtx.Bone2 != null && vtx.Bone2.Equals(bone_old))
			{
				vtx.Bone2 = bone_new;
			}
		}

		private bool VertexBoneHas(IPEXVertex vtx, IPEXBone bone)
		{
			if (vtx.Bone1.Equals(bone))
			{
				return true;
			}
			if (vtx.Bone2 == null)
			{
				return false;
			}
			if (vtx.Bone2.Equals(bone))
			{
				return true;
			}
			return false;
		}

        private IPXBone SearchBone(IPXPmx pmx, string Name)
        {
            foreach (IPXBone item in pmx.Bone)
            {
                if (item.Name == Name)
                {
                    return item;
                }
            }
            return null;
        }

        private IPXNode GetFrameBone(IPXPmx pmx, string BFName, bool InsertFirst)
        {
            foreach (IPXNode item in pmx.Node)
            {
                if (item.Name == BFName)
                {
                    return item;
                }
            }
            IPXNode iPXNode = PEStaticBuilder.Builder.Pmx.Node();
            iPXNode.Name = BFName;
            if (InsertFirst)
            {
                pmx.Node.Insert(0, iPXNode);
            }
            else
            {
                pmx.Node.Add(iPXNode);
            }
            return iPXNode;
        }

        private void SetRootBone(IPXPmx pmx, IPXBone Bone)
        {
            pmx.RootNode.Items.Clear();
            pmx.RootNode.Items.Add(PEStaticBuilder.Builder.Pmx.BoneNodeItem(Bone));
        }

        private bool EntryBoneFrameNext(IPXPmx pmx, IPXBone Bone0, IPXBone EntryBone)
        {
            foreach (IPXNode item2 in pmx.Node)
            {
                for (int i = 0; i < item2.Items.Count; i++)
                {
                    if (item2.Items[i].IsBone && item2.Items[i].BoneItem.Bone.Equals(Bone0))
                    {
                        IPXNodeItem item = PEStaticBuilder.Builder.Pmx.BoneNodeItem(EntryBone);
                        item2.Items.Insert(i + 1, item);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EntryBoneFrameFore(IPXPmx pmx, IPXBone Bone0, IPXBone EntryBone)
        {
            foreach (IPXNode item2 in pmx.Node)
            {
                for (int i = 0; i < item2.Items.Count; i++)
                {
                    if (item2.Items[i].IsBone && item2.Items[i].BoneItem.Bone.Equals(Bone0))
                    {
                        IPXNodeItem item = PEStaticBuilder.Builder.Pmx.BoneNodeItem(EntryBone);
                        item2.Items.Insert(i, item);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EntryBoneNext(IPXPmx pmx, IPXBone Bone0, IPXBone EntryBone)
        {
            for (int i = 0; i < pmx.Bone.Count; i++)
            {
                if (pmx.Bone[i].Equals(Bone0))
                {
                    pmx.Bone.Insert(i + 1, EntryBone);
                    return true;
                }
            }
            return false;
        }

        private bool EntryBoneFore(IPXPmx pmx, IPXBone Bone0, IPXBone EntryBone)
        {
            for (int i = 0; i < pmx.Bone.Count; i++)
            {
                if (pmx.Bone[i].Equals(Bone0))
                {
                    pmx.Bone.Insert(i, EntryBone);
                    return true;
                }
            }
            return false;
        }

        private bool IsDominatedVertex(IPXVertex vtx, IPXBone bone)
        {
            return IsDominatedVertex(vtx, bone, 0.97f);
        }

        private bool IsDominatedVertex(IPXVertex vtx, IPXBone bone, float Threshold)
        {
            float num = 0f;
            float num2 = 0f;
            if (vtx.Bone1 != null)
            {
                if (vtx.Bone1.Equals(bone))
                {
                    num += vtx.Weight1;
                }
                num2 += vtx.Weight1;
            }
            if (vtx.Bone2 != null)
            {
                if (vtx.Bone2.Equals(bone))
                {
                    num += vtx.Weight2;
                }
                num2 += vtx.Weight2;
            }
            if (vtx.Bone3 != null)
            {
                if (vtx.Bone3.Equals(bone))
                {
                    num += vtx.Weight3;
                }
                num2 += vtx.Weight3;
            }
            if (vtx.Bone4 != null)
            {
                if (vtx.Bone4.Equals(bone))
                {
                    num += vtx.Weight4;
                }
                num2 += vtx.Weight4;
            }
            if (num / num2 > Threshold)
            {
                return true;
            }
            return false;
        }

        private void VertexWaitSort(IPXVertex vtx)
        {
            if (vtx.Weight4 > vtx.Weight3)
            {
                float weight = vtx.Weight3;
                IPXBone bone = vtx.Bone3;
                vtx.Weight3 = vtx.Weight4;
                vtx.Bone3 = vtx.Bone4;
                vtx.Weight4 = weight;
                vtx.Bone4 = bone;
            }
            if (vtx.Weight3 > vtx.Weight2)
            {
                float weight = vtx.Weight2;
                IPXBone bone = vtx.Bone2;
                vtx.Weight2 = vtx.Weight3;
                vtx.Bone2 = vtx.Bone3;
                vtx.Weight3 = weight;
                vtx.Bone3 = bone;
            }
            if (vtx.Weight2 > vtx.Weight1)
            {
                float weight = vtx.Weight1;
                IPXBone bone = vtx.Bone1;
                vtx.Weight2 = vtx.Weight2;
                vtx.Bone2 = vtx.Bone2;
                vtx.Weight2 = weight;
                vtx.Bone2 = bone;
            }
        }

        private void VertexBoneReplace(IPXVertex vtx, IPXBone bone_old, IPXBone bone_new)
        {
            if (vtx.Bone1.Equals(bone_old))
            {
                vtx.Bone1 = bone_new;
            }
            if (vtx.Bone2 == null)
            {
                return;
            }
            if (vtx.Bone2.Equals(bone_old))
            {
                vtx.Bone2 = bone_new;
            }
            if (vtx.Bone3 != null)
            {
                if (vtx.Bone3.Equals(bone_old))
                {
                    vtx.Bone3 = bone_new;
                }
                if (vtx.Bone4 != null && vtx.Bone4.Equals(bone_old))
                {
                    vtx.Bone4 = bone_new;
                }
            }
        }

        private bool VertexBoneHas(IPXVertex vtx, IPXBone bone)
        {
            if (vtx.Bone1.Equals(bone))
            {
                return true;
            }
            if (vtx.Bone2 == null)
            {
                return false;
            }
            if (vtx.Bone2.Equals(bone))
            {
                return true;
            }
            if (vtx.Bone3 == null)
            {
                return false;
            }
            if (vtx.Bone3.Equals(bone))
            {
                return true;
            }
            if (vtx.Bone4 == null)
            {
                return false;
            }
            if (vtx.Bone4.Equals(bone))
            {
                return true;
            }
            return false;
        }

        private bool BoneMatch(IPXBone bone1, IPXBone bone2)
        {
            if (bone1 == null)
            {
                return false;
            }
            if (bone2 == null)
            {
                return false;
            }
            return bone1.Equals(bone2);
        }

        private void BodyBoneReplace(IPXBody body, IPXBone bone_old, IPXBone bone_new)
        {
            if (body.Bone.Equals(bone_old))
            {
                body.Bone = bone_new;
            }
        }

        private int FullBodyBoneReplace(IList<IPXBody> Bodys, IPXBone bone_old, IPXBone bone_new)
        {
            int num = 0;
            foreach (IPXBody Body in Bodys)
            {
                if (Body.Bone != null && Body.Bone.Equals(bone_old))
                {
                    Body.Bone = bone_new;
                    num++;
                }
            }
            return num;
        }

        public void Dispose()
		{
		}
	}
}
