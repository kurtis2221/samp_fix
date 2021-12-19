using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace samp_fix
{
    public partial class Form1 : Form
    {
        [Flags]
        enum SAFlags
        {
            Zero = 0,
            Sun = 1,
            Grass = 1 << 1,
            Shadows = 1 << 2,
            Blur = 1 << 3,
            ZoneName = 1 << 4,
            CarName = 1 << 5
        }

        SAFlags check_flag;

        const string filename = "samp_fix.ini";

        MemoryEdit.Memory mem;
        KeyHook.GlobalKeyboardHook gkh = new KeyHook.GlobalKeyboardHook();

        CheckBox[] fix_flags;

        int[] mem_length = new int[]
        {
            1,
            1,
            2,
            1,
            3,
            1
        };

        int tmp = 0;

        uint[] MemoryAddresses = new uint[]
        {
            0x0053C136, //Sun Flare
            0x0053C159, //Grass
            0x0053EA08, //Shadow
            0x0053EA0D,
            0x00704E8A, //Blur
            0x005720A5, //Zone Names
            0x00590099, //Int Zones
            0x005952A6,
            0x0058FBE9 //Car Names
        };

        long[] OriginalCode = new long[]
        {
            0x001C0465E8,
            0x000A0E42E8,
            0x00C40350B9,
            0x001C809EE8,
            0xFFFFE211E8,
            0xFFFFFE76E8,
            0x00197AC2E8,
            0xFFFFCF85E8,
            0xFFFFB2B2E8
        };

        const long NOP = 0x9090909090;

        FileStream fs;
        StreamWriter sw;
        StreamReader sr;
        System.Media.SoundPlayer snd = new System.Media.SoundPlayer();

        char[] cright = new char[] {'s','u','K','i','r','t'};

        public Form1()
        {
            InitializeComponent();
            fix_flags = new CheckBox[]
            {
                checkBox1, checkBox2, checkBox3,
                checkBox4, checkBox5, checkBox6
            };
            if (File.Exists(filename))
            {
                sr = new StreamReader(filename);
                check_flag = 0;
                int.TryParse(sr.ReadToEnd(), out tmp);
                check_flag += tmp;
                tmp = 0;
                sr.Close();
                for (int i = 0; i < fix_flags.Length; i++)
                {
                    tmp = 1 << i;
                    if ((check_flag & (SAFlags)tmp) == (SAFlags)tmp)
                        fix_flags[i].Checked = true;
                }
                tmp = 0;
            }
            gkh.Hook();
            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
        }

        private void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                if (MemoryEdit.Memory.IsProcessOpen("gta_sa"))
                {
                    mem = new MemoryEdit.Memory("gta_sa", 0x001F0FFF);
                    DoChanges();
                    snd.Stream = Properties.Resources.success;
                    snd.Play();
                    System.Threading.Thread.Sleep(1000);
                    Application.Exit();
                }
                else
                {
                    snd.Stream = Properties.Resources.error;
                    snd.Play();
                }
            }
        }

        private void DoChanges()
        {
            int i, i2;
            tmp = 0;
            for (i = 0; i < fix_flags.Length; i++)
            {
                if (fix_flags[i].Checked)
                {
                    for (i2 = 0; i2 < mem_length[i]; i2++)
                    {
                        mem.WriteByte(MemoryAddresses[tmp], BitConverter.GetBytes(OriginalCode[tmp]), 5);
                        tmp += 1;
                    }
                }
                else
                {
                    for (i2 = 0; i2 < mem_length[i]; i2++)
                    {
                        mem.WriteByte(MemoryAddresses[tmp], BitConverter.GetBytes(NOP), 5);
                        tmp += 1;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program written by " +
                cright[2] + cright[1] + cright[4] + cright[5] + cright[3] + cright[0] +
                " (2013)\n" +
                "HU\n==\n\n" +
                "Használat:\n1.Jelöld be amire szükséged van\n2.Indítsd el a játékot\n" +
                "3.Játék közben nyomd meg az F12-t\n\n" +
                "EN\n==\n" +
                "Useage:\n1.Check those things you want\n2.Start the game\n" +
                "3.In-game press F12\n\n" +
                "Written in C# 2008 Express Edition (.Net 3.5)",
                "About SA-MP Fixer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fs = new FileStream(filename, File.Exists(filename) ? FileMode.Truncate : FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fs);
            check_flag = 0;
            for (int i = 0; i < fix_flags.Length; i++)
                if (fix_flags[i].Checked) check_flag += 1 << i;
            sw.Write((int)check_flag);
            sw.Close();
            fs.Close();
        }
    }
}