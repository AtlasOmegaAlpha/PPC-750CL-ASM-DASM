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
                // extsb, extsh, icbi, lbzux, lbzx, lfdux, lfdx, lfsux, lfsx, lhaux, lhax, lhbrx, lzhux, lzhx
                {
                    UInt32 rD = ReadBits(m[i], 5);
                    // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eieio; rS on and, andc, cntlzw, ecowx, eqv, extsb, extsh, icbi;
                    // frD on lfdux, lfdx, lfsux, lfsx
                    UInt32 rA = ReadBits(m[i], 5); // Reserved on eieio
                    UInt32 rB = ReadBits(m[i], 5); // Reserved on addme, addze, cntlzw, eieio, extsb, extsh
                    UInt32 OE = ReadBits(m[i], 1);
                    UInt32 SO = ReadBits(m[i], 9);
                    UInt32 Rc = ReadBits(m[i], 1);
                    // Reserved on dcbf, dcbi, dcbst, dcbt, dcbtst, eciwx, ecowx, eieio, lbzux, lbzx, lfdux, lfsx, lhaux, lhax,
                    // lhbrx, lzhux, lzhx
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
                        CS = " " + crfD.ToString() + ", " + L.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
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
                    else if (SO == 55) // lfsux (Actually 567, but the previous bit (1) is from OE)
                    {
                        Output = "lfsux";
                        CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
                    }
                    else if (SO == 23) // lfsx (Actually 535, but the previous bit (1) is from OE)
                    {
                        Output = "lfsx";
                        CS = " fr" + rD.ToString() + ", r" + rA.ToString() + ", r" + rB.ToString();
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
                    else
                    {
                        NotInstruction();
                    }
                    if (OE == 1 && SO != 502 && SO != 342 && SO != 442 && SO != 410 && SO != 470 && SO != 119 && SO != 87
                        && SO != 55 && SO != 23 && SO != 278) // o
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
                else if (PO == 19) // bcctr, bclr, crand, crandc, creqv, crnand, crnor, cror, crorc, crxor, isync
                {
                    UInt32 BO = ReadBits(m[i], 5); // crbD on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor; reserved on isync
                    UInt32 BI = ReadBits(m[i], 5); // crbA on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor; reserved on isync
                    UInt32 crbB = ReadBits(m[i], 5); // Reserved on bcctr, bclr, isync
                    UInt32 SO = ReadBits(m[i], 10);
                    UInt32 LK = ReadBits(m[i], 1); // Reserved on crand, crandc, creqv, crnand, crnor, cror, crorc, crxor, isync
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
                else if (PO == 63)
                    // fabs (fabs, fabs.), fadd (fadd, fadd.), fcmpo, fcmpu, fctiw (fctiw, fctiw.), fctiwz (fctiwz, fctiwz.), fdiv (fdiv, fdiv.),
                    // fmadd (fmadd, fmadd.), fmr (fmr, fmr.), fmsub (fmsub, fmsub.), fmul (fmul, fmul.), fnabs (fnabs, fnabs.),
                    // fneg (fneg, fneg.), fnmadd (fnmadd, fnmadd.), fnmsub (fnmsub, fnmsub.), frsp (frsp, frsp.), frsqrte (frsqrte, frsqrte.),
                    // fsel (fsel, fsel.), fsub (fsub, fsub.)
                {
                    UInt32 frD = ReadBits(m[i], 5);
                    UInt32 frA = ReadBits(m[i], 5); // Reserved on fabs, fctiw, fctiwz, fmr, fnabs, fneg, frsp, frsqrte
                    UInt32 frB = ReadBits(m[i], 5); // Reserved on fmul
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
