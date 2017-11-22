using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    public class B2B_ItmDet
    {
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public string txval { get; set; }
        public string irt { get; set; }
        public string iamt { get; set; }
        public string crt { get; set; }
        public string camt { get; set; }
        public string srt { get; set; }
        public string samt { get; set; }
        public string csrt { get; set; }
        public string csamt { get; set; }
        public string elg { get; set; }
    }

    public class B2B_Itc
    {
        public string tx_i { get; set; }
        public string tx_s { get; set; }
        public string tx_c { get; set; }
        public string tx_cs { get; set; }
        public string tc_c { get; set; }
        public string tc_i { get; set; }
        public string tc_s { get; set; }
        public string tc_cs { get; set; }
    }
    public class B2B_Itm
    {
        public string num { get; set; }
        public B2B_ItmDet itm_det { get; set; }
        public B2B_Itc itc { get; set; }
    }

    public class B2B_Inv
    {
        public string flag { get; set; }
        public string chksum { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public string pos { get; set; }
        public string rchrg { get; set; }
        public string updby { get; set; }
        public List<B2B_Itm> itms { get; set; }
    }

    public class B2B
    {
        public string ctin { get; set; }
        public string cfs { get; set; }
        public List<B2B_Inv> inv { get; set; }
    }

    public class B2BJson
    {
        public List<B2B> b2b { get; set; }
    }

    public class ItmDet
    {
        public string rt { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
        public string camt { get; set; }
        public string samt { get; set; }
    }

    public class Itm
    {
        public string num { get; set; }
        public ItmDet itm_det { get; set; }
    }

    public class Inv
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public string pos { get; set; }
        public string rchrg { get; set; }
        public string etin { get; set; }
        public string inv_typ { get; set; }
        public List<Itm> itms { get; set; }
    }

    public class B2b
    {
        public string ctin { get; set; }
        public List<Inv> inv { get; set; }
    }

    public class ItmDet2
    {
        public string rt { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Itm2
    {
        public string num { get; set; }
        public ItmDet2 itm_det { get; set; }
    }

    public class Inv2
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public string etin { get; set; }
        public List<Itm2> itms { get; set; }
    }

    public class B2cl
    {
        public string pos { get; set; }
        public List<Inv2> inv { get; set; }
    }

    public class ItmDet3
    {
        public string rt { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Itm3
    {
        public string num { get; set; }
        public ItmDet3 itm_det { get; set; }
    }

    public class Nt
    {
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string p_gst { get; set; }
        public string rsn { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public List<Itm3> itms { get; set; }
    }

    public class Cdnr
    {
        public string ctin { get; set; }
        public List<Nt> nt { get; set; }
    }

    public class B2cs
    {
        public string sply_ty { get; set; }
        public string rt { get; set; }
        public string typ { get; set; }
        public string etin { get; set; }
        public string pos { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Itm4
    {
        public string txval { get; set; }
        public string rt { get; set; }
        public string iamt { get; set; }
    }

    public class Inv3
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public string sbpcode { get; set; }
        public string sbnum { get; set; }
        public string sbdt { get; set; }
        public List<Itm4> itms { get; set; }
    }

    public class Exp
    {
        public string exp_typ { get; set; }
        public List<Inv3> inv { get; set; }
    }

    public class Datum
    {
        public string num { get; set; }
        public string hsn_sc { get; set; }
        public string desc { get; set; }
        public string uqc { get; set; }
        public string qty { get; set; }
        public string val { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Hsn
    {
        public List<Datum> data { get; set; }
    }

    public class Inv4
    {
        public string sply_ty { get; set; }
        public string expt_amt { get; set; }
        public string nil_amt { get; set; }
        public string ngsup_amt { get; set; }
    }

    public class Nil
    {
        public List<Inv4> inv { get; set; }
    }

    public class Itm5
    {
        public string rt { get; set; }
        public string ad_amt { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Txpd
    {
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public List<Itm5> itms { get; set; }
    }

    public class Itm6
    {
        public string rt { get; set; }
        public string ad_amt { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class At
    {
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public List<Itm6> itms { get; set; }
    }

    public class Doc
    {
        public string num { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string totnum { get; set; }
        public string cancel { get; set; }
        public string net_issue { get; set; }
    }

    public class DocDet
    {
        public string doc_num { get; set; }
        public List<Doc> docs { get; set; }
    }

    public class DocIssue
    {
        public List<DocDet> doc_det { get; set; }
    }

    public class ItmDet4
    {
        public string rt { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class Itm7
    {
        public string num { get; set; }
        public ItmDet4 itm_det { get; set; }
    }

    public class Cdnur
    {
        public string typ { get; set; }
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string p_gst { get; set; }
        public string rsn { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string val { get; set; }
        public List<Itm7> itms { get; set; }
    }

    public class RootObject
    {
        public string gstin { get; set; }
        public string fp { get; set; }
        public string gt { get; set; }
        public string cur_gt { get; set; }
        public List<B2b> b2b { get; set; }
        public List<B2cl> b2cl { get; set; }
        public List<Cdnr> cdnr { get; set; }
        public List<B2cs> b2cs { get; set; }
        public List<Exp> exp { get; set; }
        public Hsn hsn { get; set; }
        public Nil nil { get; set; }
        public List<Txpd> txpd { get; set; }
        public List<At> at { get; set; }
        public DocIssue doc_issue { get; set; }
        public List<Cdnur> cdnur { get; set; }
    }

    //---------------------

    public class B2BBatch
    {
        public string gstin { get; set; }
        public string fp { get; set; }
        public string gt { get; set; }
        public string cur_gt { get; set; }
        public List<B2b> b2b { get; set; }
    }

    public class B2BTest
    {
        public B2BBatch array { get; set; }
    }

}
