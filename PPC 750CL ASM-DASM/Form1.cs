using System;
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
                    // extsb, extsh, icbi, lbzux, lbzx, lfdux, lfdx, lfsux, lfsx, lhaux, lhax, lhbrx, lzhux, lzhx, lswi, lswx, lwarx,
                    // lwbrx, lwzux, lwzx, mcrxr, mfcr, mfmsr, mfspr, mfsr, mfsrin, mftb, mtcrf, mtmsr, mtspr, mtsr, mtsrin, mulhw (mulhw, mulhw.),
                    // mulhwu (mulhwu, mulhwu.), mullw (mullw, mullw., mullwo, mullwo.), nand (nand, nand.), neg (neg, neg., nego, nego.),
                    // nor (nor, nor.), or (or, or.), orc (orc, orc.), slw (slw, slw.), sraw (sraw, sraw.), srawi (srawi, srawi.),
                    // srw (srw, srw.), stbux, stbx, stfdux, stfdx, stfiwx, stfsux, stfsx, sthbrx, sthux, sthx, stswi, stswx, stwbrx,
                    // stwcx., stwux, stwx, subf (subf, subf., subfo, subfo.), subfc (subfc, subfc., subfco, subfco.),
                    // subfe (subfe, subfe., subfeo. subfeo.), subfme (subfme, subfme., subfmeo, subfmeo.), subfze (subfze, subfze., subfzeo, subfzeo.),
                    // sync, tlbie, tlbsync, tw, xor (xor, xor.)
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eieio, sync, tlbie, tlbsync;
                    // rS on and, andc, cntlzw, ecowx, eqv, extsb, extsh, icbi, mtcrf, mtmsr, mtspr, mtsr, mtsrin, nand, nor, or,
                    // orc, slw, sraw, srawi, srw, stbux, stbx, sthbrx, sthux, sthx, stswi, stswx, stwbrx, stwcx., stwux, stwx, xor;
                    // frD on lfdux, lfdx, lfsux, lfsx; frS on stfdux, stfdx, stfiwx, stfsux, stfsx;
                    // TO on tw
                    UInt32 rA = ReadBits(m[i], 5); // Reserved on eieio, mfcr, mfmsr, mfsrin, mtmsr, mtsrin, sync, tlbie, tlbsync
                    UInt32 rB = ReadBits(m[i], 5);
                    // Reserved on addme, addze, cntlzw, eieio, extsb, extsh, mfcr, neg, subfme, subfze, sync, tlbsync;
                    // NB on lswi, mfmsr, mtmsr, stswi
                    UInt32 OE = ReadBits(m[i], 1); // Reserved on mulhw, mulhwu
                    UInt32 SO = ReadBits(m[i], 9);
                    UInt32 Rc = ReadBits(m[i], 1);
                    // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eciwx, ecowx, eieio, lbzux, lbzx, lfdux, lfsx, lhaux, lhax,
                    // lhbrx, lzhux, lzhx, lswi, lswx, lwarx, lwbrx, lwzux, lwzx, mfcr, mtmsr, stbux, stbx, stfdux, stfdx, stfiwx,
                    // stfsux, stfsx, sthbrx, sthux, sthx, stswi, stswx, stwbrx, stwux, stwx, tlbie, tlbsync, tw
                    String CS = " ";
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
                    else if (SO == 0 || SO == 32) // cmp, cmpl
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        p++; // Reserved
                        UInt32 L = ReadBits(m[i], 1);
                        rA = ReadBits(m[i], 5);
                        rB = ReadBits(m[i], 5);
                        CS = " crf" + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        Output = "cmp";
                        if (SO == 32) // cmpl
                        {
                            Output += "l";
                        }
                    }
                    else if (SO == 26) // cntlzw
                    {
                        Output = "cntlzw";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString();
                    }
                    else if (SO == 86) // dcbf, sync (For sync: actually 598, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "dcbf";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "sync";
                            CS = "";
                        }
                    }
                    else if (SO == 470) // dcbi
                    {
                        Output = "dcbi";
                        CS = " r" + rA.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 54) // dcbst, tlbsync (For tlbsync: actually 566, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "dcbst";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "tlbsync";
                            CS = "";
                        }
                    }
                    else if (SO == 278) // dcbt, lhbrx (For lhbrx: actually 790, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "lhbrx";
                        }
                        else
                        {
                            Output = "dcbt";
                            CS = " r" + rA.ToString() + ", r" + rB.ToString();
                        }
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
                    else if (SO == 284) // eqv
                    {
                        p = 16;
                        rB = ReadBits(m[i], 6);
                        Output = "eqv";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
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
                    else if (SO == 470) // icbi (Actually 982, but the previous bit (1) is from OE)
                    {
                        Output = "icbi";
                        CS = " r" + rA.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 119) // lbzux, lfdux (For lfdux: actually 631, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "lfdux";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "lbzux";
                        }
                    }
                    else if (SO == 87) // lbzx, lfdx (For lfdx: actually 599, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "lfdx";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "lbzx";
                        }
                    }
                    else if (SO == 55) // lfsux, lwzux (For lfsux: actually 567, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "lfsux";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "lwzux";
                        }
                    }
                    else if (SO == 23) // lfsx, lwzx (For lfsx: actually 535, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "lfsx";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "lwzx";
                        }
                    }
                    else if (SO == 375) // lhaux
                    {
                        Output = "lhaux";
                    }
                    else if (SO == 343) // lhax
                    {
                        Output = "lhax";
                    }
                    else if (SO == 311) // lhzux
                    {
                        Output = "lhzux";
                    }
                    else if (SO == 279) // lzhx
                    {
                        Output = "lzhx";
                    }
                    else if (SO == 85) // lswi (Actually 597, but the previous bit (1) is from OE)
                    {
                        Output = "lswi";
                    }
                    else if (SO == 21) // lswx (Actually 533, but the previous bit (1) is from OE)
                    {
                        Output = "lswx";
                    }
                    else if (SO == 20) // lwarx
                    {
                        Output = "lwarx";
                    }
                    else if (SO == 22) // lwbrx (Actually 534, but the previous bit (1) is from OE)
                    {
                        Output = "lwbrx";
                    }
                    else if (SO == 0) // mcrxr (Actually 512, but the previous bit (1) is from OE)
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        Output = "mcrxr";
                        CS = " crf" + crfD.ToString();
                    }
                    else if (SO == 19) // mfcr
                    {
                        Output = "mfrcr";
                        CS = " r" + rD.ToString();
                    }
                    else if (SO == 83) // mfmsr, mfsr (For mfsr: actually 595, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "mfmsr";
                            CS = " r" + rD.ToString();
                        }
                        else
                        {
                            p = 12;
                            UInt32 SR = ReadBits(m[i], 4);
                            Output = "mfsr";
                            CS = " r" + rD.ToString() + ", " + SR.ToString();
                        }
                    }
                    else if (SO == 339) // mfspr
                    {
                        p = 11;
                        UInt32 spr = ReadBits(m[i], 10);
                        Output = "mfspr";
                        CS = " r" + rD.ToString() + ", " + spr.ToString(); 
                    }
                    else if (SO == 147) // mfsrin (Actually 659, but the previous bit (1) is from OE)
                    {
                        Output = "mfsrin";
                        CS = " r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 371) // mftb
                    {
                        p = 11;
                        UInt32 tbr = ReadBits(m[i], 10);
                        Output = "mftb";
                        CS = " r" + rD.ToString() + ", " + tbr.ToString();
                    }
                    else if (SO == 144) // mtcrf
                    {
                        p = 12;
                        UInt32 CRM = ReadBits(m[i], 8);
                        Output = "mtcrf";
                        CS = " " + CRM + ", r" + rD.ToString();
                    }
                    else if (SO == 146) // mtmsr
                    {
                        Output = "mtmsr";
                        CS = " r" + rD.ToString();
                    }
                    else if (SO == 467) // mtspr
                    {
                        p = 11;
                        UInt32 spr = ReadBits(m[i], 10);
                        Output = "mtspr";
                        CS = " r" + rD.ToString() + ", " + spr.ToString();
                    }
                    else if (SO == 210) // mtsr
                    {
                        p = 12;
                        UInt32 SR = ReadBits(m[i], 4);
                        Output = "mtsr";
                        CS = " r" + rD.ToString() + ", " + SR.ToString();
                    }
                    else if (SO == 242) // mtsrin
                    {
                        Output = "mtsrin";
                        CS = " r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 75) // mulhw
                    {
                        Output = "mulhw";
                    }
                    else if (SO == 11) // mulhwu
                    {
                        Output = "mulhwu";
                    }
                    else if (SO == 235) // mullw
                    {
                        Output = "mullw";
                    }
                    else if (SO == 476) // nand
                    {
                        Output = "nand";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 104) // neg
                    {
                        Output = "neg";
                        CS = " r" + rD.ToString() + ", r" + rA.ToString();
                    }
                    else if (SO == 124) // nor
                    {
                        Output = "nor";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 444) // or
                    {
                        Output = "or";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 412) // orc
                    {
                        Output = "orc";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 24) // slw, srw (For srw: actually 536, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "slw";
                        }
                        else
                        {
                            Output = "srw";
                        }
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 280) // sraw (Actually 792, but the previous bit (1) is from OE)
                    {
                        Output = "sraw";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 312) // srawi (Actually 824, but the previous bit (1) is from OE)
                    {
                        Output = "srawi";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 247) // stbux, stfdux (For stfdux: actually 759, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "stbux";
                        }
                        else
                        {
                            Output = "stfdux";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                    }
                    else if (SO == 215) // stbx, stfdx (For stfdx: actually 727, but the previous bit (1) is from OE)
                    {
                        if (OE == 0)
                        {
                            Output = "stbx";
                        }
                        else
                        {
                            Output = "stfdx";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                    }
                    else if (SO == 471) // stfiwx (Actually 983, but the previous bit (1) is from OE)
                    {
                        Output = "stfiwx";
                        CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 183) // stfsux, stwux (For stfsux: actually 695, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "stfsux";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "stwux";
                        }
                    }
                    else if (SO == 151) // stfsx, stwx (For stfsx: actually 663, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "stfsx";
                            CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                        }
                        else
                        {
                            Output = "stwx";
                        }
                    }
                    else if (SO == 406) // sthbrx (Actually 918, but the previous bit (1) is from OE)
                    {
                        Output = "sthbrx";
                    }
                    else if (SO == 439) // sthux
                    {
                        Output = "sthux";
                    }
                    else if (SO == 407) // sthx
                    {
                        Output = "sthx";
                    }
                    else if (SO == 213) // stswi (Actually 725, but the previous bit (1) is from OE)
                    {
                        Output = "stswi";
                        CS = " r" + rD.ToString() + ", r" + rA.ToString() + ", " + rB.ToString();
                    }
                    else if (SO == 149) // stswx (Actually 661, but the previous bit (1) is from OE)
                    {
                        Output = "stswx";
                    }
                    else if (SO == 150) // stwbrx, stwcx. (For stwbrx: actually 662, but the previous bit (1) is from OE)
                    {
                        if (OE == 1)
                        {
                            Output = "stwbrx";
                        }
                        else
                        {
                            Output = "stwcx";
                        }
                    }
                    else if (SO == 40) // subf
                    {
                        Output = "subf";
                    }
                    else if (SO == 8) // subfc
                    {
                        Output = "subfc";
                    }
                    else if (SO == 136) // subfe
                    {
                        Output = "subfeo";
                    }
                    else if (SO == 232) // subfme
                    {
                        Output = "subfme";
                        CS = " r" + rD.ToString() + ", r" + rA.ToString();
                    }
                    else if (SO == 200) // subfze
                    {
                        Output = "subfze";
                        CS = " r" + rD.ToString() + ", r" + rA.ToString();
                    }
                    else if (SO == 306) // tlbie
                    {
                        Output = "tlbie";
                        CS = " r" + rB.ToString();
                    }
                    else if (SO == 4) // tw
                    {
                        Output = "tw";
                        CS = " " + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 316) // xor
                    {
                        Output = "xor";
                        CS = " r" + rA.ToString() + ", r" + rD.ToString() + ", r" + rB.ToString();
                    }
                    else
                    {
                        NotInstruction();
                    }
                    if (OE == 1 && SO != 502 && SO != 342 && SO != 442 && SO != 410 && SO != 470 && SO != 119 && SO != 87
                        && SO != 55 && SO != 23 && SO != 278 && SO != 85 && SO != 21 && SO != 22 && SO != 0 && SO != 83
                        && SO != 659 && SO != 280 && SO != 312 && SO != 247 && SO != 215 && SO != 471 && SO != 183
                        && SO != 151 && SO != 406 && SO != 213 && SO != 149 && SO != 150 && SO != 86 && SO != 54) // o
                    {
                        Output += "o";
                    }
                    if (Rc == 1) // .
                    {
                        Output += ".";
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
                else if (PO == 19) // bcctr, bclr, crand, crandc, creqv, crnand, crnor, cror, crorc, crxor, isync, mcrf, rfi
                {
                    UInt32 BO = ReadBits(m[i], 5); // crbD on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor;
                    // Reserved on isync, rfi
                    UInt32 BI = ReadBits(m[i], 5); // crbA on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor;
                    // Reserved on isync, rfi
                    UInt32 crbB = ReadBits(m[i], 5); // Reserved on bcctr, bclr, isync, rfi
                    UInt32 SO = ReadBits(m[i], 10);
                    UInt32 LK = ReadBits(m[i], 1);
                    // Reserved on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor, isync, rfi
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
                    else if (SO == 150) // isync
                    {
                        Output = "isync";
                        CS = "";
                    }
                    else if (SO == 0) // mcrf
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        p += 2; // Reserved
                        UInt32 crfS = ReadBits(m[i], 3);
                        CS = " crf" + crfD.ToString() + ", crf" + crfS.ToString();
                        Output = "mcrf";
                    }
                    else if (SO == 50) // rfi
                    {
                        Output = "rfi";
                        CS = "";
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
                    Output = "cmpi crf" + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", " + SIMM;
                }
                else if (PO == 10) // cmpli
                {
                    UInt32 crfD = ReadBits(m[i], 3);
                    p++; // Reserved
                    UInt32 L = ReadBits(m[i], 1);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "cmpli crf" + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", " + UIMM;
                }
                else if (PO == 63)
                    // fabs (fabs, fabs.), fadd (fadd, fadd.), fcmpo, fcmpu, fctiw (fctiw, fctiw.), fctiwz (fctiwz, fctiwz.), fdiv (fdiv, fdiv.),
                    // fmadd (fmadd, fmadd.), fmr (fmr, fmr.), fmsub (fmsub, fmsub.), fmul (fmul, fmul.), fnabs (fnabs, fnabs.),
                    // fneg (fneg, fneg.), fnmadd (fnmadd, fnmadd.), fnmsub (fnmsub, fnmsub.), frsp (frsp, frsp.), frsqrte (frsqrte, frsqrte.),
                    // fsel (fsel, fsel.), fsub (fsub, fsub.), mcrfs, mffs (mffs, mffs.), mtfsb0 (mtfsb0, mtfsb0.), mftsb1 (mtfsb1, mtfsb1.),
                    // mtfsf (mtfsf, mtfsf.), mtfsfi (mtfsfi, mtfsfi.)
                {
                    UInt32 frD = ReadBits(m[i], 5); // crbD on mtfsb0, mtfsb1
                    UInt32 frA = ReadBits(m[i], 5);
                    // Reserved on fabs, fctiw, fctiwz, fmr, fnabs, fneg, frsp, frsqrte, mffs, mtfsb0, mtfsb1
                    UInt32 frB = ReadBits(m[i], 5); // Reserved on fmul, mffs, mtfsb0, mtfsb1
                    UInt32 SO = ReadBits(m[i], 10);
                    UInt32 Rc = ReadBits(m[i], 1); // Reserved on fcmpo, fcmpu
                    String CS = " fr" + frD.ToString() + ", fr" + frB.ToString();
                    if (SO == 264) // fabs
                    {
                        Output = "fabs";
                    }
                    else if (SO == 21) // fadd
                    {
                        Output = "fadd";
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 32 || SO == 0) // fcmpo, fcmpu
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        if (SO == 32)
                        {
                            Output = "fcmpo";
                        }
                        else
                        {
                            Output = "fcmpu";
                        }
                        CS = " crf" + crfD.ToString() + ", fr" + frA.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 14) // fctiw
                    {
                        Output = "fctiw";
                    }
                    else if (SO == 15) // fctiwz
                    {
                        Output = "fctiwz";
                    }
                    else if (SO == 18) // fdiv
                    {
                        Output = "fdiv";
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frB.ToString();
                    }
                    else if ((SO & 31) == 29 || (SO & 31) == 28 || (SO & 31) == 31 || (SO & 31) == 30 || (SO & 31) == 23)
                        // fmadd, fmsub, fnmadd, fnmsub, fsel
                    {
                        UInt32 frC = SO & 992;
                        if ((SO & 31) == 29) // fmadd
                        {
                            Output = "fmadd";
                        }
                        else if ((SO & 31) == 28) // fmsub
                        {
                            Output = "fmsub";
                        }
                        else if ((SO & 31) == 31) // fnmadd
                        {
                            Output = "fnmadd";
                        }
                        else if ((SO & 31) == 30) // fnmsub
                        {
                            Output = "fnmsub";
                        }
                        else // fsel
                        {
                            Output = "fsel";
                        }
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frC.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 72) // fmr
                    {
                        Output = "fmr";
                    }
                    else if ((SO & 31) == 25) // fmul
                    {
                        UInt32 frC = SO & 992;
                        Output = "fmul";
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frC.ToString();
                    }
                    else if (SO == 136) // fnabs
                    {
                        Output = "fnabs";
                    }
                    else if (SO == 40) // fneg
                    {
                        Output = "fneg";
                    }
                    else if (SO == 12) // frsp
                    {
                        Output = "frsp";
                    }
                    else if (SO == 26) // frsqrte
                    {
                        Output = "frsqrte";
                    }
                    else if (SO == 20) // fsub
                    {
                        Output = "fsub";
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 64) // mcrfs
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        p += 2; // Reserved
                        UInt32 crfS = ReadBits(m[i], 3);
                        CS = " crf" + crfD.ToString() + ", crf" + crfS.ToString();
                        Output = "mcrfs";
                    }
                    else if (SO == 583) // mffs
                    {
                        Output = "mffs";
                        CS = " fr" + frD.ToString();
                    }
                    else if (SO == 70) // mtfsb0
                    {
                        Output = "mtfsb0";
                        CS = " crb" + frD.ToString();
                    }
                    else if (SO == 38) // mtfsb1
                    {
                        Output = "mtfsb1";
                        CS = " crb" + frD.ToString();
                    }
                    else if (SO == 711) // mtfsf
                    {
                        p = 7;
                        UInt32 FM = ReadBits(m[i], 8);
                        Output = "mtfsf";
                        CS = " FM" + FM.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 134) // mtfsfi
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        p = 16;
                        UInt32 IMM = ReadBits(m[i], 4);
                        Output = "mtfsfi";
                        CS = " crf" + crfD.ToString() + ", " + IMM.ToString();
                    }
                    else
                    {
                        NotInstruction();
                    }
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 59)
                    // fadds (fadds, fadds.), fdivs (fdivs, fdivs.), fmadds (fmadds, fmadds.), fmsubs (fmsubs, fmsubs.), fmuls (fmuls, fmuls.),
                    // fnmadds (fnmadds, fnmadds.), fnmsubs (fnmsubs, fnmsubs.), fres (fres, fres.), fsubs (fsubs, fsubs.)
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 frA = ReadBits(m[i], 5); // Reserved on fres
                    UInt32 frB = ReadBits(m[i], 5); // Reserved on fmul
                    UInt32 SO = ReadBits(m[i], 10);
                    UInt32 Rc = ReadBits(m[i], 1);
                    String CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frB.ToString();
                    if (SO == 21) // fadds
                    {
                        Output = "fadds";
                    }
                    else if (SO == 18) // fdivs
                    {
                        Output = "fdivs";
                    }
                    else if ((SO & 31) == 29 || (SO & 31) == 28 || (SO & 31) == 31 || (SO & 31) == 30) // fmadds, fmsubs, fnmadds, fnmsubs
                    {
                        UInt32 frC = SO & 992;
                        if ((SO & 31) == 29)
                        {
                            Output = "fmadds";
                        }
                        else if ((SO & 31) == 28)
                        {
                            Output = "fmsubs";
                        }
                        else if ((SO & 31) == 31)
                        {
                            Output = "fnmadds";
                        }
                        else
                        {
                            Output = "fnmsubs";
                        }
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frC.ToString() + ", fr" + frB.ToString();
                    }
                    else if ((SO & 31) == 25) // fmuls
                    {
                        UInt32 frC = SO & 992;
                        Output = "fmuls";
                        CS = " fr" + frD.ToString() + ", fr" + frA.ToString() + ", fr" + frC.ToString();
                    }
                    else if (SO == 24) // fres
                    {
                        Output = "fres";
                        CS = " fr" + frD.ToString() + ", fr" + frB.ToString();
                    }
                    else if (SO == 21) // fsubs
                    {
                        Output = "fsubs";
                    }
                    else
                    {
                        NotInstruction();
                    }
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 34) // lbz
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lbz r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 35) // lbzu
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lbzu r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 50) // lfd
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lfd fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 51) // lfdu
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lfdu fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 48) // lfs
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lfs fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 49) // lfsu
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lfsu fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 42) // lha
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lha r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 43) // lhau
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lhau r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 40) // lhz
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lhz r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 41) // lhzu
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lhzu r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 46) // lmw
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lmw r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 32) // lwz
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lwz r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 33) // lwzu
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "lwzu r" + rD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 7) // mulli
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "mulli r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 24) // ori
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "ori r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else if (PO == 25) // oris
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "oris r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else if (PO == 56) // psq_l
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 W = ReadBits(m[i], 1);
                    UInt32 I = ReadBits(m[i], 3);
                    UInt32 d = ReadBits(m[i], 12);
                    Output = "psq_l fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + "), " + W.ToString() + ", " + I.ToString();
                }
                else if (PO == 57) // psq_lu
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 W = ReadBits(m[i], 1);
                    UInt32 I = ReadBits(m[i], 3);
                    UInt32 d = ReadBits(m[i], 12);
                    Output = "psq_lu fr" + frD.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + "), " + W.ToString() + ", " + I.ToString();
                }
                else if (PO == 4) // psq_lux, psq_lx, psq_stux, psq_stx, ps_abs (ps_abs, ps_abs.), ps_add (ps_add, ps_add.),
                    // ps_cmpo0, ps_cmpo1, ps_cmpu0, ps_cmpu1, ps_div (ps_div, ps_div.), ps_madd (ps_madd, ps_madd.),
                    // ps_madds0 (ps_madds0, ps_madds0.), ps_madds1 (ps_madds1, ps_madds1.), ps_merge00 (ps_merge00, ps_merge00.),
                    // ps_merge01 (ps_merge01, ps_merge01.), ps_merge10 (ps_merge10, ps_merge10.), ps_merge11 (ps_merge11, ps_merge11.),
                    // ps_mr (ps_mr, ps_mr.), ps_msub (ps_msub, ps_msub.), ps_mul (ps_mul, ps_mul.), ps_muls0 (ps_muls0, ps_muls0.),
                    // ps_muls1 (ps_muls1, ps_muls1.), ps_nabs (ps_nabs, ps_nabs.), ps_nmadd (ps_nmadd, ps_nmadd.),
                    // ps_nmsub (ps_nmsub, ps_nmsub.), ps_res (ps_res, ps_res.), ps_rsqrte (ps_rsqrte, ps_rsqrte.),
                    // ps_sel (ps_sel, ps_sel.), ps_sub (ps_sub, ps_sub.), ps_sum0 (ps_sum0, ps_sum0.), ps_sum1 (ps_sum1, ps_sum1.)
                {
                    UInt32 frD = ReadBits(m[i], 5); // frS for psq_stux, psq_stx
                    UInt32 rA = ReadBits(m[i], 5);
                    // Reserved on ps_abs, ps_mr, ps_nabs; frA on ps_add, ps_div, ps_merge00, ps_merge01, ps_merge10, ps_merge11,
                    // ps_res, ps_rsqrte, ps_sub
                    UInt32 rB = ReadBits(m[i], 5);
                    // frB on ps_abs, ps_add, ps_div, ps_merge00, ps_merge01, ps_merge10, ps_merge11, ps_mr, ps_res, ps_rsqrte,
                    // ps_sub
                    UInt32 W = ReadBits(m[i], 1);
                    UInt32 I = ReadBits(m[i], 3);
                    UInt32 SO = ReadBits(m[i], 6);
                    UInt32 Rc = ReadBits(m[i], 1); // Reserved on psq_lux, psq_lx, psq_stux, psq_stx
                    String CS = " fr" + frD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString() + ", " + W.ToString() + ", " + I.ToString();
                    if (SO == 38) // psq_lux
                    {
                        Output = "psq_lux";
                    }
                    else if (SO == 6) // psq_lx
                    {
                        Output = "psq_lx";
                    }
                    else if (SO == 39) // psq_stux
                    {
                        Output = "psq_stux";
                    }
                    else if (SO == 7) // psq_stx
                    {
                        Output = "psq_stx";
                    }
                    else if (SO == 264) // ps_abs
                    {
                        Output = "ps_abs";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 21) // ps_add
                    {
                        Output = "ps_add";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 32) // ps_cmpo0
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        Output = "ps_cmpo0";
                        CS = " crf" + crfD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 96) // ps_cmpo1
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        Output = "ps_cmpo1";
                        CS = " crf" + crfD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 0) // ps_cmpu0
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        Output = "ps_cmpu0";
                        CS = " crf" + crfD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 64) // ps_cmpu1
                    {
                        p = 6;
                        UInt32 crfD = ReadBits(m[i], 3);
                        Output = "ps_cmpu1";
                        CS = " crf" + crfD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 18) // ps_div
                    {
                        Output = "ps_div";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 29) // ps_madd
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_madd";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 14) // ps_madds0
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_madds0";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 15) // ps_madds1
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_madds1";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 528) // ps_merge00
                    {
                        Output = "ps_merge00";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 560) // ps_merge01
                    {
                        Output = "ps_merge01";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 592) // ps_merge10
                    {
                        Output = "ps_merge10";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 624) // ps_merge11
                    {
                        Output = "ps_merge11";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 72) // ps_mr
                    {
                        Output = "ps_mr";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 28) // ps_msub
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_msub";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 25) // ps_mul
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_mul";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString();
                    }
                    else if ((SO & 31) == 12) // ps_muls0
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_muls0";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString();
                    }
                    else if ((SO & 31) == 13) // ps_muls1
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_muls1";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString();
                    }
                    else if (SO == 136) // ps_nabs
                    {
                        Output = "ps_nabs";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 40) // ps_neg
                    {
                        Output = "ps_neg";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 31) // ps_nmadd
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_nmadd";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 30) // ps_nmsub
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_nmsub";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 24) // ps_res
                    {
                        Output = "ps_res";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 26) // ps_rsqrte
                    {
                        Output = "ps_rsqrte";
                        CS = " fr" + frD.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 23) // ps_sel
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_sel";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if (SO == 20) // ps_sub
                    {
                        Output = "ps_sub";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 10) // ps_sum0
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_sub0";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else if ((SO & 31) == 11) // ps_sum1
                    {
                        p = 21;
                        UInt32 frC = ReadBits(m[i], 5);
                        Output = "ps_sum1";
                        CS = " fr" + frD.ToString() + ", fr" + rA.ToString() + ", fr" + frC.ToString() + ", fr" + rB.ToString();
                    }
                    else
                    {
                        NotInstruction();
                    }
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 60) // psq_st
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 W = ReadBits(m[i], 1);
                    UInt32 I = ReadBits(m[i], 3);
                    UInt32 d = ReadBits(m[i], 12);
                    Output = "psq_st fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + "), " + W.ToString() + ", " + I.ToString();
                }
                else if (PO == 61) // psq_stu
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 W = ReadBits(m[i], 1);
                    UInt32 I = ReadBits(m[i], 3);
                    UInt32 d = ReadBits(m[i], 12);
                    Output = "psq_stu fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + "), " + W.ToString() + ", " + I.ToString();
                }
                else if (PO == 20) // rlwimi (rlwimi, rlwimi.)
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SH = ReadBits(m[i], 5);
                    UInt32 MB = ReadBits(m[i], 5);
                    UInt32 ME = ReadBits(m[i], 5);
                    UInt32 Rc = ReadBits(m[i], 1);
                    Output = "rlwimi";
                    String CS = " r" + rA.ToString() + ", r" + rS.ToString() + ", " + SH.ToString() + ", " + MB.ToString() + ", " + ME.ToString();
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 21) // rlwinm (rlwinm, rlwinm.)
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SH = ReadBits(m[i], 5);
                    UInt32 MB = ReadBits(m[i], 5);
                    UInt32 ME = ReadBits(m[i], 5);
                    UInt32 Rc = ReadBits(m[i], 1);
                    Output = "rlwinm";
                    String CS = " r" + rA.ToString() + ", r" + rS.ToString() + ", " + SH.ToString() + ", " + MB.ToString() + ", " + ME.ToString();
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 23) // rlwnm (rlwnm, rlwnm.)
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 rB = ReadBits(m[i], 5);
                    UInt32 MB = ReadBits(m[i], 5);
                    UInt32 ME = ReadBits(m[i], 5);
                    UInt32 Rc = ReadBits(m[i], 1);
                    Output = "rlwnm";
                    String CS = " r" + rA.ToString() + ", r" + rS.ToString() + ", " + rB.ToString() + ", " + MB.ToString() + ", " + ME.ToString();
                    if (Rc == 1) // .
                    {
                        Output += ".";
                    }
                    Output += CS;
                }
                else if (PO == 17) // sc
                {
                    p = 30;
                    UInt32 SO = ReadBits(m[i], 1);
                    if (SO == 1)
                    {
                        Output = "sc";
                    }
                    else
                    {
                        NotInstruction();
                    }
                }
                else if (PO == 38) // stb
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stb r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 39) // stbu
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stbu r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 54) // stfd
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stfd fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 55) // stfdu
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stfdu fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 52) // stfs
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stfs fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 53) // stfsu
                {
                    UInt32 frS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stfsu fr" + frS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 44) // sth
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "sth r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 45) // sthu
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "sthu r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 47) // stmw
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stmw r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 36) // stw
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stw r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 37) // stwu
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 d = ReadBits(m[i], 16);
                    Output = "stwu r" + rS.ToString() + ", " + d.ToString() + " (r" + rA.ToString() + ")";
                }
                else if (PO == 8) // subfic
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "subfic r" + rD.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 3) // twi
                {
                    UInt32 TO = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 SIMM = ReadBits(m[i], 16);
                    Output = "twi " + TO.ToString() + ", r" + rA.ToString() + ", " + SIMM.ToString();
                }
                else if (PO == 26) // xori
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "xori r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else if (PO == 27) // xoris
                {
                    UInt32 rS = ReadBits(m[i], 5);
                    UInt32 rA = ReadBits(m[i], 5);
                    UInt32 UIMM = ReadBits(m[i], 16);
                    Output = "xoris r" + rA.ToString() + ", r" + rS.ToString() + ", " + UIMM.ToString();
                }
                else
                {
                    NotInstruction();
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
