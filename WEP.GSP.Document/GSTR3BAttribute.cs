using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    public class GSTR3BAttribute
    {

        public string gstin { get; set; }

        public string ret_period { get; set; }

        public SupDetails sup_details { get; set; }

        public SupInter inter_sup { get; set; }

        public EligibleITC itc_elg { get; set; }

        public InwardSupply inward_sup { get; set; }

        public InterestLateFee intr_ltfee { get; set; }

    }

    public class SupDetails
    {
        public OutSupDetails osup_det { get; set; }

        public OSupZero osup_zero { get; set; }

        public OSupDetail osup_nil_exmp { get; set; }

        public OutSupDetails isup_rev { get; set; }

        public OSupDetail osup_nongst { get; set; }
    }

    public class OutSupDetails
    {
        public string txval { get; set; }
        public string iamt { get; set; }
        public string camt { get; set; }
        public string samt { get; set; }
        public string csamt { get; set; }
    }

    public class OSupZero
    {
        public string txval { get; set; }
        public string iamt { get; set; }
        public string csamt { get; set; }
    }

    public class OSupDetail
    {
        public string txval { get; set; }
    }

    public class SupInter
    {
        public List<DetailsInter> unreg_details { get; set; }

        public List<DetailsInter> comp_details { get; set; }

        public List<DetailsInter> uin_details { get; set; }
    }

    public class DetailsInter
    {
        public string pos { get; set; }
        public string txval { get; set; }
        public string iamt { get; set; }

    }

    public class EligibleITC
    {
        public List<DetailsITC> itc_avl { get; set; }

        public List<DetailsITC> itc_rev { get; set; }

        public List<NetDetailsITC> itc_net { get; set; }

        public List<NetDetailsITC> itc_inelg { get; set; }
    }

    public class DetailsITC
    {
        public string ty { get; set; }
        public string iamt { get; set; }
        public string camt { get; set; }
        public string samt { get; set; }
        public string csamt { get; set; }

    }
    public class NetDetailsITC
    {
        public string iamt { get; set; }
        public string camt { get; set; }
        public string samt { get; set; }
        public string csamt { get; set; }

    }

    public class InwardSupply
    {
        public List<InwardSupplyDetails> isup_details { get; set; }

    }

    public class InwardSupplyDetails
    {
        public string ty { get; set; }
        public string inter { get; set; }
        public string intra { get; set; }

    }

    public class InterestLateFee
    {
        public InterestPaid intr_details { get; set; }

    }

    public class InterestPaid
    {
        public string iamt { get; set; }
        public string camt { get; set; }
        public string samt { get; set; }
        public string csamt { get; set; }
    }
}
