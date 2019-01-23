﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPC_750CL_ASM_DASM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int p;

        private void button1_Click(object sender, EventArgs e)
        {
            ASM.Text = "";
            List<UInt32> m = new List<UInt32>();
            String[] MLCodesString = MachineLanguage.Text.Split(' ');
            for (int i = 0; i < MLCodesString.Length; i++)
            {
                m.Add(Convert.ToUInt32(MLCodesString[i], 16));
                p = 0;
                String Output = "";
                UInt32 PO = ReadBits(m[i], 6); // Primary Operation identifier (first 6 bits)
                if (PO == 31)
                // add (add, add., addo, addo.), addc (addc, addc. addco, addco.), adde (adde, adde., addeo, addeo.),
                // addme (addme, addme., addmeo, addmeo.). addze (addze, addze., addzeo, addzeo.), and (and, and.), andc (andc, andc.),
                // cmp, cmpl, cntlzw (cntlzw, cntlzw.), dcbf, dcbi, dcbst, dcbt, dcbtst, divw, divwu, eciwx, ecowx, eieio, eqv (eqv, eqv.)
                // extsb, extsh
                {
                    UInt32 rD = ReadBits(m[i], 5); // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eieio; rS on ecowx, eqv, extsb, extsh
                    UInt32 rA = ReadBits(m[i], 5); // Reserved on eieio
                    UInt32 rB = ReadBits(m[i], 5); // Reserved on eieio, extsb, extsh
                    UInt32 OE = ReadBits(m[i], 1);
                    UInt32 SO = ReadBits(m[i], 9);
                    UInt32 Rc = ReadBits(m[i], 1); // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eciwx, ecowx, eieio
                    String CS = " ";
                    if (SO == 0 || SO == 32) // cmp, cmpl
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        p++; // Reserved
                        UInt32 L = ReadBits(m[i], 1);
                        rA = ReadBits(m[i], 5);
                        rB = ReadBits(m[i], 5);
                        CS = " " + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        Output = "cmp";
                        if (SO == 32) // cmpl
                        {
                            Output += "l";
                        }

                    }
                    else if (SO == 284) // eqv
                    {
                        p = 16;
                        rB = ReadBits(m[i], 6);
                        Output = "eqv";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                        if (Rc == 1) // .
                        {
                            Output += ".";
                        }
                    }
                    else
                    {
                        CS += "r" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        if (SO == 266) // add
                        {
                            Output = "add";
                        }
                        else if (SO == 10) // addc
                        {
                            Output = "addc";
                        }
                        else if (SO == 138) // adde
                        {
                            Output = "adde";
                        }
                        else if (SO == 234) // addme
                        {
                            Output = "addme";
                            CS = " r" + rD.ToString() + ", r" + rA.ToString();
                        }
                        else if (SO == 202) // addze
                        {
                            Output = "addze";
                            CS = " r" + rD.ToString() + ", r" + rA.ToString();
                        }
                        else if (SO == 28) // and
                        {
                            Output = "and";
                            CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 29) // andc
                        {
                            Output = "andc";
                            CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 26) // cntlzw
                        {
                            Output = "cntlzw";
                            CS = " r" + rA.ToString() + ", r" + rD.ToString();
                        }
                        else if (SO == 86) // dcbf
                        {
                            Output = "dcbf";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 470) // dcbi
                        {
                            Output = "dcbi";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 54) // dcbst
                        {
                            Output = "dcbst";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 278) // dcbt
                        {
                            Output = "dcbt";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 246) // dcbtst
                        {
                            Output = "dcbtst";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 502) // dcbz (Actually 1014, but the previous bit (1) is from OE)
                        {
                            Output = "dcbz";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else if (SO == 491) // divw
                        {
                            Output = "divw";
                        }
                        else if (SO == 459) // divwu
                        {
                            Output = "divwu";
                        }
                        else if (SO == 310) // eciwx
                        {
                            Output = "eciwx";
                        }
                        else if (SO == 438) // ecowx
                        {
                            Output = "ecowx";
                        }
                        else if (SO == 342) // eieio (Actually 854, but the previous bit (1) is from OE)
                        {
                            Output = "eieio";
                            CS = "";
                        }
                        else if (SO == 442) // extsb (Actually 954, but the previous bit (1) is from OE)
                        {
                            Output = "extsb";
                            CS = " r" + rA.ToString() + ", r" + rD.ToString();
                        }
                        else if (SO == 410) // extsh (Actually 922, but the previous bit (1) is from OE)
                        {
                            Output = "extsh";
                            CS = " r" + rA.ToString() + ", r" + rD.ToString();
                        }
                        else
                        {
                            NotInstruction();
                        }
                        if (OE == 1 && SO != 502 && SO != 342 && SO != 442 && SO != 410) // o
                        {
                            Output += "o";
                        }
                        if (Rc == 1) // .
                        {
                            Output += ".";
                        }
                    }
                    Output += CS;
                }
                else if (PO == 14) // addi
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "addi r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 12) // addic
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "addic r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 13) // addic.
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "addic. r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 15) // addis
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "addis r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 28) // andi.
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "andi. r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else if (PO == 29) // andis.
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "andis. r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else if (PO == 18) // b (b, ba, bl, bla)
                {
                    UInt32 LI = ReadBits(m[i], 24);
                    UInt32 AA = ReadBits(m[i], 1);
                    UInt32 LK = ReadBits(m[i], 1);
                    String CS = " " + LI.ToString();
                    Output = "b";
                    if (LK == 1)
                    {
                        Output += "l";
                    }
                    if (AA == 1)
                    {
                        Output += "a";
                    }
                    Output += CS;
                }
                else if (PO == 16) // bc (bc, bca, bcl, bcla)
                {
                    UInt32 BO = ReadBits(m[i], 5);
                    UInt32 BI = ReadBits(m[i], 5);
                    UInt32 BD = ReadBits(m[i], 14);
                    UInt32 AA = ReadBits(m[i], 1);
                    UInt32 LK = ReadBits(m[i], 1);
                    String CS = " " + BO.ToString() + ", " + BI.ToString() + " ," + BD.ToString();
                    Output = "bc";
                    if (LK == 1)
                    {
                        Output += "l";
                    }
                    if (AA == 1)
                    {
                        Output += "a";
                    }
                    Output += CS;
                }
                else if (PO == 19) // bcctr, bclr, crand, crandc, creqv, crnand, crnor, cror, crorc, crxor
                {
                    UInt32 BO = ReadBits(m[i], 5); // crbD on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor
                    UInt32 BI = ReadBits(m[i], 5); // crbA on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor
                    UInt32 crbB = ReadBits(m[i], 5); // Reserved on bcctr, bclr
                    UInt32 SO = ReadBits(m[i], 10);
                    UInt32 LK = ReadBits(m[i], 1); // Reserved on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor
                    String CS = " crb" + BO.ToString() + ", crb" + BI.ToString() + ", crb" + crbB.ToString();
                    if (SO == 528) // bcctr
                    {
                        Output = "bcctr";
                        CS = " " + BO.ToString() + ", " + BI.ToString();
                    }
                    else if (SO == 16) // bclr
                    {
                        Output = "bclr";
                        CS = " " + BO.ToString() + ", " + BI.ToString();
                    }
                    else if (SO == 257) // crand
                    {
                        Output = "crand";
                    }
                    else if (SO == 129) // crandc
                    {
                        Output = "crandc";
                    }
                    else if (SO == 289) // creqv
                    {
                        Output = "creqv";
                    }
                    else if (SO == 225) // crnand
                    {
                        Output = "crnand";
                    }
                    else if (SO == 33) // crnor
                    {
                        Output = "crnor";
                    }
                    else if (SO == 449) // cror
                    {
                        Output = "cror";
                    }
                    else if (SO == 417) // crorc
                    {
                        Output = "crorc";
                    }
                    else if (SO == 193) // crxor
                    {
                        Output = "crxor";
                    }
                    else
                    {
                        NotInstruction();
                    }
                    if (LK == 1)
                    {
                        Output += "l";
                    }
                    Output += CS;
                }
                else if (PO == 11) // cmpi
                {
                    UInt32 crfD = ReadBits(m[i], 3);
                    p++; // Reserved
                    UInt32 L = ReadBits(m[i], 1);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "cmpi " + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", " + SIMM;
                }
                else if (PO == 10) // cmpli
                {
                    UInt32 crfD = ReadBits(m[i], 3);
                    p++; // Reserved
                    UInt32 L = ReadBits(m[i], 1);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "cmpli " + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", " + UIMM;
                }

                ASM.Text += Output + "\n";
            }
        }

        private UInt32 ReadBits(UInt32 i, int c)
        {
            UInt32 result = (i << p) >> (32 - c);
            p += c;
            return result;
        }

        private void NotInstruction()
        {

        }
    }
}