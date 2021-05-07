using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public enum AuroraType
    {
        TacticalMapForm, EconomicsForm, GameState
    }

    public class KnowledgeBase
    {
        private readonly Lib Lib;

        internal KnowledgeBase(Lib lib)
        {
            Lib = lib;
        }

        public IEnumerable<KeyValuePair<AuroraType, string>> GetKnownTypeNames()
        {
            if (Lib.AuroraChecksum == "chm1c7")
            {
                yield return new KeyValuePair<AuroraType, string>(AuroraType.TacticalMapForm, "jt");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.EconomicsForm, "gz");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.GameState, "aw");
            }
        }

        public object GetGameState(Form map)
        {
            switch (Lib.AuroraChecksum)
            {
                case "chm1c7": return map.GetType().GetField("a", AccessTools.all).GetValue(map);
                default: return null;
            }
        }

        public string GetSaveFunctions()
        {
            switch (Lib.AuroraChecksum)
            {
                case "chm1c7": return "g2,g3,g4,hd,he,hf,hk,hl,hm,i5,hn,ho,hp,hg,hq,hs,hr,ht,hu,h5,hw,hx,hy,hz,h0,h1,h2,h3,h4,h6,h7,hh,h8,h9,ia,ib,ic,id,ig,ih,ii,ij,ik,il,im,in,hv,io,ip,iq,ir,is,it,iy,iu,iv,iw,ix,iz,i0,i1,i2,i3,i4,i7,i8,i9,i6,hc,hi,hb,ha,g9,g8,g7,hj,ie,g5,g6";
                default: return null;
            }
        }
    }
}
